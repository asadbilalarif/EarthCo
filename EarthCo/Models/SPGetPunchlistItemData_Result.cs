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
    
    public partial class SPGetPunchlistItemData_Result
    {
        public int PunchlistItemId { get; set; }
        public int ItemId { get; set; }
        public string Name { get; set; }
        public int Qty { get; set; }
        public string Description { get; set; }
        public double Rate { get; set; }
        public double Amount { get; set; }
        public Nullable<double> Tax { get; set; }
        public int PunchlistDetailId { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool isActive { get; set; }
        public bool isDelete { get; set; }
    }
}
