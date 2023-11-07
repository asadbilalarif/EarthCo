using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class BillList
    {
        public string SupplierName { get; set; }
        public DateTime Date { get; set; }
        public Double? Amount { get; set; }
        public string Memo { get; set; }
        public string Tags { get; set; }
    }
}