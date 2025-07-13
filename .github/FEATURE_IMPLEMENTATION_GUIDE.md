# Feature Implementation Guide for HOMMS Backend

## 📋 Pre-Implementation Checklist

### 1. Feature Analysis
- [ ] Identify the business requirements
- [ ] Determine affected entities and relationships
- [ ] Define required permissions and roles
- [ ] Plan API endpoints and DTOs
- [ ] Consider multi-tenant/branch implications

### 2. Database Changes
- [ ] Create/modify entities in `HOMMS.Domain/Entities/`
- [ ] Add entity configurations in `HOMMS.Infrastructure/Configurations/`
- [ ] Update `ApplicationDbContext.cs` with new DbSets
- [ ] Create and run migrations
- [ ] Update seed data if needed

### 3. Repository Layer
- [ ] Create interface in `Infrastructure/Repositories/Interfaces/I[Entity]Repository.cs`
- [ ] Implement repository in `Infrastructure/Repositories/Implementations/[Entity]Repository.cs`
- [ ] Register repository in `Program.cs` DI container
- [ ] Follow existing patterns for branch filtering and soft delete

### 4. Service Layer
- [ ] Create interface in `Application/Interfaces/I[Feature]Service.cs`
- [ ] Implement service in `Application/Implementations/[Feature]Service.cs`
- [ ] Register service in `Program.cs` DI container
- [ ] Include proper error handling and logging

### 5. API Layer
- [ ] Create controller in `HOMMS.API/Controllers/[Feature]Controller.cs`
- [ ] Apply proper authorization policies
- [ ] Use standardized `ApiResponseBase<T>` responses
- [ ] Add API versioning attributes
- [ ] Include proper validation and error handling

### 6. DTOs and Mapping
- [ ] Create request/response DTOs in `Domain/Dtos/`
- [ ] Implement mapping between entities and DTOs
- [ ] Ensure no entity exposure in API responses

### 7. Security & Authorization
- [ ] Define required permissions in database
- [ ] Update role seeding with new permissions
- [ ] Apply `[Authorize(Policy = "...")]` attributes
- [ ] Test branch-specific access control

### 8. Testing & Documentation
- [ ] Test all endpoints with different user roles
- [ ] Update Swagger documentation
- [ ] Test multi-tenant scenarios
- [ ] Verify error handling and logging

## 🔄 Standard Implementation Pattern

### Repository Pattern
```csharp
// Interface
public interface IExampleRepository : IGenericRepository<Example>
{
    Task<Example?> GetByCustomFieldAsync(string field);
    Task<IEnumerable<Example>> GetActiveExamplesAsync();
}

// Implementation
public class ExampleRepository : GenericRepository<Example>, IExampleRepository
{
    public ExampleRepository(ApplicationDbContext context) : base(context) { }
    
    public async Task<Example?> GetByCustomFieldAsync(string field)
    {
        return await _context.Examples
            .FirstOrDefaultAsync(e => e.CustomField == field);
    }
}
```

### Service Pattern
```csharp
// Interface
public interface IExampleService
{
    Task<ApiResponseBase<ExampleDto>> GetExampleAsync(int id);
    Task<ApiResponseBase<ExampleDto>> CreateExampleAsync(CreateExampleDto dto);
}

// Implementation
public class ExampleService : IExampleService
{
    private readonly IExampleRepository _repository;
    private readonly ILogger<ExampleService> _logger;
    
    public ExampleService(IExampleRepository repository, ILogger<ExampleService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<ApiResponseBase<ExampleDto>> GetExampleAsync(int id)
    {
        try
        {
            var example = await _repository.GetByIdAsync(id);
            if (example == null)
                return ApiResponseBase<ExampleDto>.NotFound("Example not found");
                
            var dto = MapToDto(example);
            return ApiResponseBase<ExampleDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting example with id {Id}", id);
            return ApiResponseBase<ExampleDto>.Failure("Internal server error");
        }
    }
}
```

### Controller Pattern
```csharp
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class ExampleController : ControllerBase
{
    private readonly IExampleService _service;
    
    public ExampleController(IExampleService service)
    {
        _service = service;
    }
    
    [HttpGet("{id}")]
    [Authorize(Policy = "examples:read")]
    public async Task<ActionResult<ApiResponseBase<ExampleDto>>> GetExample(int id)
    {
        var result = await _service.GetExampleAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}
```

## 🚀 Migration Commands

```bash
# Create new migration
dotnet ef migrations add [MigrationName] --project HOMMS.Infrastructure --startup-project HOMMS.API

# Update database
dotnet ef database update --project HOMMS.Infrastructure --startup-project HOMMS.API

# Remove last migration
dotnet ef migrations remove --project HOMMS.Infrastructure --startup-project HOMMS.API
```

## 🔐 Permission System

### Adding New Permissions
1. Update `BranchRole` seeding in `HOMMS.Infrastructure/Seeds/`
2. Add permission strings following pattern: `"[entity]:[action]"`
3. Examples: `"orders:read"`, `"foods:create"`, `"reports:view"`

### Branch Context Usage
```csharp
// In services, use IBranchContext to get current branch
private readonly IBranchContext _branchContext;

public async Task<Example> GetBranchSpecificData()
{
    var branchId = _branchContext.BranchId;
    return await _repository.GetByBranchAsync(branchId);
}
```

## 📝 Coding Standards

- Follow **SOLID principles**
- Use **async/await** for all database operations
- Implement **proper error handling** with try-catch
- Use **dependency injection** for all services
- Return **ApiResponseBase<T>** from all API endpoints
- Apply **proper authorization** to all endpoints
- Include **comprehensive logging**
- Follow **existing naming conventions**

## 🧪 Testing Approach

- Test with different user roles and permissions
- Verify branch-specific data access
- Test error scenarios and edge cases
- Validate API response formats
- Check authorization policies