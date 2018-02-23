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
            //SendPhotoIputFile();
            //Thread.Sleep(10000);
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
        async public Task SendPhotoIputFile()
        {
            string pathToPhoto = HostingEnvironment.MapPath("~/Images/01.jpg");
            string BaseUrl = "https://api.telegram.org/bot";
            long id = 378999844;
            string answer = "sadf";
            using (MultipartFormDataContent form = new MultipartFormDataContent())
            {
                string url = BaseUrl + "529156147:AAF1nyFwPKMw606JZhlFUrm9MUbaNizQ3rc" + "/sendPhoto";
                string fileName = pathToPhoto.Split('\\').Last();
                //if (replyMarkup != "")
                //    form.Add(new StringContent(replyMarkup, Encoding.UTF8), "reply_markup");
                form.Add(new StringContent(id.ToString(), Encoding.UTF8), "chat_id");
                form.Add(new StringContent(answer.ToString(), Encoding.UTF8), "caption");
                using (FileStream fileStream = new FileStream(pathToPhoto, FileMode.Open, FileAccess.Read))
                {
                    form.Add(new StreamContent(fileStream), "photo", fileName);
                    using (HttpClient client = new HttpClient())
                       await client.PostAsync(url, form);
                }
            }
            
        }
        string ReceiveToken(Update update, int? id)
        {
            string token;
            using (botEntities2 bot = new botEntities2())
                token = bot.Token.Where(x => x.Id == id).First().token1;
            return token;
        }

    }
}