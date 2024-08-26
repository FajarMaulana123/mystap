﻿using Azure.Core;
using Humanizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using mystap.Models;
using System;
using System.Globalization;
using System.IO;
using System.Linq.Dynamic.Core;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            ViewBag.userAccount = _context.users.Where(p => p.locked != 1).Where(p => p.statPekerja == "PLANNER").Where(p => p.status == "PEKERJA").Where(p => p.alias != null && p.alias != "").ToList();
            ViewBag.project = _context.project.Where(p => p.deleted == 0 && p.active == "1").ToList();
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
                    select += "<option value='"+val.sub_group +"' data-urut='"+val.urut+"'>" + val.sub_group + "</ option >";
                }

                return base.Content(select, "text/html");
            }
            catch
            {
                throw;
            }
        }
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


        public IActionResult Create_Sow(IFormCollection formcollaction)
        {
            try
            {
                

                //Fetch the File Name.
              

                string tahun = formcollaction["tahun"];
                string inisial = formcollaction["inisial"];
                var cek = _context.sow.Where(p => p.tahun == tahun).Where(w => w.tahun.Contains(tahun)).Select(p => new
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

                    string tahun = Request.Form["tahun"].FirstOrDefault();
                    string inisial = Request.Form["inisial"].FirstOrDefault();
                    var cek = _context.sow.Where(p => p.tahun == tahun).Where(w => w.tahun.Contains(tahun)).Max(p => new { kode = p.jobCode, tahun = p.tahun });
                    int no = 0;
                    if (cek.kode != tahun)
                    {
                        no = 1;
                    }
                    else
                    {
                        no = Convert.ToInt32(cek.kode) + 1;
                    }

                    obj.jobCode = Request.Form["inisial"].FirstOrDefault() + "-" + no.ToString("D3");
                    obj.noSOW = Request.Form["event"].FirstOrDefault() + "-" + no.ToString("D3") + "-" + Request.Form["inisial"].FirstOrDefault() + Request.Form["urut"].FirstOrDefault() + "-" + Request.Form["area"].FirstOrDefault() + "/" + Request.Form["codeKabo"].FirstOrDefault() + "/" + tahun.Substring(tahun.Length - 2); ;
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
                            obj.file = "upload/bom/" + fileName;
                        }

                    }
                    else
                    {
                        obj.file = Request.Form["file_"].FirstOrDefault();
                    }
                  
                    obj.modifyBy = "Rama";
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

        public IActionResult ContractTracking()
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
      
       
        public IActionResult DurasiStep()
        {
            ViewBag.project = _context.project.Where(p => p.deleted == 0).Where(p => p.active == "1").ToList();
            return View();
        }
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
        public IActionResult Create_DurasiStep(IFormCollection formcollaction)
        {
            try
            {
                Durasi durasi = new Durasi();
                durasi.id_project = Convert.ToInt64(formcollaction["id_project"]);
                durasi.kat_tender = formcollaction["kat_tender"];
                durasi.susun_kak = Convert.ToInt32(formcollaction["susun_kak"]);
                durasi.susun_oe = Convert.ToInt32(formcollaction["susun_oe"]);
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
        public IActionResult Update_DurasiStep(Durasi durasi)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Durasi obj = _context.durasi.Where(p => p.id == id).FirstOrDefault();

                if (obj != null)
                {

                    obj.id_project = Convert.ToInt64(Request.Form["id_project"].FirstOrDefault());
                    obj.kat_tender = Request.Form["kat_tender"].FirstOrDefault();
                    obj.susun_kak = Convert.ToInt32(Request.Form["susun_kak"].FirstOrDefault());
                    obj.susun_oe = Convert.ToInt32(Request.Form["susun_oe"].FirstOrDefault());
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
                    return Json(new { Results = true });
                }
                return Json(new { Results = false });
            }
            catch
            {
                throw;
            }
        }

       
    }
}
