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
            using (botEntities2 db = new botEntities2())
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
        public ActionResult ShowShop()
        {
            CustomRoleProvider l = new CustomRoleProvider();
            bool con = l.IsUserInRole(User.Identity.Name, "User");
            if (!con)
                return RedirectToAction("Index", "Home");
            List<List<Category>> y = new List<List<Category>>();
            List<List<Product>> product = new List<List<Product>>();
            using (botEntities2 bd = new botEntities2())
            {
                string u = User.Identity.Name;
                Users user = bd.Users.Where(x => x.Email == u).First();
                List<Token> k = bd.Token.Where(x => x.UserID == user.Id).ToList();
                 
                
                foreach (Token m in k)
                {
                    y.Add(m.Category.ToList());

                    for (int i = 0; i < y.Count; i++)
                    {
                        product.Add(m.Category.ToList()[i].Product.ToList());
                    }
                    
                }
            }
            ViewBag.Products = product;
            return View(y);
        }
        public ActionResult ShowOrder()
        {

            CustomRoleProvider l = new CustomRoleProvider();
            bool con = l.IsUserInRole(User.Identity.Name, "User");
            if (!con)
                return RedirectToAction("Index", "Home");
            using (botEntities2 bd = new botEntities2())
            {

                string u = User.Identity.Name;
                Users user = bd.Users.Where(x => x.Email == u).First();
                List<Token> k = bd.Token.Where(x => x.UserID == user.Id).ToList();

                List<string> text = new List<string>();
                foreach (Token m  in k)
                {
                  
                    List<Recycle> rec = bd.Recycle.Where(x => x.Token.Id == m.Id).ToList();
                        for (int i = 0; i < rec.Count; i++)
                        {
                            
                        }
                }
                
            }
            return View();
        }
        public string Subscribe()
        {
           
            string token;
            using (botEntities2 bot = new botEntities2())
                token = bot.Token.Where(x => x.Id == 6).First().token1;
            SetWebHook(token, 6);
            return "Установли";
        }
        public string Subscribe2()
        {
            SetWebHook();
            return "ух";
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
        static string SetWebHook()
        {
            string token = "523345948:AAFptjJ0J17eBQynKKmNPp9Jbhvb3y8UA8A";
            string BaseUrl = "https://api.telegram.org/bot";
            string adress = BaseUrl + token + "/setWebhook";
            NameValueCollection nvc = new NameValueCollection();
            WebClient client = new WebClient();
            string addresTo = @"https://botshop.azurewebsites.net/api/register/update/";
            nvc.Add("url", addresTo);
            client.UploadValues(adress, nvc);
            return addresTo;
        }
    }
}