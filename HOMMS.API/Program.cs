using Asp.Versioning;
using Asp.Versioning.Conventions;
using HOMMS.API.Middleware;
using HOMMS.Application.Implementations;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Extensions;
using HOMMS.Infrastructure.Repositories.Implementations;
using HOMMS.Infrastructure.Repositories.Interfaces;
using HOMMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Serilog;

using Microsoft.AspNetCore.Authorization;
using HOMMS.Application.Implementations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Asp.Versioning;
using Asp.Versioning.Conventions;


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

// Add CORS policy for development and production
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCorsPolicy", policy =>
    {
        policy.WithOrigins("*")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
    
    //options.AddPolicy("ProdCorsPolicy", policy =>
    //{
    //    policy.WithOrigins(
    //        "https://homms.cuahangkinhdoanh.com",
    //        "http://localhost:3000",
    //        "http://localhost:3001"
    //    )
    //    .AllowAnyHeader()
    //    .AllowAnyMethod()
    //    .AllowCredentials(); // Allow credentials for authenticated requests
    //});
});

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
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"),
        new MediaTypeApiVersionReader("x-api-version")
    );
}).AddMvc(options =>
{
    options.Conventions.Add(new VersionByNamespaceConvention());
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // Correct format for API Explorer
    options.SubstituteApiVersionInUrl = true; // Substitute version in URL
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
builder.Services.AddScoped<IMenuDetailRepository, MenuDetailRepository>();
builder.Services.AddScoped<IRevenueRepository, RevenueRepository>();
builder.Services.AddScoped<ISystemLogRepository, SystemLogRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IBranchUserRoleRepository, BranchUserRoleRepository>();
builder.Services.AddScoped<IDiseaseCategoryFoodRestrictionRepository, DiseaseCategoryFoodRestrictionRepository>();
 
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
 

// Add Disease Category Repository
builder.Services.AddScoped<IDiseaseCategoryRepository, DiseaseCategoryRepository>();

// Register generic repository for all entities
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

// Register custom permission authorization handler
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

// Register permission policies (add more as needed)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Permission:orders:view", policy =>
       policy.Requirements.Add(new PermissionRequirement("orders:view")));
    options.AddPolicy("Permission:orders:add", policy =>
        policy.Requirements.Add(new PermissionRequirement("orders:add")));
    options.AddPolicy("Permission:orders:edit", policy =>
        policy.Requirements.Add(new PermissionRequirement("orders:edit")));
    options.AddPolicy("Permission:orders:delete", policy =>
        policy.Requirements.Add(new PermissionRequirement("orders:delete")));
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
    // Areas
    options.AddPolicy("Permission:areas:view", policy =>
        policy.Requirements.Add(new PermissionRequirement("areas:view")));
    options.AddPolicy("Permission:areas:add", policy =>
        policy.Requirements.Add(new PermissionRequirement("areas:add")));
    options.AddPolicy("Permission:areas:edit", policy =>
        policy.Requirements.Add(new PermissionRequirement("areas:edit")));
    options.AddPolicy("Permission:areas:delete", policy =>
        policy.Requirements.Add(new PermissionRequirement("areas:delete")));
    // Locations
    options.AddPolicy("Permission:locations:view", policy =>
        policy.Requirements.Add(new PermissionRequirement("locations:view")));
    options.AddPolicy("Permission:locations:add", policy =>
        policy.Requirements.Add(new PermissionRequirement("locations:add")));
    options.AddPolicy("Permission:locations:edit", policy =>
        policy.Requirements.Add(new PermissionRequirement("locations:edit")));
    options.AddPolicy("Permission:locations:delete", policy =>
        policy.Requirements.Add(new PermissionRequirement("locations:delete")));
    // Add more policies for other permissions as needed
    //SystemLog
    options.AddPolicy("Permission:systemlog:view", policy =>
        policy.Requirements.Add(new PermissionRequirement("systemlog:view")));
    options.AddPolicy("Permission:systemlog:add", policy =>
       policy.Requirements.Add(new PermissionRequirement("systemlog:add")));
    //Patient
    options.AddPolicy("Permission:Patient:view", policy =>
      policy.Requirements.Add(new PermissionRequirement("Patient:view")));
    options.AddPolicy("Permission:Patient:add", policy =>
     policy.Requirements.Add(new PermissionRequirement("Patient:add")));
    options.AddPolicy("Permission:Patient:edit", policy =>
        policy.Requirements.Add(new PermissionRequirement("Patient:edit")));
    options.AddPolicy("Permission:Patient:delete", policy =>
        policy.Requirements.Add(new PermissionRequirement("Patient:delete")));

    // Disease Categories
    options.AddPolicy("Permission:diseasecategories:view", policy =>
        policy.Requirements.Add(new PermissionRequirement("diseasecategories:view")));
    options.AddPolicy("Permission:diseasecategories:add", policy =>
        policy.Requirements.Add(new PermissionRequirement("diseasecategories:add")));
    options.AddPolicy("Permission:diseasecategories:edit", policy =>
        policy.Requirements.Add(new PermissionRequirement("diseasecategories:edit")));
    options.AddPolicy("Permission:diseasecategories:delete", policy =>
        policy.Requirements.Add(new PermissionRequirement("diseasecategories:delete")));
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
builder.Services.AddScoped<IMenuDetailService,MenuDetailService>();
builder.Services.AddScoped<IRevenueService, RevenueService>();

builder.Services.AddScoped<ISystemLogService, SystemLogService>();
builder.Services.AddScoped<IPatientService, PatientService>();


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IEmailVerifyService, EmailVerifyService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IBranchUserRoleService,BranchUserRoleService>();
// Add Disease Category Service
builder.Services.AddScoped<IDiseaseCategoryService, DiseaseCategoryService>();
builder.Services.AddScoped<IDiseaseCategoryFoodRestrictionService, DiseaseCategoryFoodRestrictionService>();

builder.Services.AddScoped<IVnPayService, VnPayService>();

 
builder.Services.AddScoped<ICommentService, CommentService>();
 

//builder.Services.AddScoped<IImageService, ImageService>();


// Disease Category and Patient Dietary Services
// TODO: Uncomment when service implementations are created
// builder.Services.AddScoped<IDiseaseCategoryService, DiseaseCategoryService>();
// builder.Services.AddScoped<IPatientDietaryService, PatientDietaryService>();    
// builder.Services.AddScoped<IDietaryValidationService, DietaryValidationService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Configure multiple Swagger documents for different API versions
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "HOMMS API", Version = "v1" });
    options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "HOMMS API", Version = "v2" });
    
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






