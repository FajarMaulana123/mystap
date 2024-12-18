﻿using Azure.Core;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using MessagePack;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using mystap.Helpers;
using mystap.Models;
using System.Data;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Runtime.CompilerServices;

namespace mystap.Controllers
{
    public class DataController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IWebHostEnvironment environment;

        public DataController(DatabaseContext context, IWebHostEnvironment environment)
        {
            _context = context;
            this.environment = environment;
        }

		[AuthorizedAction]
		public IActionResult Rapat()
        {
            ViewBag.role = "RAPAT";
            if (Module.hasModule("RAPAT", HttpContext.Session))
            {
                ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == 1).ToList();
                return View();
            }
            else
            {
                return NotFound();
            }
        }

		[AuthorizedAction]
		public async Task<IActionResult> Get_Rapat()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                // Skipping number of Rows count
                var start = Request.Form["start"].FirstOrDefault();
                // Paging Length 10, 20
                var length = Request.Form["length"].FirstOrDefault();
                // Sort Column Name
                var sortColumn = Request.Form["columns[" + Request.Form["order[1][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                // Sort Column Direction (asc, desc)
                var sortColumnDirection = Request.Form["order[1][dir]"].FirstOrDefault();
                // Search Value from (Search box)
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                // Paging Size (10, 20, 50, 100)
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;


                var project_filter = Request.Form["project"].FirstOrDefault();
                var project_rev = Request.Form["project_rev"].FirstOrDefault();

                var customerData = _context.rapat.Include("users").Where(s => s.id_project == Convert.ToInt64(project_filter)).Where(s => s.deleted == 0).Select(a => new { id = a.id, id_project = a.id_project, tanggal = a.tanggal, judul = a.judul, materi = a.materi, notulen = a.notulen, nama_ = a.users.alias, created_date = a.created_date });

                // Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.judul.StartsWith(searchValue) || m.notulen.StartsWith(searchValue) || m.nama_.StartsWith(searchValue));
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
		[HttpPost]
        public IActionResult Create_Rapat(IFormCollection formcollaction)
        {

            try
            {
                Rapat rapat = new Rapat();
                rapat.id_project = Convert.ToInt32(formcollaction["id_project"]);
                rapat.judul = formcollaction["judul"];
                rapat.created_by = HttpContext.Session.GetInt32("id");
                rapat.tanggal = Convert.ToDateTime(formcollaction["tanggal"]);
                rapat.created_date = DateTime.Now;
                rapat.deleted = 0;

                if (Request.Form.Files.Count() != 0)
                {
                    var i = 0;
                    foreach (var val in Request.Form.Files)
                    {
                        IFormFile postFile = Request.Form.Files[i];
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + postFile.FileName;
                        if (postFile.Name == "materi")
                        {
                            string path = environment.WebRootPath + "/upload/rapat/materi/" + fileName;
                            using (var stream = System.IO.File.Create(path))
                            {
                                postFile.CopyTo(stream);
                                rapat.materi = "upload/rapat/materi/" + fileName;
                            }
                        }
                        if (postFile.Name == "notulen")
                        {
                            string path = environment.WebRootPath + "/upload/rapat/notulen/" + fileName;
                            using (var stream = System.IO.File.Create(path))
                            {
                                postFile.CopyTo(stream);
                                rapat.notulen = "upload/rapat/notulen/" + fileName;
                            }
                        }
                        i++;
                    }

                }
                
                
                Boolean t;
                if (rapat != null)
                {
                    _context.rapat.Add(rapat);
                    _context.SaveChanges();
                    t = true;
                }
                else
                {
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
		public IActionResult Update_Rapat(Rapat rapat)
        {

            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Rapat obj = _context.rapat.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {
                    obj.id_project = Convert.ToInt64(Request.Form["id_project"]);
                    obj.judul = Request.Form["judul"].FirstOrDefault();
                    obj.tanggal = Convert.ToDateTime(Request.Form["tanggal"].FirstOrDefault()).Date;

                    if (Request.Form.Files.Count() != 0)
                    {
                        var i = 0;
                        foreach (var val in Request.Form.Files)
                        {
                            IFormFile postFile = Request.Form.Files[i];
                            string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + postFile.FileName;
                            if (postFile.Name == "materi")
                            {
                                string ExitingFile = environment.WebRootPath + "/" + Request.Form["materi_"].FirstOrDefault();
                                System.IO.File.Delete(ExitingFile);

                                string path = environment.WebRootPath + "/upload/rapat/materi/" + fileName;
                                using (var stream = System.IO.File.Create(path))
                                {
                                    postFile.CopyTo(stream);
                                    obj.materi = "upload/rapat/materi/" + fileName;
                                }
                            }
                            if (postFile.Name == "notulen")
                            {
                                string ExitingFile = environment.WebRootPath + "/" + Request.Form["notulen_"].FirstOrDefault();
                                System.IO.File.Delete(ExitingFile);

                                string path = environment.WebRootPath + "/upload/rapat/notulen/" + fileName;
                                using (var stream = System.IO.File.Create(path))
                                {
                                    postFile.CopyTo(stream);
                                    obj.notulen = "upload/rapat/notulen/" + fileName;
                                }
                            }
                            i++;
                        }

                    }
                    else
                    {
                        obj.materi = Request.Form["materi_"].FirstOrDefault();
                        obj.notulen = Request.Form["notulen_"].FirstOrDefault();
                    }
                    _context.SaveChanges();
                    return Json(new { result = true });
                }
                return Json(new { result = false });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult Deleted_Rapat(Rapat rapat)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                Rapat obj = _context.rapat.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
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
		public IActionResult Steerco()
        {

            ViewBag.role = "STEERCO";
            if (Module.hasModule("STEERCO", HttpContext.Session))
            {
                ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == 1).ToList();
                return View();
            }
            else
            {
                return NotFound();
            }
        }

		[AuthorizedAction]
		public async Task<IActionResult> Get_Steerco()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                // Skipping number of Rows count
                var start = Request.Form["start"].FirstOrDefault();
                // Paging Length 10, 20
                var length = Request.Form["length"].FirstOrDefault();
                // Sort Column Name
                var sortColumn = Request.Form["columns[" + Request.Form["order[1][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                // Sort Column Direction (asc, desc)
                var sortColumnDirection = Request.Form["order[1][dir]"].FirstOrDefault();
                // Search Value from (Search box)
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                // Paging Size (10, 20, 50, 100)
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;


                var project_filter = Request.Form["project"].FirstOrDefault();
                var project_rev = Request.Form["project_rev"].FirstOrDefault();

                var customerData = _context.steerco.Include("users").Where(s => s.id_project == Convert.ToInt64(project_filter)).Where(s => s.deleted == 0).Select(a => new { id = a.id, id_project = a.id_project, tanggal = a.tanggal, judul = a.judul, materi = a.materi, notulen = a.notulen, nama_ = a.users.alias, created_date = a.created_date });

                // Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.judul.StartsWith(searchValue) || m.notulen.StartsWith(searchValue) || m.nama_.StartsWith(searchValue));
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
		public IActionResult Create_Steerco(IFormCollection formcollaction)
        {
            try
            {
                Steerco steerco = new Steerco();
                steerco.id_project = Convert.ToInt32(formcollaction["id_project"]);
                steerco.judul = formcollaction["judul"];
                steerco.created_by = HttpContext.Session.GetInt32("id");
                steerco.tanggal = Convert.ToDateTime(formcollaction["tanggal"]);
                steerco.created_date = DateTime.Now;
                steerco.deleted = 0;

                if (Request.Form.Files.Count() != 0)
                {
                    var i = 0;
                    foreach(var val in Request.Form.Files)
                    {
                        IFormFile postFile = Request.Form.Files[i];
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + postFile.FileName;
                        if(postFile.Name == "materi")
                        {
                            string path = environment.WebRootPath + "/upload/steerco/materi/" + fileName;
                            using (var stream = System.IO.File.Create(path))
                            {
                                postFile.CopyTo(stream);
                                steerco.materi = "upload/steerco/materi/" + fileName;
                            }
                        }
                        if (postFile.Name == "notulen")
                        {
                            string path = environment.WebRootPath + "/upload/steerco/notulen/" + fileName;
                            using (var stream = System.IO.File.Create(path))
                            {
                                postFile.CopyTo(stream);
                                steerco.notulen = "upload/steerco/notulen/" + fileName;
                            }
                        }
                        i++;
                    }

                }


                Boolean t;
                if (steerco != null)
                {
                    _context.steerco.Add(steerco);
                    _context.SaveChanges();
                    t = true;
                }
                else
                {
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
		public IActionResult Update_Steerco(Steerco steerco)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Steerco obj = _context.steerco.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {
                    obj.id_project = Convert.ToInt64(Request.Form["id_project"]);
                    obj.judul = Request.Form["judul"].FirstOrDefault();
                    obj.tanggal = Convert.ToDateTime(Request.Form["tanggal"].FirstOrDefault()).Date;

                    if (Request.Form.Files.Count() != 0)
                    {

                        var i = 0;
                        foreach (var val in Request.Form.Files)
                        {
                            IFormFile postFile = Request.Form.Files[i];
                            string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + postFile.FileName;
                            if (postFile.Name == "materi")
                            {
                                string ExitingFile = environment.WebRootPath + "/" + Request.Form["materi_"].FirstOrDefault();
                                System.IO.File.Delete(ExitingFile);

                                string path = environment.WebRootPath + "/upload/steerco/materi/" + fileName;
                                using (var stream = System.IO.File.Create(path))
                                {
                                    postFile.CopyTo(stream);
                                    obj.materi = "upload/steerco/materi/" + fileName;
                                }
                            }
                            if (postFile.Name == "notulen")
                            {
                                string ExitingFile = environment.WebRootPath + "/" + Request.Form["notulen_"].FirstOrDefault();
                                System.IO.File.Delete(ExitingFile);

                                string path = environment.WebRootPath + "/upload/steerco/notulen/" + fileName;
                                using (var stream = System.IO.File.Create(path))
                                {
                                    postFile.CopyTo(stream);
                                    obj.notulen = "upload/steerco/notulen/" + fileName;
                                }
                            }
                            i++;
                        }
                    }
                    else
                    {
                        obj.materi = Request.Form["materi_"].FirstOrDefault();
                        obj.notulen = Request.Form["notulen_"].FirstOrDefault();
                    }
                    _context.SaveChanges();
                    return Json(new { result = true });
                }
                return Json(new { result = false });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult Deleted_Steerco(Steerco steerco)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                Steerco obj = _context.steerco.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
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
		public IActionResult Pir()
        {

            ViewBag.role = "PIR";
            if (Module.hasModule("PIR", HttpContext.Session))
            {
                ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == 1).ToList();
                return View();
            }
            else
            {
                return NotFound();
            }
        }

		[AuthorizedAction]
		public async Task<IActionResult> Get_Pir()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                // Skipping number of Rows count
                var start = Request.Form["start"].FirstOrDefault();
                // Paging Length 10, 20
                var length = Request.Form["length"].FirstOrDefault();
                // Sort Column Name
                var sortColumn = Request.Form["columns[" + Request.Form["order[1][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                // Sort Column Direction (asc, desc)
                var sortColumnDirection = Request.Form["order[1][dir]"].FirstOrDefault();
                // Search Value from (Search box)
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                // Paging Size (10, 20, 50, 100)
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;


                var project_filter = Request.Form["project"].FirstOrDefault();
                var project_rev = Request.Form["project_rev"].FirstOrDefault();


                var customerData = _context.pir.Include("users").Where(s => s.id_project == Convert.ToInt64(project_filter)).Where(s => s.deleted == 0).Select(a => new { id = a.id, id_project = a.id_project, tanggal = a.tanggal, judul = a.judul, materi = a.materi, notulen = a.notulen, nama_ = a.users.alias, created_date = a.created_date });

                // Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.judul.StartsWith(searchValue) || m.notulen.StartsWith(searchValue) || m.nama_.StartsWith(searchValue));
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
		[HttpPost]
        public IActionResult Create_Pir(IFormCollection formcollaction)
        {

            try
            {
                Pir pir = new Pir();
                pir.id_project = Convert.ToInt32(formcollaction["id_project"]);
                pir.judul = formcollaction["judul"];
                pir.created_by = HttpContext.Session.GetInt32("id");
                pir.tanggal = Convert.ToDateTime(formcollaction["tanggal"]);
                pir.created_date = DateTime.Now;
                pir.deleted = 0;

                if (Request.Form.Files.Count() != 0)
                {
                    var i = 0;
                    foreach (var val in Request.Form.Files)
                    {
                        IFormFile postFile = Request.Form.Files[i];
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + postFile.FileName;
                        if (postFile.Name == "materi")
                        {
                            string path = environment.WebRootPath + "/upload/pir/materi/" + fileName;
                            using (var stream = System.IO.File.Create(path))
                            {
                                postFile.CopyTo(stream);
                                pir.materi = "upload/pir/materi/" + fileName;
                            }
                        }
                        if (postFile.Name == "notulen")
                        {
                            string path = environment.WebRootPath + "/upload/pir/notulen/" + fileName;
                            using (var stream = System.IO.File.Create(path))
                            {
                                postFile.CopyTo(stream);
                                pir.notulen = "upload/pir/notulen/" + fileName;
                            }
                        }
                        i++;
                    }

                }


                Boolean t;
                if (pir != null)
                {
                    _context.pir.Add(pir);
                    _context.SaveChanges();
                    t = true;
                }
                else
                {
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
		public IActionResult Update_Pir(Pir pir)
        {

            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Pir obj = _context.pir.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {
                    obj.id_project = Convert.ToInt64(Request.Form["id_project"]);
                    obj.judul = Request.Form["judul"].FirstOrDefault();
                    obj.tanggal = Convert.ToDateTime(Request.Form["tanggal"].FirstOrDefault()).Date;

                    if (Request.Form.Files.Count() != 0)
                    {
                        var i = 0;
                        foreach (var val in Request.Form.Files)
                        {
                            IFormFile postFile = Request.Form.Files[i];
                            string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + postFile.FileName;
                            if (postFile.Name == "materi")
                            {
                                string ExitingFile = environment.WebRootPath + "/" + Request.Form["materi_"].FirstOrDefault();
                                System.IO.File.Delete(ExitingFile);

                                string path = environment.WebRootPath + "/upload/pir/materi/" + fileName;
                                using (var stream = System.IO.File.Create(path))
                                {
                                    postFile.CopyTo(stream);
                                    obj.materi = "upload/pir/materi/" + fileName;
                                }
                            }
                            if (postFile.Name == "notulen")
                            {
                                string ExitingFile = environment.WebRootPath + "/" + Request.Form["notulen_"].FirstOrDefault();
                                System.IO.File.Delete(ExitingFile);

                                string path = environment.WebRootPath + "/upload/pir/notulen/" + fileName;
                                using (var stream = System.IO.File.Create(path))
                                {
                                    postFile.CopyTo(stream);
                                    obj.notulen = "upload/pir/notulen/" + fileName;
                                }
                            }
                            i++;
                        }
                    }
                    else
                    {
                        obj.materi = Request.Form["materi_"].FirstOrDefault();
                        obj.notulen = Request.Form["notulen_"].FirstOrDefault();
                    }
                    _context.SaveChanges();
                    return Json(new { result = true });
                }
                return Json(new { result = false });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult Deleted_Pir(Pir pir)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                Pir obj = _context.pir.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
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
		public IActionResult Project()
        {
            ViewBag.role = "PROJECT";
            if (Module.hasModule("PROJECT", HttpContext.Session))
            {
                ViewBag.plant = _context.plans.Where(p => p.deleted == 0).ToList();
                return View();
            }
            else
            {
                return NotFound();
            }
        }

		[AuthorizedAction]
		public async Task<IActionResult> Get_Project()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();

                var start = Request.Form["start"].FirstOrDefault();

                var length = Request.Form["length"].FirstOrDefault();

                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                var status = Request.Form["status"].FirstOrDefault();
                var taoh = Request.Form["taoh"].FirstOrDefault();

                var customerData = _context.project.Where(s => s.deleted == 0).Select(a => new { projectNo = a.projectNo, description = a.description, revision = a.revision, month = a.month, year = a.year, active = a.active, deleted = a.deleted, updated = a.updated, id = a.id, tglTA = a.tglTA, tglSelesaiTA = a.tglSelesaiTA, deletedBy = a.deletedBy, createdDate = a.createdDate, lastModify = a.lastModify, modifyBy = a.modifyBy, plansID = a.plansID, durasiTABrick = a.durasiTABrick, finalDate = a.finalDate, additional1Date = a.additional1Date, additional2Date = a.additional2Date, taoh = a.taoh });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.description.Contains(searchValue) || b.revision.Contains(searchValue));
                }


                if (!string.IsNullOrEmpty(status))
                {
                    customerData = customerData.Where(b => b.active == Convert.ToInt32(status));
                }

                if (!string.IsNullOrEmpty(taoh))
                {
                    customerData = customerData.Where(b => b.taoh == taoh);
                }

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
		public IActionResult Create_Project(IFormCollection formcollaction)
        {
            try
            {
                string date = formcollaction["year"];

                Project project = new Project();
                project.month = date.Split('-')[1].ToString();
                project.year = date.Split('-')[0].ToString();
                project.plansID = Convert.ToInt64(formcollaction["plant"]);
                project.description = formcollaction["description"];
                project.revision = formcollaction["revision"];
                project.tglTA = Convert.ToDateTime(formcollaction["execution_date"]).Date;
                project.tglSelesaiTA = Convert.ToDateTime(formcollaction["finish_date"]).Date;
                project.durasiTABrick = Convert.ToInt32(formcollaction["durasiTABrick"]);
                project.taoh = formcollaction["section"];
                project.projectNo = formcollaction["kode_plant"] + project.month + project.year;
                project.active = 1;
                project.createdBy = HttpContext.Session.GetInt32("id");
                project.createdDate = DateTime.Now;
                project.deleted = 0;

                Boolean t;
                if (project != null)
                {
                    _context.project.Add(project);
                    _context.SaveChanges();
                    t = true;
                }
                else
                {
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
		public IActionResult Update_Project(Project project)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Project obj = _context.project.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {
                    string date = Request.Form["year"];

                    obj.description = Request.Form["description"].FirstOrDefault();
                    obj.month = date.Split('-')[1].ToString();
                    obj.year = date.Split('-')[0].ToString();
                    obj.updated = 1;
                    obj.tglTA = Convert.ToDateTime(Request.Form["execution_date"].FirstOrDefault()).Date;
                    obj.tglSelesaiTA = Convert.ToDateTime(Request.Form["finish_date"].FirstOrDefault()).Date;
                    obj.lastModify = DateTime.Now;
                    obj.modifyBy = HttpContext.Session.GetInt32("id");
                    obj.plansID = Convert.ToInt64(Request.Form["plant"].FirstOrDefault());
                    obj.durasiTABrick = Convert.ToInt32(Request.Form["durasiTABrick"].FirstOrDefault());
                    obj.taoh = Request.Form["section"].FirstOrDefault();
                    _context.SaveChanges();
                    return Json(new { result = true });
                }
                return Json(new { result = false });
            }
            catch
            {
                throw;
            }
        }


		[AuthorizedAction]
		public IActionResult Delete_Project(Project project)

        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                Project obj = _context.project.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
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
        public IActionResult StatusProject(Project project)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                Project obj = _context.project.Where(p => p.id == id).FirstOrDefault();
                if (obj != null)
                {
                    if(obj.active == 1)
                    {
                        obj.active = 0;
                    }
                    else
                    {
                        obj.active = 1;
                    }


                    _context.SaveChanges();

                    return Json(new { title = "Sukses!", icon = "success", status = "Berhasil Merubah Status" });
                }
                return Json(new { title = "Maaf!", icon = "error", status = "Tidak Dapat di Hapus!, Silahkan Hubungi Administrator " });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult Equipments()
        {
            ViewBag.role = "MANAGE_EQUIPMENT";
            if (Module.hasModule("MANAGE_EQUIPMENT", HttpContext.Session))
            {
                ViewBag.weight = _context.equipments.Where(p => p.weight_unit != "").Where(p => p.deleted == 0).GroupBy(p => new { p.weight_unit }).Select(p => new { weight_unit = p.Key.weight_unit }).ToList();
                ViewBag.planner_group = _context.equipments.Where(p => p.planner_group != "").Where(p => p.deleted == 0).GroupBy(p => new { p.planner_group }).Select(p => new { planner_group = p.Key.planner_group }).ToList();
                ViewBag.main_work_center = _context.equipments.Where(p => p.main_work_center != "").Where(p => p.deleted == 0).GroupBy(p => new { p.main_work_center }).Select(p => new { main_work_center = p.Key.main_work_center }).ToList();
                ViewBag.location = _context.equipments.Where(p => p.location != "").Where(p => p.deleted == 0).GroupBy(p => new { p.location }).Select(p => new { location = p.Key.location }).ToList();
                ViewBag.cost_center = _context.equipments.Where(p => p.cost_center != "").Where(p => p.deleted == 0).GroupBy(p => new { p.cost_center }).Select(p => new { cost_center = p.Key.cost_center }).ToList();
                ViewBag.wbs_element = _context.equipments.Where(p => p.WBS_element != "").Where(p => p.deleted == 0).GroupBy(p => new { p.WBS_element }).Select(p => new { WBS_element = p.Key.WBS_element }).ToList();
                ViewBag.manufacturer = _context.equipments.Where(p => p.manufacturer != "").Where(p => p.deleted == 0).GroupBy(p => new { p.manufacturer }).Select(p => new { manufacturer = p.Key.manufacturer }).ToList();
                ViewBag.funcLocID = _context.equipments.Where(p => p.funcLocID != "").Where(p => p.deleted == 0).GroupBy(p => new { p.funcLocID }).Select(p => new { funcLocID = p.Key.funcLocID }).ToList();
                ViewBag.craft = _context.equipments.Where(p => p.craft != "").Where(p => p.deleted == 0).GroupBy(p => new { p.craft }).Select(p => new { craft = p.Key.craft }).ToList();
                ViewBag.unit_proses = _context.unitProses.Where(p => p.deleted == 0).ToList();

                return View();
            }
            else
            {
                return NotFound();
            }

            
        }

		[AuthorizedAction]
		public async Task<IActionResult> Get_Equipments()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                // Skipping number of Rows count
                var start = Request.Form["start"].FirstOrDefault();
                // Paging Length 10, 20
                var length = Request.Form["length"].FirstOrDefault();
                // Sort Column Name
                var sortColumn = Request.Form["columns[" + Request.Form["order[1][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                // Sort Column Direction (asc, desc)
                var sortColumnDirection = Request.Form["order[1][dir]"].FirstOrDefault();
                // Search Value from (Search box)
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                // Paging Size (10, 20, 50, 100)
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;


                var customerData = _context.equipments.Include("users").Where(s => s.deleted == 0)
                    .Select(a => new
                    {
                        id = a.id,
                        eqTagNo = a.eqTagNo,
                        eqDesc = a.eqDesc,
                        funcLocID = a.funcLocID,
                        weight = a.weight,
                        weight_unit = a.weight_unit,
                        size = a.size,
                        start_up_date = a.start_up_date,
                        acquisition_value = a.acquisition_value,
                        currency_key = a.currency_key,
                        acquisition_date = a.acquisition_date,
                        planning_plant = a.planning_plant,
                        planner_group = a.planner_group,
                        main_work_center = a.main_work_center,
                        catalog_profile = a.catalog_profile,
                        maint_plant = a.maint_plant,
                        location = a.location,
                        plant_section = a.plant_section,
                        main_asset_no = a.main_asset_no,
                        asset_sub_no = a.asset_sub_no,
                        cost_center = a.cost_center,
                        WBS_element = a.WBS_element,
                        Position = a.Position,
                        tin = a.tin,
                        manufacturer = a.manufacturer,
                        model = a.model,
                        part_no = a.part_no,
                        serial_no = a.serial_no,
                        eqp_cat = a.eqp_cat,
                        date_valid = a.date_valid,
                        object_type = a.object_type,
                        country_of_manuf = a.country_of_manuf,
                        year_of_const = a.year_of_const,
                        month_of_const = a.month_of_const,
                        plant_main_work_center = a.plant_main_work_center,
                        const_type = a.const_type,
                        permit_assign = a.permit_assign,
                        Criticallity = a.Criticallity,
                        Remark = a.Remark,
                        unitProses = a.unitProses,
                        createdBy = a.users.name,
                        dateCreated = a.dateCreated,
                        updated = a.updated,
                        updatedBy = a.users.name,
                        deleted = a.deleted,
                        deletedBy = a.users.name,
                        responsibility = a.responsibility,
                        craft = a.craft,
                        eqGroupID = a.eqGroupID,
                        unitKilang = a.unitKilang,
                        catProf = a.catProf
                    });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.eqDesc.Contains(searchValue) || b.eqTagNo.Contains(searchValue) || b.funcLocID.Contains(searchValue));
                }

                //Console.WriteLine(customerData);
                // Total number of rows count
                //Console.WriteLine(customerData);
                recordsTotal = customerData.Count();

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
		public IActionResult Create_Equipments(IFormCollection formcollaction)
        {
            try
            {
                int cek = _context.equipments.Where(p => p.eqTagNo == Request.Form["eqtagno"].FirstOrDefault()).Count();

                Boolean c;
                Boolean t;
                if (cek == 0)
                {
                    c = true;
                    Equipments equipments = new Equipments();
                    equipments.eqTagNo = Request.Form["eqtagno"].FirstOrDefault();
                    equipments.eqDesc = formcollaction["description"];
                    equipments.funcLocID = formcollaction["func_location"];
                    equipments.weight = formcollaction["weight"];
                    equipments.weight_unit = formcollaction["jenis_weight"];
                    equipments.size = formcollaction["size"];
                    equipments.start_up_date = formcollaction["start_up_date"];
                    equipments.currency_key = formcollaction["currency_key"];
                    equipments.acquisition_value = formcollaction["acquisition_value"];
                    equipments.acquisition_date = formcollaction["date_acquisition"];
                    equipments.planning_plant = formcollaction["planning_plant"];
                    equipments.planner_group = formcollaction["planning_group"];
                    equipments.main_work_center = formcollaction["main_work_center"];
                    equipments.catalog_profile = (formcollaction["catalog_profile"] != "") ? Convert.ToInt32(formcollaction["catalog_profile"]) : null;
                    equipments.maint_plant = formcollaction["main_plant"];
                    equipments.location = formcollaction["location"];
                    equipments.plant_section = formcollaction["plant_section"];
                    equipments.main_asset_no = formcollaction["main_asset_no"];
                    equipments.asset_sub_no = formcollaction["asset_sub_no"];
                    equipments.WBS_element = formcollaction["wbsElement"];
                    equipments.Position = formcollaction["position"];
                    equipments.tin = formcollaction["tin"];
                    equipments.manufacturer = formcollaction["manufacturer"];
                    equipments.model = formcollaction["model"];
                    equipments.part_no = formcollaction["part_no"];
                    equipments.serial_no = formcollaction["serial_no"];
                    equipments.eqp_cat = formcollaction["equipment_category"];
                    equipments.date_valid = formcollaction["date_validation"];
                    equipments.object_type = formcollaction["object_type"];
                    equipments.craft = formcollaction["craft"];
                    equipments.country_of_manuf = formcollaction["country_of_manufacture"];
                    equipments.unitProses = formcollaction["unit_proses"];
                    equipments.year_of_const = formcollaction["year_const"];
                    equipments.unitKilang = formcollaction["unit_kilang"];
                    equipments.month_of_const = formcollaction["month_const"];
                    equipments.plant_main_work_center = formcollaction["plant_main_work_center"];
                    equipments.const_type = formcollaction["const_type"];
                    equipments.cost_center = formcollaction["cost_center"];
                    equipments.permit_assign = formcollaction["premit_assign"];
                    equipments.Criticallity = formcollaction["critical"];
                    equipments.Remark = formcollaction["remark"];
                    equipments.deleted = 0;
                    equipments.createdBy = HttpContext.Session.GetInt32("id"); 
                    equipments.dateCreated = DateTime.Now;


                    if (equipments != null)
                    {
                        _context.equipments.Add(equipments);
                        _context.SaveChanges();
                        t = true;
                    }
                    else
                    {
                        t = false;
                    }

                    return Json(new { result = t, cek = c });
                }
                else
                {
                    c = false;
                    return Json(new { cek = c });
                }




            }
            catch (Exception)
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult Update_Equipments(Equipments equipments)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Equipments obj = _context.equipments.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {
                    obj.eqTagNo = Request.Form["eqtagno"].FirstOrDefault();
                    obj.eqDesc = Request.Form["description"].FirstOrDefault();
                    obj.funcLocID = Request.Form["func_location"].FirstOrDefault();
                    obj.weight = Request.Form["weight"].FirstOrDefault();
                    obj.weight_unit = Request.Form["jenis_weight"].FirstOrDefault();
                    obj.size = Request.Form["size"].FirstOrDefault();
                    obj.start_up_date = Request.Form["start_up_date"].FirstOrDefault();
                    obj.currency_key = Request.Form["currency_key"].FirstOrDefault();
                    obj.acquisition_value = Request.Form["acquisition_value"].FirstOrDefault();
                    obj.acquisition_date = Request.Form["date_acquisition"].FirstOrDefault();
                    obj.planning_plant = Request.Form["planning_plant"].FirstOrDefault();
                    obj.planner_group = Request.Form["planning_group"].FirstOrDefault();
                    obj.main_work_center = Request.Form["main_work_center"].FirstOrDefault();
                    obj.catalog_profile = (Request.Form["catalog_profile"].FirstOrDefault() != "") ? Convert.ToInt32(Request.Form["catalog_profile"].FirstOrDefault()) : null;
                    obj.maint_plant = Request.Form["main_plant"].FirstOrDefault();
                    obj.location = Request.Form["location"].FirstOrDefault();
                    obj.plant_section = Request.Form["plant_section"].FirstOrDefault();
                    obj.main_asset_no = Request.Form["main_asset_no"].FirstOrDefault();
                    obj.asset_sub_no = Request.Form["asset_sub_no"].FirstOrDefault();
                    obj.WBS_element = Request.Form["wbsElement"].FirstOrDefault();
                    obj.Position = Request.Form["position"].FirstOrDefault();
                    obj.tin = Request.Form["tin"].FirstOrDefault();
                    obj.manufacturer = Request.Form["manufacturer"].FirstOrDefault();
                    obj.model = Request.Form["model"].FirstOrDefault();
                    obj.part_no = Request.Form["part_no"].FirstOrDefault();
                    obj.serial_no = Request.Form["serial_no"].FirstOrDefault();
                    obj.eqp_cat = Request.Form["equipment_category"].FirstOrDefault();
                    obj.date_valid = Request.Form["date_validation"].FirstOrDefault();
                    obj.object_type = Request.Form["object_type"].FirstOrDefault();
                    obj.craft = Request.Form["craft"].FirstOrDefault();
                    obj.country_of_manuf = Request.Form["country_of_manufacture"].FirstOrDefault();
                    obj.unitProses = Request.Form["unit_proses"].FirstOrDefault();
                    obj.year_of_const = Request.Form["year_const"].FirstOrDefault();
                    obj.unitKilang = Request.Form["unit_kilang"].FirstOrDefault();
                    obj.month_of_const = Request.Form["month_const"].FirstOrDefault();
                    obj.plant_main_work_center = Request.Form["plant_main_work_center"].FirstOrDefault();
                    obj.const_type = Request.Form["const_type"].FirstOrDefault();
                    obj.permit_assign = Request.Form["premit_assign"].FirstOrDefault();
                    obj.cost_center = Request.Form["cost_center"].FirstOrDefault();
                    obj.Criticallity = Request.Form["critical"].FirstOrDefault();
                    obj.Remark = Request.Form["remark"].FirstOrDefault();
                    obj.updatedBy = HttpContext.Session.GetInt32("id");

                    _context.SaveChanges();
                    return Json(new { result = true, cek = true });
                }
                return Json(new { result = false, cek = true });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult Deleted_Equipments(Equipments equipments)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                Equipments obj = _context.equipments.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {
                    obj.deletedBy = HttpContext.Session.GetInt32("id");
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
		public IActionResult CatalogProfile()
        {
            ViewBag.role = "CATALOG_PROFILE";
            if (Module.hasModule("CATALOG_PROFILE", HttpContext.Session))
            {
                //ViewBag.project = _context.project.Where(p => p.deleted == 0).ToList();
                ViewBag.disiplin_ = _context.disiplins.Where(p => p.deleted == 0).ToList();
                ViewBag.catprof = _context.catalogProfile.Where(p => p.deleted == 0).GroupBy(p => new { p.equipment_class }).Select(p => new { equipment_class = p.Key.equipment_class }).ToList();


                return View();
            }
            else
            {
                return NotFound();
            }
            
        }

		[AuthorizedAction]
		public async Task<IActionResult> Get_Catalog_Profile()
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

                var customerData = (from c in _context.catalogProfile
                                    join d in _context.disiplins on c.disiplin equals d.inisial
                                    select new
                                    {
                                        id = c.id,
                                        code = c.code,
                                        equipment_class = c.equipment_class,
                                        equipment_group = c.equipment_group,
                                        disiplin = c.disiplin,
                                        long_description = c.long_description,
                                        disiplins = d.disiplins,
                                        deleted = c.deleted
                                    });

                customerData = customerData.Where(s => s.deleted == 0);

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.long_description.Contains(searchValue) || b.equipment_class.Contains(searchValue) || b.equipment_group.Contains(searchValue));
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
		public IActionResult Create_Catalog_Profile(IFormCollection formcollaction)
        {
            try
            {
                CatalogProfile catalogProfile = new CatalogProfile();
                catalogProfile.code = Convert.ToInt32(formcollaction["code"]);
                catalogProfile.disiplin = formcollaction["disiplin"];
                catalogProfile.equipment_class = formcollaction["equipment_class"];
                catalogProfile.equipment_group = formcollaction["equipment_group"];
                catalogProfile.long_description = formcollaction["long_description"];
                catalogProfile.created_date = DateTime.Now;
                catalogProfile.createdBy = HttpContext.Session.GetInt32("id");
                catalogProfile.deleted = 0;

                Boolean t;
                if (catalogProfile != null)
                {
                    _context.catalogProfile.Add(catalogProfile);
                    _context.SaveChanges();
                    t = true;
                }
                else
                {
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
		public IActionResult Update_Catalog_Profile(CatalogProfile catalogProfile)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                CatalogProfile obj = _context.catalogProfile.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {
                    obj.code = Convert.ToInt32(Request.Form["code"].FirstOrDefault());
                    obj.disiplin = Request.Form["disiplin"].FirstOrDefault();
                    obj.equipment_class = Request.Form["equipment_class"].FirstOrDefault();
                    obj.equipment_group = Request.Form["equipment_group"].FirstOrDefault();
                    obj.long_description = Request.Form["long_description"].FirstOrDefault();
                    obj.lastmodify = DateTime.Now;
                    obj.modifyBy = HttpContext.Session.GetInt32("id");

                    _context.SaveChanges();
                    return Json(new { result = true });
                }
                return Json(new { result = false });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult Deleted_Catalog_Profile(CatalogProfile catalogProfile)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                CatalogProfile obj = _context.catalogProfile.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {
                    obj.deletedBy = HttpContext.Session.GetInt32("id");
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
		public IActionResult Memo()
        {
            ViewBag.role = "REQUEST_MEMO";
            if (Module.hasModule("REQUEST_MEMO", HttpContext.Session))
            {
                ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == 1).ToList();
                ViewBag.requestors = _context.requestors.Where(p => p.deleted == 0).ToList();
                return View();
            }
            else
            {
                return NotFound();
            }
           
        }

		[AuthorizedAction]
		public async Task<IActionResult> Get_Memo()
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

                var project = Request.Form["project"].FirstOrDefault();
                var memo = Request.Form["memo"].FirstOrDefault();

                var customerData = (from a in _context.memo
                                    join p in _context.project on a.projectID equals p.id
                                    join s in _context.users on a.createdBy equals s.id
                                    join q in _context.requestors on a.requestor equals q.id
                                    select new
                                    {
                                        id = a.id,
                                        projectID = a.projectID,
                                        projectName = a.project.description,
                                        reqNo = a.reqNo,
                                        reqDate = a.reqDate,
                                        reqDesc = a.reqDesc,
                                        reqYear = a.reqYear,
                                        attach = a.attach,
                                        requestorName = a.requestors.name,
                                        showing = a.showing,
                                        deleted = a.deleted,
                                        deletedBy = a.users.name,
                                        updated = a.updated,
                                        updatedBy = a.users.name,
                                        createBy = a.users.alias,
                                        dateCreated = a.dateCreated
                                    });


                customerData = customerData.Where(p => p.deleted == 0);

                if (project != "")
                {
                    customerData = customerData.Where(p => p.projectID == Convert.ToInt32(project));
                }

                if (memo != "")
                {
                    customerData = customerData.Where(p => p.reqNo.StartsWith(memo));
                }
                // Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.reqNo.Contains(searchValue) || m.reqDesc.Contains(searchValue) || m.createBy.Contains(searchValue) || m.requestorName.Contains(searchValue));
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
		public IActionResult Create_Memo(IFormCollection formcollaction)
        {
            try
            {
                Memo memo = new Memo();
                memo.projectID = Convert.ToInt32(formcollaction["projectID"]);
                memo.reqNo = formcollaction["reqNo"];
                memo.reqDesc = formcollaction["reqDesc"];
                memo.reqDate = Convert.ToDateTime(formcollaction["reqDate"]);
                memo.reqYear = DateTime.Parse(Request.Form["reqDate"].ToString()).Year.ToString();
                memo.requestor = Convert.ToInt32(formcollaction["requestor"]);
                memo.showing = Convert.ToInt32(formcollaction["showing"]);
                memo.createdBy = HttpContext.Session.GetInt32("id");
                memo.deleted = 0;
                memo.dateCreated = DateTime.Now;

                if (Request.Form.Files.Count() != 0)
                {
                    IFormFile postedFile = Request.Form.Files[0];
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + postedFile.FileName;
                    string path = environment.WebRootPath + "/upload/memo/" + fileName;
                    using (var stream = System.IO.File.Create(path))
                    {
                        postedFile.CopyTo(stream);
                        memo.attach = "upload/memo/" + fileName;
                    }

                }

                Boolean t;
                if (memo != null)
                {
                    _context.memo.Add(memo);
                    _context.SaveChanges();
                    t = true;
                }
                else
                {
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
		public IActionResult Update_Memo(Memo memo)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Memo obj = _context.memo.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {
                    obj.projectID = Convert.ToInt32(Request.Form["projectID"].FirstOrDefault());
                    obj.reqNo = Request.Form["reqNo"].FirstOrDefault();
                    obj.reqDesc = Request.Form["reqDesc"].FirstOrDefault();
                    obj.reqDate = Convert.ToDateTime(Request.Form["reqDate"].FirstOrDefault());
                    obj.requestor = Convert.ToInt32(Request.Form["requestor"].FirstOrDefault());
                   
                    if (Request.Form.Files.Count() != 0)
                    {
                        if (obj.attach != null)
                        {
                            string ExitingFile = environment.WebRootPath + "/" + obj.attach;
                            System.IO.File.Delete(ExitingFile);
                        }

                        IFormFile postedFile = Request.Form.Files[0];
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + postedFile.FileName;
                        string path = environment.WebRootPath + "/upload/memo/" + fileName;

                        using (var stream = System.IO.File.Create(path))
                        {
                            postedFile.CopyTo(stream);
                            obj.attach = "upload/memo/" + fileName;
                        }

                    }
                    else
                    {
                        obj.attach = Request.Form["file_"].FirstOrDefault();
                    }
                    obj.showing = Convert.ToInt32(Request.Form["showing"].FirstOrDefault());
                    obj.updated = 1;
                    obj.updatedBy = HttpContext.Session.GetInt32("id");
                    _context.SaveChanges();
                    return Json(new { Results = true });
                }
                return Json(new { Results = false });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult Deleted_Memo(Memo memo)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                Memo obj = _context.memo.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
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
		public IActionResult Requestor()
        {

            ViewBag.role = "REQUESTOR";
            if (Module.hasModule("REQUESTOR", HttpContext.Session))
            {
                return View();
            }
            else
            {
                return NotFound();
            }
        }

		[AuthorizedAction]
		public async Task<IActionResult> Get_Requestor()
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



                var customerData = _context.requestors.Where(s => s.deleted == 0).Select(a => new { id = a.id, fungsi = a.fungsi, name = a.name, description = a.description,dateCreated = a.dateCreated });


                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.name.Contains(searchValue) || b.description.Contains(searchValue));
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
		public IActionResult Create_Requestor(IFormCollection formcollaction)
        {
            try
            {
                Requestor requestor = new Requestor();

                requestor.name = formcollaction["name"];
                requestor.description = formcollaction["description"];
                requestor.deleted = 0;
                //requestor.updated = 0;
                requestor.createdBy = HttpContext.Session.GetInt32("id");
                requestor.dateCreated = DateTime.Now;

                Boolean t;
                if (requestor != null)
                {
                    _context.requestors.Add(requestor);
                    _context.SaveChanges();
                    t = true;
                }
                else
                {
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
		public IActionResult Update_Requestor(Requestor requestor)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Requestor obj = _context.requestors.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {
                    //obj.fungsi = Request.Form["fungsi"].FirstOrDefault();
                    obj.description = Request.Form["description"].FirstOrDefault();
                    obj.name = Request.Form["name"].FirstOrDefault();
                    obj.updated = 1;
                    obj.updatedBy = HttpContext.Session.GetInt32("id");

                    _context.SaveChanges();
                    return Json(new { result = true });
                }
                return Json(new { result = false });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult Deleted_Requestor(Requestor requestor)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                Requestor obj = _context.requestors.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {
                    obj.deleted = 1;
                    obj.deletedBy = HttpContext.Session.GetInt32("id");
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
		public IActionResult Unit()
        {
            ViewBag.role = "MANAGE_UNIT";
            if (Module.hasModule("MANAGE_UNIT", HttpContext.Session))
            {
                ViewBag.plans = _context.plans.Where(p => p.deleted == 0).ToList();
                ViewBag.unitProses = _context.unitProses.Where(p => p.deleted == 0).ToList();
                return View();
            }
            else
            {
                return NotFound();
            }
            
        }

		[AuthorizedAction]
		public async Task<IActionResult> Get_Unit()
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


                var customerData = _context.unit.Where(s => s.deleted == 0).Select(a => new { id = a.id, unitPlan = a.unitPlan, codeJob = a.codeJob, unitCode = a.unitCode, unitProses = a.unitProses, unitKilang = a.unitKilang, unitGroup = a.unitGroup, groupName = a.groupName, unitName = a.unitName });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.unitPlan.Contains(searchValue) || b.unitKilang.Contains(searchValue));
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
		public IActionResult Create_Unit(IFormCollection formcollaction)
        {
            try
            {
                Unit unit = new Unit();
                unit.unitPlan = formcollaction["unitPlan"];
                unit.codeJob = formcollaction["codeJob"];
                unit.unitCode = formcollaction["unitCode"];
                unit.unitProses = formcollaction["unitProses"];
                unit.unitKilang = formcollaction["unitKilang"];
                unit.unitGroup = formcollaction["unitGroup"];
                unit.groupName = formcollaction["groupName"];
                unit.unitName = formcollaction["unitName"];
                unit.createdBy = HttpContext.Session.GetInt32("id");
                unit.deleted = 0;
                unit.dateCreated = DateTime.Now;

                Boolean t;
                if (unit != null)
                {
                    _context.unit.Add(unit);
                    _context.SaveChanges();
                    t = true;
                }
                else
                {
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
		public IActionResult Update_Unit(Unit unit)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Unit obj = _context.unit.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {
                    obj.unitPlan = Request.Form["unitPlan"].FirstOrDefault();
                    obj.codeJob = Request.Form["codeJob"].FirstOrDefault();
                    obj.unitCode = Request.Form["unitCode"].FirstOrDefault();
                    obj.unitProses = Request.Form["unitProses"].FirstOrDefault();
                    obj.unitKilang = Request.Form["unitKilang"].FirstOrDefault();
                    obj.unitGroup = Request.Form["unitGroup"].FirstOrDefault();
                    obj.groupName = Request.Form["groupName"].FirstOrDefault();
                    obj.unitName = Request.Form["unitName"].FirstOrDefault();
                    obj.updated = 1;
                    obj.updatedBy = HttpContext.Session.GetInt32("id");
                    _context.SaveChanges();
                    return Json(new { result = true });
                }
                return Json(new { result = false });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult Deleted_Unit(Unit unit)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                Unit obj = _context.unit.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
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
		public IActionResult getUnitKilang()
        {
            var unitCode = Request.Form["unitCode"].FirstOrDefault();
            var data = _context.unit.Where(p => p.unitCode == unitCode).Where(p => p.deleted == 0).OrderBy(p => p.unitKilang).ToList();
            var select = "<option value=''>Select Unit</option>";
            foreach (var val in data)
            {
                select += "<option value='" + val.unitKilang + "' data-id='" + val.id + "' data-codeJob='" + val.codeJob + "'>" + val.unitKilang + "</option>";
            }

            return Ok(select);
        }

		[AuthorizedAction]
		public IActionResult Bom()
        {
            ViewBag.role = "BOM";
            if (Module.hasModule("BOM", HttpContext.Session))
            {
                ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == 1).ToList();
                ViewBag.disiplin = _context.disiplins.Where(p => p.deleted != 1).ToList();
                ViewBag.equipment = _context.equipments.Where(p => p.deleted != 1).ToList();

                return View();
            }
            else
            {
                return NotFound();
            }
           
        }

		[AuthorizedAction]
		public async Task<IActionResult> Get_Boms()
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

                var project_filter = Request.Form["project"].FirstOrDefault();
                var disiplin_filter = Request.Form["disiplin"].FirstOrDefault();

                var sql = @"SELECT bom.id, max(bom.no_wo) as no_wo, max(bom.tag_no) as tag_no, max(bom.id_project) as id_project, max(bom.disiplin) as disiplin, max(bom.created_date) as created_date,max(bom.created_by) as created_by ,max(users.alias) as alias, max(bom.last_modify) as last_modify, max(bom.modify_by) as modify_by, max(bom.deleted) as deleted, string_agg(bom_files.files, ';') as file_bom
                            FROM Mystap.dbo.bom
                            LEFT JOIN Mystap.dbo.bom_files on bom.id = bom_files.id_bom
                            LEFT JOIN Mystap.dbo.users on bom.created_by = users.id
                            LEFT JOIN Mystap.dbo.project on bom.id_project = project.id
                            where bom.deleted = 0
                            group by bom.id";

                var c = FormattableStringFactory.Create(sql);
                var customerData = _context.viewBom.FromSql(c);

                if (!string.IsNullOrEmpty(project_filter))
                {
                    customerData = customerData.Where(s => s.id_project == Convert.ToInt64(project_filter));
                }

                if (!string.IsNullOrEmpty(disiplin_filter))
                {
                    customerData = customerData.Where(s => s.disiplin == disiplin_filter);
                }

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.tag_no.Contains(searchValue) || b.no_wo.Contains(searchValue) || b.disiplin.Contains(searchValue));
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
		public IActionResult Create_Bom(IFormCollection formcollaction)
        {
            Bom bom = new Bom();
            bom.id_project = Convert.ToInt32(formcollaction["id_project"]);
            bom.tag_no = formcollaction["tag_no"];
            bom.no_wo = formcollaction["no_wo"];
            bom.disiplin = formcollaction["disiplin"];
            bom.created_date = DateTime.Now;
            bom.created_by = HttpContext.Session.GetInt32("id");
            bom.deleted = 0;
            Boolean t;
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.bom.Add(bom);
                _context.SaveChanges();

                var id_bom = bom.id;
                var files = formcollaction["attach"];
                if (Request.Form.Files.Count() != 0)
                {
                    if (files.Count > 0)
                    {
                        var cek = _context.bomFiles.Where(p => p.id_bom == bom.id).FirstOrDefault();
                        if (cek != null)
                        {
                            _context.bomFiles.Where(p => p.id_bom == bom.id).ExecuteDelete();
                        }
                        foreach (var val in files)
                        {
                            BomFiles bomFiles = new BomFiles();
                            IFormFile postedFile = Request.Form.Files[0];
                            string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + postedFile.FileName;
                            string path = environment.WebRootPath + "/upload/bom/" + fileName;
                            using (var stream = System.IO.File.Create(path))
                            {
                                postedFile.CopyTo(stream);
                                bomFiles.files = "upload/bom/" + fileName;
                                
                            }
                            bomFiles.id_bom = (int)id_bom;
                            _context.bomFiles.Add(bomFiles);
                            _context.SaveChanges();
                        }
                    }
                }

                t = true;
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                t = false;
            }

            return Json(new { result = t });
        }

		[AuthorizedAction]
		public IActionResult Update_Bom(Bom bom)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Bom obj = _context.bom.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {
                    obj.id_project = Convert.ToInt32(Request.Form["id_project"].FirstOrDefault());
                    obj.tag_no = Request.Form["tag_no"].FirstOrDefault();
                    obj.no_wo = Request.Form["no_wo"].FirstOrDefault();
                    obj.disiplin = Request.Form["disiplin"].FirstOrDefault();
                    obj.last_modify = DateTime.Now;
                    obj.modify_by = HttpContext.Session.GetInt32("id");
                    Boolean t;
                    using var transaction = _context.Database.BeginTransaction();
                    try
                    {
                        _context.SaveChanges();

                        var id_bom = bom.id;
                        var files = Request.Form["attach"];
                        if (Request.Form.Files.Count() != 0)
                        {
                            if (files.Count > 0)
                            {
                                var cek = _context.bomFiles.Where(p => p.id_bom == bom.id).FirstOrDefault();
                                if (cek != null)
                                {
                                    _context.bomFiles.Where(p => p.id_bom == bom.id).ExecuteDelete();
                                }
                                foreach (var val in files)
                                {
                                    BomFiles bomFiles = new BomFiles();
                                    IFormFile postedFile = Request.Form.Files[0];
                                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + postedFile.FileName;
                                    string path = environment.WebRootPath + "/upload/bom/" + fileName;
                                    using (var stream = System.IO.File.Create(path))
                                    {
                                        postedFile.CopyTo(stream);
                                        bomFiles.files = "upload/bom/" + fileName;

                                    }
                                    bomFiles.id_bom = (int)id_bom;
                                    _context.bomFiles.Add(bomFiles);
                                    _context.SaveChanges();
                                }
                            }
                        }

                        t = true;
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        t = false;
                    }

                    return Json(new { result = t });
                }
                return Json(new { result = false });
            }
            catch
            {
                throw;
            }
        }

        [AuthorizedAction]
        public IActionResult Deleted_Bom(Bom bom)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                Bom obj = _context.bom.Where(p => p.id == id).FirstOrDefault();

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
        public IActionResult Profile_()
        {

            return View();
        }

        
        [AuthorizedAction]
        [HttpPost]
        public IActionResult Profile(Users users)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Users obj =  _context.users.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {
                    obj.username = Request.Form["username"].FirstOrDefault();
                    obj.email = Request.Form["email"].FirstOrDefault();
                    obj.foto = Request.Form["foto"].FirstOrDefault();
                    if (obj.password != null)
                    {
                        obj.password = Request.Form["password"].FirstOrDefault();
                    }
                     _context.SaveChanges();
                    return Json(new { result = true });

                }
                return Json(new { result = false });
            }
            catch
            {
                throw;
            }
        }

    }

}