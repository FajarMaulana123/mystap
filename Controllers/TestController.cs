using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using mystap.Models;
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
                var query = "SELECT plans.*, users.name as created_by FROM plans left join users on users.id = plans.createdBy";

                var customerData = _context.plans.FromSqlRaw(query);
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
    }
}
