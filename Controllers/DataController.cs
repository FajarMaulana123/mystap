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

            ViewBag.equipments = _context.equipments
                .Where(p => p.deleted == 0)
                .GroupBy(x => new { x.funcLocID, x.weight })
                .Select(z => new
                {
                    unitCode = z.Key.funcLocID,
                    unitProses = z.Key.weight

                })
                .ToList();
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
        /*public IActionResult Update_Equipments(Equipments equipments)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Equipments obj = _context.equipments.Where(p => p.id == id).FirstOrDefault();

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
        }*/

        public IActionResult Deleted_Equipments(Equipments equipments)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                Equipments obj = _context.equipments.Where(p => p.id == id).FirstOrDefault();

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


                var customerData = _context.catalogProfile.Include("users").Where(s => s.deleted == 0).Select(a => new { id = a.id, code = a.code, equipment_class = a.equipment_class, equipment_group = a.equipment_group, disiplin = a.disiplin, long_description = a.long_description });

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
                CatalogProfile catalogProfile = new CatalogProfile();
                catalogProfile.code = Convert.ToInt32(formcollaction["code"]);
                catalogProfile.disiplin = formcollaction["disiplin"];
                catalogProfile.equipment_class = formcollaction["equipment_class"];
                catalogProfile.equipment_group = formcollaction["equipment_group"];
                catalogProfile.long_description = formcollaction["long_description"];
                catalogProfile.created_date = formcollaction["created_date"];
                catalogProfile.createdBy = formcollaction["disiplin"];
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

        public IActionResult Deleted_Catalog_Profile(CatalogProfile catalogProfile)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                CatalogProfile obj = _context.catalogProfile.Where(p => p.id == id).FirstOrDefault();

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

        public IActionResult Memo()
        {

            ViewBag.project = _context.project.Where(p => p.deleted == 0).ToList();
            ViewBag.requestors = _context.requestors.Where(p => p.deleted == 0).ToList();
            return View();
        }
        public IActionResult Get_Memo()
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


                var customerData = _context.memo.Include("project").Include("requestors").Include("users").Where(s => s.deleted == 0).Select(a => new { id = a.id, projectName = a.project.description, reqNo = a.reqNo, reqDate = a.reqDate, reqDesc = a.reqDesc, reqYear = a.reqYear, attach = a.attach, requestorName = a.requestors.name , showing = a.showing, deleted = a.deleted, deletedBy = a.users.name, updated = a.updated, updatedBy = a.users.name, createBy = a.users.alias, dateCreated = a.dateCreated  });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.reqDesc.StartsWith(searchValue));
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
        public IActionResult Create_Memo(IFormCollection formcollaction)
        {
            try
            {
                Memo memo = new Memo();
                memo.projectID = Convert.ToInt32(formcollaction["projectID"]);
                memo.reqNo = formcollaction["reqNo"];
                memo.reqDesc = formcollaction["reqDesc"];
                memo.reqDate = formcollaction["reqDate"];
                memo.requestor = Convert.ToInt32(formcollaction["requestor"]);
                memo.attach = formcollaction["attach"];
                memo.showing = Convert.ToInt32(formcollaction["showing"]);
                memo.createdBy = 1;
                memo.deleted = 0;

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
                    obj.reqDate = Request.Form["reqDate"].FirstOrDefault();
                    obj.requestor = Convert.ToInt32(Request.Form["requestor"].FirstOrDefault());
                    obj.attach = Request.Form["attach"].FirstOrDefault();
                    obj.showing = Convert.ToInt32(Request.Form["showing"].FirstOrDefault());
                    obj.updated = 1;
                    obj.updatedBy = 1;
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

        public IActionResult Deleted_Memo(Memo memo)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                Memo obj = _context.memo.Where(p => p.id == id).FirstOrDefault();

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

        public IActionResult Requestor()
        {

            return View();
        }
        public IActionResult Get_Requestor()
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


                var customerData = _context.requestors.Include("users").Where(s => s.deleted == 0).Select(a => new { id = a.id, fungsi = a.fungsi, name = a.name, description = a.description,dateCreated = a.dateCreated });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.description.StartsWith(searchValue));
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
        public IActionResult Create_Requestor(IFormCollection formcollaction)
        {
            try
            {
                Requestor requestor = new Requestor();
                requestor.fungsi =formcollaction["fungsi"];
                requestor.name = formcollaction["name"];
                requestor.description = formcollaction["description"];
                requestor.deleted = 0;
                requestor.updated = 0;
                requestor.createdBy = 1;
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
        public IActionResult Update_Requestor(Requestor requestor)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Requestor obj = _context.requestors.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {
                    obj.fungsi = Request.Form["fungsi"].FirstOrDefault();
                    obj.description = Request.Form["description"].FirstOrDefault();
                    obj.name = Request.Form["name"].FirstOrDefault();
                    obj.updated = 1;
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

        public IActionResult Deleted_Requestor(Requestor requestor)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                Requestor obj = _context.requestors.Where(p => p.id == id).FirstOrDefault();

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

        public IActionResult Unit()
        {
            ViewBag.plans = _context.plans.Where(p => p.deleted == 0).ToList();
            ViewBag.unitProses = _context.unitProses.Where(p => p.deleted == 0).ToList();
            return View();
        }
        public IActionResult Get_Unit()
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


                var customerData = _context.unit.Where(s => s.deleted == 0).Select(a => new { id = a.id, unitPlan = a.unitPlan, codeJob = a.codeJob, unitCode = a.unitCode, unitProses = a.unitProses, unitKilang = a.unitKilang, unitGroup = a.unitGroup, groupName = a.groupName, unitName =a.unitName });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.unitPlan.StartsWith(searchValue));
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
                unit.createdBy = 1;
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
                    obj.updatedBy = 1;
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

        public IActionResult Deleted_Unit(Unit unit)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                Unit obj = _context.unit.Where(p => p.id == id).FirstOrDefault();

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