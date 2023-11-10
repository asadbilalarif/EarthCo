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
    
    public partial class tblPurchaseOrder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPurchaseOrder()
        {
            this.tblPurchaseOrderFiles = new HashSet<tblPurchaseOrderFile>();
            this.tblPurchaseOrderItems = new HashSet<tblPurchaseOrderItem>();
            this.tblBills = new HashSet<tblBill>();
        }
    
        public int PurchaseOrderId { get; set; }
        public int SupplierId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string Tags { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public int RegionalManager { get; set; }
        public Nullable<int> TermId { get; set; }
        public int Requestedby { get; set; }
        public int StatusId { get; set; }
        public string InvoiceNumber { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public string BillNumber { get; set; }
        public Nullable<int> BillId { get; set; }
        public string EstimateNumber { get; set; }
        public Nullable<int> EstimateId { get; set; }
        public string MemoInternal { get; set; }
        public string Message { get; set; }
        public string ShipTo { get; set; }
        public double Amount { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool isActive { get; set; }
        public bool isDelete { get; set; }
    
        public virtual tblEstimate tblEstimate { get; set; }
        public virtual tblPurchaseOrderStatu tblPurchaseOrderStatu { get; set; }
        public virtual tblTerm tblTerm { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPurchaseOrderFile> tblPurchaseOrderFiles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPurchaseOrderItem> tblPurchaseOrderItems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBill> tblBills { get; set; }
        public virtual tblUser tblUser { get; set; }
        public virtual tblUser tblUser1 { get; set; }
        public virtual tblUser tblUser2 { get; set; }
    }
}