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
            this.tblEstimateFiles = new HashSet<tblEstimateFile>();
            this.tblPurchaseOrders = new HashSet<tblPurchaseOrder>();
        }
    
        public int EstimateId { get; set; }
        public string EstimateNumber { get; set; }
        public string ServiceLocation { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> IssueDate { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> EstimateStatusId { get; set; }
        public string QBStatus { get; set; }
        public string EstimateNotes { get; set; }
        public string ServiceLocationNotes { get; set; }
        public string PrivateNotes { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public Nullable<bool> isActive { get; set; }
    
        public virtual tblCustomer tblCustomer { get; set; }
        public virtual tblEstimateStatu tblEstimateStatu { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblEstimateItem> tblEstimateItems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblEstimateFile> tblEstimateFiles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPurchaseOrder> tblPurchaseOrders { get; set; }
    }
}
