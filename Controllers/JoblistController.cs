using Azure.Core;
using MessagePack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using mystap.Models;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Runtime.CompilerServices;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Configuration;
using ExcelDataReader;
using Microsoft.Extensions.Hosting;
using DocumentFormat.OpenXml.Office2010.Excel;

using Microsoft.AspNetCore.Authorization;
using mystap;
using mystap.Helpers;
using DocumentFormat.OpenXml.Presentation;
namespace joblist.Controllers
{
    public class JoblistController : Controller
    {
        private readonly DatabaseContext _context;
        IConfiguration configuration;
        IWebHostEnvironment hostEnvironment;
        public JoblistController(DatabaseContext context, IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this.configuration = configuration;
            this.hostEnvironment = hostEnvironment;
        }

		[AuthorizedAction]
		public IActionResult Joblist()
        {
            ViewBag.role = "JOB_LIST";
            if (Module.hasModule("JOB_LIST", HttpContext.Session))
            {
                ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == 1).ToList();
                ViewBag.equipment = _context.equipments.Where(p => p.deleted == 0).ToList();
                ViewBag.unitCode = _context.unit.Where(p => p.deleted != 1).GroupBy(p => new { p.unitCode, p.unitProses }).Select(p => new { unitCode = p.Key.unitCode, unitProses = p.Key.unitProses }).ToList();
                ViewBag.unit = _context.unit.Where(p => p.deleted == 0).ToList();
                return View();
            }
            else
            {
                return NotFound();
            }
            
        }

		[AuthorizedAction]
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

                var project = Request.Form["project"].FirstOrDefault();
                var project_rev = Request.Form["project_rev"].FirstOrDefault();
                var eqtagno = Request.Form["eqTagNo"].FirstOrDefault();
                var jobno = Request.Form["jobNo"].FirstOrDefault();
                var unitcode = Request.Form["unitCode"].FirstOrDefault();
                var user_section = Request.Form["user_section"].FirstOrDefault();
                // Getting all Customer data
                var query = @"select max(joblist.id) as id, max(equipments.id) as id_eqtagno, max(joblist.jobNo) as jobNo, max(joblist.userSection) as userSection,max(project.projectNo) as projectNo, max(project.description) as description, max(unit.unitCode) as nama_unit, max(users.name) as name, max(joblist.keterangan) as keterangan,max(project.revision) as revision, equipments.eqTagNo, CASE WHEN count(case when c.status = 'not_ready' then c.id else NULL end) > '0' THEN 'not_ready' WHEN max(c.status) is null then 'not_ready' ELSE 'ready' END AS 'status_tagno' from Mystap.dbo.joblist  " +
                    " left join (SELECT  b.id, b.projectID,  b.joblist_id, (CASE WHEN b.dikerjakan = 'tidak' then 'ready' WHEN b.jasa != '0' AND b.material != '0' THEN CASE WHEN b.sts_material = 'ready' AND b.sts_kontrak = 'ready' THEN 'ready'  ELSE 'not_ready' END WHEN b.material != '0' THEN isnull(b.sts_material, 'not_ready') WHEN b.jasa != '0' THEN isnull(b.sts_kontrak, 'not_ready') END) as status FROM (SELECT a.id, a.joblist_id, joblist.projectID, a.jobDesc, a.jasa, a.material, a.dikerjakan, " +
                    " (CASE WHEN a.dikerjakan = 'tidak' then 'not_execute' WHEN a.jasa != '0' THEN  (SELECT (CASE WHEN contracttracking.aktualSP is null THEN 'not_ready' ELSE 'ready' END) as status FROM Mystap.dbo.contracttracking left join Mystap.dbo.joblist_detail on joblist_detail.no_jasa = contracttracking.idPaket where contracttracking.projectID = " + project + " and joblist_detail.id = a.id and contracttracking.deleted != '1') ELSE 'not_identify' END) as sts_kontrak, " +
                    " (CASE WHEN a.dikerjakan = 'tidak' then 'not_execute' WHEN a.material != '0' THEN (CASE WHEN a.status_material = 'COMPLETED' AND (SELECT (CASE WHEN joblist_detail.material != '0' THEN CASE WHEN count(DISTINCT case when procurement_.status_ != 'terpenuhi_stock' AND procurement_.status_ != 'onsite' then procurement_.[order] else NULL end) > '0' THEN 'not_ready' ELSE 'ready' END ELSE NULL END) as sts_material " +
                    " FROM (SELECT work_order.[order], (case when zpm01.pr is null or zpm01.pr = '' then (case when zpm01.reqmt_qty = zpm01.qty_res then 'terpenuhi_stock' else 'create_pr' end) else (case when (zpm01.pr is not null or zpm01.pr != '') and (p.po is null or p.po = '')then (case when (zpm01.status_pengadaan = 'tunggu_pr' or zpm01.status_pengadaan = 'evaluasi_dp3' or zpm01.status_pengadaan is null) " +
                    " then 'outstanding_pr' when zpm01.status_pengadaan = 'inquiry_harga' then 'inquiry_harga' when (zpm01.status_pengadaan = 'hps_oe' or zpm01.status_pengadaan = 'bidder_list' or zpm01.status_pengadaan = 'penilaian_kualifikasi' or zpm01.status_pengadaan = 'rfq') then 'hps_oe' when (zpm01.status_pengadaan = 'pemasukan_penawaran' or zpm01.status_pengadaan = 'pembukaan_penawaran' or zpm01.status_pengadaan = 'evaluasi_penawaran' or zpm01.status_pengadaan = 'klarifikasi_spesifikasi' or zpm01.status_pengadaan = 'evaluasi_teknis' or zpm01.status_pengadaan = 'evaluasi_tkdn' or zpm01.status_pengadaan = 'negosiasi'  or zpm01.status_pengadaan = 'lhp') then 'proses_tender' when (zpm01.status_pengadaan = 'pengumuman_pemenang' or zpm01.status_pengadaan = 'penunjuk_pemenang' or zpm01.status_pengadaan = 'purchase_order') then 'Penetapan Pemenang' else 'outstanding_pr' end) " +
                    " when (zpm01.pr is not null or zpm01.pr != '') and (p.po is not null or p.po != '') then (case when p.dci is null or p.dci = '' then 'tunggu_onsite' else 'onsite' end) end) end) as status_ " +
                    " FROM Mystap.dbo.zpm01 left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order left join Mystap.dbo.purch_order as p on p.material = zpm01.material AND p.pr = zpm01.pr AND p.item_pr = zpm01.itm_pr where work_order.revision = '" + project_rev + "' and ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) GROUP BY [work_order].[order],zpm01.itm, zpm01.pr,zpm01.reqmt_qty, zpm01.qty_res, po, zpm01.status_pengadaan, dci) procurement_ " +
                    " join Mystap.dbo.joblist_detail_wo on joblist_detail_wo.[order] = procurement_.[order] join Mystap.dbo.joblist_detail on joblist_detail.id = joblist_detail_wo.jobListDetailID where joblist_detail.id = a.id GROUP by joblist_detail.material ) = 'ready' then 'ready' ELSE 'not_ready' END) ELSE 'not_identify' END) as sts_material " +
                    " FROM Mystap.dbo.joblist_detail a LEFT JOIN Mystap.dbo.joblist on joblist.id = a.joblist_id  where joblist.projectID = " + project + " and a.deleted != '1') b) c on c.joblist_id = joblist.id  LEFT JOIN Mystap.dbo.equipments on equipments.id = joblist.id_eqTagNo LEFT JOIN Mystap.dbo.project on project.id = joblist.projectID " +
                    " LEFT JOIN Mystap.dbo.unit on unit.id = joblist.unitCode LEFT JOIN Mystap.dbo.users on users.id = joblist.createBy where joblist.deleted != '1' ";


                if (project != "") {
                    query += " AND joblist.projectID = "+project+" ";
                }

                if (eqtagno != "") {
                    query += " AND equipments.eqTagNo like '"+eqtagno+"%' ";
                }

                if (jobno != "") {
                    query += " AND joblist.jobNo like '"+jobno+"%' ";
                }

                if (unitcode != "") {
                    query += " AND unit.unitCode = '"+unitcode+"' ";
                }

                if (user_section != "") {
                    query += " AND joblist.userSection = '"+user_section+"' ";
                }

                query += " group by equipments.eqTagNo  ";


                var c = FormattableStringFactory.Create(query);
                var customerData = _context.view_joblist.FromSql(c);

