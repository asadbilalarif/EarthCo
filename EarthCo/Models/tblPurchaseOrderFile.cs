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
    
    public partial class tblPurchaseOrderFile
    {
        public int PurchaseOrderFileId { get; set; }
        public string FileName { get; set; }
        public string Caption { get; set; }
        public string FilePath { get; set; }
        public string Share { get; set; }
        public Nullable<int> PurchaseOrderId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public Nullable<bool> isActive { get; set; }
        public Nullable<bool> idDelete { get; set; }
    
        public virtual tblPurchaseOrder tblPurchaseOrder { get; set; }
    }
}
