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
using System;
using Newtonsoft.Json;

namespace telegramBod.Controllers
{

    public class AdminController : Controller
    {
        [HttpPost]
        public ActionResult DeleteShop(string email)
        {
          
                using (botEntities3 bd = new botEntities3())
                {

                try
                {
                    List<Users> users = bd.Users.ToList();
                    List<Token> tokens = bd.Token.ToList();
                    List<Category> category = bd.Category.ToList();
                    List<Product> product = bd.Product.ToList();
                    List<Recycle> recyc = bd.Recycle.ToList();



                    Users user = bd.Users.Where(x => x.Email == email).FirstOrDefault();

                    Token token = bd.Token.Where(x => x.UserID == user.Id).FirstOrDefault();

                    TelegramUser tguser = bd.TelegramUser.Where(x => x.UserId == user.Id).FirstOrDefault();
                    List<OrderRecycle> rs = null;
                    try
                    {
                        OrderRecycle or = bd.OrderRecycle.Where(x => x.TokenId == token.Id).FirstOrDefault();
                        if (or != null)
                            rs = bd.OrderRecycle.Where(x => x.TokenId == token.Id).ToList();
                    }
                    catch
                    {

                    }
                    List<Category> category_remove = new List<Category>();
                    List<Product> product_remove = new List<Product>();

                    if (category != null)
                        try
                        {
                            foreach (Category i in category)
                                if (i.TokenId == token.Id)
                                {
                                    category_remove.Add(i);
                                    foreach (Product prod in product)

                                        if (prod.CategoryId == i.CategoryId)
                                            product_remove.Add(prod);
                                }
                        }
                        catch
                        {

                        }


                    if (product_remove != null)
                        try
                        {
                            foreach (Product prod in product_remove)
                                bd.Product.Remove(prod);
                        }
                        catch
                        {

                        }
                    if (category_remove != null)
                    {
                        try
                        {
                            foreach (Category cat in category_remove)
                                bd.Category.Remove(cat);
                        }
                        catch { }
                    }

                    if (recyc != null)
                        try
                        {
                            foreach (Recycle rec in recyc)
                            {
                                if (rec.TokenId == token.Id)
                                    bd.Recycle.Remove(rec);
                            }
                        }
                        catch
                        {

                        }
                    if(rs!=null)
                    foreach(OrderRecycle j in rs)
                    {
                        bd.OrderRecycle.Remove(j);
                    }


                    if(tguser!=null)
                    bd.TelegramUser.Remove(tguser);
                    if(token!=null)
                    bd.Token.Remove(token);
                  
                    bd.Users.Remove(user);

                    bd.SaveChanges();
                }
                 catch
                {

                }
                return RedirectToAction("AShowShops");

            }
        }
        public ActionResult AShowShops()
        {
            CustomRoleProvider l = new CustomRoleProvider();
               bool con = l.IsUserInRole(User.Identity.Name, "Admin");
            if (!con)
                return View("Index", "Home");

            using (botEntities3 bd = new botEntities3())
            {
                try { 
                List<ShopsModel> shops = new List<ShopsModel>();

                List<Users> users = bd.Users.ToList();
                List<Token> tokens = bd.Token.ToList();
                List<Category> category = bd.Category.ToList();
                List<Product> product = bd.Product.ToList();

                List<Category> category_list = new List<Category>();
                List<Product> product_list = new List<Product>();

                    foreach (Users temp_user in users)
                    {
                        string k = null;
                        try
                        {
                          k= temp_user.TelegramUser.Where(x => x.UserId == temp_user.Id).FirstOrDefault().BotChannel;
                        }
                        catch
                        {
                            if (k == null)
                                k = " ";
                        }
                        if (k == null)
                            k = " ";
                        string name = temp_user.Email;
                        string nameShop = k;
                        Token temp_tokens = new Token();
                        for (int i = 0; i < tokens.Count; i++)
                        {
                            if (temp_user.Id == tokens.ElementAt(i).UserID)
                            {
                                temp_tokens = tokens.ElementAt(i);
                                break;
                            }

                        }
                        category_list = new List<Category>();
                        product_list = new List<Product>();
                        foreach (Category temp_category in category)
                        {
                            if (temp_tokens.Id == temp_category.TokenId)
                            {
                                foreach (Product prod in product)

                                    if (prod.CategoryId == temp_category.CategoryId)
                                        product_list.Add(prod);


                            }
                        }

                        shops.Add(new ShopsModel(nameShop,name, product_list));
                    
                    }
                    return View(shops);

                }
                catch
                {

                }
                return View();
            }
        }
        public ActionResult AddElement()
        {
            CustomRoleProvider l = new CustomRoleProvider();
            bool con = l.IsUserInRole(User.Identity.Name, "User");
            if (!con)
                return View("Index", "Home");

            return View();
        }
        [HttpPost]
        public ActionResult AddElement(string category,string nameproduct,string description,string price,string count, HttpPostedFileBase upload)
        {
            Users user;
            Token k;
            using (botEntities3 bd = new botEntities3())
            {
                string u = User.Identity.Name;
                user = bd.Users.Where(x => x.Email == u).FirstOrDefault();
                k = bd.Token.Where(x => x.UserID == user.Id).FirstOrDefault();
            }
            string o = "";
            string path = " ";
            string fileName = " ";
            string ps = "";
            try
            {
                if (upload != null)
                {
                    // получаем имя файла


                    fileName = System.IO.Path.GetFileName(upload.FileName);
                    Random rnd = new Random();
                    ps = "";
                    for (int i = 0; i < 3; i++)
                    {
                        ps += (char)rnd.Next('a', 'z');
                    }
                    // сохраняем файл в папку Files в проекте
                    ps = ps + fileName;

                    path = Path.Combine(Server.MapPath("~/Images"), ps);

                    upload.SaveAs(path);
                }
     ;
                string path1 = $@"<a href=""http://botshop.azurewebsites.net/Images/{ps}""> </a>";
                using (botEntities3 db = new botEntities3())
                {


                    Category c = db.Category.Where(x => x.NameCategory == category).Where(x => x.TokenId == k.Id).FirstOrDefault();

                    if (c == null)
                    {
                        c = new Category() { NameCategory = category, CategoryId = user.Id, TokenId = k.Id };
                        db.Category.Add(c);
                        db.SaveChanges();
                    }

                    db.Product.Add(new Product() { Category = c, CategoryId = user.Id, ProductDescription = description, ProductName = nameproduct, ProductPrice = int.Parse(price), ProductPhoto = path1, Counts = int.Parse(count) });
                    db.SaveChanges();
                }
                return View("AddElementS");
            }
            catch
            {
                return View("AddElementF");
            }
         
            
        }
        [HttpGet]
        public ActionResult Move(string name)
        {
          return  Redirect(@"https://t.me/" + name);
        }
        [HttpPost]
        public ActionResult DeleteMes(int id)
        {
            using (botEntities3 bd = new botEntities3())
            {
               Form k = bd.Form.Where(x => x.Id == id).FirstOrDefault();
                bd.Form.Remove(k);
                bd.SaveChanges();
            }
           return RedirectToAction("ShowForm");
        }

