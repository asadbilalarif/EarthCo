using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class IrrigationControllerAuditReportClass
    {
        public SPGetIrrigationAuditReportData_Result Data { get; set; }
        public SPGetIrrigationControllerAuditReportData_Result ControllerData { get; set; }
        public List<SPGetControllerAuditReportFileData_Result> FileData { get; set; }
    }
}