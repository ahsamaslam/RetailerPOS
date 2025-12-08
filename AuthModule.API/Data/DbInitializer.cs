using AuthModule.API.Data;
using AuthModule.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class DbInitializer : IDbInitializer
{
    // Keep original default permissions (will be added alongside page perms)
    public static readonly List<(string Name, string Description)> DefaultPermissions = new()
    {
        ("ViewDashboard", "Access to view the dashboard"),
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
        "Admin",
        "Branches",
        "Categories",
        "Customer",
        "Employee",
        "Groups",
        "Items",
        "ItemType",
        "OpeningBalance",
        "Sales",
        "Purchases",
        "SubGroups",
        "Vendors"
    };

    // Actions for each page
    private static readonly string[] PageActions = new[] { "View", "Create", "Edit", "Delete" };

    public DbInitializer(ApplicationDbContext db, RoleManager<IdentityRole> roleManager)
    {
        _db = db;
        _roleManager = roleManager;
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

        await _db.SaveChangesAsync(cancellationToken);

        // 3. Ensure default roles exist ------------------------------------
        foreach (var roleName in DefaultRoles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

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
                if (!await _db.RolePermissions.AnyAsync(rp => rp.RoleId == superAdminRole.Id && rp.PermissionId == p.Id, cancellationToken))
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
                if (!await _db.RolePermissions.AnyAsync(rp => rp.RoleId == adminRole.Id && rp.PermissionId == p.Id, cancellationToken))
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
            // page-level perms: View, Add, Edit
            var managerPagePerms = allPermissions.Where(p =>
                Pages.Any(page => p.Name == $"{page}.View" || p.Name == $"{page}.Add" || p.Name == $"{page}.Edit"));

            foreach (var p in managerPagePerms)
            {
                if (!await _db.RolePermissions.AnyAsync(rp => rp.RoleId == managerRole.Id && rp.PermissionId == p.Id, cancellationToken))
                {
                    _db.RolePermissions.Add(new RolePermission
                    {
                        RoleId = managerRole.Id,
                        PermissionId = p.Id
                    });
                }
            }

            // include a few global perms for managers (adjust as you like)
            var managerGlobal = new[] { "ViewDashboard", "ViewReports" };
            foreach (var g in managerGlobal)
            {
                var gp = allPermissions.FirstOrDefault(x => x.Name == g);
                if (gp != null && !await _db.RolePermissions.AnyAsync(rp => rp.RoleId == managerRole.Id && rp.PermissionId == gp.Id, cancellationToken))
                {
                    _db.RolePermissions.Add(new RolePermission
                    {
                        RoleId = managerRole.Id,
                        PermissionId = gp.Id
                    });
                }
            }
        }

        // 6. User role: Add + View for pages + minimal global perms
        var userRole = await GetRoleAsync("user");
        if (userRole != null)
        {
            // page-level perms: Add, View
            var userPagePerms = allPermissions.Where(p =>
                Pages.Any(page => p.Name == $"{page}.View" || p.Name == $"{page}.Add"));

            foreach (var p in userPagePerms)
            {
                if (!await _db.RolePermissions.AnyAsync(rp => rp.RoleId == userRole.Id && rp.PermissionId == p.Id, cancellationToken))
                {
                    _db.RolePermissions.Add(new RolePermission
                    {
                        RoleId = userRole.Id,
                        PermissionId = p.Id
                    });
                }
            }

            // include small set of global perms (adjust as you like)
            var userGlobal = new[] { "ViewDashboard" };
            foreach (var g in userGlobal)
            {
                var gp = allPermissions.FirstOrDefault(x => x.Name == g);
                if (gp != null && !await _db.RolePermissions.AnyAsync(rp => rp.RoleId == userRole.Id && rp.PermissionId == gp.Id, cancellationToken))
                {
                    _db.RolePermissions.Add(new RolePermission
                    {
                        RoleId = userRole.Id,
                        PermissionId = gp.Id
                    });
                }
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
