using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class GetPunchlistDetail
    {
        public SPGetPunchlistDetailDataById_Result DetailData { get; set; }
        public List<SPGetPunchlistItemData_Result> ItemData { get; set; }
    }
}