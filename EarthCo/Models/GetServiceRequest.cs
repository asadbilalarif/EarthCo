using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class GetServiceRequest
    {
        public int ServiceRequestId { get; set; }
        public string ServiceRequestNumber { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
        public string Assign { get; set; }
        public int? AssignToId { get; set; }
        public string Status { get; set; }
        public string WorkRequest { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string Type { get; set; }
        public List<SPGetServiceRequestLatLongData_Result> LatLngData { get; set; }

    }
}