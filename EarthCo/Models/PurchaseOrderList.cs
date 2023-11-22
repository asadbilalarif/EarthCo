using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class PurchaseOrderList
    {
        public int? PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string SupplierName { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string RegionalManager { get; set; }
        public string RequestedBy { get; set; }
        public string EstimateNumber { get; set; }
        public string BillNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public Double? Amount { get; set; }
    }
}