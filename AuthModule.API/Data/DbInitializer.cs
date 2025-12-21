using AuthModule.API.Data;
using AuthModule.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

public class DbInitializer : IDbInitializer
{
    // Keep original default permissions (will be added alongside page perms)
    public static readonly List<(string Name, string Description)> DefaultPermissions = new()
    {
        //("ViewDashboard", "Access to view the dashboard"),
        ("ManageUsers", "Permission to create, edit, and delete users"),
        ("ManageRoles", "Permission to create, edit, and delete roles"),
        ("ViewReports", "Access to view reports"),
        ("EditSettings", "Permission to modify application settings"),
        ("AuditLogs", "Access to view audit logs"),
    };

    // Add Manager role and ensure SuperAdmin/Admin/User/Manager are present
    private static readonly string[] DefaultRoles = new[]
    {
        "superadmin",
        "admin",
        "manager",
        "user"
    };

    private readonly ApplicationDbContext _db;
    private readonly RoleManager<IdentityRole> _roleManager;

    // Pages for which we'll create View/Add/Edit/Delete permissions
    private static readonly string[] Pages = new[]
    {
        "Dashboard",
        "Branches",
        "Categories",
        "Customers",
        "Groups",
        "Items",
        "ItemType",
        "OpeningBalances",
        "Sales",
        "Purchases",
        "SubGroups",
        "Vendors"
    };

    // Actions for each page
    private static readonly string[] PageActions = new[] { "View", "Create", "Edit", "Delete" };

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;

    public DbInitializer(
        ApplicationDbContext db,
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        IConfiguration config)
    {
        _db = db;
        _roleManager = roleManager;
        _userManager = userManager;
        _config = config;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        // 1. Create Default (global) permissions ------------------------------
        foreach (var (name, desc) in DefaultPermissions)
        {
            if (!await _db.Permissions.AnyAsync(p => p.Name == name, cancellationToken))
            {
                _db.Permissions.Add(new Permission
                {
                    Name = name,
                    Description = desc
                });
            }
        }

        // 2. Create page-specific permissions (Add/View/Edit/Delete)
        foreach (var page in Pages)
        {
            foreach (var action in PageActions)
            {
                // permission name convention: "<Page>.<Action>" e.g. "Items.View"
                var permName = $"{page}.{action}";
                var permDesc = $"{action} permission for {page} page";
                if (!await _db.Permissions.AnyAsync(p => p.Name == permName, cancellationToken))
                {
                    _db.Permissions.Add(new Permission
                    {
                        Name = permName,
                        Description = permDesc
                    });
                }
            }
        }

         // 3. Ensure default roles exist ------------------------------------
        foreach (var roleName in DefaultRoles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
        await _db.SaveChangesAsync(cancellationToken);

        // load permissions after creation
        var allPermissions = await _db.Permissions.ToListAsync(cancellationToken);

        // Helper: ensure role exists and return IdentityRole (or null)
        async Task<IdentityRole?> GetRoleAsync(string roleName)
        {
            return await _roleManager.FindByNameAsync(roleName);
        }

        // 4. Assign permissions to SuperAdmin and Admin (ALL permissions) ----
        var superAdminRole = await GetRoleAsync("superadmin");
        var adminRole = await GetRoleAsync("admin");

        if (superAdminRole != null)
        {
            foreach (var p in allPermissions)
            {
                var existingRolePermissions = await _db.RolePermissions
                                            .Select(rp => new { rp.RoleId, rp.PermissionId })
                                            .ToListAsync(cancellationToken);

                var rpSet = new HashSet<(string RoleId, int PermissionId)>(
                    existingRolePermissions.Select(x => (x.RoleId, x.PermissionId))
                );
                if (rpSet.Add((superAdminRole.Id, p.Id)))
                {
                    _db.RolePermissions.Add(new RolePermission
                    {
                        RoleId = superAdminRole.Id,
                        PermissionId = p.Id
                    });
                }
            }
        }

        if (adminRole != null)
        {
            foreach (var p in allPermissions)
            {
                var existingRolePermissions = await _db.RolePermissions
                                            .Select(rp => new { rp.RoleId, rp.PermissionId })
                                            .ToListAsync(cancellationToken);

                var rpSet = new HashSet<(string RoleId, int PermissionId)>(
                    existingRolePermissions.Select(x => (x.RoleId, x.PermissionId))
                );
                if (rpSet.Add((adminRole.Id, p.Id)))
                {
                    _db.RolePermissions.Add(new RolePermission
                    {
                        RoleId = adminRole.Id,
                        PermissionId = p.Id
                    });
                }
            }
        }

        // 5. Manager role: View, Add, Edit for pages + some sensible global perms
        var managerRole = await GetRoleAsync("manager");
        if (managerRole != null)
        {
            foreach (var p in allPermissions)
            {

                var existingRolePermissions = await _db.RolePermissions
                                            .Select(rp => new { rp.RoleId, rp.PermissionId })
                                            .ToListAsync(cancellationToken);

                var rpSet = new HashSet<(string RoleId, int PermissionId)>(
                    existingRolePermissions.Select(x => (x.RoleId, x.PermissionId))
                );
                if (rpSet.Add((managerRole.Id, p.Id)))
                {
                    _db.RolePermissions.Add(new RolePermission
                    {
                        RoleId = managerRole.Id,
                        PermissionId = p.Id
                    });
                }
            }
        }

        // 6. User role: Add + View for pages + minimal global perms
        var userRole = await GetRoleAsync("user");
        if (userRole != null)
        {
            foreach (var p in allPermissions)
            {
                var existingRolePermissions = await _db.RolePermissions
                                            .Select(rp => new { rp.RoleId, rp.PermissionId })
                                            .ToListAsync(cancellationToken);

                var rpSet = new HashSet<(string RoleId, int PermissionId)>(
                    existingRolePermissions.Select(x => (x.RoleId, x.PermissionId))
                );
                if (rpSet.Add((userRole.Id, p.Id)))
                {
                    _db.RolePermissions.Add(new RolePermission
                    {
                        RoleId = userRole.Id,
                        PermissionId = p.Id
                    });
                }
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        // 7. Create default SuperAdmin user -----------------------------
        var adminSection = _config.GetSection("DefaultAdmin");

        var adminUserName = adminSection["UserName"];
        var adminEmail = adminSection["Email"];
        var adminPassword = adminSection["Password"];

        if (!string.IsNullOrWhiteSpace(adminUserName) &&
            !string.IsNullOrWhiteSpace(adminPassword))
        {
            var superAdminUser =
                await _userManager.FindByNameAsync(adminUserName);

            if (superAdminUser == null)
            {
                superAdminUser = new ApplicationUser
                {
                    UserName = adminUserName,
                    Email = adminEmail ?? adminUserName,
                    EmailConfirmed = true
                };

                var createResult =
                    await _userManager.CreateAsync(superAdminUser, adminPassword);

                if (!createResult.Succeeded)
                {
                    throw new Exception(
                        "Failed to create SuperAdmin user: " +
                        string.Join(", ", createResult.Errors.Select(e => e.Description))
                    );
                }
            }

            // Ensure SuperAdmin role assignment
            if (!await _userManager.IsInRoleAsync(superAdminUser, "superadmin"))
            {
                await _userManager.AddToRoleAsync(superAdminUser, "superadmin");
            }
        }
    }
}
