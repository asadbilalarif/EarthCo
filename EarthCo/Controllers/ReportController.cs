using EarthCo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EarthCo.Controllers
{
    public class ReportController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();
        [HttpGet]
        public List<tblServiceRequest> GetServiceRequestList()
        {
            List<tblServiceRequest> Data = new List<tblServiceRequest>();
            Data = DB.tblServiceRequests.ToList();
            return Data;
        }
    }
}
