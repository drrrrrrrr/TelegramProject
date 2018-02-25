using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace telegramBod.Models
{
    public class LoginModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }


    }
    public class ShowShop
    {
      public  List<List<Product>> p;
      public  List<List<Category>> c;
        public ShowShop(List<List<Product>> pp, List<List<Category>> zz)
        {
            p = pp;
            c = zz;
        }
    }
    public class Parser
    {
        public Category cat;
       
        public List<Product> p;
        public Parser(Category _cat, List<Product> _p)
        {
            cat = _cat;
            p = _p;
        }
        public Parser(Category _cat)
        {
            cat = _cat;
            p = new List<Product>();
        }
    }
}