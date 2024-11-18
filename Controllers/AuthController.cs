using Microsoft.AspNetCore.Mvc;
using mystap.Models;
using Newtonsoft.Json;
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
							HttpContext.Session.SetString("statPekerja", admin.statPekerja);
                            HttpContext.Session.SetString("role", admin.role);
							HttpContext.Session.SetString("alias", admin.alias);
							HttpContext.Session.SetString("fungsi", admin.subSection);
							HttpContext.Session.SetString("foto", admin.foto);

                            var builderM = (from um in _context.userModul
											join m in _context.modul on um.id_modul equals m.id_modul
											select new
											{
												id_user = um.id_user,
												nama = m.nama,
												alias = m.alias
											}).Where(p => p.id_user == admin.id && p.alias != "");

                            List<string> nama = new List<string>();
                            List<string> alias = new List<string>();

                            if (builderM.Count() > 0)
							{
								foreach(var val in builderM.ToList())
								{
									nama.Add(val.nama);
									alias.Add(val.alias);
                                }
							}
							
							HttpContext.Session.SetString("admin_modules", JsonConvert.SerializeObject(alias));



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

        public IActionResult GetModul()
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                var data = _context.userModul.Where(p => p.id_user == id).ToList();
                return Json(new { data_modul = data });
            }
            catch
            {
                throw;
            }
        }

    }
}
