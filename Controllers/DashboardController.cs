using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mystap.Models;
using System.Linq.Dynamic.Core;

namespace mystap.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DatabaseContext _context;
        public DashboardController(DatabaseContext context)
        {
            _context = context;
        }
        public IActionResult DashboardEquipment()
        {
            return View();
        }

        public IActionResult GetReadinessEquipment()
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
               
                var query = "select " +
                    "joblist.jobNo, " +
                    "joblist.eqTagNo, " +
                    "CASE " +
                    "WHEN count(case when  c.status = 'not_ready' then c.id else NULL end) > '0' THEN 'not_ready' " +
                    "WHEN c.status is null then 'not_ready' " +
                    "ELSE 'ready' " +
                    "END AS 'status_tagno' " +
                    "from Mystap.dbo.joblist  " +
                    "left join (SELECT  b.id, b.eqTagNo,  b.projectID,  b.joblist_id, " +
                    "(CASE WHEN b.dikerjakan = 'tidak' then 'ready' " +
                    "WHEN b.jasa != '0' AND b.material != '0' THEN " +
                    "CASE WHEN b.sts_material = 'ready' AND b.sts_kontrak = 'ready' THEN 'ready'  ELSE 'not_ready' END " +
                    "WHEN b.material != '0' THEN isnull(b.sts_material, 'not_ready') " +
                    "WHEN b.jasa != '0' THEN isnull(b.sts_kontrak, 'not_ready') END) as status " +
                    "FROM (SELECT a.id, a.joblist_id, joblist.eqTagNo, joblist.projectID, a.jobDesc, a.jasa, a.material, a.dikerjakan, " +
                    "(CASE WHEN a.dikerjakan = 'tidak' then 'not_execute' WHEN a.jasa != '0' THEN  " +
                    "(SELECT (CASE WHEN contracttracking.aktualSP is null THEN 'not_ready' ELSE 'ready' END) as status " +
                    "FROM Mystap.dbo.contracttracking left join Mystap.dbo.joblist_detail on joblist_detail.no_jasa = contracttracking.idPaket where contracttracking.projectID = '7' and joblist_detail.id = a.id and contracttracking.deleted != '1') " +
                    "ELSE 'not_identify' END) as sts_kontrak, " +
                    "(CASE WHEN a.dikerjakan = 'tidak' then 'not_execute' " +
                    "WHEN a.material != '0' THEN (CASE WHEN a.status_material = 'COMPLETED' AND " +
                    "(SELECT (CASE WHEN joblist_detail.material != '0' THEN CASE WHEN count(DISTINCT case when procurement_.status_ != 'terpenuhi_stock' AND procurement_.status_ != 'onsite' then procurement_.[order] else NULL end) > '0' THEN 'not_ready' " +
                    "ELSE 'ready' END ELSE NULL END) as sts_material FROM (SELECT work_order.[order], (case when zpm01.pr is null or zpm01.pr = '' then (case when zpm01.reqmt_qty = zpm01.qty_res then 'terpenuhi_stock' else 'create_pr' end) else (case when (zpm01.pr is not null or zpm01.pr != '') and (p.po is null or p.po = '')then (case when (zpm01.status_pengadaan = 'tunggu_pr' or zpm01.status_pengadaan = 'evaluasi_dp3' or zpm01.status_pengadaan is null) then 'outstanding_pr' when zpm01.status_pengadaan = 'inquiry_harga' then 'inquiry_harga' when (zpm01.status_pengadaan = 'hps_oe' or zpm01.status_pengadaan = 'bidder_list' or zpm01.status_pengadaan = 'penilaian_kualifikasi' or zpm01.status_pengadaan = 'rfq') then 'hps_oe' when (zpm01.status_pengadaan = 'pemasukan_penawaran' or zpm01.status_pengadaan = 'pembukaan_penawaran' or zpm01.status_pengadaan = 'evaluasi_penawaran' or zpm01.status_pengadaan = 'klarifikasi_spesifikasi' or zpm01.status_pengadaan = 'evaluasi_teknis' or zpm01.status_pengadaan = 'evaluasi_tkdn' or zpm01.status_pengadaan = 'negisiasi'  or zpm01.status_pengadaan = 'lhp') then 'proses_tender' when (zpm01.status_pengadaan = 'pengumuman_pemenang' or zpm01.status_pengadaan = 'penunjuk_pemenang' or zpm01.status_pengadaan = 'purchase_order') then 'Penetapan Pemenang' else 'outstanding_pr' end) when (zpm01.pr is not null or zpm01.pr != '') and (p.po is not null or p.po != '') then (case when p.dci is null or p.dci = '' then 'tunggu_onsite' else 'onsite' end) end) end) as status_ " +
                    "FROM Mystap.dbo.zpm01 left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order left join Mystap.dbo.purch_order as p on p.material = zpm01.material AND p.pr = zpm01.pr AND p.item_pr = zpm01.itm_pr where work_order.revision = 'COAPOC24' and ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) " +
                    "GROUP BY [work_order].[order],zpm01.material,zpm01.itm, case when zpm01.pr is null or zpm01.pr = '' then (case when zpm01.reqmt_qty = zpm01.qty_res then 'terpenuhi_stock' else 'create_pr' end) else (case when (zpm01.pr is not null or zpm01.pr != '') and (p.po is null or p.po = '')then (case when (zpm01.status_pengadaan = 'tunggu_pr' or zpm01.status_pengadaan = 'evaluasi_dp3' or zpm01.status_pengadaan is null) then 'outstanding_pr' when zpm01.status_pengadaan = 'inquiry_harga' then 'inquiry_harga' when (zpm01.status_pengadaan = 'hps_oe' or zpm01.status_pengadaan = 'bidder_list' or zpm01.status_pengadaan = 'penilaian_kualifikasi' or zpm01.status_pengadaan = 'rfq') then 'hps_oe' when (zpm01.status_pengadaan = 'pemasukan_penawaran' or zpm01.status_pengadaan = 'pembukaan_penawaran' or zpm01.status_pengadaan = 'evaluasi_penawaran' or zpm01.status_pengadaan = 'klarifikasi_spesifikasi' or zpm01.status_pengadaan = 'evaluasi_teknis' or zpm01.status_pengadaan = 'evaluasi_tkdn' or zpm01.status_pengadaan = 'negisiasi'  or zpm01.status_pengadaan = 'lhp') then 'proses_tender' when (zpm01.status_pengadaan = 'pengumuman_pemenang' or zpm01.status_pengadaan = 'penunjuk_pemenang' or zpm01.status_pengadaan = 'purchase_order') then 'Penetapan Pemenang' else 'outstanding_pr' end) when (zpm01.pr is not null or zpm01.pr != '') and (p.po is not null or p.po != '') then (case when p.dci is null or p.dci = '' then 'tunggu_onsite' else 'onsite' end) end) end) procurement_ " +
                    "join Mystap.dbo.joblist_detail_wo on joblist_detail_wo.[order] = procurement_.[order] " +
                    "join Mystap.dbo.joblist_detail on joblist_detail.id = joblist_detail_wo.jobListDetailID " +
                    "where joblist_detail.id = a.id GROUP by joblist_detail.material ) = 'ready' " +
                    "then 'ready' ELSE 'not_ready' END) ELSE 'not_identify' END) as sts_material FROM Mystap.dbo.joblist_detail a " +
                    "LEFT JOIN Mystap.dbo.joblist on joblist.id = a.joblist_id where joblist.projectID = '7' and a.deleted != '1') b) c on c.joblist_id = joblist.id " +
                    "LEFT JOIN Mystap.dbo.project on project.projectNo = joblist.projectNo  " +
                    "where joblist.projectID = '7' and joblist.deleted != '1' " +
                    "group by joblist.jobNo, joblist.eqTagNo, c.status";

                var customerData = _context.view_readiness_equipment.FromSqlRaw(query);
                // Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.jobNo.StartsWith(searchValue) || m.eqTagNo.StartsWith(searchValue));
                }
                // Total number of rows count
                Console.WriteLine(customerData);
                recordsTotal = customerData.Count();
                // Paging
                var data = customerData.Skip(skip).Take(pageSize).ToList();
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
