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
    
    public partial class SPGetPunchlistDetailData_Result
    {
        public int PunchlistDetailId { get; set; }
        public string PhotoPath { get; set; }
        public string Notes { get; set; }
        public Nullable<bool> isAfterPhoto { get; set; }
        public string AfterPhotoPath { get; set; }
        public Nullable<bool> isComplete { get; set; }
        public int PunchlistId { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool isActive { get; set; }
        public bool isDelete { get; set; }
    }
}
