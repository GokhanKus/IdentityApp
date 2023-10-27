using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{
	public class RolesController : Controller
	{
		private readonly RoleManager<AppRole> _roleManager;
		private readonly UserManager<AppUser> _userManager;
		public RolesController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
		{
			_roleManager = roleManager;
			_userManager = userManager;
		}
		public IActionResult Index()
		{
			return View(_roleManager.Roles);
		}
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(AppRole model)
		{
			if (ModelState.IsValid)
			{
				var result = await _roleManager.CreateAsync(model);

				if (result.Succeeded)
				{
					return RedirectToAction("Index");
				}

				foreach (var err in result.Errors)
				{
					ModelState.AddModelError("", err.Description);
				}
			}
			return View(model);
		}

		public async Task<IActionResult> Edit(string id)
		{
			var role = await _roleManager.FindByIdAsync(id);
			if (role != null && role.Name != null)
			{
				ViewBag.Users = await _userManager.GetUsersInRoleAsync(role.Name);
				return View(role);
			}
			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<IActionResult> Edit(AppRole model)
		{
			if (ModelState.IsValid)
			{

				var role = await _roleManager.FindByIdAsync(model.Id);
				if (role != null)
				{
					role.Name = model.Name;
					var result = await _roleManager.UpdateAsync(role);
					if (result.Succeeded)
					{
						return RedirectToAction("Index");
					}
					foreach (var err in result.Errors)
					{
						ModelState.AddModelError("", err.Description);
					}

					if (role.Name != null)	ViewBag.Users = await _userManager.GetUsersInRoleAsync(role.Name);
						
				}
			}
			return View(model);
		}
	}
}

/*Authentication	temel olarak 3 farklı kimlik doğrulama yöntemi vardır;
 
Cookie Based Authentication			=> tarayıcılarda kullanılan doğrulama mesela kullanıcı tarayıcıya bir mail ve parola ile bir login işlemi gerçekleştirince
biz kullanıcının tarayıcısına bir cookie(çerez) bırakıyoruz ve kullanıcı bu siteyi sonra tekrar ziyaret ederse
hatırlanması için gereken bazı bilgilerin tarayıcı belleğine saklanmasıdır. Yani user browserında saklı kalan bir bilgi ve buna cookie diyoruz.
Ve cookie kullanarak bir Authentication işlemi gerçekleştirebiliyoruz.
diyelim ki Kullanıcı bir login islemi gerceklestirdi (mail sifre bilgisi girdi) uygulama o kullanıcının tarayıcısına o cookieyi yani bilgiyi saklar ve 
daha sonra kullanıcı her seferinde uygulamayı talep ettiginde o cookie de server tarafına uygulamaya gonderilir ve
uygulama bu cookie icindeki bilgiye bakarak tekrar bir login islemine gerek kalmadan user'ın istediği kaynağı ona sunma islemidir


Token Based Authentication - JWT	=> (Json Web Token olarak da adlandırılır)
biz bir token bilgisi uygulamada olusturuyoruz ve bu token bilgisini usera gonderiyoruz ve user bu token bilgisini
her seferinde bu tokenı talep ettigi kaynağa, uygulamaya, tekrar göndermesi gerekiyor. bu yontem mobil uygulamalarda kullanılıyor

External Provider Authentication	=> google ile giris yap, facebook ile giris yap olayı budur.
eger sen google ile facebook ile vs. Authentication islemi yapabiliyorsan benim uygulamamda sana guveniyor ve bu Authentication islemini benim app tarafından da kabul etme islemidir
 
 */
