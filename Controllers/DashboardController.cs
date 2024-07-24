using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using mystap.Models;
using System.Data;
using System.Linq.Dynamic.Core;

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
        public IActionResult DashboardEquipment()
        {
            ViewBag.project = _context.project.Where(p => p.deleted == 0 ).Where(p => p.active == "1").ToList();
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

                var customerData = _context.view_readiness_equipment.FromSql($"select joblist.id, joblist.projectID, project.revision, joblist.jobNo, joblist.eqTagNo, CASE WHEN count(case when  c.status = 'not_ready' then c.id else NULL end) > '0' THEN 'not_ready' WHEN c.status is null then 'not_ready' ELSE 'ready' END AS 'status_tagno' from Mystap.dbo.joblist  left join (SELECT  b.id, b.eqTagNo,  b.projectID,  b.joblist_id, (CASE WHEN b.dikerjakan = 'tidak' then 'ready' WHEN b.jasa != '0' AND b.material != '0' THEN CASE WHEN b.sts_material = 'ready' AND b.sts_kontrak = 'ready' THEN 'ready'  ELSE 'not_ready' END WHEN b.material != '0' THEN isnull(b.sts_material, 'not_ready') WHEN b.jasa != '0' THEN isnull(b.sts_kontrak, 'not_ready') END) as status FROM (SELECT a.id, a.joblist_id, joblist.eqTagNo, joblist.projectID, a.jobDesc, a.jasa, a.material, a.dikerjakan, (CASE WHEN a.dikerjakan = 'tidak' then 'not_execute' WHEN a.jasa != '0' THEN  (SELECT (CASE WHEN contracttracking.aktualSP is null THEN 'not_ready' ELSE 'ready' END) as status FROM Mystap.dbo.contracttracking left join Mystap.dbo.joblist_detail on joblist_detail.no_jasa = contracttracking.idPaket where contracttracking.projectID = { project_filter } and joblist_detail.id = a.id and contracttracking.deleted != '1') ELSE 'not_identify' END) as sts_kontrak, (CASE WHEN a.dikerjakan = 'tidak' then 'not_execute' WHEN a.material != '0' THEN (CASE WHEN a.status_material = 'COMPLETED' AND (SELECT (CASE WHEN joblist_detail.material != '0' THEN CASE WHEN count(DISTINCT case when procurement_.status_ != 'terpenuhi_stock' AND procurement_.status_ != 'onsite' then procurement_.[order] else NULL end) > '0' THEN 'not_ready' ELSE 'ready' END ELSE NULL END) as sts_material FROM (SELECT work_order.[order], (case when zpm01.pr is null or zpm01.pr = '' then (case when zpm01.reqmt_qty = zpm01.qty_res then 'terpenuhi_stock' else 'create_pr' end) else (case when (zpm01.pr is not null or zpm01.pr != '') and (p.po is null or p.po = '')then (case when (zpm01.status_pengadaan = 'tunggu_pr' or zpm01.status_pengadaan = 'evaluasi_dp3' or zpm01.status_pengadaan is null) then 'outstanding_pr' when zpm01.status_pengadaan = 'inquiry_harga' then 'inquiry_harga' when (zpm01.status_pengadaan = 'hps_oe' or zpm01.status_pengadaan = 'bidder_list' or zpm01.status_pengadaan = 'penilaian_kualifikasi' or zpm01.status_pengadaan = 'rfq') then 'hps_oe' when (zpm01.status_pengadaan = 'pemasukan_penawaran' or zpm01.status_pengadaan = 'pembukaan_penawaran' or zpm01.status_pengadaan = 'evaluasi_penawaran' or zpm01.status_pengadaan = 'klarifikasi_spesifikasi' or zpm01.status_pengadaan = 'evaluasi_teknis' or zpm01.status_pengadaan = 'evaluasi_tkdn' or zpm01.status_pengadaan = 'negisiasi'  or zpm01.status_pengadaan = 'lhp') then 'proses_tender' when (zpm01.status_pengadaan = 'pengumuman_pemenang' or zpm01.status_pengadaan = 'penunjuk_pemenang' or zpm01.status_pengadaan = 'purchase_order') then 'Penetapan Pemenang' else 'outstanding_pr' end) when (zpm01.pr is not null or zpm01.pr != '') and (p.po is not null or p.po != '') then (case when p.dci is null or p.dci = '' then 'tunggu_onsite' else 'onsite' end) end) end) as status_ FROM Mystap.dbo.zpm01 left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order left join Mystap.dbo.purch_order as p on p.material = zpm01.material AND p.pr = zpm01.pr AND p.item_pr = zpm01.itm_pr where work_order.revision = {project_rev} and ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) GROUP BY [work_order].[order],zpm01.material,zpm01.itm, zpm01.pr,zpm01.reqmt_qty, zpm01.qty_res, po, zpm01.status_pengadaan, dci) procurement_ join Mystap.dbo.joblist_detail_wo on joblist_detail_wo.[order] = procurement_.[order] join Mystap.dbo.joblist_detail on joblist_detail.id = joblist_detail_wo.jobListDetailID where joblist_detail.id = a.id GROUP by joblist_detail.material ) = 'ready' then 'ready' ELSE 'not_ready' END) ELSE 'not_identify' END) as sts_material FROM Mystap.dbo.joblist_detail a LEFT JOIN Mystap.dbo.joblist on joblist.id = a.joblist_id where joblist.projectID = {project_filter} and a.deleted != '1') b) c on c.joblist_id = joblist.id LEFT JOIN Mystap.dbo.project on project.projectNo = joblist.projectNo  where joblist.projectID = { project_filter } and joblist.deleted != '1' group by joblist.id, joblist.projectID, project.revision, joblist.jobNo, joblist.eqTagNo, c.status");

                
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

        public async Task<IActionResult> GrafikReadinessEquipment()
        {
            try
            {
                var project_filter = Request.Form["project_filter"].FirstOrDefault();
                var project_rev = Request.Form["project_rev"].FirstOrDefault();
                var customerData = _context.view_grafik_readiness.FromSql($"SELECT case when d.status_tagno = 'ready' then 'READY' else 'NOT READY' end as status_tagno,count(d.id) as total FROM ( select joblist.id, joblist.jobNo, joblist.eqTagNo, CASE WHEN count(case when  c.status = 'not_ready' then c.id else NULL end) > '0' THEN 'not_ready' WHEN c.status is null then 'not_ready' ELSE 'ready' END AS 'status_tagno' from Mystap.dbo.joblist  left join (SELECT  b.id, b.eqTagNo,  b.projectID,  b.joblist_id, (CASE WHEN b.dikerjakan = 'tidak' then 'ready' WHEN b.jasa != '0' AND b.material != '0' THEN CASE WHEN b.sts_material = 'ready' AND b.sts_kontrak = 'ready' THEN 'ready'  ELSE 'not_ready' END WHEN b.material != '0' THEN isnull(b.sts_material, 'not_ready') WHEN b.jasa != '0' THEN isnull(b.sts_kontrak, 'not_ready') END) as status FROM (SELECT a.id, a.joblist_id, joblist.eqTagNo, joblist.projectID, a.jobDesc, a.jasa, a.material, a.dikerjakan, (CASE WHEN a.dikerjakan = 'tidak' then 'not_execute' WHEN a.jasa != '0' THEN  (SELECT (CASE WHEN contracttracking.aktualSP is null THEN 'not_ready' ELSE 'ready' END) as status FROM Mystap.dbo.contracttracking left join Mystap.dbo.joblist_detail on joblist_detail.no_jasa = contracttracking.idPaket where contracttracking.projectID = {project_filter} and joblist_detail.id = a.id and contracttracking.deleted != '1') ELSE 'not_identify' END) as sts_kontrak, (CASE WHEN a.dikerjakan = 'tidak' then 'not_execute' WHEN a.material != '0' THEN (CASE WHEN a.status_material = 'COMPLETED' AND (SELECT (CASE WHEN joblist_detail.material != '0' THEN CASE WHEN count(DISTINCT case when procurement_.status_ != 'terpenuhi_stock' AND procurement_.status_ != 'onsite' then procurement_.[order] else NULL end) > '0' THEN 'not_ready' ELSE 'ready' END ELSE NULL END) as sts_material FROM (SELECT work_order.[order], (case when zpm01.pr is null or zpm01.pr = '' then (case when zpm01.reqmt_qty = zpm01.qty_res then 'terpenuhi_stock' else 'create_pr' end) else (case when (zpm01.pr is not null or zpm01.pr != '') and (p.po is null or p.po = '')then (case when (zpm01.status_pengadaan = 'tunggu_pr' or zpm01.status_pengadaan = 'evaluasi_dp3' or zpm01.status_pengadaan is null) then 'outstanding_pr' when zpm01.status_pengadaan = 'inquiry_harga' then 'inquiry_harga' when (zpm01.status_pengadaan = 'hps_oe' or zpm01.status_pengadaan = 'bidder_list' or zpm01.status_pengadaan = 'penilaian_kualifikasi' or zpm01.status_pengadaan = 'rfq') then 'hps_oe' when (zpm01.status_pengadaan = 'pemasukan_penawaran' or zpm01.status_pengadaan = 'pembukaan_penawaran' or zpm01.status_pengadaan = 'evaluasi_penawaran' or zpm01.status_pengadaan = 'klarifikasi_spesifikasi' or zpm01.status_pengadaan = 'evaluasi_teknis' or zpm01.status_pengadaan = 'evaluasi_tkdn' or zpm01.status_pengadaan = 'negisiasi'  or zpm01.status_pengadaan = 'lhp') then 'proses_tender' when (zpm01.status_pengadaan = 'pengumuman_pemenang' or zpm01.status_pengadaan = 'penunjuk_pemenang' or zpm01.status_pengadaan = 'purchase_order') then 'Penetapan Pemenang' else 'outstanding_pr' end) when (zpm01.pr is not null or zpm01.pr != '') and (p.po is not null or p.po != '') then (case when p.dci is null or p.dci = '' then 'tunggu_onsite' else 'onsite' end) end) end) as status_ FROM Mystap.dbo.zpm01 left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order left join Mystap.dbo.purch_order as p on p.material = zpm01.material AND p.pr = zpm01.pr AND p.item_pr = zpm01.itm_pr where work_order.revision = {project_rev} and ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) GROUP BY [work_order].[order],zpm01.material,zpm01.itm, zpm01.pr,zpm01.reqmt_qty, zpm01.qty_res, po, zpm01.status_pengadaan, dci) procurement_ join Mystap.dbo.joblist_detail_wo on joblist_detail_wo.[order] = procurement_.[order] join Mystap.dbo.joblist_detail on joblist_detail.id = joblist_detail_wo.jobListDetailID where joblist_detail.id = a.id GROUP by joblist_detail.material ) = 'ready' then 'ready' ELSE 'not_ready' END) ELSE 'not_identify' END) as sts_material FROM Mystap.dbo.joblist_detail a LEFT JOIN Mystap.dbo.joblist on joblist.id = a.joblist_id where joblist.projectID = {project_filter} and a.deleted != '1') b) c on c.joblist_id = joblist.id LEFT JOIN Mystap.dbo.project on project.projectNo = joblist.projectNo  where joblist.projectID = {project_filter} and joblist.deleted != '1' group by joblist.id, joblist.jobNo, joblist.eqTagNo, c.status) d GROUP by d.status_tagno;");
                var data = await customerData.ToListAsync();
                return Json(new {data = data});
            }
            catch
            {
                throw;
            }
        }

        public async Task<IActionResult> GetEquipment()
        {
            try
            {
                var project = Request.Form["project"].FirstOrDefault();
                var unit = Request.Form["unit"].FirstOrDefault();
                var query = (from j in _context.joblist
                             join p in _context.project on j.projectNo equals p.projectNo
                             join e in _context.equipments on j.eqTagNo equals e.eqTagNo
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

        public async Task<IActionResult> ReadinessDetail(int projectId, string rev, int joblist)
        {
            try
            {
                var query = _context.view_readiness_detail.FromSql($"SELECT a.id, a.joblist_id, joblist.eqTagNo, STUFF((SELECT distinct  t2.[order] + ',' from joblist_detail_wo t2  where a.id= t2.jobListDetailID FOR XML PATH(''), TYPE ).value('.', 'NVARCHAR(MAX)')  ,1,0,'') wo,a.no_jasa, joblist.projectID, a.jobDesc, a.jasa, a.material, (CASE WHEN a.dikerjakan = 'tidak' then 'not_identify' WHEN a.jasa != '0' THEN  (SELECT (CASE WHEN contracttracking.aktualSP is null THEN 'not_ready' ELSE 'ready' END) as status FROM Mystap.dbo.contracttracking left join Mystap.dbo.joblist_detail on joblist_detail.no_jasa = contracttracking.idPaket where contracttracking.projectID = {projectId} and joblist_detail.id = a.id and contracttracking.deleted != '1') ELSE 'not_identify' END) as sts_kontrak, (CASE WHEN a.dikerjakan = 'tidak' then 'not_identify' WHEN a.material != '0' THEN (CASE WHEN a.status_material = 'COMPLETED' AND (SELECT (CASE WHEN joblist_detail.material != '0' THEN CASE WHEN count(DISTINCT case when procurement_.status_ != 'terpenuhi_stock' AND procurement_.status_ != 'onsite' then procurement_.[order] else NULL end) > '0' THEN 'not_ready' ELSE 'ready' END ELSE NULL END) as sts_material FROM (SELECT work_order.[order], (case when zpm01.pr is null or zpm01.pr = '' then (case when zpm01.reqmt_qty = zpm01.qty_res then 'terpenuhi_stock' else 'create_pr' end) else (case when (zpm01.pr is not null or zpm01.pr != '') and (p.po is null or p.po = '')then (case when (zpm01.status_pengadaan = 'tunggu_pr' or zpm01.status_pengadaan = 'evaluasi_dp3' or zpm01.status_pengadaan is null) then 'outstanding_pr' when zpm01.status_pengadaan = 'inquiry_harga' then 'inquiry_harga' when (zpm01.status_pengadaan = 'hps_oe' or zpm01.status_pengadaan = 'bidder_list' or zpm01.status_pengadaan = 'penilaian_kualifikasi' or zpm01.status_pengadaan = 'rfq') then 'hps_oe' when (zpm01.status_pengadaan = 'pemasukan_penawaran' or zpm01.status_pengadaan = 'pembukaan_penawaran' or zpm01.status_pengadaan = 'evaluasi_penawaran' or zpm01.status_pengadaan = 'klarifikasi_spesifikasi' or zpm01.status_pengadaan = 'evaluasi_teknis' or zpm01.status_pengadaan = 'evaluasi_tkdn' or zpm01.status_pengadaan = 'negisiasi'  or zpm01.status_pengadaan = 'lhp') then 'proses_tender' when (zpm01.status_pengadaan = 'pengumuman_pemenang' or zpm01.status_pengadaan = 'penunjuk_pemenang' or zpm01.status_pengadaan = 'purchase_order') then 'Penetapan Pemenang' else 'outstanding_pr' end) when (zpm01.pr is not null or zpm01.pr != '') and (p.po is not null or p.po != '') then (case when p.dci is null or p.dci = '' then 'tunggu_onsite' else 'onsite' end) end) end) as status_ FROM Mystap.dbo.zpm01 left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order left join Mystap.dbo.purch_order as p on p.material = zpm01.material AND p.pr = zpm01.pr AND p.item_pr = zpm01.itm_pr where work_order.revision = {rev} and ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) GROUP BY [work_order].[order],zpm01.material,zpm01.itm, zpm01.pr,zpm01.reqmt_qty, zpm01.qty_res, po, zpm01.status_pengadaan, dci) procurement_ join Mystap.dbo.joblist_detail_wo on joblist_detail_wo.[order] = procurement_.[order] join Mystap.dbo.joblist_detail on joblist_detail.id = joblist_detail_wo.jobListDetailID where joblist_detail.id = a.id GROUP by joblist_detail.material ) = 'ready' then 'ready' ELSE 'not_ready' END) ELSE 'not_identify' END) as sts_material FROM Mystap.dbo.joblist_detail a LEFT JOIN Mystap.dbo.joblist on joblist.id = a.joblist_id where joblist.projectID = {projectId} and joblist.id = {joblist}  and a.deleted != '1'");
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

                    var query = _context.view_detail_order.FromSql($"select work_order.main_work_ctr,work_order.[order],work_order.[description],zpm01.material,zpm01.itm,zpm01.material_description,zpm01.del,zpm01.unloading_point,zpm01.bun,zpm01.reqmt_date,zpm01.reqmt_qty,zpm01.qty_res,zpm01.pr as pr,zpm01.itm_pr as pr_item,zpm01.qty_pr as pr_qty,zpm01.status_pengadaan,purch_order.po,purch_order.po_quantity as po_qty,purch_order.item_po as po_item, purch_order.deliv_date as deliv_date,purch_order.dci as dci, (case when purch_order.po is not null then purch_order.deliv_date else DATEADD(DAY, ((case when zpm01.dt_purch is not null or zpm01.dt_purch != '' then zpm01.dt_purch when zpm01.dt_iv is not null or zpm01.dt_iv != '' then zpm01.dt_iv else (case when zpm01.dt_ta is not null or zpm01.dt_ta != '' then zpm01.dt_ta else 0 end) end) + (case when zpm01.dt_status_pengadaan is not null or zpm01.dt_status_pengadaan != '' then zpm01.dt_status_pengadaan else 52 end)), CONVERT (DATE, GETDATE())) end)as prognosa_, (case when zpm01.unloading_point = 'XX' then 'LLD' else 'Non LLD' end) as lld, (case when zpm01.pr is null or zpm01.pr = '' then (case when zpm01.reqmt_qty = zpm01.qty_res then 'terpenuhi_stock' else 'create_pr' end) else (case when (zpm01.pr is not null or zpm01.pr != '') and (purch_order.po is null or purch_order.po = '') then (case when (zpm01.status_pengadaan = 'tunggu_pr' or zpm01.status_pengadaan = 'evaluasi_dp3' or zpm01.status_pengadaan is null) then 'outstanding_pr' when zpm01.status_pengadaan = 'inquiry_harga' then 'inquiry_harga' when (zpm01.status_pengadaan = 'hps_oe' or zpm01.status_pengadaan = 'bidder_list' or zpm01.status_pengadaan = 'penilaian_kualifikasi' or zpm01.status_pengadaan = 'rfq') then 'hps_oe' when (zpm01.status_pengadaan = 'pemasukan_penawaran' or zpm01.status_pengadaan = 'pembukaan_penawaran' or zpm01.status_pengadaan = 'evaluasi_penawaran' or zpm01.status_pengadaan = 'klarifikasi_spesifikasi' or zpm01.status_pengadaan = 'evaluasi_teknis' or zpm01.status_pengadaan = 'evaluasi_tkdn' or zpm01.status_pengadaan = 'negisiasi'  or zpm01.status_pengadaan = 'lhp') then 'proses_tender' when (zpm01.status_pengadaan = 'pengumuman_pemenang' or zpm01.status_pengadaan = 'penunjuk_pemenang' or zpm01.status_pengadaan = 'purchase_order') then 'Penetapan Pemenang' else 'outstanding_pr' end) when (zpm01.pr is not null or zpm01.pr != '') and (purch_order.po is not null or purch_order.po != '') then (case when purch_order.dci is null or purch_order.dci = '' then 'tunggu_onsite' else 'onsite' end) end) end) as status_ from zpm01 left join work_order on work_order.[order] = zpm01.no_order left join purch_order on purch_order.material = zpm01.material AND purch_order.pr = zpm01.pr AND purch_order.item_pr = zpm01.itm_pr where work_order.[order] in ('8202205958') and ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != 0)) group by work_order.[order],zpm01.material,zpm01.itm,work_order.main_work_ctr,work_order.[description],zpm01.material_description,zpm01.del,zpm01.unloading_point, zpm01.bun,zpm01.reqmt_date,zpm01.reqmt_qty,zpm01.qty_res,  zpm01.status_pengadaan,purch_order.po, zpm01.pr,zpm01.itm_pr, zpm01.qty_pr ,purch_order.po_quantity,purch_order.item_po ,purch_order.deliv_date ,purch_order.dci ,zpm01.dt_purch,zpm01.dt_iv,zpm01.dt_ta,zpm01.dt_status_pengadaan");
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



    }
}
