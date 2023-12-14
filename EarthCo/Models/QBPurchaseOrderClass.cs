using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class QBPurchaseOrderCUClass
    {
        public class QBPurchaseOrderClass
        {
            public List<LineDetail> Line { get; set; }
            public VendorRef VendorRef { get; set; }
            public APAccountRef APAccountRef { get; set; }
            public decimal TotalAmt { get; set; }
            public string POStatus { get; set; }
            public string Id { get; set; }
            public string SyncToken { get; set; }
        }

        public class LineDetail
        {
            public decimal Amount { get; set; }
            public string DetailType { get; set; }
            public ItemBasedExpenseLineDetail ItemBasedExpenseLineDetail { get; set; }
        }

        public class ItemBasedExpenseLineDetail
        {
            public ItemRef ItemRef { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Qty { get; set; }
        }

        public class ItemRef
        {
            public string value { get; set; }
        }

        public class VendorRef
        {
            public string value { get; set; }
        }

        public class APAccountRef
        {
            public string value { get; set; }
        }
    }
}