﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EFTest
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class LandenStedenTalenEntities : DbContext
    {
        public LandenStedenTalenEntities()
            : base("name=LandenStedenTalenEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Land> Landen { get; set; }
        public virtual DbSet<Stad> Steden { get; set; }
        public virtual DbSet<Taal> Talen { get; set; }
    }
}