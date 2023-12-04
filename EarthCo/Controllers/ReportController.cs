using EarthCo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EarthCo.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class ReportController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();
        [HttpGet]
        public IHttpActionResult GetReportList(int CustomerId,int Year,int Month,string Type)
        {
            try
            {
                if(Type.ToLower()=="proposal")
                {
                    List<SPGetEstimateReportList_Result> Data = new List<SPGetEstimateReportList_Result>();
                    Data = DB.SPGetEstimateReportList(CustomerId,Year,Month).ToList();
                    if (Data == null || Data.Count == 0)
                    {
                        return NotFound();
                    }
                    return Ok(Data);
                }
                if(Type.ToLower()=="servicerequest" || Type.ToLower() == "service request")
                {
                    List<SPGetServiceRequestReportList_Result> Data = new List<SPGetServiceRequestReportList_Result>();
                    Data = DB.SPGetServiceRequestReportList(CustomerId, Year, Month).ToList();
                    if(Data==null || Data.Count==0)
                    {
                        return NotFound();
                    }
                    return Ok(Data);
                }
                return NotFound();
            }
            catch (DbEntityValidationException dbEx)
            {
                string ErrorString = "";
                // Handle DbEntityValidationException
                foreach (var item in dbEx.EntityValidationErrors)
                {
                    foreach (var item1 in item.ValidationErrors)
                    {
                        ErrorString += item1.ErrorMessage + " ,";
                    }
                }
                Console.WriteLine($"DbEntityValidationException occurred: {dbEx.Message}");
                // Additional handling specific to DbEntityValidationException
                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = new StringContent(ErrorString);
                return ResponseMessage(responseMessage);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An exception occurred: {ex.Message}");
                // Additional handling for generic exceptions
                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = ex.InnerException != null && ex.InnerException.InnerException != null ? new StringContent(ex.InnerException.InnerException.Message) : new StringContent(ex.Message);
                return ResponseMessage(responseMessage);
            }
            
        }
    }
}
