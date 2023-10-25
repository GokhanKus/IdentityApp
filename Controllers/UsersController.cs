using IdentityApp.Data;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{
	public class UsersController:Controller
	{
		IdentityContext _userManager;
        public UsersController(IdentityContext userManager)
        {
			  _userManager = userManager;

		}
        public IActionResult Index()
		{
			var model = _userManager.Users;	
			return View(model);
		}
	}
}
