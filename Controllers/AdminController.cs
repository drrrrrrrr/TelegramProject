using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using telegramBod.Models;
using telegramBod.Providers;

using System.IO;
using OfficeOpenXml;

namespace telegramBod.Controllers
{

    public class AdminController : Controller
    {
        // GET: Admin
        //public ActionResult Index()
        //{
        //    CustomRoleProvider l = new CustomRoleProvider();
        //    bool con = l.IsUserInRole(User.Identity.Name, "User");
        //    if (!con)
        //        return RedirectToAction("Index", "Home");
        //    using (botEntities2 db = new botEntities2())
        //    {
                
        //        string u = User.Identity.Name;
        //        Users user = db.Users.Where(x => x.Email == u).First();
               
        //    }
        //    RedirectToAction("Admin", "ShowShop");
        //        return View();
        //}
        public ActionResult Andex()
        {
            return View();
        }
        public ActionResult ShowShop()
        {
            List<Parser> parse = new List<Parser>();
            CustomRoleProvider l = new CustomRoleProvider();
            bool con = l.IsUserInRole(User.Identity.Name, "User");
            if (!con)
                return RedirectToAction("Index", "Home");

            using (botEntities2 bd = new botEntities2())
            {
                string u = User.Identity.Name;
                Users user = bd.Users.Where(x => x.Email == u).First();
                List<Token> k = bd.Token.Where(x => x.UserID == user.Id).ToList();
                 
                
                foreach (Token m in k)
                {
                    List<Category> list = m.Category.ToList();
                    for (int i = 0; i < list.Count; i++)
                    {
                        Parser px = new Parser(list[i]);
                        px.p = list[i].Product.ToList();
                        parse.Add(px);
                    }
                
                }
               
            }
            return View(parse);
        }
        [HttpPost]
        public ActionResult ShowShop(string namep,string namecat)
        {

            using (botEntities2 bd = new botEntities2())
            {
                string u = User.Identity.Name;
                Users user = bd.Users.Where(x => x.Email == u).First();
                List<Token> k = bd.Token.Where(x => x.UserID == user.Id).ToList();
                foreach (Token m in k)
                {
                    Product p = bd.Product.Where(x => x.ProductName == namep).Where(x => x.Category.NameCategory == namecat).Where(x => x.Category.TokenId == m.Id).FirstOrDefault();
                    if (p != null)
                    {
                        bd.Product.Remove(p);
                        bd.SaveChanges();
                        return RedirectToAction("ShowShop", "Admin");
                    }
                }
            }
                return RedirectToAction("ShowShop", "Admin");
        }
        public ActionResult ShowRecycle()
            {

                CustomRoleProvider l = new CustomRoleProvider();
                bool con = l.IsUserInRole(User.Identity.Name, "User");
                if (!con)
                    return RedirectToAction("Index", "Home");
                List<List<Recycle>> res = new List<List<Recycle>>();
                using (botEntities2 bd = new botEntities2())
                {

                    string u = User.Identity.Name;
                    Users user = bd.Users.Where(x => x.Email == u).First();
                    List<Token> k = bd.Token.Where(x => x.UserID == user.Id).ToList();

                foreach (var z in k)
                    res.Add(bd.Recycle.Where(x => x.TokenId == z.Id).ToList());

                }
            
            return View(res);
        }
        public ActionResult ImportFromExcel()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ImportFromExcel(HttpPostedFileBase upload)
        {
            Users user;
            Token k;
            using (botEntities2 bd = new botEntities2())
            {
                string u = User.Identity.Name;
                user = bd.Users.Where(x => x.Email == u).First();
                k = bd.Token.Where(x => x.UserID == user.Id).FirstOrDefault();
            }
            if (upload != null)
            {
                // получаем имя файла
                string fileName = System.IO.Path.GetFileName(upload.FileName);

                // сохраняем файл в папку Files в проекте
                upload.SaveAs(Server.MapPath("~/Files/" + fileName));
                string o =ImportFile(user.Id,k.Id,fileName);
                if(o=="ok")
                {
                    SetWebHook(k.token1, user.Id);
                }
            }
            return RedirectToAction("ShowShop");
        }
        public ActionResult ToAll()
        {
           

            return View();
        }
        void SendMessage(string token,string message,string chat_id)
        {
            string BaseUrl = "https://api.telegram.org/bot";
            string address = BaseUrl + token + "/sendMessage";
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("chat_id", chat_id.ToString());
            nvc.Add("text", message);
            nvc.Add("parse_mode", "HTML");
            using (WebClient client = new WebClient())
                client.UploadValues(address, nvc);
        }
        [HttpPost]
        public ActionResult ToAll(string title,string text)
        {
            using (botEntities2 bd = new botEntities2())
            {
                string u = User.Identity.Name;
                Users user = bd.Users.Where(x => x.Email == u).First();
                Token k = bd.Token.Where(x => x.UserID == user.Id).FirstOrDefault();
                string username = user.TelegramUser.Where(x => x.UserId == user.Id).FirstOrDefault().Username;
                List<Recycle> rec = bd.Recycle.Where(x => x.TokenId == k.Id).ToList();
                string split = "";
                if (username != null)
                {
                    
                    for (int i = 0; i < rec.Count; i++)
                    {
                        if (split.Contains(rec[i].UserName))
                            continue;
                        split += rec[i].UserName;
                        SendMessage(k.token1, text, rec[i].UserName);
                    }
                }
            }
            return View();
        }
        string ImportFile(int userId,int tokenID,string fileName)
        {
            var package = new ExcelPackage(new FileInfo(Server.MapPath("~/Files/" + fileName)));

            ExcelWorksheet workSheet = package.Workbook.Worksheets[1];


            for (int i = workSheet.Dimension.Start.Row + 2; i <= workSheet.Dimension.End.Row; i++)
            {
                int j = workSheet.Dimension.Start.Column + 1;

                ImportElement ie = new ImportElement();
                ie.Category = workSheet.Cells[i, j].Value.ToString();
                ie.Name = workSheet.Cells[i, ++j].Value.ToString();
                string s = workSheet.Cells[i, ++j].Value.ToString();
                ie.Price = int.Parse(s);
                ie.Description = workSheet.Cells[i, ++j].Value.ToString();
                ie.Photo = workSheet.Cells[i, ++j].Value.ToString();
                ie.Count = int.Parse(workSheet.Cells[i, ++j].Value.ToString());


                using (botEntities2 db = new botEntities2())
                {
                    Users user = db.Users.Where(x => x.Id == userId).First();
                    Category c = db.Category.Where(x => x.NameCategory == ie.Category).FirstOrDefault();

                    if (c == null)
                    {
                        c = new Category() { NameCategory = ie.Category, CategoryId = user.Id,TokenId=tokenID};
                        db.Category.Add(c);
                        db.SaveChanges();
                    }

                    db.Product.Add(new Product() { Category = c, CategoryId = user.Id, ProductDescription = ie.Description, ProductName = ie.Name, ProductPrice = ie.Price,ProductPhoto=ie.Photo,Counts=ie.Count});
                    db.SaveChanges();
                }
            }
            return "";
        }
        public class ImportElement
        {
            public string Category { get; set; }
            public string Name { get; set; }
            public int Price { get; set; }
            public string Description { get; set; }
            public string Photo { get; set; }
            public int Count { get; set; }
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
        //{
        //    SetWebHook();
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