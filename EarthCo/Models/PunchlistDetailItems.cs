using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class PunchlistDetailItems
    {
        public tblPunchlistDetail PunchlistDetailData { get; set; }
        public HttpPostedFile Files { get; set; }
        public HttpPostedFile AfterFiles { get; set; }
        public List<tblPunchlistItem> PunchlistItemsData { get; set; }
    }
}