using HOMMS.API.Middleware;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Extensions;
using HOMMS.Infrastructure.Repositories.Implementations;
using HOMMS.Infrastructure.Repositories.Interfaces;
using HOMMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using HOMMS.Application.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddCustomServices();

// Add CORS policy for development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("DevCorsPolicy", policy =>
        {
            policy.WithOrigins(
                "http://localhost:3000", // React default
                "http://localhost:4200", // Angular default
                "http://localhost:5173"  // Vite default
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
    });
}

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("HOMMS.Infrastructure")
    ));

// Add Identity services
builder.Services.AddIdentityServices(builder.Configuration);

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Register branch context for multi-tenancy
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IBranchContext, BranchContext>();

// Register repositories
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IFoodRepository, FoodRepository>();
builder.Services.AddScoped<IFoodCategoryRepository, FoodCategoryRepository>();
builder.Services.AddScoped<IOrderDetailsRepository, OrderDetailsRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IRevenueRepository, RevenueRepository>();

// Register generic repository for all entities
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

// Register custom permission authorization handler
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

// Register permission policies (add more as needed)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Permission:orders:view", policy =>
       policy.Requirements.Add(new PermissionRequirement("orders:view")));
    options.AddPolicy("Permission:orders:add", policy =>
        policy.Requirements.Add(new PermissionRequirement("orders:add")));
    options.AddPolicy("Permission:orders:edit", policy =>
        policy.Requirements.Add(new PermissionRequirement("orders:edit")));
    // Foods
    options.AddPolicy("Permission:foods:view", policy =>
        policy.Requirements.Add(new PermissionRequirement("foods:view")));
    options.AddPolicy("Permission:foods:add", policy =>
        policy.Requirements.Add(new PermissionRequirement("foods:add")));
    options.AddPolicy("Permission:foods:edit", policy =>
        policy.Requirements.Add(new PermissionRequirement("foods:edit")));
    options.AddPolicy("Permission:foods:delete", policy =>
        policy.Requirements.Add(new PermissionRequirement("foods:delete")));
    // FoodCategories
    options.AddPolicy("Permission:foodcategories:view", policy =>
        policy.Requirements.Add(new PermissionRequirement("foodcategories:view")));
    options.AddPolicy("Permission:foodcategories:add", policy =>
        policy.Requirements.Add(new PermissionRequirement("foodcategories:add")));
    options.AddPolicy("Permission:foodcategories:edit", policy =>
        policy.Requirements.Add(new PermissionRequirement("foodcategories:edit")));
    options.AddPolicy("Permission:foodcategories:delete", policy =>
        policy.Requirements.Add(new PermissionRequirement("foodcategories:delete")));
    // Add more policies for other permissions as needed
});

// Register PrintUrlsHostedService
builder.Services.AddHostedService<HOMMS.API.PrintUrlsHostedService>();

// Register IUnitOfWork, UnitOfWork, IBranchService, and BranchService
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IFoodService, FoodService>();
builder.Services.AddScoped<IFoodCategoryService, FoodCategoryService>();
builder.Services.AddScoped<IPublicMenuService, PublicMenuService>();
builder.Services.AddScoped<IRevenueService, RevenueService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "HOMMS API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // Use CORS policy in development
    app.UseCors("DevCorsPolicy");
}

app.UseHttpsRedirection();

// Add Serilog request logging
app.UseSerilogRequestLogging();

// Add global exception middleware (should be early in the pipeline)
app.UseMiddleware<GlobalExceptionMiddleware>();

// Add branch context middleware (before authentication/authorization)
app.UseMiddleware<BranchContextMiddleware>();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting web host");

    // Seed the database
    await app.SeedDatabaseAsync();

    // Print listening URLs to the terminal and log with Serilog
    var addresses = app.Urls;
    foreach (var address in addresses)
    {
        Console.WriteLine($"Now listening on: {address}");
        Log.Information("Now listening on serilog: {Address}", address);
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// PermissionRequirement and PermissionHandler definitions
public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }
    public PermissionRequirement(string permission) => Permission = permission;
}

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.HasClaim("permission", requirement.Permission))
            context.Succeed(requirement);
        return Task.CompletedTask;
    }
}