using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Data
{
	public static class IdentityDataSeeding
	{
		private const string adminUser = "admin";
		private const string adminPassword = "Admin_123";

		public static async void IdentityTestUser(IApplicationBuilder app)
		{
			var scope = app.ApplicationServices.CreateScope();
			var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();

			if (!context.Database.GetAppliedMigrations().Any()) //olusturulmus ancak uygulanmamıs migration varsa update-database yapalım alttaki yorum satırındaki de olur
			{
				context.Database.Migrate();
			}
			//if (context.Database.GetPendingMigrations().Any())	context.Database.Migrate();		
			
			var userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<AppUser>>(); //buradaki IdentityUser bizim dbmizdeki AspNetUsers tablomuza denk geliyor.
			//userManager uzerinden artık bir kullanıcı olusturabiliriz.

			var user = await userManager.FindByNameAsync(adminUser);
			if (user == null)
			{
				user = new AppUser
				{
					FullName = "Gokhan Kus",
					UserName = adminUser,
					Email = "gkus1998@gmail.com",
					PhoneNumber = "123456789"
				};
				await userManager.CreateAsync(user, adminPassword);
			}
		}
	}
}
