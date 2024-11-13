using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using mystap.Models;
using System.Linq.Dynamic;
using System.Linq.Dynamic.Core;


namespace mystap.Controllers
{
    public class AwalController : Controller
    {
        private readonly DatabaseContext _context;
        public AwalController(DatabaseContext context, IWebHostEnvironment environment)
        {
            _context = context;
        }

        [AuthorizedAction]
		public IActionResult Awal()
        {
            ViewBag.role = "";
            ViewBag.project = _context.project.Where(p => p.deleted == 0 && p.active == 1).ToList();
            return View();
        }

        [AuthorizedAction]
        public async Task<IActionResult> DataPlanner()
        {
            try
            {
                var id = HttpContext.Session.GetInt32("id");
                var status = HttpContext.Session.GetString("status");
                var statPekerja = HttpContext.Session.GetString("statPekerja");
                var project = Request.Form["project_filter"].FirstOrDefault();

                if(status == "PEKERJA" && statPekerja == "PLANNER")
                {
                    var sql = (from jd in _context.joblist_Detail
                               join u in _context.users on jd.pic equals u.id into Users
                               from u in Users.DefaultIfEmpty()
                               join j in _context.joblist on jd.joblist_id equals j.id into Joblist
                               from j in Joblist.DefaultIfEmpty()
                               join p in _context.project on j.projectID equals p.id into Project
                               from p in Project.DefaultIfEmpty()
                               select new
                               {
                                   id_user = u.id,
                                   id_project = p.id,
                                   alias = u.alias,
                                   not_planned = (jd.status_job == "NOT_IDENTIFY" ? (long?)jd.id : null),
                                   not_completed = (jd.status_job == "NOT_COMPLETED" ? (long?)jd.id : null),
                                   completed = (jd.status_job == "COMPLETED" ? (long?)jd.id : null),
                                   deleted = jd.deleted
                               }).Where(p => p.deleted == 0 && p.id_user == id);

                    if (project != "")
                    {
                        sql = sql.Where(w => w.id_project == Convert.ToInt32(project));
                    }

                    var data = await sql.GroupBy(g => new
                            {
                                g.id_user
                            }).Select(z => new
                            {
                                id_user = z.Key.id_user,
                                not_planned = z.Select(p => p.not_planned).Distinct().Count(),
                                not_completed = z.Select(p => p.not_completed).Distinct().Count(),
                                completed = z.Select(p => p.completed).Distinct().Count(),
                            }).FirstOrDefaultAsync();

                    return Json(data);
                }
                else
                {
                    return Json(new { not_planned = 0, not_completed = 0, completed = 0 });
                }
            }
            catch
            {
                throw;
            }
        }

    }
}
