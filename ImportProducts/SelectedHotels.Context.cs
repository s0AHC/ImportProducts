﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ImportProducts
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SelectedHotelsEntities : DbContext
    {
        public SelectedHotelsEntities()
            : base("name=SelectedHotelsEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Feed> Feeds { get; set; }
        public DbSet<ListItem> ListItems { get; set; }
    }
}
