using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using telegramBod.Models;
using telegramBod.Providers;

namespace telegramBod.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //string result = "Вы не авторизованы";
            //if (User.Identity.IsAuthenticated)
            //{
            //    CustomRoleProvider l = new CustomRoleProvider();
            //    bool con = l.IsUserInRole(User.Identity.Name, "User");
            //    if (con)
            //        return "true";
            //    else return "false";
            //}
            //return result;
            return View();
        }

    }
}