// Configure Google Login//
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.CallbackPath = "/google-response";
});






var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(options =>
//    {
//        // Configure Swagger UI for multiple API versions
//        options.SwaggerEndpoint("/swagger/v1/swagger.json", "HOMMS API v1");
//        options.SwaggerEndpoint("/swagger/v2/swagger.json", "HOMMS API v2");
//    });
//    // Use CORS policy in development
//    app.UseCors("DevCorsPolicy");
//}
//else
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(options =>
//    {
//        // Configure Swagger UI for multiple API versions
//        options.SwaggerEndpoint("/swagger/v1/swagger.json", "HOMMS API v1");
//        options.SwaggerEndpoint("/swagger/v2/swagger.json", "HOMMS API v2");
//    });
//    // Use CORS policy in production
//    app.UseCors("ProdCorsPolicy");
//}

{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Configure Swagger UI for multiple API versions
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "HOMMS API v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "HOMMS API v2");
    });
    // Use CORS policy in development
    app.UseCors("DevCorsPolicy");
}


app.UseHttpsRedirection();

// Configure static files to serve uploaded images
app.UseStaticFiles();

// Configure static files for uploads folder specifically with CORS support
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(
//        Path.Combine(builder.Environment.WebRootPath, "uploads")),
//    RequestPath = "/uploads",
//    OnPrepareResponse = context =>
//    {
//        // Add CORS headers for all uploaded files
//        // Allow specific origins in production, wildcard in development
//        var origin = context.Context.Request.Headers["Origin"].FirstOrDefault();
//        var allowedOrigins = new[] { 
//            "https://homms.cuahangkinhdoanh.com", 
//            "http://localhost:3000", 
//            "http://localhost:3001" 
//        };
        
//        if (app.Environment.IsDevelopment() || allowedOrigins.Contains(origin))
//        {
//            context.Context.Response.Headers.Add("Access-Control-Allow-Origin", 
//                app.Environment.IsDevelopment() ? "*" : origin);
//        }
        
//        context.Context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, HEAD, OPTIONS");
//        context.Context.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, Content-Type, Accept, Authorization");
//        context.Context.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Length, Content-Type");
        
//        // Cache uploaded images for better performance
//        context.Context.Response.Headers.Add("Cache-Control", "public, max-age=3600");
        
//        // Add Vary header for proper caching with CORS
//        context.Context.Response.Headers.Add("Vary", "Origin");
//    }
//});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "uploads")),
    RequestPath = "/uploads",
    OnPrepareResponse = context =>
    {
        // Add CORS headers for all uploaded files
        context.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        context.Context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, HEAD, OPTIONS");
        context.Context.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, Content-Type, Accept, Authorization");

        // Cache uploaded images for better performance
        context.Context.Response.Headers.Add("Cache-Control", "public, max-age=3600");

        context.Context.Response.Headers.Add("Vary", "Origin");
    }
});

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
    // Uncomment to seed the database with initial data
    //await app.SeedDatabaseAsync();

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
    private readonly IAuthService _authService;
    private readonly IBranchContext _branchContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PermissionHandler(IAuthService authService, IBranchContext branchContext, IHttpContextAccessor httpContextAccessor)
    {
        _authService = authService;
        _branchContext = branchContext;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null || !context.User.Identity?.IsAuthenticated == true)
            return;

        var userId = context.User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return;

        // SystemAdmin has access to all permissions across all branches
        if (context.User.IsInRole("SystemAdmin"))
        {
            context.Succeed(requirement);
            return;
        }

        // Get current branchId from branch context
        int branchId = _branchContext.GetCurrentBranchId();

        // Get permissions for this user and branch
        var permissions = await _authService.GetUserBranchPermissionsAsync(userId, branchId);
        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
