using HOMMS.Infrastructure.Seeds;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Extensions;
namespace HOMMS.API.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemAdminSeedController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public SystemAdminSeedController(
            IServiceProvider serviceProvider,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _serviceProvider = serviceProvider;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("foods")]
        public async Task<IActionResult> SeedFoods()
        {
            await FoodSeedData.SeedFoodsAsync(_serviceProvider);
            return Ok("Foods seeded.");
        }

        [HttpPost("areas")]
        public async Task<IActionResult> SeedAreas()
        {
            await AreaSeedData.AreaSeedDataAsync(_serviceProvider);
            return Ok("Areas seeded.");
        }

        [HttpPost("locations")]
        public async Task<IActionResult> SeedLocations()
        {
            await LocationSeedData.LocationSeedDataAsync(_serviceProvider);
            return Ok("Locations seeded.");
        }

        [HttpPost("menus")]
        public async Task<IActionResult> SeedMenus()
        {
            await MenuSeedData.MenuSeedDataAsync(_serviceProvider);
            return Ok("Menus seeded.");
        }

        [HttpPost("orders")]
        public async Task<IActionResult> SeedOrders()
        {
            await OrderSeedData.OrderSeedDataAsync(_serviceProvider);
            return Ok("Orders seeded.");
        }

        [HttpPost("patients")]
        public async Task<IActionResult> SeedPatients()
        {
            await PatientSeedData.SeedPatientsAsync(_serviceProvider);
            return Ok("Patients seeded.");
        }

        [HttpPost("foodcategories")]
        public async Task<IActionResult> SeedFoodCategories()
        {
            await FoodCategorySeedData.SeedCateAsync(_serviceProvider);
            return Ok("Food categories seeded.");
        }

        [HttpPost("branch")]
        public async Task<IActionResult> SeedBranch()
        {
            await BranchSeedData.SeedDefaultBranchAsync(_serviceProvider);
            return Ok("Branch seeded.");
        }

        [HttpPost("branchroles")]
        public async Task<IActionResult> SeedBranchRoles([FromQuery] int branchId = 1)
        {
            await BranchRoleSeedData.SeedBranchRolesAsync(_serviceProvider, branchId);
            return Ok("Branch roles seeded.");
        }

        [HttpPost("branchuserroles")]
        public async Task<IActionResult> SeedBranchUserRoles([FromQuery] int branchId = 1)
        {
            await BranchUserRoleSeedData.SeedBranchUserRolesAsync(_serviceProvider, branchId);
            return Ok("Branch user roles seeded.");
        }

        [HttpPost("diseasecategories")]
        public async Task<IActionResult> SeedDiseaseCategories()
        {
            await DiseaseCategorySeedData.SeedDiseaseCategoriesAsync(_serviceProvider);
            return Ok("Disease categories seeded.");
        }

        [HttpPost("patientdiseasecategories")]
        public async Task<IActionResult> SeedPatientDiseaseCategories()
        {
            await DiseaseCategorySeedData.SeedPatientDiseaseCategoriesAsync(_serviceProvider);
            return Ok("Patient disease categories seeded.");
        }

        [HttpPost("identity")]
        public async Task<IActionResult> SeedIdentity([FromQuery] int branchId = 1)
        {
            await IdentitySeedData.SeedRolesAndAdminAsync(_serviceProvider, branchId);
            return Ok("Identity (roles and admin) seeded.");
        }

        [HttpPost("menudetails")]
        public async Task<IActionResult> SeedMenuDetails()
        {
            await MenuDetailSeedData.MenuDetailDataAsync(_serviceProvider);
            return Ok("Menu details seeded.");
        }

        [HttpPost("orderdetails")]
        public async Task<IActionResult> SeedOrderDetails()
        {
            await OrderDetailsSeedData.OrderDetailsSeedDataAsync(_serviceProvider);
            return Ok("Order details seeded.");
        }

        [HttpPost("systemlogs")]
        public async Task<IActionResult> SeedSystemLogs()
        {
            await SystemLogSeedData.SystemLogSeedDataAsync(_serviceProvider);
            return Ok("System logs seeded.");
        }

        [HttpPost("diseasecategoryfoodrestrictions")]
        public async Task<IActionResult> SeedDiseaseCategoryFoodRestrictions()
        {
            await DiseaseCategoryFoodRestrictionSeedData.SeedDiseaseCategoryFoodRestrictionsAsync(_serviceProvider);
            return Ok("Disease category food restrictions seeded.");
        }

        [HttpPost("clear-all-data")]
        public async Task<IActionResult> ClearAllData()
        {
            var dbContext = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.ClearAllDataAsync();
            return Ok("All data cleared from all tables.");
        }

        [HttpPost("all-full")]
        public async Task<IActionResult> SeedAllFull()
        {
            int branchId = await BranchSeedData.SeedDefaultBranchAsync(_serviceProvider);
            await IdentitySeedData.SeedRolesAndAdminAsync(_serviceProvider, branchId);
            await BranchRoleSeedData.SeedBranchRolesAsync(_serviceProvider, branchId);
            await BranchUserRoleSeedData.SeedBranchUserRolesAsync(_serviceProvider, branchId);
            await FoodCategorySeedData.SeedCateAsync(_serviceProvider);
            await FoodSeedData.SeedFoodsAsync(_serviceProvider);
            await AreaSeedData.AreaSeedDataAsync(_serviceProvider);
            await LocationSeedData.LocationSeedDataAsync(_serviceProvider);
            await MenuSeedData.MenuSeedDataAsync(_serviceProvider);
            await MenuDetailSeedData.MenuDetailDataAsync(_serviceProvider);
            await SystemLogSeedData.SystemLogSeedDataAsync(_serviceProvider);
            await OrderSeedData.OrderSeedDataAsync(_serviceProvider);
            await OrderDetailsSeedData.OrderDetailsSeedDataAsync(_serviceProvider);
            await PatientSeedData.SeedPatientsAsync(_serviceProvider);
            await DiseaseCategorySeedData.SeedDiseaseCategoriesAsync(_serviceProvider);
            await DiseaseCategorySeedData.SeedPatientDiseaseCategoriesAsync(_serviceProvider);
            await DiseaseCategoryFoodRestrictionSeedData.SeedDiseaseCategoryFoodRestrictionsAsync(_serviceProvider);
            return Ok("All seed data applied (full, with clear).");
        }

        [HttpGet("debug-user-roles")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponseBase<object>>> DebugUserRoles([FromQuery] string email = "admin@homms.com")
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return NotFound(ApiResponseBase<object>.Error($"User with email '{email}' not found"));
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var systemAdminRoleExists = await _roleManager.RoleExistsAsync("SystemAdmin");
                var allRoles = await _roleManager.Roles.ToListAsync();
                var isInSystemAdminRole = await _userManager.IsInRoleAsync(user, "SystemAdmin");

                var debugInfo = new
                {
                    UserInfo = new
                    {
                        user.Id,
                        user.Email,
                        user.FirstName,
                        user.LastName,
                        user.IsActive
                    },
                    UserRoles = userRoles.ToList(),
                    SystemAdminRoleExists = systemAdminRoleExists,
                    IsInSystemAdminRole = isInSystemAdminRole,
                    AllSystemRoles = allRoles.Select(r => new { r.Id, r.Name, r.Description }).ToList(),
                    RoleCheck = new
                    {
                        ContainsSystemAdmin = userRoles.Contains("SystemAdmin"),
                        ContainsAdmin = userRoles.Contains("Admin"),
                        ContainsManager = userRoles.Contains("Manager"),
                        RoleCount = userRoles.Count
                    }
                };

                return Ok(ApiResponseBase<object>.Success(debugInfo, "Debug information retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseBase<object>.Error($"Debug failed: {ex.Message}"));
            }
        }

        [HttpPost("force-assign-system-admin")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponseBase<object>>> ForceAssignSystemAdmin([FromQuery] string email = "admin@homms.com")
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return NotFound(ApiResponseBase<object>.Error($"User with email '{email}' not found"));
                }

                if (!await _roleManager.RoleExistsAsync("SystemAdmin"))
                {
                    var systemAdminRole = new ApplicationRole
                    {
                        Name = "SystemAdmin",
                        Description = "System administrator - has access to all branches and system settings",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _roleManager.CreateAsync(systemAdminRole);
                }

                var existingRoles = await _userManager.GetRolesAsync(user);
                if (existingRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, existingRoles);
                }

                var result = await _userManager.AddToRoleAsync(user, "SystemAdmin");

                if (result.Succeeded)
                {
                    var newRoles = await _userManager.GetRolesAsync(user);
                    var isNowSystemAdmin = await _userManager.IsInRoleAsync(user, "SystemAdmin");

                    return Ok(ApiResponseBase<object>.Success(new
                    {
                        Message = "SystemAdmin role assigned successfully",
                        UserId = user.Id,
                        Email = user.Email,
                        PreviousRoles = existingRoles.ToList(),
                        NewRoles = newRoles.ToList(),
                        IsNowSystemAdmin = isNowSystemAdmin
                    }, "Role assignment completed"));
                }
                else
                {
                    return BadRequest(ApiResponseBase<object>.Error($"Failed to assign SystemAdmin role: {string.Join(", ", result.Errors.Select(e => e.Description))}"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseBase<object>.Error($"Force assignment failed: {ex.Message}"));
            }
        }

        [HttpGet("debug-current-user")]
        [Authorize]
        public async Task<ActionResult<ApiResponseBase<object>>> DebugCurrentUser()
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(ApiResponseBase<object>.Error("Cannot find user ID in token"));
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(ApiResponseBase<object>.Error("User not found in database"));
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var isSystemAdmin = await _userManager.IsInRoleAsync(user, "SystemAdmin");

                var debugInfo = new
                {
                    TokenClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList(),
                    DatabaseUser = new
                    {
                        user.Id,
                        user.Email,
                        user.FirstName,
                        user.LastName,
                        user.IsActive
                    },
                    UserRoles = userRoles.ToList(),
                    IsSystemAdmin = isSystemAdmin,
                    RoleChecks = new
                    {
                        HasSystemAdminClaim = User.IsInRole("SystemAdmin"),
                        DatabaseSystemAdminCheck = isSystemAdmin,
                        ClaimsRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
                    }
                };

                return Ok(ApiResponseBase<object>.Success(debugInfo, "Current user debug information"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseBase<object>.Error($"Debug current user failed: {ex.Message}"));
            }
        }

        [HttpPost("reseed-admin")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponseBase<object>>> ReseedAdmin()
        {
            try
            {
                var adminUser = await _userManager.FindByEmailAsync("admin@homms.com");

                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = "admin@homms.com",
                        Email = "admin@homms.com",
                        FirstName = "System",
                        LastName = "Administrator",
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    var createResult = await _userManager.CreateAsync(adminUser, "Admin@123456");
                    if (!createResult.Succeeded)
                    {
                        return BadRequest(ApiResponseBase<object>.Error($"Failed to create admin user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}"));
                    }
                }

                if (!await _roleManager.RoleExistsAsync("SystemAdmin"))
                {
                    var systemAdminRole = new ApplicationRole
                    {
                        Name = "SystemAdmin",
                        Description = "System administrator - has access to all branches and system settings",
                        CreatedAt = DateTime.UtcNow
                    };
                    var roleCreateResult = await _roleManager.CreateAsync(systemAdminRole);
                    if (!roleCreateResult.Succeeded)
                    {
                        return BadRequest(ApiResponseBase<object>.Error($"Failed to create SystemAdmin role: {string.Join(", ", roleCreateResult.Errors.Select(e => e.Description))}"));
                    }
                }

                var existingRoles = await _userManager.GetRolesAsync(adminUser);
                if (existingRoles.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(adminUser, existingRoles);
                    if (!removeResult.Succeeded)
                    {
                        return BadRequest(ApiResponseBase<object>.Error($"Failed to remove existing roles: {string.Join(", ", removeResult.Errors.Select(e => e.Description))}"));
                    }
                }

                var addRoleResult = await _userManager.AddToRoleAsync(adminUser, "SystemAdmin");
                if (!addRoleResult.Succeeded)
                {
                    return BadRequest(ApiResponseBase<object>.Error($"Failed to add SystemAdmin role: {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}"));
                }

                var newRoles = await _userManager.GetRolesAsync(adminUser);
                var isSystemAdmin = await _userManager.IsInRoleAsync(adminUser, "SystemAdmin");

                return Ok(ApiResponseBase<object>.Success(new
                {
                    Message = "Admin user reseeded successfully",
                    AdminUser = new
                    {
                        adminUser.Id,
                        adminUser.Email,
                        adminUser.FirstName,
                        adminUser.LastName,
                        adminUser.IsActive
                    },
                    PreviousRoles = existingRoles.ToList(),
                    NewRoles = newRoles.ToList(),
                    IsSystemAdmin = isSystemAdmin,
                    VerificationCheck = new
                    {
                        RoleExists = await _roleManager.RoleExistsAsync("SystemAdmin"),
                        UserInRole = await _userManager.IsInRoleAsync(adminUser, "SystemAdmin"),
                        AllUserRoles = newRoles.ToList()
                    }
                }, "Admin user reseeded and verified"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseBase<object>.Error($"Reseed admin failed: {ex.Message}"));
            }
        }

        [HttpPost("rebuild-identity")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponseBase<object>>> RebuildIdentity()
        {
            try
            {
                var neededRoles = new Dictionary<string, string>
                {
                    { "SystemAdmin", "System administrator - has access to all branches and system settings" },
                    { "Manager", "Branch Manager (Admin for each branch) - management their own branch" },
                    { "Staff", "Hospital/canteen staff member - access determined by branch roles" },
                    { "Patient", "Hospital patient - limited access for ordering food" },
                    { "Customer", "Guest user - wanna be a Customer will register account" }
                };

                var createdRoles = new List<string>();
                foreach (var roleInfo in neededRoles)
                {
                    if (!await _roleManager.RoleExistsAsync(roleInfo.Key))
                    {
                        var role = new ApplicationRole
                        {
                            Name = roleInfo.Key,
                            Description = roleInfo.Value,
                            CreatedAt = DateTime.UtcNow
                        };
                        var result = await _roleManager.CreateAsync(role);
                        if (result.Succeeded)
                        {
                            createdRoles.Add(roleInfo.Key);
                        }
                    }
                }

                var adminResult = await ReseedAdmin();

                return Ok(ApiResponseBase<object>.Success(new
                {
                    Message = "Identity system rebuilt successfully",
                    CreatedRoles = createdRoles,
                    AdminResult = adminResult.Value
                }, "Identity system rebuild completed"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseBase<object>.Error($"Rebuild identity failed: {ex.Message}"));
            }
        }

        [HttpPost("test-admin-login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponseBase<object>>> TestAdminLogin([FromBody] LoginTestModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return NotFound(ApiResponseBase<object>.Error("User not found"));
                }

                var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!passwordValid)
                {
                    return BadRequest(ApiResponseBase<object>.Error("Invalid password"));
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var isSystemAdmin = await _userManager.IsInRoleAsync(user, "SystemAdmin");

                return Ok(ApiResponseBase<object>.Success(new
                {
                    Message = "Login test successful",
                    User = new
                    {
                        user.Id,
                        user.Email,
                        user.FirstName,
                        user.LastName
                    },
                    Roles = userRoles.ToList(),
                    IsSystemAdmin = isSystemAdmin,
                    TestResults = new
                    {
                        PasswordValid = passwordValid,
                        RoleContainsSystemAdmin = userRoles.Contains("SystemAdmin"),
                        UserManagerIsInRole = isSystemAdmin,
                        AllAvailableRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync()
                    }
                }, "Login test completed"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseBase<object>.Error($"Login test failed: {ex.Message}"));
            }
        }
    }

    public class LoginTestModel
    {
        public string Email { get; set; } = "admin@homms.com";
        public string Password { get; set; } = "Admin@123456";
    }
}