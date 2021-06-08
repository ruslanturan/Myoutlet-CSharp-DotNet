using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Myoutlet.ge.Areas.Main.Controllers
{
    public class LoginAuthentication : AuthorizeAttribute, IAuthorizationFilter
    {
        // GET: LoginAuthentication
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                return;
            }
            if(HttpContext.Current.Session["Logged"] == null)
            {
                filterContext.Result = new RedirectResult("/main/Login/login");
            }
        }
    }
}