using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using telegramBod.Models;

namespace telegramBod.Controllers
{
    [Route(@"api/register/update/")]
    public class RegisterController : ApiController
    {
        private string Register(Update update)
        {
           
            // поиск пользователя в бд
            Users user = null;
            string answer = "";
            using (botEntities2 db = new botEntities2())
            {
                try
                {
                    Token tok = db.Token.Where(x => x.token1 == update.message.text).FirstOrDefault();
                    if (tok != null)
                        return "Такой токен уже есть в нашей системе";
                    TelegramUser u = db.TelegramUser.Where(x => x.Username == update.message.from.id.ToString()).First();
                    if(u!=null)
                     user = db.Users.FirstOrDefault(x => x.Id == u.UserId);
                }
                catch
                {
                    user = null;
                }
 
            }
            if (user != null)
            {
                using (botEntities2 db = new botEntities2())
                {
                    Token t = new Token();
                    t.token1 = update.message.text;
                    t.UserID = user.Id;
                    db.Token.Add(t);
                    db.SaveChanges();
                }
                answer += "Магазин успешно добавлен";
                return answer;

            }
            else
            {
                string em = "";
                string pas = "";

                Random ran = new Random();
                for (int i = 0; i < 4; i++)
                {
                    em += (char)ran.Next('a', 'z');
                    pas += (char)ran.Next('a', 'z');
                }
                try
                {
                    int count;
                    using (botEntities2 db = new botEntities2())
                    {
                        count = db.Users.Count() + 1;
                        em += db.Users.Count().ToString() + "@bot.ru";
                        user = new Users()
                        {
                            Email = em,
                            RoleId = 1,
                            Passwords = pas,
                            Id = count
                        };
                        db.Users.Add(user);
                        db.SaveChanges();
                    }
                    using (botEntities2 db = new botEntities2())
                    {
                        TelegramUser t = new TelegramUser();
                        //  count = db.Users.Where(x => x.Id == count).First().;
                        t.UserId = count;
                        t.Username = update.message.from.id.ToString();
                        db.TelegramUser.Add(t);
                        db.SaveChanges();
                    }
                    using (botEntities2 db = new botEntities2())
                    {


                        Token k = new Token();
                        k.token1 = update.message.text;
                        k.UserID = count;
                        db.Token.Add(k);
                        db.SaveChanges();
                    }
                    return "Регистрация прошла успешно" + Environment.NewLine + "  Ваш логин:" + " " + em + "Ваш пароль " + pas + Environment.NewLine
                     + "Используйте эти данные для входа в систему" + @"http://botshop.azurewebsites.net/Account/Login";

                }
                catch
                {
                    return "Неверные входные данные";
                }
                //   answer = "Вы успешно зарегистрировались" + " " + "Ваш логин " + em;
            }
            }
        
        public OkResult Update([FromBody]Update update)
        {
                if (update.message != null)
                {
                    if (update.message.text != null)
                    {
                        
                        
                        SendAnswer(update,update.message.text);
                        return Ok();
                    }
                }
                if (update.callback_query != null)
                {
                    AnswerIsQuery(update);
                    return Ok();
                }
            

            return Ok();
        }
        void AnswerIsQuery(Update update)
        {
            string reply_markup = "";
            string answer = "ll";
            string[] _data = update.callback_query.data.Split(' ');
            switch (_data[0])
            {
      
                case "reg":
                    answer = "Введите токен";
                   // MainMenu(update, id, out reply_markup);
                    break;
                case "voc":
                    answer = RecivePassword(update);
                    MainMenu(update,  out reply_markup);
                    break;
                case "about":
                    answer = "Здравствуй! Ты уже совсем близок к созданию своего Telegram-магазина!" +
                       Environment.NewLine +
                                         "Следуй инструкциям и достигнешь цели! " +
                                          Environment.NewLine +
                                           "1 .Перейди в @BotFather и напиши /newbot " +
                                            Environment.NewLine +
                                           " 2.Придумай название для своего магазина " +
                                            Environment.NewLine +
                                           " Например: FruitShop_Bot или FruitShopBot " +
                                            Environment.NewLine +
                                           " 3.Появившийся токен скопируй и отправь нашему боту ";
                    MainMenu(update, out reply_markup);
                    break;
            }
            ChangeMessage(update, answer,  reply_markup);
        }

