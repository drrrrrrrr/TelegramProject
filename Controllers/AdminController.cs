using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using telegramBod.Models;
using telegramBod.Providers;

namespace telegramBod.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            CustomRoleProvider l = new CustomRoleProvider();
            bool con = l.IsUserInRole(User.Identity.Name, "User");
            if (!con)
                return RedirectToAction("Index", "Home");
            using (botEntities1 db = new botEntities1())
            {
                
                string u = User.Identity.Name;
                Users user = db.Users.Where(x => x.Email == u).First();
               
            }
                return View();
        }
        public ActionResult Andex()
        {
            return View();
        }
        public string Subscribe()
        {
           
            string token;
            using (botEntities1 bot = new botEntities1())
                token = bot.Token.Where(x => x.Id == 6).First().token1;
            SetWebHook(token, 6);
            return "Установли";
        }
        static string SetWebHook(string token, int i)
        {

            string BaseUrl = "https://api.telegram.org/bot";
            string adress = BaseUrl + token + "/setWebhook";
            NameValueCollection nvc = new NameValueCollection();
            WebClient client = new WebClient();
            string addresTo = @"https://botshop.azurewebsites.net/api/message/update/" + i;
            nvc.Add("url", addresTo);
            client.UploadValues(adress, nvc);
            return addresTo;
        }
    }
}