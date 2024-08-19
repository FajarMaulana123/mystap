using Azure.Core;
using MessagePack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using mystap.Models;
using System.Data;
using System.Globalization;
using System.Linq.Dynamic.Core;

namespace joblist.Controllers
{
    public class JoblistController : Controller
    {
        private readonly DatabaseContext _context;
        public JoblistController(DatabaseContext context)
        {
            _context = context;
        }
        public IActionResult Planning()
        {
            return View();
        }
        public IActionResult Joblist()
        {
			ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == "1").ToList();
            ViewBag.unit = _context.unit.Where(p => p.deleted != 1).GroupBy(p => new { p.unitCode , p.unitProses }).Select(p => new { unitCode = p.Key.unitCode , unitProses = p.Key.unitProses}).ToList();
            return View();
        }

        public async Task<IActionResult> Get_Joblist()
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
                // Getting all Customer data
                var customerData = _context.joblist.Include("project").Include("unit").Include("users").Where(s => s.project.active == "0").Where(s => s.deleted == 0).Select(a => new { id = a.id, jobNo = a.jobNo,projectNo = a.projectNo, description = a.project.description, nama_unit = a.unit.unitCode, eqTagNo = a.eqTagNo, userSection = a.userSection, name = a.users.name, keterangan = a.keterangan });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.keterangan.StartsWith(searchValue));
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

        public async Task<IActionResult> Get_Joblist_Detail(int projectId, string rev, int joblist)
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
                // Getting all Customer data
                var query = _context.view_readiness_detail.FromSql($"SELECT a.id, a.joblist_id, joblist.eqTagNo, STUFF((SELECT distinct  t2.[order] + ',' from joblist_detail_wo t2  where a.id= t2.jobListDetailID FOR XML PATH(''), TYPE ).value('.', 'NVARCHAR(MAX)')  ,1,0,'') wo,a.no_jasa, joblist.projectID, a.jobDesc, a.jasa, a.material, (CASE WHEN a.dikerjakan = 'tidak' then 'not_identify' WHEN a.jasa != '0' THEN  (SELECT (CASE WHEN contracttracking.aktualSP is null THEN 'not_ready' ELSE 'ready' END) as status FROM Mystap.dbo.contracttracking left join Mystap.dbo.joblist_detail on joblist_detail.no_jasa = contracttracking.idPaket where contracttracking.projectID = {projectId} and joblist_detail.id = a.id and contracttracking.deleted != '1') ELSE 'not_identify' END) as sts_kontrak, (CASE WHEN a.dikerjakan = 'tidak' then 'not_identify' WHEN a.material != '0' THEN (CASE WHEN a.status_material = 'COMPLETED' AND (SELECT (CASE WHEN joblist_detail.material != '0' THEN CASE WHEN count(DISTINCT case when procurement_.status_ != 'terpenuhi_stock' AND procurement_.status_ != 'onsite' then procurement_.[order] else NULL end) > '0' THEN 'not_ready' ELSE 'ready' END ELSE NULL END) as sts_material FROM (SELECT work_order.[order], (case when zpm01.pr is null or zpm01.pr = '' then (case when zpm01.reqmt_qty = zpm01.qty_res then 'terpenuhi_stock' else 'create_pr' end) else (case when (zpm01.pr is not null or zpm01.pr != '') and (p.po is null or p.po = '')then (case when (zpm01.status_pengadaan = 'tunggu_pr' or zpm01.status_pengadaan = 'evaluasi_dp3' or zpm01.status_pengadaan is null) then 'outstanding_pr' when zpm01.status_pengadaan = 'inquiry_harga' then 'inquiry_harga' when (zpm01.status_pengadaan = 'hps_oe' or zpm01.status_pengadaan = 'bidder_list' or zpm01.status_pengadaan = 'penilaian_kualifikasi' or zpm01.status_pengadaan = 'rfq') then 'hps_oe' when (zpm01.status_pengadaan = 'pemasukan_penawaran' or zpm01.status_pengadaan = 'pembukaan_penawaran' or zpm01.status_pengadaan = 'evaluasi_penawaran' or zpm01.status_pengadaan = 'klarifikasi_spesifikasi' or zpm01.status_pengadaan = 'evaluasi_teknis' or zpm01.status_pengadaan = 'evaluasi_tkdn' or zpm01.status_pengadaan = 'negosiasi'  or zpm01.status_pengadaan = 'lhp') then 'proses_tender' when (zpm01.status_pengadaan = 'pengumuman_pemenang' or zpm01.status_pengadaan = 'penunjuk_pemenang' or zpm01.status_pengadaan = 'purchase_order') then 'Penetapan Pemenang' else 'outstanding_pr' end) when (zpm01.pr is not null or zpm01.pr != '') and (p.po is not null or p.po != '') then (case when p.dci is null or p.dci = '' then 'tunggu_onsite' else 'onsite' end) end) end) as status_ FROM Mystap.dbo.zpm01 left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order left join Mystap.dbo.purch_order as p on p.material = zpm01.material AND p.pr = zpm01.pr AND p.item_pr = zpm01.itm_pr where work_order.revision = {rev} and ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) GROUP BY [work_order].[order],zpm01.material,zpm01.itm, zpm01.pr,zpm01.reqmt_qty, zpm01.qty_res, po, zpm01.status_pengadaan, dci) procurement_ join Mystap.dbo.joblist_detail_wo on joblist_detail_wo.[order] = procurement_.[order] join Mystap.dbo.joblist_detail on joblist_detail.id = joblist_detail_wo.jobListDetailID where joblist_detail.id = a.id GROUP by joblist_detail.material ) = 'ready' then 'ready' ELSE 'not_ready' END) ELSE 'not_identify' END) as sts_material FROM Mystap.dbo.joblist_detail a LEFT JOIN Mystap.dbo.joblist on joblist.id = a.joblist_id where joblist.projectID = {projectId} and joblist.id = {joblist}  and a.deleted != '1'");
                var data = await query.ToListAsync();

              
                var customerData = _context.joblist_Detail.Include("contracttracking").Include("joblist").Include("equipments").Include("project").Include("unit").Include("users").Where(s => s.deleted == 0).Select(a => new { id = a.id, eqTagNo = a.equipments.eqTagNo, jobDesc = a.jobDesc, alias = a.users.alias, status = a.freezing, isJasa = a.jasa, noPaket = a.contracttracking.noPaket, judul_paket = a.contracttracking.judulPekerjaan, wo_jasa = a.contracttracking.WO, no_po = a.contracttracking.po, no_sp = a.contracttracking.noSP, status_jasa = a.status_jasa, ismaterial = a.material, order = a.no_order, status_material = a.status_material, ket_status_material = a.ket_status_material, all_in_kontrak = a.all_in_kontrak, lldi = a.lldi, status_job = a.status_job, /*sts_ready = a.status_material,*/ disiplin = a.disiplin });
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.jobDesc.StartsWith(searchValue));
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
    }
}
