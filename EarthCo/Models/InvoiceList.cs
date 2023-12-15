using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class InvoiceList
    {
        public int? InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public string BillNumber { get; set; }
        public string EstimateNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public string CustomerName { get; set; }
        public Double? BalanceAmount { get; set; }
        public Double? TotalAmount { get; set; }
        public Double? ProfitPercentage { get; set; }
    }
}