using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class ControllerAuditReportClass
    {
        public int ControllerAuditReportId { get; set; }
        public HttpPostedFile ControllerPhoto { get; set; }
        public string ControllerName { get; set; }
        public Nullable<bool> BrokenValve { get; set; }
        public Nullable<bool> BrokenLaterals { get; set; }
        public Nullable<bool> BrokenHeads { get; set; }
        public Nullable<int> HowMany { get; set; }
        public string RepairMadeOrNeeded { get; set; }
        public HttpPostedFile Photo { get; set; }
        public List<HttpPostedFile> MoreFiles { get; set; }
        public int IrrigationAuditReportId { get; set; }
        public Nullable<bool> MorePhotos { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool isActive { get; set; }
        public bool isDelete { get; set; }
    }
}