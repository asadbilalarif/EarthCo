using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class GetPunchlistList
    {
        public SPGetPunchlistData_Result Data { get; set; }
        public List<GetPunchlistDetailList> DetailDataList { get; set; }
        public string CustomerName { get; set; }
        public string AssignToName { get; set; }
        public string Status { get; set; }
        public string StatusColor { get; set; }
    }
}