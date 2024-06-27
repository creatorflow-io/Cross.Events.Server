using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Cross.Identity;

public class SeedData
{
    public static async Task EnsureSeedDataAsync(WebApplication app)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            var adminRole = await roleMgr.FindByNameAsync("admin");
            if(adminRole == null)
            {
                var result = await roleMgr.CreateAsync(new ApplicationRole("admin"));
                if (!result.Succeeded)
                {
					throw new Exception(result.Errors.First().Description);
				}
            }

            var contributorRole = await roleMgr.FindByNameAsync("contributor");
            if (contributorRole == null)
            {
				var result = await roleMgr.CreateAsync(new ApplicationRole("contributor"));
				if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }

            var readerRole = await roleMgr.FindByNameAsync("reader");
            if (readerRole == null)
            {
				var result = await roleMgr.CreateAsync(new ApplicationRole("reader"));
				if (!result.Succeeded)
                {
					throw new Exception(result.Errors.First().Description);
				}
			}

            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var alice = await userMgr.FindByNameAsync("alice");
            if (alice == null)
            {
                alice = new ApplicationUser
                {
                    UserName = "alice",
                    Email = "AliceSmith@email.com",
                    EmailConfirmed = true,
                };
                var result = await userMgr.CreateAsync(alice, "Pass123$");
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = await userMgr.AddClaimsAsync(alice, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Alice Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Alice"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        });
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("alice created");

                var rs = await userMgr.AddToRoleAsync(alice, "admin");
                if (rs.Succeeded) { 
                    Log.Debug("alice added to admin role");
                }
                else
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }
            else
            {
                Log.Debug("alice already exists");
            }

            var bob = await userMgr.FindByNameAsync("bob");
            if (bob == null)
            {
                bob = new ApplicationUser
                {
                    UserName = "bob",
                    Email = "BobSmith@email.com",
                    EmailConfirmed = true
                };
                var result = await userMgr.CreateAsync(bob, "Pass123$");
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = await userMgr.AddClaimsAsync(bob, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Bob Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Bob"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                            new Claim("location", "somewhere")
                        });
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("bob created");

                var rs = await userMgr.AddToRoleAsync(bob, "contributor");
                if (rs.Succeeded)
                {
                    Log.Debug("bob added to contributor role");
                }
                else
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }
            else
            {
                Log.Debug("bob already exists");
            }

			var maria = await userMgr.FindByNameAsync("maria");
			if (maria == null)
			{
				maria = new ApplicationUser
				{
					UserName = "maria",
					Email = "MariaOra@email.com",
					EmailConfirmed = true
				};
				var result = await userMgr.CreateAsync(maria, "Pass123$");
				if (!result.Succeeded)
				{
					throw new Exception(result.Errors.First().Description);
				}

				result = await userMgr.AddClaimsAsync(maria, new Claim[]{
							new Claim(JwtClaimTypes.Name, "Maria Smith"),
							new Claim(JwtClaimTypes.GivenName, "maria"),
							new Claim(JwtClaimTypes.FamilyName, "Smith"),
							new Claim(JwtClaimTypes.WebSite, "http://bob.com")
						});
				if (!result.Succeeded)
				{
					throw new Exception(result.Errors.First().Description);
				}
				Log.Debug("maria created");

				var rs = await userMgr.AddToRoleAsync(maria, "reader");
                
                if (rs.Succeeded)
                {
                    Log.Debug("maria added to reader role");
                }
                else
                {
                    throw new Exception(result.Errors.First().Description);
                }
			}
			else
			{
				Log.Debug("maria already exists");
			}
		}
    }
}
