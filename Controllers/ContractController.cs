﻿using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using mystap.Models;
using System.Linq.Dynamic.Core;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace mystap.Controllers
{
    public class ContractController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        private readonly DatabaseContext _context;
        public ContractController(DatabaseContext context)
        {
            _context = context;
        }
        public IActionResult Sow()
        {
            ViewBag.groups_ = _context.sowGroup
               .Where(p => p.deleted == 0)
               .GroupBy(x => new { x.inisial, x.groups_ })
               .Select(z => new
               {
                   inisial = z.Key.inisial,
                   groups_ = z.Key.groups_

               })
               .ToList();
            ViewBag.sow_group = _context.sowGroup.Where(p => p.deleted != 1).ToList();
            ViewBag.userAccount = _context.users.Where(p => p.locked != 1).Where(p => p.statPekerja == "PLANNER").Where(p => p.alias != null).Where(p => p.alias != "").Where(p => p.statPekerja == "PEKERJA").ToList();
            ViewBag.project = _context.project.Where(p => p.deleted == 0).ToList();
            ViewBag.unit = _context.unit
               .Where(p => p.deleted == 0)
               .GroupBy(x => new { x.unitCode, x.unitProses })
               .Select(z => new
               {
                   unitCode = z.Key.unitCode,
                   unitProses = z.Key.unitProses

               })
               .ToList();

            return View();
        }
        public async Task<IActionResult> GetSowGroup()
        {
            try
            {
                var groups_ = Request.Form["groups_"].FirstOrDefault();
                var query = _context.sowGroup.Where(p => p.groups_ == groups_).Where(p => p.deleted == 0).OrderBy(p => p.sub_group);
                var data = await query.ToListAsync();
                var select = "<option value=''> Select Sub Group </option> ";
                foreach (var val in data)
                {
                    select += "<option value='val.subGroup' data-urut='val.urut'>" + val.sub_group + "</ option >";
                }

                return base.Content(select, "text/html");
            }
            catch
            {
                throw;
            }
        }
        public IActionResult Get_Sow()
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


                var customerData = _context.sow.Include("project").Include("users").Where(s => s.deleted == 0).Select(a => new { id = a.id, noSOW = a.noSOW, jobCode = a.jobCode, judulPekerjaan = a.judulPekerjaan, planner = a.planner, kabo = a.kabo, events = a.events, groups = a.groups, subGroups = a.subGroups, area = a.area, tahun = a.tahun, description = a.project.description, createdBy = a.users.alias, modifyBy = a.users.alias });

               /* if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }*/

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.judulPekerjaan.StartsWith(searchValue));
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

      

        public IActionResult Create_Sow(IFormCollection formcollaction)
        {
            try
            {
                Sow sow = new Sow();
                sow.projectID = 7;
                sow.noSOW = formcollaction["noSOW"];
                sow.events = formcollaction["events"];
                sow.groups = formcollaction["groups"];
                sow.subGroups = formcollaction["subGroups"];
                sow.area = formcollaction["area"];
                sow.kabo = formcollaction["kabo"];
                sow.tahun = formcollaction["tahun"];
                sow.judulPekerjaan = formcollaction["judulPekerjaan"];
                sow.planner = formcollaction["planner"];
                sow.jobCode = formcollaction["jobCode"];
                sow.file = formcollaction["file"];
                sow.createdBy = "Rama";
                sow.createdDate = DateTime.Now;
                sow.deleted = 0;

                Boolean t;
                if (sow != null)
                {
                    _context.sow.Add(sow);
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
        public IActionResult Update_Sow(Sow sow)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Sow obj = _context.sow.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {
                    obj.projectID = 7;
                    obj.noSOW = Request.Form["noSOW"].FirstOrDefault();
                    obj.events = Request.Form["events"].FirstOrDefault();
                    obj.groups = Request.Form["groups"].FirstOrDefault();
                    obj.subGroups = Request.Form["subGroups"].FirstOrDefault();
                    obj.area = Request.Form["area"].FirstOrDefault();
                    obj.kabo = Request.Form["kabo"].FirstOrDefault();
                    obj.tahun = Request.Form["tahun"].FirstOrDefault();
                    obj.judulPekerjaan = Request.Form["judulPekerjaan"].FirstOrDefault();
                    obj.planner = Request.Form["planner"].FirstOrDefault();
                    obj.jobCode = Request.Form["jobCode"].FirstOrDefault();
                    obj.file = Request.Form["file"].FirstOrDefault();
                    //obj.modifyBy = 'Rama';
                    obj.lastModify = DateTime.Now;
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

        public IActionResult Deleted_Sow(Sow sow)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                Sow obj = _context.sow.Where(p => p.id == id).FirstOrDefault();

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
