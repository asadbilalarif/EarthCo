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
    using System.Collections.Generic;
    
    public partial class tblEstimate
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblEstimate()
        {
            this.tblEstimateItems = new HashSet<tblEstimateItem>();
            this.tblInvoices = new HashSet<tblInvoice>();
            this.tblPurchaseOrders = new HashSet<tblPurchaseOrder>();
            this.tblEstimateFiles = new HashSet<tblEstimateFile>();
        }
    
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
        public string DocNumber { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool isActive { get; set; }
        public bool isDelete { get; set; }
        public Nullable<double> TotalAmount { get; set; }
        public Nullable<double> BalanceAmount { get; set; }
    
        public virtual tblContact tblContact { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblEstimateItem> tblEstimateItems { get; set; }
        public virtual tblUser tblUser { get; set; }
        public virtual tblUser tblUser1 { get; set; }
        public virtual tblUser tblUser2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblInvoice> tblInvoices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPurchaseOrder> tblPurchaseOrders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblEstimateFile> tblEstimateFiles { get; set; }
        public virtual tblServiceLocation tblServiceLocation { get; set; }
        public virtual tblEstimateStatu tblEstimateStatu { get; set; }
    }
}