        private string RecivePassword(Update update)
        {
            try
            {
                using (botEntities2 bd = new botEntities2())
                {
                    TelegramUser tg = bd.TelegramUser.Where(x => x.Username == update.callback_query.from.id.ToString()).FirstOrDefault();
                    if (tg == null)
                        return "Вы еще не зареистрированы";
                    Users users = bd.Users.Where(x => x.Id == tg.UserId).FirstOrDefault();
                    if (users != null)
                    {
                        return "Ваш пароль " + users.Passwords   + "     Ваш логин   " + " " + users.Email;
                    }
                    else
                    {
                        return "Вы еще не зарегистрированы";
                    }

                }
            }
            catch
            {
                return "Вы еще не зареистрированы";
            }
        }

        void AddMainButtons(InlineKeyboard keyboard, Update update)
        {
            List<InlineKeyboardButton> line = new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton("Добавить магазин", "reg" ),
                new InlineKeyboardButton("Восстановить пароль", "voc" )
            };
            keyboard.AddLine(line);
            keyboard.AddButton(new InlineKeyboardButton("Как пользоваться", "about"));
        }
        string MainMenu(Update update, out string reply_markup)
        {
         
         
            InlineKeyboard keyboard = new InlineKeyboard();
            AddMainButtons(keyboard, update);

            reply_markup = JsonConvert.SerializeObject(keyboard);
            return "Все категории";
        }
        public void SendAnswer(Update update, string message)
        {
            string reply_markup = "";
            string answer = "";
            if(update.message.text.Length==45)
            {
                string k = Register(update);
                SendMessage(update.message.chat.id , k ,reply_markup);
                return;
            }
            switch (message.ToLower())
            {
                    case "/start":
                    answer = "Здравствуй! Ты уже совсем близок к созданию своего Telegram-магазина!" +
                        Environment.NewLine+
                                          "Следуй инструкциям и достигнешь цели! " +
                                           Environment.NewLine +
                                            "1 .Перейди в @BotFather и напиши /newbot " +
                                             Environment.NewLine +
                                            " 2.Придумай название для своего магазина " +
                                             Environment.NewLine +
                                            " Например: FruitShop_Bot или FruitShopBot " +
                                             Environment.NewLine +
                                            " 3.Появившийся токен скопируй и отправь нашему боту ";
                    MainMenu(update, out reply_markup);
                    break;

                default:
                    answer = "Выберете пункт из меню";
                    MainMenu(update, out reply_markup);
                    break;
            }
            if (update.message.chat.id != 0)
                SendMessage(update.message.chat.id, answer, reply_markup);

        }

        static void SendMessage(long chat_id, string message,string reply_markup)
        {
            string token = "523345948:AAFptjJ0J17eBQynKKmNPp9Jbhvb3y8UA8A";
            string BaseUrl = "https://api.telegram.org/bot";
            string address = BaseUrl + token + "/sendMessage";
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("chat_id", chat_id.ToString());
            nvc.Add("text", message);
            if (reply_markup != "")
            {
                nvc.Add("reply_markup", reply_markup);
            }
            nvc.Add("parse_mode", "HTML");
            using (WebClient client = new WebClient())
                client.UploadValues(address, nvc);
        }
        void ChangeMessage(Update update, string message,string reply_markup = "")
        {
            string token = "523345948:AAFptjJ0J17eBQynKKmNPp9Jbhvb3y8UA8A";
            string BaseUrl = "https://api.telegram.org/bot";

            string adress = BaseUrl + token + "/editMessageText";
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("chat_id", update.callback_query.message.chat.id.ToString());
            nvc.Add("message_id", update.callback_query.message.message_id.ToString());
            nvc.Add("text", message);
            if (reply_markup != "")
                nvc.Add("reply_markup", reply_markup);
            nvc.Add("parse_mode", "HTML");
            using (WebClient client = new WebClient())
                client.UploadValues(adress, nvc);
        }
    }
}
