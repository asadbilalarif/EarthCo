using EarthCo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EarthCo.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class DashboardController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();
        [HttpGet]
        public IHttpActionResult GetDashboardData()
        {
            try
            {
                GetDashboardData DashboardData = new GetDashboardData();
                List<GetEstimateItem> EstData = new List<GetEstimateItem>();

                List<tblEstimate> Data = new List<tblEstimate>();
                Data = DB.tblEstimates.Where(x => x.isDelete == false).OrderByDescending(o=>o.EstimateId).Take(5).ToList();
                if (Data != null && Data.Count != 0)
                {
                    foreach (tblEstimate item in Data)
                    {
                        GetEstimateItem EstimateTemp = new GetEstimateItem();
                        EstimateTemp.EstimateId = (int)item.EstimateId;
                        EstimateTemp.CustomerId = (int)item.CustomerId;
                        EstimateTemp.CustomerName = item.tblUser.FirstName + " " + item.tblUser.LastName;
                        EstimateTemp.RegionalManager = item.tblUser1.FirstName + " " + item.tblUser1.LastName;
                        EstimateTemp.Date = item.CreatedDate;
                        EstimateTemp.Status = item.tblEstimateStatu.Status;
                        EstimateTemp.StatusColor = item.tblEstimateStatu.Color;
                        EstimateTemp.EstimateNumber = item.EstimateNumber;
                        EstimateTemp.DescriptionofWork = item.EstimateNotes;

                        EstimateTemp.ProfitPercentage = item.ProfitPercentage;
                        EstimateTemp.EstimateAmount = (double)item.tblEstimateItems.Sum(s => s.Amount);
                        EstData.Add(EstimateTemp);
                    }
                }

                var userIdClaim = User.Identity as ClaimsIdentity;
                int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                List<GetServiceRequest> ServiceData = new List<GetServiceRequest>();
                GetServiceRequest Temp = null;
                List<tblServiceRequest> SRData = new List<tblServiceRequest>();

                int RoleId = (int)DB.tblUsers.Where(x => x.UserId == UserId).Select(s => s.RoleId).FirstOrDefault();
                if (RoleId == 1)
                {
                    SRData = DB.tblServiceRequests.Where(x => x.isDelete == false).OrderByDescending(o => o.ServiceRequestId).Take(5).ToList();
                }
                else
                {
                    SRData = DB.tblServiceRequests.Where(x => x.Assign == UserId && x.isDelete == false).OrderByDescending(o => o.ServiceRequestId).Take(5).ToList();
                }

                if (SRData == null || SRData.Count == 0)
                {
                    return NotFound();
                }

                foreach (var item in SRData)
                {
                    Temp = new GetServiceRequest();

                    Temp.ServiceRequestId = item.ServiceRequestId;
                    Temp.CustomerName = item.tblUser.CompanyName;
                    if (item.Assign != null && item.Assign != 0)
                    {
                        Temp.Assign = item.tblUser1.FirstName + " " + item.tblUser1.LastName;
                    }

                    Temp.ServiceRequestNumber = item.ServiceRequestNumber;
                    Temp.Status = item.tblSRStatu.Status;
                    Temp.StatusColor = item.tblSRStatu.Color;
                    Temp.WorkRequest = item.WorkRequest;
                    Temp.CreatedDate = (DateTime)item.CreatedDate;
                    Temp.Type = item.tblSRType.Type;

                    ServiceData.Add(Temp);
                }

                DashboardData.EstimateData = EstData;
                DashboardData.ServiceRequestData = ServiceData;
                DashboardData.OpenServiceRequestCount = DB.tblServiceRequests.Where(x=>x.SRStatusId==1).Count();
                DashboardData.OpenEstimateCount = DB.tblEstimates.Where(x=>x.EstimateStatusId==4).Count();
                DashboardData.ApprovedEstimateCount = DB.tblEstimates.Where(x=>x.EstimateStatusId==1).Count();
                DashboardData.ClosedBillCount = DB.tblEstimates.Where(x=>x.EstimateStatusId==2).Count();
                DashboardData.OpenPunchlistCount = DB.tblPunchlists.Where(x=>x.StatusId==2).Count();
                DashboardData.BilledInvoiceCount = DB.tblInvoices.Where(x=>x.BillId!=null).Count();


                return Ok(DashboardData); // 200 - Successful response with data
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
