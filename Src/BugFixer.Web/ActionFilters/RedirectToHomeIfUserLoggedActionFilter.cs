using Microsoft.AspNetCore.Mvc.Filters;

namespace BugFixer.Web.ActionFilters
{
    public class RedirectToHomeIfUserLoggedActionFilter : ActionFilterAttribute
    {
        override public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.HttpContext.Response.Redirect("/");
            }
        }
    }
}
