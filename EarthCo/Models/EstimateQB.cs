using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class EstimateQB
    {
        public class SalesItemLineDetail
        {
            public ItemRef ItemRef { get; set; }
            public decimal UnitPrice { get; set; }
            public int Qty { get; set; }
        }

        public class ItemRef
        {
            public string value { get; set; }
            public string name { get; set; }
        }

        public class Line
        {
            public string Id { get; set; }
            public int LineNum { get; set; }
            public string Description { get; set; }
            public decimal Amount { get; set; }
            public string DetailType { get; set; }
            public SalesItemLineDetail SalesItemLineDetail { get; set; }
        }

        public class BillEmail
        {
            public string Address { get; set; }
        }

        public class QBEstimateClass
        {
            public List<Line> Line { get; set; }
            public CustomerRef CustomerRef { get; set; }
            public string SyncToken { get; set; }
            public string Id { get; set; }
            public string DocNumber { get; set; }
            public decimal TotalAmt { get; set; }
            public BillEmail BillEmail { get; set; }
        }

        public class CustomerRef
        {
            public string value { get; set; }
            public string name { get; set; }
        }

    }
}