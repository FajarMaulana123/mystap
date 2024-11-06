using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using mystap.Models;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;


namespace mystap.Helpers
{
	public class Module
	{
		public static bool isLogin(ISession session)
		{
			var js = session.GetString("username");
			if(JsonConvert.DeserializeObject(session.GetString("admin_modules")) != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        public static bool hasModule(string mdl, ISession session)
        {
            var c = JsonConvert.DeserializeObject<List<string>>(session.GetString("admin_modules"));
            var b = Array.Find(c.ToArray(), s => s == mdl);

            if (b == mdl)
            {
                session.SetString("module", mdl);
                return true;
            }
            else
            {
                session.SetString("module", "");
                return false;
            }
        }

        public static bool forModule(string mdl, ISession session)
		{
			var c = JsonConvert.DeserializeObject<List<string>>(session.GetString("admin_modules"));
			var b = Array.Find(c.ToArray(), s => s == mdl);

			if (b == mdl)
			{
				session.SetString("module", mdl);
				return true;
			}
			else
			{
				session.SetString("module", "");
				return false;
			}
		}
	}
}
