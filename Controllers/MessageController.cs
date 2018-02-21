
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Results;
using telegramBod.Models;

namespace telegramBod.Controllers
{
    public class MessageController : ApiController
    {
       
        [Route(@"api/message/update/{id}")] //webhook uri part
        public  OkResult Update([FromBody]Update update, int? id)
        {
        //    try
        //    {
        //        SendPhotoIputFile(update, id, HostingEnvironment.MapPath(@"/Images/01.jpg"));
        //        Thread.Sleep(2000);
        //        SendMessage(update.message.chat.id, "yra", ReceiveToken(update, id));
        //    }
        //    catch
        //    {
        //        SendMessage(update.message.chat.id, "sad", ReceiveToken(update, id));
        //    }
            if (id != null)
            {
                if (update.message != null)
                {
                    SendAnswer(update, update.message.chat.id, Text(update), id);
                    return Ok();
                }
                if (update.callback_query != null)
                {
                    AnswerIsQuery(update, id);
                    return Ok();
                }
            }
            
            return Ok();
        }
        string Text(Update up)
        {
            string answer = JsonConvert.SerializeObject(up);
            return answer;
        }
        public void SendAnswer(Update update, long chat_id, string message, int? id)
        {
            string reply_markup = "";
            string answer = "";
           
                answer = MainMenu(update, id, out reply_markup);
            
            //catch
            //{
            //    answer += "  сломался";
            //}
            string token = ReceiveToken(update, id);
            if (update.message.chat.id != 0)
                SendMessage(update.message.chat.id, answer, token, reply_markup);

        }

        static void SendMessage(long chat_id, string message, string token, string reply_markup = "")
        {
            string BaseUrl = "https://api.telegram.org/bot";
            string address = BaseUrl + token + "/sendMessage";
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("chat_id", chat_id.ToString());
            nvc.Add("text", message);
            if (reply_markup != "")
            {
                nvc.Add("reply_markup", reply_markup);
            }
            using (WebClient client = new WebClient())
                client.UploadValues(address, nvc);
        }

        string ReceiveToken(Update update, int? id)
        {
            string token;
            using (botEntities bot = new botEntities())
                token = bot.Token.Where(x => x.Id == id).First().token1;
            return token;
        }
        void ChangeMessage(Update update, string message, int? id, string reply_markup = "")
        {
            string token = ReceiveToken(update, id);
            string BaseUrl = "https://api.telegram.org/bot";

            string adress = BaseUrl + token + "/editMessageText";
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("chat_id", update.callback_query.message.chat.id.ToString());
            nvc.Add("message_id", update.callback_query.message.message_id.ToString());
            nvc.Add("text", message);
            if (reply_markup != "")
                nvc.Add("reply_markup", reply_markup);
            using (WebClient client = new WebClient())
                client.UploadValues(adress, nvc);
        }

        void AnswerIsQuery(Update update, int? id)
        {
            string reply_markup = "";
            string answer = "ll";
            string[] _data = update.callback_query.data.Split(' ');
            switch (_data[0])
            {
                case "?":
                    answer = "Вопрос!";
                    MainMenu(update, id, out reply_markup);
                    break;
                case "about":
                    answer = "Что-нибудь можно написать";
                    MainMenu(update, id, out reply_markup);
                    break;
                case "корзина":
                    answer = Recycle(update, id, out reply_markup);
                    break;
                default:
                    answer = Shop(_data.Length < 1 ? "" : _data[0], _data.Length < 2 ? "" : _data[1], update, id, out reply_markup);
                    break;
            }
            answer += Environment.NewLine + update.callback_query.data;
            ChangeMessage(update, answer, id, reply_markup);
        }
        string Recycle(Update update, int? id, out string reply_markup)
        {
            string answer = "";
            reply_markup = "";
            TelegramRecycle rec = new TelegramRecycle(update, id);
            string[] _data = update.callback_query.data.Split();
            switch (_data[1] ?? "")
            {
                case "покупка":
                    rec.AddBuy(_data[2], _data[3]);
                    answer = Shop(_data[2], _data[3], update, id, out reply_markup);
                    break;
                case "отобразить":
                    answer = rec.ShowMyBuy();
                    InlineKeyboard keyboard = new InlineKeyboard();
                    List<InlineKeyboardButton> line = new List<InlineKeyboardButton>()
                    {
                        new InlineKeyboardButton("Оформить","корзина оформить"),
                        new InlineKeyboardButton("Изменить","корзина изменить")
                    };
                    keyboard.AddLine(line);
                    keyboard.AddButton(new InlineKeyboardButton("Назад", "about"));
                    reply_markup = JsonConvert.SerializeObject(keyboard);
                    break;
                case "изменить":answer = rec.ChangeMyBuy(out reply_markup);
                    break;
                case "удалить":
                    rec.DeleteElement(_data[2], _data[3]);
                    answer = rec.ChangeMyBuy(out reply_markup);
                    break;
                case "оформить":

                default: break;
            }

            // answer = "В корзине" + rec.Count + "  шт";
            //InlineKeyboard keyboard = new InlineKeyboard();
            //keyboard.AddButton(new InlineKeyboardButton("Назад", "about"));
            //AddMainButtons(keyboard);
            //reply_markup = JsonConvert.SerializeObject(keyboard);
            //Shop(_data[2], _data[3], update, id, out reply_markup);
            if (reply_markup == "")
                MainMenu(update, id, out reply_markup);
            return answer;
        }
      
