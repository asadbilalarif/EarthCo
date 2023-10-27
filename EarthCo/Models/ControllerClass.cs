using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class ControllerClass
    {
        public int ControllerId { get; set; }
        public string MakeAndModel { get; set; }
        public string ControllerPhotoPath { get; set; }
        public HttpPostedFile ControllerPhoto { get; set; }
        public string SerialNumber { get; set; }
        public string LoacationClosestAddress { get; set; }
        public Nullable<bool> isSatelliteBased { get; set; }
        public string TypeofWater { get; set; }
        public string MeterNumber { get; set; }
        public string MeterSize { get; set; }
        public Nullable<int> NumberofStation { get; set; }
        public Nullable<int> NumberofValves { get; set; }
        public Nullable<int> NumberofBrokenMainLines { get; set; }
        public string TypeofValves { get; set; }
        public string LeakingValves { get; set; }
        public string MalfunctioningValves { get; set; }
        public Nullable<int> NumberofBrokenLateralLines { get; set; }
        public Nullable<int> NumberofBrokenHeads { get; set; }
        public string RepairsMade { get; set; }
        public string UpgradesMade { get; set; }
        public string PhotoPath { get; set; }
        public HttpPostedFile Photo { get; set; }
        public Nullable<int> IrrigationId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public Nullable<bool> isActive { get; set; }

    }
}