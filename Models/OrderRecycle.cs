//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace telegramBod.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderRecycle
    {
        public int Id { get; set; }
        public string NameCategory { get; set; }
        public string NameProduct { get; set; }
        public string UserName { get; set; }
        public string Dates { get; set; }
        public int TokenId { get; set; }
    
        public virtual Token Token { get; set; }
    }
}
