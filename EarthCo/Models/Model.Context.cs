﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EarthCo.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class earthcoEntities : DbContext
    {
        public earthcoEntities()
            : base("name=earthcoEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tblAccessLevel> tblAccessLevels { get; set; }
        public virtual DbSet<tblContant> tblContants { get; set; }
        public virtual DbSet<tblController> tblControllers { get; set; }
        public virtual DbSet<tblCustomer> tblCustomers { get; set; }
        public virtual DbSet<tblEmployee> tblEmployees { get; set; }
        public virtual DbSet<tblEstimate> tblEstimates { get; set; }
        public virtual DbSet<tblEstimateItem> tblEstimateItems { get; set; }
        public virtual DbSet<tblEstimateStatu> tblEstimateStatus { get; set; }
        public virtual DbSet<tblIrrigation> tblIrrigations { get; set; }
        public virtual DbSet<tblLog> tblLogs { get; set; }
        public virtual DbSet<tblMenu> tblMenus { get; set; }
        public virtual DbSet<tblPunchlist> tblPunchlists { get; set; }
        public virtual DbSet<tblPunchlistItem> tblPunchlistItems { get; set; }
        public virtual DbSet<tblRole> tblRoles { get; set; }
        public virtual DbSet<tblServiceRequest> tblServiceRequests { get; set; }
        public virtual DbSet<tblSRFile> tblSRFiles { get; set; }
        public virtual DbSet<tblSRItem> tblSRItems { get; set; }
        public virtual DbSet<tblSRStatu> tblSRStatus { get; set; }
        public virtual DbSet<tblSRType> tblSRTypes { get; set; }
        public virtual DbSet<tblUser> tblUsers { get; set; }
        public virtual DbSet<tblSetting> tblSettings { get; set; }
    }
}
