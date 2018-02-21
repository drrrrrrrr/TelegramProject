using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using telegramBod.Models;

namespace telegramBod.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Andex()
        {
            return View();
        }
        public string Subscribe()
        {
           
            string token;
            using (botEntities bot = new botEntities())
                token = bot.Token.Where(x => x.Id == 4).First().token1;
            SetWebHook(token, 4);
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