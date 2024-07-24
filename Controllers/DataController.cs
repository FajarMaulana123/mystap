using Azure.Core;
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
            ViewBag.project = _context.project.Where(p => p.deleted == 0).ToList();
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
                //Console.WriteLine(customerData);
                recordsTotal = customerData.Count();
                var data = customerData.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFilter = recordsTotal, recordsTotal = recordsTotal, data = data });
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

                var customerData = _context.project./*Include("users").*/Where(s => s.deleted == 0).Select(a => new { projectNo = a.projectNo, description = a.description, revision = a.revision, month = a.month, year = a.year, active = a.active, deleted = a.deleted, updated = a.updated, id = a.id, tglTA = a.tglTA, tglSelesaiTA = a.tglSelesaiTA, deletedBy = a.deletedBy, createdDate = a.createdDate, lastModify = a.lastModify, modifyBy = a.modifyBy, plansID = a.plansID, durasiTABrick = a.durasiTABrick, finalDate = a.finalDate, additional1Date = a.additional1Date, additional2Date = a.additional2Date, taoh = a.taoh });

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
                project.tglTA = Convert.ToDateTime(formcollaction["tglTA"]);
                project.tglSelesaiTA = Convert.ToDateTime(formcollaction["tglSelesaiTA"]);
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
                    obj.tglTA = Convert.ToDateTime(Request.Form["tglTA"].FirstOrDefault());
                    obj.tglSelesaiTA = Convert.ToDateTime(Request.Form["tglSelesaiTA"].FirstOrDefault());
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

        public IActionResult Equipments()
        {

            ViewBag.funlocID = _context.equipments.Where(p => p.funcLocID != "").GroupBy(p => p.funcLocID).Select(p => new{funcLocID = p.Key,Entity = p.FirstOrDefault()}).OrderBy(p => p.funcLocID).ToList();
            ViewBag.project = _context.project.Where(p => p.deleted == 0).ToList();
            return View();
        }
        public IActionResult Get_Equipments()
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


                var customerData = _context.equipments.Include("users").Where(s => s.deleted == 0).Select(a => new { id = a.id, eqTagNo = a.eqTagNo, eqDesc = a.eqDesc, funcLocID = a.funcLocID, weight = a.weight, weight_unit = a.weight_unit, size = a.size, start_up_date = a.start_up_date, acquisition_value = a.acquisition_value, planning_plant = a.planning_plant, planner_group = a.planner_group, main_work_center = a.main_work_center, catalog_profile = a.catalog_profile, maint_plant = a.maint_plant, location = a.location, plant_section = a.plant_section, main_asset_no = a.main_asset_no, asset_sub_no = a.asset_sub_no, cost_center = a.cost_center, WBS_element = a.WBS_element, Position = a.Position, tin = a.tin, manufacturer = a.manufacturer, model = a.model,part_no = a.part_no, serial_no = a.serial_no, eqp_cat = a.eqp_cat, date_valid = a.date_valid, object_type = a.object_type, country_of_manuf = a.country_of_manuf, year_of_const = a.year_of_const, month_of_const = a.month_of_const, plant_main_work_center = a.plant_main_work_center, const_type = a.const_type, permit_assign = a.permit_assign, Criticallity = a.Criticallity, Remark = a.Remark, unitProses = a.unitProses, createdBy = a.users.name, dateCreated = a.dateCreated, updated = a.updated, updatedBy = a.users.name, deleted = a.deleted, deletedBy = a.users.name, responsibility = a.responsibility, craft = a.craft, eqGroupID = a.eqGroupID, unitKilang = a.unitKilang, catProf = a.catProf  });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.eqDesc.StartsWith(searchValue));
                }
                //Console.WriteLine(customerData);
                recordsTotal = customerData.Count();
                var data = customerData.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFilter = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IActionResult Create_Equipments(IFormCollection formcollaction)
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
        public IActionResult Update_Equipments(Rapat rapat)
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

        public IActionResult Deleted_Equipments(Rapat rapat)
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
        public IActionResult CatalogProfile()
        {

            
            ViewBag.project = _context.project.Where(p => p.deleted == 0).ToList();
            return View();
        }
        public IActionResult Get_Catalog_Profile()
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


                var customerData = _context.catalog_profile.Include("users").Where(s => s.deleted == "0").Select(a => new { id = a.id, code = a.code, equipment_class = a.equipment_class, equipment_group = a.equipment_group, disiplin = a.disiplin, long_description = a.long_description });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.long_description.StartsWith(searchValue));
                }
                //Console.WriteLine(customerData);
                recordsTotal = customerData.Count();
                var data = customerData.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFilter = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IActionResult Create_Catalog_Profile(IFormCollection formcollaction)
        {
            try
            {
                CatalogProfile catalog_profile = new CatalogProfile();
                catalog_profile.code =formcollaction["code"];
                catalog_profile.disiplin = formcollaction["disiplin"];
                catalog_profile.equipment_class = formcollaction["equipment_class"];
                catalog_profile.equipment_group = formcollaction["equipment_group"];
                catalog_profile.long_description = formcollaction["long_description"];
                catalog_profile.created_date = formcollaction["created_date"];
                catalog_profile.createdBy = formcollaction["disiplin"];
                catalog_profile.deleted = "0";

                Boolean t;
                if (catalog_profile != null)
                {
                    _context.catalog_profile.Add(catalog_profile);
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
        public IActionResult Update_Catalog_Profile(Rapat rapat)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                CatalogProfile obj = _context.catalog_profile.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {
                    obj.code = Request.Form["code"].FirstOrDefault();
                    obj.disiplin = Request.Form["disiplin"].FirstOrDefault();
                    obj.equipment_class = Request.Form["equipment_class"].FirstOrDefault();
                    obj.equipment_group = Request.Form["equipment_group"].FirstOrDefault();
                    obj.long_description = Request.Form["long_description"].FirstOrDefault();
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

        public IActionResult Deleted_Catalog_Profile(Rapat rapat)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                CatalogProfile obj = _context.catalog_profile.Where(p => p.id == id).FirstOrDefault();

                if (obj == null)
                {
                    obj.deleted = "1";
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