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
    
    public partial class tblBill
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblBill()
        {
            this.tblBillFiles = new HashSet<tblBillFile>();
            this.tblBillItems = new HashSet<tblBillItem>();
        }
    
        public int BillId { get; set; }
        public int SupplierId { get; set; }
        public string BillNumber { get; set; }
        public string Tags { get; set; }
        public System.DateTime BillDate { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<int> PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public Nullable<int> TermId { get; set; }
        public string Memo { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string DocNumber { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool isActive { get; set; }
        public bool isDelete { get; set; }
    
        public virtual tblPurchaseOrder tblPurchaseOrder { get; set; }
        public virtual tblTerm tblTerm { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBillFile> tblBillFiles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBillItem> tblBillItems { get; set; }
        public virtual tblUser tblUser { get; set; }
    }
}
