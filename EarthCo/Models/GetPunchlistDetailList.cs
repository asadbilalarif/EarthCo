using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class GetPunchlistDetailList
    {
        public SPGetPunchlistDetailData_Result DetailData { get; set; }
        public List<SPGetPunchlistItemData_Result> ItemData { get; set; }
    }
}