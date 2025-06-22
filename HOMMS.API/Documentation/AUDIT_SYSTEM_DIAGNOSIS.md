# Audit System Diagnosis and Testing Guide

## Overview

The audit system is **fully implemented** in your codebase and should automatically populate audit fields for entities inheriting from `BaseAuditableEntity`. This guide helps you diagnose why audit fields might still appear as NULL.

## Implementation Status ✅

Your audit system includes:

1. **✅ Base Entity Structure**: `BaseAuditableEntity<T>` with audit fields
2. **✅ DbContext Integration**: `ApplyAuditInformation()` method in `ApplicationDbContext`
3. **✅ User Context**: `IHttpContextAccessor` for accessing current user
4. **✅ SaveChanges Override**: Both sync and async methods call audit logic
5. **✅ Debug Logging**: Comprehensive logging added for troubleshooting

## Audit Fields

The following fields are automatically populated:

### Creation Audit (EntityState.Added)

- `CreatedAt`: UTC timestamp when entity was created
- `CreatedBy`: User ID who created the entity
- `LastModifiedAt`: Set to NULL on creation
- `LastModifiedBy`: Set to NULL on creation

### Modification Audit (EntityState.Modified)

- `LastModifiedAt`: UTC timestamp when entity was last modified
- `LastModifiedBy`: User ID who last modified the entity
- `CreatedAt`: Preserved (not modified)
- `CreatedBy`: Preserved (not modified)

### Soft Delete Audit (ISoftDeletable)

- `IsDeleted`: Set to true
- `DeletedAt`: UTC timestamp when entity was deleted
- `DeletedBy`: User ID who deleted the entity

## Testing Steps

### Step 1: Check Debug Output

1. **Start the API** in Debug mode
2. **Open Debug Output Window** in Visual Studio (Debug → Windows → Output)
3. **Create a food category** using the test endpoint or normal API
4. **Look for debug messages** starting with `[AUDIT DEBUG]` and `[TEST]`

### Step 2: Use Test Endpoint

```http
POST /api/v1/FoodCategories/test-audit
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "name": "Test Category",
  "branchId": 1
}
```

This endpoint will:

- Create a test category with audit logging
- Show current user information
- Display debug output in console
- Return both created and retrieved category data

### Step 3: Check Database Directly

Query the database to verify audit fields:

```sql
SELECT TOP 10
    Id, Name,
    CreatedAt, CreatedBy,
    LastModifiedAt, LastModifiedBy,
    IsDeleted, DeletedAt, DeletedBy
FROM FoodCategories
ORDER BY Id DESC
```

### Step 4: Verify User Authentication

Check if the user context is properly available:

```csharp
// In any controller action
System.Diagnostics.Debug.WriteLine($"User ID: {User?.FindFirst(ClaimTypes.NameIdentifier)?.Value}");
System.Diagnostics.Debug.WriteLine($"User Name: {User?.Identity?.Name}");
System.Diagnostics.Debug.WriteLine($"Is Authenticated: {User?.Identity?.IsAuthenticated}");
```

## Common Issues and Solutions

### Issue 1: User ID is NULL

**Symptoms**: `CreatedBy`, `LastModifiedBy`, `DeletedBy` are always NULL
**Possible Causes**:

- User not authenticated
- JWT token missing correct claims
- `IHttpContextAccessor` not properly injected

**Debug Steps**:

1. Check debug output for `[AUDIT DEBUG] User not authenticated`
2. Verify JWT token contains `sub` or `NameIdentifier` claim
3. Test with Postman using valid JWT token

**Solution**: Ensure JWT token contains user ID claim:

```csharp
// In JWT generation, ensure you include:
new Claim(ClaimTypes.NameIdentifier, user.Id),
new Claim("sub", user.Id),
```

### Issue 2: ApplyAuditInformation Not Called

**Symptoms**: No `[AUDIT DEBUG]` messages in output
**Possible Causes**:

- Repository not using DbContext.SaveChanges()
- Direct SQL operations bypassing Entity Framework
- Custom save methods not calling base.SaveChanges()

**Debug Steps**:

1. Check if `[AUDIT DEBUG] ApplyAuditInformation called` appears
2. Verify repository calls `SaveChanges()` or `SaveChangesAsync()`
3. Check custom repository methods

**Solution**: Ensure all data operations use EF Core SaveChanges:

```csharp
// Correct usage in repository
await DbContext.SaveChangesAsync(); // This triggers audit

// Incorrect usage that bypasses audit
await DbContext.Database.ExecuteSqlRawAsync("INSERT INTO..."); // This doesn't trigger audit
```

### Issue 3: Entity Not Implementing IAuditableEntity

**Symptoms**: Specific entity types not getting audit information
**Possible Causes**:

- Entity doesn't inherit from `BaseAuditableEntity`
- Entity uses wrong base class

**Debug Steps**:

1. Check entity class inheritance
2. Look for `[AUDIT DEBUG] Found 0 auditable entities`

**Solution**: Ensure entity inherits correctly:

```csharp
// Correct
public class FoodCategory : BaseAuditableEntity<int>, IBranchEntity

// Incorrect
public class FoodCategory : BaseEntity<int> // Missing audit functionality
```

### Issue 4: Custom Repository Methods

**Symptoms**: Some operations don't trigger audit, others do
**Possible Causes**:

- Custom methods like `AddAndSaveAsync()` bypassing audit
- Direct DbSet operations

**Solution**: Review custom repository methods:

```csharp
// Current implementation in your codebase (CORRECT)
public async Task AddAndSaveAsync(FoodCategory category)
{
    await DbSet.AddAsync(category);
    await DbContext.SaveChangesAsync(); // This triggers audit ✅
}

// Problematic implementation (AVOID)
public async Task AddAndSaveAsync(FoodCategory category)
{
    // Direct SQL execution bypasses audit ❌
    await DbContext.Database.ExecuteSqlRawAsync("INSERT INTO...");
}
```

## Verification Checklist

- [ ] **Debug messages appear** when creating/updating entities
- [ ] **User authentication working** (JWT token valid)
- [ ] **User ID claims present** in JWT token
- [ ] **Repository methods use SaveChanges()**
- [ ] **Entities inherit from BaseAuditableEntity**
- [ ] **Database schema includes audit columns**
- [ ] **IHttpContextAccessor registered** in DI container

## Entity Mapping Issues

Ensure AutoMapper profiles don't interfere with audit fields:

```csharp
// In DomainToDtoProfile.cs - Make sure audit fields are mapped
CreateMap<FoodCategory, FoodCategoryDto>()
    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
    .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
    .ForMember(dest => dest.LastModifiedAt, opt => opt.MapFrom(src => src.LastModifiedAt))
    .ForMember(dest => dest.LastModifiedBy, opt => opt.MapFrom(src => src.LastModifiedBy));
```

## Next Steps

1. **Run the test endpoint** with valid authentication
2. **Check debug output** for audit messages
3. **Verify database** contains audit data
4. **Report findings** with debug output and database query results

If audit fields are still NULL after following these steps, please share:

- Debug output from test endpoint
- Database query results
- JWT token claims (redacted sensitive info)
- Any error messages in API logs

## Removing Debug Code

After verification, remove debug logging from production:

1. Remove `System.Diagnostics.Debug.WriteLine` statements
2. Remove test endpoint from controller
3. Keep the audit functionality intact

The audit system is robust and should work correctly once the underlying issue is identified and resolved.
