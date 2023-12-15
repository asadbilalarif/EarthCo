using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class QBInvoiceCUClass
    {
        public class QBInvoiceClass
        {
            public List<LineDetail> Line { get; set; }
            public CustomerRef CustomerRef { get; set; }
            public CustomerMemo CustomerMemo { get; set; }
            public string Id { get; set; }
            public string SyncToken { get; set; }
            public string DocNumber { get; set; }
            public DateTime DueDate { get; set; }
            public decimal Balance { get; set; }
        }

        public class LineDetail
        {
            public decimal Amount { get; set; }
            public string Description { get; set; }
            public string DetailType { get; set; }
            public SalesItemLineDetail SalesItemLineDetail { get; set; }
        }

        public class SalesItemLineDetail
        {
            public ItemRef ItemRef { get; set; }
            public decimal Qty { get; set; }
            public decimal UnitPrice { get; set; }
        }

        public class ItemRef
        {
            public string value { get; set; }
        }

        public class CustomerRef
        {
            public string value { get; set; }
        }

        public class CustomerMemo
        {
            public string value { get; set; }
        }
    }
}