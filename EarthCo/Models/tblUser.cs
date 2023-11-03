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
    
    public partial class tblUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblUser()
        {
            this.tblContacts = new HashSet<tblContact>();
            this.tblServiceLocations = new HashSet<tblServiceLocation>();
            this.tblSettings = new HashSet<tblSetting>();
            this.tblServiceRequests = new HashSet<tblServiceRequest>();
            this.tblServiceRequests1 = new HashSet<tblServiceRequest>();
        }
    
        public int UserId { get; set; }
        public string username { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string AltPhone { get; set; }
        public string ImagePath { get; set; }
        public string Fax { get; set; }
        public string Notes { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Code { get; set; }
        public string Country { get; set; }
        public string AccountNumber { get; set; }
        public Nullable<int> RoleId { get; set; }
        public Nullable<int> CustomerTypeId { get; set; }
        public Nullable<int> UserTypeId { get; set; }
        public Nullable<System.DateTime> LastLogin { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public Nullable<bool> isLoginAllow { get; set; }
        public Nullable<bool> isActive { get; set; }
        public Nullable<bool> isDelete { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblContact> tblContacts { get; set; }
        public virtual tblCustomerType tblCustomerType { get; set; }
        public virtual tblRole tblRole { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblServiceLocation> tblServiceLocations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSetting> tblSettings { get; set; }
        public virtual tblUserType tblUserType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblServiceRequest> tblServiceRequests { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblServiceRequest> tblServiceRequests1 { get; set; }
    }
}
