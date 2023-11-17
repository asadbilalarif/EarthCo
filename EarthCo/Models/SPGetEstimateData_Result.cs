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
    
    public partial class SPGetEstimateData_Result
    {
        public int EstimateId { get; set; }
        public string EstimateNumber { get; set; }
        public int ServiceLocationId { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> IssueDate { get; set; }
        public int CustomerId { get; set; }
        public int ContactId { get; set; }
        public int RegionalManagerId { get; set; }
        public int AssignTo { get; set; }
        public int RequestedBy { get; set; }
        public int EstimateStatusId { get; set; }
        public string QBStatus { get; set; }
        public string EstimateNotes { get; set; }
        public string ServiceLocationNotes { get; set; }
        public string PrivateNotes { get; set; }
        public Nullable<double> Tax { get; set; }
        public Nullable<double> Discount { get; set; }
        public Nullable<double> Shipping { get; set; }
        public Nullable<double> Profit { get; set; }
        public double ProfitPercentage { get; set; }
        public string Tags { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool isActive { get; set; }
        public bool isDelete { get; set; }
    }
}
