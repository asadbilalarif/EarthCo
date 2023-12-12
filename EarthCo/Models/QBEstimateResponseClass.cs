using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class QBEstimateResponseClass
    {
        public class EstimateMain
        {
            public EstimateData Estimate { get; set; }
            public DateTime Time { get; set; }
        }

        public class EstimateData
        {
            public string Domain { get; set; }
            public bool Sparse { get; set; }
            public string Id { get; set; }
            public string SyncToken { get; set; }
            public MetaData MetaData { get; set; }
            public List<CustomField> CustomField { get; set; }
            public string DocNumber { get; set; }
            public DateTime TxnDate { get; set; }
            public CurrencyRef CurrencyRef { get; set; }
            public string PrivateNote { get; set; }
            public string TxnStatus { get; set; }
            public List<Line> Line { get; set; }
            public TxnTaxDetail TxnTaxDetail { get; set; }
            public CustomerRef CustomerRef { get; set; }
            public CustomerMemo CustomerMemo { get; set; }
            public BillAddr BillAddr { get; set; }
            public decimal TotalAmt { get; set; }
            public bool ApplyTaxAfterDiscount { get; set; }
            public string PrintStatus { get; set; }
            public string EmailStatus { get; set; }
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
            public SubTotalLineDetail SubTotalLineDetail { get; set; }
        }

        public class SalesItemLineDetail
        {
            public ItemRef ItemRef { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Qty { get; set; }
            public ItemAccountRef ItemAccountRef { get; set; }
            public TaxCodeRef TaxCodeRef { get; set; }
        }

        public class ItemRef
        {
            public string value { get; set; }
            public string name { get; set; }
        }

        public class ItemAccountRef
        {
            public string value { get; set; }
            public string name { get; set; }
        }

        public class TaxCodeRef
        {
            public string value { get; set; }
        }

        public class SubTotalLineDetail
        {
        }

        public class TxnTaxDetail
        {
            public decimal TotalTax { get; set; }
        }

        public class CustomerRef
        {
            public string value { get; set; }
            public string name { get; set; }
        }

        public class CustomerMemo
        {
            public string value { get; set; }
        }

        public class BillAddr
        {
            public string Id { get; set; }
            public string Line1 { get; set; }
            public string City { get; set; }
            public string CountrySubDivisionCode { get; set; }
            public string PostalCode { get; set; }
            public string Lat { get; set; }
            public string Long { get; set; }
        }
    }
}