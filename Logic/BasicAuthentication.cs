using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FactFlux.Logic
{
    public class BasicAuthenticationAttribute : ActionFilterAttribute
    {
        protected string Username { get; set; }
        protected string Password { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var req = filterContext.HttpContext.Request;
            var auth = req.Headers["Authorization"];
            if (!String.IsNullOrEmpty(auth))
            {
                var cred = System.Text.ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(auth.Substring(6))).Split(':');
                var user = new { Name = cred[0], Pass = cred[1] };

                using (var db = new FactFluxEntities())
                {
                    var findUser = db.AspNetUsers.Where(x => x.UserName == user.Name).FirstOrDefault();

                    var userIsAdmin = findUser.AspNetRoles.Where(x => x.Id == "1").Any();

                    if (userIsAdmin)
                    {
                        return;
                    }
                }
            }

            throw new Exception("Not Authorized");
        }
    }
}