using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class QBItem
    {
        public class ItemResponse
        {
            public Item Item { get; set; }
            public DateTime Time { get; set; }
        }

        public class Item
        {
            public string Name { get; set; }
            public bool Active { get; set; }
            public string FullyQualifiedName { get; set; }
            public bool Taxable { get; set; }
            public decimal UnitPrice { get; set; }
            public string Type { get; set; }
            public IncomeAccountRef IncomeAccountRef { get; set; }
            public decimal PurchaseCost { get; set; }
            public ExpenseAccountRef ExpenseAccountRef { get; set; }
            public bool TrackQtyOnHand { get; set; }
            public string Domain { get; set; }
            public bool Sparse { get; set; }
            public string Id { get; set; }
            public string SyncToken { get; set; }
            public MetaData MetaData { get; set; }
        }

        public class IncomeAccountRef
        {
            public string value { get; set; }
            public string name { get; set; }
        }

        public class ExpenseAccountRef
        {
            public string value { get; set; }
            public string name { get; set; }
        }

        public class MetaData
        {
            public DateTime CreateTime { get; set; }
            public DateTime LastUpdatedTime { get; set; }
        }
    }
}