using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.MSIdentity.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using mystap.Models;
using Newtonsoft.Json;
using NuGet.Packaging;
using System.Data;
using System.Linq.Dynamic;
using System.Linq.Dynamic.Core;


namespace mystap.Controllers
{
    public class TestController : Controller
    {
        private readonly DatabaseContext _context;
        public TestController(DatabaseContext context)
        {
            _context = context;
        }
        public IActionResult Plans()
        {

            return View();
        }
        
        public IActionResult Get_Plans()
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
                // Getting all Customer data                //var query = "SELECT plans.*, users.name as created_by FROM plans left join users on users.id = plans.createdBy where plans.deleted = 0";


                var customerData = _context.plans.Include("users").Where(s => s.deleted == 0).Select(p => new {id = p.id,plans = p.plans, planDesc = p.planDesc, created_by = p.users.name, createdDate = p.createdDate});
                // Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m=> m.planDesc.StartsWith(searchValue));
                }
                // Total number of rows count
                Console.WriteLine(customerData);
                recordsTotal = customerData.Count();
                // Paging
                var data = customerData.Skip(skip).Take(pageSize).ToList();
                // Returning Json Data
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult Create_Plant (IFormCollection formcollaction)
        {
            try
            {
                Plans plans = new Plans();
                plans.plans = formcollaction["plans"];
                plans.planDesc = formcollaction["planDesc"];
                plans.createdBy = 1;
                plans.createdDate = DateTime.Now;
                plans.deleted = 0;

                Boolean t;
                if (plans != null)
                {
                    _context.plans.Add(plans);
                    _context.SaveChanges();
                    t = true;
                }
                else
                {
                    t = false;
                }

                //MAKE DB CALL and handle the response
                
                return Json(new { result = t });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult Update_plant (Plans plans)
        {
            try
            {
                int id = Int32.Parse(Request.Form["hidden_id"].FirstOrDefault());
                Plans obj = _context.plans.Where(p => p.id == id).FirstOrDefault();


                if (obj != null)
                {
                    obj.plans = Request.Form["plans"].FirstOrDefault();
                    obj.planDesc = Request.Form["planDesc"].FirstOrDefault();
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

        public IActionResult Delete_plant(Plans plans)
        {
            try
            {
                int id = Int32.Parse(Request.Form["id"].FirstOrDefault());
                Plans obj = _context.plans.Where(p => p.id == id).FirstOrDefault();


                if (obj != null)
                {
                    obj.deleted = 1;
                    _context.SaveChanges();

                    return Json(new { title = "Sukses!", icon = "success", status = "Berhasil Dihapus!" });
                }

                return Json(new { title = "Maaf!", icon = "error", status = "Tidak Dapat di Hapus!, Silahkan Hubungi Administrator " });
            }
            catch
            {
                throw;
            }
        }
    }
}
