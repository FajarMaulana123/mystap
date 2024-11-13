using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.MSIdentity.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;
using mystap.Helpers;
using mystap.Models;
using Newtonsoft.Json;
using NuGet.Packaging;
using System.Collections;
using System.Data;
using System.Linq.Dynamic;
using System.Linq.Dynamic.Core;
using System.Security.Claims;

namespace mystap.Controllers
{
    public class UserController : Controller
    {
        private readonly DatabaseContext _context;
       
        public UserController(DatabaseContext context)
        {
            _context = context;
        }

        [AuthorizedAction]
        public IActionResult Users()
        {
            ViewBag.role = "USERMANAGEMENT";
            if (Module.hasModule("USERMANAGEMENT", HttpContext.Session))
            {
                ViewBag.plant = _context.plans.Where(p => p.deleted == 0).ToList();
                ViewBag.fungsi = _context.fungsiBagian.GroupBy(p => p.fungsi).Select(p => p.Key).ToList();
                ViewBag.bagian = _context.fungsiBagian.GroupBy(p => p.bagian).Select(p => p.Key).ToList();
                ViewBag.groupModule = _context.modul.GroupBy(p => p.group).Select(p => new
                {
                    group = p.Key,
                    jumlah = p.Select(s => s.group).Count()
                }).OrderBy(p => p.jumlah).ToList();

                return View(_context.modul.Where(s => s.status == "ACTIVE").ToList());
            }
            else
            {
                return NotFound();
            }
        }

        [AuthorizedAction]
        public async Task<IActionResult> Get_User()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();

                var start = Request.Form["start"].FirstOrDefault();

                var length = Request.Form["length"].FirstOrDefault();

