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
            return View();
        }
        [HttpPost]
        public ActionResult Subscribe(Form con)
        {

            if (con.Email == null ||con.Mesages==null||con.Names==null||con.Titles==null)
                return RedirectToAction("Index", "Home");
            else
            {
                //beginform
                using (botEntities2 bd = new botEntities2())
                {
                    Form form = new Form()
                    {
                        Email = con.Email,
                        Mesages = con.Mesages,
                        Names = con.Names,
                        Titles = con.Titles
                    };
                    bd.Form.Add(form);
                    bd.SaveChanges();
                }
                return RedirectToAction("Index", "Home");
            }

        }
        [HttpPost]
        public ActionResult Subscr(Sub contact)
        {
           
            if (contact.Email == null)
                return RedirectToAction("Index", "Home");
            else
            {
                //beginform
                using (botEntities2 bd = new botEntities2())
                {
                    Sub c = new Sub() { Email = contact.Email };
                    bd.Sub.Add(c);
                    bd.SaveChanges();
                }
                return RedirectToAction("Index", "Home");
            }

        }


    }
}