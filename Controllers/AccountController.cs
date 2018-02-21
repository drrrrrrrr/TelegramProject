using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using telegramBod.Models;
using telegramBod.Providers;

namespace telegramBod.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                CustomRoleProvider l = new CustomRoleProvider();
                bool con = l.IsUserInRole(User.Identity.Name, "User");
                if (con)
                    return RedirectToAction("Index", "Admin");
                else return RedirectToAction("Andex", "Admin"); 
            }
            if (ModelState.IsValid)
            {
                // поиск пользователя в бд
                Users user = null;
                using (botEntities db = new botEntities())
                {
                    user = db.Users.FirstOrDefault(u => u.Email == model.Name && u.Passwords == model.Password);
                    string a;
                }
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(model.Name, true);
                    
                    if(user.Roles.Names=="User")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Andex", "Admin");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Пользователя с таким логином и паролем нет");
                }
            }

            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                int count;
                Users user = null;
                using (botEntities db = new botEntities())
                {
                    user = db.Users.FirstOrDefault(u => u.Email == model.Name);
                }
                if (user == null)
                {
                    // создаем нового пользователя
                    using (botEntities db = new botEntities())
                    {
                        count = db.Users.Count() + 1;
                        db.Users.Add(new Users { Email = model.Name, Passwords = model.Password, RoleId = 1, Id = count });

                        db.SaveChanges();
                        user = db.Users.Where(u => u.Email == model.Name && u.Passwords == model.Password).FirstOrDefault();
                        //    db.SaveChanges();
                    }
                    // если пользователь удачно добавлен в бд
                    if (user != null)
                    {
                        FormsAuthentication.SetAuthCookie(model.Name, true);
                        return RedirectToAction("Index", "Admin");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь с таким логином уже существует");
                }
            }

            return View(model);
        }
    }
}