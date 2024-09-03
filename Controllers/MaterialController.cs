using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using mystap.Models;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
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
        private readonly IMemoryCache _memoryCache;
        public IActionResult Material()
        {
           
            return View();
        }

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

                var project_filter = Request.Form["project"].FirstOrDefault();
                var project_rev = Request.Form["project_rev"].FirstOrDefault();
                var rev = Request.Form["rev"].FirstOrDefault();
                var filter = Request.Form["filter"].FirstOrDefault();
                var lldi = Request.Form["lldi"].FirstOrDefault();

                var w = "";
                var ws = "";

                var query = @"select " +
                           " b.*, " +
                           " (case " +
                           "     when b.status_ = 'terpenuhi_stock' or b.status_ = 'onsite' then 'ready' " +
                           "     else (case  " +
                           "         when CONVERT (DATE, GETDATE()) > b.finish_date then 'delay' " +
                           "         when b.prognosa_ < b.md then 'on_track' " +
                           "         else 'delay' " +
                           "     end) " +
                           "  end) as status_ready from (select work_order.[order],max(work_order.[description]) as wo_description,zpm01.material,max(zpm01.material_description) as [description],zpm01.itm,max(zpm01.bun)as bun,max(zpm01.reqmt_qty) as reqmt_qty, " +
                           "  max(zpm01.pr) as pr,max(zpm01.itm_pr) as pr_item,max(zpm01.qty_pr) as pr_qty,max(zpm01.qty_res) as qty_res,max(purch_order.po) as po,max(purch_order.qty_delivered) as po_qty,max(purch_order.item_po) as po_item,max(purch_order.dci) as dci, " +
                           " max(project.tglSelesaiTA) as finish_date, (case when max(project.taoh) = 'OH' then DATEADD(DAY, (case when max(zpm01.prognosa_matl) is not null then max(zpm01.prognosa_matl) else 0 end),max(prognosa_oh.[start_date])) else max(project.tglTA) end) as md, " +
                           " (case when max(purch_order.po) is not null then DATEDIFF(day, max(purch_order.doc_date),max(purch_order.deliv_date))  else (case  when max(zpm01.dt_purch) is not null or max(zpm01.dt_purch) != '' then max(zpm01.dt_purch)  when max(zpm01.dt_iv) is not null or max(zpm01.dt_iv) != '' then max(zpm01.dt_iv) else ( case when max(zpm01.dt_ta) is not null or max(zpm01.dt_ta) != '' then max(zpm01.dt_ta) else 0 end)  end)  end) as dt_ , " +
                           " (case                    when max(zpm01.unloading_point) = 'XX' then 'LLD'                   else 'Non LLD'                end) as lld, " +
                           " (case when max(purch_order.po) is not null then max(purch_order.deliv_date) else DATEADD(DAY, ((case when max(zpm01.dt_purch) is not null or max(zpm01.dt_purch) != '' then max(zpm01.dt_purch) when max(zpm01.dt_iv) is not null or max(zpm01.dt_iv) != '' then max(zpm01.dt_iv) else (case when max(zpm01.dt_ta) is not null or max(zpm01.dt_ta) != '' then max(zpm01.dt_ta) else 0 end) end) + (case when max(zpm01.dt_status_pengadaan) is not null or max(zpm01.dt_status_pengadaan) != '' then max(zpm01.dt_status_pengadaan) else 52 end)), CONVERT (DATE, GETDATE())) end) as prognosa_, " +
                           " (case when max(zpm01.pr) is null or max(zpm01.pr) = '' then (case when max(zpm01.reqmt_qty) = max(zpm01.qty_res) then 'terpenuhi_stock' else 'create_pr' end) else (case when (max(zpm01.pr) is not null or max(zpm01.pr) != '') and (max(purch_order.po) is not null or max(purch_order.po) != '') then (case when max(purch_order.dci) is null or max(purch_order.dci) = '' then 'tunggu_onsite' else 'onsite'  end) when (max(zpm01.status_pengadaan) is not null or max(zpm01.status_pengadaan) != '') then max(zpm01.status_pengadaan) else 'outstanding_pr' end) end) as status_ from Mystap.dbo.zpm01 " +
                           " left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order " +
                           " left join Mystap.dbo.joblist_detail_wo on joblist_detail_wo.[order] = work_order.[order] left join Mystap.dbo.joblist_detail on joblist_detail.id = joblist_detail_wo.jobListDetailID left join Mystap.dbo.joblist on joblist.id = joblist_detail.joblist_id   left join Mystap.dbo.prognosa_oh on prognosa_oh.id_joblist = joblist.id  " +
                           " left join Mystap.dbo.project on project.revision = work_order.revision " +
                           " left join Mystap.dbo.purch_order on purch_order.material = zpm01.material AND purch_order.pr = zpm01.pr AND purch_order.item_pr = zpm01.itm_pr where ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != 0)) " +
                           w +
                           " group by work_order.[order],zpm01.material, zpm01.itm) as b " + ws;

                var c = FormattableStringFactory.Create(query);
                var datas = _context.zpm01.FromSql(c);

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
                    datas = datas.Where(m => m.no_order.StartsWith(searchValue));
                }
                // Total number of rows count
                recordsTotal = datas.Count();
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

        public IActionResult Outstanding_Reservasi()
        {

            return View();
        }

        public async Task<IActionResult> Get_Outstanding_Reservasi()
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

                var project_filter = Request.Form["project"].FirstOrDefault();
                var project_rev = Request.Form["project_rev"].FirstOrDefault();
                var rev = Request.Form["rev"].FirstOrDefault();
                var filter = Request.Form["filter"].FirstOrDefault();
                var lldi = Request.Form["lldi"].FirstOrDefault();

                var w = "";
                var ws = "";

                var query = @"select " +
                           " b.*, " +
                           " (case " +
                           "     when b.status_ = 'terpenuhi_stock' or b.status_ = 'onsite' then 'ready' " +
                           "     else (case  " +
                           "         when CONVERT (DATE, GETDATE()) > b.finish_date then 'delay' " +
                           "         when b.prognosa_ < b.md then 'on_track' " +
                           "         else 'delay' " +
                           "     end) " +
                           "  end) as status_ready from (select work_order.[order],max(work_order.[description]) as wo_description,zpm01.material,max(zpm01.material_description) as [description],zpm01.itm,max(zpm01.bun)as bun,max(zpm01.reqmt_qty) as reqmt_qty, " +
                           "  max(zpm01.pr) as pr,max(zpm01.itm_pr) as pr_item,max(zpm01.qty_pr) as pr_qty,max(zpm01.qty_res) as qty_res,max(purch_order.po) as po,max(purch_order.qty_delivered) as po_qty,max(purch_order.item_po) as po_item,max(purch_order.dci) as dci, " +
                           " max(project.tglSelesaiTA) as finish_date, (case when max(project.taoh) = 'OH' then DATEADD(DAY, (case when max(zpm01.prognosa_matl) is not null then max(zpm01.prognosa_matl) else 0 end),max(prognosa_oh.[start_date])) else max(project.tglTA) end) as md, " +
                           " (case when max(purch_order.po) is not null then DATEDIFF(day, max(purch_order.doc_date),max(purch_order.deliv_date))  else (case  when max(zpm01.dt_purch) is not null or max(zpm01.dt_purch) != '' then max(zpm01.dt_purch)  when max(zpm01.dt_iv) is not null or max(zpm01.dt_iv) != '' then max(zpm01.dt_iv) else ( case when max(zpm01.dt_ta) is not null or max(zpm01.dt_ta) != '' then max(zpm01.dt_ta) else 0 end)  end)  end) as dt_ , " +
                           " (case                    when max(zpm01.unloading_point) = 'XX' then 'LLD'                   else 'Non LLD'                end) as lld, " +
                           " (case when max(purch_order.po) is not null then max(purch_order.deliv_date) else DATEADD(DAY, ((case when max(zpm01.dt_purch) is not null or max(zpm01.dt_purch) != '' then max(zpm01.dt_purch) when max(zpm01.dt_iv) is not null or max(zpm01.dt_iv) != '' then max(zpm01.dt_iv) else (case when max(zpm01.dt_ta) is not null or max(zpm01.dt_ta) != '' then max(zpm01.dt_ta) else 0 end) end) + (case when max(zpm01.dt_status_pengadaan) is not null or max(zpm01.dt_status_pengadaan) != '' then max(zpm01.dt_status_pengadaan) else 52 end)), CONVERT (DATE, GETDATE())) end) as prognosa_, " +
                           " (case when max(zpm01.pr) is null or max(zpm01.pr) = '' then (case when max(zpm01.reqmt_qty) = max(zpm01.qty_res) then 'terpenuhi_stock' else 'create_pr' end) else (case when (max(zpm01.pr) is not null or max(zpm01.pr) != '') and (max(purch_order.po) is not null or max(purch_order.po) != '') then (case when max(purch_order.dci) is null or max(purch_order.dci) = '' then 'tunggu_onsite' else 'onsite'  end) when (max(zpm01.status_pengadaan) is not null or max(zpm01.status_pengadaan) != '') then max(zpm01.status_pengadaan) else 'outstanding_pr' end) end) as status_ from Mystap.dbo.zpm01 " +
                           " left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order " +
                           " left join Mystap.dbo.joblist_detail_wo on joblist_detail_wo.[order] = work_order.[order] left join Mystap.dbo.joblist_detail on joblist_detail.id = joblist_detail_wo.jobListDetailID left join Mystap.dbo.joblist on joblist.id = joblist_detail.joblist_id   left join Mystap.dbo.prognosa_oh on prognosa_oh.id_joblist = joblist.id  " +
                           " left join Mystap.dbo.project on project.revision = work_order.revision " +
                           " left join Mystap.dbo.purch_order on purch_order.material = zpm01.material AND purch_order.pr = zpm01.pr AND purch_order.item_pr = zpm01.itm_pr where ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != 0)) " +
                           w +
                           " group by work_order.[order],zpm01.material, zpm01.itm) as b " + ws;

                var c = FormattableStringFactory.Create(query);
                var datas = _context.zpm01.FromSql(c);

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
                    datas = datas.Where(m => m.no_order.StartsWith(searchValue));
                }
                // Total number of rows count
                recordsTotal = datas.Count();
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
    }
}
