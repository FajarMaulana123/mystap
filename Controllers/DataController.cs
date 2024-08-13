﻿using Azure.Core;
using MessagePack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mystap.Models;
using System.Data;
using System.Globalization;
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

            ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == "1").ToList();

            return View();
        }
        public async Task<IActionResult>  Get_Rapat()
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
        public IActionResult Create_Rapat(IFormCollection formcollaction)
        {
            try
            {
                Rapat rapat = new Rapat();
                rapat.id_project = Convert.ToInt64(formcollaction["id_project"]);
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
                    obj.id_project = Convert.ToInt64(Request.Form["id_project"]);
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

        public IActionResult Steerco()
        {

            ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == "1").ToList();

            return View();
        }
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
        public IActionResult Create_Steerco(IFormCollection formcollaction)
        {
            try
            {
                Steerco steerco = new Steerco();
                steerco.id_project = Convert.ToInt64(formcollaction["id_project"]);
                steerco.judul = formcollaction["judul"];
                steerco.materi = formcollaction["materi"];
                steerco.notulen = formcollaction["notulen"];
                steerco.created_by = 1;
                steerco.created_date = DateTime.Now;
                steerco.deleted = 0;

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

        public IActionResult Deleted_Steerco(Steerco steerco)
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
            ViewBag.plant = _context.plans.Where(p => p.deleted == 0).ToList();
            return View();
        }
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
                    customerData = customerData.Where(b => b.description.StartsWith(searchValue));
                }

                if (!string.IsNullOrEmpty(status))
                {
                    customerData = customerData.Where(b => b.active == status);
                }

                if (!string.IsNullOrEmpty(taoh))
                {
                    customerData = customerData.Where(b => b.taoh == taoh);
                }

                recordsTotal = customerData.Count();
                var data = await customerData.Skip(skip).Take(pageSize).ToListAsync();
                return Json(new { draw = draw, recordFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
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
                project.active = "1";
                project.createdBy = 1;
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
                    string date = Request.Form["year"];

                    obj.description = Request.Form["description"].FirstOrDefault();
                    obj.month = date.Split('-')[1].ToString();
                    obj.year = date.Split('-')[0].ToString();
                    obj.updated = 1;
                    obj.tglTA = Convert.ToDateTime(Request.Form["execution_date"].FirstOrDefault()).Date;
                    obj.tglSelesaiTA = Convert.ToDateTime(Request.Form["finish_date"].FirstOrDefault()).Date;
                    obj.lastModify = DateTime.Now;
                    obj.modifyBy = 1;
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

        public IActionResult Equipments()
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
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                // Sort Column Direction (asc, desc)
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                // Search Value from (Search box)
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                // Paging Size (10, 20, 50, 100)
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;


                var customerData = _context.equipments.Include("users").Where(s => s.deleted == 0)
                    .Select(a => new { id = a.id, 
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
                        catProf = a.catProf  });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.eqDesc.StartsWith(searchValue) || b.eqTagNo.StartsWith(searchValue));
                }
                //Console.WriteLine(customerData);
                recordsTotal = customerData.Count();
                var data = await customerData.Skip(skip).Take(pageSize).ToListAsync();
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
                    equipments.createdBy = 1;
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
                    obj.updatedBy = 1;

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

                var sortColumn = Request.Form["columns[" + Request.Form["order[1][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                var sortColumnDirection = Request.Form["order[1][dir]"].FirstOrDefault();

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

                var sortColumn = Request.Form["columns[" + Request.Form["order[1][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                var sortColumnDirection = Request.Form["order[1][dir]"].FirstOrDefault();

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

                var sortColumn = Request.Form["columns[" + Request.Form["order[1][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                var sortColumnDirection = Request.Form["order[1][dir]"].FirstOrDefault();

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

                var sortColumn = Request.Form["columns[" + Request.Form["order[1][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                var sortColumnDirection = Request.Form["order[1][dir]"].FirstOrDefault();

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

        public IActionResult getUnitKilang()
        {
            var unitCode = Request.Form["unitCode"].FirstOrDefault();
            var data = _context.unit.Where(p => p.unitCode == unitCode).Where(p => p.deleted == 0).OrderBy(p => p.unitKilang).ToList();
            var select = "<option value=''>Select Unit</option>";
            foreach (var val in data) {
                select += "<option value='" + val.unitKilang + "' data-id='" + val.id + "' data-codeJob='" + val.codeJob+ "'>"+ val.unitKilang+ "</option>";
            }

            return Ok(select);
        }
    }

}