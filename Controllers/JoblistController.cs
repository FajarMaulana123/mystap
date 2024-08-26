using Azure.Core;
using MessagePack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using mystap.Models;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Runtime.CompilerServices;

namespace joblist.Controllers
{
    public class JoblistController : Controller
    {
        private readonly DatabaseContext _context;
        public JoblistController(DatabaseContext context)
        {
            _context = context;
        }
        
        public IActionResult Joblist()
        {
			ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == "1").ToList();
            ViewBag.equipment = _context.equipments.Where(p => p.deleted == 0).ToList();
            ViewBag.unitCode = _context.unit.Where(p => p.deleted != 1).GroupBy(p => new { p.unitCode , p.unitProses }).Select(p => new { unitCode = p.Key.unitCode , unitProses = p.Key.unitProses}).ToList();
            ViewBag.unit = _context.unit.Where(p => p.deleted == 0).ToList();
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

                var project = Request.Form["project"].FirstOrDefault();
                var project_rev = Request.Form["project_rev"].FirstOrDefault();
                var eqtagno = Request.Form["eqTagNo"].FirstOrDefault();
                var jobno = Request.Form["jobNo"].FirstOrDefault();
                var unitcode = Request.Form["unitCode"].FirstOrDefault();
                var user_section = Request.Form["user_section"].FirstOrDefault();
                // Getting all Customer data
                var query = @"select max(joblist.id) as id, max(joblist.jobNo) as jobNo, max(joblist.userSection) as userSection,max(project.projectNo) as projectNo, max(project.description) as description, max(unit.unitCode) as nama_unit, max(users.name) as name, max(joblist.keterangan) as keterangan,max(project.revision) as revision, equipments.eqTagNo, CASE WHEN count(case when c.status = 'not_ready' then c.id else NULL end) > '0' THEN 'not_ready' WHEN max(c.status) is null then 'not_ready' ELSE 'ready' END AS 'status_tagno' from Mystap.dbo.joblist  " +
                    " left join (SELECT  b.id, b.projectID,  b.joblist_id, (CASE WHEN b.dikerjakan = 'tidak' then 'ready' WHEN b.jasa != '0' AND b.material != '0' THEN CASE WHEN b.sts_material = 'ready' AND b.sts_kontrak = 'ready' THEN 'ready'  ELSE 'not_ready' END WHEN b.material != '0' THEN isnull(b.sts_material, 'not_ready') WHEN b.jasa != '0' THEN isnull(b.sts_kontrak, 'not_ready') END) as status FROM (SELECT a.id, a.joblist_id, joblist.projectID, a.jobDesc, a.jasa, a.material, a.dikerjakan, " +
                    " (CASE WHEN a.dikerjakan = 'tidak' then 'not_execute' WHEN a.jasa != '0' THEN  (SELECT (CASE WHEN contracttracking.aktualSP is null THEN 'not_ready' ELSE 'ready' END) as status FROM Mystap.dbo.contracttracking left join Mystap.dbo.joblist_detail on joblist_detail.no_jasa = contracttracking.idPaket where contracttracking.projectID = " + project + " and joblist_detail.id = a.id and contracttracking.deleted != '1') ELSE 'not_identify' END) as sts_kontrak, " +
                    " (CASE WHEN a.dikerjakan = 'tidak' then 'not_execute' WHEN a.material != '0' THEN (CASE WHEN a.status_material = 'COMPLETED' AND (SELECT (CASE WHEN joblist_detail.material != '0' THEN CASE WHEN count(DISTINCT case when procurement_.status_ != 'terpenuhi_stock' AND procurement_.status_ != 'onsite' then procurement_.[order] else NULL end) > '0' THEN 'not_ready' ELSE 'ready' END ELSE NULL END) as sts_material " +
                    " FROM (SELECT work_order.[order], (case when zpm01.pr is null or zpm01.pr = '' then (case when zpm01.reqmt_qty = zpm01.qty_res then 'terpenuhi_stock' else 'create_pr' end) else (case when (zpm01.pr is not null or zpm01.pr != '') and (p.po is null or p.po = '')then (case when (zpm01.status_pengadaan = 'tunggu_pr' or zpm01.status_pengadaan = 'evaluasi_dp3' or zpm01.status_pengadaan is null) " +
                    " then 'outstanding_pr' when zpm01.status_pengadaan = 'inquiry_harga' then 'inquiry_harga' when (zpm01.status_pengadaan = 'hps_oe' or zpm01.status_pengadaan = 'bidder_list' or zpm01.status_pengadaan = 'penilaian_kualifikasi' or zpm01.status_pengadaan = 'rfq') then 'hps_oe' when (zpm01.status_pengadaan = 'pemasukan_penawaran' or zpm01.status_pengadaan = 'pembukaan_penawaran' or zpm01.status_pengadaan = 'evaluasi_penawaran' or zpm01.status_pengadaan = 'klarifikasi_spesifikasi' or zpm01.status_pengadaan = 'evaluasi_teknis' or zpm01.status_pengadaan = 'evaluasi_tkdn' or zpm01.status_pengadaan = 'negosiasi'  or zpm01.status_pengadaan = 'lhp') then 'proses_tender' when (zpm01.status_pengadaan = 'pengumuman_pemenang' or zpm01.status_pengadaan = 'penunjuk_pemenang' or zpm01.status_pengadaan = 'purchase_order') then 'Penetapan Pemenang' else 'outstanding_pr' end) " +
                    " when (zpm01.pr is not null or zpm01.pr != '') and (p.po is not null or p.po != '') then (case when p.dci is null or p.dci = '' then 'tunggu_onsite' else 'onsite' end) end) end) as status_ " +
                    " FROM Mystap.dbo.zpm01 left join Mystap.dbo.work_order on work_order.[order] = zpm01.no_order left join Mystap.dbo.purch_order as p on p.material = zpm01.material AND p.pr = zpm01.pr AND p.item_pr = zpm01.itm_pr where work_order.revision = '" + project_rev + "' and ((del is null or del != 'X') and (zpm01.reqmt_qty is not null and zpm01.reqmt_qty != '0')) GROUP BY [work_order].[order],zpm01.material,zpm01.itm, zpm01.pr,zpm01.reqmt_qty, zpm01.qty_res, po, zpm01.status_pengadaan, dci) procurement_ " +
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
                    customerData = customerData.Where(b => b.description.StartsWith(searchValue));
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

