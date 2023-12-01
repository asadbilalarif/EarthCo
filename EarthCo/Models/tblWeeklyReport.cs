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
    
    public partial class tblWeeklyReport
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblWeeklyReport()
        {
            this.tblWeeklyReportFiles = new HashSet<tblWeeklyReportFile>();
        }
    
        public int WeeklyReportId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> ContactId { get; set; }
        public Nullable<int> ServiceLocationId { get; set; }
        public string JobName { get; set; }
        public string Notes { get; set; }
        public Nullable<int> AssignTo { get; set; }
        public Nullable<System.DateTime> ReportForWeekOf { get; set; }
        public Nullable<int> Thisweekrotation { get; set; }
        public Nullable<int> Nextweekrotation { get; set; }
        public string ProposalsCompleted { get; set; }
        public string ProposalsSubmitted { get; set; }
        public string ProposalsNotes { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool isActive { get; set; }
        public bool isDelete { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblWeeklyReportFile> tblWeeklyReportFiles { get; set; }
    }
}
