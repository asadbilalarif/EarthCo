using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class GetServiceRequestData
    {
        public SPGetServiceRequestData_Result Data { get; set; }
        public List<SPGetServiceRequestItemData_Result> ItemData { get; set; }
        public List<SPGetServiceRequestFileData_Result> FileData { get; set; }
    }
}