using IdentityApp.Data;
using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Controllers
{
	public class UsersController:Controller
	{
		private UserManager<AppUser> _userManager;
        public UsersController(UserManager<AppUser> userManager)
        {
			  _userManager = userManager;

		}
        public IActionResult Index()
		{
			var model = _userManager.Users;	
			return View(model);
		}
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(CreateViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new AppUser
				{
					UserName = model.Email,
					Email = model.Email,
					FullName = model.FullName
				};

				var result = await _userManager.CreateAsync(user, model.Password);

				if (result.Succeeded)
				{
					return RedirectToAction("Index");
				}
				foreach (IdentityError err  in result.Errors) //ilgili hata mesajlarını yazdıralım eğer valid değilse
				{
					ModelState.AddModelError("", err.Description);
				}
			}
			return View(model);
		}
	}
}
