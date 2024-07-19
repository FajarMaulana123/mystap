using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mystap.Models;
using System.Data;
using System.Linq.Dynamic.Core;

namespace mystap.Controllers
{
    public class DataController : Controller
    {
        private readonly DatabaseContext _context;
        public DataController(DatabaseContext context)
        {
            _context = context;
        }
        public IActionResult Rapat()
        {
            return View();
        }
        public IActionResult Get_Rapat()
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

                var customerData = _context.rapat.Include("users").Where(s => s.deleted == 0).Select(a => new { id = a.id, id_project = a.id_project, tanggal = a.tanggal, judul = a.judul, materi = a.materi, notulen = a.notulen, name = a.users.name, created_date = a.created_date });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.judul.StartsWith(searchValue));
                }
                Console.WriteLine(customerData);
                recordsTotal = customerData.Count();
                var data = customerData.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw, recordFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IActionResult Create_Rapat(IFormCollection formcollaction)
        {
            try
            {
                Rapat rapat = new Rapat();
                rapat.id_project = "7";
                rapat.judul = formcollaction["judul"];
                rapat.materi = formcollaction["materi"];
                rapat.notulen = formcollaction["notulen"];
                rapat.created_by = 1;
                rapat.created_date = DateTime.Now;
                rapat.deleted = 0;

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
        public IActionResult Update_Rapat(Rapat rapat)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Rapat obj = _context.rapat.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {
                    obj.id_project = "7";
                    obj.judul = Request.Form["judul"].FirstOrDefault();
                    obj.materi = Request.Form["materi"].FirstOrDefault();
                    obj.notulen = Request.Form["notulen"].FirstOrDefault();
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

        public IActionResult Deleted_Rapat(Rapat rapat)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                Rapat obj = _context.rapat.Where(p => p.id == id).FirstOrDefault();

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

        public IActionResult Project()
        {
            return View();
        }
        public IActionResult Get_Project()
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

                var customerData = _context.project.Include("users").Where(s => s.deleted == 0).Select(a => new { projectNo = a.projectNo, description = a.description, revision = a.revision, month = a.month, year = a.year, active = a.active,deleted = a.deleted, updated = a.updated,id = a.id, tglTA = a.tglTA, tglSelesaiTA = a.tglSelesaiTA, deletedBy = a.deletedBy, name = a.users.name , createdDate = a.createdDate, lastModify = a.lastModify, modifyBy = a.modifyBy, plansID = a.plansID, durasiTABrick = a.durasiTABrick, finalDate = a.finalDate, additional1Date = a.additional1Date, additional2Date = a.additional2Date, taoh = a.taoh });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.description.StartsWith(searchValue));
                }
                Console.WriteLine(customerData);
                recordsTotal = customerData.Count();
                var data = customerData.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw, recordFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IActionResult Create_Project(IFormCollection formcollaction)
        {
            try
            {
                Project project = new Project();
                project.projectNo = "7";
                project.description = formcollaction["description"];
                project.revision = formcollaction["revision"];
                project.month = formcollaction["month"];
                project.year = formcollaction["year"];
                project.active = "1";
                project.deleted = 0;
                project.updated = 0;
                project.tglTA = DateTime.Now;
                project.tglSelesaiTA = DateTime.Now;
                project.deletedBy = 0;
                project.createdBy = 1;
                project.createdDate = DateTime.Now;
                project.lastModify = DateTime.Now;
                project.modifyBy = 0;
                project.plansID = 1;
                project.durasiTABrick = 11;
                project.finalDate = formcollaction["finalDate"];
                project.additional1Date = formcollaction["additional1Date"];
                project.additional2Date = formcollaction["additional2Date"];
                project.taoh = formcollaction["taoh"];



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
        public IActionResult Update_Project(Project project)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Project obj = _context.project.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {
                    obj.description = Request.Form["description"].FirstOrDefault();
                    obj.month = Request.Form["month"].FirstOrDefault();
                    obj.year = Request.Form["year"].FirstOrDefault();
                    obj.active = Request.Form["active"].FirstOrDefault();
                    obj.updated = 1;
                    obj.tglTA = DateTime.Now;
                    obj.tglSelesaiTA = DateTime.Now;
                    obj.lastModify = DateTime.Now;
                    obj.modifyBy = 1;
                    obj.plansID = 1;
                    obj.durasiTABrick = 11;
                    obj.finalDate = Request.Form["finalDate"].FirstOrDefault();
                    obj.additional1Date = Request.Form["additional1Date"].FirstOrDefault();
                    obj.additional2Date = Request.Form["additional2Date"].FirstOrDefault();
                    obj.taoh = Request.Form["taoh"].FirstOrDefault();
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

        public IActionResult Deleted_Project(Project project)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                Rapat obj = _context.rapat.Where(p => p.id == id).FirstOrDefault();

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
    }

}