        public IActionResult CreateJoblist()
        {
            ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == "1").ToList();
            ViewBag.equipment = _context.equipments.Where(p => p.deleted == 0).ToList();
            ViewBag.unitCode = _context.unit.Where(p => p.deleted != 1).GroupBy(p => new { p.unitCode, p.unitProses }).Select(p => new { unitCode = p.Key.unitCode, unitProses = p.Key.unitProses }).ToList();
            ViewBag.unit = _context.unit.Where(p => p.deleted == 0).ToList();

            return View();
        }

        public IActionResult UpdateJoblist(long? id)
        {
            ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == "1").ToList();
            ViewBag.equipment = _context.equipments.Where(p => p.deleted == 0).ToList();
            ViewBag.unitCode = _context.unit.Where(p => p.deleted != 1).GroupBy(p => new { p.unitCode, p.unitProses }).Select(p => new { unitCode = p.Key.unitCode, unitProses = p.Key.unitProses }).ToList();
            ViewBag.unit = _context.unit.Where(p => p.deleted == 0).ToList();

            var data = (from j in _context.joblist
                        join e in _context.equipments on j.id_eqTagNo equals e.id
                        join u in _context.unit on j.unitCode equals u.id
                        join p in _context.project on j.projectID equals p.id
                        select new
                        {
                            projectID = p.id,
                            projectNo = p.projectNo,
                            taoh = p.taoh,
                            unitCode = u.unitCode,
                            codeJobe = u.codeJob,
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
                joblist.createBy = 1;
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
                    obj.updatedBy = 1;
                    obj.modifyBy = 1;
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
            catch
            {
                throw;
            }
        }

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


        public IActionResult Planning()
        {
            return View();
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

              
                //var customerData = _context.joblist_Detail.Include("contracttracking").Include("joblist").Include("equipments").Include("project").Include("unit").Include("users").Where(s => s.deleted == 0).Select(a => new { id = a.id, eqTagNo = a.equipments.eqTagNo, jobDesc = a.jobDesc, alias = a.users.alias, status = a.freezing, isJasa = a.jasa, noPaket = a.contracttracking.noPaket, judul_paket = a.contracttracking.judulPekerjaan, wo_jasa = a.contracttracking.WO, no_po = a.contracttracking.po, no_sp = a.contracttracking.noSP, status_jasa = a.status_jasa, ismaterial = a.material, order = a.no_order, status_material = a.status_material, ket_status_material = a.ket_status_material, all_in_kontrak = a.all_in_kontrak, lldi = a.lldi, status_job = a.status_job, /*sts_ready = a.status_material,*/ disiplin = a.disiplin });
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                //{
                //    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                //}

                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    customerData = customerData.Where(b => b.jobDesc.StartsWith(searchValue));
                //}
                //// Total number of rows count
                ////Console.WriteLine(customerData);
                //recordsTotal = customerData.Count();
                //// Paging
                //var datas = await customerData.Skip(skip).Take(pageSize).ToListAsync();
                //var data = _memoryCache.Get("products");
                //data = await _memoryCache.Set("products", datas, expirationTime);
                // Returning Json Data
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
