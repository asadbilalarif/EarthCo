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
    
    public partial class tblInvoice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblInvoice()
        {
            this.tblInvoiceItems = new HashSet<tblInvoiceItem>();
            this.tblInvoiceFiles = new HashSet<tblInvoiceFile>();
        }
    
        public int InvoiceId { get; set; }
        public int CustomerId { get; set; }
        public Nullable<int> ServiceLocationId { get; set; }
        public Nullable<int> ContactId { get; set; }
        public string InvoiceNumber { get; set; }
        public Nullable<int> EstimateId { get; set; }
        public string EstimateNumber { get; set; }
        public Nullable<int> BillId { get; set; }
        public string BillNumber { get; set; }
        public string Tags { get; set; }
        public Nullable<int> TermId { get; set; }
        public Nullable<int> AssignTo { get; set; }
        public System.DateTime IssueDate { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public string CustomerMessage { get; set; }
        public string MemoInternal { get; set; }
        public double TotalAmount { get; set; }
        public double BalanceAmount { get; set; }
        public Nullable<double> Tax { get; set; }
        public Nullable<double> Discount { get; set; }
        public Nullable<double> Shipping { get; set; }
        public Nullable<double> Profit { get; set; }
        public double ProfitPercentage { get; set; }
        public Nullable<int> StatusId { get; set; }
        public string DocNumber { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool isActive { get; set; }
        public bool isDelete { get; set; }
    
        public virtual tblContact tblContact { get; set; }
        public virtual tblTerm tblTerm { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblInvoiceItem> tblInvoiceItems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblInvoiceFile> tblInvoiceFiles { get; set; }
        public virtual tblServiceLocation tblServiceLocation { get; set; }
        public virtual tblInvoiceStatu tblInvoiceStatu { get; set; }
        public virtual tblEstimate tblEstimate { get; set; }
        public virtual tblUser tblUser { get; set; }
        public virtual tblUser tblUser1 { get; set; }
    }
}
