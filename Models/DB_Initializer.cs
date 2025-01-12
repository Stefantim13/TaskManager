using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TaskManager.Data;
using TaskManager.Models;


namespace TaskManager.Models
{
    public class DB_Initializer
    {
        async public static Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                var hasher = new PasswordHasher<ApplicationUser>();

                var roles = new List<IdentityRole>
                {
                    new IdentityRole { Name = "Administrator", NormalizedName = "ADMINISTRATOR" },
                    new IdentityRole { Name = "User", NormalizedName= "USER" },
                    // new IdentityRole { Name = "Organizator", NormalizedName = "ORGANIZATOR" },
                    // new IdentityRole { Name = "Membru", NormalizedName = "MEMBRU" },
                };
                
                foreach (IdentityRole role in roles)
                {
                    if(await roleManager.RoleExistsAsync(role.Name))
                    {
                        continue;
                    }
                    var result = await roleManager.CreateAsync(role);

                    if (result.Succeeded)
                    {
                        continue;
                        var username = role.Name.ToLower().Trim();
                        var email = $"{username}@test.com";
                        var newUser = new ApplicationUser
                        {
                            UserName = username,
                            NormalizedUserName = username.ToUpper(),
                            Email = email,
                            NormalizedEmail = email.ToUpper(),
                            EmailConfirmed = true
                        };
                        Console.WriteLine(newUser.Email);
                        Console.WriteLine(newUser.UserName);

                        var createResult = await userManager.CreateAsync(newUser, "Parola123!");

                        var user = await userManager.FindByNameAsync("administrator");

                        if (user == null)
                        {
                            Console.WriteLine("User not found");
                        }
                        else
                        {
                            Console.WriteLine($"User found: {user.UserName} | {username} | {user.UserName == username}");
                        }

                        if (createResult.Succeeded)
                        {
                            var assignRoleResult = await userManager.AddToRoleAsync(newUser, role.Name);

                            if (assignRoleResult.Succeeded)
                            {
                                Console.WriteLine(role.Name.ToLower() + " has been added to the database.");
                            }
                        }
                        else
                        {
                            foreach (var error in createResult.Errors)
                            {
                                Console.WriteLine($"Error: {error.Description}");
                            }
                        }
                    }
                }
            }
        }
    }
}
