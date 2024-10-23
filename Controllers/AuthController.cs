using Microsoft.AspNetCore.Mvc;
using mystap.Models;
using System.Security.Cryptography;
using System.Text;

namespace mystap.Controllers
{
	public class AuthController : Controller
	{
		private readonly DatabaseContext _context;
		public AuthController(DatabaseContext context, IWebHostEnvironment environment)
		{
			_context = context;
		}

		public IActionResult Login()
		{
			return View();
		}

		public IActionResult ValidateLogin()
		{
			try
			{
				var username = Request.Form["username"].FirstOrDefault();
				var password = Request.Form["password"].FirstOrDefault();

				var admin = _context.users.Where(p => p.username == username).FirstOrDefault();
				if(admin != null)
				{
					if(password == EncryptPassword.Decrypt(admin.password))
					{
						if(admin.locked == 0)
						{

							HttpContext.Session.SetInt32("id", Convert.ToInt32(admin.id));
							HttpContext.Session.SetString("username", admin.username);
							HttpContext.Session.SetString("status", admin.status);
							HttpContext.Session.SetString("role", admin.role);
							HttpContext.Session.SetString("alias", admin.alias);

							return Json(new { result = true, text = "Login Berhasil" });
						}
						else
						{
							return Json(new { result = false, text = "Akun Terkunci!, Harap segera hubungi Administrator" });
						}
					}
					else
					{
						return Json(new { result = false, text = "Password Anda Salah!" });
					}
				}
				else
				{
					return Json(new { result = false, text = "Akun Tidak ditemukan!" });
				}

			}
			catch
			{
				throw;
			}
		}


		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Login", "Auth");
		}

	}
}
