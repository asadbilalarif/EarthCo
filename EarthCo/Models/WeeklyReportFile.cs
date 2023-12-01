using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class WeeklyReportFile
    {
        public tblWeeklyReport WeeklyReportData { get; set; }
        public List<HttpPostedFile> Files { get; set; }
    }
}