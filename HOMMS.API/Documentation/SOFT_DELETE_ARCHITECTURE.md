# Soft Delete Architecture Documentation

## Overview

The HOMMS system implements a **centralized soft delete architecture** using Entity Framework Core interceptors in `ApplicationDbContext`. This ensures consistent behavior and complete audit trails for all delete operations.

## Architecture Design

### **Central Implementation: ApplicationDbContext**

All soft delete logic is implemented in `ApplicationDbContext.SaveChanges()` and `SaveChangesAsync()`:

```csharp
case EntityState.Deleted:
    if (entry.Entity is ISoftDeletable hardDeletedEntity)
    {
        // Convert hard delete to soft delete
        entry.State = EntityState.Modified;
        hardDeletedEntity.IsDeleted = true;
        hardDeletedEntity.DeletedAt = currentTime;
        hardDeletedEntity.DeletedBy = currentUserId;
    }
    break;
```

### **Benefits of This Approach**

#### **1. Universal Coverage** ✅

- **All delete operations** are intercepted, regardless of how they're initiated
- Works with Repository, Service, or direct DbContext operations
- No way to accidentally bypass soft delete

#### **2. Complete Audit Trail** ✅

- Automatically sets `IsDeleted = true`
- Records `DeletedAt` with current UTC timestamp
- Records `DeletedBy` with current authenticated user ID
- Consistent with other audit operations (Create/Update)

#### **3. Fail-Safe Design** ✅

- Even incorrect usage like `DbSet.Remove()` is handled properly
- Developers cannot accidentally perform hard deletes on soft deletable entities

#### **4. Centralized Logic** ✅

- Single source of truth for soft delete behavior
- Easy to maintain and modify
- No code duplication across repositories

## Entity Setup

### **Implementing ISoftDeletable**

Entities that support soft delete must implement `ISoftDeletable`:

```csharp
public class FoodCategory : BaseAuditableEntity<int>
{
    // BaseAuditableEntity already implements ISoftDeletable
    // ISoftDeletable provides: IsDeleted, DeletedAt, DeletedBy
}
```

### **Base Entity Hierarchy**

```
IEntity<T> (Id property)
    ↓
IAuditableEntity (CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
    ↓
ISoftDeletable (IsDeleted, DeletedAt, DeletedBy)
    ↓
BaseAuditableEntity<T> (combines all above)
    ↓
Your Entity Classes (FoodCategory, Food, etc.)
```

## Usage Examples

### **Repository Delete Operations**

```csharp
// Repository method - simplified approach
public async Task<bool> DeleteAsync(TEntity entity)
{
    // Mark for deletion - ApplicationDbContext handles the rest
    DbSet.Remove(entity);

    var result = await DbContext.SaveChangesAsync();
    return result > 0;
}
```

### **Service Delete Operations**

```csharp
// Service method
public async Task<bool> DeleteFoodCategoryAsync(int categoryId)
{
    var category = await _repository.GetByIdAsync(categoryId);
    if (category == null) return false;

    // This will trigger soft delete automatically
    return await _repository.DeleteAsync(category);
}
```

### **Direct DbContext Operations**

```csharp
// Even direct operations are safe
var category = await context.FoodCategories.FindAsync(id);
context.FoodCategories.Remove(category); // Converted to soft delete
await context.SaveChangesAsync();
```

## Query Filtering

### **Automatic Filtering**

The system automatically excludes soft deleted entities from queries:

```csharp
// This query automatically excludes IsDeleted = true entities
var activeCategories = await context.FoodCategories.ToListAsync();
```

### **Global Query Filter Implementation**

```csharp
private void ApplySoftDeleteFilter<TEntity>(dynamic builder)
    where TEntity : class, ISoftDeletable
{
    builder.HasQueryFilter((Expression<Func<TEntity, bool>>)(e => !e.IsDeleted));
}
```

### **Including Deleted Entities (When Needed)**

```csharp
// To include soft deleted entities, use IgnoreQueryFilters()
var allCategories = await context.FoodCategories
    .IgnoreQueryFilters()
    .ToListAsync();

// Or filter specifically
var deletedCategories = await context.FoodCategories
    .IgnoreQueryFilters()
    .Where(c => c.IsDeleted)
    .ToListAsync();
```

## Best Practices

### **✅ DO:**

1. **Use Repository Methods**: Use `repository.DeleteAsync()` for consistency
2. **Trust the System**: Don't manually set `IsDeleted = true` in business logic
3. **Use Global Filters**: Rely on automatic filtering for normal queries
4. **Check Audit Fields**: Use `DeletedAt` and `DeletedBy` for audit purposes

### **❌ DON'T:**

1. **Manual Soft Delete**: Don't manually set `IsDeleted = true` in services
2. **Bypass Repository**: Avoid direct `DbSet.Remove()` unless necessary
3. **Ignore Filters**: Don't use `IgnoreQueryFilters()` unless specifically needed
4. **Mix Approaches**: Don't implement soft delete logic in multiple places

## Debugging and Monitoring

### **Debug Logging**

The system includes comprehensive debug logging:

```csharp
System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] Converting hard delete to soft delete for {entry.Entity.GetType().Name}");
```

### **Audit Trail Verification**

Check that audit fields are populated:

```sql
-- Verify soft delete audit trail
SELECT Id, Name, IsDeleted, DeletedAt, DeletedBy, CreatedAt, CreatedBy
FROM FoodCategories
WHERE IsDeleted = 1;
```

### **Testing Soft Delete**

```csharp
// Test soft delete behavior
[Test]
public async Task DeleteAsync_SoftDeletableEntity_SetsAuditFields()
{
    // Arrange
    var category = new FoodCategory { Name = "Test" };
    await repository.AddAsync(category);

    // Act
    await repository.DeleteAsync(category);

    // Assert
    var deleted = await context.FoodCategories
        .IgnoreQueryFilters()
        .FirstAsync(c => c.Id == category.Id);

    Assert.IsTrue(deleted.IsDeleted);
    Assert.IsNotNull(deleted.DeletedAt);
    Assert.IsNotNull(deleted.DeletedBy);
}
```

## Migration from Manual Soft Delete

If you had manual soft delete implementations:

### **Before (Manual)**

```csharp
// Old approach - manual soft delete in repository
if (entity is ISoftDeletable softDeletable)
{
    softDeletable.IsDeleted = true;
    DbContext.Entry(entity).State = EntityState.Modified;
}
```

### **After (Automatic)**

```csharp
// New approach - let ApplicationDbContext handle it
DbSet.Remove(entity);
// ApplicationDbContext automatically converts to soft delete with audit trail
```

## Conclusion

The centralized soft delete architecture in `ApplicationDbContext` provides:

- **Reliability**: No way to accidentally perform hard deletes
- **Consistency**: Same behavior across all delete operations
- **Audit Compliance**: Complete audit trail with timestamps and user tracking
- **Simplicity**: Repositories and services don't need soft delete logic
- **Maintainability**: Single place to modify soft delete behavior

This approach follows Entity Framework Core best practices and ensures data integrity across your entire application.