                var sortColumn = Request.Form["columns[" + Request.Form["order[1][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                var sortColumnDirection = Request.Form["order[1][dir]"].FirstOrDefault();

                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;


                var customerData = _context.users.Where(s => s.deleted == 0);

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }


                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.name.StartsWith(searchValue));
                }
                // Total number of rows count
                //Console.WriteLine(customerData);
                recordsTotal = customerData.Count();
                // Paging
                var datas = await customerData.Skip(skip).Take(pageSize).ToListAsync();
                //var data = _memoryCache.Get("products");
                //data = await _memoryCache.Set("products", datas, expirationTime);
                // Returning Json Data
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = datas });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AuthorizedAction]
        public IActionResult Create_User(IFormCollection formcollaction)
        {
            try
            {
                
                Users users = new Users();
                users.name = formcollaction["nama"];
                users.email = formcollaction["email"];
                users.username = formcollaction["username"];
                users.alias = formcollaction["alias"];
                users.plant = formcollaction["plant"];
                users.asal = formcollaction["asal"];
                users.uSection = formcollaction["fungsi"];
                users.subSection = formcollaction["bagian"];
                users.status = formcollaction["status"];
                users.statPekerja = formcollaction["statPekerja"];
                users.password = EncryptPassword.Encrypt(formcollaction["password"]);
                users.noPekerja = formcollaction["noPekerja"];
                users.role = formcollaction["role"];
                users.created_at = DateTime.Now;
                users.createBy = HttpContext.Session.GetInt32("id");
                users.deleted = 0;

                Boolean t;
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    _context.users.Add(users);
                    _context.SaveChanges();

                    var module = formcollaction["permission[]"];
                    if (!module.IsNullOrEmpty())
                    {
                     
                        foreach (var val in module)
                        {
                            UserModul obj = new UserModul();
                            obj.id_modul = val;
                            obj.id_user = Convert.ToInt32(users.id);

                            _context.userModul.Add(obj);
                            _context.SaveChanges();

                        }
                    }
                    transaction.Commit();
                    t = true;
                }
                catch
                {
                    transaction.Rollback();
                    t = false;
                }
                return Json(new { result = t });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AuthorizedAction]
        public IActionResult Update_User(Users users)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Users obj = _context.users.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {
                    obj.name = Request.Form["nama"].FirstOrDefault();
                    obj.username = Request.Form["username"].FirstOrDefault();
                    obj.email = Request.Form["email"].FirstOrDefault();
                    obj.alias = Request.Form["alias"].FirstOrDefault();
                    obj.plant = Request.Form["plant"].FirstOrDefault();
                    obj.asal = Request.Form["asal"].FirstOrDefault();
                    obj.uSection = Request.Form["fungsi"].FirstOrDefault();
                    obj.subSection = Request.Form["bagian"].FirstOrDefault();
                    obj.status = Request.Form["status"].FirstOrDefault();
                    obj.statPekerja = Request.Form["statPekerja"].FirstOrDefault();
                    obj.noPekerja = Request.Form["noPekerja"].FirstOrDefault();
                    obj.role = Request.Form["role"].FirstOrDefault();
                    obj.updated_at = DateTime.Now;
                    obj.updatedBy = HttpContext.Session.GetInt32("id");

                    if (Request.Form["password"].FirstOrDefault() != "")
                    {
                        obj.password = EncryptPassword.Encrypt(Request.Form["password"].FirstOrDefault());
                    }

                    Boolean t;
                    using var transaction = _context.Database.BeginTransaction();
                    try
                    {

                        _context.SaveChanges();

                        var module = Request.Form["permission[]"];
                        if (!module.IsNullOrEmpty())
                        {
                            var cek = _context.userModul.AsNoTracking().Where(p => p.id_user == id).ToList();
                            if (!cek.IsNullOrEmpty())
                            {
                                _context.userModul.Where(p => p.id_user == id).ExecuteDelete();
                            }
                            foreach (var c in module) 
                            {

                                UserModul um = new UserModul();
                                um.id_user = Convert.ToInt32(obj.id);
                                um.id_modul = c;

                                _context.userModul.Add(um);
                                _context.SaveChanges();
                            }
                        }


                        transaction.Commit();
                        t = true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        t = false;
                    }
                    return Json(new { result = t });
                }
                return Json(new { Results = false });
            }
            catch
            {
                throw;
            }
        }


		

        public IActionResult Locked_User(Users users)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Users obj = _context.users.Where(p => p.id == id).FirstOrDefault();
                //var r = new Dictionary<string, string>();

                if (obj.locked == 1)
                {
                    obj.locked = 0;

                    //if (obj.locked == 0)
                    //{
                    //    r["title"] = "Sukses!";
                    //    r["icon"] = "success";
                    //    r["status"] = "Berhasil Unlocked!";
                    //}
                    //else
                    //{
                    //    r["title"] = "Maaf!";
                    //    r["icon"] = "error";
                    //    r["status"] = "<br><b>Tidak dapat di Hapus! <br> Silakan hubungi Administrator.</b>";
                    //}
                    _context.SaveChanges();
                    return Json(new { Results = true });
                }
                else
                {
                    obj.locked = 1;

                    //if (obj.locked == 1)
                    //{
                    //    r["title"] = "Sukses!";
                    //    r["icon"] = "success";
                    //    r["status"] = "Berhasil Locked";
                    //}
                    //else
                    //{
                    //    r["title"] = "Maaf!";
                    //    r["icon"] = "error";
                    //    r["status"] = "<br><b>Tidak dapat di Hapus! <br> Silakan hubungi Administrator.</b>";
                    //}
                    _context.SaveChanges();
                    return Json(new { Results = true });
                }
            }
            catch
            {
                throw;
            }
        }
        //[AuthorizedAction]
        public IActionResult Deleted_User(Users users)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                Users obj = _context.users.Where(p => p.id == id).FirstOrDefault();

                if (obj == null)
                {
                    obj.deleted = 1;
                    _context.SaveChanges();

                    return Json(new { title = "Sukses!", icon = "success", status = "Berhasil Dihapus" });
                }
                return Json(new { title = "Maaf!", icon = "error", status = "Tidak Dapat di Hapus!, Silahkan Hubungi Administrator " });
            }
            catch
            {
                throw;
            }
        }

        [AuthorizedAction]
        public IActionResult Profile(long id)
        {
            
            return View();
          
        }

        [AuthorizedAction]
        public async Task<IActionResult> Edit_Profile(Users Update_User)
        {
            {
                if (ModelState.IsValid)
                {
                    Users existingUser = _context.users.FirstOrDefault(u => u.id == Update_User.id);

                    // Get the currently authenticated user's ID
                    var authenticatedUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                    if (existingUser != null || existingUser.id == Update_User.id )
                    {
                        // User is trying to update another user's profile or the profile doesn't exist, handle accordingly
                        return NotFound();
                    }

                    existingUser.username = Update_User.username;
                    existingUser.email = Update_User.email;

                    if (Request.Form["password"].FirstOrDefault() != "")
                    {
                        existingUser.password = EncryptPassword.Encrypt(Request.Form["password"].FirstOrDefault());
                    }

                    _context.SaveChanges();
                }

                return View(Profile);
            }
        }
    }
}
