using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class GetInvoiceData
    {
        public SPGetInvoiceData_Result Data { get; set; }
        public List<SPGetInvoiceItemData_Result> ItemData { get; set; }
        public List<SPGetInvoiceItemData_Result> CostItemData { get; set; }
        public List<SPGetInvoiceFileData_Result> FileData { get; set; }
    }
}