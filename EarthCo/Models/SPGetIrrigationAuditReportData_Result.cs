//------------------------------------------------------------------------------
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
    
    public partial class SPGetIrrigationAuditReportData_Result
    {
        public int IrrigationAuditReportId { get; set; }
        public string Title { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Nullable<int> ContactId { get; set; }
        public string ContactName { get; set; }
        public string ContactCompany { get; set; }
        public string ContactEmail { get; set; }
        public Nullable<int> RegionalManagerId { get; set; }
        public Nullable<System.DateTime> ReportDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool isActive { get; set; }
        public bool isDelete { get; set; }
        public string RegionalManagerName { get; set; }
    }
}
