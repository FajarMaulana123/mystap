using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using mystap.Helpers;
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
    public class DashboardController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IMemoryCache _memoryCache;

        public DashboardController(DatabaseContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

		[AuthorizedAction]
		public IActionResult DashboardEquipment()

        {
            ViewBag.role = "DASHBOARD_EQUIPMENT";
            if (Module.hasModule("DASHBOARD_EQUIPMENT", HttpContext.Session))
            {
                ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == 1).ToList();
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
            else
            {
                return NotFound();
            }
            
        }

		[AuthorizedAction]
		public async Task<IActionResult> GetReadinessEquipment()
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

                var filter = Request.Form["columns[2][search][value]"].FirstOrDefault();

                var project_filter = Request.Form["project_filter"].FirstOrDefault();
                var project_rev = Request.Form["project_rev"].FirstOrDefault();
                // _context.Database.SetCommandTimeout(TimeSpan.FromMinutes(20));

                //var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);

                var customerData = _context.view_readiness_equipment.FromSql($"select max(joblist.id) as id, max(project.id) as projectID, max(project.revision) as revision, max(joblist.jobNo) as jobNo, c.eqTagNo, CASE WHEN count(case when  c.status = 'not_ready' then c.id else NULL end) > '0' THEN 'not_ready' WHEN max(c.status) is null then 'not_ready' ELSE 'ready' END AS 'status_tagno' from Mystap.dbo.joblist  left join (SELECT  b.id, b.eqTagNo,  b.projectID,  b.joblist_id, (CASE WHEN b.dikerjakan = 'tidak' then 'ready' WHEN b.jasa != '0' AND b.material != '0' THEN CASE WHEN b.sts_material = 'ready' AND b.sts_kontrak = 'ready' THEN 'ready'  ELSE 'not_ready' END WHEN b.material != '0' THEN isnull(b.sts_material, 'not_ready') WHEN b.jasa != '0' THEN isnull(b.sts_kontrak, 'not_ready') END) as status FROM (SELECT a.id, a.joblist_id, equipments.eqTagNo, joblist.projectID, a.jobDesc, a.jasa, a.material, a.dikerjakan, (CASE WHEN a.dikerjakan = 'tidak' then 'not_execute' WHEN a.jasa != '0' THEN  (SELECT (CASE WHEN contracttracking.aktualSP is null THEN 'not_ready' ELSE 'ready' END) as status FROM Mystap.dbo.contracttracking left join Mystap.dbo.joblist_detail on joblist_detail.no_jasa = contracttracking.idPaket where contracttracking.projectID = { project_filter } and joblist_detail.id = a.id and contracttracking.deleted != '1') ELSE 'not_identify' END) as sts_kontrak, (CASE WHEN a.dikerjakan = 'tidak' then 'not_execute' WHEN a.material != '0' THEN (CASE WHEN a.status_material = 'COMPLETED' AND (SELECT (CASE WHEN joblist_detail.material != '0' THEN CASE WHEN count(DISTINCT case when procurement_.status_ != 'terpenuhi_stock' AND procurement_.status_ != 'onsite' then procurement_.[order] else NULL end) > '0' THEN 'not_ready' ELSE 'ready' END ELSE NULL END) as sts_material FROM (SELECT work_order.[order], (case when zpm01.pr is null or zpm01.pr = '' then (case when zpm01.reqmt_qty = zpm01.qty_res then 'terpenuhi_stock' else 'create_pr' end) else (case when (zpm01.pr is not null or zpm01.pr != '') and (p.po is null or p.po = '')then (case when (zpm01.status_pengadaan = 'tunggu_pr' or zpm01.status_pengadaan = 'evaluasi_dp3' or zpm01.status_pengadaan is null) then 'outstanding_pr' when zpm01.status_pengadaan = 'inquiry_harga' then 'inquiry_harga' when (zpm01.status_pengadaan = 'hps_oe' or zpm01.status_pengadaan = 'bidder_list' or zpm01.status_pengadaan = 'penilaian_kualifikasi' or zpm01.status_pengadaan = 'rfq') then 'hps_oe' when (zpm01.status_pengadaan = 'pemasukan_penawaran' or zpm01.status_pengadaan = 'pembukaan_penawaran' or zpm01.status_pengadaan = 'evaluasi_penawaran' or zpm01.status_pengadaan = 'klarifikasi_spesifikasi' or zpm01.status_pengadaan = 'evaluasi_teknis' or zpm01.status_pengadaan = 'evaluasi_tkdn' or zpm01.status_pengadaan = 'negosiasi'  or zpm01.status_pengadaan = 'lhp') then 'proses_tender' when (zpm01.status_pengadaan = 'pengumuman_pemenang' or zpm01.status_pengadaan = 'penunjuk_pemenang' or zpm01.status_pengadaan = 'purchase_order') then 'Penetapan Pemenang' else 'outstanding_pr' end) when (zpm01.pr is not null or zpm01.pr != '') and (p.po is not null or p.po != '') then (case when p.dci is null or p.dci = '' then 'tunggu_onsite' else 'onsite' end) end) end) as status_ FROM Mystap.dbo.zpm01 left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order left join Mystap.dbo.purch_order as p on p.material = zpm01.material AND p.pr = zpm01.pr AND p.item_pr = zpm01.itm_pr where work_order.revision = {project_rev} and ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) GROUP BY [work_order].[order],zpm01.itm, zpm01.pr,zpm01.reqmt_qty, zpm01.qty_res, po, zpm01.status_pengadaan, dci) procurement_ join Mystap.dbo.joblist_detail_wo on joblist_detail_wo.[order] = procurement_.[order] join Mystap.dbo.joblist_detail on joblist_detail.id = joblist_detail_wo.jobListDetailID where joblist_detail.id = a.id GROUP by joblist_detail.material ) = 'ready' then 'ready' ELSE 'not_ready' END) ELSE 'not_identify' END) as sts_material FROM Mystap.dbo.joblist_detail a LEFT JOIN Mystap.dbo.joblist on joblist.id = a.joblist_id  LEFT JOIN Mystap.dbo.equipments on equipments.id = joblist.id_eqTagNo where joblist.projectID = { project_filter } and a.deleted != '1') b) c on c.joblist_id = joblist.id LEFT JOIN Mystap.dbo.project on project.id = joblist.projectID  where joblist.projectID = { project_filter } and joblist.deleted != '1' group by c.eqTagNo");

                
                // Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.jobNo.StartsWith(searchValue) || m.eqTagNo.StartsWith(searchValue) || m.status_tagno.StartsWith(searchValue));
                }

                if (!string.IsNullOrEmpty(filter))
                {
                    customerData = customerData.Where(m => m.status_tagno == filter);
                }


                // Total number of rows count
                //Console.WriteLine(customerData);
                //recordsTotal = customerData.Count();
                // Paging
                var datas = await customerData.ToListAsync();
                //var data = _memoryCache.Get("products");
                //data = await _memoryCache.Set("products", datas, expirationTime);
                // Returning Json Data
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = datas });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public async Task<IActionResult> GrafikReadinessEquipment()
        {
            try
            {
                var project_filter = Request.Form["project_filter"].FirstOrDefault();
                var project_rev = Request.Form["project_rev"].FirstOrDefault();
                var customerData = _context.view_grafik_readiness.FromSql($"SELECT case when d.status_tagno = 'ready' then 'READY' else 'NOT READY' end as status_tagno,count(d.id) as total FROM ( select max(joblist.id) as id, max(project.id) as projectID, max(project.revision) as revision, max(joblist.jobNo) as jobNo, c.eqTagNo, CASE WHEN count(case when  c.status = 'not_ready' then c.id else NULL end) > '0' THEN 'not_ready' WHEN max(c.status) is null then 'not_ready' ELSE 'ready' END AS 'status_tagno' from Mystap.dbo.joblist  left join (SELECT  b.id, b.eqTagNo,  b.projectID,  b.joblist_id, (CASE WHEN b.dikerjakan = 'tidak' then 'ready' WHEN b.jasa != '0' AND b.material != '0' THEN CASE WHEN b.sts_material = 'ready' AND b.sts_kontrak = 'ready' THEN 'ready'  ELSE 'not_ready' END WHEN b.material != '0' THEN isnull(b.sts_material, 'not_ready') WHEN b.jasa != '0' THEN isnull(b.sts_kontrak, 'not_ready') END) as status FROM (SELECT a.id, a.joblist_id, equipments.eqTagNo, joblist.projectID, a.jobDesc, a.jasa, a.material, a.dikerjakan, (CASE WHEN a.dikerjakan = 'tidak' then 'not_execute' WHEN a.jasa != '0' THEN  (SELECT (CASE WHEN contracttracking.aktualSP is null THEN 'not_ready' ELSE 'ready' END) as status FROM Mystap.dbo.contracttracking left join Mystap.dbo.joblist_detail on joblist_detail.no_jasa = contracttracking.idPaket where contracttracking.projectID = {project_filter} and joblist_detail.id = a.id and contracttracking.deleted != '1') ELSE 'not_identify' END) as sts_kontrak, (CASE WHEN a.dikerjakan = 'tidak' then 'not_execute' WHEN a.material != '0' THEN (CASE WHEN a.status_material = 'COMPLETED' AND (SELECT (CASE WHEN joblist_detail.material != '0' THEN CASE WHEN count(DISTINCT case when procurement_.status_ != 'terpenuhi_stock' AND procurement_.status_ != 'onsite' then procurement_.[order] else NULL end) > '0' THEN 'not_ready' ELSE 'ready' END ELSE NULL END) as sts_material FROM (SELECT work_order.[order], (case when zpm01.pr is null or zpm01.pr = '' then (case when zpm01.reqmt_qty = zpm01.qty_res then 'terpenuhi_stock' else 'create_pr' end) else (case when (zpm01.pr is not null or zpm01.pr != '') and (p.po is null or p.po = '')then (case when (zpm01.status_pengadaan = 'tunggu_pr' or zpm01.status_pengadaan = 'evaluasi_dp3' or zpm01.status_pengadaan is null) then 'outstanding_pr' when zpm01.status_pengadaan = 'inquiry_harga' then 'inquiry_harga' when (zpm01.status_pengadaan = 'hps_oe' or zpm01.status_pengadaan = 'bidder_list' or zpm01.status_pengadaan = 'penilaian_kualifikasi' or zpm01.status_pengadaan = 'rfq') then 'hps_oe' when (zpm01.status_pengadaan = 'pemasukan_penawaran' or zpm01.status_pengadaan = 'pembukaan_penawaran' or zpm01.status_pengadaan = 'evaluasi_penawaran' or zpm01.status_pengadaan = 'klarifikasi_spesifikasi' or zpm01.status_pengadaan = 'evaluasi_teknis' or zpm01.status_pengadaan = 'evaluasi_tkdn' or zpm01.status_pengadaan = 'negosiasi'  or zpm01.status_pengadaan = 'lhp') then 'proses_tender' when (zpm01.status_pengadaan = 'pengumuman_pemenang' or zpm01.status_pengadaan = 'penunjuk_pemenang' or zpm01.status_pengadaan = 'purchase_order') then 'Penetapan Pemenang' else 'outstanding_pr' end) when (zpm01.pr is not null or zpm01.pr != '') and (p.po is not null or p.po != '') then (case when p.dci is null or p.dci = '' then 'tunggu_onsite' else 'onsite' end) end) end) as status_ FROM Mystap.dbo.zpm01 left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order left join Mystap.dbo.purch_order as p on p.material = zpm01.material AND p.pr = zpm01.pr AND p.item_pr = zpm01.itm_pr where work_order.revision = {project_rev} and ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) GROUP BY [work_order].[order],zpm01.itm, zpm01.pr,zpm01.reqmt_qty, zpm01.qty_res, po, zpm01.status_pengadaan, dci) procurement_ join Mystap.dbo.joblist_detail_wo on joblist_detail_wo.[order] = procurement_.[order] join Mystap.dbo.joblist_detail on joblist_detail.id = joblist_detail_wo.jobListDetailID where joblist_detail.id = a.id GROUP by joblist_detail.material ) = 'ready' then 'ready' ELSE 'not_ready' END) ELSE 'not_identify' END) as sts_material FROM Mystap.dbo.joblist_detail a LEFT JOIN Mystap.dbo.joblist on joblist.id = a.joblist_id LEFT JOIN Mystap.dbo.equipments on equipments.id = joblist.id_eqTagNo where joblist.projectID = {project_filter} and a.deleted != '1') b) c on c.joblist_id = joblist.id LEFT JOIN Mystap.dbo.project on project.projectNo = joblist.projectNo  where joblist.projectID = {project_filter} and joblist.deleted != '1' group by c.eqTagNo) d GROUP by d.status_tagno;");
                var data = await customerData.ToListAsync();
                return Json(new {data = data});
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public async Task<IActionResult> GetEquipment()
        {
            try
            {
                var project = Request.Form["project"].FirstOrDefault();
                var unit = Request.Form["unit"].FirstOrDefault();
                var query = (from j in _context.joblist
                             join p in _context.project on j.projectNo equals p.projectNo
                             join e in _context.equipments on j.id_eqTagNo equals e.id
                             join c in _context.catalogProfile on e.catalog_profile equals c.code
                             join u in _context.unit on j.unitCode equals u.id
                             select new
                             {
                                 unitCode = u.unitCode,
                                 unitKilang = u.unitKilang,
                                 projectId = p.id,
                                 id = j.id,
                                 vessel = (c.groups == "VESSEL" ? (long?)j.id : null),
                                 heat_exchanger = (c.groups == "HEAT EXCHANGER" ? (long?)j.id : null),
                                 heater = (c.groups == "HEATER" ? (long?)j.id : null),
                                 instrument = (c.groups == "INSTRUMENT" ? (long?)j.id : null),
                                 listrik = (c.groups == "LISTRIK" ? (long?)j.id : null),
                                 piping_system = (c.groups == "PIPING SYSTEM" ? (long?)j.id : null),
                                 rotating_equipment = (c.groups == "ROTATING EQUIPMENT" ? (long?)j.id : null),
                                 other = (c.groups != "VESSEL" && c.groups != "HEAT EXCHANGER" && c.groups != "HEATER" && c.groups != "INSTRUMENT" && c.groups != "LISTRIK" && c.groups != "PIPING SYSTEM" && c.groups != "ROTATING EQUIPMENT" ? (long?)j.id : null),

                             });

                if (project != "")
                {
                    query = query.Where(w => w.projectId == Convert.ToInt32(project));
                }

                if (unit != "")
                {
                    query = query.Where(w => w.unitCode == unit);
                }

                var data = await query.GroupBy(g => new
                             {
                                 g.unitCode,
                                 g.unitKilang
                             })
                             .Select(z => new
                             {
                                 unitCode = z.Key.unitCode,
                                 unitKilang = z.Key.unitKilang,
                                 vessel = z.Select(p => p.vessel).Distinct().Count(),
                                 heatExchanger = z.Select(p => p.heat_exchanger).Distinct().Count(),
                                 heater = z.Select(p => p.heater).Distinct().Count(),
                                 instrument = z.Select(p => p.instrument).Distinct().Count(),
                                 listrik = z.Select(p => p.listrik).Distinct().Count(),
                                 pipingSystem = z.Select(p => p.piping_system).Distinct().Count(),
                                 rotatingEquipment = z.Select(p => p.rotating_equipment).Distinct().Count(),
                                 other = z.Select(p => p.other).Distinct().Count(),
                                 total = z.Select(p => p.id).Distinct().Count(),
                             }).ToListAsync();

                return Json(data);
            }
            catch 
            {
                throw;
            }
        }

		[AuthorizedAction]
		public async Task<IActionResult> ReadinessDetail(int projectId, string rev, int joblist)
        {
            try
            {
                var query = _context.view_readiness_detail.FromSql($"SELECT a.id, a.joblist_id, equipments.eqTagNo, STUFF((SELECT distinct  t2.[order] + ',' from joblist_detail_wo t2  where a.id= t2.jobListDetailID FOR XML PATH(''), TYPE ).value('.', 'NVARCHAR(MAX)')  ,1,0,'') wo,a.no_jasa, joblist.projectID, a.jobDesc, a.jasa, a.material, (CASE WHEN a.dikerjakan = 'tidak' then 'not_identify' WHEN a.jasa != '0' THEN  (SELECT (CASE WHEN contracttracking.aktualSP is null THEN 'not_ready' ELSE 'ready' END) as status FROM Mystap.dbo.contracttracking left join Mystap.dbo.joblist_detail on joblist_detail.no_jasa = contracttracking.idPaket where contracttracking.projectID = {projectId} and joblist_detail.id = a.id and contracttracking.deleted != '1') ELSE 'not_identify' END) as sts_kontrak, (CASE WHEN a.dikerjakan = 'tidak' then 'not_identify' WHEN a.material != '0' THEN (CASE WHEN a.status_material = 'COMPLETED' AND (SELECT (CASE WHEN joblist_detail.material != '0' THEN CASE WHEN count(DISTINCT case when procurement_.status_ != 'terpenuhi_stock' AND procurement_.status_ != 'onsite' then procurement_.[order] else NULL end) > '0' THEN 'not_ready' ELSE 'ready' END ELSE NULL END) as sts_material FROM (SELECT work_order.[order], (case when zpm01.pr is null or zpm01.pr = '' then (case when zpm01.reqmt_qty = zpm01.qty_res then 'terpenuhi_stock' else 'create_pr' end) else (case when (zpm01.pr is not null or zpm01.pr != '') and (p.po is null or p.po = '')then (case when (zpm01.status_pengadaan = 'tunggu_pr' or zpm01.status_pengadaan = 'evaluasi_dp3' or zpm01.status_pengadaan is null) then 'outstanding_pr' when zpm01.status_pengadaan = 'inquiry_harga' then 'inquiry_harga' when (zpm01.status_pengadaan = 'hps_oe' or zpm01.status_pengadaan = 'bidder_list' or zpm01.status_pengadaan = 'penilaian_kualifikasi' or zpm01.status_pengadaan = 'rfq') then 'hps_oe' when (zpm01.status_pengadaan = 'pemasukan_penawaran' or zpm01.status_pengadaan = 'pembukaan_penawaran' or zpm01.status_pengadaan = 'evaluasi_penawaran' or zpm01.status_pengadaan = 'klarifikasi_spesifikasi' or zpm01.status_pengadaan = 'evaluasi_teknis' or zpm01.status_pengadaan = 'evaluasi_tkdn' or zpm01.status_pengadaan = 'negosiasi'  or zpm01.status_pengadaan = 'lhp') then 'proses_tender' when (zpm01.status_pengadaan = 'pengumuman_pemenang' or zpm01.status_pengadaan = 'penunjuk_pemenang' or zpm01.status_pengadaan = 'purchase_order') then 'Penetapan Pemenang' else 'outstanding_pr' end) when (zpm01.pr is not null or zpm01.pr != '') and (p.po is not null or p.po != '') then (case when p.dci is null or p.dci = '' then 'tunggu_onsite' else 'onsite' end) end) end) as status_ FROM Mystap.dbo.zpm01 left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order left join Mystap.dbo.purch_order as p on p.material = zpm01.material AND p.pr = zpm01.pr AND p.item_pr = zpm01.itm_pr where work_order.revision = {rev} and ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) GROUP BY [work_order].[order],zpm01.itm, zpm01.pr,zpm01.reqmt_qty, zpm01.qty_res, po, zpm01.status_pengadaan, dci) procurement_ join Mystap.dbo.joblist_detail_wo on joblist_detail_wo.[order] = procurement_.[order] join Mystap.dbo.joblist_detail on joblist_detail.id = joblist_detail_wo.jobListDetailID where joblist_detail.id = a.id GROUP by joblist_detail.material ) = 'ready' then 'ready' ELSE 'not_ready' END) ELSE 'not_identify' END) as sts_material FROM Mystap.dbo.joblist_detail a LEFT JOIN Mystap.dbo.joblist on joblist.id = a.joblist_id LEFT JOIN Mystap.dbo.equipments on equipments.id = joblist.id_eqTagNo where joblist.projectID = {projectId} and joblist.id = {joblist}  and a.deleted != '1'");
                var data = await query.ToListAsync();

                var isi = "<table class='table'><tr><th>Desc</th><th>Jasa</th><th> Material </th></tr>";
                foreach (var val in data) {
                    var sts_kontrak = "";
                    var sts_material = "";
                    if (val.sts_kontrak == "ready") {
                        sts_kontrak = "<span class='badge bg-success detail_kontrak' data-jasa='"+ val.no_jasa+"'>READY</span>'";
                    } else if (val.sts_kontrak == "not_ready") {
                        sts_kontrak = "<span class='badge bg-danger detail_kontrak' data-jasa='"+ val.no_jasa+"'>NOT READY</span>'";
                    } else if (val.sts_kontrak == "not_identify") {
                        sts_kontrak = "N/R";
                    } else
                    {
                        sts_kontrak = "<span class='badge bg-secondary'>UNDEFINED</span>";
                    }


                    if (val.sts_material == "ready") {
                        sts_material = "<span class='badge bg-success detail_material' data-id='"+ val.wo+ "'>READY</span>'";
                    } else if (val.sts_material == "not_ready") {
                        sts_material = "<span class='badge bg-danger detail_material' data-id='"+ val.wo+ "'>NOT READY</span>'";
                    } else if (val.sts_material == "not_identify") {
                        sts_material = "N/R";
                    } else
                    {
                        sts_material = "<span class='badge bg-secondary'>UNDEFINED</span>";
                    }


                    isi += "<tr><td>" + val.jobDesc +"</td><td> " + sts_kontrak +" </td><td> " + sts_material +" </td> </tr> ";
                }
                isi += "</table>";


                return base.Content(isi, "text/html");
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public async Task<IActionResult> OrderDetail()
        {
            try
            {
                var id = Request.Form["id"].FirstOrDefault();
                if(id != "")
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

                    var query = _context.view_detail_order.FromSql($"select work_order.main_work_ctr,work_order.[order],work_order.[description],max(zpm01.material) as material,zpm01.itm,zpm01.material_description,zpm01.del,zpm01.unloading_point,zpm01.bun,zpm01.reqmt_date,zpm01.reqmt_qty,zpm01.qty_res,zpm01.pr as pr,zpm01.itm_pr as pr_item,zpm01.qty_pr as pr_qty,zpm01.status_pengadaan,purch_order.po,purch_order.po_quantity as po_qty,purch_order.item_po as po_item, purch_order.deliv_date as deliv_date,purch_order.dci as dci, (case when purch_order.po is not null then purch_order.deliv_date else DATEADD(DAY, ((case when zpm01.dt_purch is not null or zpm01.dt_purch != '' then zpm01.dt_purch when zpm01.dt_iv is not null or zpm01.dt_iv != '' then zpm01.dt_iv else (case when zpm01.dt_ta is not null or zpm01.dt_ta != '' then zpm01.dt_ta else 0 end) end) + (case when zpm01.dt_status_pengadaan is not null or zpm01.dt_status_pengadaan != '' then zpm01.dt_status_pengadaan else 52 end)), CONVERT (DATE, GETDATE())) end)as prognosa_, (case when zpm01.unloading_point = 'XX' then 'LLD' else 'Non LLD' end) as lld, (case when zpm01.pr is null or zpm01.pr = '' then (case when zpm01.reqmt_qty = zpm01.qty_res then 'terpenuhi_stock' else 'create_pr' end) else (case when (zpm01.pr is not null or zpm01.pr != '') and (purch_order.po is null or purch_order.po = '') then (case when (zpm01.status_pengadaan = 'tunggu_pr' or zpm01.status_pengadaan = 'evaluasi_dp3' or zpm01.status_pengadaan is null) then 'outstanding_pr' when zpm01.status_pengadaan = 'inquiry_harga' then 'inquiry_harga' when (zpm01.status_pengadaan = 'hps_oe' or zpm01.status_pengadaan = 'bidder_list' or zpm01.status_pengadaan = 'penilaian_kualifikasi' or zpm01.status_pengadaan = 'rfq') then 'hps_oe' when (zpm01.status_pengadaan = 'pemasukan_penawaran' or zpm01.status_pengadaan = 'pembukaan_penawaran' or zpm01.status_pengadaan = 'evaluasi_penawaran' or zpm01.status_pengadaan = 'klarifikasi_spesifikasi' or zpm01.status_pengadaan = 'evaluasi_teknis' or zpm01.status_pengadaan = 'evaluasi_tkdn' or zpm01.status_pengadaan = 'negosiasi'  or zpm01.status_pengadaan = 'lhp') then 'proses_tender' when (zpm01.status_pengadaan = 'pengumuman_pemenang' or zpm01.status_pengadaan = 'penunjuk_pemenang' or zpm01.status_pengadaan = 'purchase_order') then 'Penetapan Pemenang' else 'outstanding_pr' end) when (zpm01.pr is not null or zpm01.pr != '') and (purch_order.po is not null or purch_order.po != '') then (case when purch_order.dci is null or purch_order.dci = '' then 'tunggu_onsite' else 'onsite' end) end) end) as status_ from zpm01 left join work_order on work_order.[order] = zpm01.no_order left join purch_order on purch_order.material = zpm01.material AND purch_order.pr = zpm01.pr AND purch_order.item_pr = zpm01.itm_pr where work_order.[order] in ('8202205958') and ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != 0)) group by work_order.[order],zpm01.itm,work_order.main_work_ctr,work_order.[description],zpm01.material_description,zpm01.del,zpm01.unloading_point, zpm01.bun,zpm01.reqmt_date,zpm01.reqmt_qty,zpm01.qty_res,  zpm01.status_pengadaan,purch_order.po, zpm01.pr,zpm01.itm_pr, zpm01.qty_pr ,purch_order.po_quantity,purch_order.item_po ,purch_order.deliv_date ,purch_order.dci ,zpm01.dt_purch,zpm01.dt_iv,zpm01.dt_ta,zpm01.dt_status_pengadaan");
                    query.AsEnumerable();

                    // Sorting
                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                    {
                        query = query.OrderBy(sortColumn + " " + sortColumnDirection);
                    }

                    //search
                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        query = query.Where(m => m.order.StartsWith(searchValue) || m.itm.StartsWith(searchValue) || m.material.StartsWith(searchValue) || m.material_description.StartsWith(searchValue) || m.lld.StartsWith(searchValue) || m.status_.StartsWith(searchValue));
                    }
                    // Total number of rows count
                    //Console.WriteLine(customerData);
                    recordsTotal = query.Count();
                    // Paging
                    var data = await query.ToListAsync();
                    // Returning Json Data
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
                }

                return Json(new { draw = 0, recordsFiltered = 0, recordsTotal = 0, data = new { } });
                
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult DetailJasa()
        {
            try
            {
                var order_ = Request.Form["order_"].FirstOrDefault();
                var project_id = Request.Form["project_id"].FirstOrDefault();
                var currStat = Request.Form["currStat"].FirstOrDefault();

                var query = _context.contractTracking.Where(p => p.deleted == 0);

                if (!string.IsNullOrEmpty(order_))
                {
                    var od = long.Parse(order_);
                    query = query.Where(p => p.idPaket == od);
                }

                if(!string.IsNullOrEmpty(project_id))
                {
                    var pi = long.Parse(project_id);
                    query = query.Where(p => p.projectID == pi);
                }

                if(!string.IsNullOrEmpty(currStat))
                {
                    query = query.Where(p => p.currStat == currStat);
                }

                var data = query.ToList();
                var table = "";
                if (data != null)
                {
                    foreach (var d in data) {
                        var isi = "";
                        if (d.aktualSP != null) {
                            isi = "<span class='text-primary'><i class='fa fa-circle fs-20px fa-fw '></i></span>";
                        } else
                        {
                            if (d.t_light > 30) {
                                isi = "<span class='text-green-600 text-center'><i class='fa fa-circle fs-20px fa-fw'></i></span>";
                            } else if (d.t_light <= 30 && d.t_light > 20) {
                                isi = "<span class='text-warning text-center'><i class='fa fa-circle fs-20px fa-fw '></i></span>";
                            } else if (d.t_light <= 20) {
                                isi = "<span class='text-danger text-center'><i class='fa fa-circle fs-20px fa-fw'></i></span>";
                            } else
                            {
                                isi = "-";
                            }
                        }
                        var file = "";
                        if (d.file_sp != null) {
                        file = "<a href='" + d.file_sp + "' class='btn btn-info btn-sm' target='_blank'>File</a>";
                            } else
                            {
                        file = "";
                            }

                        table += "<tr><td rowspan = '2'>" + d.WO + "<br>" + d.po + "<br>" + d.PR + "<br>" + d.noSP + "<br>" + file + "</td>" +
                            "<td rowspan = '2'>" + d.judulPekerjaan + "</td>"+
                            "<td rowspan = '2' style = 'text-align: center;'>" + isi + "<br>" + d.pic + "</td>" +
                            "<td> P </td>" +
                            "<td style = 'text-align: center;  --bs-table-accent-bg: #ffcc80;'>" + d.target_kak + "</td>" +
                            "<td style = 'text-align: center;  --bs-table-accent-bg: #ffcc80;'>" + d.target_oe + "</td>" +
                            "<td style = 'text-align: center; color:blue;  --bs-table-accent-bg: #ffcc80;'>" + d.targetCO + "</td>" +
                            "<td style = 'text-align: center;  --bs-table-accent-bg: #ffcc80;'>" + d.target_pengumuman + "</td>" +
                            "<td style = 'text-align: center;  --bs-table-accent-bg: #ffcc80;'>" + d.target_sertifikasi + "</td> " +
                            "<td style = 'text-align: center;  --bs-table-accent-bg: #ffcc80;'>" + d.target_prakualifikasi + "</td>" +
                            "<td style = 'text-align: center;  --bs-table-accent-bg: #ffcc80;'>" + d.target_undangan + "</td>" +
                            "<td style = 'text-align: center;  --bs-table-accent-bg: #ffcc80;'>" + d.target_pemberian + "</td>" +
                            "<td style = 'text-align: center;   --bs-table-accent-bg: #ffcc80;'>" + d.targetBukaPH + "</td>" +
                            "<td style = 'text-align: center;  --bs-table-accent-bg: #ffcc80;'>" + d.target_pembukaan + "</td>" +
                            "<td style = 'text-align: center;  --bs-table-accent-bg: #ffcc80;'>" + d.target_evaluasi + "</td>" +
                            "<td style = 'text-align: center;  --bs-table-accent-bg: #ffcc80;'>" + d.target_negosiasi + "</td>" +
                            "<td style = 'text-align: center;  --bs-table-accent-bg: #ffcc80;'>" + d.target_usulan_pemenang + "</td>" +
                            "<td style = 'text-align: center;  --bs-table-accent-bg: #ffcc80;'>" + d.target_keputusan_pemenang + "</td>" +
                            "<td style = 'text-align: center;  --bs-table-accent-bg: #ffcc80;'>" + d.target_pengumuman_pemenang + "</td>" +
                            "<td style = 'text-align: center;  --bs-table-accent-bg: #ffcc80;'>" + d.target_pengajuan_sanggah + "</td>" +
                            "<td style = 'text-align: center;  --bs-table-accent-bg: #ffcc80;'>" + d.target_jawaban_sanggah + "</td>" +
                            "<td style = 'text-align: center;  --bs-table-accent-bg: #ffcc80;'>" + d.target_penunjukan_pemenang + "</td>" +
                            "<td style = 'text-align: center; color:blue; --bs-table-accent-bg: #ffcc80;'>" + d.targetSP + "</td>" +
                            "<td rowspan = '2'>" + d.currStat + "</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td> A </td>" +
                            "<td style = 'text-align: center;'>" + d.akt_kak + "</td>" +
                            "<td style = 'text-align: center;'>" + d.akt_oe + "</td>" +
                            "<td style = 'text-align: center;'>" + d.aktualCO + "</td>" +
                            "<td style = 'text-align: center;'>" + d.akt_pengumuman + "</td>" +
                            "<td style = 'text-align: center;'>" + d.akt_sertifikasi + "</td>" +
                            "<td style = 'text-align: center;'>" + d.akt_prakualifikasi + "</td>" +
                            "<td style = 'text-align: center;'>" + d.akt_undangan + "</td>" +
                            "<td style = 'text-align: center;'>" + d.akt_pemberian + "</td>" +
                            "<td style = 'text-align: center;'>" + d.aktualBukaPH + "</td>" +
                            "<td style = 'text-align: center;'>" + d.akt_pembukaan + "</td>" +
                            "<td style = 'text-align: center;'>" + d.akt_evaluasi + "</td>" +
                            "<td style = 'text-align: center;'>" + d.akt_negosiasi + "</td>" +
                            "<td style = 'text-align: center;'>" + d.akt_usulan_pemenang +"</td>" +
                            "<td style = 'text-align: center;'>" + d.akt_keputusan_pemenang + "</td>" +
                            "<td style = 'text-align: center;'>" + d.akt_pengumuman_pemenang + "</td>" +
                            "<td style = 'text-align: center;'>" + d.akt_pengajuan_sanggah + "</td>" +
                            "<td style = 'text-align: center;'>" + d.akt_jawaban_sanggah + "</td>" +
                            "<td style = 'text-align: center;'>" + d.akt_penunjukan_pemenang + "</td>" +
                            "<td style = 'text-align: center;'>" + d.aktualSP + "</td>" +
                        "</tr>";
                    }
                }
                else
                {
                table += "<tr>" +
                            "<td colspan = '24' style = 'text-align:center'> Data tidak ditemukan</td>" +
                       "</tr>";
                }

                return Ok(table);

            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult DashboardJoblist()
        {
            ViewBag.role = "DASHBOARD_JOBLIST";
            if (Module.hasModule("DASHBOARD_JOBLIST", HttpContext.Session))
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
		public async Task<IActionResult> GetReadinessJoblist()
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

                var filter_status = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var filter_disiplin = Request.Form["columns[2][search][value]"].FirstOrDefault();

                var project_filter = Request.Form["project_filter"].FirstOrDefault();
                var project_rev = Request.Form["project_rev"].FirstOrDefault();
                // _context.Database.SetCommandTimeout(TimeSpan.FromMinutes(20));

                //var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);

                var customerData = _context.view_readiness_joblist.FromSql($"SELECT paket_joblist.id_paket, max(paket_joblist.tag_no) as tag_no, max(paket_joblist.created_date) as created_date, max(paket_joblist.disiplin) as disiplin, max(paket_joblist.no_memo) as no_memo, max(project.description) as description, max(project.revision) as revision, max(project.id) as projectID, concat((case when max(paket_joblist.additional) is not null then 'add - ' else '' end),max(paket_joblist.no_paket)) as no_add, (CASE WHEN  COUNT(distinct case when c.status != 'ready' then c.id else null end) > 0 THEN 'not_ready' WHEN max(c.status) is null then 'not_ready' ELSE 'ready' END) as 'status_tagno' FROM paket_joblist LEFT JOIN (SELECT b.id, b.id_paket, b.joblist_id, (CASE WHEN b.dikerjakan = 'tidak' then 'ready'  WHEN b.jasa != '0' AND b.material != '0' THEN CASE WHEN b.sts_material = 'ready' AND b.sts_kontrak = 'ready'  THEN 'ready' ELSE 'not_ready' END WHEN b.material != '0' THEN ISNULL(b.sts_material, 'not_ready') WHEN b.jasa != '0' THEN ISNULL(b.sts_kontrak, 'not_ready') END) as status FROM (SELECT a.id, a.joblist_id, a.id_paket, a.jobDesc, a.jasa, a.material, a.dikerjakan, (CASE WHEN a.dikerjakan = 'tidak' then 'not_identify' WHEN a.jasa != '0' THEN  (SELECT  (CASE  WHEN contracttracking.aktualSP is null THEN 'not_ready' ELSE 'ready'  END) as status  FROM contracttracking left join joblist_detail on joblist_detail.no_jasa = contracttracking.idPaket  where contracttracking.projectID = {project_filter} and joblist_detail.id = a.id and contracttracking.deleted != '1') ELSE 'not_identify' END) as sts_kontrak, (CASE WHEN a.dikerjakan = 'tidak' then 'not_identify' WHEN a.material != '0' THEN (CASE WHEN a.status_material = 'COMPLETED' AND (SELECT (CASE  WHEN joblist_detail.material != '0' THEN  CASE WHEN count(DISTINCT (case when procurement_.status_ != 'terpenuhi_stock' AND procurement_.status_ != 'onsite' then procurement_.[order] else NULL end)) > 0 THEN 'not_ready' ELSE 'ready' END ELSE NULL END) as sts_material FROM (SELECT work_order.[order], (case when zpm01.pr is null or zpm01.pr = '' then (case when zpm01.reqmt_qty = zpm01.qty_res then 'terpenuhi_stock' else 'create_pr' end) else (case  when (zpm01.pr is not null or zpm01.pr != '') and (p.po is null or p.po = '')then (case when (zpm01.status_pengadaan = 'tunggu_pr' or zpm01.status_pengadaan = 'evaluasi_dp3' or zpm01.status_pengadaan is null) then 'outstanding_pr' when zpm01.status_pengadaan = 'inquiry_harga' then 'inquiry_harga' when (zpm01.status_pengadaan = 'hps_oe' or zpm01.status_pengadaan = 'bidder_list' or zpm01.status_pengadaan = 'penilaian_kualifikasi' or zpm01.status_pengadaan = 'rfq') then 'hps_oe' when (zpm01.status_pengadaan = 'pemasukan_penawaran' or zpm01.status_pengadaan = 'pembukaan_penawaran' or zpm01.status_pengadaan = 'evaluasi_penawaran' or zpm01.status_pengadaan = 'klarifikasi_spesifikasi' or zpm01.status_pengadaan = 'evaluasi_teknis' or zpm01.status_pengadaan = 'evaluasi_tkdn' or zpm01.status_pengadaan = 'negosiasi'  or zpm01.status_pengadaan = 'lhp') then 'proses_tender' when (zpm01.status_pengadaan = 'pengumuman_pemenang' or zpm01.status_pengadaan = 'penunjuk_pemenang' or zpm01.status_pengadaan = 'purchase_order') then 'Penetapan Pemenang' else 'outstanding_pr' end) when (zpm01.pr is not null or zpm01.pr != '') and (p.po is not null or p.po != '') then (case when p.dci is null or p.dci = '' then 'tunggu_onsite' else 'onsite' end) end) end) as status_ FROM zpm01 left join work_order on work_order.[order] = zpm01.no_order  left join purch_order as p   on p.material = zpm01.material AND p.pr = zpm01.pr AND p.item_pr = zpm01.itm_pr  where work_order.revision = {project_rev} and ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) GROUP BY work_order.[order],zpm01.itm, zpm01.pr,zpm01.reqmt_qty, zpm01.qty_res, po, zpm01.status_pengadaan, dci) procurement_ left join joblist_detail_wo on joblist_detail_wo.[order] = procurement_.[order] left join joblist_detail on joblist_detail.id = joblist_detail_wo.jobListDetailID left join joblist on joblist.id = joblist_detail.joblist_id where joblist.projectID = {project_filter} and joblist_detail.id = a.id GROUP by joblist_detail.material  ) = 'ready' then 'ready' ELSE 'not_ready' END) ELSE 'not_identify' END) as sts_material FROM joblist_detail a LEFT JOIN paket_joblist on paket_joblist.id_paket = a.id_paket where paket_joblist.projectID = {project_filter} and a.deleted != '1') b) c on c.id_paket = paket_joblist.id_paket LEFT JOIN project on project.id = paket_joblist.projectID  where project.id = {project_filter} group by paket_joblist.id_paket having count(c.id) > 0 ");


                // Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.tag_no.StartsWith(searchValue) || m.disiplin.StartsWith(searchValue) || m.status_tagno.StartsWith(searchValue));
                }

                if (!string.IsNullOrEmpty(filter_status))
                {
                    customerData = customerData.Where(m => m.status_tagno == filter_status);
                }

                if (!string.IsNullOrEmpty(filter_disiplin))
                {
                    customerData = customerData.Where(m => m.disiplin == filter_disiplin);

                }

                // Total number of rows count
                //Console.WriteLine(customerData);
                //recordsTotal = customerData.Count();
                // Paging
                var datas = await customerData.ToListAsync();
                //var data = _memoryCache.Get("products");
                //data = await _memoryCache.Set("products", datas, expirationTime);
                // Returning Json Data
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = datas });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public async Task<IActionResult> ReadinessDetailJoblist(int projectId, string rev, int paketJoblist)
        {
            try
            {
                var query = _context.view_readiness_detail.FromSql($"SELECT a.id, a.joblist_id, joblist.eqTagNo, STUFF((SELECT distinct  t2.[order] + ',' from joblist_detail_wo t2  where a.id= t2.jobListDetailID FOR XML PATH(''), TYPE ).value('.', 'NVARCHAR(MAX)')  ,1,0,'') wo,a.no_jasa, joblist.projectID, a.jobDesc, a.jasa, a.material, (CASE WHEN a.dikerjakan = 'tidak' then 'not_identify' WHEN a.jasa != '0' THEN  (SELECT (CASE WHEN contracttracking.aktualSP is null THEN 'not_ready' ELSE 'ready' END) as status FROM Mystap.dbo.contracttracking left join Mystap.dbo.joblist_detail on joblist_detail.no_jasa = contracttracking.idPaket where contracttracking.projectID = {projectId} and joblist_detail.id = a.id and contracttracking.deleted != '1') ELSE 'not_identify' END) as sts_kontrak, (CASE WHEN a.dikerjakan = 'tidak' then 'not_identify' WHEN a.material != '0' THEN (CASE WHEN a.status_material = 'COMPLETED' AND (SELECT (CASE WHEN joblist_detail.material != '0' THEN CASE WHEN count(DISTINCT case when procurement_.status_ != 'terpenuhi_stock' AND procurement_.status_ != 'onsite' then procurement_.[order] else NULL end) > '0' THEN 'not_ready' ELSE 'ready' END ELSE NULL END) as sts_material FROM (SELECT work_order.[order], (case when zpm01.pr is null or zpm01.pr = '' then (case when zpm01.reqmt_qty = zpm01.qty_res then 'terpenuhi_stock' else 'create_pr' end) else (case when (zpm01.pr is not null or zpm01.pr != '') and (p.po is null or p.po = '')then (case when (zpm01.status_pengadaan = 'tunggu_pr' or zpm01.status_pengadaan = 'evaluasi_dp3' or zpm01.status_pengadaan is null) then 'outstanding_pr' when zpm01.status_pengadaan = 'inquiry_harga' then 'inquiry_harga' when (zpm01.status_pengadaan = 'hps_oe' or zpm01.status_pengadaan = 'bidder_list' or zpm01.status_pengadaan = 'penilaian_kualifikasi' or zpm01.status_pengadaan = 'rfq') then 'hps_oe' when (zpm01.status_pengadaan = 'pemasukan_penawaran' or zpm01.status_pengadaan = 'pembukaan_penawaran' or zpm01.status_pengadaan = 'evaluasi_penawaran' or zpm01.status_pengadaan = 'klarifikasi_spesifikasi' or zpm01.status_pengadaan = 'evaluasi_teknis' or zpm01.status_pengadaan = 'evaluasi_tkdn' or zpm01.status_pengadaan = 'negosiasi'  or zpm01.status_pengadaan = 'lhp') then 'proses_tender' when (zpm01.status_pengadaan = 'pengumuman_pemenang' or zpm01.status_pengadaan = 'penunjuk_pemenang' or zpm01.status_pengadaan = 'purchase_order') then 'Penetapan Pemenang' else 'outstanding_pr' end) when (zpm01.pr is not null or zpm01.pr != '') and (p.po is not null or p.po != '') then (case when p.dci is null or p.dci = '' then 'tunggu_onsite' else 'onsite' end) end) end) as status_ FROM Mystap.dbo.zpm01 left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order left join Mystap.dbo.purch_order as p on p.material = zpm01.material AND p.pr = zpm01.pr AND p.item_pr = zpm01.itm_pr where work_order.revision = {rev} and ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) GROUP BY [work_order].[order],zpm01.itm, zpm01.pr,zpm01.reqmt_qty, zpm01.qty_res, po, zpm01.status_pengadaan, dci) procurement_ join Mystap.dbo.joblist_detail_wo on joblist_detail_wo.[order] = procurement_.[order] join Mystap.dbo.joblist_detail on joblist_detail.id = joblist_detail_wo.jobListDetailID where joblist_detail.id = a.id GROUP by joblist_detail.material ) = 'ready' then 'ready' ELSE 'not_ready' END) ELSE 'not_identify' END) as sts_material FROM Mystap.dbo.joblist_detail a LEFT JOIN paket_joblist on paket_joblist.id_paket = a.id_paket LEFT JOIN Mystap.dbo.joblist on joblist.id = a.joblist_id where joblist.projectID = {projectId} and paket_joblist.id_paket = {paketJoblist}  and a.deleted != '1' ");
                var data = await query.ToListAsync();

                var isi = "<table class='table'><tr><th>EqTagNo</th><th>Desc</th><th>Jasa</th><th> Material </th></tr>";
                foreach (var val in data)
                {
                    var sts_kontrak = "";
                    var sts_material = "";
                    if (val.sts_kontrak == "ready")
                    {
                        sts_kontrak = "<span class='badge bg-success detail_kontrak' data-jasa='" + val.no_jasa + "'>READY</span>'";
                    }
                    else if (val.sts_kontrak == "not_ready")
                    {
                        sts_kontrak = "<span class='badge bg-danger detail_kontrak' data-jasa='" + val.no_jasa + "'>NOT READY</span>'";
                    }
                    else if (val.sts_kontrak == "not_identify")
                    {
                        sts_kontrak = "N/R";
                    }
                    else
                    {
                        sts_kontrak = "<span class='badge bg-secondary'>UNDEFINED</span>";
                    }


                    if (val.sts_material == "ready")
                    {
                        sts_material = "<span class='badge bg-success detail_material' data-id='" + val.wo + "'>READY</span>'";
                    }
                    else if (val.sts_material == "not_ready")
                    {
                        sts_material = "<span class='badge bg-danger detail_material' data-id='" + val.wo + "'>NOT READY</span>'";
                    }
                    else if (val.sts_material == "not_identify")
                    {
                        sts_material = "N/R";
                    }
                    else
                    {
                        sts_material = "<span class='badge bg-secondary'>UNDEFINED</span>";
                    }


                    isi += "<tr><td>"+val.eqTagNo+"</td><td>" + val.jobDesc + "</td><td> " + sts_kontrak + " </td><td> " + sts_material + " </td> </tr> ";
                }
                isi += "</table>";


                return base.Content(isi, "text/html");
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public async Task<IActionResult> GrafikReadinessJoblist()
        {
            try
            {
                var project_filter = Request.Form["project_filter"].FirstOrDefault();
                var project_rev = Request.Form["project_rev"].FirstOrDefault();
                var customerData = _context.view_grafik_readiness.FromSql($"SELECT (case when d.status_tagno = 'ready' then 'READY' else 'NOT READY' end) as status_tagno, count(distinct d.id_paket) as total FROM (SELECT paket_joblist.id_paket, CASE WHEN COUNT(DISTINCT (case when c.status != 'ready' then c.id else NULL end)) > 0 THEN 'not_ready' WHEN max(c.status) is null then 'not_ready' ELSE 'ready' END AS status_tagno FROM paket_joblist LEFT JOIN (SELECT  b.id, b.id_paket,  b.joblist_id, (CASE WHEN b.dikerjakan = 'tidak' then 'ready' WHEN b.jasa != 0 AND b.material != 0 THEN  CASE  WHEN b.sts_material = 'ready' AND b.sts_kontrak = 'ready'  THEN 'ready'   ELSE 'not_ready' END  WHEN b.material != 0 THEN isnull(b.sts_material, 'not_ready') WHEN b.jasa != 0 THEN isnull(b.sts_kontrak, 'not_ready') END) as status FROM (SELECT a.id, paket_joblist.id_paket ,a.joblist_id, equipments.eqTagNo, joblist.projectID, a.jobDesc, a.jasa, a.material, a.dikerjakan, (CASE WHEN a.dikerjakan = 'tidak' then 'not_execute' WHEN a.jasa != '0' THEN  (SELECT (CASE WHEN contracttracking.aktualSP is null THEN 'not_ready' ELSE 'ready' END) as status FROM Mystap.dbo.contracttracking left join Mystap.dbo.joblist_detail on joblist_detail.no_jasa = contracttracking.idPaket where contracttracking.projectID = {project_filter} and joblist_detail.id = a.id and contracttracking.deleted != '1') ELSE 'not_identify' END) as sts_kontrak, (CASE WHEN a.dikerjakan = 'tidak' then 'not_execute' WHEN a.material != '0' THEN (CASE WHEN a.status_material = 'COMPLETED' AND (SELECT (CASE WHEN joblist_detail.material != '0' THEN CASE WHEN count(DISTINCT case when procurement_.status_ != 'terpenuhi_stock' AND procurement_.status_ != 'onsite' then procurement_.[order] else NULL end) > '0' THEN 'not_ready' ELSE 'ready' END ELSE NULL END) as sts_material FROM (SELECT work_order.[order], (case when zpm01.pr is null or zpm01.pr = '' then (case when zpm01.reqmt_qty = zpm01.qty_res then 'terpenuhi_stock' else 'create_pr' end) else (case when (zpm01.pr is not null or zpm01.pr != '') and (p.po is null or p.po = '')then (case when (zpm01.status_pengadaan = 'tunggu_pr' or zpm01.status_pengadaan = 'evaluasi_dp3' or zpm01.status_pengadaan is null) then 'outstanding_pr' when zpm01.status_pengadaan = 'inquiry_harga' then 'inquiry_harga' when (zpm01.status_pengadaan = 'hps_oe' or zpm01.status_pengadaan = 'bidder_list' or zpm01.status_pengadaan = 'penilaian_kualifikasi' or zpm01.status_pengadaan = 'rfq') then 'hps_oe' when (zpm01.status_pengadaan = 'pemasukan_penawaran' or zpm01.status_pengadaan = 'pembukaan_penawaran' or zpm01.status_pengadaan = 'evaluasi_penawaran' or zpm01.status_pengadaan = 'klarifikasi_spesifikasi' or zpm01.status_pengadaan = 'evaluasi_teknis' or zpm01.status_pengadaan = 'evaluasi_tkdn' or zpm01.status_pengadaan = 'negosiasi'  or zpm01.status_pengadaan = 'lhp') then 'proses_tender' when (zpm01.status_pengadaan = 'pengumuman_pemenang' or zpm01.status_pengadaan = 'penunjuk_pemenang' or zpm01.status_pengadaan = 'purchase_order') then 'Penetapan Pemenang' else 'outstanding_pr' end) when (zpm01.pr is not null or zpm01.pr != '') and (p.po is not null or p.po != '') then (case when p.dci is null or p.dci = '' then 'tunggu_onsite' else 'onsite' end) end) end) as status_ FROM Mystap.dbo.zpm01 left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order left join Mystap.dbo.purch_order as p on p.material = zpm01.material AND p.pr = zpm01.pr AND p.item_pr = zpm01.itm_pr where work_order.revision = {project_rev} and ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) GROUP BY [work_order].[order],zpm01.itm, zpm01.pr,zpm01.reqmt_qty, zpm01.qty_res, po, zpm01.status_pengadaan, dci) procurement_ join Mystap.dbo.joblist_detail_wo on joblist_detail_wo.[order] = procurement_.[order] join Mystap.dbo.joblist_detail on joblist_detail.id = joblist_detail_wo.jobListDetailID where joblist_detail.id = a.id GROUP by joblist_detail.material ) = 'ready' then 'ready' ELSE 'not_ready' END) ELSE 'not_identify' END) as sts_material FROM Mystap.dbo.joblist_detail a LEFT JOIN paket_joblist on paket_joblist.id_paket = a.id_paket LEFT JOIN joblist on joblist.id = a.joblist_id LEFT JOIN Mystap.dbo.equipments on equipments.id = joblist.id_eqTagNo where paket_joblist.projectID = {project_filter} and a.deleted != 1) b) c on c.id_paket = paket_joblist.id_paket where paket_joblist.projectID = {project_filter} GROUP BY paket_joblist.id_paket having count(c.id) > 0) d GROUP by d.status_tagno;");
                var data = await customerData.ToListAsync();
                return Json(new { data = data });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult DashboardJobplan()
        {
            ViewBag.role = "DASHBOARD_JOBPLAN";
            if (Module.hasModule("DASHBOARD_JOBPLAN", HttpContext.Session))
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
		public async Task<IActionResult> SummaryMaterial()
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
                var lldi = Request.Form["lldi"].FirstOrDefault();

                var query = (from j in _context.joblist_Detail
                             join u in _context.users on j.pic equals u.id
                             join jl in _context.joblist on j.joblist_id equals jl.id
                             join p in _context.project on jl.projectID equals p.id
                             select new
                             {
                                 id = u.id,
                                 alias = u.alias,
                                 projectID = p.id,
                                 lldi = j.lldi,
                                 pic = j.pic,
                                 material = j.material,
                                 deleted = j.deleted,
                                 not_planned = (j.status_material == "NOT_PLANNED" ? (long?) j.id : null),
                                 not_completed = (j.status_material == "NOT_COMPLETED" ? (long?) j.id : null),
                                 completed = (j.status_material == "COMPLETED" ? (long?) j.id : null),

                             });

                query = query.Where(w => w.material == 1).Where(w => w.deleted == 0).Where(w => w.pic != null);

                if (project != "")
                {
                    query = query.Where(w => w.projectID == Convert.ToInt32(project));
                }

                if (lldi != "")
                {
                    query = query.Where(w => w.lldi == Convert.ToInt32(lldi));
                }

                // Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    query = query.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    query = query.Where(m => m.alias.StartsWith(searchValue) );
                }

                
                var data = await query.GroupBy(g => new
                            {
                                g.pic,
                                g.alias,
                                g.id
                            })
                            .Select(z => new
                            {
                                 id = z.Key.id,
                                 alias = z.Key.alias,
                                 not_planned = z.Select(p => p.not_planned).Distinct().Count(),
                                 not_completed = z.Select(p => p.not_completed).Distinct().Count(),
                                 completed = z.Select(p => p.completed).Distinct().Count(),
                                 
                            }).ToListAsync();

                recordsTotal = query.GroupBy(g => new
                {
                    g.pic,
                    g.alias,
                    g.id
                }).Count();

                //var data = _memoryCache.Get("products");
                //data = await _memoryCache.Set("products", datas, expirationTime);
                // Returning Json Data
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public async Task<IActionResult> SummaryJasa()
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

                var query = (from j in _context.joblist_Detail
                             join u in _context.users on j.pic equals u.id
                             join jl in _context.joblist on j.joblist_id equals jl.id
                             join p in _context.project on jl.projectID equals p.id
                             select new
                             {
                                 id = u.id,
                                 alias = u.alias,
                                 projectID = p.id,
                                 lldi = j.lldi,
                                 pic = j.pic,
                                 jasa = j.jasa,
                                 deleted = j.deleted,
                                 not_planned = (j.status_jasa == "NOT_PLANNED" ? (long?)j.id : null),
                                 not_completed = (j.status_jasa == "NOT_COMPLETED" ? (long?)j.id : null),
                                 completed = (j.status_jasa == "COMPLETED" ? (long?)j.id : null),

                             });

                query = query.Where(w => w.jasa == 1).Where(w => w.deleted == 0).Where(w => w.pic != null);

                if (project != "")
                {
                    query = query.Where(w => w.projectID == Convert.ToInt32(project));
                }

                // Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    query = query.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    query = query.Where(m => m.alias.StartsWith(searchValue));
                }


                var data = await query.GroupBy(g => new
                {
                    g.pic,
                    g.alias,
                    g.id
                })
                            .Select(z => new
                            {
                                id = z.Key.id,
                                alias = z.Key.alias,
                                not_planned = z.Select(p => p.not_planned).Distinct().Count(),
                                not_completed = z.Select(p => p.not_completed).Distinct().Count(),
                                completed = z.Select(p => p.completed).Distinct().Count(),
                            }).ToListAsync();

                recordsTotal = query.GroupBy(g => new
                {
                    g.pic,
                    g.alias,
                    g.id
                }).Count();

                //var data = _memoryCache.Get("products");
                //data = await _memoryCache.Set("products", datas, expirationTime);
                // Returning Json Data
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public async Task<IActionResult> GrafikJobplan()
        {
            try
            {
                var kategori = Request.Form["kategori"].FirstOrDefault();
                var project = Request.Form["project"].FirstOrDefault();
                var query = (from j in _context.joblist_Detail
                             join jo in _context.joblist on j.joblist_id equals jo.id
                             join p in _context.project on jo.projectID equals p.id
                             select new
                             {
                                id = jo.id,
                                projectID = p.id,
                                material = j.material,
                                deleted = j.deleted,
                                execution = j.execution,
                                revision = j.revision,
                                disiplin = j.disiplin,
                                lldi = (j.lldi == 0) ? "NON LLDI" : "LLDI",
                                responsibility = j.responsibility


                             });
                query = query.Where(w => w.deleted == 0).Where(w => w.projectID == Convert.ToInt32(project));

                if (kategori == "execution")
                {
                     var data = await query.GroupBy(g => new
                        {
                            g.execution
                        }).Select(m => new { 
                            unit =  m.Key.execution, 
                            total = m.Select(p => p.id).Count()
                        }).ToListAsync();
                    return Json(new { data = data });
                }

                if (kategori == "revision")
                {
                    var data = await query.GroupBy(g => new
                        {
                            g.revision
                        }).Select(m => new {
                            unit = m.Key.revision,
                            total = m.Select(p => p.id).Count()
                        }).ToListAsync();
                     return Json(new { data = data });
                }

                if (kategori == "disiplin")
                {
                    var data = await query.GroupBy(g => new
                        {
                            g.disiplin
                        }).Select(m => new {
                            unit = m.Key.disiplin,
                            total = m.Select(p => p.id).Count()
                        }).ToListAsync();
                    return Json(new { data = data });
                }

                if (kategori == "lldi")
                {
                    query = query.Where(w => w.material == 1);
                    var data = await query.GroupBy(g => new
                        {
                            g.lldi
                        }).Select(m => new {
                            unit = m.Key.lldi,
                            total = m.Select(p => p.id).Count()
                        }).ToListAsync();
                    return Json(new { data = data });
                }

                if (kategori == "responsibility")
                {
                    var data = await query.GroupBy(g => new
                        {
                            g.responsibility
                        }).Select(m => new {
                            unit = m.Key.responsibility,
                            total = m.Select(p => p.id).Count()
                        }).ToListAsync();
                    return Json(new { data = data });
                }

                return Json(new { data = new { } });

            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult DashboardKontrak()
        {
            ViewBag.role = "DASHBOARD_KONTRAK";
            if (Module.hasModule("DASHBOARD_KONTRAK", HttpContext.Session))
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
        public async Task<IActionResult> KontrakByUnit()
        {
            try
            {
                var kategori = Request.Form["kategori"].FirstOrDefault();
                var project = Request.Form["project"].FirstOrDefault();
                var query = (from j in _context.contractTracking
                             join p in _context.project on j.projectID equals p.id
                             select new
                             {
                                 idPaket = j.idPaket,
                                 projectID = p.id,
                                 deleted = j.deleted,
                                 unit = j.unit,
                                 kategoriPaket = j.kategoriPaket,
                                 kriteria = j.kriteria,
                                 tipePaket = j.tipePaket,
                                 dirPWS = j.dirPWS,
                                 sourceVendor = j.sourceVendor,
                                 asalSource = j.asalSource,
                                 katTender = j.katTender


                             });
                query = query.Where(w => w.deleted == 0).Where(w => w.projectID == Convert.ToInt32(project));

                if (kategori == "UNIT")
                {
                    var data = await query.GroupBy(g => new
                    {
                        g.unit
                    }).Select(m => new {
                        unit = m.Key.unit,
                        total = m.Select(p => p.idPaket).Count()
                    }).ToListAsync();
                    return Json(new { data = data });
                }

                if (kategori == "KATEGORI") {
                    var data = await query.GroupBy(g => new
                    {
                        g.kategoriPaket
                    }).Select(m => new {
                        unit = m.Key.kategoriPaket,
                        total = m.Select(p => p.idPaket).Count()
                    }).ToListAsync();
                    return Json(new { data = data });
                } 
                
                if (kategori == "KRITERIA") {
                    var data = await query.GroupBy(g => new
                    {
                        g.kriteria
                    }).Select(m => new {
                        unit = m.Key.kriteria,
                        total = m.Select(p => p.idPaket).Count()
                    }).ToListAsync();
                    return Json(new { data = data });
                }
                
                if (kategori == "TIPE") {
                    var data = await query.GroupBy(g => new
                    {
                        g.tipePaket
                    }).Select(m => new {
                        unit = m.Key.tipePaket,
                        total = m.Select(p => p.idPaket).Count()
                    }).ToListAsync();
                    return Json(new { data = data });
                } 
                
                if (kategori == "DIREKSI_PENGAWAS") {
                    var data = await query.GroupBy(g => new
                    {
                        g.dirPWS
                    }).Select(m => new {
                        unit = m.Key.dirPWS,
                        total = m.Select(p => p.idPaket).Count()
                    }).ToListAsync();
                    return Json(new { data = data });
                } 
                
                if (kategori == "SOURCE_VENDOR") {
                    var data = await query.GroupBy(g => new
                    {
                        g.sourceVendor
                    }).Select(m => new {
                        unit = m.Key.sourceVendor,
                        total = m.Select(p => p.idPaket).Count()
                    }).ToListAsync();
                    return Json(new { data = data });
                } 
                
                if (kategori == "ASAL_SOURCE") {
                    var data = await query.GroupBy(g => new
                    {
                        g.asalSource
                    }).Select(m => new {
                        unit = m.Key.asalSource,
                        total = m.Select(p => p.idPaket).Count()
                    }).ToListAsync();
                    return Json(new { data = data });
                } 
                
                if (kategori == "PENGADAAN") {
                    var data = await query.GroupBy(g => new
                    {
                        g.katTender
                    }).Select(m => new {
                        unit = m.Key.katTender,
                        total = m.Select(p => p.idPaket).Count()
                    }).ToListAsync();
                    return Json(new { data = data });
                }




                return Json(new { data = new { } });

            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public async Task<IActionResult> KontrakByStatus()
        {
            try
            {
                var project = Request.Form["project"].FirstOrDefault();
                var query = (from j in _context.contractTracking
                             join p in _context.project on j.projectID equals p.id
                             select new
                             {
                                 idPaket = j.idPaket,
                                 currStat = j.currStat,
                                 projectID = p.id,
                                 deleted = j.deleted,

                             });
                query = query.Where(w => w.currStat != null).Where(w => w.deleted == 0);

                if(project != "")
                {
                    query = query.Where(w => w.projectID == Convert.ToInt32(project));
                }
                var list = new[] { "PENYUSUNAN KAK", "PENYUSUNAN OE", "KIRIM PAKET KE CO", "PENGUMUMAN PENDAFTARAN", "SERTIFIKASI", "PRAKUALIFIKASI", "UNDANGAN & PENGAMBILAN DOKUMEN", "PEMBERIAN PENJELASAN", "PENYAMPAIAN DOKUMEN PENAWARAN", "PEMBUKAAN DOKUMEN PENAWARAN", "EVALUASI PENAWARAN", "NEGOSIASI", "USULAN PENETAPAN CALON PEMENANG", "KEPUTUSAN PEMENANG", "PENGUMUMAN PEMENANG", "PENGAJUAN SANGGAH", "JAWABAN SANGGAH", "PENUNJUKAN PEMENANG", "PROSES SPB" };
                var datas = await query.GroupBy(y => new
                            {
                                y.currStat
                            }).OrderBy(o => list.Contains(o.Key.currStat)).Select(z => new
                            {
                                currStat = z.Key.currStat,
                                total = z.Select(p => p.idPaket).Distinct().Count()
                            }).ToListAsync();

                var total = 0;
                var isi = "" ;

                foreach(var val in datas)
                {
                    total += val.total;

                    isi += "<a class='widget-list-item  detail_kontrak rounded-0 pt-3px' style='background-color:white;' data-status='" + val.currStat + "'>" +
                        "<div class='widget-list-media icon'>" +
                        "<i class='fa fa-info-circle text-blue fs-10px me-2'></i>" +
                        "</div>" +
                        "<div class='widget-list-content'>" +
                            "<div class='widget-list-title' style='color:black;'>" + val.currStat +"</div>" +
                        "</div>" +
                        "<div class='widget-list-action text-nowrap text-black-500'>" +
                            "<span data-animation='number' data-value='"+ val.total + "'>" + val.total + "</span>"+
                        "</div>"+
                    "</a>";
                }


                return Json(new { data = isi, total = total });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public async Task<IActionResult> ChartKontrakByStatus()
        {
            try
            {
                var project = Request.Form["project"].FirstOrDefault();
                var query = (from j in _context.contractTracking
                             join p in _context.project on j.projectID equals p.id
                             select new
                             {
                                 idPaket = j.idPaket,
                                 currStat = j.currStat,
                                 projectID = p.id,
                                 deleted = j.deleted,

                             });
                query = query.Where(w => w.currStat != null).Where(w => w.deleted == 0);

                if (project != "")
                {
                    query = query.Where(w => w.projectID == Convert.ToInt32(project));
                }
                var list = new[] { "PENYUSUNAN KAK", "PENYUSUNAN OE", "KIRIM PAKET KE CO", "PENGUMUMAN PENDAFTARAN", "SERTIFIKASI", "PRAKUALIFIKASI", "UNDANGAN & PENGAMBILAN DOKUMEN", "PEMBERIAN PENJELASAN", "PENYAMPAIAN DOKUMEN PENAWARAN", "PEMBUKAAN DOKUMEN PENAWARAN", "EVALUASI PENAWARAN", "NEGOSIASI", "USULAN PENETAPAN CALON PEMENANG", "KEPUTUSAN PEMENANG", "PENGUMUMAN PEMENANG", "PENGAJUAN SANGGAH", "JAWABAN SANGGAH", "PENUNJUKAN PEMENANG", "PROSES SPB" };
                var datas = await query.GroupBy(y => new
                {
                    y.currStat
                }).OrderBy(o => list.Contains(o.Key.currStat)).Select(z => new
                {
                    currStat = z.Key.currStat,
                    total = z.Select(p => p.idPaket).Distinct().Count()
                }).ToListAsync();

              


                return Json(new { data = datas});
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public async Task<IActionResult> CountProgressJasa()
        {
            try
            {
                var project = Request.Form["project_filter"].FirstOrDefault();
                var unit = Request.Form["unit_filter"].FirstOrDefault();
                var pic = Request.Form["pic_filter"].FirstOrDefault();

                var query = (from j in _context.contractTracking
                             join p in _context.project on j.projectID equals p.id
                             select new
                             {
                                idPaket = j.idPaket,
                                deleted = j.deleted,
                                projectID = p.id,
                                unit = j.unit,
                                pic = j.pic,
                                sp = (j.aktualSP != null) ? (long?)j.idPaket : null,
                                on_track = (j.t_light > 30 && j.aktualSP == null) ? (long?)j.idPaket : null,
                                potensi_delay = (j.t_light <= 30 && j.t_light > 20 && j.aktualSP == null) ? (long?)j.idPaket : null,
                                delay = (j.t_light <= 20 && j.aktualSP == null) ? (long?)j.idPaket : null,

                             });
                query = query.Where(w => w.deleted == 0);
                if (project != "")
                {
                    query = query.Where(w => w.projectID == Convert.ToInt32(project));
                }

                //if (unit != "")
                //{
                //    query = query.Where(w => w.unit == unit);
                //}

                //if (pic != "")
                //{
                //    query = query.Where(w => w.pic == pic);
                //}

                var data = await query.GroupBy(g => new
                            {})
                            .Select(z => new
                            {
                                sp = z.Select(p => p.sp).Distinct().Count(),
                                on_track = z.Select(p => p.on_track).Distinct().Count(),
                                potensi_delay = z.Select(p => p.potensi_delay).Distinct().Count(),
                                delay = z.Select(p => p.delay).Distinct().Count()
                            }).FirstOrDefaultAsync();

                return Json(new { data = data });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult DashboardMaterial()
        {
            ViewBag.role = "DASHBOARD_MATERIAL";
            if (Module.hasModule("DASHBOARD_MATERIAL", HttpContext.Session))
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
		public async Task<IActionResult> CountSummaryMaterial()
        {
            try
            {
                var project_id = Request.Form["project_id"].FirstOrDefault();
                var lldi = Request.Form["lldi"].FirstOrDefault();

                var w = "";
                if(project_id != "")
                {
                    w += " and zpm01.revision = '"+project_id+"' ";
                }

                if(lldi != "all")
                {
                    if(lldi == "0")
                    {
                        w += " and ( zpm01.unloading_point is null or zpm01.unloading_point != 'xx') ";
                    }
                    else
                    {
                        w += " and zpm01.unloading_point = 'xx' ";
                    }
                }

                var query = @"select " +
                    "(case" +
                    "   when b.status_ = 'terpenuhi_stock' or b.status_ = 'onsite' then 'Ready'" +
                    " else (case " +
                    " when CONVERT (DATE, GETDATE()) > b.finish_date then 'Delay'" +
                    " when b.prognosa_ < b.md then 'On Track'" +
                    " else 'Delay'" +
                    " end)" +
                    " end) as status_ready, " +
                    "count(b.[order]) as total from " +
                    "(select work_order.[order], max(project.tglSelesaiTA) as finish_date, (case when max(project.taoh) = 'OH' then DATEADD(DAY, (case when max(zpm01.prognosa_matl) is not null then max(zpm01.prognosa_matl) else 0 end),max(prognosa_oh.[start_date])) else max(project.tglTA) end) as md," +
                    "(case when purch_order.po is not null then max(purch_order.deliv_date) else DATEADD(DAY, ((case when max(zpm01.dt_purch) is not null or max(zpm01.dt_purch) != '' then max(zpm01.dt_purch) when max(zpm01.dt_iv) is not null or max(zpm01.dt_iv) != '' then max(zpm01.dt_iv) else (case when max(zpm01.dt_ta) is not null or max(zpm01.dt_ta) != '' then max(zpm01.dt_ta) else 0 end) end) + (case when max(zpm01.dt_status_pengadaan) is not null or max(zpm01.dt_status_pengadaan) != '' then max(zpm01.dt_status_pengadaan) else 52 end)), CONVERT (DATE, GETDATE())) end)as prognosa_," +
                    "(case when zpm01.pr is null or zpm01.pr = '' then (case when zpm01.reqmt_qty = zpm01.qty_res then 'terpenuhi_stock' else 'create_pr' end) else (case when (zpm01.pr is not null or zpm01.pr != '') and (purch_order.po is null or purch_order.po = '') then (case when (zpm01.status_pengadaan = 'tunggu_pr' or zpm01.status_pengadaan = 'evaluasi_dp3' or zpm01.status_pengadaan is null) then 'outstanding_pr' when zpm01.status_pengadaan = 'inquiry_harga' then 'inquiry_harga' when (zpm01.status_pengadaan = 'hps_oe' or zpm01.status_pengadaan = 'bidder_list' or zpm01.status_pengadaan = 'penilaian_kualifikasi' or zpm01.status_pengadaan = 'rfq') then 'hps_oe' when (zpm01.status_pengadaan = 'pemasukan_penawaran' or zpm01.status_pengadaan = 'pembukaan_penawaran' or zpm01.status_pengadaan = 'evaluasi_penawaran' or zpm01.status_pengadaan = 'klarifikasi_spesifikasi' or zpm01.status_pengadaan = 'evaluasi_teknis' or zpm01.status_pengadaan = 'evaluasi_tkdn' or zpm01.status_pengadaan = 'negosiasi'  or zpm01.status_pengadaan = 'lhp') then 'proses_tender' when (zpm01.status_pengadaan = 'pengumuman_pemenang' or zpm01.status_pengadaan = 'penunjuk_pemenang' or zpm01.status_pengadaan = 'purchase_order') then 'penetapan_pemenang' else 'outstanding_pr' end) when (zpm01.pr is not null or zpm01.pr != '') and (purch_order.po is not null or purch_order.po != '') then (case when purch_order.dci is null or purch_order.dci = '' then 'tunggu_onsite' else 'onsite' end) end) end) as status_ from zpm01 " +
                    "left join work_order on work_order.[order] = zpm01.no_order  " +
                    "left join Mystap.dbo.joblist_detail_wo on joblist_detail_wo.[order] = work_order.[order] left join Mystap.dbo.joblist_detail on joblist_detail.id = joblist_detail_wo.jobListDetailID left join Mystap.dbo.joblist on joblist.id = joblist_detail.joblist_id   left join Mystap.dbo.prognosa_oh on prognosa_oh.id_joblist = joblist.id  " +
                    "left join project on project.revision = work_order.revision " +
                    "left join purch_order on purch_order.material = zpm01.material AND purch_order.pr = zpm01.pr AND purch_order.item_pr = zpm01.itm_pr where ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) ";
                query += w;
                query += " group by work_order.[order], zpm01.itm, zpm01.pr,zpm01.reqmt_qty,zpm01.qty_res,zpm01.status_pengadaan,purch_order.po, purch_order.dci)b group by " +
                    "(case" +
                    " when b.status_ = 'terpenuhi_stock' or b.status_ = 'onsite' then 'Ready'" +
                    " else (case " +
                    " when CONVERT (DATE, GETDATE()) > b.finish_date then 'Delay'" +
                    " when b.prognosa_ < b.md then 'On Track' " +
                    " else 'Delay'" +
                    " end) " +
                    "end)";

                var c = FormattableStringFactory.Create(query);

                var data = await  _context.view_count_summary_material.FromSql(c).ToListAsync();
                return Json(new { datas = data});
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult DetailSummaryMaterial(string? rev, string? filter, string? lldi)
        {
            ViewData["rev"] = rev;
            ViewData["filter"] = filter;
            ViewData["lldi"] = lldi;
            return View();
        }

		[AuthorizedAction]
		public async Task<IActionResult> DetailSummaryMaterial_()
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

                var rev = Request.Form["rev"].FirstOrDefault();
                var filter = Request.Form["filter"].FirstOrDefault();
                var lldi = Request.Form["lldi"].FirstOrDefault();

                var w = "";
                var ws = "";

                if (rev != "")
                {
                    w += " and zpm01.revision = '" + rev + "' ";
                }

                if (lldi != "all")
                {
                    if (lldi == "0")
                    {
                        w += " and ( zpm01.unloading_point is null or zpm01.unloading_point != 'xx') ";
                    }
                    else
                    {
                        w += " and zpm01.unloading_point = 'xx' ";
                    }
                }

                if(filter != "")
                {
                    ws += " where (case "+
	                       "     when b.status_ = 'terpenuhi_stock' or b.status_ = 'onsite' then 'ready' "+
	                       "     else (case "+
		                   "         when CONVERT (DATE, GETDATE()) > b.finish_date then 'delay' "+
		                   "         when b.prognosa_ < b.md then 'on_track' "+
                           "         else 'delay' "+
                           "     end) "+
                           "  end) = '"+filter+"' ";
                }

                var query = @"select " +
                           " b.*, " +
                           " (case " +
                           "     when b.status_ = 'terpenuhi_stock' or b.status_ = 'onsite' then 'ready' " +
                           "     else (case  " +
                           "         when CONVERT (DATE, GETDATE()) > b.finish_date then 'delay' " +
                           "         when b.prognosa_ < b.md then 'on_track' " +
                           "         else 'delay' " +
                           "     end) " +
                           "  end) as status_ready from (select work_order.[order],max(work_order.[description]) as wo_description,max(zpm01.material) as material,max(zpm01.material_description) as [description],zpm01.itm,max(zpm01.bun)as bun,max(zpm01.reqmt_qty) as reqmt_qty, " +
                           "  max(zpm01.pr) as pr,max(zpm01.itm_pr) as pr_item,max(zpm01.qty_pr) as pr_qty,max(zpm01.qty_res) as qty_res,max(purch_order.po) as po,max(purch_order.qty_delivered) as po_qty,max(purch_order.item_po) as po_item,max(purch_order.dci) as dci, " +
                           " max(project.tglSelesaiTA) as finish_date, (case when max(project.taoh) = 'OH' then DATEADD(DAY, (case when max(zpm01.prognosa_matl) is not null then max(zpm01.prognosa_matl) else 0 end),max(prognosa_oh.[start_date])) else max(project.tglTA) end) as md, " +
                           " (case when max(purch_order.po) is not null then DATEDIFF(day, max(purch_order.doc_date),max(purch_order.deliv_date))  else (case  when max(zpm01.dt_purch) is not null or max(zpm01.dt_purch) != '' then max(zpm01.dt_purch)  when max(zpm01.dt_iv) is not null or max(zpm01.dt_iv) != '' then max(zpm01.dt_iv) else ( case when max(zpm01.dt_ta) is not null or max(zpm01.dt_ta) != '' then max(zpm01.dt_ta) else 0 end)  end)  end) as dt_ , "+
                           " (case                    when max(zpm01.unloading_point) = 'XX' then 'LLD'                   else 'Non LLD'                end) as lld, "+
                           " (case when max(purch_order.po) is not null then max(purch_order.deliv_date) else DATEADD(DAY, ((case when max(zpm01.dt_purch) is not null or max(zpm01.dt_purch) != '' then max(zpm01.dt_purch) when max(zpm01.dt_iv) is not null or max(zpm01.dt_iv) != '' then max(zpm01.dt_iv) else (case when max(zpm01.dt_ta) is not null or max(zpm01.dt_ta) != '' then max(zpm01.dt_ta) else 0 end) end) + (case when max(zpm01.dt_status_pengadaan) is not null or max(zpm01.dt_status_pengadaan) != '' then max(zpm01.dt_status_pengadaan) else 52 end)), CONVERT (DATE, GETDATE())) end) as prognosa_, " +
                           " (case when max(zpm01.pr) is null or max(zpm01.pr) = '' then (case when max(zpm01.reqmt_qty) = max(zpm01.qty_res) then 'terpenuhi_stock' else 'create_pr' end) else (case when (max(zpm01.pr) is not null or max(zpm01.pr) != '') and (max(purch_order.po) is not null or max(purch_order.po) != '') then (case when max(purch_order.dci) is null or max(purch_order.dci) = '' then 'tunggu_onsite' else 'onsite'  end) when (max(zpm01.status_pengadaan) is not null or max(zpm01.status_pengadaan) != '') then max(zpm01.status_pengadaan) else 'outstanding_pr' end) end) as status_ from Mystap.dbo.zpm01 " +
                           " left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order " +
                           " left join Mystap.dbo.joblist_detail_wo on joblist_detail_wo.[order] = work_order.[order] left join Mystap.dbo.joblist_detail on joblist_detail.id = joblist_detail_wo.jobListDetailID left join Mystap.dbo.joblist on joblist.id = joblist_detail.joblist_id   left join Mystap.dbo.prognosa_oh on prognosa_oh.id_joblist = joblist.id  " +
                           " left join Mystap.dbo.project on project.revision = work_order.revision " +
                           " left join Mystap.dbo.purch_order on purch_order.material = zpm01.material AND purch_order.pr = zpm01.pr AND purch_order.item_pr = zpm01.itm_pr where ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) " +
                           w +
                           " group by work_order.[order], zpm01.itm) as b " + ws;

                var c = FormattableStringFactory.Create(query);
                var datas = _context.view_detail_summary_material.FromSql(c);

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    datas = datas.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    datas = datas.Where(m => m.description.StartsWith(searchValue));
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

		[AuthorizedAction]
		public async Task<IActionResult> GrafikMaterial()
        {
            try
            {
                var rev = Request.Form["project_id"].FirstOrDefault();
                var lldi = Request.Form["lldi"].FirstOrDefault();
                var w = "";
                if (rev != "")
                {
                    w += " and zpm01.revision = '" + rev + "' ";
                }

                if (lldi != "all")
                {
                    if (lldi == "0")
                    {
                        w += " and ( zpm01.unloading_point is null or zpm01.unloading_point != 'xx') ";
                    }
                    else
                    {
                        w += " and zpm01.unloading_point = 'xx' ";
                    }
                }
                var query = @"select b.status_, count(b.[order]) as total from (select work_order.[order],max(zpm01.revision) as revision,(case when max(zpm01.pr) is null or max(zpm01.pr) = '' then (case when max(zpm01.reqmt_qty) = max(zpm01.qty_res) then 'terpenuhi_stock' else 'create_pr' end) else (case when (max(zpm01.pr) is not null or max(zpm01.pr) != '') and (max(purch_order.po) is null or max(purch_order.po) = '') then (case when (max(zpm01.status_pengadaan) = 'tunggu_pr' or max(zpm01.status_pengadaan) = 'evaluasi_dp3' or max(zpm01.status_pengadaan) is null) then 'outstanding_pr' when max(zpm01.status_pengadaan) = 'inquiry_harga' then 'inquiry_harga' when (max(zpm01.status_pengadaan) = 'hps_oe' or max(zpm01.status_pengadaan) = 'bidder_list' or max(zpm01.status_pengadaan) = 'penilaian_kualifikasi' or max(zpm01.status_pengadaan) = 'rfq') then 'hps_oe' when (max(zpm01.status_pengadaan) = 'pemasukan_penawaran' or max(zpm01.status_pengadaan) = 'pembukaan_penawaran' or max(zpm01.status_pengadaan) = 'evaluasi_penawaran' or max(zpm01.status_pengadaan) = 'klarifikasi_spesifikasi' or max(zpm01.status_pengadaan) = 'evaluasi_teknis' or max(zpm01.status_pengadaan) = 'evaluasi_tkdn' or max(zpm01.status_pengadaan) = 'negosiasi'  or max(zpm01.status_pengadaan) = 'lhp') then 'proses_tender' when (max(zpm01.status_pengadaan) = 'pengumuman_pemenang' or max(zpm01.status_pengadaan) = 'penunjuk_pemenang' or max(zpm01.status_pengadaan) = 'purchase_order') then 'penetapan_pemenang' else 'outstanding_pr' end) when (max(zpm01.pr) is not null or max(zpm01.pr) != '') and (max(purch_order.po) is not null or max(purch_order.po) != '') then (case when max(purch_order.dci) is null or max(purch_order.dci) = '' then 'tunggu_onsite' else 'onsite' end) end) end) as status_ from Mystap.dbo.zpm01 
                            left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order 
                            left join Mystap.dbo.prognosa_oh on prognosa_oh.eqTagno = work_order.equipment
                            left join Mystap.dbo.project on project.revision = work_order.revision
                            left join Mystap.dbo.purch_order on purch_order.material = zpm01.material AND purch_order.pr = zpm01.pr AND purch_order.item_pr = zpm01.itm_pr 
                            where ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0'))" + w+
                            "group by work_order.[order], zpm01.itm) as b group by b.status_";
                var c = FormattableStringFactory.Create(query);
                var data = await _context.view_grafik_material.FromSql(c).ToListAsync();
                return Json(new { datas = data });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public async Task<IActionResult> GrafikProgresMaterial()
        {
            try
            {
                var rev = Request.Form["revision"].FirstOrDefault();

                var data = await _context.historyReservasi.Where(p => p.revision == rev).Select(p => new { date = p.created_date.ToString("yyyy-MM-dd"), total_item = p.total_item, stock = p.stock, create_pr = p.create_pr, outstanding_pr = p.outstanding_pr, inquiry_harga = p.inquiry_harga, hps_oe = p.hps_oe, proses_tender = p.proses_tender, penetapan_pemenang = p.penetapan_pemenang, tunggu_onsite = p.tunggu_onsite, onsite = p.onsite }).ToListAsync();
                return Json(new { datas = data });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public async Task<IActionResult> SummaryMaterial_()
        {
            try
            {
                var project = Request.Form["project_id"].FirstOrDefault();
                var lldi = Request.Form["lldi"].FirstOrDefault();
                var project_name = Request.Form["project_name"].FirstOrDefault();
                var count =  _context.zpm01.Where(p => p.revision == project).Where(p => ((p.del != null || p.del == "X") || (p.reqmt_qty == null || p.reqmt_qty == "0")))
                .Select(p => new
                {
                    tot_del = p.id,
                }).Count();
                var w = "";
                if (project != "")
                {
                    w += " and zpm01.revision = '" + project + "' ";
                }

                if (lldi != "all")
                {
                    if (lldi == "0")
                    {
                        w += " and (zpm01.unloading_point is null or zpm01.unloading_point != 'xx') ";
                    }
                    else
                    {
                        w += " and zpm01.unloading_point = 'xx' ";
                    }
                }
                var query = "select b.revision," +
                    " count(b.[order]) as total, " +
                    " count( case when b.status_ = 'terpenuhi_stock' then b.[order] else null end) as stock," +
                    " count( case when b.status_ = 'create_pr' then b.[order] else null end) as belum_pr," +
                    " count( case when b.status_ = 'outstanding_pr' or b.status_ = 'inquiry_harga' or b.status_ = 'hps_oe' or b.status_ = 'proses_tender' or b.status_ = 'penetapan_pemenang' then b.[order] else null end) as sudah_pr,\r\ncount( case when b.status_ = 'outstanding_pr' then b.[order] else null end) as outstanding_pr, " +
                    " count( case when b.status_ = 'inquiry_harga' then b.[order] else null end) as inquiry_harga, " +
                    " count( case when b.status_ = 'hps_oe' then b.[order] else null end) as hps_oe, " +
                    " count( case when b.status_ = 'proses_tender' then b.[order] else null end) as proses_tender, " +
                    " count( case when b.status_ = 'penetapan_pemenang' then b.[order] else null end) as penetapan_pemenang, " +
                    " count( case when b.status_ = 'tunggu_onsite' or b.status_ = 'onsite' then b.[order] else null end) as sudah_po, " +
                    " count( case when b.status_ = 'tunggu_onsite' then b.[order] else null end) as belum_tiba, " +
                    " count( case when b.status_ = 'onsite' then b.[order] else null end) as sudah_tiba from (select work_order.[order],max(zpm01.revision) as revision,(case when max(zpm01.pr) is null or max(zpm01.pr) = '' then (case when max(zpm01.reqmt_qty) = max(zpm01.qty_res) then 'terpenuhi_stock' else 'create_pr' end) else (case when (max(zpm01.pr) is not null or max(zpm01.pr) != '') and (max(purch_order.po) is null or max(purch_order.po) = '') then (case when (max(zpm01.status_pengadaan) = 'tunggu_pr' or max(zpm01.status_pengadaan) = 'evaluasi_dp3' or max(zpm01.status_pengadaan) is null) then 'outstanding_pr' when max(zpm01.status_pengadaan) = 'inquiry_harga' then 'inquiry_harga' when (max(zpm01.status_pengadaan) = 'hps_oe' or max(zpm01.status_pengadaan) = 'bidder_list' or max(zpm01.status_pengadaan) = 'penilaian_kualifikasi' or max(zpm01.status_pengadaan) = 'rfq') then 'hps_oe' when (max(zpm01.status_pengadaan) = 'pemasukan_penawaran' or max(zpm01.status_pengadaan) = 'pembukaan_penawaran' or max(zpm01.status_pengadaan) = 'evaluasi_penawaran' or max(zpm01.status_pengadaan) = 'klarifikasi_spesifikasi' or max(zpm01.status_pengadaan) = 'evaluasi_teknis' or max(zpm01.status_pengadaan) = 'evaluasi_tkdn' or max(zpm01.status_pengadaan) = 'negosiasi'  or max(zpm01.status_pengadaan) = 'lhp') then 'proses_tender' when (max(zpm01.status_pengadaan) = 'pengumuman_pemenang' or max(zpm01.status_pengadaan) = 'penunjuk_pemenang' or max(zpm01.status_pengadaan) = 'purchase_order') then 'penetapan_pemenang' else 'outstanding_pr' end) when (max(zpm01.pr) is not null or max(zpm01.pr) != '') and (max(purch_order.po) is not null or max(purch_order.po) != '') then (case when max(purch_order.dci) is null or max(purch_order.dci) = '' then 'tunggu_onsite' else 'onsite' end) end) end) as status_ from Mystap.dbo.zpm01 " +
                    " left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order " +
                    " left join Mystap.dbo.prognosa_oh on prognosa_oh.eqTagno = work_order.equipment " +
                    " left join Mystap.dbo.project on project.revision = work_order.revision " +
                    " left join Mystap.dbo.purch_order on purch_order.material = zpm01.material AND purch_order.pr = zpm01.pr AND purch_order.item_pr = zpm01.itm_pr where ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) " +
                    w+
                    " group by work_order.[order], zpm01.itm) as b group by b.revision";
                var c = FormattableStringFactory.Create(query);
                var data = await _context.view_summary_material.FromSql(c).FirstOrDefaultAsync();


                double stock = (data != null) ? (((float) data.stock * 100.0) / (float)data.total) : 0;
                double blm_pr = (data != null) ? (((float)data.belum_pr / (float)data.total) * 100.0) : 0;
                double sudah_pr = (data != null) ? (((float)data.sudah_pr / (float)data.total) * 100.0) : 0;
                double outstanding_pr = (data != null) ? (((float)data.outstanding_pr / (float)data.sudah_pr) * (((float)data.sudah_pr / (float)data.total) * 100.0)) : 0;
                double inquiry_harga = (data != null) ? (((float)data.inquiry_harga / (float)data.sudah_pr) * (((float)data.sudah_pr / (float)data.total) * 100.0)) : 0;
                double hps_oe = (data != null) ? (((float)data.hps_oe / (float)data.sudah_pr) * (((float)data.sudah_pr / (float)data.total) * 100.0)) : 0;
                double proses_tender = (data != null) ? (((float)data.proses_tender / (float)data.sudah_pr) * (((float)data.sudah_pr / (float)data.total) * 100.0)) : 0;
                double penetapan_pemenang = (data != null) ? (((float)data.penetapan_pemenang / (float)data.sudah_pr) * (((float)data.sudah_pr / (float)data.total) * 100.0)) : 0;
                double sudah_po = (data != null) ? (((float)data.sudah_po / (float)data.total) * 100.0) : 0;
                double blm_tiba = (data != null) ? (((float)data.belum_tiba / (float)data.sudah_po) * (((float)data.sudah_po / (float)data.total) * 100.0)) : 0;
                double sudah_tiba = (data != null) ? (((float)data.sudah_tiba / (float)data.sudah_po) * (((float)data.sudah_po / (float)data.total) * 100.0)) : 0;

                var total = (data != null) ? data.total : 0;
                var rev = (data != null) ? data.revision : "undefined";
                var stok = (data != null) ? data.stock : 0;
                var belum_pr = (data != null) ? data.belum_pr : 0;
                var sdh_pr = (data != null) ? data.sudah_pr : 0;
                var outs_pr = (data != null) ? data.outstanding_pr : 0;
                var inq_harga = (data != null) ? data.inquiry_harga : 0;
                var hpsoe = (data != null) ? data.hps_oe : 0;
                var pros_tender = (data != null) ? data.proses_tender : 0;
                var pemenang = (data != null) ? data.penetapan_pemenang : 0;
                var sdh_po = (data != null) ? data.sudah_po : 0;
                var belum_tiba = (data != null) ? data.belum_tiba : 0;
                var sdh_tiba = (data != null) ? data.sudah_tiba : 0;


                var isi = "<tr>" +
                    "<td><b>1.</b></td>" +
                    "<td><b>Total Item ( '" + project_name + "' )</b></td>" +
                    "<td><a href='/detail_summary_material_pengadaan/" + rev + "/total/" + lldi + "' target='_blank' style='color:black;'>" + total + "</a></td>" +
                    "<td></td>" +
                "</tr>" +
                "<tr>" +
                    "<td><b> 2.</b></td>" +
                    "<td><b> Dipenuhi dari Stock</b></td> " +
                    "<td><a href = '/detail_summary_material_pengadaan/" + rev + "/terpenuhi_stock/" + lldi + "'  target='_blank' style='color:blue;' > " + stok + "</a></td>" +
                    "<td>" + stock.ToString("0.00") + " %</td>" +
                "</tr> " +
                "<tr>" +
                    "<td><b>3.</b></td>" +
                    "<td><b>Reservasi Belum PR</b></td>" +
                    "<td><a href='/detail_summary_material_pengadaan/" + rev + "/create_pr/" + lldi + "' target='_blank' style='color:red;'>" + belum_pr + "</a></td>" +
                    "<td>" + blm_pr.ToString("0.00") + " %</td>" +
                "</tr>" +
                "<tr>" +
                    "<td><b>4.</b></td>" +
                    "<td><b>Reservasi Sudah PR Belum PO</b></td>" +
                    "<td><a href='/detail_summary_material_pengadaan/" + rev + "/sudah_pr_belum_po/" + lldi + "' target='_blank' style='color:black;'>" + sdh_pr + "</a></td>" +
                    "<td>" + sudah_pr.ToString("0.00") + " %</td>" +
                "</tr>" +
                "<tr>" +
                    "<td></td>" +
                    "<td>a. Outstanding PR</td>" +
                    "<td><a href='/detail_summary_material_pengadaan/" + rev + "/outstanding_pr/" + lldi + "' target='_blank' style='color:darkorange;'>" + outs_pr + "</a></td>" +
                    "<td>" + outstanding_pr.ToString("0.00") + " %</td>" +
                "</tr>" +
                "<tr>" +
                    "<td></td>" +
                    "<td>b. Inquiry Harga</td>" +
                    "<td><a href='/detail_summary_material_pengadaan/" + rev + "/inquiry_harga/" + lldi + "' target='_blank' style='color:orange;'>" + inq_harga + "</a></td>" +
                    "<td>" + inquiry_harga.ToString("0.00") + " %</td>" +
                "</tr>" +
                "<tr>" +
                    "<td></td>" +
                    "<td>c. Penyusunan HPS/OE</td>" +
                    "<td><a href='/detail_summary_material_pengadaan/" + rev + "/hps_oe/" + lldi + "' target='_blank' style='color:slategrey;'>" + hpsoe + "</a></td>" +
                    "<td>" + hps_oe.ToString("0.00") + " %</td>" +
                "</tr>" +
                "<tr>" +
                    "<td></td>" +
                    "<td>d. Proses Tender</td>" +
                    "<td><a href='/detail_summary_material_pengadaan/" + rev + "/proses_tender/" + lldi + "' target='_blank' style='color:darkseagreen;'>" + pros_tender + "</a></td>" +
                    "<td>" + proses_tender.ToString("0.00") + " %</td>" +
                "</tr>" +
                "<tr>" +
                    "<td></td>" +
                    "<td>e. Penetapan Pemenang</td>" +
                    "<td><a href='/detail_summary_material_pengadaan/" + rev + "/penetapan_pemenang/" + lldi + "' target='_blank' style='color:green;'>" + pemenang + "</a></td>" +
                    "<td>" + penetapan_pemenang.ToString("0.00") + " %</td>" +
                "</tr>" +
                "<tr>" +
                    "<td><b>5.</b></td> " +
                    "<td><b>Reservasi Sudah PO</b></td>" +
                    "<td><a href='/detail_summary_material_pengadaan/" + rev + "/sudah_po/" + lldi + "' target='_blank' style='color:black;'>" + sdh_po + "</a></td>" +
                    "<td>" + sudah_po.ToString("0.00") + " %</td>" +
                "</tr>" +
                "<tr>" +
                    "<td ></td>" +
                    "<td>a. Tunggu On Site</td>" +
                    "<td><a href='/detail_summary_material_pengadaan/" + rev + "/tunggu_onsite/" + lldi + "' target='_blank' style='color:deepskyblue;'>" + belum_tiba + "</a></td>" +
                    "<td>" + blm_tiba.ToString("0.00") + " %</td>" +
                "</tr>" +
                "<tr>" +
                    "<td></td>" +
                    "<td>b. Sudah On Site</td>" +
                    "<td><a href='/detail_summary_material_pengadaan/" + rev + "/onsite/" + lldi + "' target='_blank' style='color:blue;'>" + sdh_tiba + "</a></td> " +
                    "<td>" + sudah_tiba.ToString("0.00") + " %</td>" +
                "</tr>";

                return Json(new { datas = isi, count_del = count });

            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult DetailMaterial(string? rev, string? filter, string? lldi)
        {
            ViewData["rev"] = rev;
            ViewData["filter"] = filter;
            ViewData["lldi"] = lldi;
            return View();
        }

		[AuthorizedAction]
		public async Task<IActionResult> DetailMaterialPengadaan()
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

                var rev = Request.Form["rev"].FirstOrDefault();
                var filter = Request.Form["filter"].FirstOrDefault();
                var lldi = Request.Form["lldi"].FirstOrDefault();
                var w = "";
                var n = "";

                if (filter != "deleted")
                {
                    w += " and (( zpm01.del is null or zpm01.del != 'X') and ( zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) ";
                }
                else
                {
                    w += " and (( zpm01.del is not null or zpm01.del = 'X') or ( zpm01.reqmt_qty is null or zpm01.reqmt_qty = '0')) ";
                }

                if (lldi != "all")
                {
                    if (lldi == "0")
                    {
                        w += " and ( zpm01.unloading_point is null or  zpm01.unloading_point != 'xx') ";
                    }
                    else
                    {
                        w += " and zpm01.unloading_point = 'xx' ";
                    }
                }

                if( filter != "total" && filter != "deleted")
                {
                    if (filter == "sudah_pr_belum_po")
                    {
                        n =  "('outstanding_pr', 'inquiry_harga', 'hps_oe', 'proses_tender', 'penetapan_pemenang')";
                    }
                    else if (filter == "sudah_po")
                    {
                        n = "('tunggu_onsite','onsite')";
                    }
                    else
                    {
                        n = "('"+filter+"')";
                    }


                    w += " and (case when zpm01.pr is null or zpm01.pr = '' then (case when zpm01.reqmt_qty = zpm01.qty_res then 'terpenuhi_stock' else 'create_pr' end) else (case when (zpm01.pr is not null or zpm01.pr != '') and (purch_order.po is null or purch_order.po = '') then (case when (zpm01.status_pengadaan = 'tunggu_pr' or zpm01.status_pengadaan = 'evaluasi_dp3' or zpm01.status_pengadaan is null) then 'outstanding_pr' when zpm01.status_pengadaan = 'inquiry_harga' then 'inquiry_harga' when (zpm01.status_pengadaan = 'hps_oe' or zpm01.status_pengadaan = 'bidder_list' or zpm01.status_pengadaan = 'penilaian_kualifikasi' or zpm01.status_pengadaan = 'rfq') then 'hps_oe' when (zpm01.status_pengadaan = 'pemasukan_penawaran' or zpm01.status_pengadaan = 'pembukaan_penawaran' or zpm01.status_pengadaan = 'evaluasi_penawaran' or zpm01.status_pengadaan = 'klarifikasi_spesifikasi' or zpm01.status_pengadaan = 'evaluasi_teknis' or zpm01.status_pengadaan = 'evaluasi_tkdn' or zpm01.status_pengadaan = 'negosiasi'  or zpm01.status_pengadaan = 'lhp') then 'proses_tender' when (zpm01.status_pengadaan = 'pengumuman_pemenang' or zpm01.status_pengadaan = 'penunjuk_pemenang' or zpm01.status_pengadaan = 'purchase_order') then 'penetapan_pemenang' else 'outstanding_pr' end) when (zpm01.pr is not null or zpm01.pr != '') and (purch_order.po is not null or purch_order.po != '') then (case when purch_order.dci is null or purch_order.dci = '' then 'tunggu_onsite' else 'onsite' end) end) end) in " + n + " ";

                }

                var query = @"select " +
                    " max(work_order.main_work_ctr) as main_work_ctr,work_order.[order],max(work_order.description) as description,max(zpm01.material) as material,max(zpm01.material_description) as material_description,zpm01.itm,max(zpm01.bun) as bun,max(zpm01.reqmt_qty) as reqmt_qty, max(zpm01.pr) as pr,max(zpm01.itm_pr) as pr_item,max(zpm01.qty_pr) as pr_qty,max(zpm01.qty_res) as qty_res,max(purch_order.po) as po,max(purch_order.po_quantity) as po_qty,max(purch_order.item_po) as po_item,max(purch_order.dci) as dci,max(zpm01.del) as del, " +
                    " (case when max(purch_order.po) is not null then DATEDIFF(day, max(purch_order.doc_date),max(purch_order.deliv_date))  else (case  when max(zpm01.dt_purch) is not null or max(zpm01.dt_purch) != '' then max(zpm01.dt_purch)  when max(zpm01.dt_iv) is not null or max(zpm01.dt_iv) != '' then max(zpm01.dt_iv) else ( case when max(zpm01.dt_ta) is not null or max(zpm01.dt_ta) != '' then max(zpm01.dt_ta) else 0 end)  end)  end) as dt_ , " +
                    " (case                    when max(zpm01.unloading_point) = 'XX' then 'LLD'                   else 'Non LLD'                end) as lld, " +
                    " (case when max(purch_order.po) is not null then max(purch_order.deliv_date) else DATEADD(DAY, ((case when max(zpm01.dt_purch) is not null or max(zpm01.dt_purch) != '' then max(zpm01.dt_purch) when max(zpm01.dt_iv) is not null or max(zpm01.dt_iv) != '' then max(zpm01.dt_iv) else (case when max(zpm01.dt_ta) is not null or max(zpm01.dt_ta) != '' then max(zpm01.dt_ta) else 0 end) end) + (case when max(zpm01.dt_status_pengadaan) is not null or max(zpm01.dt_status_pengadaan) != '' then max(zpm01.dt_status_pengadaan) else 52 end)), CONVERT (DATE, GETDATE())) end)as prognosa_, " +
                    " (case when max(zpm01.pr) is null or max(zpm01.pr) = '' then (case when max(zpm01.reqmt_qty) = max(zpm01.qty_res) then 'terpenuhi_stock' else 'create_pr' end) else (case when (max(zpm01.pr) is not null or max(zpm01.pr) != '') and (max(purch_order.po) is not null or max(purch_order.po) != '') then (case when max(purch_order.dci) is null or max(purch_order.dci) = '' then 'tunggu_onsite' else 'onsite'  end) when (max(zpm01.status_pengadaan) is not null or max(zpm01.status_pengadaan) != '') then max(zpm01.status_pengadaan) else 'outstanding_pr' end) end) as status_ from Mystap.dbo.zpm01 " +
                    " left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order " +
                    " left join Mystap.dbo.prognosa_oh on prognosa_oh.eqTagno = work_order.equipment " +
                    " left join Mystap.dbo.project on project.revision = work_order.revision " +
                    " left join Mystap.dbo.purch_order on purch_order.material = zpm01.material AND purch_order.pr = zpm01.pr AND purch_order.item_pr = zpm01.itm_pr where  zpm01.revision = '" + rev + "' " +
                    w +
                    " group by work_order.[order], zpm01.itm ";



                var c = FormattableStringFactory.Create(query);
                var datas = _context.view_detail_material.FromSql(c);

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    datas = datas.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    datas = datas.Where(m => m.material_description.StartsWith(searchValue));
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
