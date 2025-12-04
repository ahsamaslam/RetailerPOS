using AuthModule.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthModule.API.Data
{
    public class DbInitializer : IDbInitializer
    {
        public static readonly List<(string Name, string Description)> DefaultPermissions = new()
        {
            ("ViewDashboard", "Access to view the dashboard"),
            ("ManageUsers", "Permission to create, edit, and delete users"),
            ("ManageRoles", "Permission to create, edit, and delete roles"),
            ("ViewReports", "Access to view reports"),
            ("EditSettings", "Permission to modify application settings"),
            ("AuditLogs", "Access to view audit logs"),
        };
        private static readonly string[] DefaultRoles = new[]
        {
            "Admin",
            "SuperAdmin",
            "User"
        };
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbInitializer(ApplicationDbContext db, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            // 1. Create Default Permissions ------------------------------
            foreach (var (name, desc) in DefaultPermissions)
            {
                if (!await _db.Permissions.AnyAsync(p => p.Name == name))
                {
                    _db.Permissions.Add(new Permission
                    {
                        Name = name,
                        Description = desc
                    });
                }
            }

            await _db.SaveChangesAsync();


            // 2. Create Default Roles ------------------------------------
            foreach (var roleName in DefaultRoles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }


            // 3. Assign Permissions to Roles -----------------------------
            var allPermissions = await _db.Permissions.ToListAsync();

            // SuperAdmin gets ALL permissions
            var superAdmin = await _roleManager.FindByNameAsync("SuperAdmin");
            foreach (var p in allPermissions)
            {
                if (!await _db.RolePermissions.AnyAsync(rp => rp.RoleId == superAdmin.Id && rp.PermissionId == p.Id))
                {
                    _db.RolePermissions.Add(new RolePermission
                    {
                        RoleId = superAdmin.Id,
                        PermissionId = p.Id
                    });
                }
            }

            // Admin gets selected permissions
            var admin = await _roleManager.FindByNameAsync("Admin");
            var adminAllowed = allPermissions.Where(p =>
                p.Name == "ViewDashboard" ||
                p.Name == "ManageUsers" ||
                p.Name == "ManageRoles" ||
                p.Name == "ViewReports"
            );

            foreach (var p in adminAllowed)
            {
                if (!await _db.RolePermissions.AnyAsync(rp => rp.RoleId == admin.Id && rp.PermissionId == p.Id))
                {
                    _db.RolePermissions.Add(new RolePermission
                    {
                        RoleId = admin.Id,
                        PermissionId = p.Id
                    });
                }
            }

            // User gets minimal permissions
            var userRole = await _roleManager.FindByNameAsync("User");
            var userAllowed = allPermissions.Where(p =>
                p.Name == "ViewDashboard" ||
                p.Name == "ViewReports"
            );

            foreach (var p in userAllowed)
            {
                if (!await _db.RolePermissions.AnyAsync(rp => rp.RoleId == userRole.Id && rp.PermissionId == p.Id))
                {
                    _db.RolePermissions.Add(new RolePermission
                    {
                        RoleId = userRole.Id,
                        PermissionId = p.Id
                    });
                }
            }

            await _db.SaveChangesAsync();
        }

    }
}
