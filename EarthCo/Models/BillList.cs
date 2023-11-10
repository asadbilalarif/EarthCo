﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class BillList
    {
        public string SupplierName { get; set; }
        public DateTime DueDate { get; set; }
        public Double? Amount { get; set; }
        public string Memo { get; set; }
        public string Currency { get; set; }
        public string Tags { get; set; }
    }
}