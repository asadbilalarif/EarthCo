using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class PurchaseOrderQB
    {
        public class PurchaseOrderResponse
        {
            public PurchaseOrderData PurchaseOrder { get; set; }
            public DateTime Time { get; set; }
        }

        public class PurchaseOrderData
        {
            public string POStatus { get; set; }
            public string Domain { get; set; }
            public bool Sparse { get; set; }
            public string Id { get; set; }
            public string SyncToken { get; set; }
            public MetaData MetaData { get; set; }
            public List<CustomField> CustomField { get; set; }
            public string DocNumber { get; set; }
            public DateTime TxnDate { get; set; }
            public CurrencyRef CurrencyRef { get; set; }
            public List<object> LinkedTxn { get; set; }
            public List<Line> Line { get; set; }
            public VendorRef VendorRef { get; set; }
            public APAccountRef APAccountRef { get; set; }
            public decimal TotalAmt { get; set; }
        }

        public class MetaData
        {
            public DateTime CreateTime { get; set; }
            public DateTime LastUpdatedTime { get; set; }
        }

        public class CustomField
        {
            public string DefinitionId { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
        }

        public class CurrencyRef
        {
            public string Value { get; set; }
            public string Name { get; set; }
        }

        public class Line
        {
            public string Id { get; set; }
            public int LineNum { get; set; }
            public decimal Amount { get; set; }
            public string DetailType { get; set; }
            public ItemBasedExpenseLineDetail ItemBasedExpenseLineDetail { get; set; }
        }

        public class ItemBasedExpenseLineDetail
        {
            public CustomerRef CustomerRef { get; set; }
            public string BillableStatus { get; set; }
            public ItemRef ItemRef { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Qty { get; set; }
            public TaxCodeRef TaxCodeRef { get; set; }
        }

        public class CustomerRef
        {
            public string Value { get; set; }
            public string Name { get; set; }
        }

        public class ItemRef
        {
            public string value { get; set; }
            public string Name { get; set; }
        }

        public class TaxCodeRef
        {
            public string Value { get; set; }
        }

        public class VendorRef
        {
            public string value { get; set; }
            public string Name { get; set; }
        }

        public class APAccountRef
        {
            public string value { get; set; }
            public string Name { get; set; }
        }
    }
}