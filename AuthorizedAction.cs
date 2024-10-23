using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace mystap
{
	public class AuthorizedAction: ActionFilterAttribute
	{
		public override void OnResultExecuting(ResultExecutingContext filterContext)
		{

		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);

			if (filterContext.HttpContext.Session.GetString("id") == null)
			{
				filterContext.Result = new RedirectToRouteResult(
					new RouteValueDictionary { { "controller", "Auth" }, { "action", "Login" } });
				return;
			}
		}
	}
}
