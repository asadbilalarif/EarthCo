using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class QBItemCUClass
    {
        public class QBItemClass
        {
            public string Id { get; set; }
            public string SyncToken { get; set; }
            public string Name { get; set; }
            public IncomeAccountRef IncomeAccountRef { get; set; }
            public ExpenseAccountRef ExpenseAccountRef { get; set; }
            public string Type { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal PurchaseCost { get; set; }
        }

        public class IncomeAccountRef
        {
            public string value { get; set; }
        }

        public class ExpenseAccountRef
        {
            public string value { get; set; }
        }
    }
}