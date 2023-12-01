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
    
    public partial class tblServiceRequest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblServiceRequest()
        {
            this.tblSRFiles = new HashSet<tblSRFile>();
            this.tblSRItems = new HashSet<tblSRItem>();
            this.tblServiceRequestLatLongs = new HashSet<tblServiceRequestLatLong>();
        }
    
        public int ServiceRequestId { get; set; }
        public string ServiceRequestNumber { get; set; }
        public int ServiceLocationId { get; set; }
        public int ContactId { get; set; }
        public string JobName { get; set; }
        public Nullable<int> Assign { get; set; }
        public string WorkRequest { get; set; }
        public string ActionTaken { get; set; }
        public Nullable<System.DateTime> CompletedDate { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public int CustomerId { get; set; }
        public int SRTypeId { get; set; }
        public int SRStatusId { get; set; }
        public string DocNumber { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool isActive { get; set; }
        public bool isDelete { get; set; }
    
        public virtual tblServiceLocation tblServiceLocation { get; set; }
        public virtual tblUser tblUser { get; set; }
        public virtual tblSRStatu tblSRStatu { get; set; }
        public virtual tblSRType tblSRType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSRFile> tblSRFiles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSRItem> tblSRItems { get; set; }
        public virtual tblUser tblUser1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblServiceRequestLatLong> tblServiceRequestLatLongs { get; set; }
    }
}
