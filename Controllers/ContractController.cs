using Azure.Core;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Vml;
using Humanizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using mystap.Helpers;
using mystap.Models;
using System;
using System.Globalization;
using System.IO;
using System.Linq.Dynamic.Core;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using DocumentFormat.OpenXml.Wordprocessing;
using iText.Html2pdf;
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using Spire.Doc.Documents;
//using SelectPdf;

namespace mystap.Controllers
{
    public class ContractController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}
        private readonly DatabaseContext _context;
        private readonly IWebHostEnvironment environment;
        public ContractController(DatabaseContext context, IWebHostEnvironment environment)
        {
            _context = context;
            this.environment = environment;
        }

		[AuthorizedAction]
		public IActionResult Sow()
        {
            ViewBag.role = "SOW";
            if (Module.hasModule("SOW", HttpContext.Session))
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
                ViewBag.userAccount = _context.users.Where(p => p.locked != 1).Where(p => p.statPekerja == "PLANNER").Where(p => p.status == "PEKERJA").Where(p => p.alias != null && p.alias != "").ToList();
                ViewBag.project = _context.project.Where(p => p.deleted == 0 && p.active == 1).ToList();
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
                    select += "<option value='"+val.sub_group +"' data-urut='"+val.urut+"'>" + val.sub_group + "</ option >";
                }

                return base.Content(select, "text/html");
            }
            catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public async Task<IActionResult> Get_Sow()
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

                var project = Request.Form["project"].FirstOrDefault();
                var events = Request.Form["events"].FirstOrDefault();
                var groups = Request.Form["groups"].FirstOrDefault();
                var area = Request.Form["area"].FirstOrDefault();
                var kabo = Request.Form["kabo"].FirstOrDefault();
                var tahun = Request.Form["tahun"].FirstOrDefault();
                var no = Request.Form["no"].FirstOrDefault();


                var customerData = (from a in _context.sow
                                    join p in _context.project on a.projectID equals p.id
                                    select new
                                    {
                                        id = a.id,
                                        projectId = p.id,
                                        noSOW = a.noSOW,
                                        jobCode = a.jobCode,
                                        judulPekerjaan = a.judulPekerjaan,
                                        planner = a.planner,
                                        kabo = a.kabo,
                                        events = a.events,
                                        groups = a.groups,
                                        subGroups = a.subGroups,
                                        area = a.area,
                                        file = a.file,
                                        tahun = a.tahun,
                                        description = p.description,
                                        createdBy = a.createdBy,
                                        modifyBy = a.modifyBy,
                                        deleted = a.deleted
                                    });

                customerData = customerData.Where(p => p.deleted == 0);

                if(project != "")
                {
                    customerData = customerData.Where(p => p.projectId == Convert.ToInt32(project));
                }

                if(events != "")
                {
                    customerData = customerData.Where(p => p.events == events);
                }

                if(groups != "")
                {
                    customerData = customerData.Where(p => p.groups == groups);
                }

                if(area != "")
                {
                    customerData = customerData.Where(p => p.area == area);
                }

                if(kabo != "")
                {
                    customerData = customerData.Where(p => p.kabo == kabo);
                }

                if(tahun != "")
                {
                    customerData = customerData.Where(p => p.tahun == tahun);
                }

                if(no != "")
                {
                    customerData = customerData.Where(p => p.noSOW.StartsWith(no));
                }


                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.judulPekerjaan.StartsWith(searchValue));
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

		[AuthorizedAction]
		public IActionResult Create_Sow(IFormCollection formcollaction)
        {
            try
            {
                

                //Fetch the File Name.
              

                string tahun = formcollaction["tahun"];
                string inisial = formcollaction["inisial"];
                var cek = _context.sow.Where(p => p.tahun == tahun).Where(w => w.jobCode.Contains(inisial)).Select(p => new
                            {
                                tahun = p.tahun,
                                kode = p.jobCode.Max()
                            }).FirstOrDefault();
                var no = 0;
                if (cek.tahun != tahun )
                {
                    no = 1;
                }
                else
                {
                    var p = cek.kode;
                    no = p + 1;
                }

                Sow sow = new Sow();
                sow.jobCode = formcollaction["inisial"] + "-" + no.ToString("D3");
                sow.noSOW = formcollaction["event"] + "-" + no.ToString("D3") + "-" + formcollaction["inisial"] + formcollaction["urut"] + "-" + formcollaction["area"] + "/" + formcollaction["codeKabo"] + "/" + tahun.Substring(tahun.Length - 2); ;
                sow.projectID = Convert.ToInt32(formcollaction["project"]);
                sow.events = formcollaction["events"];
                sow.groups = formcollaction["groups_"];
                sow.subGroups = formcollaction["subGroup"];
                sow.area = formcollaction["area"];
                sow.kabo = formcollaction["kabo"];
                sow.tahun = formcollaction["tahun"];
                sow.judulPekerjaan = formcollaction["judulPekerjaan"];
                sow.planner = formcollaction["planner"];

                if(Request.Form.Files.Count() != 0)
                {
                    IFormFile postedFile = Request.Form.Files[0];
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + postedFile.FileName;
                    string path = environment.WebRootPath + "/upload/sow/" + fileName;
                    using (var stream = System.IO.File.Create(path))
                    {
                        postedFile.CopyTo(stream);
                        sow.file = "upload/sow/" + fileName;
                    }

                }

                sow.createdBy = HttpContext.Session.GetString("alias");
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

		[AuthorizedAction]
		public IActionResult Update_Sow(Sow sow)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Sow obj = _context.sow.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {

                    string tahun = Request.Form["tahun"].FirstOrDefault();
                    string inisial = Request.Form["inisial"].FirstOrDefault();
                    //var cek = _context.sow.Where(p => p.tahun == tahun).Where(w => w.tahun.Contains(tahun)).Max(p => new { kode = p.jobCode, tahun = p.tahun });
                    //int no = 0;
                    //if (cek.kode != tahun)
                    //{
                    //    no = 1;
                    //}
                    //else
                    //{
                    //    no = Convert.ToInt32(cek.kode) + 1;
                    //}

                    //obj.jobCode = Request.Form["inisial"].FirstOrDefault() + "-" + no.ToString("D3");
                    //obj.noSOW = Request.Form["event"].FirstOrDefault() + "-" + no.ToString("D3") + "-" + Request.Form["inisial"].FirstOrDefault() + Request.Form["urut"].FirstOrDefault() + "-" + Request.Form["area"].FirstOrDefault() + "/" + Request.Form["codeKabo"].FirstOrDefault() + "/" + tahun.Substring(tahun.Length - 2); ;
                    obj.projectID = Convert.ToInt32(Request.Form["project"].FirstOrDefault());
                    obj.events = Request.Form["events"].FirstOrDefault();
                    obj.groups = Request.Form["groups_"].FirstOrDefault();
                    obj.subGroups = Request.Form["subGroup"].FirstOrDefault();
                    obj.area = Request.Form["area"].FirstOrDefault();
                    obj.kabo = Request.Form["kabo"].FirstOrDefault();
                    obj.tahun = Request.Form["tahun"].FirstOrDefault();
                    obj.judulPekerjaan = Request.Form["judulPekerjaan"].FirstOrDefault();
                    obj.planner = Request.Form["planner"].FirstOrDefault();

                    if (Request.Form.Files.Count() != 0)
                    {
                        if(obj.file != null)
                        {
                            string ExitingFile = environment.WebRootPath+ "/" + obj.file;
                            System.IO.File.Delete(ExitingFile);
                        }

                        IFormFile postedFile = Request.Form.Files[0];
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + postedFile.FileName;
                        string path = environment.WebRootPath + "/upload/sow/" + fileName;
                        
                        using (var stream = System.IO.File.Create(path))
                        {
                            postedFile.CopyTo(stream);
                            obj.file = "upload/sow/" + fileName;
                        }

                    }
                    else
                    {
                        obj.file = Request.Form["file_"].FirstOrDefault();
                    }
                  
                    obj.modifyBy = HttpContext.Session.GetString("alias");
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
		public IActionResult Deleted_Sow(Sow sow)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                Sow obj = _context.sow.Where(p => p.id == id).FirstOrDefault();

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
		public IActionResult DurasiStep()
        {
            ViewBag.role = "DURASI_STEP";
            if (Module.hasModule("DURASI_STEP", HttpContext.Session))
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
		public async Task<IActionResult> Get_DurasiStep()
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

                var filter = Request.Form["columns[2][search][value]"].FirstOrDefault();

                var project_filter = Request.Form["project"].FirstOrDefault();
                var kat_tender_filter = Request.Form["kat_tender"].FirstOrDefault();

                var customerData = (from a in _context.durasi
                                    join p in _context.project on a.id_project equals p.id
                                    select new
                                    {
                                        id = a.id,
                                        id_project = p.id,
                                        description = a.project.description,
                                        kat_tender = a.kat_tender,
                                        susun_kak = a.susun_kak,
                                        persetujuan = a.persetujuan,
                                        susun_oe = a.susun_oe,
                                        kirim_ke_co = a.kirim_ke_co,
                                        pengumuman_pendaftaran = a.pengumuman_pendaftaran,
                                        sertifikasi = a.sertifikasi,
                                        prakualifikasi = a.prakualifikasi,
                                        undangan = a.undangan,
                                        pemberian = a.pemberian,
                                        penyampaian = a.penyampaian,
                                        pembukaan = a.pembukaan,
                                        evaluasi = a.evaluasi,
                                        negosiasi = a.negosiasi,
                                        usulan = a.usulan,
                                        keputusan = a.keputusan,
                                        pengumuman_pemenang = a.pengumuman_pemenang,
                                        pengajuan_sanggah = a.pengajuan_sanggah,
                                        jawaban_sanggah = a.jawaban_sanggah,
                                        tunjuk_pemenang = a.tunjuk_pemenang,
                                        proses_spb = a.proses_spb
                                    });
                    

                if(project_filter != "")
                {
                    customerData = customerData.Where(a => a.id_project == Convert.ToInt64(project_filter));
                }

                if(kat_tender_filter != "")
                {
                    customerData = customerData.Where(a => a.kat_tender == kat_tender_filter);
                }
                    
                // Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.kat_tender.StartsWith(searchValue));
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

		[AuthorizedAction]
		public IActionResult Create_DurasiStep(IFormCollection formcollaction)
        {
            try
            {
                Durasi durasi = new Durasi();
                durasi.id_project = Convert.ToInt64(formcollaction["id_project"]);
                durasi.kat_tender = formcollaction["kat_tender"];
                durasi.susun_kak = Convert.ToInt32(formcollaction["susun_kak"]);
                durasi.susun_oe = Convert.ToInt32(formcollaction["susun_oe"]);
                durasi.persetujuan = Convert.ToInt32(formcollaction["persetujuan"]);
                durasi.kirim_ke_co = Convert.ToInt32(formcollaction["kirim_ke_co"]);
                durasi.pengumuman_pendaftaran = Convert.ToInt32(formcollaction["pengumuman_pendaftaran"]);
                durasi.sertifikasi = Convert.ToInt32(formcollaction["sertifikasi"]);
                durasi.prakualifikasi = Convert.ToInt32(formcollaction["prakualifikasi"]);
                durasi.undangan = Convert.ToInt32(formcollaction["undangan"]);
                durasi.pemberian = Convert.ToInt32(formcollaction["pemberian"]);
                durasi.penyampaian = Convert.ToInt32(formcollaction["penyampaian"]);
                durasi.pembukaan = Convert.ToInt32(formcollaction["pembukaan"]);
                durasi.evaluasi = Convert.ToInt32(formcollaction["evaluasi"]);
                durasi.negosiasi = Convert.ToInt32(formcollaction["negosiasi"]);
                durasi.usulan = Convert.ToInt32(formcollaction["usulan"]);
                durasi.keputusan = Convert.ToInt32(formcollaction["keputusan"]);
                durasi.pengumuman_pemenang = Convert.ToInt32(formcollaction["pengumuman_pemenang"]);
                durasi.pengajuan_sanggah = Convert.ToInt32(formcollaction["pengajuan_sanggah"]);
                durasi.jawaban_sanggah = Convert.ToInt32(formcollaction["jawaban_sanggah"]);
                durasi.tunjuk_pemenang = Convert.ToInt32(formcollaction["tunjuk_pemenang"]);
                durasi.proses_spb = Convert.ToInt32(formcollaction["proses_spb"]);

                Boolean t;
                if (durasi != null)
                {
                    _context.durasi.Add(durasi);
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

		[AuthorizedAction]
		public IActionResult Update_DurasiStep(Durasi durasi)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Durasi obj = _context.durasi.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {

                    //obj.id_project = Convert.ToInt64(Request.Form["id_project"].FirstOrDefault());
                    //obj.kat_tender = Request.Form["kat_tender"].FirstOrDefault();
                    obj.susun_kak = Convert.ToInt32(Request.Form["susun_kak"].FirstOrDefault());
                    obj.susun_oe = Convert.ToInt32(Request.Form["susun_oe"].FirstOrDefault());
                    obj.persetujuan = Convert.ToInt32(Request.Form["persetujuan"].FirstOrDefault());
                    obj.kirim_ke_co = Convert.ToInt32(Request.Form["kirim_ke_co"].FirstOrDefault());
                    obj.pengumuman_pendaftaran = Convert.ToInt32(Request.Form["pengumuman_pendaftaran"].FirstOrDefault());
                    obj.sertifikasi = Convert.ToInt32(Request.Form["sertifikasi"].FirstOrDefault());
                    obj.prakualifikasi = Convert.ToInt32(Request.Form["prakualifikasi"].FirstOrDefault());
                    obj.undangan = Convert.ToInt32(Request.Form["undangan"].FirstOrDefault());
                    obj.pemberian = Convert.ToInt32(Request.Form["pemberian"].FirstOrDefault());
                    obj.penyampaian = Convert.ToInt32(Request.Form["penyampaian"].FirstOrDefault());
                    obj.pembukaan = Convert.ToInt32(Request.Form["pembukaan"].FirstOrDefault());
                    obj.evaluasi = Convert.ToInt32(Request.Form["evaluasi"].FirstOrDefault());
                    obj.negosiasi = Convert.ToInt32(Request.Form["negosiasi"].FirstOrDefault());
                    obj.usulan = Convert.ToInt32(Request.Form["usulan"].FirstOrDefault());
                    obj.keputusan = Convert.ToInt32(Request.Form["keputusan"].FirstOrDefault());
                    obj.pengumuman_pemenang = Convert.ToInt32(Request.Form["pengumuman_pemenang"].FirstOrDefault());
                    obj.pengajuan_sanggah = Convert.ToInt32(Request.Form["pengajuan_sanggah"].FirstOrDefault());
                    obj.jawaban_sanggah = Convert.ToInt32(Request.Form["tunjuk_pemenang"].FirstOrDefault());
                    obj.tunjuk_pemenang = Convert.ToInt32(Request.Form["tunjuk_pemenang"].FirstOrDefault());
                    obj.proses_spb = Convert.ToInt32(Request.Form["proses_spb"].FirstOrDefault());




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
		public IActionResult ContractTracking()
        {
            ViewBag.role = "MANAGE_CONTRACT";
            if (Module.hasModule("MANAGE_CONTRACT", HttpContext.Session))
            {
                ViewBag.user = _context.users.Where(p => p.locked != 1).Where(p => p.statPekerja == "PLANNER").Where(p => p.alias != null && p.alias != "").Where(p => p.status == "PEKERJA").ToList();
                ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == 1).ToList();
                ViewBag.unit = _context.contractItem.Where(p => p.item_group == "UNIT").ToList();

                return View();
            }
            else
            {
                return NotFound();
            }

            
        }

		[AuthorizedAction]
		public async Task<IActionResult> ContractTracking_()
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

                var project = Request.Form["project_filter"].FirstOrDefault();
                var unit = Request.Form["unit_filter"].FirstOrDefault();
                var pic = Request.Form["pic_filter"].FirstOrDefault();

                var customerData = (from ct in _context.contractTracking
                                    join sow in _context.sow on ct.id_sow equals sow.id
                                    select new
                                    {
                                        contract = ct,
                                        judul_pekerjaan = sow.judulPekerjaan,
                                    }).Where(p => p.contract.deleted != 1);



                if (!string.IsNullOrEmpty(project))
                {
                    customerData = customerData.Where(p => p.contract.projectID == Convert.ToInt32(project));
                }

                if (!string.IsNullOrEmpty(unit))
                {
                    customerData = customerData.Where(p => p.contract.unit == unit);
                }

                if (!string.IsNullOrEmpty(pic))
                {
                    customerData = customerData.Where(p => p.contract.pic == pic);
                }


                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.judul_pekerjaan.StartsWith(searchValue));
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

		[AuthorizedAction]
		public IActionResult CreateContract()
        {
            ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == 1).ToList();
            ViewBag.pic = _context.users.Where(p => p.locked != 1).Where(p => p.statPekerja == "PLANNER").Where(p => p.alias != null && p.alias != "").Where(p => p.status == "PEKERJA").ToList();
            ViewBag.unitProses = _context.unitProses.Where(p => p.deleted == 0).ToList();
            ViewBag.unit = _context.contractItem.Where(p => p.item_group == "UNIT").ToList();
            ViewBag.kategori = _context.contractItem.Where(p => p.item_group == "KATEGORIPAKET").ToList();
            ViewBag.kriteria = _context.contractItem.Where(p => p.item_group == "KRITERIA").ToList();
            ViewBag.skill = _context.contractItem.Where(p => p.item_group == "SKILLGROUP").ToList();
            ViewBag.srcvendor = _context.contractItem.Where(p => p.item_group == "SRCVENDOR").ToList();
            ViewBag.asalsrc = _context.contractItem.Where(p => p.item_group == "ASALSRC").ToList();
            ViewBag.koordinasi = _context.contractItem.Where(p => p.item_group == "KOORDINASI").ToList();
            ViewBag.kat_tender = _context.contractItem.Where(p => p.item_group == "KATEGORITENDER").ToList();
            ViewBag.dirpws = _context.contractItem.Where(p => p.item_group == "DIRPWS").ToList();
            return View();
        }

		[AuthorizedAction]
		public IActionResult GetSow()
        {
            var id = Request.Form["project"].FirstOrDefault();
            var data = _context.sow.Where(p => p.projectID == Convert.ToInt64(id) && p.deleted == 0).ToList();
            var isi = "<option value=''>Select List SOW</option>";
            foreach(var val in data)
            {
                isi += "<option value='"+val.id+"' data-judul='"+val.judulPekerjaan+"'>" + val.judulPekerjaan + "</option>";
            }

            return Ok(isi);
        }

		[AuthorizedAction]
		public IActionResult GetKatTender()
        {
            var id_project = Convert.ToInt32(Request.Form["project"].FirstOrDefault());
            var data = _context.durasi.Where(p => p.id_project == id_project)
                .GroupBy(p => new { p.kat_tender })
                .Select(j => new
                {
                    kat_tender = j.Key.kat_tender,
                    project_id = j.Select(p => p.id_project),
                    total = j.Select(p => p.kirim_ke_co + p.pengumuman_pendaftaran + p.sertifikasi + p.prakualifikasi + p.undangan + p.pemberian + p.pemberian + p.penyampaian + p.pembukaan + p.evaluasi + p.negosiasi + p.usulan + p.keputusan + p.pengumuman_pemenang + p.pengajuan_sanggah + p.jawaban_sanggah + p.tunjuk_pemenang + p.proses_spb).Sum(),
                    total_ph = j.Select(p => p.pembukaan + p.evaluasi + p.negosiasi + p.usulan + p.keputusan + p.pengumuman_pemenang + p.pengajuan_sanggah + p.jawaban_sanggah + p.tunjuk_pemenang + p.proses_spb).Sum(),
                }).ToList();

            var isi = "<option value=''>Select Kat Tender</option>";
            foreach (var val in data)
            {
                isi += "<option value='" + val.kat_tender + "' data-total='"+val.total+ "' data-total_ph='"+val.total_ph+"'>" + val.kat_tender + "</option>";
            }

            return Ok(isi);
        }

		[AuthorizedAction]
		public IActionResult CreateContract_()
        {
            try
            {
                var unit = Request.Form["unit"].FirstOrDefault();
                var bulan = Request.Form["bulan"].FirstOrDefault();
                var idCode = Request.Form["unit"].FirstOrDefault() + bulan.Split('-')[1].ToString() + bulan.Split('-')[0].ToString().Substring(2, 2).ToString();

                var cek = _context.contractTracking.Where(p => p.noPaket.StartsWith(idCode)).Select(p => new
                {
                    kode = p.noPaket.Max()
                }).FirstOrDefault();

                var no = 0;
                if (cek == null)
                {
                    no = 1;
                }
                else
                {
                    var p = cek.kode;
                    no = p + 1;
                }

                ContractTracking val = new ContractTracking();
                val.noPaket = idCode + no.ToString("D3");
                val.projectID = Convert.ToInt64(Request.Form["projectID"].FirstOrDefault());
                val.bulan = bulan.Split('-')[1].ToString();
                val.tahun = bulan.Split('-')[0].ToString();
                val.id_sow = Convert.ToInt64(Request.Form["judulPekerjaan"].FirstOrDefault());
                //val.judulPekerjaan = Request.Form["judulPekerjaan"].FirstOrDefault();
                val.unit = Request.Form["unit"].FirstOrDefault();
                val.pic = Request.Form["pic"].FirstOrDefault();
                val.kategoriPaket = Request.Form["kategoriPaket"].FirstOrDefault();
                val.tipePaket = Request.Form["tipePaket"].FirstOrDefault();
                val.kriteria = Request.Form["kriteria"].FirstOrDefault();
                val.katPaket = Request.Form["katPaket"].FirstOrDefault();
                val.skillGroup = Request.Form["skillGroup"].FirstOrDefault();
                val.csms = Request.Form["csms"].FirstOrDefault();
                val.sourceVendor = Request.Form["sourceVendor"].FirstOrDefault();
                val.asalSource = Request.Form["asalSource"].FirstOrDefault();
                val.tipe_koordinasi = Convert.ToInt32(Request.Form["tipe_koordinasi"].FirstOrDefault());
                val.koordinasi = Request.Form["koordinasi"].FirstOrDefault();
                val.kota = Request.Form["kota"].FirstOrDefault();
                val.disiplin = Request.Form["disiplin"].FirstOrDefault();
                val.dirPWS = Request.Form["direksiPengawas"].FirstOrDefault();
                val.perluAanwijzing = Convert.ToInt32(Request.Form["preluAanwijzing"].FirstOrDefault());
                val.katTender = Request.Form["katTender"].FirstOrDefault();
                val.deadLine = Convert.ToDateTime(Request.Form["deadline"].FirstOrDefault()).Date;
                val.persiapan = Convert.ToInt32(Request.Form["persiapan"].FirstOrDefault());
                val.fabrikasi = Convert.ToInt32(Request.Form["fabrikasi"].FirstOrDefault());
                val.mech = Convert.ToInt32(Request.Form["mdays"].FirstOrDefault());
                val.finishing = Convert.ToInt32(Request.Form["finishing"].FirstOrDefault());
                val.pemeliharaan = Convert.ToInt32(Request.Form["maint"].FirstOrDefault());
                val.targetCO = Convert.ToDateTime(Request.Form["target_co"].FirstOrDefault()).Date;
                val.mulai = Convert.ToDateTime(Request.Form["start_date"].FirstOrDefault()).Date;
                val.selesai = Convert.ToDateTime(Request.Form["end_date"].FirstOrDefault()).Date;
                val.targetBukaPH = Convert.ToDateTime(Request.Form["target_buka_ph"].FirstOrDefault()).Date;
                val.targetSP = Convert.ToDateTime(Request.Form["target_terbit_sp"].FirstOrDefault()).Date;
                val.dateCreated = DateTime.Now;
                val.createdBy = HttpContext.Session.GetInt32("id");
                val.deleted = 0;

                var durasi = _context.durasi.Where(p => p.id_project == Convert.ToInt64(Request.Form["projectID"].FirstOrDefault()) && p.kat_tender == Request.Form["katTender"].FirstOrDefault()).FirstOrDefault();
                if (durasi != null)
                {
                    DateTime target = Convert.ToDateTime(Request.Form["target_terbit_sp"].FirstOrDefault()).AddDays(-Convert.ToDouble(durasi.proses_spb));
                    val.target_penunjukan_pemenang = target.Date;
                    val.target_jawaban_sanggah = Convert.ToDateTime(val.target_penunjukan_pemenang).AddDays(-Convert.ToDouble(durasi.tunjuk_pemenang));
                    val.target_pengajuan_sanggah = Convert.ToDateTime(val.target_jawaban_sanggah).AddDays(-Convert.ToDouble(durasi.jawaban_sanggah));
                    val.target_pengumuman_pemenang = Convert.ToDateTime(val.target_pengajuan_sanggah).AddDays(-Convert.ToDouble(durasi.pengajuan_sanggah));
                    val.target_keputusan_pemenang = Convert.ToDateTime(val.target_pengumuman_pemenang).AddDays(-Convert.ToDouble(durasi.pengumuman_pemenang));
                    val.target_usulan_pemenang = Convert.ToDateTime(val.target_keputusan_pemenang).AddDays(-Convert.ToDouble(durasi.keputusan));
                    val.target_negosiasi = Convert.ToDateTime(val.target_usulan_pemenang).AddDays(-Convert.ToDouble(durasi.usulan));
                    val.target_evaluasi = Convert.ToDateTime(val.target_negosiasi).AddDays(-Convert.ToDouble(durasi.negosiasi));
                    val.target_pembukaan = Convert.ToDateTime(val.target_evaluasi).AddDays(-Convert.ToDouble(durasi.evaluasi));
                    DateTime target_penyampaian = Convert.ToDateTime(val.target_pembukaan).AddDays(-Convert.ToDouble(durasi.pembukaan));
                    val.target_pemberian = Convert.ToDateTime(target_penyampaian).AddDays(-Convert.ToDouble(durasi.penyampaian));
                    val.target_undangan = Convert.ToDateTime(val.target_pemberian).AddDays(-Convert.ToDouble(durasi.pemberian));
                    val.target_prakualifikasi = Convert.ToDateTime(val.target_undangan).AddDays(-Convert.ToDouble(durasi.undangan));
                    val.target_sertifikasi = Convert.ToDateTime(val.target_prakualifikasi).AddDays(-Convert.ToDouble(durasi.prakualifikasi));
                    val.target_pengumuman = Convert.ToDateTime(val.target_sertifikasi).AddDays(-Convert.ToDouble(durasi.sertifikasi));
                    DateTime target_co = Convert.ToDateTime(val.target_pengumuman).AddDays(-Convert.ToDouble(durasi.pengumuman_pendaftaran));
                    val.target_persetujuan = Convert.ToDateTime(target_co).AddDays(-Convert.ToDouble(durasi.kirim_ke_co));
                    val.target_oe = Convert.ToDateTime(val.target_persetujuan).AddDays(-Convert.ToDouble(durasi.persetujuan));
                    val.target_kak = Convert.ToDateTime(val.target_oe).AddDays(-Convert.ToDouble(durasi.susun_oe));

                    DateTime a = Convert.ToDateTime(Request.Form["deadline"].FirstOrDefault()).Date;
                    DateTime b = Convert.ToDateTime(Request.Form["target_terbit_sp"].FirstOrDefault()).Date;
                    val.t_light = (a - b).Days;
                }

                Boolean t;
                if (val != null)
                {
                    _context.contractTracking.Add(val);
                    _context.SaveChanges();
                    t = true;
                }
                else
                {
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
		public IActionResult UpdateContract(long id)
        {
            ViewBag.sow = _context.sow.Where(p => p.deleted == 0).ToList();
            ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == 1).ToList();
            ViewBag.user = _context.users.Where(p => p.locked != 1).Where(p => p.statPekerja == "PLANNER").Where(p => p.alias != null && p.alias != "").Where(p => p.status == "PEKERJA").ToList();
            ViewBag.unitProses = _context.unitProses.Where(p => p.deleted == 0).ToList();
            ViewBag.unit = _context.contractItem.Where(p => p.item_group == "UNIT").ToList();
            ViewBag.kategori = _context.contractItem.Where(p => p.item_group == "KATEGORIPAKET").ToList();
            ViewBag.kriteria = _context.contractItem.Where(p => p.item_group == "KRITERIA").ToList();
            ViewBag.skill = _context.contractItem.Where(p => p.item_group == "SKILLGROUP").ToList();
            ViewBag.srcvendor = _context.contractItem.Where(p => p.item_group == "SRCVENDOR").ToList();
            ViewBag.asalsrc = _context.contractItem.Where(p => p.item_group == "ASALSRC").ToList();
            ViewBag.koordinasi = _context.contractItem.Where(p => p.item_group == "KOORDINASI").ToList();
            ViewBag.kat_tender = _context.contractItem.Where(p => p.item_group == "KATEGORITENDER").ToList();
            ViewBag.dirpws = _context.contractItem.Where(p => p.item_group == "DIRPWS").ToList();
            ViewBag.manpower = _context.contractItem.Where(p => p.item_group == "MANPOWER").ToList();
            ViewBag.fisilitator = _context.contractItem.Where(p => p.item_group == "FISILITATOR").ToList();
            ViewBag.status = _context.contractItem.Where(p => p.item_group == "STATUS").ToList();
            ViewBag.tools = _context.contractItem.Where(p => p.item_group == "TOOLS").ToList();
            ViewBag.koordinasi = _context.contractItem.Where(p => p.item_group == "KOORDINASI").ToList();

            var contract = _context.contractTracking.Where(p => p.idPaket == id).FirstOrDefault();
            ViewBag.durasi = _context.durasi.Where(p => p.id_project == contract.projectID && p.kat_tender == contract.katTender).FirstOrDefault();
            ViewBag.data = contract;

            return View();
        }

		[AuthorizedAction]
		public IActionResult UpdateContract_()
        {
            try
            {
                var bulan = Request.Form["bulan"].FirstOrDefault();
                int id_ = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                ContractTracking val = _context.contractTracking.Where(p => p.idPaket == id_).FirstOrDefault();
                Boolean t;
                if (val != null)
                {
                    val.unit = Request.Form["unit"].FirstOrDefault();
                    val.bulan = bulan.Split('-')[1].ToString();
                    val.tahun = bulan.Split('-')[0].ToString();
                    val.pic = Request.Form["pic"].FirstOrDefault();
                    val.projectID = Convert.ToInt64(Request.Form["projectNo"].FirstOrDefault());
                    val.katPaket = Request.Form["katPaket"].FirstOrDefault();
                    val.tipePaket = Request.Form["tipePaket"].FirstOrDefault();
                    val.kriteria = Request.Form["kriteria"].FirstOrDefault();
                    val.id_sow = Convert.ToInt64(Request.Form["judulPekerjaan"].FirstOrDefault());
                    //val.judulPekerjaan = Request.Form["judulPekerjaan"].FirstOrDefault();
                    val.tipe_koordinasi = Convert.ToInt32(Request.Form["tipe_koordinasi"].FirstOrDefault());
                    val.koordinasi = Request.Form["koordinasi"].FirstOrDefault();
                    val.disiplin = Request.Form["disiplin"].FirstOrDefault();
                    val.dirPWS = Request.Form["direksiPengawas"].FirstOrDefault();

                    val.katTender = Request.Form["katTender"].FirstOrDefault();
                    val.WO = Request.Form["orderNo"].FirstOrDefault();
                    val.PR = Request.Form["pr_no"].FirstOrDefault();
                    val.po = Request.Form["po_no"].FirstOrDefault();
                    val.noSP = Request.Form["sp_no"].FirstOrDefault();
                    val.deadLine = Convert.ToDateTime(Request.Form["dead_line"].FirstOrDefault()).Date;
                    val.targetSP = Convert.ToDateTime(Request.Form["terbit_sp"].FirstOrDefault()).Date;
                    val.skillGroup = Request.Form["skillGroup"].FirstOrDefault();
                    val.csms = Request.Form["csms"].FirstOrDefault();
                    val.sourceVendor = Request.Form["sourceVendor"].FirstOrDefault();
                    val.asalSource = Request.Form["asalSource"].FirstOrDefault();
                    val.kota = Request.Form["kota"].FirstOrDefault();

                    if (Request.Form.Files.Count() != 0)
                    {
                        if (val.file_sp != null)
                        {
                            string ExitingFile = environment.WebRootPath + "/" + val.file_sp;
                            System.IO.File.Delete(ExitingFile);
                        }

                        IFormFile postedFile = Request.Form.Files[0];
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + postedFile.FileName;
                        string path = environment.WebRootPath + "/upload/paket_jasa/" + fileName;

                        using (var stream = System.IO.File.Create(path))
                        {
                            postedFile.CopyTo(stream);
                            val.file_sp = "upload/paket_jasa/" + fileName;
                        }

                    }
                    else
                    {
                        val.file_sp = Request.Form["file_"].FirstOrDefault();
                    }

                    DateTime a = Convert.ToDateTime(Request.Form["deadline"].FirstOrDefault()).Date;
                    DateTime b = Convert.ToDateTime(Request.Form["target_terbit_sp"].FirstOrDefault()).Date;
                    val.t_light = (a - b).Days;


                    val.target_penunjukan_pemenang = (Request.Form["target_penunjukan_pemenang"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["target_penunjukan_pemenang"].FirstOrDefault()).Date : null;
                    val.target_jawaban_sanggah = (Request.Form["target_jawaban_sanggah"].FirstOrDefault() != "") ?  Convert.ToDateTime(Request.Form["target_jawaban_sanggah"].FirstOrDefault()).Date : null;
                    val.target_pengajuan_sanggah = (Request.Form["target_pengajuan_sanggah"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["target_pengajuan_sanggah"].FirstOrDefault()).Date : null;
                    val.target_pengumuman_pemenang = (Request.Form["target_pengumuman_pemenang"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["target_pengumuman_pemenang"].FirstOrDefault()).Date : null;
                    val.target_keputusan_pemenang = (Request.Form["target_keputusan_pemenang"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["target_keputusan_pemenang"].FirstOrDefault()).Date : null;
                    val.target_usulan_pemenang = (Request.Form["target_usulan_penetapan_calon_pemenang"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["target_usulan_penetapan_calon_pemenang"].FirstOrDefault()).Date : null;
                    val.target_negosiasi = (Request.Form["target_negosiasi"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["target_negosiasi"].FirstOrDefault()).Date : null;
                    val.target_evaluasi = (Request.Form["target_penetapan_evaluasi_penawaran"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["target_penetapan_evaluasi_penawaran"].FirstOrDefault()).Date : null;
                    val.target_pembukaan = (Request.Form["target_pembukaan_penawaran"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["target_pembukaan_penawaran"].FirstOrDefault()).Date : null;
                    val.targetBukaPH = (Request.Form["target_penyampaian_dokumen"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["target_penyampaian_dokumen"].FirstOrDefault()).Date : null;
                    val.target_pemberian = (Request.Form["target_pemberian_penjelasan"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["target_pemberian_penjelasan"].FirstOrDefault()).Date : null;
                    val.target_undangan = (Request.Form["target_undangan"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["target_undangan"].FirstOrDefault()).Date : null;
                    val.target_prakualifikasi = (Request.Form["target_prakualifikasi"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["target_prakualifikasi"].FirstOrDefault()).Date : null;
                    val.target_sertifikasi = (Request.Form["target_sertifikasi"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["target_sertifikasi"].FirstOrDefault()).Date : null;
                    val.target_pengumuman = (Request.Form["target_pengumuman_pendaftaran"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["target_pengumuman_pendaftaran"].FirstOrDefault()).Date : null;
                    val.targetCO = (Request.Form["target_kirim_paket_ke_co"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["target_kirim_paket_ke_co"].FirstOrDefault()).Date : null;
                    val.target_persetujuan = (Request.Form["target_persetujuan"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["target_persetujuan"].FirstOrDefault()).Date : null;
                    val.target_oe = (Request.Form["target_susun_oe"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["target_susun_oe"].FirstOrDefault()).Date : null;
                    val.target_kak = (Request.Form["target_susun_kak"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["target_susun_kak"].FirstOrDefault()).Date : null;


                    val.akt_penunjukan_pemenang = (Request.Form["aktual_penunjukan_pemenang"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["aktual_penunjukan_pemenang"].FirstOrDefault()).Date : null;
                    val.akt_jawaban_sanggah = (Request.Form["aktual_jawaban_sanggah"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["aktual_jawaban_sanggah"].FirstOrDefault()).Date : null;
                    val.akt_pengajuan_sanggah = (Request.Form["aktual_pengajuan_sanggah"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["aktual_pengajuan_sanggah"].FirstOrDefault()).Date : null;
                    val.akt_pengumuman_pemenang = (Request.Form["aktual_pengumuman_pemenang"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["aktual_pengumuman_pemenang"].FirstOrDefault()).Date : null;
                    val.akt_keputusan_pemenang = (Request.Form["aktual_keputusan_pemenang"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["aktual_keputusan_pemenang"].FirstOrDefault()).Date : null;
                    val.akt_usulan_pemenang = (Request.Form["aktual_usulan_penetapan_calon_pemenang"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["aktual_usulan_penetapan_calon_pemenang"].FirstOrDefault()).Date : null;
                    val.akt_negosiasi = (Request.Form["aktual_negosiasi"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["aktual_negosiasi"].FirstOrDefault()).Date : null;
                    val.akt_evaluasi = (Request.Form["aktual_penetapan_evaluasi_penawaran"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["aktual_penetapan_evaluasi_penawaran"].FirstOrDefault()).Date : null;
                    val.akt_pembukaan = (Request.Form["aktual_pembukaan_penawaran"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["aktual_pembukaan_penawaran"].FirstOrDefault()).Date : null;
                    val.aktualBukaPH = (Request.Form["aktual_penyampaian_dokumen"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["aktual_penyampaian_dokumen"].FirstOrDefault()).Date : null;
                    val.akt_pemberian = (Request.Form["aktual_pemberian_penjelasan"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["aktual_pemberian_penjelasan"].FirstOrDefault()).Date : null;
                    val.akt_undangan = (Request.Form["aktual_undangan"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["aktual_undangan"].FirstOrDefault()).Date : null;
                    val.akt_prakualifikasi = (Request.Form["aktual_prakualifikasi"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["aktual_prakualifikasi"].FirstOrDefault()).Date : null;
                    val.akt_sertifikasi = (Request.Form["aktual_sertifikasi"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["aktual_sertifikasi"].FirstOrDefault()).Date : null;
                    val.akt_pengumuman = (Request.Form["aktual_pengumuman_pendaftaran"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["aktual_pengumuman_pendaftaran"].FirstOrDefault()).Date : null;
                    val.aktualCO = (Request.Form["aktual_kirim_paket_ke_co"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["aktual_kirim_paket_ke_co"].FirstOrDefault()).Date : null;
                    val.akt_persetujuan = (Request.Form["akt_persetujuan"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["akt_persetujuan"].FirstOrDefault()).Date : null;
                    val.akt_oe = (Request.Form["akt_persetujuan"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["aktual_susun_oe"].FirstOrDefault()).Date : null;
                    val.akt_kak = (Request.Form["aktual_susun_kak"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["aktual_susun_kak"].FirstOrDefault()).Date : null;


                    val.aktualSP = (Request.Form["aktual_proses_spb"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["aktual_proses_spb"].FirstOrDefault()).Date : null;
                    val.currStat = (Request.Form["current"].FirstOrDefault() != "") ? Request.Form["current"].FirstOrDefault() : null;
                    val.currStatDesc = Request.Form["ket_current"].FirstOrDefault();

                    val.mulai = (Request.Form["start"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["start"].FirstOrDefault()).Date : null;
                    val.selesai = (Request.Form["end"].FirstOrDefault() != "") ? Convert.ToDateTime(Request.Form["end"].FirstOrDefault()).Date : null;
                    val.persiapan = Convert.ToInt32(Request.Form["preparing"].FirstOrDefault());
                    val.fabrikasi = Convert.ToInt32(Request.Form["fabrikasi"].FirstOrDefault());
                    val.mech = Convert.ToInt32(Request.Form["mechanical"].FirstOrDefault());
                    val.finishing = Convert.ToInt32(Request.Form["finishing"].FirstOrDefault());
                    val.pemeliharaan = Convert.ToInt32(Request.Form["maintenance"].FirstOrDefault());

                    using var transaction = _context.Database.BeginTransaction();
                    try
                    {
                        _context.SaveChanges();
                        if (val.WO != "")
                        {
                            Joblist_Detail jobplan = _context.joblist_Detail.Where(p => p.no_jasa == id_).FirstOrDefault();
                            if (jobplan != null)
                            {
                                if (Convert.ToString(val.aktualCO) != "")
                                {
                                    jobplan.status_jasa = "COMPLETED";
                                }
                                else
                                {
                                    jobplan.status_jasa = "NOT_COMPLETED";
                                }
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
                   
                }
                else
                {
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
		public async Task<IActionResult> RelatedJoblist()
        {
            try {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();

                var start = Request.Form["start"].FirstOrDefault();

                var length = Request.Form["length"].FirstOrDefault();

                var sortColumn = Request.Form["columns[" + Request.Form["order[1][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                var sortColumnDirection = Request.Form["order[1][dir]"].FirstOrDefault();

                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                var customerData = (from j in _context.joblist
                                    join jd in _context.joblist_Detail on j.id equals jd.joblist_id
                                    join e in _context.equipments on j.id_eqTagNo equals e.id
                                    select new
                                    {
                                        id = jd.id,
                                        eqTagNo = e.eqTagNo,
                                        jobNo = j.jobNo,
                                        jobDesc = jd.jobDesc,
                                        no_jasa = jd.no_jasa,
                                        deleted = j.deleted,
                                    }).Where(p => p.no_jasa == Convert.ToInt32(Request.Form["id_contract"].FirstOrDefault()) && p.deleted == 0);


                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.eqTagNo.StartsWith(searchValue));
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
            } catch
            {
                throw;
            }
        }

		[AuthorizedAction]
		public IActionResult getDataTarget()
        {
            try
            {
                var durasi = _context.durasi.Where(p => p.id_project == Convert.ToInt64(Request.Form["id_project"].FirstOrDefault()) && p.kat_tender == Request.Form["kattender"].FirstOrDefault()).FirstOrDefault();
                 
                if (durasi != null)
                {
                    var target_sp = Convert.ToDateTime(Request.Form["target_sp"].FirstOrDefault()).Date;
                    var target_penunjukan_pemenang = Convert.ToDateTime(Request.Form["target_sp"].FirstOrDefault()).AddDays(-Convert.ToDouble(durasi.proses_spb)) ;
                    var target_jawaban_sanggah = Convert.ToDateTime(target_penunjukan_pemenang).AddDays(-Convert.ToDouble(durasi.tunjuk_pemenang));
                    var target_pengajuan_sanggah = Convert.ToDateTime(target_jawaban_sanggah).AddDays(-Convert.ToDouble(durasi.jawaban_sanggah));
                    var target_pengumuman_pemenang = Convert.ToDateTime(target_pengajuan_sanggah).AddDays(-Convert.ToDouble(durasi.pengajuan_sanggah));
                    var target_keputusan_pemenang = Convert.ToDateTime(target_pengumuman_pemenang).AddDays(-Convert.ToDouble(durasi.pengumuman_pemenang));
                    var target_usulan_pemenang = Convert.ToDateTime(target_keputusan_pemenang).AddDays(-Convert.ToDouble(durasi.keputusan));
                    var target_negosiasi = Convert.ToDateTime(target_usulan_pemenang).AddDays(-Convert.ToDouble(durasi.usulan));
                    var target_evaluasi = Convert.ToDateTime(target_negosiasi).AddDays(-Convert.ToDouble(durasi.negosiasi));
                    var target_pembukaan = Convert.ToDateTime(target_evaluasi).AddDays(-Convert.ToDouble(durasi.evaluasi));
                    var target_penyampaian = Convert.ToDateTime(target_pembukaan).AddDays(-Convert.ToDouble(durasi.pembukaan));
                    var target_pemberian = Convert.ToDateTime(target_penyampaian).AddDays(-Convert.ToDouble(durasi.penyampaian));
                    var target_undangan = Convert.ToDateTime(target_pemberian).AddDays(-Convert.ToDouble(durasi.pemberian));
                    var target_prakualifikasi = Convert.ToDateTime(target_undangan).AddDays(-Convert.ToDouble(durasi.undangan));
                    var target_sertifikasi = Convert.ToDateTime(target_prakualifikasi).AddDays(-Convert.ToDouble(durasi.prakualifikasi));
                    var target_pengumuman = Convert.ToDateTime(target_sertifikasi).AddDays(-Convert.ToDouble(durasi.sertifikasi));
                    var target_co = Convert.ToDateTime(target_pengumuman).AddDays(-Convert.ToDouble(durasi.pengumuman_pendaftaran));
                    var target_persetujuan = Convert.ToDateTime(target_co).AddDays(-Convert.ToDouble(durasi.kirim_ke_co));
                    var target_oe = Convert.ToDateTime(target_persetujuan).AddDays(-Convert.ToDouble(durasi.persetujuan));
                    var target_kak = Convert.ToDateTime(target_oe).AddDays(-Convert.ToDouble(durasi.susun_oe));

                    return Json(new
                    {
                        result = true,
                        target_spb = target_sp,
                        target_penunjukan_pemenang = target_penunjukan_pemenang,
                        target_jawaban_sanggah = target_jawaban_sanggah,
                        target_pengajuan_sanggah = target_pengajuan_sanggah,
                        target_pengumuman_pemenang = target_pengumuman_pemenang,
                        target_keputusan_pemenang = target_keputusan_pemenang,
                        target_usulan_pemenang = target_usulan_pemenang,
                        target_negosiasi = target_negosiasi,
                        target_evaluasi = target_evaluasi,
                        target_pembukaan = target_pembukaan,
                        target_penyampaian = target_penyampaian,
                        target_pemberian = target_pemberian,
                        target_undangan = target_undangan,
                        target_prakualifikasi = target_prakualifikasi,
                        target_sertifikasi = target_sertifikasi,
                        target_pengumuman = target_pengumuman,
                        target_co = target_co,
                        target_persetujuan = target_persetujuan,
                        target_oe = target_oe,
                        target_kak = target_kak,
                    });
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
		public IActionResult deleteWoJasa()
        {
            try
            {
                Joblist_Detail jobplan = _context.joblist_Detail.Where(p => p.id == Convert.ToInt64(Request.Form["id"].FirstOrDefault())).FirstOrDefault();
                if(jobplan != null)
                {
                    jobplan.no_jasa = null;
                    jobplan.order_jasa = null;
                    jobplan.pekerjaan = null;
                    jobplan.status_jasa = "NOT_PLANNED";
                    if (jobplan.jasa != 0 || jobplan.all_in_kontrak != 0 || jobplan.material != 0)
                    {
                       

                        if (jobplan.status_jasa != "")
                        {
                            jobplan.status_job = jobplan.status_jasa;
                        }

                        if (jobplan.jasa != 0 || jobplan.material != 0)
                        {
                            if (jobplan.status_material == "COMPLETED" && jobplan.status_jasa != "COMPLETED")
                            {
                                jobplan.status_job = "NOT_COMPLETED";
                            }

                            if (jobplan.status_jasa == "COMPLETED" && jobplan.status_material != "COMPLETED")
                            {
                                jobplan.status_job = "NOT_COMPLETED";
                            }

                            if (jobplan.status_jasa != "COMPLETED" && jobplan.status_material != "COMPLETED")
                            {
                                jobplan.status_job = "NOT_COMPLETED";
                            }

                            if (jobplan.status_jasa == "NOT_PLANNED" && jobplan.status_material == "NOT_PLANNED")
                            {
                                jobplan.status_job = "NOT_COMPLETED";
                            }

                            if (jobplan.status_jasa == "COMPLETED" && jobplan.status_material == "COMPLETED")
                            {
                                jobplan.status_job = "COMPLETED";
                            }
                        }
                    }
                    else
                    {
                        jobplan.status_job = "NOT_IDENTIFY";
                    }

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
        public IActionResult Progress()
        {
            ViewBag.role = "MANAGE_CONTRACT";
            if (Module.hasModule("MANAGE_CONTRACT", HttpContext.Session))
            {
                ViewBag.user = _context.users.Where(p => p.locked != 1).Where(p => p.statPekerja == "PLANNER").Where(p => p.alias != null && p.alias != "").Where(p => p.status == "PEKERJA").ToList();
                ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == 1).ToList();
                ViewBag.kategori_paket = _context.contractItem.Where(p => p.item_group == "KATEGORIPAKET").ToList();
                

                return View();
            }
            else
            {
                return NotFound();
            }


        }


        [AuthorizedAction]
        public IActionResult Progress_()
        {
            try
            {

                var project_filter = Request.Form["project_filter"].FirstOrDefault();
                var kategori_paket_filter = Request.Form["kategori_paket_filter"].FirstOrDefault();
                var pic_filter = Request.Form["pic_filter"].FirstOrDefault();

                var query = _context.contractTracking.Where(p => p.deleted == 0);

                if (!string.IsNullOrEmpty(project_filter))
                {
                    var pi = long.Parse(project_filter);
                    query = query.Where(p => p.projectID == pi);
                }

                if (!string.IsNullOrEmpty(kategori_paket_filter))
                {
                    query = query.Where(p => p.kategoriPaket == kategori_paket_filter);
                }

                if (!string.IsNullOrEmpty(pic_filter))
                {
                    query = query.Where(p => p.pic == pic_filter);
                }

                var data = query.ToList();
                var table_progres = "";
                if (data != null)
                {
                    foreach (var d in data)
                    {
                        var isi = "";
                        if (d.aktualSP != null)
                        {
                            isi = "<span class='text-primary'><i class='fa fa-circle fs-20px fa-fw '></i></span>";
                        }
                        else
                        {
                            if (d.t_light > 30)
                            {
                                isi = "<span class='text-green-600 text-center'><i class='fa fa-circle fs-20px fa-fw'></i></span>";
                            }
                            else if (d.t_light <= 30 && d.t_light > 20)
                            {
                                isi = "<span class='text-warning text-center'><i class='fa fa-circle fs-20px fa-fw '></i></span>";
                            }
                            else if (d.t_light <= 20)
                            {
                                isi = "<span class='text-danger text-center'><i class='fa fa-circle fs-20px fa-fw'></i></span>";
                            }
                            else
                            {
                                isi = "-";
                            }
                        }
                        var file = "";
                        if (d.file_sp != null)
                        {
                            file = "<a href='" + d.file_sp + "' class='btn btn-info btn-sm' target='_blank'>File</a>";
                        }
                        else
                        {
                            file = "";
                        }

                        table_progres += "<tr><td rowspan = '2'>" + d.WO + "<br>" + d.po + "<br>" + d.PR + "<br>" + d.noSP + "<br>" + file + "</td>" +
                            "<td rowspan = '2'>" + d.judulPekerjaan + "</td>" +
                            "<td rowspan = '2' style = 'text-align: center;'>" + isi + "<br>" + d.pic + "</td>" +
                            "<td> P </td>" +
                            "<td style = 'text-align: center;  --bs-table-accent-bg: #ffcc80;'>" + d.target_kak + "</td>" +
                            "<td style = 'text-align: center;  --bs-table-accent-bg: #ffcc80;'>" + d.target_persetujuan + "</td>" +
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
                            "<td style = 'text-align: center;'>" + d.akt_persetujuan + "</td>" +
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
                            "<td style = 'text-align: center;'>" + d.akt_usulan_pemenang + "</td>" +
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
                    table_progres += "<tr>" +
                                "<td colspan = '24' style = 'text-align:center'> Data tidak ditemukan</td>" +
                           "</tr>";
                }

                return Ok(table_progres);

            }
            catch
            {
                throw;
            }

        }
        
        public FileResult ExportToPDF(string gridHtml)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(gridHtml)))
            {
                ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
                PdfWriter writer = new PdfWriter(byteArrayOutputStream);
                PdfDocument pdfDocument = new PdfDocument(writer);
                pdfDocument.SetDefaultPageSize(iText.Kernel.Geom.PageSize.A3.Rotate());
                HtmlConverter.ConvertToPdf(stream, pdfDocument);
                pdfDocument.Close();
                return File(byteArrayOutputStream.ToArray(), "application/pdf", "Grid.pdf");
            }
        }
    }
}
