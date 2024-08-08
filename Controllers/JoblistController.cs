using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using mystap.Models;
using System.Data;
using System.Linq.Dynamic;
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
        public IActionResult Joblist()
        {
            return View();
        }
        public IActionResult Planning()
        {
            return View();
        }

        public IActionResult Get_Joblist()
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
                //Console.WriteLine(customerData);
                recordsTotal = customerData.Count();
                var data = customerData.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFilter = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult Get_Joblist_Detail()
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
                var customerData = _context.joblist_Detail.Include("contracttracking").Include("joblist").Include("equipments").Include("project").Include("unit").Include("users").Where(s => s.deleted == 0).Select(a => new { id = a.id, eqTagNo = a.equipments.eqTagNo, jobDesc = a.jobDesc, alias = a.users.alias, status = a.freezing, isJasa = a.jasa, noPaket = a.contracttracking.noPaket, judul_paket = a.contracttracking.judulPekerjaan, wo_jasa = a.contracttracking.WO, no_po = a.contracttracking.po, no_sp = a.contracttracking.noSP, status_jasa = a.status_jasa, ismaterial = a.material, order = a.no_order, status_material = a.status_material, ket_status_material = a.ket_status_material, all_in_kontrak = a.all_in_kontrak, lldi = a.lldi, status_job = a.status_job, /*sts_ready = a.status_material,*/ disiplin = a.disiplin });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(b => b.jobDesc.StartsWith(searchValue));
                }
                //Console.WriteLine(customerData);
                recordsTotal = customerData.Count();
                var data = customerData.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFilter = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
