using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class GetPurchaseOrderData
    {
        public SPGetPurchaseOrderData_Result Data { get; set; }
        public List<SPGetPurchaseOrderItemData_Result> ItemData { get; set; }
        public List<SPGetPurchaseOrderFileData_Result> FileData { get; set; }
    }
}