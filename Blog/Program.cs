using Blog.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace Blog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            try
            {
                var scope = host.Services.CreateScope();

                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                context.Database.EnsureCreated();

                var adminRole = new IdentityRole("Admin");

                if (!context.Roles.Any())
                {
                    // Create a role
                    roleManager.CreateAsync(adminRole).GetAwaiter().GetResult();
                }

                if (!context.Users.Any(u => u.UserName == "admin"))
                {
                    // Create an admin
                    var adminUser = new IdentityUser
                    {
                        UserName = "admin",
                        Email = "admin@test.com"
                    };

                    var result = userManager.CreateAsync(adminUser, "password").GetAwaiter().GetResult();

                    // Add role to user
                    userManager.AddToRoleAsync(adminUser, adminRole.Name);
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
