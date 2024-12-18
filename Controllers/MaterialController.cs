﻿using Azure.Core;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using ExcelDataReader;
using joblist.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using mystap.Helpers;
using mystap.Models;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace mystap.Controllers
{

    public class MaterialController : Controller
    {
        private readonly DatabaseContext _context;
        IWebHostEnvironment hostEnvironment;
        IExcelDataReader reader;
        public MaterialController(DatabaseContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this.hostEnvironment = hostEnvironment;
        }

		[AuthorizedAction]
		public IActionResult Material()
        {
            ViewBag.role = "RESERVASI";
            if (Module.hasModule("RESERVASI", HttpContext.Session))
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
		public async Task<IActionResult> Get_Material()
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

                var project_filter = Request.Form["project_filter"].FirstOrDefault();
                var status_ready = Request.Form["status_ready"].FirstOrDefault();

                var w = "";
                var ws = "";

                if (!string.IsNullOrEmpty(project_filter)) {
                    w += " AND zpm01.revision = '" + project_filter + "' ";
                }

                if (!string.IsNullOrEmpty(status_ready)) {
                    ws += " where (case when b.status_ = 'terpenuhi_stock' or b.status_ = 'onsite' then 'ready' else (case when CONVERT (DATE, GETDATE()) > b.finish_date then 'delay' when b.prognosa_ < b.md then 'on_track' else 'delay' end) end) = '" + status_ready + "' ";
                }

                var query = @"select " +
                            "b.*, " +
                            "(case " +
                            "when b.status_ = 'terpenuhi_stock' or b.status_ = 'onsite' then 'ready' " +
                            "else (case  " +
                            "when CONVERT (DATE, GETDATE()) > b.finish_date then 'delay' " +
                            "when b.prognosa_ < b.md then 'on_track' " +
                            "else 'delay' " +
                            "end) " +
                            "end) as status_ready, " +
                            "(case when b.fls = 'X' then 'fis' " +
                            "    when b.reqmt_qty = b.pr_qty then 'balance' " +
                            "    when b.reqmt_qty != b.pr_qty then " +
                            "        (case when b.reqmt_qty = (b.diff_qty + b.pr_qty) then 'balance' " +
                            "        else 'not_balance' " +
                            "        end) " +
                            "    when b.status_ = 'terpenuhi_stock' and b.reqmt_qty = b.qty_res then 'balance' " +
                            "    else 'not_balance' " +
                            "end) as status_qty " +
                            "from (select " +
                            "work_order.[order],max(work_order.[description]) as wo_description,max(work_order.main_work_ctr) as main_work_ctr,max(zpm01.id) as id, max(zpm01.revision) as revision, max(zpm01.reserv_no) as reserv_no,max(zpm01.material) as material,max(zpm01.material_description) as [description],zpm01.itm,max(zpm01.bun) as bun,max(zpm01.reqmt_qty) as reqmt_qty, " +
                            "max(zpm01.pr) as pr,max(zpm01.itm_pr) as pr_item,max(zpm01.qty_pr) as pr_qty,max(zpm01.qty_res) as qty_res,max(purch_order.po) as po,max(purch_order.qty_delivered) as po_qty,max(purch_order.item_po) as po_item,max(purch_order.dci) as dci, " +
                            "max(project.tglSelesaiTA) as finish_date, (max(zpm01.diff_qty) + max(zpm01.qty_pr)) as tot_, max(zpm01.fls) as fls, max(zpm01.diff_qty) as diff_qty, " +
                            "(case when max(project.taoh) = 'OH' then DATEADD(DAY, (case when  max(zpm01.prognosa_matl) is not null then max(zpm01.prognosa_matl) else 0 end),max(prognosa_oh.[start_date])) else max(project.tglTA) end) as md, " +
                            "(case when max(purch_order.po) is not null then DATEDIFF(day, max(purch_order.doc_date),max(purch_order.deliv_date))  else (case  when max(zpm01.dt_purch) is not null or max(zpm01.dt_purch) != '' then max(zpm01.dt_purch)  when max(zpm01.dt_iv) is not null or max(zpm01.dt_iv) != '' then max(zpm01.dt_iv) else ( case when max(zpm01.dt_ta) is not null or max(zpm01.dt_ta) != '' then max(zpm01.dt_ta) else 0 end)  end)  end) as dt_ , " +
                            "(case when max(purch_order.po) is not null then max(purch_order.deliv_date) else DATEADD(DAY, ((case when max(zpm01.dt_purch) is not null or max(zpm01.dt_purch) != '' then max(zpm01.dt_purch) when max(zpm01.dt_iv) is not null or max(zpm01.dt_iv) != '' then max(zpm01.dt_iv) else (case when max(zpm01.dt_ta) is not null or max(zpm01.dt_ta) != '' then max(zpm01.dt_ta) else 0 end) end) + (case when max(zpm01.dt_status_pengadaan) is not null or max(zpm01.dt_status_pengadaan) != '' then max(zpm01.dt_status_pengadaan) else 52 end)), CONVERT (DATE, GETDATE())) end) as prognosa_, " +
                            "(case when max(zpm01.pr) is null or max(zpm01.pr) = '' then (case when max(zpm01.reqmt_qty) = max(zpm01.qty_res) then 'terpenuhi_stock' else 'create_pr' end) else (case when (max(zpm01.pr) is not null or max(zpm01.pr) != '') and (max(purch_order.po) is not null or max(purch_order.po) != '') then (case when max(purch_order.dci) is null or max(purch_order.dci) = '' then 'tunggu_onsite' else 'onsite'  end) when (max(zpm01.status_pengadaan) is not null or max(zpm01.status_pengadaan) != '') then max(zpm01.status_pengadaan) else 'outstanding_pr' end) end) as status_  from Mystap.dbo.zpm01 " +
                            "left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order " +
                            "left join Mystap.dbo.prognosa_oh on prognosa_oh.eqTagno = work_order.equipment " +
                            "left join Mystap.dbo.project on project.revision = work_order.revision " +
                            "left join Mystap.dbo.purch_order on purch_order.material = zpm01.material AND purch_order.pr = zpm01.pr AND purch_order.item_pr = zpm01.itm_pr " +
                            "where ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) " + w + " group by work_order.[order],zpm01.itm ) as b " + ws;

                var c = FormattableStringFactory.Create(query);
                var datas = _context.viewReservasi.FromSql(c);

                /*  if (project_filter != "")
                  {
                      datas = datas.Where(p => p.revision == project_filter);
                  }*/

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    datas = datas.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    datas = datas.Where(m => m.order.Contains(searchValue) || m.description.Contains(searchValue) || m.wo_description.Contains(searchValue) || m.material.Contains(searchValue) || m.pr.Contains(searchValue) || m.po.Contains(searchValue) || m.status_qty.Contains(searchValue));
                }
                // Total number of rows count
                recordsTotal = datas.Count();
                if(pageSize > 0)
                {
                    datas = datas.Skip(skip).Take(pageSize);
                }
                // Paging
                var data = await datas.ToListAsync();
                // Returning Json Data
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch
            {
                throw;
            }
        }

        [AuthorizedAction]
        public IActionResult DeleteReservasi(Zpm01 zpm01)
        {
            try
            {
                Zpm01 obj = _context.zpm01.Where(p => p.no_order == Request.Form["order"].FirstOrDefault() && p.itm == Request.Form["itm"].FirstOrDefault()).FirstOrDefault();

                if (obj != null)
                {
                    obj.del = "X";
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
		public IActionResult OutstandingReservasi()
        {
            ViewBag.role = "OUTSTANDING_RESERVASI";
            if (Module.hasModule("OUTSTANDING_RESERVASI", HttpContext.Session))
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
		public async Task<IActionResult> OutstandingReservasi_()
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

                var project_filter = Request.Form["project_filter"].FirstOrDefault();
                var lldi_filter = Request.Form["lldi_filter"].FirstOrDefault();
                var taoh_filter = Request.Form["taoh_filter"].FirstOrDefault();

                var w = "";
                if (!string.IsNullOrEmpty(project_filter))
                {
                    w += " and work_order.revision = '" + project_filter + "' ";
                }

                if (!string.IsNullOrEmpty(lldi_filter))
                {
                    if (lldi_filter == "LLDI")
                    {
                        w += " and zpm01.unloading_point = '" + lldi_filter + "' ";
                    }
                    else
                    {
                        w += " and (zpm01.unloading_point is null or zpm01.unloading_point != 'xx') ";
                    }
                }

                if (!string.IsNullOrEmpty(taoh_filter))
                {
                    w += " and right(zpm01.pg, 1) = '" + taoh_filter + "' ";
                }

                var query = @"select " +
                    " max(zpm01.id) as id,work_order.[order] as [order],max(work_order.[description]) as wo_description, max(work_order.main_work_ctr) as main_work_ctr,max(zpm01.material) as material, max(zpm01.revision) as revision, max(zpm01.reserv_no) as reserv_no,max(zpm01.material_description) as material_desc,zpm01.itm,max(zpm01.sloc) as sloc,max(zpm01.pg) as pg,max(zpm01.ict) as ict, max(zpm01.del) as del, " +
                    " max(zpm01.recipient) as recipient, max(zpm01.unloading_point) as unloading_point, max(zpm01.fls) as fls, max(zpm01.cost_ctrs) as cost_ctrs,max(zpm01.reqmt_date) as reqmt_date,max(zpm01.bun)as bun,max(zpm01.reqmt_qty) as reqmt_qty, max(zpm01.qty_f_avail_check) as qty_f_avail_check, max(zpm01.qty_withdrawn) as qty_withdrawn, " +
                    " max(zpm01.price) as price,max(zpm01.per) as per, max(zpm01.crcy) as crcy,max(zpm01.pr) as pr,max(zpm01.itm_pr) as pr_item,max(zpm01.qty_pr) as pr_qty,max(zpm01.qty_res) as qty_res ,max(zpm01.prognosa_matl) as prognosa_matl" +
                    " from Mystap.dbo.zpm01 " +
                    " left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order  " +
                    " where ((zpm01.del is null or zpm01.del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) and (zpm01.pr is null or zpm01.pr = '') and zpm01.reqmt_qty != zpm01.qty_res " + w + " group by work_order.[order],zpm01.itm ";

                var c = FormattableStringFactory.Create(query);
                var datas = _context.viewOutstandingReservasi.FromSql(c);

                /*  if (project_filter != "")
                  {
                      datas = datas.Where(p => p.revision == project_filter);
                  }*/

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    datas = datas.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    datas = datas.Where(m => m.order.Contains(searchValue) || m.reserv_no.Contains(searchValue) || m.wo_description.Contains(searchValue) || m.material_desc.Contains(searchValue) || m.material.Contains(searchValue));
                }
                // Total number of rows count
                recordsTotal = datas.Count();
                // Paging
                if (pageSize > 0)
                {
                    datas = datas.Skip(skip).Take(pageSize);
                }
                var data = await datas.ToListAsync();
                // Returning Json Data
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult PenggunaanMaterial()
        {
            ViewBag.role = "PENGGUNAAN_MATERIAL";
            if (Module.hasModule("PENGGUNAAN_MATERIAL", HttpContext.Session))
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
		public async Task<IActionResult> PenggunaanMaterial_()
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

                var project_filter = Request.Form["project_filter"].FirstOrDefault();
                var lldi_filter = Request.Form["lldi_filter"].FirstOrDefault();
                var taoh_filter = Request.Form["taoh_filter"].FirstOrDefault();

                var w = "";
                if (!string.IsNullOrEmpty(project_filter))
                {
                    w += " and work_order.revision = '" + project_filter + "' ";
                }

                if (!string.IsNullOrEmpty(lldi_filter))
                {
                    if (lldi_filter == "LLDI")
                    {
                        w += " and zpm01.unloading_point = '" + lldi_filter + "' ";
                    }
                    else
                    {
                        w += " and (zpm01.unloading_point is null or zpm01.unloading_point != 'xx') ";
                    }
                }


                var query = @"select " +
                    " max(zpm01.id) as id,work_order.[order] as [order],max(work_order.[description]) as wo_description, max(work_order.main_work_ctr) as main_work_ctr,max(zpm01.material) as material, max(zpm01.revision) as revision, max(zpm01.reserv_no) as reserv_no,max(zpm01.material_description) as material_desc,zpm01.itm,max(zpm01.sloc) as sloc,max(zpm01.pg) as pg,max(zpm01.ict) as ict, max(zpm01.del) as del, " +
                    " max(zpm01.recipient) as recipient, max(zpm01.unloading_point) as unloading_point, max(zpm01.fls) as fls, max(zpm01.cost_ctrs) as cost_ctrs,max(zpm01.reqmt_date) as reqmt_date,max(zpm01.bun)as bun,max(zpm01.reqmt_qty) as reqmt_qty, max(zpm01.qty_f_avail_check) as qty_f_avail_check, max(zpm01.qty_withdrawn) as qty_withdrawn, " +
                    " max(zpm01.price) as price,max(zpm01.per) as per, max(zpm01.crcy) as crcy,max(zpm01.pr) as pr,max(zpm01.itm_pr) as pr_item,max(zpm01.qty_pr) as pr_qty,max(zpm01.qty_res) as qty_res ,max(zpm01.prognosa_matl) as prognosa_matl" +
                    " from Mystap.dbo.zpm01 " +
                    " left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order  " +
                    " where ((zpm01.del is null or zpm01.del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) " + w + " group by work_order.[order],zpm01.itm ";

                var c = FormattableStringFactory.Create(query);
                var datas = _context.viewOutstandingReservasi.FromSql(c);

                /*  if (project_filter != "")
                  {
                      datas = datas.Where(p => p.revision == project_filter);
                  }*/

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    datas = datas.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    datas = datas.Where(m => m.order.Contains(searchValue) || m.reserv_no.Contains(searchValue) || m.material_desc.Contains(searchValue));
                }
                // Total number of rows count
                recordsTotal = datas.Count();
                // Paging
                if (pageSize > 0)
                {
                    datas = datas.Skip(skip).Take(pageSize);
                }
                var data = await datas.ToListAsync();
                // Returning Json Data
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult AddPrognosaMaterial()
        {
            try
            {
                string id = Request.Form["id"];
                string[] id_ = id.Split(",");

                var prognosa_matl = Request.Form["prognosa_matl"].FirstOrDefault();


                foreach (var val in id_)
                {
                    Zpm01 job = _context.zpm01.Where(p => p.id == Convert.ToInt64(val)).FirstOrDefault();
                    if (job != null)
                    {
                        job.prognosa_matl = Convert.ToInt32(prognosa_matl);
                        _context.SaveChanges();
                    }
                }

                return Json(new { result = true });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult EditPR()
        {
            ViewBag.role = "EDIT_RESERVASI";
            if (Module.hasModule("EDIT_RESERVASI", HttpContext.Session))
            {
                ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == 1).ToList();
                ViewBag.wo = "";
                return View();
            }
            else
            {
                return NotFound();
            }
            
        }

		[AuthorizedAction]
		public IActionResult GetWorkOrder()
        {
            try
            {
                var order = Request.Form["order"].FirstOrDefault();
                var data = _context.work_order.Where(p => p.order == order).FirstOrDefault();
                return Json(new { data = data });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public async Task<IActionResult> GetMaterial_()
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

                var no_order = Request.Form["no_order"].FirstOrDefault();
                var check = Request.Form["check"].FirstOrDefault();
                var pr = Request.Form["pr"].FirstOrDefault();
                var itm_pr = Request.Form["itm_pr"].FirstOrDefault();

                if (no_order != "")
                {

                    var w = "";
                    if (!string.IsNullOrEmpty(no_order))
                    {
                        w += " and zpm01.no_order = '" + no_order + "' ";
                    }

                    if (!string.IsNullOrEmpty(pr))
                    {
                        w += " and zpm01.pr = '" + pr + "' ";
                    }

                    if (!string.IsNullOrEmpty(itm_pr))
                    {
                        w += " and zpm01.pr = '" + itm_pr + "' ";
                    }

                    if (check == "outs")
                    {
                        w += "(case when max(zpm01.fls) = 'X' then 'fis' " +
                            "    when max(zpm01.reqmt_qty) = max(zpm01.qty_pr) then 'balance' " +
                            "    when max(zpm01.reqmt_qty) != max(zpm01.qty_pr) then " +
                            "        (case when max(zpm01.reqmt_qty) = (max(zpm01.diff_qty) + max(zpm01.qty_pr)) then 'balance' " +
                            "        else 'not_balance' " +
                            "        end) " +
                            "    when max(zpm01.reqmt_qty) = max(zpm01.qty_res) then 'balance' " +
                            "    else 'not_balance' " +
                            "end) = 'not_balance' ";
                    }

                    var query = @"select " +
                            "work_order.[order],max(zpm01.id) as id, max(zpm01.material) as material,max(zpm01.material_description) as [description],zpm01.itm,max(zpm01.bun) as bun,max(zpm01.reqmt_qty) as reqmt_qty, max(zpm01.reqmt_date) as reqmt_date, max(zpm01.pg) as pg, " +
                            "max(zpm01.pr) as pr,max(zpm01.itm_pr) as pr_item,max(zpm01.qty_pr) as pr_qty,max(zpm01.qty_res) as qty_res, " +
                            "(case when max(zpm01.fls) = 'X' then 'fis' " +
                            "    when max(zpm01.reqmt_qty) = max(zpm01.qty_pr) then 'balance' " +
                            "    when max(zpm01.reqmt_qty) != max(zpm01.qty_pr) then " +
                            "        (case when max(zpm01.reqmt_qty) = (max(zpm01.diff_qty) + max(zpm01.qty_pr)) then 'balance' " +
                            "        else 'not_balance' " +
                            "        end) " +
                            "    when max(zpm01.reqmt_qty) = max(zpm01.qty_res) then 'balance' " +
                            "    else 'not_balance' " +
                            "end) as status_qty " +
                            "from Mystap.dbo.zpm01 " +
                            "left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order " +
                            "left join Mystap.dbo.purch_order on purch_order.material = zpm01.material AND purch_order.pr = zpm01.pr AND purch_order.item_pr = zpm01.itm_pr " +
                            "where ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) " + w + " group by work_order.[order],zpm01.itm";

                    var c = FormattableStringFactory.Create(query);
                    var datas = _context.viewUpdatePr.FromSql(c);

                    /*  if (project_filter != "")
                      {
                          datas = datas.Where(p => p.revision == project_filter);
                      }*/

                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                    {
                        datas = datas.OrderBy(sortColumn + " " + sortColumnDirection);
                    }

                    //search
                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        datas = datas.Where(m => m.order.Contains(searchValue) || m.material.Contains(searchValue) || m.description.Contains(searchValue) || m.pr.Contains(searchValue));
                    }
                    // Total number of rows count
                    recordsTotal = datas.Count();
                    if (pageSize > 0)
                    {
                        datas = datas.Skip(skip).Take(pageSize);
                    }
                    // Paging
                    var data = await datas.ToListAsync();
                    // Returning Json Data
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
                }
                else
                {
                    return Json(new { data = "" });
                }
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult UpdatePR()
        {
            var id_ = Convert.ToInt64(Request.Form["hidden_id"].FirstOrDefault());
            var cek = _context.zpm01.Where(p => p.no_order == Request.Form["hidden_order"].FirstOrDefault()).Where(p => p.id == id_).FirstOrDefault();
            Zpm01 job = _context.zpm01.Where(p => p.id == id_).FirstOrDefault();
            if (job != null)
            {
                if (cek.reqmt_qty == Request.Form["stock"].FirstOrDefault())
                {
                    job.pr = null;
                    job.itm_pr = null;
                    job.qty_pr = null;
                    job.qty_res = Request.Form["stock"].FirstOrDefault();
                }
                else
                {
                    job.pr = Request.Form["pr"].FirstOrDefault();
                    job.itm_pr = Request.Form["pr_item"].FirstOrDefault();
                    job.qty_pr = Request.Form["qty_pr"].FirstOrDefault();
                    job.qty_res = Request.Form["stock"].FirstOrDefault();
                }
                _context.SaveChanges();
                return Json(new { result = true, title = "Berhasil!", icon = "success", text = "Berhasil di Update!!" });
            }

            return Json(new { result = false, title = "Gagal!", icon = "error", text = "Gagal di Update!!" });

        }

		[AuthorizedAction]
		public IActionResult ListPR()
        {
            ViewBag.role = "LIST_PR";
            if (Module.hasModule("LIST_PR", HttpContext.Session))
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
		public async Task<IActionResult> ListPR_()
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

                var project = Request.Form["project_filter"].FirstOrDefault();
                var lldi = Request.Form["lldi_filter"].FirstOrDefault();
                var status_pengadaan = Request.Form["status_pengadaan_filter"].FirstOrDefault();
                var taoh = Request.Form["taoh_filter"].FirstOrDefault();
                var eqtagno = Request.Form["eqTagNo_filter"].FirstOrDefault();
                var pr = Request.Form["pr_filter"].FirstOrDefault();
                var material = Request.Form["wo_filter"].FirstOrDefault();
                var w = "";
                if (!string.IsNullOrEmpty(lldi))
                {
                    if(lldi == "LLDI")
                    {
                        w += " and zpm01.unloading_point = 'xx' ";
                    }
                    else
                    {
                        w += " and (zpm01.unloading_point is null or zpm01.unloading_point != 'xx') ";

                    }
                }

                if (!string.IsNullOrEmpty(status_pengadaan))
                {
                    w += " and zpm01.status_pengadaan ='"+status_pengadaan+"' ";

                }

                if (!string.IsNullOrEmpty(taoh))
                {
                    w += " and right(zpm01.pg, 1) = '"+taoh+"' ";
                }

                if (!string.IsNullOrEmpty(eqtagno))
                {
                    w += " and work_order.equipment like '"+eqtagno+"%' ";
                }

                if (!string.IsNullOrEmpty(pr))
                {
                    w += " and zpm01.pr like '" + pr + "%' ";
                }

                if (!string.IsNullOrEmpty(material))
                {
                    w += " and zpm01.material like '" + material + "%' ";
                }

                var query = @"select "+
                    " max(zpm01.pr) as pr, max(zpm01.itm_pr) as pr_item, max(zpm01.qty_pr) as qty_pr, max(zpm01.pg) as pg, max(zpm01.reqmt_date) as reqmt_date, work_order.[order],max(zpm01.material) as material,max(zpm01.material_description) as material_description "+
                    " from zpm01 "+
                    " left join work_order on work_order.[order] = zpm01.no_order "+
                    " left join purch_order on purch_order.material = zpm01.material AND purch_order.pr = zpm01.pr AND purch_order.item_pr = zpm01.itm_pr "+
                    " where (zpm01.pr is not null or zpm01.pr != '') and ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) and zpm01.reqmt_qty != '0' "+
                    " and zpm01.revision = '"+project+"' "+w+" group by work_order.[order], zpm01.itm ";

                var c = FormattableStringFactory.Create(query);
                var datas = _context.viewListPr.FromSql(c);

                /*  if (project_filter != "")
                  {
                      datas = datas.Where(p => p.revision == project_filter);
                  }*/

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    datas = datas.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    datas = datas.Where(m => m.order.Contains(searchValue) || m.pr.Contains(searchValue) || m.material_description.Contains(searchValue) || m.material.Contains(searchValue));
                }
                // Total number of rows count
                recordsTotal = datas.Count();
                // Paging
                if (pageSize > 0)
                {
                    datas = datas.Skip(skip).Take(pageSize);
                }
                var data = await datas.ToListAsync();
                // Returning Json Data
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult DistribusiPR()
        {
            ViewBag.role = "DISTRIBUSI_PR";
            if (Module.hasModule("DISTRIBUSI_PR", HttpContext.Session))
            {
                ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == 1).ToList();
                ViewBag.buyer = _context.users.Where(p => p.subSection == "PURCHASING" && p.deleted == 0 && p.locked == 0).ToList();
                return View();
            }
            else
            {
                return NotFound();
            }
           
        }

		[AuthorizedAction]
		public async Task<IActionResult> DistribusiPR_()
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

                var project = Request.Form["project_filter"].FirstOrDefault();
                var lldi = Request.Form["lldi_filter"].FirstOrDefault();
                var status_pengadaan = Request.Form["status_pengadaan_filter"].FirstOrDefault();
                var taoh = Request.Form["taoh_filter"].FirstOrDefault();
                var eqtagno = Request.Form["eqTagNo_filter"].FirstOrDefault();
                var pr = Request.Form["pr_filter"].FirstOrDefault();
                var material = Request.Form["wo_filter"].FirstOrDefault();
                var buyer = Request.Form["buyer_filter"].FirstOrDefault();
                var w = "";
                if (!string.IsNullOrEmpty(lldi))
                {
                    if (lldi == "LLDI")
                    {
                        w += " and zpm01.unloading_point = 'xx' ";
                    }
                    else
                    {
                        w += " and (zpm01.unloading_point is null or zpm01.unloading_point != 'xx') ";

                    }
                }

                if (!string.IsNullOrEmpty(status_pengadaan))
                {
                    w += " and zpm01.status_pengadaan ='" + status_pengadaan + "' ";

                }

                if (!string.IsNullOrEmpty(taoh))
                {
                    w += " and right(zpm01.pg, 1) = '" + taoh + "' ";
                }

                if (!string.IsNullOrEmpty(eqtagno))
                {
                    w += " and work_order.equipment like '" + eqtagno + "%' ";
                }

                if (!string.IsNullOrEmpty(pr))
                {
                    w += " and zpm01.pr like '" + pr + "%' ";
                }

                if (!string.IsNullOrEmpty(material))
                {
                    w += " and zpm01.material like '" + material + "%' ";
                }

                if (!string.IsNullOrEmpty(buyer))
                {
                    w += " and zpm01.buyer = '"+buyer+"' ";
                }

                var query = @"select " +
                    " max(zpm01.id) as id, max(zpm01.pr) as pr, max(zpm01.itm_pr) as pr_item, max(zpm01.qty_pr) as qty_pr, max(zpm01.pg) as pg, max(zpm01.reqmt_date) as reqmt_date, work_order.[order],max(zpm01.material) as material,max(zpm01.material_description) as material_description, " +
                    " max(purch_order.po) as po,max(purch_order.po_quantity) as qty_po,max(purch_order.item_po) as po_item,max(work_order.equipment) as equipment, " +
                    " (case when max(zpm01.unloading_point) = 'xx' then 'LLD' else 'Non LLD' end) as lld, (case when max(zpm01.doc_pr) != 'true' or max(zpm01.doc_pr) is null then 'false' else 'true' end) as doc_pr, " +
                    " max(zpm01.status_pr) as status_pr, max(zpm01.dt_ta) as dt_ta,max(zpm01.dt_iv) as dt_iv,max(zpm01.dt_purch) as dt_purch,max(zpm01.status_pengadaan) as status_pengadaan,max(zpm01.buyer) as buyer,max(zpm01.keterangan) as keterangan " +
                    " from zpm01 " +
                    " left join work_order on work_order.[order] = zpm01.no_order " +
                    " left join purch_order on purch_order.material = zpm01.material AND purch_order.pr = zpm01.pr AND purch_order.item_pr = zpm01.itm_pr " +
                    " where (zpm01.pr is not null or zpm01.pr != '') and ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) and zpm01.reqmt_qty != '0' " +
                    " and zpm01.revision = '" + project + "' " + w + " group by work_order.[order], zpm01.itm ";

                var c = FormattableStringFactory.Create(query);
                var datas = _context.viewDistribusiPr.FromSql(c);

                /*  if (project_filter != "")
                  {
                      datas = datas.Where(p => p.revision == project_filter);
                  }*/

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    datas = datas.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    datas = datas.Where(m => m.order.Contains(searchValue) || m.pr.Contains(searchValue) || m.material.Contains(searchValue) || m.material_description.Contains(searchValue) || m.po.Contains(searchValue) || m.equipment.Contains(searchValue) || m.buyer.Contains(searchValue) || m.keterangan.Contains(searchValue));
                }
                // Total number of rows count
                recordsTotal = datas.Count();
                // Paging
                if (pageSize > 0)
                {
                    datas = datas.Skip(skip).Take(pageSize);
                }
                var data = await datas.ToListAsync();
                // Returning Json Data
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult RemoveBuyer()
        {
            try
            {
                var id = Request.Form["id[]"];

                foreach (var val in id)
                {
                    Zpm01 job = _context.zpm01.Where(p => p.id == Convert.ToInt64(val)).FirstOrDefault();
                    if (job != null)
                    {
                        job.buyer = null;
                        _context.SaveChanges();
                    }
                }

                return Json(new { result = true });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult DocPR()
        {
            try
            {
                var id = Request.Form["id[]"];
                var info = Request.Form["info"].FirstOrDefault();

                foreach (var val in id)
                {
                    Zpm01 job = _context.zpm01.Where(p => p.id == Convert.ToInt64(val)).FirstOrDefault();
                    if (job != null)
                    {
                        job.doc_pr = info;
                        _context.SaveChanges();
                    }
                }

                return Json(new { result = true });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult StatusPengadaan()
        {
            try
            {
                var id = Request.Form["id[]"];

                var status_pengadaan = Request.Form["status_pengadaan"].FirstOrDefault();
                var dt_status_pengadaan = Request.Form["dt_status_pengadaan"].FirstOrDefault();
                var keterangan = Request.Form["keterangan"].FirstOrDefault();
                var dt_ = Request.Form["dt_"].FirstOrDefault();
                var buyer = Request.Form["buyer"].FirstOrDefault();

                foreach (var val in id)
                {
                    Zpm01 job = _context.zpm01.Where(p => p.id == Convert.ToInt64(val)).FirstOrDefault();
                    if (job != null)
                    {

                        if (!string.IsNullOrEmpty(status_pengadaan))
                        {
                            job.status_pengadaan = status_pengadaan;
                            job.dt_status_pengadaan = Convert.ToInt32(dt_status_pengadaan);
                        }

                        if (!string.IsNullOrEmpty(keterangan))
                        {
                            job.keterangan = keterangan;
                        }

                        if (!string.IsNullOrEmpty(dt_))
                        {
                            job.dt_purch = Convert.ToInt32(dt_);
                        }

                        if (!string.IsNullOrEmpty(buyer))
                        {
                            job.buyer = buyer;
                        }

                        _context.SaveChanges();
                    }
                }

                return Json(new { result = true });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult AddDT()
        {
            try
            {
                var id = Request.Form["id[]"];

                var dt = Convert.ToInt32(Request.Form["dt"].FirstOrDefault());
                var bagian = "TA"; //alias fungsi user

                if (bagian == "TA" || bagian == "INVENTORY" || bagian == "PURCHASING")
                {
                    foreach (var val in id)
                    {
                        Zpm01 job = _context.zpm01.Where(p => p.id == Convert.ToInt64(val)).FirstOrDefault();
                        if (job != null)
                        {
                            if (bagian == "TA")
                            {
                                job.dt_ta = dt;
                            }
                            else if (bagian == "INVENTORY")
                            {
                                job.dt_iv = dt;
                            }
                            else if (bagian == "PURCHASING")
                            {
                                job.dt_purch = dt;
                            }
                            _context.SaveChanges();
                        }
                    }
                    return Json(new { result = true });
                }
                else
                {
                    return Json(new { result = false });
                }
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult ImportMaterial()
        {
            try
            {
                var name = Request.Form["hidden_info"].FirstOrDefault();
                if(name == "wo")
                {
                    var file = Request.Form.Files[0];
                    return ImportWo(file);
                }
                else if (name == "zpm01")
                {
                    var file = Request.Form.Files[0];
                    return ImportZpm01(file);
                }
                else if(name == "zpm02")
                {
                    var file = Request.Form.Files[0];
                    return ImportZpm02(file);
                }
                else if (name == "zpm03")
                {
                    var file = Request.Form.Files[0];
                    return ImportZpm03(file);
                }
                else if (name == "zpm05")
                {
                    var file = Request.Form.Files[0];
                    return ImportZpm05(file);
                }
                else if (name == "zpm07")
                {
                    var file = Request.Form.Files[0];
                    return ImportZpm07(file);
                }
                else if (name == "mb25")
                {
                    var file = Request.Form.Files[0];
                    return ImportMb25(file);
                }
                else if (name == "pr_inv")
                {
                    var file = Request.Form.Files[0];
                    return ImportPrInv(file);
                }
                else
                {
                    return Json(new { result = false, text = "Fatal Error!!" });
                }
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult ImportWo(IFormFile file)
        {
            try
            {

                if (file == null)
                {
                    return Json(new { result = false, text = "File Tidak Ada!!" });
                }


                // Create the Directory if it is not exist
                string dirPath = Path.Combine(hostEnvironment.WebRootPath, "ReceivedReports");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                // MAke sure that only Excel file is used 
                string dataFileName = Path.GetFileName(file.FileName);

                if (dataFileName != "work_order")
                {
                    return Json(new { result = false, text = "Harap Sesuaikan Penamaan File!!" });
                }

                string extension = Path.GetExtension(dataFileName);

                string[] allowedExtsnions = new string[] { ".xls", ".xlsx" };

                if (!allowedExtsnions.Contains(extension))
                {
                    return Json(new { result = false, text = "Maaf! File ini tidak diperbolehkan, pastikan file berekstensi .xls atau .xlsx diupload." });
                }

                // Make a Copy of the Posted File from the Received HTTP Request
                string saveToPath = Path.Combine(dirPath, dataFileName);

                using (FileStream stream = new FileStream(saveToPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                string filpath = Path.GetFullPath(file.FileName);

                // USe this to handle Encodeing differences in .NET Core
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                // read the excel file
                using (var stream = new FileStream(saveToPath, FileMode.Open))
                {
                    if (extension == ".xls")
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    else
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                    DataSet ds = new DataSet();
                    ds = reader.AsDataSet();
                    reader.Close();

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        // Read the the Table
                        DataTable serviceDetails = ds.Tables[0];
                        for (int i = 1; i < serviceDetails.Rows.Count; i++)
                        {
                            WorkOrder obj = _context.work_order.Where(p => p.order == serviceDetails.Rows[i][4].ToString()).FirstOrDefault();
                            if (obj != null)
                            {
                                obj.plant = serviceDetails.Rows[i][0].ToString();
                                obj.notification = serviceDetails.Rows[i][1].ToString();
                                obj.created_on = Convert.ToDateTime(serviceDetails.Rows[i][2].ToString());
                                obj.superior_order = serviceDetails.Rows[i][3].ToString();
                                obj.order = serviceDetails.Rows[i][4].ToString();
                                obj.description = serviceDetails.Rows[i][5].ToString();
                                obj.equipment = serviceDetails.Rows[i][6].ToString();
                                obj.func_loc = serviceDetails.Rows[i][7].ToString();
                                obj.location = serviceDetails.Rows[i][8].ToString();
                                obj.revision = serviceDetails.Rows[i][9].ToString();
                                obj.system_status = serviceDetails.Rows[i][10].ToString();
                                obj.user_status = Convert.ToInt32(serviceDetails.Rows[i][11].ToString());
                                obj.wbs_ord_header = serviceDetails.Rows[i][12].ToString();
                                obj.total_plnnd_costs = Convert.ToInt32(serviceDetails.Rows[i][13].ToString());
                                obj.total_act_costs = Convert.ToInt32(serviceDetails.Rows[i][14].ToString());
                                obj.planner_group = serviceDetails.Rows[i][15].ToString();
                                obj.main_work_ctr = serviceDetails.Rows[i][16].ToString();
                                obj.changed_by = serviceDetails.Rows[i][17].ToString();
                                obj.bas_start_date = Convert.ToDateTime(serviceDetails.Rows[i][18].ToString());
                                obj.basic_fin_date = Convert.ToDateTime(serviceDetails.Rows[i][19].ToString());
                                obj.actual_release = Convert.ToDateTime(serviceDetails.Rows[i][20].ToString());
                                obj.cost_center = serviceDetails.Rows[i][21].ToString();
                                obj.entered_by = serviceDetails.Rows[i][22].ToString();

                                _context.SaveChanges();

                            }
                            else
                            {

                                WorkOrder wo = new WorkOrder();
                                wo.plant = serviceDetails.Rows[i][0].ToString();
                                wo.notification = serviceDetails.Rows[i][1].ToString();
                                wo.created_on = Convert.ToDateTime(serviceDetails.Rows[i][2].ToString());
                                wo.superior_order = serviceDetails.Rows[i][3].ToString();
                                wo.order = serviceDetails.Rows[i][4].ToString();
                                wo.description = serviceDetails.Rows[i][5].ToString();
                                wo.equipment = serviceDetails.Rows[i][6].ToString();
                                wo.func_loc = serviceDetails.Rows[i][7].ToString();
                                wo.location = serviceDetails.Rows[i][8].ToString();
                                wo.revision = serviceDetails.Rows[i][9].ToString();
                                wo.system_status = serviceDetails.Rows[i][10].ToString();
                                wo.user_status = Convert.ToInt32(serviceDetails.Rows[i][11].ToString());
                                wo.wbs_ord_header = serviceDetails.Rows[i][12].ToString();
                                wo.total_plnnd_costs = Convert.ToInt32(serviceDetails.Rows[i][13].ToString());
                                wo.total_act_costs = Convert.ToInt32(serviceDetails.Rows[i][14].ToString());
                                wo.planner_group = serviceDetails.Rows[i][15].ToString();
                                wo.main_work_ctr = serviceDetails.Rows[i][16].ToString();
                                wo.changed_by = serviceDetails.Rows[i][17].ToString();
                                wo.bas_start_date = Convert.ToDateTime(serviceDetails.Rows[i][18].ToString());
                                wo.basic_fin_date = Convert.ToDateTime(serviceDetails.Rows[i][19].ToString());
                                wo.actual_release = Convert.ToDateTime(serviceDetails.Rows[i][20].ToString());
                                wo.cost_center = serviceDetails.Rows[i][21].ToString();
                                wo.entered_by = serviceDetails.Rows[i][22].ToString();



                                // Add the record in Database
                                _context.work_order.Add(wo);
                                _context.SaveChanges();
                            }
                        }
                    }
                }

                return Json(new { result = true, text = "Berhasil" });
            }
            catch (Exception)
            {
                return Json(new { result = false, text = "Harap Hubungi Administrator!!" });
            }
        }

		[AuthorizedAction]
		public IActionResult ImportZpm01(IFormFile file)
        {
            try
            {

                if (file == null)
                {
                    return Json(new { result = false, text = "File Tidak Ada!!" });
                }


                //// Create the Directory if it is not exist
                string dirPath = Path.Combine(hostEnvironment.WebRootPath, "ReceivedReports");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                // MAke sure that only Excel file is used 
                string dataFileName = Path.GetFileName(file.FileName);

                if(dataFileName != "zpm01")
                {
                    return Json(new { result = false, text = "Harap Sesuaikan Penamaan File!!" });
                }

                string extension = Path.GetExtension(dataFileName);

                string[] allowedExtsnions = new string[] { ".xls", ".xlsx" };

                if (!allowedExtsnions.Contains(extension))
                {
                    return Json(new { result = false, text = "Maaf! File ini tidak diperbolehkan, pastikan file berekstensi .xls atau .xlsx diupload." });
                }

                // Make a Copy of the Posted File from the Received HTTP Request
                string saveToPath = Path.Combine(dirPath, dataFileName);

                using (FileStream stream = new FileStream(saveToPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // USe this to handle Encodeing differences in .NET Core
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                // read the excel file
                using (var stream = new FileStream(saveToPath, FileMode.Open))
                {
                    if (extension == ".xls")
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    else
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                    DataSet ds = new DataSet();
                    ds = reader.AsDataSet();
                    reader.Close();

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        // Read the the Table
                        DataTable serviceDetails = ds.Tables[0];
                        for (int i = 1; i < serviceDetails.Rows.Count; i++)
                        {
                            Zpm01 obj = _context.zpm01.Where(p => p.no_order == serviceDetails.Rows[i][0].ToString() && p.itm == serviceDetails.Rows[i][5].ToString()).FirstOrDefault();
                            if (obj != null)
                            {
                                obj.no_order = serviceDetails.Rows[i][0].ToString();
                                obj.revision = serviceDetails.Rows[i][1].ToString();
                                obj.reserv_no = serviceDetails.Rows[i][2].ToString();
                                obj.material = serviceDetails.Rows[i][3].ToString();
                                obj.material_description = serviceDetails.Rows[i][4].ToString();
                                obj.itm = serviceDetails.Rows[i][5].ToString();
                                obj.sloc = serviceDetails.Rows[i][6].ToString();
                                obj.pg = serviceDetails.Rows[i][7].ToString();
                                obj.ict = serviceDetails.Rows[i][8].ToString();
                                obj.del = serviceDetails.Rows[i][9].ToString();
                                obj.unloading_point = serviceDetails.Rows[i][11].ToString();
                                obj.fls = serviceDetails.Rows[i][12].ToString();
                                obj.cost_ctrs = serviceDetails.Rows[i][13].ToString();
                                obj.gl_acct = serviceDetails.Rows[i][14].ToString();
                                obj.bun = serviceDetails.Rows[i][15].ToString();
                                obj.reqmt_date = serviceDetails.Rows[i][16].ToString();
                                obj.reqmt_qty = serviceDetails.Rows[i][17].ToString();
                                obj.qty_f_avail_check = serviceDetails.Rows[i][18].ToString();
                                obj.qty_withdrawn = serviceDetails.Rows[i][19].ToString();
                                obj.price = serviceDetails.Rows[i][20].ToString();
                                obj.per = serviceDetails.Rows[i][21].ToString();
                                obj.crcy = serviceDetails.Rows[i][22].ToString();

                                _context.SaveChanges();

                            }
                            else
                            {

                                Zpm01 data = new Zpm01();
                                data.no_order = serviceDetails.Rows[i][0].ToString();
                                data.revision = serviceDetails.Rows[i][1].ToString();
                                data.reserv_no = serviceDetails.Rows[i][2].ToString();
                                data.material = serviceDetails.Rows[i][3].ToString();
                                data.material_description = serviceDetails.Rows[i][4].ToString();
                                data.itm = serviceDetails.Rows[i][5].ToString();
                                data.sloc = serviceDetails.Rows[i][6].ToString();
                                data.pg = serviceDetails.Rows[i][7].ToString();
                                data.ict = serviceDetails.Rows[i][8].ToString();
                                data.del = serviceDetails.Rows[i][9].ToString();
                                data.unloading_point = serviceDetails.Rows[i][11].ToString();
                                data.fls = serviceDetails.Rows[i][12].ToString();
                                data.cost_ctrs = serviceDetails.Rows[i][13].ToString();
                                data.gl_acct = serviceDetails.Rows[i][14].ToString();
                                data.bun = serviceDetails.Rows[i][15].ToString();
                                data.reqmt_date = serviceDetails.Rows[i][16].ToString();
                                data.reqmt_qty = serviceDetails.Rows[i][17].ToString();
                                data.qty_f_avail_check = serviceDetails.Rows[i][18].ToString();
                                data.qty_withdrawn = serviceDetails.Rows[i][19].ToString();
                                data.price = serviceDetails.Rows[i][20].ToString();
                                data.per = serviceDetails.Rows[i][21].ToString();
                                data.crcy = serviceDetails.Rows[i][22].ToString();

                                // Add the record in Database
                                _context.zpm01.Add(data);
                                _context.SaveChanges();
                            }
                        }
                    }
                }

                return Json(new { result = true, text = "Berhasil" });
            }
            catch (Exception)
            {
                return Json(new { result = false, text = "Harap Hubungi Administrator!!" });
            }
        }

		[AuthorizedAction]
		public IActionResult ImportZpm02(IFormFile file)
        {
            try
            {

                if (file == null)
                {
                    return Json(new { result = false, text = "File Tidak Ada!!" });
                }


                //// Create the Directory if it is not exist
                string dirPath = Path.Combine(hostEnvironment.WebRootPath, "ReceivedReports");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                // MAke sure that only Excel file is used 
                string dataFileName = Path.GetFileName(file.FileName);

                if (dataFileName != "zpm02")
                {
                    return Json(new { result = false, text = "Harap Sesuaikan Penamaan File!!" });
                }

                string extension = Path.GetExtension(dataFileName);

                string[] allowedExtsnions = new string[] { ".xls", ".xlsx" };

                if (!allowedExtsnions.Contains(extension))
                {
                    return Json(new { result = false, text = "Maaf! File ini tidak diperbolehkan, pastikan file berekstensi .xls atau .xlsx diupload." });
                }

                // Make a Copy of the Posted File from the Received HTTP Request
                string saveToPath = Path.Combine(dirPath, dataFileName);

                using (FileStream stream = new FileStream(saveToPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // USe this to handle Encodeing differences in .NET Core
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                // read the excel file
                using (var stream = new FileStream(saveToPath, FileMode.Open))
                {
                    if (extension == ".xls")
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    else
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                    DataSet ds = new DataSet();
                    ds = reader.AsDataSet();
                    reader.Close();

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        // Read the the Table
                        DataTable serviceDetails = ds.Tables[0];
                        for (int i = 1; i < serviceDetails.Rows.Count; i++)
                        {
                            Zpm02 obj = _context.zpm02.Where(p => p.purch_req == serviceDetails.Rows[i][3].ToString() && p.item == Convert.ToInt32(serviceDetails.Rows[i][4].ToString())).FirstOrDefault();
                            if (obj != null)
                            {
                                obj.plant = serviceDetails.Rows[i][0].ToString();
                                obj.material = serviceDetails.Rows[i][1].ToString();
                                obj.material_description = serviceDetails.Rows[i][2].ToString();
                                obj.purch_req = serviceDetails.Rows[i][3].ToString();
                                obj.item = Convert.ToInt32(serviceDetails.Rows[i][4].ToString());
                                obj.d = serviceDetails.Rows[i][5].ToString();
                                obj.rel = serviceDetails.Rows[i][6].ToString();
                                obj.s = serviceDetails.Rows[i][7].ToString();
                                obj.pgr = serviceDetails.Rows[i][8].ToString();
                                obj.tracking_no = serviceDetails.Rows[i][9].ToString();
                                obj.qty_req = Convert.ToInt32(serviceDetails.Rows[i][10].ToString());
                                obj.un = serviceDetails.Rows[i][11].ToString();
                                obj.req_date = Convert.ToDateTime(serviceDetails.Rows[i][12].ToString());
                                obj.valn_price = serviceDetails.Rows[i][13].ToString();
                                obj.crcy = serviceDetails.Rows[i][14].ToString();
                                obj.per = Convert.ToInt32(serviceDetails.Rows[i][15].ToString());
                                obj.release_dt = Convert.ToDateTime(serviceDetails.Rows[i][16].ToString());


                                _context.SaveChanges();

                            }
                            else
                            {

                                Zpm02 data = new Zpm02();
                                data.plant = serviceDetails.Rows[i][0].ToString();
                                data.material = serviceDetails.Rows[i][1].ToString();
                                data.material_description = serviceDetails.Rows[i][2].ToString();
                                data.purch_req = serviceDetails.Rows[i][3].ToString();
                                data.item = Convert.ToInt32(serviceDetails.Rows[i][4].ToString());
                                data.d = serviceDetails.Rows[i][5].ToString();
                                data.rel = serviceDetails.Rows[i][6].ToString();
                                data.s = serviceDetails.Rows[i][7].ToString();
                                data.pgr = serviceDetails.Rows[i][8].ToString();
                                data.tracking_no = serviceDetails.Rows[i][9].ToString();
                                data.qty_req = Convert.ToInt32(serviceDetails.Rows[i][10].ToString());
                                data.un = serviceDetails.Rows[i][11].ToString();
                                data.req_date = Convert.ToDateTime(serviceDetails.Rows[i][12].ToString());
                                data.valn_price = serviceDetails.Rows[i][13].ToString();
                                data.crcy = serviceDetails.Rows[i][14].ToString();
                                data.per = Convert.ToInt32(serviceDetails.Rows[i][15].ToString());
                                data.release_dt = Convert.ToDateTime(serviceDetails.Rows[i][16].ToString());


                                // Add the record in Database
                                _context.zpm02.Add(data);
                                _context.SaveChanges();
                            }
                        }
                    }
                }

                return Json(new { result = true, text = "Berhasil" });
            }
            catch (Exception)
            {
                return Json(new { result = false, text = "Harap Hubungi Administrator!!" });
            }
        }

		[AuthorizedAction]
		public IActionResult ImportZpm03(IFormFile file)
        {
            try
            {

                if (file == null)
                {
                    return Json(new { result = false, text = "File Tidak Ada!!" });
                }


                // Create the Directory if it is not exist
                string dirPath = Path.Combine(hostEnvironment.WebRootPath, "ReceivedReports");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                // MAke sure that only Excel file is used 
                string dataFileName = Path.GetFileName(file.FileName);

                if (dataFileName != "zpm03")
                {
                    return Json(new { result = false, text = "Harap Sesuaikan Penamaan File!" });
                }

                string extension = Path.GetExtension(dataFileName);

                string[] allowedExtsnions = new string[] { ".xls", ".xlsx" };

                if (!allowedExtsnions.Contains(extension))
                {
                    return Json(new { result = false, text = "Maaf! File ini tidak diperbolehkan, pastikan file berekstensi .xls atau .xlsx diupload." });
                }

                // Make a Copy of the Posted File from the Received HTTP Request
                string saveToPath = Path.Combine(dirPath, dataFileName);

                using (FileStream stream = new FileStream(saveToPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // USe this to handle Encodeing differences in .NET Core
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                // read the excel file
                using (var stream = new FileStream(saveToPath, FileMode.Open))
                {
                    if (extension == ".xls")
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    else
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                    DataSet ds = new DataSet();
                    ds = reader.AsDataSet();
                    reader.Close();

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        // Read the the Table
                        DataTable serviceDetails = ds.Tables[0];
                        for (int i = 1; i < serviceDetails.Rows.Count; i++)
                        {
                            Zpm01 obj = _context.zpm01.Where(p => p.no_order == serviceDetails.Rows[i][0].ToString() && p.material == serviceDetails.Rows[i][1].ToString() && p.revision == serviceDetails.Rows[i][3].ToString()).FirstOrDefault();
                            if (obj != null)
                            {
                                obj.pr = serviceDetails.Rows[i][4].ToString();
                                obj.itm_pr = serviceDetails.Rows[i][5].ToString();
                                obj.req_date = Convert.ToDateTime(serviceDetails.Rows[i][15].ToString());
                                obj.qty_pr = serviceDetails.Rows[i][16].ToString();
                               
                                _context.SaveChanges();

                            }
                        }
                    }
                }

                return Json(new { result = true, text = "Berhasil" });
            }
            catch (Exception)
            {
                return Json(new { result = false, text = "Harap Hubungi Administrator!!" });
            }
        }

		[AuthorizedAction]
		public IActionResult ImportZpm05(IFormFile file)
        {
            try
            {

                if (file == null)
                {
                    return Json(new { result = false, text = "File Tidak Ada!!" });
                }


                // Create the Directory if it is not exist
                string dirPath = Path.Combine(hostEnvironment.WebRootPath, "ReceivedReports");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                // MAke sure that only Excel file is used 
                string dataFileName = Path.GetFileName(file.FileName);

                if (dataFileName != "zpm05")
                {
                    return Json(new { result = false, text = "Harap Sesuaikan Penamaan File!!" });
                }

                string extension = Path.GetExtension(dataFileName);

                string[] allowedExtsnions = new string[] { ".xls", ".xlsx" };

                if (!allowedExtsnions.Contains(extension))
                {
                    return Json(new { result = false, text = "Maaf! File ini tidak diperbolehkan, pastikan file berekstensi .xls atau .xlsx diupload." });
                }

                // Make a Copy of the Posted File from the Received HTTP Request
                string saveToPath = Path.Combine(dirPath, dataFileName);

                using (FileStream stream = new FileStream(saveToPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // USe this to handle Encodeing differences in .NET Core
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                // read the excel file
                using (var stream = new FileStream(saveToPath, FileMode.Open))
                {
                    if (extension == ".xls")
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    else
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                    DataSet ds = new DataSet();
                    ds = reader.AsDataSet();
                    reader.Close();

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        // Read the the Table
                        DataTable serviceDetails = ds.Tables[0];
                        for (int i = 1; i < serviceDetails.Rows.Count; i++)
                        {
                            PurchOrder obj = _context.purch_order.Where(p => p.pr == serviceDetails.Rows[i][3].ToString() && p.item_pr == serviceDetails.Rows[i][4].ToString()).FirstOrDefault();
                            if (obj != null)
                            {
                                obj.gl_acct = serviceDetails.Rows[i][0].ToString();
                                obj.material = serviceDetails.Rows[i][1].ToString();
                                obj.short_text = serviceDetails.Rows[i][2].ToString();
                                obj.pr = serviceDetails.Rows[i][3].ToString();
                                obj.item_pr = serviceDetails.Rows[i][4].ToString();
                                obj.po = serviceDetails.Rows[i][5].ToString();
                                obj.item_po = serviceDetails.Rows[i][6].ToString();
                                obj.dci = serviceDetails.Rows[i][7].ToString();
                                obj.net_price = serviceDetails.Rows[i][9].ToString();
                                obj.crcy = serviceDetails.Rows[i][10].ToString();
                                obj.per = serviceDetails.Rows[i][11].ToString();
                                obj.oun = serviceDetails.Rows[i][13].ToString();
                                obj.doc_date = serviceDetails.Rows[i][14].ToString();
                                obj.deliv_date = serviceDetails.Rows[i][15].ToString();
                                obj.qty_delivered = serviceDetails.Rows[i][16].ToString();
                              
                                _context.SaveChanges();

                            }
                            else
                            {
                                PurchOrder data = new PurchOrder();
                                data.gl_acct = serviceDetails.Rows[i][0].ToString();
                                data.material = serviceDetails.Rows[i][1].ToString();
                                data.short_text = serviceDetails.Rows[i][2].ToString();
                                data.pr = serviceDetails.Rows[i][3].ToString();
                                data.item_pr = serviceDetails.Rows[i][4].ToString();
                                data.po = serviceDetails.Rows[i][5].ToString();
                                data.item_po = serviceDetails.Rows[i][6].ToString();
                                data.dci = serviceDetails.Rows[i][7].ToString();
                                data.net_price = serviceDetails.Rows[i][9].ToString();
                                data.crcy = serviceDetails.Rows[i][10].ToString();
                                data.per = serviceDetails.Rows[i][11].ToString();
                                data.oun = serviceDetails.Rows[i][13].ToString();
                                data.doc_date = serviceDetails.Rows[i][14].ToString();
                                data.deliv_date = serviceDetails.Rows[i][15].ToString();
                                data.qty_delivered = serviceDetails.Rows[i][16].ToString();

                                _context.purch_order.Add(data);
                                _context.SaveChanges();
                            }
                        }
                    }
                }

                return Json(new { result = true, text = "Berhasil" });
            }
            catch (Exception)
            {
                return Json(new { result = false, text = "Harap Hubungi Administrator!!" });
            }
        }

		[AuthorizedAction]
		public IActionResult ImportZpm07(IFormFile file)
        {
            try
            {

                if (file == null)
                {
                    return Json(new { result = false, text = "File Tidak Ada!!" });
                }


                // Create the Directory if it is not exist
                string dirPath = Path.Combine(hostEnvironment.WebRootPath, "ReceivedReports");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                // MAke sure that only Excel file is used 
                string dataFileName = Path.GetFileName(file.FileName);

                if (dataFileName != "zpm07")
                {
                    return Json(new { result = false, text = "Harap Sesuikan Penamaan File!!" });
                }

                string extension = Path.GetExtension(dataFileName);

                string[] allowedExtsnions = new string[] { ".xls", ".xlsx" };

                if (!allowedExtsnions.Contains(extension))
                {
                    return Json(new { result = false, text = "Maaf! File ini tidak diperbolehkan, pastikan file berekstensi .xls atau .xlsx diupload." });
                }

                // Make a Copy of the Posted File from the Received HTTP Request
                string saveToPath = Path.Combine(dirPath, dataFileName);

                using (FileStream stream = new FileStream(saveToPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // USe this to handle Encodeing differences in .NET Core
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                // read the excel file
                using (var stream = new FileStream(saveToPath, FileMode.Open))
                {
                    if (extension == ".xls")
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    else
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                    DataSet ds = new DataSet();
                    ds = reader.AsDataSet();
                    reader.Close();

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        // Read the the Table
                        DataTable serviceDetails = ds.Tables[0];
                        for (int i = 1; i < serviceDetails.Rows.Count; i++)
                        {
                            if(serviceDetails.Rows[i][6].ToString() == "")
                            {
                                PurchOrder obj = _context.purch_order.Where(p => p.pr == serviceDetails.Rows[i][3].ToString() && p.item_pr == serviceDetails.Rows[i][4].ToString()).FirstOrDefault();
                                if (obj != null)
                                {
                               
                                    obj.pr = serviceDetails.Rows[i][0].ToString();
                                    obj.item_pr = serviceDetails.Rows[i][1].ToString();
                                    obj.material = serviceDetails.Rows[i][2].ToString();
                                    obj.short_text = serviceDetails.Rows[i][3].ToString();
                                    obj.po = serviceDetails.Rows[i][4].ToString();
                                    obj.item_po = serviceDetails.Rows[i][5].ToString();
                                    obj.d = serviceDetails.Rows[i][6].ToString();
                                    obj.dci = serviceDetails.Rows[i][7].ToString();
                                    obj.pgr = serviceDetails.Rows[i][8].ToString();
                                    obj.doc_date = serviceDetails.Rows[i][9].ToString();
                                    obj.po_quantity = serviceDetails.Rows[i][10].ToString();
                                    obj.qty_delivered = serviceDetails.Rows[i][11].ToString();
                                    obj.deliv_date = serviceDetails.Rows[i][12].ToString();
                                    obj.oun = serviceDetails.Rows[i][13].ToString();
                                    obj.net_price = serviceDetails.Rows[i][14].ToString();
                                    obj.crcy = serviceDetails.Rows[i][15].ToString();
                                    obj.per = serviceDetails.Rows[i][16].ToString();
                               

                                    _context.SaveChanges();

                                }
                                else
                                {
                                    PurchOrder data = new PurchOrder();
                                    data.pr = serviceDetails.Rows[i][0].ToString();
                                    data.item_pr = serviceDetails.Rows[i][1].ToString();
                                    data.material = serviceDetails.Rows[i][2].ToString();
                                    data.short_text = serviceDetails.Rows[i][3].ToString();
                                    data.po = serviceDetails.Rows[i][4].ToString();
                                    data.item_po = serviceDetails.Rows[i][5].ToString();
                                    data.d = serviceDetails.Rows[i][6].ToString();
                                    data.dci = serviceDetails.Rows[i][7].ToString();
                                    data.pgr = serviceDetails.Rows[i][8].ToString();
                                    data.doc_date = serviceDetails.Rows[i][9].ToString();
                                    data.po_quantity = serviceDetails.Rows[i][10].ToString();
                                    data.qty_delivered = serviceDetails.Rows[i][11].ToString();
                                    data.deliv_date = serviceDetails.Rows[i][12].ToString();
                                    data.oun = serviceDetails.Rows[i][13].ToString();
                                    data.net_price = serviceDetails.Rows[i][14].ToString();
                                    data.crcy = serviceDetails.Rows[i][15].ToString();
                                    data.per = serviceDetails.Rows[i][16].ToString();

                                    _context.purch_order.Add(data);
                                    _context.SaveChanges();
                                }
                            }
                        }
                    }
                }

                return Json(new { result = true, text = "Berhasil" });
            }
            catch (Exception)
            {
                return Json(new { result = false, text = "Harap Hubungi Administrator!!" });
            }
        }

		[AuthorizedAction]
		public IActionResult ImportMb25(IFormFile file)
        {
            try
            {

                if (file == null)
                {
                    return Json(new { result = false, text = "File Tidak Ada!!" });
                }


                // Create the Directory if it is not exist
                string dirPath = Path.Combine(hostEnvironment.WebRootPath, "ReceivedReports");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                // MAke sure that only Excel file is used 
                string dataFileName = Path.GetFileName(file.FileName);

                if (dataFileName != "mb25")
                {
                    return Json(new { result = false, text = "Harap Sesuaikan Penamaan File!" });
                }

                string extension = Path.GetExtension(dataFileName);

                string[] allowedExtsnions = new string[] { ".xls", ".xlsx" };

                if (!allowedExtsnions.Contains(extension))
                {
                    return Json(new { result = false, text = "Maaf! File ini tidak diperbolehkan, pastikan file berekstensi .xls atau .xlsx diupload." });
                }

                // Make a Copy of the Posted File from the Received HTTP Request
                string saveToPath = Path.Combine(dirPath, dataFileName);

                using (FileStream stream = new FileStream(saveToPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }


                // USe this to handle Encodeing differences in .NET Core
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                // read the excel file
                using (var stream = new FileStream(saveToPath, FileMode.Open))
                {
                    if (extension == ".xls")
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    else
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                    DataSet ds = new DataSet();
                    ds = reader.AsDataSet();
                    reader.Close();

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        // Read the the Table
                        DataTable serviceDetails = ds.Tables[0];
                        for (int i = 1; i < serviceDetails.Rows.Count; i++)
                        {
                            Zpm01 obj = _context.zpm01.Where(p => p.no_order == serviceDetails.Rows[i][2].ToString() && p.itm == serviceDetails.Rows[i][0].ToString()).FirstOrDefault();
                            if (obj != null)
                            {
                                obj.diff_qty = serviceDetails.Rows[i][10].ToString();

                                _context.SaveChanges();

                            }
                        }
                    }
                }

                return Json(new { result = true, text = "Berhasil" });
            }
            catch (Exception)
            {
                return Json(new { result = false, text = "Harap Hubungi Administrator!!" });
            }
        }

		[AuthorizedAction]
		public IActionResult ImportPrInv(IFormFile file)
        {
            try
            {

                if (file == null)
                {
                    return Json(new { result = false, text = "File Tidak Ada!!" });
                }


                // Create the Directory if it is not exist
                string dirPath = Path.Combine(hostEnvironment.WebRootPath, "ReceivedReports");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                // MAke sure that only Excel file is used 
                string dataFileName = Path.GetFileName(file.FileName);

                if (dataFileName != "pr_inv")
                {
                    return Json(new { result = false, text = "Harap Sesuaikan Penamaan File!" });
                }

                string extension = Path.GetExtension(dataFileName);

                string[] allowedExtsnions = new string[] { ".xls", ".xlsx" };

                if (!allowedExtsnions.Contains(extension))
                {
                    return Json(new { result = false, text = "Maaf! File ini tidak diperbolehkan, pastikan file berekstensi .xls atau .xlsx diupload." });
                }

                // Make a Copy of the Posted File from the Received HTTP Request
                string saveToPath = Path.Combine(dirPath, dataFileName);

                using (FileStream stream = new FileStream(saveToPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }


                // USe this to handle Encodeing differences in .NET Core
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                // read the excel file
                using (var stream = new FileStream(saveToPath, FileMode.Open))
                {
                    if (extension == ".xls")
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    else
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                    DataSet ds = new DataSet();
                    ds = reader.AsDataSet();
                    reader.Close();

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        // Read the the Table
                        DataTable serviceDetails = ds.Tables[0];
                        for (int i = 1; i < serviceDetails.Rows.Count; i++)
                        {
                            Zpm01 obj = _context.zpm01.Where(p => p.no_order == serviceDetails.Rows[i][0].ToString() && p.itm == serviceDetails.Rows[i][1].ToString()).FirstOrDefault();
                            if (obj != null)
                            {
                                obj.qty_res = serviceDetails.Rows[i][6].ToString();
                                obj.pr = serviceDetails.Rows[i][7].ToString();
                                obj.itm_pr = serviceDetails.Rows[i][8].ToString();
                                obj.qty_pr = serviceDetails.Rows[i][9].ToString();

                                _context.SaveChanges();

                            }
                        }
                    }
                }

                return Json(new { result = true, text = "Berhasil" });
            }
            catch (Exception)
            {
                return Json(new { result = false, text = "Harap Hubungi Administrator!!" });
            }
        }
    }
}
