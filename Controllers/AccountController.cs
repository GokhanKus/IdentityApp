﻿using IdentityApp.Interfaces;
using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Specialized;

namespace IdentityApp.Controllers
{

	public class AccountController : Controller
	{
		private readonly RoleManager<AppRole> _roleManager;
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly IEmailSender _emailSender;
		public AccountController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSender emailSender)
		{
			_roleManager = roleManager;
			_userManager = userManager;
			_signInManager = signInManager;
			_emailSender = emailSender;
		}
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{

				var user = await _userManager.FindByEmailAsync(model.Email);
				if (user != null)
				{
					await _signInManager.SignOutAsync();

					if (!await _userManager.IsEmailConfirmedAsync(user)) //eger user'ın emaili onaylı değilse
					{
						ModelState.AddModelError("", "hesabınızı onaylayınız");
						return View(model);
					}

					var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);
					if (result.Succeeded)
					{
						await _userManager.ResetAccessFailedCountAsync(user);
						await _userManager.SetLockoutEndDateAsync(user, null);
						return RedirectToAction("Index", "Home");
					}
					else if (result.IsLockedOut)
					{
						var lockoutDate = await _userManager.GetLockoutEndDateAsync(user);
						var timeLeft = lockoutDate.Value - DateTime.UtcNow;
						ModelState.AddModelError("", $"Hesabınız kitlendi, lütfen {timeLeft.Minutes + 1} dakika sonra tekrar deneyiniz.");
					}
					else
					{
						ModelState.AddModelError("", "parolanız hatalı");
					}

				}
				else
				{
					ModelState.AddModelError("", "bu e-mail adresi ile bir hesap bulunamadı");
				}
			}
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
					UserName = model.UserName,
					//UserName = model.Email, //mailde "." gibi farklı sembollerin olması username icin skıntı hata verir o yüzden farklı algo veya direkt kullanıcıdan username bilgisini alalım
					Email = model.Email,
					FullName = model.FullName
				};

				IdentityResult result = await _userManager.CreateAsync(user, model.Password);

				if (result.Succeeded)
				{
					var token = await _userManager.GenerateEmailConfirmationTokenAsync(user); //ilgili user icin bize bir token bilgisi üretsin ve program.cste AddDefaultTokenProvider()
					var url = Url.Action("ConfirmEmail", "Account", new { user.Id, token });

					await _emailSender.SendEmailAsync(user.Email,
						"Hesap Onayi",
						$"Lutfen e-mail hesabinizi onaylamak icin linke <a href='http://localhost:40759{url}'>tiklayiniz.</a>");
					//return RedirectToAction("Login", "Account");

					TempData["message"] = "E-mail hesabınızdaki onay mailine tıklayınız.";
					return RedirectToAction("Login");
				}
				foreach (IdentityError err in result.Errors) //ilgili hata mesajlarını yazdıralım eğer valid değilse
				{
					ModelState.AddModelError("", err.Description);
				}
			}
			return View(model);
		}

		public async Task<IActionResult> ConfirmEmail(string id, string token)
		{
			if (id == null || token == null) //ise hata mesajını tempdata ile layoutta yazalım
			{
				TempData["message"] = "gecersiz token bilgisi";
				return View();
			}
			var user = await _userManager.FindByIdAsync(id);
			if (user != null)
			{
				var result = await _userManager.ConfirmEmailAsync(user, token);
				if (result.Succeeded)
				{
					TempData["message"] = "hesabınız onaylandı";
					return RedirectToAction("Login");
				}
			}
			TempData["message"] = "Kullanıcı bulunamadı";
			return View();
		}
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync(); //application altında cookienin silinmesi?
			return RedirectToAction("Login");
		}
		public IActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ForgotPassword(string email)
		{
			if (string.IsNullOrEmpty(email))
			{
				TempData["message"] = "E-Posta adresinizi giriniz";
				return View();
			}

			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				TempData["message"] = "E-Posta ile eşleşen bir kullanıcı bulunamadı";
				return View();
			}

			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			var url = Url.Action("ResetPassword", "Account", new { user.Id, token });
			await _emailSender.SendEmailAsync(email, "Parola Sıfırlama", $"Parolanızı yenilemek için linke <a href='http://localhost:40759{url}'>tıklayınız</a>.");

			TempData["message"] = "E-posta adresinize gönderilen link ile sifrenizi sıfırlayabilirsiniz.";
			return View();
		}
		public IActionResult ResetPassword(string id, string token)
		{
			if (id == null || token == null)
			{
				return RedirectToAction("Login");
			}
			var model = new ResetPasswordModel { Token = token };
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				if (user == null)
				{
					TempData["message"] = "Bu mail adresiyle eslesen bir kullanici yok";
					return RedirectToAction("Login");
				}
				var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

				if (result.Succeeded)
				{
					TempData["message"] = "Sifrenizi degistirildi";
					return RedirectToAction("Login");
				}
				foreach (IdentityError err in result.Errors)
				{
					ModelState.AddModelError("", err.Description);
				}
			}
			return View();
		}
	}
}
