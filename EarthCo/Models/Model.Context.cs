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
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
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
        public virtual DbSet<tblCustomer> tblCustomers { get; set; }
        public virtual DbSet<tblEmployee> tblEmployees { get; set; }
        public virtual DbSet<tblLog> tblLogs { get; set; }
        public virtual DbSet<tblMenu> tblMenus { get; set; }
        public virtual DbSet<tblRole> tblRoles { get; set; }
        public virtual DbSet<tblSRType> tblSRTypes { get; set; }
        public virtual DbSet<tblSetting> tblSettings { get; set; }
        public virtual DbSet<tblCustomerType> tblCustomerTypes { get; set; }
        public virtual DbSet<tblContact> tblContacts { get; set; }
        public virtual DbSet<tblUserType> tblUserTypes { get; set; }
        public virtual DbSet<tblTag> tblTags { get; set; }
        public virtual DbSet<tblTerm> tblTerms { get; set; }
        public virtual DbSet<tblPunchlistItem> tblPunchlistItems { get; set; }
        public virtual DbSet<tblSRItem> tblSRItems { get; set; }
        public virtual DbSet<tblPunchlist> tblPunchlists { get; set; }
        public virtual DbSet<tblIrrigation> tblIrrigations { get; set; }
        public virtual DbSet<tblBillFile> tblBillFiles { get; set; }
        public virtual DbSet<tblEstimateFile> tblEstimateFiles { get; set; }
        public virtual DbSet<tblInvoiceFile> tblInvoiceFiles { get; set; }
        public virtual DbSet<tblPunchlistDetail> tblPunchlistDetails { get; set; }
        public virtual DbSet<tblPurchaseOrderFile> tblPurchaseOrderFiles { get; set; }
        public virtual DbSet<tblSRFile> tblSRFiles { get; set; }
        public virtual DbSet<tblController> tblControllers { get; set; }
        public virtual DbSet<tblMonthlyLandsacpe> tblMonthlyLandsacpes { get; set; }
        public virtual DbSet<tblSyncLog> tblSyncLogs { get; set; }
        public virtual DbSet<tblServiceRequestLatLong> tblServiceRequestLatLongs { get; set; }
        public virtual DbSet<tblWeeklyReport> tblWeeklyReports { get; set; }
        public virtual DbSet<tblWeeklyReportFile> tblWeeklyReportFiles { get; set; }
        public virtual DbSet<tblServiceLocation> tblServiceLocations { get; set; }
        public virtual DbSet<tblServiceRequest> tblServiceRequests { get; set; }
        public virtual DbSet<tblToken> tblTokens { get; set; }
        public virtual DbSet<tblEstimateStatu> tblEstimateStatus { get; set; }
        public virtual DbSet<tblInvoiceStatu> tblInvoiceStatus { get; set; }
        public virtual DbSet<tblPunchlistStatu> tblPunchlistStatus { get; set; }
        public virtual DbSet<tblPurchaseOrderStatu> tblPurchaseOrderStatus { get; set; }
        public virtual DbSet<tblSRStatu> tblSRStatus { get; set; }
        public virtual DbSet<tblPurchaseOrder> tblPurchaseOrders { get; set; }
        public virtual DbSet<tblPurchaseOrderItem> tblPurchaseOrderItems { get; set; }
        public virtual DbSet<tblBill> tblBills { get; set; }
        public virtual DbSet<tblBillItem> tblBillItems { get; set; }
        public virtual DbSet<tblItem> tblItems { get; set; }
        public virtual DbSet<tblEstimate> tblEstimates { get; set; }
        public virtual DbSet<tblEstimateItem> tblEstimateItems { get; set; }
        public virtual DbSet<tblInvoiceItem> tblInvoiceItems { get; set; }
        public virtual DbSet<tblGoolgeCalendar> tblGoolgeCalendars { get; set; }
        public virtual DbSet<tblInvoice> tblInvoices { get; set; }
        public virtual DbSet<tblAccount> tblAccounts { get; set; }
        public virtual DbSet<tblUser> tblUsers { get; set; }
        public virtual DbSet<tblWeeklyReportRCFile> tblWeeklyReportRCFiles { get; set; }
        public virtual DbSet<tblStoreLocation> tblStoreLocations { get; set; }
        public virtual DbSet<tblWeeklyReportRC> tblWeeklyReportRCs { get; set; }
        public virtual DbSet<tblPunchlistPhotoOnly> tblPunchlistPhotoOnlies { get; set; }
        public virtual DbSet<tblPunchlistPhotoOnlyFile> tblPunchlistPhotoOnlyFiles { get; set; }
        public virtual DbSet<tblControllerAuditReport> tblControllerAuditReports { get; set; }
        public virtual DbSet<tblControllerAuditReportFile> tblControllerAuditReportFiles { get; set; }
        public virtual DbSet<tblIrrigationAuditReport> tblIrrigationAuditReports { get; set; }
        public virtual DbSet<tblVerificationCode> tblVerificationCodes { get; set; }
    
        public virtual ObjectResult<SPGetEstimateFileData_Result> SPGetEstimateFileData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetEstimateFileData_Result>("SPGetEstimateFileData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetPurchaseOrderFileData_Result> SPGetPurchaseOrderFileData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetPurchaseOrderFileData_Result>("SPGetPurchaseOrderFileData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetBillFileData_Result> SPGetBillFileData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetBillFileData_Result>("SPGetBillFileData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetInvoiceFileData_Result> SPGetInvoiceFileData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetInvoiceFileData_Result>("SPGetInvoiceFileData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetServiceRequestFileData_Result> SPGetServiceRequestFileData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetServiceRequestFileData_Result>("SPGetServiceRequestFileData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetCustomerContactData_Result> SPGetCustomerContactData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetCustomerContactData_Result>("SPGetCustomerContactData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetPunchlistItemData_Result> SPGetPunchlistItemData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetPunchlistItemData_Result>("SPGetPunchlistItemData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetServiceRequestItemData_Result> SPGetServiceRequestItemData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetServiceRequestItemData_Result>("SPGetServiceRequestItemData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetPunchlistDetailData_Result> SPGetPunchlistDetailData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetPunchlistDetailData_Result>("SPGetPunchlistDetailData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetIrrigationControllerData_Result> SPGetIrrigationControllerData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetIrrigationControllerData_Result>("SPGetIrrigationControllerData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetPunchlistDetailDataById_Result> SPGetPunchlistDetailDataById(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetPunchlistDetailDataById_Result>("SPGetPunchlistDetailDataById", iDParameter);
        }
    
        public virtual ObjectResult<string> SPGetNumber(string type)
        {
            var typeParameter = type != null ?
                new ObjectParameter("Type", type) :
                new ObjectParameter("Type", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("SPGetNumber", typeParameter);
        }
    
        public virtual ObjectResult<SPGetIrrigationData_Result> SPGetIrrigationData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetIrrigationData_Result>("SPGetIrrigationData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetServiceRequestLatLongData_Result> SPGetServiceRequestLatLongData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetServiceRequestLatLongData_Result>("SPGetServiceRequestLatLongData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetMonthlyLandsacpeDataWithCustomer_Result> SPGetMonthlyLandsacpeDataWithCustomer(Nullable<int> customerId, Nullable<int> year, Nullable<int> month)
        {
            var customerIdParameter = customerId.HasValue ?
                new ObjectParameter("CustomerId", customerId) :
                new ObjectParameter("CustomerId", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetMonthlyLandsacpeDataWithCustomer_Result>("SPGetMonthlyLandsacpeDataWithCustomer", customerIdParameter, yearParameter, monthParameter);
        }
    
        public virtual ObjectResult<SPGetMonthlyLandsacpeData_Result> SPGetMonthlyLandsacpeData(Nullable<int> iD, Nullable<int> customerId, Nullable<int> year, Nullable<int> month)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            var customerIdParameter = customerId.HasValue ?
                new ObjectParameter("CustomerId", customerId) :
                new ObjectParameter("CustomerId", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetMonthlyLandsacpeData_Result>("SPGetMonthlyLandsacpeData", iDParameter, customerIdParameter, yearParameter, monthParameter);
        }
    
        public virtual ObjectResult<SPGetWeeklyReportData_Result> SPGetWeeklyReportData(Nullable<int> iD, Nullable<int> customerId, Nullable<int> year, Nullable<int> month)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            var customerIdParameter = customerId.HasValue ?
                new ObjectParameter("CustomerId", customerId) :
                new ObjectParameter("CustomerId", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetWeeklyReportData_Result>("SPGetWeeklyReportData", iDParameter, customerIdParameter, yearParameter, monthParameter);
        }
    
        public virtual ObjectResult<SPGetWeeklyReportListData_Result> SPGetWeeklyReportListData()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetWeeklyReportListData_Result>("SPGetWeeklyReportListData");
        }
    
        public virtual ObjectResult<SPGetWeeklyReportFileData_Result> SPGetWeeklyReportFileData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetWeeklyReportFileData_Result>("SPGetWeeklyReportFileData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetCustomerServiceLocationData_Result> SPGetCustomerServiceLocationData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetCustomerServiceLocationData_Result>("SPGetCustomerServiceLocationData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetServiceRequestData_Result> SPGetServiceRequestData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetServiceRequestData_Result>("SPGetServiceRequestData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetPunchlistData_Result> SPGetPunchlistData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetPunchlistData_Result>("SPGetPunchlistData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetPurchaseOrderData_Result> SPGetPurchaseOrderData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetPurchaseOrderData_Result>("SPGetPurchaseOrderData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetPurchaseOrderItemData_Result> SPGetPurchaseOrderItemData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetPurchaseOrderItemData_Result>("SPGetPurchaseOrderItemData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetMonthlyLandsacpeReportList_Result> SPGetMonthlyLandsacpeReportList()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetMonthlyLandsacpeReportList_Result>("SPGetMonthlyLandsacpeReportList");
        }
    
        public virtual ObjectResult<SPGetBillData_Result> SPGetBillData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetBillData_Result>("SPGetBillData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetBillItemData_Result> SPGetBillItemData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetBillItemData_Result>("SPGetBillItemData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetItemData_Result> SPGetItemData()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetItemData_Result>("SPGetItemData");
        }
    
        public virtual ObjectResult<SPGetEstimateData_Result> SPGetEstimateData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetEstimateData_Result>("SPGetEstimateData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetEstimateItemData_Result> SPGetEstimateItemData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetEstimateItemData_Result>("SPGetEstimateItemData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetInvoiceItemData_Result> SPGetInvoiceItemData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetInvoiceItemData_Result>("SPGetInvoiceItemData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetEstimateReportList_Result> SPGetEstimateReportList(Nullable<int> customerId, Nullable<int> year, Nullable<int> month)
        {
            var customerIdParameter = customerId.HasValue ?
                new ObjectParameter("CustomerId", customerId) :
                new ObjectParameter("CustomerId", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetEstimateReportList_Result>("SPGetEstimateReportList", customerIdParameter, yearParameter, monthParameter);
        }
    
        public virtual ObjectResult<SPGetServiceRequestReportList_Result> SPGetServiceRequestReportList(Nullable<int> customerId, Nullable<int> year, Nullable<int> month)
        {
            var customerIdParameter = customerId.HasValue ?
                new ObjectParameter("CustomerId", customerId) :
                new ObjectParameter("CustomerId", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetServiceRequestReportList_Result>("SPGetServiceRequestReportList", customerIdParameter, yearParameter, monthParameter);
        }
    
        public virtual ObjectResult<SPGetInvoiceData_Result> SPGetInvoiceData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetInvoiceData_Result>("SPGetInvoiceData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetStaffData_Result> SPGetStaffData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetStaffData_Result>("SPGetStaffData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetCustomerData_Result> SPGetCustomerData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetCustomerData_Result>("SPGetCustomerData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetWeeklyReportRCFileData_Result> SPGetWeeklyReportRCFileData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetWeeklyReportRCFileData_Result>("SPGetWeeklyReportRCFileData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetWeeklyReportRCListData_Result> SPGetWeeklyReportRCListData()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetWeeklyReportRCListData_Result>("SPGetWeeklyReportRCListData");
        }
    
        public virtual ObjectResult<SPGetWeeklyReportRCData_Result> SPGetWeeklyReportRCData(Nullable<int> iD, Nullable<int> customerId, Nullable<int> year, Nullable<int> month)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            var customerIdParameter = customerId.HasValue ?
                new ObjectParameter("CustomerId", customerId) :
                new ObjectParameter("CustomerId", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetWeeklyReportRCData_Result>("SPGetWeeklyReportRCData", iDParameter, customerIdParameter, yearParameter, monthParameter);
        }
    
        public virtual ObjectResult<SPGetPunchlistPhotoOnlyData_Result> SPGetPunchlistPhotoOnlyData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetPunchlistPhotoOnlyData_Result>("SPGetPunchlistPhotoOnlyData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetPunchlistPhotoOnlyFileData_Result> SPGetPunchlistPhotoOnlyFileData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetPunchlistPhotoOnlyFileData_Result>("SPGetPunchlistPhotoOnlyFileData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetPunchlistPhotoOnlyList_Result> SPGetPunchlistPhotoOnlyList()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetPunchlistPhotoOnlyList_Result>("SPGetPunchlistPhotoOnlyList");
        }
    
        public virtual ObjectResult<SPGetControllerAuditReportFileData_Result> SPGetControllerAuditReportFileData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetControllerAuditReportFileData_Result>("SPGetControllerAuditReportFileData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetIrrigationControllerAuditReportData_Result> SPGetIrrigationControllerAuditReportData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetIrrigationControllerAuditReportData_Result>("SPGetIrrigationControllerAuditReportData", iDParameter);
        }
    
        public virtual ObjectResult<SPGetIrrigationAuditReportData_Result> SPGetIrrigationAuditReportData(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPGetIrrigationAuditReportData_Result>("SPGetIrrigationAuditReportData", iDParameter);
        }
    }
}