        public ActionResult SendToAllUsers()
        {


            return View();
        }
        [HttpPost]
        public ActionResult SendToAllUsers(string title, string text)
        {

            return View("SendToAllUsers2");
        }
        public ActionResult ShowSubscribers()
        {
            using (botEntities3 bd = new botEntities3())
            {
                bool con = false;
                //если это админ или обычный пользователь
                CustomRoleProvider l = new CustomRoleProvider();
                con = l.IsUserInRole(User.Identity.Name, "Users");
                if (!con)
                {
                    List<Sub> subcribers = bd.Sub.ToList();
                    return View(subcribers);
                }

                else return View("Index", "Home");
            }

        }
        [HttpPost]
        public ActionResult DeleteEmail(string email)
        {
            using (botEntities3 bd = new botEntities3())
            {
                Sub subscrib = bd.Sub.Where(x => x.Email == email).First();
                bd.Sub.Remove(subscrib);
                bd.SaveChanges();
                return RedirectToAction("ShowSubscribers");

            }
        }
     


        public ActionResult ShowForm()
        {
            using (botEntities3 bd = new botEntities3())
            {

                bool con = false;
                //если это админ или обычный пользователь
                CustomRoleProvider l = new CustomRoleProvider();
                con = l.IsUserInRole(User.Identity.Name, "Admin");
                if (con)
                {
                    List<Form> mes = bd.Form.ToList();
                    return View(mes);
                }

                else return View("Index", "Home");
            }

        }

