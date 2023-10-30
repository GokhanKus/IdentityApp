using IdentityApp.Data;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Controllers
{
	[Authorize(Roles = "admin")] //sadece admin rolünde olan kullanıcılar bu sayfalara gidebilsin digerleri account altındaki accessdenied sayfasına gidecek (program.cs)
	//burada mesela admin rolünde olmayan kullanıcılar örn users veya /users/edit sayfasına gitmek isterse accessdenied sayfasına yönlendirilecek.
	public class UsersController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly RoleManager<AppRole> _roleManager;
		public UsersController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}
		public IActionResult Index()
		{									//admin rolünde olmayan bir user mesela users kısmına girerse ki link gözükmüyor ama url kısmını yazarsa girebilir bunun önüne geçmek için;
			//if (User.IsInRole("admin"))		//(sadece adminin girmesi gereken yer)
			//{
			//	return RedirectToAction("login", "account");
			//}
			//yukarıdaki if kısmına gerek yok cünkü sayfanın basına [Authorize(Roles = "admin")] yazdık yani buradaki herbir metot icin tek tek if yazmaktansa bu controller altındaki butun sayfaları yetkilendirdik
			var model = _userManager.Users;
			return View(model);
		}
		//[Authorize(Roles = "admin")] bunu yukarıda tanımlamak yerine örn sadece burada tanımlarsak sadece bu kısma admin rolündeki kisiler girebilir.
		//[AllowAnonymous] //bunu diyerek admine ait olan sayfada sadece ilgili parta giris izni verilebilir.
		public async Task<IActionResult> Edit(string id)
		{
			if (id == null) return RedirectToAction("Index");

			var user = await _userManager.FindByIdAsync(id);

			if (user != null)
			{
				ViewBag.Roles = await _roleManager.Roles.Select(i => i.Name).ToListAsync();
				var model = new EditViewModel
				{
					Id = user.Id,
					FullName = user.FullName,
					Email = user.Email,
					SelectedRoles = await _userManager.GetRolesAsync(user),
				};
				return View(model);
			}
			return RedirectToAction("Index");

		}

		[HttpPost]
		public async Task<IActionResult> Edit(string id, EditViewModel model)
		{
			if (id != model.Id) return RedirectToAction("Index");
			ViewBag.Roles = await _roleManager.Roles.Select(i => i.Name).ToListAsync();
			//user bilgierini güncellerken - post ederken - eşleşmeyen password veya hatalı email girilirse
			//Users/Edit.cshtml sayfasındaki ViewBag.Roles null reference hatası veriyordu o yüzden burada da viewbag.roles bilgilerini alalım ki null değer vermesin.

			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByIdAsync(model.Id);

				if (user != null)
				{

					user.Email = model.Email;
					user.FullName = model.FullName;

					var result = await _userManager.UpdateAsync(user);
					if (result.Succeeded && !string.IsNullOrEmpty(model.Password))
					{
						await _userManager.RemovePasswordAsync(user);
						await _userManager.AddPasswordAsync(user, model.Password);
					}
					if (result.Succeeded)
					{
						await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user)); //once userdan rolleri kaldıralım

						if (model.SelectedRoles != null)
						{
							await _userManager.AddToRolesAsync(user, model.SelectedRoles); //sonra usera rolleri atayalım
						}
						return RedirectToAction("Index");
					}

					foreach (IdentityError err in result.Errors) //useri güncellerken hata alırsak ilgili mesajları yazdıralım
					{
						ModelState.AddModelError("", err.Description);
					}
				}
			}
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Delete(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user != null)
			{
				await _userManager.DeleteAsync(user);
			}
			return RedirectToAction("Index");
		}
	}
}
