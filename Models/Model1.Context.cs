﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class botEntities3 : DbContext
    {
        public botEntities3()
            : base("name=botEntities3")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Form> Form { get; set; }
        public virtual DbSet<OrderRecycle> OrderRecycle { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Recycle> Recycle { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Sub> Sub { get; set; }
        public virtual DbSet<TelegramUser> TelegramUser { get; set; }
        public virtual DbSet<Token> Token { get; set; }
        public virtual DbSet<Users> Users { get; set; }
    }
}
