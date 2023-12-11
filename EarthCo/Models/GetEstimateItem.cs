using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class GetEstimateItem
    {
        public int EstimateId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCompanyName { get; set; }
        public string RegionalManager { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string StatusColor { get; set; }
        public string EstimateNumber { get; set; }
        public string DescriptionofWork { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string BillNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public double ProfitPercentage { get; set; }
        public double EstimateAmount { get; set; }
    }
}