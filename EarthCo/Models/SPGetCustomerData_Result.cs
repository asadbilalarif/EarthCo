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
    
    public partial class SPGetCustomerData_Result
    {
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
        public int UserTypeId { get; set; }
        public Nullable<System.DateTime> LastLogin { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool isLoginAllow { get; set; }
        public bool isActive { get; set; }
        public bool isDelete { get; set; }
        public Nullable<int> QBId { get; set; }
        public string SyncToken { get; set; }
    }
}
