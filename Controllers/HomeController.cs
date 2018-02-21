using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using telegramBod.Models;
using telegramBod.Providers;

namespace telegramBod.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            //string answer = "Ваш заказ: \n\n";
            //int count = 0;
            //long b = 378999844;
            //List<Recycle> p;
            //List<Product> m = new List<Product>();
            //using (botEntities bd = new botEntities())
            //{
            //    p = bd.Recycle.Where(x => x.TokenId==4).Where(x => x.UserName == b.ToString()).ToList();
            //    for (int i = 0; i < p.Count; i++)
            //    {
            //        string namecat = p[i].NameCategory;
            //        string nameprod = p[i].NameProduct;
            //        Product k = bd.Product.Where(x => x.ProductName == nameprod).Where(x => x.Category.NameCategory == namecat).First();
            //        m.Add(k);
            //    }
            //}
            //foreach (Product v in m)
            //{
            //    answer += v.ProductName + "  " + v.ProductPrice;
            //    answer += Environment.NewLine;
            //    count += v.ProductPrice;
            //}
            //answer = "Итого  " + count.ToString();
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
      
        string ReceiveToken(Update update, int? id)
        {
            string token;
            using (botEntities bot = new botEntities())
                token = bot.Token.Where(x => x.Id == id).First().token1;
            return token;
        }

    }
}