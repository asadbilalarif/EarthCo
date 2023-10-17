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
    
    public partial class tblPunchlistDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPunchlistDetail()
        {
            this.tblPunchlistItems = new HashSet<tblPunchlistItem>();
        }
    
        public int PunchlistDetailId { get; set; }
        public string PhotoPath { get; set; }
        public string Notes { get; set; }
        public string Address { get; set; }
        public Nullable<bool> isAfterPhoto { get; set; }
        public string AfterPhotoPath { get; set; }
        public Nullable<bool> isComplete { get; set; }
        public Nullable<int> PunchlistId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public Nullable<bool> isActive { get; set; }
    
        public virtual tblPunchlist tblPunchlist { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPunchlistItem> tblPunchlistItems { get; set; }
    }
}
