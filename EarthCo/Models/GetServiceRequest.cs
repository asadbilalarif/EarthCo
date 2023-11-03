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
        public int? Assign { get; set; }

        public string Status { get; set; }
        public string WorkRequest { get; set; }
        public System.DateTime CreatedDate { get; set; }

    }
}