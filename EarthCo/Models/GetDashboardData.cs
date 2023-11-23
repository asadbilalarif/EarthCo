using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class GetDashboardData
    {
        public List<GetEstimateItem> EstimateData { get; set; }
        public List<GetServiceRequest> ServiceRequestData { get; set; }
        public int OpenServiceRequestCount { get; set; }
        public int OpenEstimateCount { get; set; }
        public int ApprovedEstimateCount { get; set; }
        public int ClosedBillCount { get; set; }
        public int OpenPunchlistCount { get; set; }
        public int OpenLandscapeCount { get; set; }
        public int BilledInvoiceCount { get; set; }
    }
}