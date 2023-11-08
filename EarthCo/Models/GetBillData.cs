using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class GetBillData
    {
        public SPGetBillData_Result Data { get; set; }
        public List<SPGetBillItemData_Result> ItemData { get; set; }
        public List<SPGetBillFileData_Result> FileData { get; set; }
    }
}