        public ActionResult Andex()
        {
            CustomRoleProvider l = new CustomRoleProvider();
            bool admin = l.IsUserInRole(User.Identity.Name, "Admin");
            if(!admin)
                return RedirectToAction("Index", "Home");
            return View();
        }
        public ActionResult ShowShop()
        {
            List<Parser> parse = new List<Parser>();
            CustomRoleProvider l = new CustomRoleProvider();
            bool con = l.IsUserInRole(User.Identity.Name, "User");
            bool admin = l.IsUserInRole(User.Identity.Name, "Admin");
            if (admin)
                return RedirectToAction("Andex", "Admin");
            if (!con)
                return RedirectToAction("Index", "Home");

            using (botEntities3 bd = new botEntities3())
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

            using (botEntities3 bd = new botEntities3())
            {
                string u = User.Identity.Name;
                Users user = bd.Users.Where(x => x.Email == u).First();
                Token m = bd.Token.Where(x => x.UserID == user.Id).First();
                
                Product p = bd.Product.Where(x => x.ProductName == namep).Where(x => x.Category.NameCategory == namecat).Where(x => x.Category.TokenId == m.Id).FirstOrDefault();
                
                if (p != null)
                {
                    if (p.Counts > 0)
                    {
                        p.Counts = p.Counts - 1;
                        bd.SaveChanges();
                    }
                    if(p.Counts==0)
                    {
                        bd.Product.Remove(p);
                        bd.SaveChanges();
                    }
                }
                Category cat = bd.Category.Where(x => x.NameCategory == namecat).Where(x => x.TokenId == m.Id).First();
                if (cat.Product.Count==0)
                {
                    bd.Category.Remove(cat);
                    bd.SaveChanges();
                    return RedirectToAction("ShowShop", "Admin");
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
                using (botEntities3 bd = new botEntities3())
                {

                    string u = User.Identity.Name;
                    Users user = bd.Users.Where(x => x.Email == u).First();
                    List<Token> k = bd.Token.Where(x => x.UserID == user.Id).ToList();

                foreach (var z in k)
                    res.Add(bd.Recycle.Where(x => x.TokenId == z.Id).ToList());

                }
            
            return View(res);
        }
         public ActionResult ShowOrder()
         {
            CustomRoleProvider l = new CustomRoleProvider();
            bool con = l.IsUserInRole(User.Identity.Name, "User");
            if (!con)
                return RedirectToAction("Index", "Home");
            List<List<OrderRecycle>> res = new List<List<OrderRecycle>>();
            using (botEntities3 bd = new botEntities3())
            {
                string u = User.Identity.Name;
                Users user = bd.Users.Where(x => x.Email == u).First();
                List<Token> k = bd.Token.Where(x => x.UserID == user.Id).ToList();
                foreach (var z in k)
                    res.Add(bd.OrderRecycle.Where(x => x.TokenId == z.Id).ToList());
            }
            return View(res);
        }


        [HttpPost]
        public ActionResult DeleteFromOrderRecycle(string namep, string namecat, string username)
        {
            try
            {
                CustomRoleProvider l = new CustomRoleProvider();
                bool con = l.IsUserInRole(User.Identity.Name, "User");
                if (!con)
                    return RedirectToAction("Index", "Home");

                using (botEntities3 bd = new botEntities3())
                {
                    string u = User.Identity.Name;
                    Users user = bd.Users.Where(x => x.Email == u).First();
                    Token m = bd.Token.Where(x => x.UserID == user.Id).First();
                    OrderRecycle res = bd.OrderRecycle.Where(x => x.NameProduct == namep).Where(x => x.NameCategory == namecat).Where(x => x.TokenId == m.Id).Where(x => x.UserName == username).First();
                    bd.OrderRecycle.Remove(res);
                    bd.SaveChanges();
                }
            }
            catch
            {

            }
            return RedirectToAction("OrderRecycle", "Admin");
        }
        [HttpPost]
        public ActionResult DeleteFromRecycle(string namep,string namecat,string username)
        {
            try
            {
                CustomRoleProvider l = new CustomRoleProvider();
                bool con = l.IsUserInRole(User.Identity.Name, "User");
                if (!con)
                    return RedirectToAction("Index", "Home");

                using (botEntities3 bd = new botEntities3())
                {
                    string u = User.Identity.Name;
                    Users user = bd.Users.Where(x => x.Email == u).First();
                    Token m = bd.Token.Where(x => x.UserID == user.Id).First();
                    Recycle res = bd.Recycle.Where(x => x.NameProduct == namep).Where(x => x.NameCategory == namecat).Where(x => x.TokenId == m.Id).Where(x => x.UserName == username).First();
                    bd.Recycle.Remove(res);
                    bd.SaveChanges();
                }
            }
            catch
            {

            }
            return RedirectToAction("ShowRecycle", "Admin");
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
            using (botEntities3 bd = new botEntities3())
            {
                string u = User.Identity.Name;
                user = bd.Users.Where(x => x.Email == u).First();
                k = bd.Token.Where(x => x.UserID == user.Id).FirstOrDefault();
            }
            string o = "";

            if (upload != null)
            {
                // получаем имя файла
                string fileName = System.IO.Path.GetFileName(upload.FileName);

                // сохраняем файл в папку Files в проекте

                string path = Path.Combine(Server.MapPath("~/Images"), fileName);

                upload.SaveAs(path);
                o = ImportFile(user.Id, k.Id, fileName);

            }
                SetWebHook(k.token1,k.Id);
                return View("ImportSucess");
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
            using (botEntities3 bd = new botEntities3())
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
            return View("ToAllSuccess");
        }
        string ImportFile(int userId,int tokenID,string fileName)
        {
            try
            {
                var package = new ExcelPackage(new FileInfo(Path.Combine(Server.MapPath("~/Images"), fileName)));

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


                    using (botEntities3 db = new botEntities3())
                    {
                        Users user = db.Users.Where(x => x.Id == userId).First();
                        Token k = db.Token.Where(x => x.UserID == user.Id).FirstOrDefault();
                        Category c = db.Category.Where(x => x.NameCategory == ie.Category).Where(x=>x.TokenId==k.Id).FirstOrDefault();

                        if (c == null)
                        {
                            c = new Category() { NameCategory = ie.Category, CategoryId = user.Id, TokenId = tokenID };
                            db.Category.Add(c);
                            db.SaveChanges();
                        }

                        db.Product.Add(new Product() { Category = c, CategoryId = user.Id, ProductDescription = ie.Description, ProductName = ie.Name, ProductPrice = ie.Price, ProductPhoto = ie.Photo, Counts = ie.Count });
                        db.SaveChanges();
                    }
                }
            }
            catch
            {

            }
            return "ок";
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
        //public string Subscribe()
        //{
           
        //    string token;
        //    using (botEntities2 bot = new botEntities2())
        //        token = bot.Token.Where(x => x.Id == 6).First().token1;
        //    SetWebHook(token, 6);
        //    return "Установли";
        //}
        //public string Subscribe2()
        //    { 
        ////{
        ////    SetWebHook();
        //    return "ух";
        //}
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