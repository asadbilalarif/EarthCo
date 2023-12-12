using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class QBItemResponseClass
    {
        public class ItemQueryResponse
        {
            public QueryResponse QueryResponse { get; set; }
            public DateTime Time { get; set; }
        }

        public class QueryResponse
        {
            public List<Item> Item { get; set; }
            public int StartPosition { get; set; }
            public int MaxResults { get; set; }
        }

        public class Item
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public bool Active { get; set; }
            public string FullyQualifiedName { get; set; }
            public bool Taxable { get; set; }
            public decimal UnitPrice { get; set; }
            public string Type { get; set; }
            public IncomeAccountRef IncomeAccountRef { get; set; }
            public decimal PurchaseCost { get; set; }
            public bool TrackQtyOnHand { get; set; }
            public string Domain { get; set; }
            public bool Sparse { get; set; }
            public string Id { get; set; }
            public string SyncToken { get; set; }
            public MetaData MetaData { get; set; }
        }

        public class IncomeAccountRef
        {
            public string Value { get; set; }
            public string Name { get; set; }
        }

        public class MetaData
        {
            public DateTime CreateTime { get; set; }
            public DateTime LastUpdatedTime { get; set; }
        }
    }
}