                if (!(string.IsNullOrEmpty(sortColumn) && (string.IsNullOrEmpty(sortColumnDirection) || sortColumnDirection != "-1")))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.eqTagNo.Contains(searchValue) || b.projectNo.Contains(searchValue) || b.jobNo.Contains(searchValue));
                }

                // Total number of rows count
                //Console.WriteLine(customerData);
                recordsTotal = customerData.Count();
                if (pageSize > 0)
                {
                    // Paging
                    customerData = customerData.Skip(skip).Take(pageSize);
                }

                var datas = await customerData.ToListAsync();
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

		[AuthorizedAction]
		public IActionResult CreateJoblist()
        {
            ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == 1).ToList();
            ViewBag.equipment = _context.equipments.Where(p => p.deleted == 0).ToList();
            ViewBag.unitCode = _context.unit.Where(p => p.deleted != 1).GroupBy(p => new { p.unitCode, p.unitProses }).Select(p => new { unitCode = p.Key.unitCode, unitProses = p.Key.unitProses }).ToList();
            ViewBag.unit = _context.unit.Where(p => p.deleted == 0).ToList();

            return View();
        }

        [AuthorizedAction]
        public IActionResult UpdateJoblist(long? id)
        {
            ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == 1).ToList();
            ViewBag.equipment = _context.equipments.Where(p => p.deleted == 0).ToList();
            ViewBag.unitCode = _context.unit.Where(p => p.deleted != 1).GroupBy(p => new { p.unitCode, p.unitProses }).Select(p => new { unitCode = p.Key.unitCode, unitProses = p.Key.unitProses }).ToList();
            ViewBag.unit = _context.unit.Where(p => p.deleted == 0).ToList();

            var data = (from j in _context.joblist
                        join e in _context.equipments on j.id_eqTagNo equals e.id into Equipments
                        from e in Equipments.DefaultIfEmpty()
                        join u in _context.unit on j.unitCode equals u.id into Unit
                        from u in Unit.DefaultIfEmpty()
                        join p in _context.project on j.projectID equals p.id into Project
                        from p in Project.DefaultIfEmpty()
                        select new
                        {
                            projectID = p.id,
                            projectNo = p.projectNo,
                            taoh = p.taoh,
                            unitCode = u.unitCode,
                            codeJob = u.codeJob,
                            unitKilang = u.unitKilang,
                            eqTagNo = e.eqTagNo,
                            catalog_profile = e.catalog_profile,
                            start_date = j.start_date_oh,
                            id = j.id,
                            jobNo = j.jobNo,
                            criteriaMI = j.criteriaMI,
                            criteriaPI = j.criteriaPI,
                            criteriaOPT = j.criteriaOPT,
                            userSection = j.userSection,
                            deleted = j.deleted,
                            remarks = j.remarks,
                            keterangan = j.keterangan,

                        }).Where(p => p.id == Convert.ToInt64(id)).FirstOrDefault();

            ViewBag.data = data;
            return View();
        }

		[AuthorizedAction]
		public IActionResult CreateJoblist_(IFormCollection formcollaction)
        {
            try
            {
                
                Joblist joblist = new Joblist();
                joblist.projectNo = formcollaction["projectNo"];
                joblist.unitCode = Convert.ToInt64(formcollaction["unitId"]);
                //joblist.eqTagNo = formcollaction["eqTagNo"];
                joblist.status = 0;
                joblist.criteriaMI = (string.IsNullOrEmpty(formcollaction["criteriaMI"])) ? 0 : 1;
                joblist.criteriaPI = (string.IsNullOrEmpty(formcollaction["criteriaPI"])) ? 0 : 1;
                joblist.criteriaOPT = (string.IsNullOrEmpty(formcollaction["criteriaOPT"])) ? 0 : 1;
                joblist.userSection = formcollaction["taoh"];
                joblist.remarks = formcollaction["remarks"];
                joblist.createBy = HttpContext.Session.GetInt32("id");
                joblist.dateCreated = DateTime.Now;
                joblist.id_eqTagNo = Convert.ToInt64(formcollaction["id_eqtagno"]);
                joblist.projectID = Convert.ToInt64(formcollaction["projectID"]);

                var cek_tag = (from j in _context.joblist
                           join p in _context.project on j.projectID equals p.id
                           join e in _context.equipments on j.id_eqTagNo equals e.id
                           select new
                           {
                               id = j.id,
                               projectID = p.id,
                               jobNo = j.jobNo,
                               deleted = j.deleted,
                               eqTagNo = e.eqTagNo,
                               id_eqTagNo = e.id
                           }).Where(p => p.deleted == 0).Where(p => p.projectID == Convert.ToInt64(formcollaction["projectID"])).Where(p => p.id_eqTagNo == Convert.ToInt64(formcollaction["id_eqTagNo"])).FirstOrDefault();
                Boolean t;
                long joblist_id;
                string jobNo;

                if (cek_tag == null)
                {
                    string tahun = Request.Form["year"].FirstOrDefault();
                    string idCode = Request.Form["codeJob"].FirstOrDefault() + Request.Form["catprof"].FirstOrDefault() + Request.Form["month"].FirstOrDefault() + tahun.Substring(tahun.Length - 2);
                    var cek = _context.joblist.Where(w => w.jobNo.Contains(idCode)).Select(p => new
                    {
                        kode = p.jobNo
                    }).Max(p => p.kode);

                    int no = 0;
                    if (cek == null)
                    {
                        no = 1;
                    }
                    else
                    {
                        no = Convert.ToInt32(cek) + 1;
                    }

                    if(joblist.userSection == "OH")
                    {
                        var start_date = formcollaction["start_date"];
                        joblist.start_date_oh = Convert.ToDateTime(start_date);
                    }
                    joblist.jobNo = idCode + no.ToString("D3");
                    joblist.deleted = 0;
                    _context.joblist.Add(joblist);
                    _context.SaveChanges();

                    t = true;
                    joblist_id = joblist.id;
                    jobNo = idCode + no.ToString("D3");

                }
                else
                {
                    t = false;
                    joblist_id = cek_tag.id;
                    jobNo = cek_tag.jobNo;

                }

                return Json(new { result = t, joblist_id = joblist_id, jobNo = jobNo , eqTagNo = formcollaction["eqTagNo"] });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult updateJoblist_(IFormCollection formcollaction)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id_joblist"].FirstOrDefault());
                Joblist obj = _context.joblist.Where(p => p.id == id).FirstOrDefault();

                
                if (obj != null)
                {
                    var cek_project = (from j in _context.joblist
                                       join p in _context.project on j.projectID equals p.id
                                       select new
                                       {
                                           projectID = p.id,
                                           id = j.id
                                       }).Where(p => p.id == id).FirstOrDefault();
                    if (cek_project.projectID != Convert.ToInt32(formcollaction["projectID"]))
                    {
                        string tahun = Request.Form["year"].FirstOrDefault();
                        string idCode = Request.Form["codeJob"].FirstOrDefault() + Request.Form["catprof"].FirstOrDefault() + Request.Form["month"].FirstOrDefault() + tahun.Substring(tahun.Length - 2);
                        var cek = _context.joblist.Where(w => w.jobNo.Contains(idCode)).Max(p => p.jobNo);
                        string jn = cek;
                        int no = 0;
                        if (cek == null)
                        {
                            no = 1;
                        }
                        else
                        {
                            no = Convert.ToInt32(jn.Substring(jn.Length - 3)) + 1;
                        }
                        obj.jobNo = idCode + no.ToString("D3");
                        obj.projectNo = formcollaction["projectNo"];
                        obj.projectID = Convert.ToInt64(formcollaction["projectID"]);
                        if (formcollaction["taoh"] == "OH")
                        {
                            var start_date = formcollaction["start_date"];
                            obj.start_date_oh = Convert.ToDateTime(start_date);
                        }
                        obj.userSection = formcollaction["taoh"];
                    }
                
                    obj.criteriaMI = (string.IsNullOrEmpty(formcollaction["criteriaMI"])) ? 0 : 1;
                    obj.criteriaPI = (string.IsNullOrEmpty(formcollaction["criteriaPI"])) ? 0 : 1;
                    obj.criteriaOPT = (string.IsNullOrEmpty(formcollaction["criteriaOPT"])) ? 0 : 1;
                    obj.remarks = formcollaction["remarks"];
                    obj.updatedBy = HttpContext.Session.GetInt32("id");
                    obj.modifyBy = HttpContext.Session.GetInt32("id");
                    obj.lastModify = DateTime.Now;

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

		[AuthorizedAction]
		public IActionResult getEqtagno()
        {
            var unitCode = Request.Form["unitCode"].FirstOrDefault();
            var unitKilang = Request.Form["unitKilang"].FirstOrDefault();
            var data = _context.equipments.Where(p => p.unitProses == unitCode).Where(p => p.unitKilang == unitKilang).Where(p => p.deleted == 0).ToList();
            var select = "<option value=''>Select EqTagNo</option>";
            foreach (var val in data)
            {
                select += "<option value='" + val.eqTagNo + "' data-id='" + val.id + "' data-catprof='" + val.catalog_profile + "'>" + val.eqTagNo + "</option>";
            }

            return Ok(select);
        }

		[AuthorizedAction]
		public IActionResult cekEqtagno()
        {
            var cek_tag = (from j in _context.joblist
                           join p in _context.project on j.projectID equals p.id
                           join e in _context.equipments on j.id_eqTagNo equals e.id
                           select new
                           {
                               id = j.id,
                               jobNo = j.jobNo,
                               id_eqTagNo = e.id,
                               eqTagNo = e.eqTagNo,
                               criteriaMI = j.criteriaMI,
                               criteriaPI = j.criteriaPI,
                               criteriaOPT = j.criteriaOPT,
                               remarks = j.remarks,
                               project_id = p.id,
                               deleted = j.deleted,
                           }).Where(p => p.project_id == Convert.ToInt64(Request.Form["project"].FirstOrDefault())).Where(p => p.id_eqTagNo == Convert.ToInt64(Request.Form["eqtagno"])).Where(p => p.deleted != 1).FirstOrDefault();
 
            if (cek_tag != null)
            {
                var data = new
                {
                    joblist_id = cek_tag.id,
                    jobNo = cek_tag.jobNo,
                    eqTagNo = cek_tag.eqTagNo,
                    criteriaMI = cek_tag.criteriaMI,
                    criteriaPI = cek_tag.criteriaPI,
                    criteriaOPT = cek_tag.criteriaOPT,
                    remarks = cek_tag.remarks,
                    result = true
                };
                return Json(data);
            }
            else
            {
                var data = new
                {
                    result = false,
                };
                return Json(data);
            }
        }

		[AuthorizedAction]
		public IActionResult getMemo()
        {
            var project = Request.Form["project"].FirstOrDefault();
            var data = (from m in _context.memo
                        join p in _context.project on m.projectID equals p.id
                        select new
                        {
                            id = m.id,
                            projectID = p.id,
                            reqNo = m.reqNo,
                            reqDesc = m.reqDesc,
                            deleted = m.deleted,
                        }).Where(p => p.deleted != 1).Where(p => p.projectID == Convert.ToInt64(project)).ToList();

            var isi = "<option value=''>Select Memo</option>";
            foreach (var val in data)
            {
                isi += "<option value='"+ val.id +"'>"+ val.reqNo + "   ("+ val.reqDesc + ")</option>";
            }

            return Ok(isi);

        }

		[AuthorizedAction]
		public IActionResult getMemoSelected()
        {
            var id = Request.Form["joblist_id"].FirstOrDefault();
            var data = _context.joblistDetailMemo.Where(p => p.jobListDetailID == Convert.ToInt64(id)).Select(p => new { id_memo = p.id_memo }).ToList();
            var arr = new ArrayList();
            foreach(var val in data)
            {
                arr.Add(val.id_memo);
            }

            return Json(arr);
        }

		[AuthorizedAction]
		public IActionResult getNotifikasi()
        {
            var data = _context.notifikasi.ToList();
            var isi = "<option value=''>Select Notifikasi</option>";
            foreach (var val in data)
            {
                isi += "<option value='" + val.id + "' data-notifikasi='"+val.notifikasi+"'>" + val.notifikasi + "</option>";
            }
            return Ok(isi);
        }

		[AuthorizedAction]
		public async Task<IActionResult> JoblistDetail_()
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

                var jobNo = Request.Form["jobno"].FirstOrDefault();
                var joblist_id = Request.Form["joblist_id"].FirstOrDefault();
                
                var customerData = _context.joblist_Detail.Where(p => p.deleted != 1);

                if(jobNo != "")
                {
                    customerData = customerData.Where(p => p.jobNo ==  jobNo);
                }

                if(joblist_id != "")
                {
                    customerData = customerData.Where(p => p.joblist_id == Convert.ToInt64(joblist_id));
                }

                if (!(string.IsNullOrEmpty(sortColumn) && (string.IsNullOrEmpty(sortColumnDirection) || sortColumnDirection != "-1")))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.jobDesc.Contains(searchValue));
                }

                // Total number of rows count
                //Console.WriteLine(customerData);
                recordsTotal = customerData.Count();
                if (pageSize > 0)
                {
                    // Paging
                    customerData = customerData.Skip(skip).Take(pageSize);
                }

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
		public IActionResult createJoblistDetail(IFormCollection formcollaction)
        {
            try
            {
                var cek = _context.joblist_Detail.Where(p => p.joblist_id == Convert.ToInt64(Request.Form["joblist_id"].FirstOrDefault())).Select(p => new
                {
                    kode = p.jobDetailNo
                }).Max(p => p.kode);

                int no = 0;
                if (cek == null)
                {
                    no = 1;
                }
                else
                {
                    no = Convert.ToInt32(cek) + 1;
                }

                Joblist_Detail job = new Joblist_Detail();
                job.jobDetailNo = no.ToString("D3");
                job.joblist_id = Convert.ToInt64(Request.Form["joblist_id"].FirstOrDefault());
                job.jobNo = formcollaction["jobno_detail"];
                job.jobDesc = formcollaction["jobDesc"];
                job.alasan = formcollaction["alasan"];
                job.engineer = formcollaction["insepector"];
                job.revision = formcollaction["revision"];
                job.execution = formcollaction["eksekusi"];
                job.responsibility = formcollaction["responbility"];
                job.ram = formcollaction["ram"];
                job.cleaning = (string.IsNullOrEmpty(formcollaction["cleaning"])) ? 0 : 1;
                job.inspection = (string.IsNullOrEmpty(formcollaction["inspection"])) ? 0 : 1;
                job.repair = (string.IsNullOrEmpty(formcollaction["repair"])) ? 0 : 1;
                job.replace = (string.IsNullOrEmpty(formcollaction["replace"])) ? 0 : 1;
                job.ndt = (string.IsNullOrEmpty(formcollaction["ndt"])) ? 0 : 1;
                job.modif = (string.IsNullOrEmpty(formcollaction["modif"])) ? 0 : 1;
                job.tein = (string.IsNullOrEmpty(formcollaction["tiein"])) ? 0 : 1;
                job.coc = (string.IsNullOrEmpty(formcollaction["coc"])) ? 0 : 1;
                job.drawing = (string.IsNullOrEmpty(formcollaction["drawing"])) ? 0 : 1;
                job.measurement = (string.IsNullOrEmpty(formcollaction["measurement"])) ? 0 : 1;
                job.hsse = (string.IsNullOrEmpty(formcollaction["hsse"])) ? 0 : 1;
                job.reliability = (string.IsNullOrEmpty(formcollaction["reliability"])) ? 0 : 1;
                job.losses = (string.IsNullOrEmpty(formcollaction["losses"])) ? 0 : 1;
                job.energi = (string.IsNullOrEmpty(formcollaction["energi"])) ? 0 : 1;
                job.disiplin = formcollaction["dicipline"];
                job.project = (string.IsNullOrEmpty(formcollaction["project"])) ? 0 : 1;
                job.critical_job = (string.IsNullOrEmpty(formcollaction["critical_job"])) ? 0 : 1;
                job.freezing = (string.IsNullOrEmpty(formcollaction["freezing"])) ? 0 : 1;
                job.notif = Convert.ToInt64(formcollaction["notifikasi"]);
                job.status_job = "NOT_IDENTIFY";
                job.deleted = 0;

                using var transaction = _context.Database.BeginTransaction();
                Boolean t;
                try
                {
                    _context.joblist_Detail.Add(job);
                    _context.SaveChanges();

                    var memo = formcollaction["memo[]"];
                    if (!memo.IsNullOrEmpty()) {
                        var cek_memo = _context.joblistDetailMemo.Where(p => p.jobListDetailID == job.id).ToList();
                        if (!cek_memo.IsNullOrEmpty())
                        {
                            _context.joblistDetailMemo.Where(p => p.jobListDetailID == job.id).ExecuteDelete();
                        }
                        foreach (var val in memo)
                        {
                            JoblistDetailMemo jobmom = new JoblistDetailMemo();
                            jobmom.jobListDetailID = job.id;
                            jobmom.id_memo = Convert.ToInt64(val);

                            _context.joblistDetailMemo.Add(jobmom);
                            _context.SaveChanges();

                        }
                    }
                    transaction.Commit();
                    t = true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    t = false;
                }

                return Json(new { result = t });

            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult updateJoblistDetail(IFormCollection formcollaction)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Joblist_Detail job = _context.joblist_Detail.Where(p => p.id == id).FirstOrDefault();

                if(job != null)
                {
                    job.jobDesc = formcollaction["jobDesc"];
                    job.alasan = formcollaction["alasan"];
                    job.engineer = formcollaction["insepector"];
                    job.revision = formcollaction["revision"];
                    job.execution = formcollaction["eksekusi"];
                    job.responsibility = formcollaction["responbility"];
                    job.ram = formcollaction["ram"];
                    job.cleaning = (string.IsNullOrEmpty(formcollaction["cleaning"])) ? 0 : 1;
                    job.inspection = (string.IsNullOrEmpty(formcollaction["inspection"])) ? 0 : 1;
                    job.repair = (string.IsNullOrEmpty(formcollaction["repair"])) ? 0 : 1;
                    job.replace = (string.IsNullOrEmpty(formcollaction["replace"])) ? 0 : 1;
                    job.ndt = (string.IsNullOrEmpty(formcollaction["ndt"])) ? 0 : 1;
                    job.modif = (string.IsNullOrEmpty(formcollaction["modif"])) ? 0 : 1;
                    job.tein = (string.IsNullOrEmpty(formcollaction["tiein"])) ? 0 : 1;
                    job.coc = (string.IsNullOrEmpty(formcollaction["coc"])) ? 0 : 1;
                    job.drawing = (string.IsNullOrEmpty(formcollaction["drawing"])) ? 0 : 1;
                    job.measurement = (string.IsNullOrEmpty(formcollaction["measurement"])) ? 0 : 1;
                    job.hsse = (string.IsNullOrEmpty(formcollaction["hsse"])) ? 0 : 1;
                    job.reliability = (string.IsNullOrEmpty(formcollaction["reliability"])) ? 0 : 1;
                    job.losses = (string.IsNullOrEmpty(formcollaction["losses"])) ? 0 : 1;
                    job.energi = (string.IsNullOrEmpty(formcollaction["energi"])) ? 0 : 1;
                    job.disiplin = formcollaction["dicipline"];
                    job.project = (string.IsNullOrEmpty(formcollaction["project"])) ? 0 : 1;
                    job.critical_job = (string.IsNullOrEmpty(formcollaction["critical_job"])) ? 0 : 1;
                    job.freezing = (string.IsNullOrEmpty(formcollaction["freezing"])) ? 0 : 1;
                    job.notif = Convert.ToInt64(formcollaction["notifikasi"]);

                    using var transaction = _context.Database.BeginTransaction();
                    Boolean t;
                    try
                    {
                        _context.SaveChanges();

                        var memo = formcollaction["memo[]"];
                        if (!memo.IsNullOrEmpty())
                        {
                            var cek_memo = _context.joblistDetailMemo.Where(p => p.jobListDetailID == id).ToList();
                            if (!cek_memo.IsNullOrEmpty())
                            {
                                _context.joblistDetailMemo.Where(p => p.jobListDetailID == id).ExecuteDelete();
                            }
                            foreach (var val in memo)
                            {
                                JoblistDetailMemo jobmom = new JoblistDetailMemo();
                                jobmom.jobListDetailID = id;
                                jobmom.id_memo = Convert.ToInt64(val);

                                _context.joblistDetailMemo.Add(jobmom);
                                _context.SaveChanges();

                            }
                        }
                        transaction.Commit();
                        t = true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        t = false;
                    }

                    return Json(new { result = t });
                }
                return Json(new { result = false });
                
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult deleteJoblistDetail(Joblist_Detail joblist_detail)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                Joblist_Detail obj = _context.joblist_Detail.Where(p => p.id == id).FirstOrDefault();

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

		[AuthorizedAction]
		public IActionResult CarryOffer()
        {
            try
            {
                var project = Request.Form["project"].FirstOrDefault();
                var eqtagno = Request.Form["eqTagNo"].FirstOrDefault();
                var id = Request.Form["hidden_id"].FirstOrDefault();

                var cek = (from j in _context.joblist
                           join p in _context.project on j.projectID equals p.id
                           join e in _context.equipments on j.id_eqTagNo equals e.id
                           select new
                           {
                               id = j.id,
                               projectID = p.id,
                               id_eqTagNo = e.id,
                               deleted = j.deleted,

                           }).Where(p => p.deleted == 0).Where(p => p.projectID == Convert.ToInt32(project)).Where(p => p.id_eqTagNo == Convert.ToInt32(eqtagno)).FirstOrDefault();
                if (cek == null)
                {
                    int id_ = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                    Joblist job = _context.joblist.Where(p => p.id == id_).FirstOrDefault();
                    if(job != null)
                    {
                        job.projectID = Convert.ToInt64(project);
                        job.keterangan = Request.Form["keterangan"].FirstOrDefault();
                        job.modifyBy = HttpContext.Session.GetInt32("id");
                        job.lastModify = DateTime.Now;
 
                        _context.SaveChanges();
                        return Json(new { result = true });
                    }
                    else
                    {
                        return Json(new { result = false });

                    }

                }
                else
                {
                    return Json(new { result = "ada" });
                }

            }
            catch
            {
                throw;
            }
        }

        [AuthorizedAction]
        public IActionResult Planning()
        {
            ViewBag.role = "JOBPLAN";
            if (Module.hasModule("JOBPLAN", HttpContext.Session))
            {
                ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == 1).ToList();
                ViewBag.equipment = _context.equipments.Where(p => p.deleted == 0).ToList();
                ViewBag.unitCode = _context.unit.Where(p => p.deleted != 1).GroupBy(p => new { p.unitCode, p.unitProses }).Select(p => new { unitCode = p.Key.unitCode, unitProses = p.Key.unitProses }).ToList();
                ViewBag.unit = _context.unit.Where(p => p.deleted == 0).ToList();
                ViewBag.user = _context.users.Where(p => p.locked != 1).Where(p => p.statPekerja == "PLANNER").Where(p => p.alias != null && p.alias != "").Where(p => p.status == "PEKERJA").ToList();
                return View();
            }
            else
            {
                return NotFound();
            }
           
        }

		[AuthorizedAction]
		public async Task<IActionResult> Get_Joblist_Detail()
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
                var project = Request.Form["project"].FirstOrDefault();
                var rev = Request.Form["project_rev"].FirstOrDefault();
                var eqtagno = Request.Form["eqTagNo"].FirstOrDefault();
                var jobno = Request.Form["jobNo"].FirstOrDefault();
                var freezing = Request.Form["freezing"].FirstOrDefault();
                var status_job = Request.Form["status_job"].FirstOrDefault();
                var pic = Request.Form["pic"].FirstOrDefault();
                var lldi = Request.Form["lldi"].FirstOrDefault();
                var disiplin = Request.Form["disiplin"].FirstOrDefault();
                var status_jasa = Request.Form["jasa"].FirstOrDefault();
                var status_material = Request.Form["material"].FirstOrDefault();
                var all_in_kontrak = Request.Form["all_in_kontrak"].FirstOrDefault();
                var table = Request.Form["table"].FirstOrDefault();
                var planning = Request.Form["planning"].FirstOrDefault();

                var v = "";
                if (!string.IsNullOrEmpty(eqtagno))
                {
                    v += " AND joblist.eqTagNo like '%"+eqtagno+"%' ";
                }

                if (!string.IsNullOrEmpty(jobno))
                {
                    v += " AND joblist.jobNo like '%" + jobno + "%' ";
                }

                if (!string.IsNullOrEmpty(freezing))
                {
                    v += " AND b.freezing = "+freezing+" ";
                }


                if (!string.IsNullOrEmpty(status_job))
                {
                    v += " AND b.status_job = '" + status_job + "' ";
                }

                if (!string.IsNullOrEmpty(pic))
                {
                    v += " AND b.pic = " + pic + " ";
                }

                if (!string.IsNullOrEmpty(lldi))
                {
                    v += " AND b.lldi = " + lldi + " ";
                }

                if (!string.IsNullOrEmpty(disiplin))
                {
                    v += " AND b.disiplin = '" + disiplin + "' ";
                }

                if (!string.IsNullOrEmpty(status_jasa))
                {
                    v += " AND b.status_jasa = '" + status_jasa + "' ";
                }

                if (!string.IsNullOrEmpty(status_material))
                {
                    v += " AND b.status_material = '" + status_material + "' ";
                }

                if (!string.IsNullOrEmpty(all_in_kontrak))
                {
                    v += " AND b.all_in_kontrak = " + all_in_kontrak + " ";
                }

                if(table == "paket")
                {
                    v += " AND b.id_paket is null ";
                }

                if(planning != ""){
                    if (planning == "jasa") {
                        v += " AND b.jasa = 1 AND b.material = 0 AND b.all_in_kontrak = 0 ";
                    } else if (planning == "jasa_all_in") {
                        v += " AND b.jasa = 1 AND b.material = 0 AND b.all_in_kontrak = 1 ";
                    } else if (planning == "material") {
                        v += " AND b.jasa = 0 AND b.material = 1 AND b.all_in_kontrak = 0 ";
                    } else if (planning == "material_jasa") {
                        v += " AND b.jasa = 1 AND b.material = 1 AND b.all_in_kontrak = 0 ";
                    } else if (planning == "not_identify") {
                        v += " AND b.jasa = 0 AND b.material = 0 AND b.all_in_kontrak = 0 ";
                    }
                }

                var query = @"SELECT b.id, max(equipments.eqTagNo) as eqTagNo, max(users.alias) as alias,max(b.jobDesc) as jobDesc, max(b.alasan) as alasan, max(b.all_in_kontrak) as all_in_kontrak, max(b.cleaning) as cleaning, max(b.coc) as coc, max(b.critical_job) as critical_job, max(b.deleted) as deleted, max(b.dikerjakan) as dikerjakan, max(b.drawing) as drawing, max(b.energi) as energi, max(b.engineer) as engineer, max(b.execution) as execution, max(b.freezing) as freezing, max(b.hsse) as hsse, max(b.id_paket) as id_paket,max(b.inspection) as inspection, max(b.jasa) as jasa, max(b.jobDetailNo) as jobDetailNo, max(joblist.jobNo) as jobNo, max(joblist.id) as joblist_id, max(b.ket_status_material) as ket_status_material, max(b.keterangan) as keterangan, max(b.lldi) as lldi, max(b.losses) as losses, max(b.material) as material, max(b.measurement) as measurement, max(b.mitigasi) as mitigasi, max(b.modif) as modif, max(b.ndt) as ndt,max(b.no_jasa) as no_jasa, max(b.no_order) as no_order, max(notifikasi.id) as notif,max(notifikasi.notifikasi) as no_notif ,max(notifikasi.rekomendasi) as link_rekomendasi,max(b.order_jasa) as order_jasa, max(b.pekerjaan) as pekerjaan, max(b.pic) as pic, max(b.project) as project, max(b.ram) as ram, max(b.reliability) as reliability, max(b.repair) as repair, max(b.replace) as replace, max(b.responsibility) as responsibility, max(b.revision) as revision, max(b.status_jasa) as status_jasa, max(b.status_material) as status_material, max(b.tein) as tein, max(b.disiplin) as disiplin, max(b.status_job) as status_job, "+
                    "max(project.id) as id_project, string_agg(joblist_detail_wo.[order], ',') as wo, string_agg(request.reqNo, ',') as no_memo, string_agg(request.attach, ',') as file_memo, max(contracttracking.noPaket) as noPaket, max(contracttracking.WO) as wo_jasa, max(contracttracking.judulPekerjaan) as judul_paket, max(contracttracking.po) as no_po, max(contracttracking.noSP) as no_sp, string_agg(bom_files.files, ';') as file_bom, "+
                    "(CASE WHEN max(b.dikerjakan) = 'tidak' then 'ready' WHEN max(b.jasa) != '0' AND max(b.material) != '0' THEN CASE WHEN max(b.sts_material) = 'ready' AND max(b.sts_kontrak) = 'ready' THEN 'ready'  ELSE 'not_ready' END WHEN max(b.material) != '0' THEN isnull(max(b.sts_material), 'not_ready') WHEN max(b.jasa) != '0' THEN isnull(max(b.sts_kontrak), 'not_ready') END) as status "+ 
                    "FROM (SELECT a.*, (CASE WHEN a.dikerjakan = 'tidak' then 'not_execute' WHEN a.jasa != '0' THEN  (SELECT (CASE WHEN contracttracking.aktualSP is null THEN 'not_ready' ELSE 'ready' END) as status FROM Mystap.dbo.contracttracking left join Mystap.dbo.joblist_detail on joblist_detail.no_jasa = contracttracking.idPaket where contracttracking.projectID ="+Convert.ToInt64(project)+" and joblist_detail.id = a.id and contracttracking.deleted != '1') ELSE 'not_identify' END) as sts_kontrak, "+
                    "(CASE WHEN a.dikerjakan = 'tidak' then 'not_execute' WHEN a.material != '0' THEN (CASE WHEN a.status_material = 'COMPLETED' AND (SELECT (CASE WHEN joblist_detail.material != '0' THEN CASE WHEN count(DISTINCT case when procurement_.status_ != 'terpenuhi_stock' AND procurement_.status_ != 'onsite' then procurement_.[order] else NULL end) > '0' THEN 'not_ready' ELSE 'ready' END ELSE NULL END) as sts_material  "+
                    "FROM (SELECT work_order.[order], (case when zpm01.pr is null or zpm01.pr = '' then (case when zpm01.reqmt_qty = zpm01.qty_res then 'terpenuhi_stock' else 'create_pr' end) else (case when (zpm01.pr is not null or zpm01.pr != '') and (p.po is null or p.po = '')then (case when (zpm01.status_pengadaan = 'tunggu_pr' or zpm01.status_pengadaan = 'evaluasi_dp3' or zpm01.status_pengadaan is null) "+
                    "then 'outstanding_pr' when zpm01.status_pengadaan = 'inquiry_harga' then 'inquiry_harga' when (zpm01.status_pengadaan = 'hps_oe' or zpm01.status_pengadaan = 'bidder_list' or zpm01.status_pengadaan = 'penilaian_kualifikasi' or zpm01.status_pengadaan = 'rfq') then 'hps_oe' when (zpm01.status_pengadaan = 'pemasukan_penawaran' or zpm01.status_pengadaan = 'pembukaan_penawaran' or zpm01.status_pengadaan = 'evaluasi_penawaran' or zpm01.status_pengadaan = 'klarifikasi_spesifikasi' or zpm01.status_pengadaan = 'evaluasi_teknis' or zpm01.status_pengadaan = 'evaluasi_tkdn' or zpm01.status_pengadaan = 'negosiasi'  or zpm01.status_pengadaan = 'lhp') then 'proses_tender' when (zpm01.status_pengadaan = 'pengumuman_pemenang' or zpm01.status_pengadaan = 'penunjuk_pemenang' or zpm01.status_pengadaan = 'purchase_order') then 'Penetapan Pemenang' else 'outstanding_pr' end) "+
                    "when (zpm01.pr is not null or zpm01.pr != '') and (p.po is not null or p.po != '') then (case when p.dci is null or p.dci = '' then 'tunggu_onsite' else 'onsite' end) end) end) as status_  "+
                    "FROM Mystap.dbo.zpm01 left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order left join Mystap.dbo.purch_order as p on p.material = zpm01.material AND p.pr = zpm01.pr AND p.item_pr = zpm01.itm_pr where work_order.revision ='"+ rev +"' and ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) GROUP BY [work_order].[order],zpm01.itm, zpm01.pr,zpm01.reqmt_qty, zpm01.qty_res, po, zpm01.status_pengadaan, dci) procurement_  "+
                    "join Mystap.dbo.joblist_detail_wo on joblist_detail_wo.[order] = procurement_.[order] join Mystap.dbo.joblist_detail on joblist_detail.id = joblist_detail_wo.jobListDetailID where joblist_detail.id = a.id GROUP by joblist_detail.material ) = 'ready' then 'ready' ELSE 'not_ready' END) ELSE 'not_identify' END) as sts_material "+
                    "FROM Mystap.dbo.joblist_detail a ) b "+
                    "LEFT JOIN Mystap.dbo.joblist on joblist.id = b.joblist_id  "+
                    "LEFT JOIN Mystap.dbo.project on project.id = joblist.projectID "+
                    "LEFT JOIN Mystap.dbo.contracttracking on contracttracking.idPaket = b.no_jasa "+
                    "LEFT JOIN Mystap.dbo.equipments on equipments.id = joblist.id_eqTagNo "+
                    "LEFT JOIN Mystap.dbo.joblist_detail_memo on joblist_detail_memo.jobListDetailID = b.id "+
                    "LEFT join Mystap.dbo.joblist_detail_wo on joblist_detail_wo.jobListDetailID = b.id "+
                    "LEFT JOIN Mystap.dbo.bom on bom.no_wo = joblist_detail_wo.[order] "+
                    "LEFT JOIN Mystap.dbo.bom_files on bom_files.id_bom = bom.id "+
                    "LEFT JOIN Mystap.dbo.request on request.id = joblist_detail_memo.id_memo "+
                    "LEFT JOIN Mystap.dbo.users on users.id = b.pic "+
                    "LEFT JOIN Mystap.dbo.notifikasi on notifikasi.id = b.notif "+
                    "where joblist.projectID = "+project+" and b.deleted = 0 "+
                    v +
                    "group by b.id";

                var c = FormattableStringFactory.Create(query);
                var customerData = _context.viewPlanningJoblist.FromSql(c);

                if (!(string.IsNullOrEmpty(sortColumn) && (string.IsNullOrEmpty(sortColumnDirection) || sortColumnDirection != "-1")))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.jobDesc.Contains(searchValue) || b.eqTagNo.Contains(searchValue) || b.alias.Contains(searchValue));
                }

                // Total number of rows count
                //Console.WriteLine(customerData);
                recordsTotal = customerData.Count();
                if(pageSize > 0)
                {
                    // Paging
                    customerData = customerData.Skip(skip).Take(pageSize);
                }
                
                var datas = await customerData.ToListAsync();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = datas });

            }
            catch (Exception)
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult UpdateJobplan()
        {
            try
            {
                int id_ = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Joblist_Detail job = _context.joblist_Detail.Where(p => p.id == id_).FirstOrDefault();

                if(job != null)
                {
                    job.pic = Convert.ToInt64(Request.Form["pic"].FirstOrDefault());
                    job.jobDesc = Request.Form["jobDesc"].FirstOrDefault();
                    job.alasan = Request.Form["alasan"].FirstOrDefault();
                    job.no_jasa = (Request.Form["no_jasa"].FirstOrDefault() != "") ? Convert.ToInt32(Request.Form["no_jasa"].FirstOrDefault()) : null;
                    job.order_jasa = Request.Form["order_jasa"].FirstOrDefault();
                    job.pekerjaan = Request.Form["pekerjaan"].FirstOrDefault();
                    job.ket_status_material = Request.Form["ket_status_material"].FirstOrDefault();
                    job.jasa = (Request.Form["jasa"].FirstOrDefault() == null) ? 0 : 1;
                    job.all_in_kontrak = (Request.Form["all_in_kontrak"].FirstOrDefault() == null) ? 0 : 1;
                    job.material = (Request.Form["material"].FirstOrDefault() == null) ? 0 : 1;
                    job.lldi = (Request.Form["lldi"].FirstOrDefault() == null) ? 0 : 1;
                    job.lldi = (Request.Form["lldi"].FirstOrDefault() == null) ? 0 : 1;
                    job.status_jasa = "";
                    job.status_material = "";

                    if (job.jasa != 0)
                    {
                        job.status_jasa = "NOT_PLANNED";
                        if (Request.Form["no_jasa"].FirstOrDefault() != "")
                        {
                            var cek = _context.contractTracking.Where(p => p.aktualCO != null).Where(p => p.idPaket == Convert.ToInt64(Request.Form["no_jasa"].FirstOrDefault())).FirstOrDefault();
                            if (cek != null)
                            {
                                job.status_jasa = "COMPLETED";
                            }
                            else
                            {
                                job.status_jasa = "NOT_COMPLETED";
                            }
                        }
                    }

                    if (job.material != 0)
                    {
                        job.status_material = "NOT_PLANNED";
                        if (Request.Form["status_material"].FirstOrDefault() != "")
                        {
                            job.status_material = Request.Form["status_material"].FirstOrDefault();
                        }

                    }

                    if (job.jasa != 0 || job.all_in_kontrak != 0 || job.material != 0)
                    {
                        if (job.status_material != "")
                        {
                            job.status_job = job.status_material;
                        }

                        if (job.status_jasa != "")
                        {
                            job.status_job = job.status_jasa;
                        }

                        if (job.jasa != 0 || job.material != 0)
                        {
                            if (job.status_material == "COMPLETED" && job.status_jasa != "COMPLETED")
                            {
                                job.status_job = "NOT_COMPLETED";
                            }

                            if (job.status_jasa == "COMPLETED" && job.status_material != "COMPLETED")
                            {
                                job.status_job = "NOT_COMPLETED";
                            }

                            if (job.status_jasa != "COMPLETED" && job.status_material != "COMPLETED")
                            {
                                job.status_job = "NOT_COMPLETED";
                            }

                            if (job.status_jasa == "NOT_PLANNED" && job.status_material == "NOT_PLANNED")
                            {
                                job.status_job = "NOT_COMPLETED";
                            }

                            if (job.status_jasa == "COMPLETED" && job.status_material == "COMPLETED")
                            {
                                job.status_job = "COMPLETED";
                            }
                        }
                    }
                    else
                    {
                        job.status_job = "NOT_IDENTIFY";
                    }


                    Boolean t;

                    using var transaction = _context.Database.BeginTransaction();
                    try
                    {                    
                        _context.SaveChanges();

                        var wo = Request.Form["no_order[]"];
                        if (!wo.IsNullOrEmpty())
                        {
                            var cek = _context.joblistDetailWo.Where(p => p.JobListDetailID == Convert.ToInt64(id_)).ToList();
                            if (!cek.IsNullOrEmpty())
                            {
                                _context.joblistDetailWo.Where(p => p.JobListDetailID == Convert.ToInt64(id_)).ExecuteDelete();
                            }
                            foreach(var val in wo)
                            {
                                JoblistDetailWo jobwo = new JoblistDetailWo();
                                jobwo.JobListDetailID = id_;
                                jobwo.order = val;

                           
                                _context.joblistDetailWo.Add(jobwo);
                                _context.SaveChanges();
                            }
                        }

                        transaction.Commit();
                        t = true;

                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        t = false;
                    }
                    return Json(new {result = t});

                }

                return Json(new { result = false });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult GetPaketKontrak()
        {
            try
            {
                var id_project = Request.Form["id"].FirstOrDefault();
                var data = _context.contractTracking.Where(p => p.projectID == Convert.ToInt32(id_project)).Where(p => p.deleted == 0).ToList();
                var isi = "<option value=''>- Pilih Paket Jasa -</option>";
                foreach(var val in data)
                {
                    isi += "<option value='" + val.idPaket + "' data-no_paket='" + val.noPaket + "' data-desc='" + val.judulPekerjaan + "' data-order='" + val.WO + "' >" + val.noPaket + "-" + val.judulPekerjaan + "</option>";
                }
                return Ok(isi);
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult GetJoblistWo()
        {
            try
            {
                var id = Request.Form["id"].FirstOrDefault();
                var data = _context.joblistDetailWo.Where(p => p.JobListDetailID == Convert.ToInt32(id)).ToList();

                var isi = "";
                foreach (var val in data)
                {
                    isi += "<div class='row row-cols-lg-auto g-3 align-items-center mb-2'><div class='col-12'><div class='form-group'><input type='number' id='no_order' name='no_order[]' class='form-control form-control-sm' value='"+val.order+"'></div></div><a class='btn btn-sm btn-danger remove'>-</a></div>";
                }

                return Json(new { isi = isi, total = data.Count() });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult EksekusiJoblist()
        {
            ViewBag.role = "EKSEKUSI_JOBLIST";
            if (Module.hasModule("EKSEKUSI_JOBLIST", HttpContext.Session))
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
		public async Task<IActionResult> PaketEksekusi()
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

                var eqtagno = Request.Form["eqTagNo"].FirstOrDefault();
                var disiplin = Request.Form["disiplin"].FirstOrDefault();
                var status_eksekusi = Request.Form["status_eksekusi"].FirstOrDefault();
                var table = Request.Form["table"].FirstOrDefault();

                var data = (from jd in _context.joblist_Detail
                            join j in _context.joblist on jd.joblist_id equals j.id
                            join e in _context.equipments on j.id_eqTagNo equals e.id
                            join u in _context.users on jd.pic equals u.id
                            select new
                            {
                                d = jd,
                                eqTagNo = e.eqTagNo,
                                id_project = j.projectID,
                                alias = u.alias
                            }).Where(p => p.id_project == Convert.ToInt32(Request.Form["project"].FirstOrDefault())).Where(p => p.d.deleted == 0);

                if (!string.IsNullOrEmpty(eqtagno))
                {
                    data = data.Where(p => p.eqTagNo.StartsWith(eqtagno));
                }

                if (!string.IsNullOrEmpty(disiplin))
                {
                    data = data.Where(p => p.d.disiplin == disiplin);
                }

                if (!string.IsNullOrEmpty(status_eksekusi))
                {
                    data = data.Where(p => p.d.dikerjakan == status_eksekusi);
                }

                if(table == "paket")
                {
                    data = data.Where(p => p.d.id_paket == null);
                }


                if (!(string.IsNullOrEmpty(sortColumn) && (string.IsNullOrEmpty(sortColumnDirection) || sortColumnDirection != "-1")))
                {
                    data = data.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    data = data.Where(b => b.d.jobDesc.StartsWith(searchValue));
                }


                // Total number of rows count
                //Console.WriteLine(customerData);
                recordsTotal = data.Count();

                if (table != "paket")
                {
                    if (pageSize > 0)
                    {
                        // Paging
                        data = data.Skip(skip).Take(pageSize);
                    }
                }
                // Paging
                var datas = await data.ToListAsync();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = datas });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult StatusEksekusi()
        {
            try
            {
                string id = Request.Form["id"];
                string[] id_ = id.Split(",");

                var status_ = Request.Form["status_"].FirstOrDefault();
                var dikerjakan = Request.Form["dikerjakan"].FirstOrDefault();
                var keterangan = Request.Form["keterangan"].FirstOrDefault();
                var mitigasi = Request.Form["mitigasi"].FirstOrDefault();

                
                foreach(var val in id_)
                {
                    Joblist_Detail job = _context.joblist_Detail.Where(p => p.id == Convert.ToInt64(val)).FirstOrDefault();
                    if (job != null)
                    {
                        if (status_ == "dikerjakan") {
                            job.dikerjakan = "YA";
                            job.keterangan = keterangan;
                            job.mitigasi = null;
                        } else
                        {
                            job.dikerjakan = "TIDAK";
                            job.keterangan = keterangan;
                            job.mitigasi = mitigasi;
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
		public IActionResult CountEksekusi()
        {
            try
            {

                var project_filter = Request.Form["project_filter"].FirstOrDefault();
                var disiplin_filter = Request.Form["disiplin_filter"].FirstOrDefault();

                var w = "";

                if (!string.IsNullOrEmpty(project_filter))
                {
                    w += " and joblist.projectID = " + project_filter;
                }

                if (!string.IsNullOrEmpty(disiplin_filter))
                {
                    w += " and joblist_detail.disiplin = '" + disiplin_filter + "' ";
                }

                var query = $"select count(DISTINCT (case when joblist_detail.dikerjakan = 'YA' then joblist_detail.id else null end)) as di_kerjakan, " +
                    "count(DISTINCT (case when joblist_detail.dikerjakan = 'TIDAK' then joblist_detail.id else null end)) as tidak_dikerjakan " +
                    "from Mystap.dbo.joblist_detail " +
                    "left join Mystap.dbo.joblist on joblist.id = joblist_detail.joblist_id " +
                    "where joblist_detail.deleted = 0 " + w;

                var c = FormattableStringFactory.Create(query);
                var data = _context.viewCountEksekusi.FromSql(c).FirstOrDefault();

                return Json(new { result = data });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult PaketJoblist()
        {
            ViewBag.role = "PAKET_JOBLIST";
            if (Module.hasModule("PAKET_JOBLIST", HttpContext.Session))
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
		public async Task<IActionResult> PaketJoblist_()
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

                var project = Convert.ToInt32(Request.Form["project_filter"].FirstOrDefault());
                var project_rev = Request.Form["project_rev"].FirstOrDefault();
                var disiplin = Request.Form["disiplin"].FirstOrDefault();
                var tag_no = Request.Form["tag_no"].FirstOrDefault();

                var w = "";

                if (!string.IsNullOrEmpty(disiplin))
                {
                    w += " and paket_joblist.disiplin = '" + disiplin + "' ";
                }

                if (!string.IsNullOrEmpty(tag_no))
                {
                    w += " and paket_joblist.tag_no like '%" + tag_no + "%' ";
                }

                var query = @"select " +
                        "max(paket_joblist.id_paket) as id_paket, max(paket_joblist.tag_no) as tag_no, max(paket_joblist.no_paket) as no_paket, max(paket_joblist.no_memo) as no_memo, max(paket_joblist.disiplin) as disiplin, CONCAT((case when max(paket_joblist.additional) is not null then 'add - ' else '' end), max(paket_joblist.no_paket)) as no_add, max(project.revision) as project_rev, max(project.id) as id_project,count(c.id) as total, " +
                        "CASE WHEN count(case when  c.status = 'not_ready' then c.id else NULL end) > '0' THEN 'not_ready' WHEN max(c.status) is null then 'not_ready' ELSE 'ready' END AS 'status_tagno' " +
                        "FROM Mystap.dbo.paket_joblist " +
                        "LEFT JOIN (SELECT b.id,b.id_paket, b.joblist_id,(CASE WHEN b.dikerjakan = 'tidak' then 'ready' WHEN b.jasa != '0' AND b.material != '0' THEN CASE WHEN b.sts_material = 'ready' AND b.sts_kontrak = 'ready' THEN 'ready'  ELSE 'not_ready' END WHEN b.material != '0' THEN isnull(b.sts_material, 'not_ready') WHEN b.jasa != '0' THEN isnull(b.sts_kontrak, 'not_ready') END) as status " +
                        "FROM (SELECT a.id, a.joblist_id, a.id_paket,a.jobDesc, a.jasa, a.material, a.dikerjakan, " +
                        "(CASE WHEN a.dikerjakan = 'tidak' then 'not_execute' WHEN a.jasa != '0' THEN  (SELECT (CASE WHEN contracttracking.aktualSP is null THEN 'not_ready' ELSE 'ready' END) as status FROM Mystap.dbo.contracttracking left join Mystap.dbo.joblist_detail on joblist_detail.no_jasa = contracttracking.idPaket where contracttracking.projectID = " + project + " and joblist_detail.id = a.id and contracttracking.deleted != '1') ELSE 'not_identify' END) as sts_kontrak, " +
                        "(CASE WHEN a.dikerjakan = 'tidak' then 'not_execute' WHEN a.material != '0' THEN (CASE WHEN a.status_material = 'COMPLETED' AND (SELECT (CASE WHEN max(joblist_detail.material) != '0' THEN CASE WHEN count(DISTINCT case when procurement_.status_ != 'terpenuhi_stock' AND procurement_.status_ != 'onsite' then procurement_.[order] else NULL end) > '0' THEN 'not_ready' ELSE 'ready' END ELSE NULL END) as sts_material " +
                        "FROM (SELECT work_order.[order], (case when max(zpm01.pr) is null or max(zpm01.pr) = '' then (case when max(zpm01.reqmt_qty) = max(zpm01.qty_res) then 'terpenuhi_stock' else 'create_pr' end) else (case when (max(zpm01.pr) is not null or max(zpm01.pr) != '') and (max(p.po) is not null or max(p.po) != '') then (case when max(p.dci) is null or max(p.dci) = '' then 'tunggu_onsite' else 'onsite'  end) when (max(zpm01.status_pengadaan) is not null or max(zpm01.status_pengadaan) != '') then max(zpm01.status_pengadaan) else 'outstanding_pr' end) end) as status_ " +
                        "FROM Mystap.dbo.zpm01 left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order left join Mystap.dbo.purch_order as p on p.material = zpm01.material AND p.pr = zpm01.pr AND p.item_pr = zpm01.itm_pr " +
                        "where work_order.revision = '" + project_rev + "' and ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) GROUP BY [work_order].[order],zpm01.itm) procurement_ " +
                        "left join Mystap.dbo.joblist_detail_wo on joblist_detail_wo.[order] = procurement_.[order] " +
                        "left join Mystap.dbo.joblist_detail on joblist_detail.id = joblist_detail_wo.jobListDetailID " +
                        "left join Mystap.dbo.joblist on joblist.id = joblist_detail.joblist_id " +
                        "where joblist.projectID = " + project + " and joblist_detail.id = a.id ) = 'ready' then 'ready' ELSE 'not_ready' END) ELSE 'not_identify' END) as sts_material FROM Mystap.dbo.joblist_detail a LEFT JOIN Mystap.dbo.paket_joblist on paket_joblist.id_paket = a.id_paket left join Mystap.dbo.project on project.id = paket_joblist.projectID " +
                        "where project.id = " + project + " and a.deleted != '1') b) c on c.id_paket = paket_joblist.id_paket " +
                        "LEFT JOIN Mystap.dbo.project on project.id = paket_joblist.projectID where project.id = " + project + " " + w + "group by paket_joblist.id_paket having count(c.id) > 0 ";

                var c = FormattableStringFactory.Create(query);
                var data = _context.viewPaketJoblist.FromSql(c);

                if (!(string.IsNullOrEmpty(sortColumn) && (string.IsNullOrEmpty(sortColumnDirection) || sortColumnDirection != "-1")))
                {
                    data = data.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    data = data.Where(b => b.tag_no.StartsWith(searchValue));
                }


                // Total number of rows count
                //Console.WriteLine(customerData);
                recordsTotal = data.Count();
                // Paging
                if (pageSize > 0)
                {
                    data = data.Skip(skip).Take(pageSize);
                }
                var datas = await data.ToListAsync();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = datas });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult CreatePaketJoblist()
        {
            ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == 1).ToList();
            return View();
        }

		[AuthorizedAction]
		public IActionResult CreatePaketJoblist_()
        {
            try
            {
                string id = Request.Form["id"];
                string[] id_ = id.Split(",");

                var no_paket = Convert.ToInt64(Request.Form["no_paket"].FirstOrDefault());
                var tag_no = Request.Form["tag_no"].FirstOrDefault();
                var no_memo = Request.Form["no_memo"].FirstOrDefault();
                var disiplin = Request.Form["disiplin_paket"].FirstOrDefault();
                var additional = Convert.ToInt32(Request.Form["additional"].FirstOrDefault());
                var id_project = Convert.ToInt64(Request.Form["id_project"].FirstOrDefault());

                PaketJoblist job = new PaketJoblist();
                job.no_paket = no_paket;
                job.tag_no = tag_no;
                job.no_memo = no_memo;
                job.disiplin = disiplin;
                job.additional = additional;
                job.projectID = id_project;

                Boolean t;
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    _context.paketJoblist.Add(job);
                    _context.SaveChanges();

                    if (!id_.IsNullOrEmpty())
                    {
                        foreach (var val in id_)
                        {
                            Joblist_Detail v = _context.joblist_Detail.Where(p => p.id == Convert.ToInt64(val)).FirstOrDefault();
                            if (v != null)
                            {
                                v.id_paket = job.id_paket;
                                _context.SaveChanges();
                            }
                        }
                    }

                    transaction.Commit();
                    t = true;

                }
                catch (Exception)
                {
                    transaction.Rollback();
                    t = false;
                }
                return Json(new { result = t });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult UpdatePaketJoblist(long id)
        {
            ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == 1).ToList();
            ViewBag.paket_joblist = _context.paketJoblist.Where(p => p.id_paket == id).FirstOrDefault();

            return View();
        }

		[AuthorizedAction]
		public async Task<IActionResult> ListSelected()
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

                var eqtagno = Request.Form["eqTagNo"].FirstOrDefault();
                var disiplin = Request.Form["disiplin"].FirstOrDefault();
                var id_paket = Request.Form["id"].FirstOrDefault();

                var data = (from jd in _context.joblist_Detail
                            join j in _context.joblist on jd.joblist_id equals j.id
                            join e in _context.equipments on j.id_eqTagNo equals e.id
                            join u in _context.users on jd.pic equals u.id
                            select new
                            {
                                d = jd,
                                eqTagNo = e.eqTagNo,
                                id_project = j.projectID,
                                alias = u.alias
                            }).Where(p => p.id_project == Convert.ToInt32(Request.Form["project"].FirstOrDefault())).Where(p => p.d.deleted == 0);
                data = data.Where(p => p.d.id_paket == null || p.d.id_paket == Convert.ToInt32(id_paket));
                if (!string.IsNullOrEmpty(eqtagno))
                {
                    data = data.Where(p => p.eqTagNo.StartsWith(eqtagno));
                }

                if (!string.IsNullOrEmpty(disiplin))
                {
                    data = data.Where(p => p.d.disiplin == disiplin);
                }

                

                if (!(string.IsNullOrEmpty(sortColumn) && (string.IsNullOrEmpty(sortColumnDirection) || sortColumnDirection != "-1")))
                {
                    data = data.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    data = data.Where(b => b.d.jobDesc.StartsWith(searchValue));
                }


                // Total number of rows count
                //Console.WriteLine(customerData);
                recordsTotal = data.Count();
               
                // Paging
                var datas = await data.ToListAsync();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = datas });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult UpdatePaketJoblist_()
        {
            try
            {
                string id = Request.Form["id"];
                string[] id_ = id.Split(",");

                var no_paket = Convert.ToInt64(Request.Form["no_paket"].FirstOrDefault());
                var tag_no = Request.Form["tag_no"].FirstOrDefault();
                var no_memo = Request.Form["no_memo"].FirstOrDefault();
                var disiplin = Request.Form["disiplin_paket"].FirstOrDefault();
                var additional = Convert.ToInt32(Request.Form["additional"].FirstOrDefault());
                var id_project = Convert.ToInt64(Request.Form["id_project"].FirstOrDefault());

                

                Boolean t;
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    PaketJoblist job = _context.paketJoblist.Where(p => p.id_paket == Convert.ToInt64(Request.Form["id_paket"].FirstOrDefault())).FirstOrDefault();
                    if(job != null)
                    {
                        job.no_paket = no_paket;
                        job.tag_no = tag_no;
                        job.no_memo = no_memo;
                        job.disiplin = disiplin;
                        job.additional = additional;
                        job.projectID = id_project;
                        _context.SaveChanges();
                    }

                    if (!id_.IsNullOrEmpty())
                    {
                        Joblist_Detail c = _context.joblist_Detail.Where(p => p.id_paket == Convert.ToInt64(Request.Form["id_paket"].FirstOrDefault())).FirstOrDefault();
                        if (c != null)
                        {
                            c.id_paket = null;
                            _context.SaveChanges();
                        }
                        foreach (var val in id_)
                        {
                            Joblist_Detail v = _context.joblist_Detail.Where(p => p.id == Convert.ToInt64(val)).FirstOrDefault();
                            if (v != null)
                            {
                                v.id_paket = Convert.ToInt64(Request.Form["id_paket"].FirstOrDefault());
                                _context.SaveChanges();
                            }
                        }
                    }

                    transaction.Commit();
                    t = true;

                }
                catch (Exception)
                {
                    transaction.Rollback();
                    t = false;
                }
                return Json(new { result = t });
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult DeletePaketJoblist()
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    _context.paketJoblist.Where(p => p.id_paket == id).ExecuteDelete();
                    Joblist_Detail obj = _context.joblist_Detail.Where(p => p.id_paket == id).FirstOrDefault();

                    if (obj != null)
                    {
                        obj.id_paket = null;
                        _context.SaveChanges();
                    }

                    transaction.Commit();
                    return Json(new { title = "Sukses!", icon = "success", status = "Berhasil Dihapus" });
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return Json(new { title = "Maaf!", icon = "error", status = "Tidak Dapat di Hapus!, Silahkan Hubungi Administrator " });
                }
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public async Task<IActionResult> GetDetailPaket()
        {
            try
            {
                var isi = "<div style='background-color: lightblue;padding: 10px'><h5>Detail Paket JobList :</h5><table class='table table-bordered  table-sm'>";
                isi += "<tr class='text-center'><th>TagNo</th>" +
                    "<th width = '10%'> Desc </th> "+
                    "<th> PIC </th> "+
                    "<th> Status </th> "+
                    "<th> Jasa </th>"+
                    "<th> Status Jasa </th> "+
                    "<th> Material </th> "+
                    "<th> Status Material </th> "+
                    "<th> All in</th> "+
                    "<th> LLDI </th> "+
                    "<th> Status Job </th> "+
                    "<th> Status Ready </th> "+
                    "<th> Dis </th></tr>";

                var project = Convert.ToInt32(Request.Form["id_project"].FirstOrDefault());
                var rev = Request.Form["project_rev"].FirstOrDefault();
                var id_paket = Convert.ToInt64(Request.Form["id_paket"].FirstOrDefault());

                var query = @"SELECT b.id, max(equipments.eqTagNo) as eqTagNo, max(users.alias) as alias,max(b.jobDesc) as jobDesc, max(b.alasan) as alasan, max(b.all_in_kontrak) as all_in_kontrak, max(b.cleaning) as cleaning, max(b.coc) as coc, max(b.critical_job) as critical_job, max(b.deleted) as deleted, max(b.dikerjakan) as dikerjakan, max(b.drawing) as drawing, max(b.energi) as energi, max(b.engineer) as engineer, max(b.execution) as execution, max(b.freezing) as freezing, max(b.hsse) as hsse, max(b.id_paket) as id_paket,max(b.inspection) as inspection, max(b.jasa) as jasa, max(b.jobDetailNo) as jobDetailNo, max(joblist.jobNo) as jobNo, max(joblist.id) as joblist_id, max(b.ket_status_material) as ket_status_material, max(b.keterangan) as keterangan, max(b.lldi) as lldi, max(b.losses) as losses, max(b.material) as material, max(b.measurement) as measurement, max(b.mitigasi) as mitigasi, max(b.modif) as modif, max(b.ndt) as ndt,max(b.no_jasa) as no_jasa, max(b.no_order) as no_order, max(notifikasi.id) as notif,max(notifikasi.notifikasi) as no_notif ,max(notifikasi.rekomendasi) as link_rekomendasi,max(b.order_jasa) as order_jasa, max(b.pekerjaan) as pekerjaan, max(b.pic) as pic, max(b.project) as project, max(b.ram) as ram, max(b.reliability) as reliability, max(b.repair) as repair, max(b.replace) as replace, max(b.responsibility) as responsibility, max(b.revision) as revision, max(b.status_jasa) as status_jasa, max(b.status_material) as status_material, max(b.tein) as tein, max(b.disiplin) as disiplin, max(b.status_job) as status_job, " +
                    "max(project.id) as id_project, string_agg(joblist_detail_wo.[order], ',') as wo, string_agg(request.reqNo, ',') as no_memo, string_agg(request.attach, ',') as file_memo, max(contracttracking.noPaket) as noPaket, max(contracttracking.WO) as wo_jasa, max(contracttracking.judulPekerjaan) as judul_paket, max(contracttracking.po) as no_po, max(contracttracking.noSP) as no_sp, string_agg(bom_files.files, ';') as file_bom, " +
                    "(CASE WHEN max(b.dikerjakan) = 'tidak' then 'ready' WHEN max(b.jasa) != '0' AND max(b.material) != '0' THEN CASE WHEN max(b.sts_material) = 'ready' AND max(b.sts_kontrak) = 'ready' THEN 'ready'  ELSE 'not_ready' END WHEN max(b.material) != '0' THEN isnull(max(b.sts_material), 'not_ready') WHEN max(b.jasa) != '0' THEN isnull(max(b.sts_kontrak), 'not_ready') END) as status " +
                    "FROM (SELECT a.*, (CASE WHEN a.dikerjakan = 'tidak' then 'not_execute' WHEN a.jasa != '0' THEN  (SELECT (CASE WHEN contracttracking.aktualSP is null THEN 'not_ready' ELSE 'ready' END) as status FROM Mystap.dbo.contracttracking left join Mystap.dbo.joblist_detail on joblist_detail.no_jasa = contracttracking.idPaket where contracttracking.projectID =" + project + " and joblist_detail.id = a.id and contracttracking.deleted != '1') ELSE 'not_identify' END) as sts_kontrak, " +
                    "(CASE WHEN a.dikerjakan = 'tidak' then 'not_execute' WHEN a.material != '0' THEN (CASE WHEN a.status_material = 'COMPLETED' AND (SELECT (CASE WHEN joblist_detail.material != '0' THEN CASE WHEN count(DISTINCT case when procurement_.status_ != 'terpenuhi_stock' AND procurement_.status_ != 'onsite' then procurement_.[order] else NULL end) > '0' THEN 'not_ready' ELSE 'ready' END ELSE NULL END) as sts_material  " +
                    "FROM (SELECT work_order.[order], (case when zpm01.pr is null or zpm01.pr = '' then (case when zpm01.reqmt_qty = zpm01.qty_res then 'terpenuhi_stock' else 'create_pr' end) else (case when (zpm01.pr is not null or zpm01.pr != '') and (p.po is null or p.po = '')then (case when (zpm01.status_pengadaan = 'tunggu_pr' or zpm01.status_pengadaan = 'evaluasi_dp3' or zpm01.status_pengadaan is null) " +
                    "then 'outstanding_pr' when zpm01.status_pengadaan = 'inquiry_harga' then 'inquiry_harga' when (zpm01.status_pengadaan = 'hps_oe' or zpm01.status_pengadaan = 'bidder_list' or zpm01.status_pengadaan = 'penilaian_kualifikasi' or zpm01.status_pengadaan = 'rfq') then 'hps_oe' when (zpm01.status_pengadaan = 'pemasukan_penawaran' or zpm01.status_pengadaan = 'pembukaan_penawaran' or zpm01.status_pengadaan = 'evaluasi_penawaran' or zpm01.status_pengadaan = 'klarifikasi_spesifikasi' or zpm01.status_pengadaan = 'evaluasi_teknis' or zpm01.status_pengadaan = 'evaluasi_tkdn' or zpm01.status_pengadaan = 'negosiasi'  or zpm01.status_pengadaan = 'lhp') then 'proses_tender' when (zpm01.status_pengadaan = 'pengumuman_pemenang' or zpm01.status_pengadaan = 'penunjuk_pemenang' or zpm01.status_pengadaan = 'purchase_order') then 'Penetapan Pemenang' else 'outstanding_pr' end) " +
                    "when (zpm01.pr is not null or zpm01.pr != '') and (p.po is not null or p.po != '') then (case when p.dci is null or p.dci = '' then 'tunggu_onsite' else 'onsite' end) end) end) as status_  " +
                    "FROM Mystap.dbo.zpm01 left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order left join Mystap.dbo.purch_order as p on p.material = zpm01.material AND p.pr = zpm01.pr AND p.item_pr = zpm01.itm_pr where work_order.revision ='" + rev + "' and ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) GROUP BY [work_order].[order],zpm01.itm, zpm01.pr,zpm01.reqmt_qty, zpm01.qty_res, po, zpm01.status_pengadaan, dci) procurement_  " +
                    "join Mystap.dbo.joblist_detail_wo on joblist_detail_wo.[order] = procurement_.[order] join Mystap.dbo.joblist_detail on joblist_detail.id = joblist_detail_wo.jobListDetailID where joblist_detail.id = a.id GROUP by joblist_detail.material ) = 'ready' then 'ready' ELSE 'not_ready' END) ELSE 'not_identify' END) as sts_material " +
                    "FROM Mystap.dbo.joblist_detail a ) b " +
                    "LEFT JOIN Mystap.dbo.joblist on joblist.id = b.joblist_id  " +
                    "LEFT JOIN Mystap.dbo.project on project.id = joblist.projectID " +
                    "LEFT JOIN Mystap.dbo.contracttracking on contracttracking.idPaket = b.no_jasa " +
                    "LEFT JOIN Mystap.dbo.equipments on equipments.id = joblist.id_eqTagNo " +
                    "LEFT JOIN Mystap.dbo.joblist_detail_memo on joblist_detail_memo.jobListDetailID = b.id " +
                    "LEFT join Mystap.dbo.joblist_detail_wo on joblist_detail_wo.jobListDetailID = b.id " +
                    "LEFT JOIN Mystap.dbo.bom on bom.no_wo = joblist_detail_wo.[order] " +
                    "LEFT JOIN Mystap.dbo.bom_files on bom_files.id_bom = bom.id " +
                    "LEFT JOIN Mystap.dbo.request on request.id = joblist_detail_memo.id_memo " +
                    "LEFT JOIN Mystap.dbo.users on users.id = b.pic " +
                    "LEFT JOIN Mystap.dbo.notifikasi on notifikasi.id = b.notif " +
                    "where joblist.projectID = " + project + " and b.deleted = 0 and b.id_paket = "+id_paket+" " +
                    "group by b.id";

                var c = FormattableStringFactory.Create(query);
                var data = await _context.viewPlanningJoblist.FromSql(c).ToListAsync();

                if(data.Count() > 0)
                {
                    foreach(var val in data)
                    {
                        var p = (val.project == 1) ? "<span class='badge bg-primary'>Project</span>" : "";
                        var cp = (val.critical_job == 1) ? "<span class='badge bg-danger'>Critical Job</span>" : "";
                        var f = (val.freezing == 1) ? "<span class='badge bg-info'>Freezing</span>" : "";
                        var status = p + " " + cp + " " + f;

                        var sj = "";
                        if (val.status_jasa != null)
                        {
                            if (val.status_jasa == "COMPLETED")
                            {
                                sj = "<span class='badge bg-success shadow-none'>Completed</span>";
                            }
                            else if (val.status_jasa == "NOT_COMPLETED")
                            {
                                sj = "<span class='badge bg-warning shadow-none'>Not Completed</span>";
                            }
                            else if (val.status_jasa == "NOT_PLANNED")
                            {
                                sj = "<span class='badge bg-dark shadow-none'>Not Planned</span>";
                            }
                            else
                            {
                                sj = "<span class='badge bg-danger shadow-none'>Not Identify</span>";
                            }
                        }

                        var sm = "";
                        if (val.status_material != null)
                        {
                            if (val.status_material == "COMPLETED")
                            {
                                sm = "<span class='badge bg-success shadow-none'>Completed</span>";
                            }
                            else if (val.status_material == "NOT_COMPLETED")
                            {
                                sm = "<span class='badge bg-warning shadow-none'>Not Completed</span>";
                            }
                            else if (val.status_material == "NOT_PLANNED")
                            {
                                sm = "<span class='badge bg-dark shadow-none'>Not Planned</span>";
                            }
                            else
                            {
                                sm = "<span class='badge bg-danger shadow-none'>Not Identify</span>";
                            }
                        }

                        var sjob = "";
                        if (val.status_job == "COMPLETED")
                        {
                            sjob = "<span class='badge bg-success shadow-none'>Completed</span>";
                        }
                        else if (val.status_job == "NOT_COMPLETED")
                        {
                            sjob = "<span class='badge bg-warning shadow-none'>Not Completed</span>";
                        }
                        else if (val.status_job == "NOT_PLANNED")
                        {
                            sjob = "<span class='badge bg-dark shadow-none'>Not Planned</span>";
                        }
                        else
                        {
                            sjob = "<span class='badge bg-danger shadow-none'>Not Identify</span>";
                        }

                        var s = "";
                        if (val.status == "ready")
                        {
                            s = "<span class='badge bg-success'>Ready</span>";
                        }
                        else if (val.status == "not_ready")
                        {
                            s = "<span class='badge bg-danger'>Not Ready</span>";
                        }
                        else if (val.status == "not_identify")
                        {
                            s = "N/R";
                        }
                        else if (val.status == "not_execute")
                        {
                            s = "<span class='badge bg-black'>Not Execute</span>";
                        }
                        else
                        {
                            s = "<span class='badge bg-secondary'>Undefined</span>";
                        }

                        var jasa = (val.jasa == 1) ? "<i class='fas fa-check-square text-primary'></i>" : "-";
                        var material = (val.material == 1) ? "<i class='fas fa-check-square text-primary'></i>" : "-";
                        var all_in = (val.all_in_kontrak == 1) ? "<i class='fas fa-check-square text-primary'></i>" : "-";
                        var lldi = (val.lldi == 1) ? "<i class='fas fa-check-square text-primary'></i>" : "-";

                        isi += "<tr> "+
                                "<td>"+val.eqTagNo + " </td>"+
                                "<td>"+val.jobDesc +"<a href ='#' data-toggle = 'popover' title = 'Popover Header' data-content='Some content inside the popover'> ...</a></td> "+
                                "<td class='text-center'>"+ val.alias +"</td> "+
                                "<td class='text-center'>"+ status +"</td>"+
                                "<td class='text-center'>"+ jasa +"</td>"+
                                "<td class='text-center'>"+ sj +"</td>"+
                                "<td class='text-center'>"+ material +"</td>"+
                                "<td class='text-center'>"+ sm +"</td>" +
                                "<td class='text-center'>"+ all_in +"</td>"+
                                "<td class='text-center'>"+ lldi +"</td>"+
                                "<td class='text-center'>"+ sjob +"</td>"+
                                "<td class='text-center'>"+ s  +"</td>" +
                                "<td>"+ val.disiplin +"</td>"+
                            "</tr>";
                    }
                }
                else
                {
                    isi += "<tr><td colspan='13' class='text-center'>Data Tidak Ada</td></tr>";
                }

                isi += "</table></div>";
                return Json(new {data= isi});
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult Notifikasi()
        {
            ViewBag.role = "NOTIFIKASI";
            if (Module.hasModule("NOTIFIKASI", HttpContext.Session))
            {
               
                return View();
            }
            else
            {
                return NotFound();
            }
           
        }

		[AuthorizedAction]
		public async Task<IActionResult> Get_Notifikasi()

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
                var project = Request.Form["project"].FirstOrDefault();
                var project_rev = Request.Form["project_rev"].FirstOrDefault();
                var eqtagno = Request.Form["eqTagNo"].FirstOrDefault();
                var jobno = Request.Form["jobNo"].FirstOrDefault();
                var unitcode = Request.Form["unitCode"].FirstOrDefault();
                var user_section = Request.Form["user_section"].FirstOrDefault();
                // Getting all Customer data
                var customerData = (from c in _context.notifikasi
                                    select new
                                    {
                                        id = c.id,
                                        notification_type = c.notification_type,
                                        notifikasi = c.notifikasi,
                                        order = c.order,
                                        notification_date = c.notification_date,
                                        created_by = c.created_by,
                                        created_on = c.created_on,
                                        change_by = c.change_by,
                                        change_on = c.change_on,
                                        planner_group = c.planner_group,
                                        description = c.description,
                                        user_status = c.user_status,
                                        system_status = c.system_status,
                                        maintenance_plant = c.maintenance_plant,
                                        functional_location = c.functional_location,
                                        equipment = c.equipment,
                                        required_start = c.required_start,
                                        required_end = c.required_end,
                                        location = c.location,
                                        main_work_center = c.main_work_center,
                                        maintenance_item = c.maintenance_item,
                                        maintenance_plan = c.maintenance_plan,
                                        rekomendasi = c.rekomendasi

                                    });



                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.notifikasi.StartsWith(searchValue));
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
       
        IExcelDataReader reader;

		// GET: /<controller>/
		[AuthorizedAction]
		[HttpPost]
        public async Task<IActionResult> ImportNotif(IFormFile file)
        {
            try
            {
                // Check the File is received

                if (file == null)
                    throw new Exception("File is Not Received...");


                // Create the Directory if it is not exist
                string dirPath = Path.Combine(hostEnvironment.WebRootPath, "ReceivedReports");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                // MAke sure that only Excel file is used 
                string dataFileName = Path.GetFileName(file.FileName);

                string extension = Path.GetExtension(dataFileName);

                string[] allowedExtsnions = new string[] { ".xls", ".xlsx" };

                if (!allowedExtsnions.Contains(extension))
                    throw new Exception("Sorry! This file is not allowed, make sure that file having extension as either .xls or .xlsx is uploaded.");

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
                            Notifikasi obj = _context.notifikasi.Where(p => p.notifikasi == serviceDetails.Rows[i][1].ToString()).FirstOrDefault();
                            if (obj != null)
                            {
                                obj.notification_type = serviceDetails.Rows[i][0].ToString();
                                obj.notifikasi = serviceDetails.Rows[i][1].ToString();
                                obj.order = serviceDetails.Rows[i][2].ToString();
                                obj.notification_date = Convert.ToDateTime(serviceDetails.Rows[i][3].ToString());
                                obj.created_by = serviceDetails.Rows[i][4].ToString();
                                obj.created_on = Convert.ToDateTime(serviceDetails.Rows[i][5].ToString());
                                obj.change_by = serviceDetails.Rows[i][6].ToString();
                                obj.change_on = Convert.ToDateTime(serviceDetails.Rows[i][7].ToString());
                                obj.planner_group = serviceDetails.Rows[i][8].ToString();
                                obj.description = serviceDetails.Rows[i][9].ToString();
                                obj.user_status = Convert.ToInt32(serviceDetails.Rows[i][10].ToString());
                                obj.system_status = serviceDetails.Rows[i][11].ToString();
                                obj.maintenance_plant = serviceDetails.Rows[i][12].ToString();
                                obj.functional_location = serviceDetails.Rows[i][13].ToString();
                                obj.equipment = serviceDetails.Rows[i][14].ToString();
                                obj.required_start = Convert.ToDateTime(serviceDetails.Rows[i][15].ToString());
                                obj.required_end = Convert.ToDateTime(serviceDetails.Rows[i][16].ToString());
                                obj.location = serviceDetails.Rows[i][17].ToString();
                                obj.main_work_center = serviceDetails.Rows[i][18].ToString();
                                obj.maintenance_item = serviceDetails.Rows[i][19].ToString();
                                obj.maintenance_plan = serviceDetails.Rows[i][20].ToString();
                                obj.rekomendasi = serviceDetails.Rows[i][21].ToString();
                                _context.SaveChanges();
                            }
                            else
                            {

                                Notifikasi notifikasi = new Notifikasi();
                                notifikasi.notification_type = serviceDetails.Rows[i][0].ToString();
                                notifikasi.notifikasi = serviceDetails.Rows[i][1].ToString();
                                notifikasi.order = serviceDetails.Rows[i][2].ToString();
                                notifikasi.notification_date = Convert.ToDateTime(serviceDetails.Rows[i][3].ToString());
                                notifikasi.created_by = serviceDetails.Rows[i][4].ToString();
                                notifikasi.created_on = Convert.ToDateTime(serviceDetails.Rows[i][5].ToString());
                                notifikasi.change_by = serviceDetails.Rows[i][6].ToString();
                                notifikasi.change_on = Convert.ToDateTime(serviceDetails.Rows[i][7].ToString());
                                notifikasi.planner_group = serviceDetails.Rows[i][8].ToString();
                                notifikasi.description = serviceDetails.Rows[i][9].ToString();
                                notifikasi.user_status = Convert.ToInt32(serviceDetails.Rows[i][10].ToString());
                                notifikasi.system_status = serviceDetails.Rows[i][11].ToString();
                                notifikasi.maintenance_plant = serviceDetails.Rows[i][12].ToString();
                                notifikasi.functional_location = serviceDetails.Rows[i][13].ToString();
                                notifikasi.equipment = serviceDetails.Rows[i][14].ToString();
                                notifikasi.required_start = Convert.ToDateTime(serviceDetails.Rows[i][15].ToString());
                                notifikasi.required_end = Convert.ToDateTime(serviceDetails.Rows[i][16].ToString());
                                notifikasi.location = serviceDetails.Rows[i][17].ToString();
                                notifikasi.main_work_center = serviceDetails.Rows[i][18].ToString();
                                notifikasi.maintenance_item = serviceDetails.Rows[i][19].ToString();
                                notifikasi.maintenance_plan = serviceDetails.Rows[i][20].ToString();
                                notifikasi.rekomendasi = serviceDetails.Rows[i][21].ToString();
                          


                                // Add the record in Database
                                await _context.notifikasi.AddAsync(notifikasi);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
                return RedirectToAction("Notifikasi");
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel()
                {
                    ControllerName = this.RouteData.Values["controller"].ToString(),
                    ActionName = this.RouteData.Values["action"].ToString(),
                    ErrorMessage = ex.Message
                });
            }
        }

        public IActionResult Update_Rekomendasi(Notifikasi notifikasi)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Notifikasi obj = _context.notifikasi.Where(p => p.id == id).FirstOrDefault();

                if (Request.Form.Files.Count() != 0)
                    {
                        if (obj.rekomendasi != null)
                        {
                            string ExitingFile = hostEnvironment.WebRootPath + "/" + obj.rekomendasi;
                            System.IO.File.Delete(ExitingFile);
                        }

                        IFormFile postedFile = Request.Form.Files[0];
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + postedFile.FileName;
                        string path = hostEnvironment.WebRootPath + "/upload/rekomendasi/" + fileName;

                        using (var stream = System.IO.File.Create(path))
                        {
                            postedFile.CopyTo(stream);
                            obj.rekomendasi = "upload/rekomendasi/" + fileName;
                        }
                    _context.SaveChanges();
                    return Json(new { Results = true });
                }
                else
                    {
                        obj.rekomendasi = Request.Form["file_"].FirstOrDefault();
                    _context.SaveChanges();
                    return Json(new { Results = false });
                    }
            }
            catch
            {
                throw;
            }
        }

    }
}