        void AddRecycle(InlineKeyboard keyboard, string nameCategory, string nameProduct, Update update, int? id)
        {
            TelegramRecycle tel = new TelegramRecycle(update, id);
            List<InlineKeyboardButton> line = new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton("Корзина ("+tel.Count+")", "корзина отобразить" ),
            };
            if (nameProduct == "" || nameCategory=="")
            {
                keyboard.AddLine(line);
                return;
            }
            line.Add(new InlineKeyboardButton("Добавить в корзину", "корзина покупка " + nameCategory + " " + nameProduct));            
            keyboard.AddLine(line);
        }
        void AddMainButtons(InlineKeyboard keyboard,Update update,int ? id)
        {
            TelegramRecycle tel = new TelegramRecycle(update, id);
            List<InlineKeyboardButton> line = new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton("Есть вопрос", "?" ),
                new InlineKeyboardButton("О нас", "about" )
            };
            keyboard.AddLine(line);
        }
        string Shop(string category, string nameProduct, Update update, int? id, out string reply_markup)
        {
            //string category = shop.Split(' ')[0];
            //string nameProduct = shop.Split(' ').Length < 2 ? "" : shop.Split(' ')[1];
            string answer = "Привет";
            reply_markup = "";
            List<Category> cat = null;
            List<Product> p = null;
            // SendMessage(update.callback_query.from.id, category , ReceiveToken(update, id));
            using (botEntities bd = new botEntities())
            {
                try
                {
                    cat = bd.Category.Where(x => x.Token.Id == id).ToList();
                    p = cat.Where(x => x.NameCategory == category).First().Product.ToList();
                    if (nameProduct == "") nameProduct = p[0].ProductName;
                }
                catch
                {
                   //   SendMessage(update.callback_query.from.id, "БД УПало", ReceiveToken(update, id));
                }
            }
            InlineKeyboard keyboard = new InlineKeyboard();
            int i = 0;
            foreach (Product k in p)
            {
                if (nameProduct == k.ProductName)
                    continue;
                keyboard.AddButton(new InlineKeyboardButton(

                  k.ProductName, k.Category.NameCategory + " " + k.ProductName), i++ / 2);
            }
            AddRecycle(keyboard, category, nameProduct,update,id);
            Product chooseProduct = p.Where(x => x.ProductName == nameProduct).First();
            answer += chooseProduct.ProductPrice + Environment.NewLine + "  " + " " + chooseProduct.ProductDescription;
            keyboard.AddButton(new InlineKeyboardButton("Назад", "about"));
            AddMainButtons(keyboard,update,id);
            reply_markup = JsonConvert.SerializeObject(keyboard);
            return answer;
        }
        string MainMenu(Update update, int? id, out string reply_markup)
        {
            List<Category> list;
            string token = "";
            int i = 0;
            using (botEntities bd = new botEntities())
            {
                list = bd.Category.Where(x => x.Token.Id == id).ToList();
                token = ReceiveToken(update, id);
            }
            InlineKeyboard keyboard = new InlineKeyboard();
            foreach (Category k in list)
            {
                keyboard.AddButton(new InlineKeyboardButton(
                    k.NameCategory
                    ), i++ / 2);
            }
            AddRecycle(keyboard,"","",update, id);
            AddMainButtons(keyboard,update,id);
            
            reply_markup = JsonConvert.SerializeObject(keyboard);
            return "Все категории";
        }
        async public Task SendPhotoIputFile(Update update,int ? id ,string pathToPhoto, string catprion = "w")
        {
            string BaseUrl = "https://api.telegram.org/bot";
            using (MultipartFormDataContent form = new MultipartFormDataContent())
            {
                string url = BaseUrl + "530915522:AAEvYP3k4CwGmbLMZ41TlPSBouOXU_di1m4" + "/sendPhoto";
                string fileName = pathToPhoto.Split('\\').Last();

                form.Add(new StringContent(update.message.from.id.ToString(), Encoding.UTF8), "chat_id");
                form.Add(new StringContent(catprion.ToString(), Encoding.UTF8), "caption");
                using (FileStream fileStream = new FileStream(pathToPhoto, FileMode.Open, FileAccess.Read))
                {
                    form.Add(new StreamContent(fileStream), "photo", fileName);
                    using (HttpClient client = new HttpClient())
                        await client.PostAsync(url, form);
                }
            }
        }
    }

    class TelegramRecycle
    { 
        string username;
        int TokenIds;
        public int? Count { get; private set; }
        public TelegramRecycle(Update update, int? id)
        {
            if(update.callback_query==null)
            this.username = update.message.from.id.ToString();
            else
                this.username = update.callback_query.from.id.ToString();
            TokenIds = (int)id;
            using (botEntities bd = new botEntities())
            {
                Count = bd.Recycle.Where(x => x.UserName == username).Where(x => x.TokenId == TokenIds).ToList().Count;
            }
        }
        public void DeleteElement(string category,string product)
        {
            using (botEntities bd = new botEntities())
            {
                bd.Recycle.Remove(bd.Recycle.Where(x => x.NameProduct == product).Where(x => x.NameCategory == category).First());
                bd.SaveChanges();
            }
        }
        public string ChangeMyBuy(out string reply_markup)
        {
            reply_markup = "";
            List<Recycle> p;
            List<Product> m = new List<Product>();
            string[] ar;
            using (botEntities bd = new botEntities())
            {
                p = bd.Recycle.Where(x => x.TokenId == TokenIds).Where(x => x.UserName == username).ToList();
                 ar = new string[p.Count];
                for (int i = 0; i < p.Count; i++)
                {
                    string namecat = p[i].NameCategory;
                    string nameprod = p[i].NameProduct;
                    Product k = bd.Product.Where(x => x.ProductName == nameprod).Where(x => x.Category.NameCategory == namecat).First();
                    ar[i] = k.Category.NameCategory;
                    m.Add(k);
                }
            }

            if (m.Count == 0) return "Выберете пункт меню";
            InlineKeyboard keyboard = new InlineKeyboard();
            
                for(int i =0;i<p.Count;i++)
                {
                    keyboard.AddButton(new InlineKeyboardButton("Удалить "+ m[i].ProductName+ "("+ar[i]+")"+" цена   "+ m[i].ProductPrice+
                        "     1 шт","корзина удалить "+ar[i]+" "+m[i].ProductName));
                }
           
            keyboard.AddButton(new InlineKeyboardButton("Оформить", "корзина оформить"));
            keyboard.AddButton(new InlineKeyboardButton("Назад", "about"));
            reply_markup = JsonConvert.SerializeObject(keyboard);
            return "Изменение заказа";
        }
        public void AddBuy(string NameCategorys, string NameProducts)
        {
            using (botEntities bd = new botEntities())
            {
                bd.Recycle.Add(new Recycle()
                {
                    NameCategory = NameCategorys,
                    NameProduct = NameProducts,
                    TokenId = TokenIds,
                    UserName = username
                });
                bd.SaveChanges();
            }
        }
        public string ShowMyBuy()
        {
            string answer = "Ваш заказ: \n\n";
            List<Recycle> p;
            int count = 0;
            List<Product> m = new List<Product>();
            using (botEntities bd = new botEntities())
            {
                p = bd.Recycle.Where(x => x.TokenId == TokenIds).Where(x => x.UserName == username).ToList();
                for (int i = 0; i < p.Count; i++)
                {
                    string namecat = p[i].NameCategory;
                    string nameprod = p[i].NameProduct;
                    Product k = bd.Product.Where(x => x.ProductName == nameprod).Where(x => x.Category.NameCategory == namecat).First();
                    m.Add(k);
                }
            }
            foreach (Product v in m)
            {
                answer += v.ProductName + "  " + v.ProductPrice;
                answer += Environment.NewLine;
                count += v.ProductPrice;
            }
            answer += "Итого  " + count.ToString();
            return answer;
        }
    }
}

