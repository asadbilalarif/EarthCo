using EarthCo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EarthCo.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class ServiceRequestController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();

        [HttpGet]
        public IHttpActionResult GetServiceRequestList()
        {
            try
            {
                var userIdClaim = User.Identity as ClaimsIdentity;
                //int userId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                List<GetServiceRequest> Data = new List<GetServiceRequest>();
                GetServiceRequest Temp = null;
                List<tblServiceRequest> SRData = new List<tblServiceRequest>();

                int RoleId =(int) DB.tblUsers.Where(x => x.UserId == UserId).Select(s => s.RoleId).FirstOrDefault();
                if(RoleId==1)
                {
                    SRData = DB.tblServiceRequests.ToList();
                }
                else
                {
                    SRData = DB.tblServiceRequests.Where(x => x.Assign == UserId).ToList();
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
                    Temp.Assign = item.Assign;
                    Temp.ServiceRequestNumber = item.ServiceRequestNumber;
                    Temp.Status = item.tblSRStatu.Status;
                    Temp.WorkRequest = item.WorkRequest;
                    Temp.CreatedDate = (DateTime)item.CreatedDate;

                    Data.Add(Temp);
                }

                return Ok(Data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetServiceRequest(int id)
        {
            try
            {
                tblServiceRequest Data = new tblServiceRequest();
                Data = DB.tblServiceRequests.Where(x => x.ServiceRequestId == id).FirstOrDefault();
                if (Data == null)
                {
                    return NotFound();
                }

                return Ok(Data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult AddServiceRequest()
        {


            var Data1 = HttpContext.Current.Request.Params.Get("ServiceRequestData");
            HttpPostedFile file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;

            ServiceRequestFile ServiceRequest = new ServiceRequestFile();
            ServiceRequest.Files = new List<HttpPostedFile>();
            for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
            {
                ServiceRequest.Files.Add(HttpContext.Current.Request.Files[i]); ;
            }

            ServiceRequest.ServiceRequestData = JsonSerializer.Deserialize<tblServiceRequest>(Data1);

            


            tblServiceRequest Data = new tblServiceRequest();
            try
            {
                //HttpCookie cookieObj = Request.Cookies["User"];
                //int UserId = Int32.Parse(cookieObj["UserId"]);
                //int RoleId = Int32.Parse(cookieObj["RoleId"]);

                var userIdClaim = User.Identity as ClaimsIdentity;
                //int userId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                if (ServiceRequest.ServiceRequestData.ServiceRequestId == 0)
                {

                    if (ServiceRequest.ServiceRequestData.tblSRItems != null && ServiceRequest.ServiceRequestData.tblSRItems.Count != 0)
                    {
                        foreach (var item in ServiceRequest.ServiceRequestData.tblSRItems)
                        {
                            item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.CreatedBy = UserId;
                            item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.EditBy = UserId;
                            item.isActive = item.isActive;
                            item.SRId = Data.ServiceRequestId;
                        }
                    }


                    Data = ServiceRequest.ServiceRequestData;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = ServiceRequest.ServiceRequestData.isActive;
                    DB.tblServiceRequests.Add(Data);
                    DB.SaveChanges();

                    //if (ServiceRequest.ServiceRequestData.tblSRItems != null && ServiceRequest.ServiceRequestData.tblSRItems.Count != 0)
                    //{
                    //    tblSRItem ConData = null;

                    //    foreach (var item in ServiceRequest.ServiceRequestData.tblSRItems)
                    //    {
                    //        ConData = new tblSRItem();
                    //        ConData = item;
                    //        ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    //        ConData.CreatedBy = UserId;
                    //        ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    //        ConData.EditBy = UserId;
                    //        ConData.isActive = item.isActive;
                    //        ConData.SRId = Data.ServiceRequestId;
                    //        DB.tblSRItems.Add(ConData);
                    //        DB.SaveChanges();
                    //    }
                    //}

                    if (ServiceRequest.Files != null && ServiceRequest.Files.Count != 0)
                    {
                        tblSRFile FileData = null;
                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        int NameCount = 1;
                        foreach (var item in ServiceRequest.Files)
                        {
                            FileData = new tblSRFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("UploadFile" + Data.ServiceRequestId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading", Path.GetFileName("UploadFile" + Data.ServiceRequestId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            FileData.FileName = Path.GetFileName(item.FileName);
                            FileData.Caption = "";
                            FileData.FilePath = path;
                            FileData.SRId = Data.ServiceRequestId;
                            DB.tblSRFiles.Add(FileData);
                            DB.SaveChanges();
                            NameCount++;
                        }
                    }

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Add Service Request";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();
                    return Ok("Service Request has been added successfully.");
                }
                else
                {
                    Data = DB.tblServiceRequests.Select(r => r).Where(x => x.ServiceRequestId == ServiceRequest.ServiceRequestData.ServiceRequestId).FirstOrDefault();
                    if (Data == null)
                    {
                        return NotFound(); // Customer not found.
                    }

                    List<tblSRItem> ConList = DB.tblSRItems.Where(x => x.SRId == ServiceRequest.ServiceRequestData.ServiceRequestId).ToList();
                    if (ConList != null && ConList.Count != 0)
                    {
                        DB.tblSRItems.RemoveRange(ConList);
                        DB.SaveChanges();
                    }

                    if (ServiceRequest.ServiceRequestData.tblSRItems != null && ServiceRequest.ServiceRequestData.tblSRItems.Count != 0)
                    {
                        foreach (var item in ServiceRequest.ServiceRequestData.tblSRItems)
                        {
                            item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.CreatedBy = UserId;
                            item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.EditBy = UserId;
                            item.isActive = item.isActive;
                            item.SRId = Data.ServiceRequestId;
                        }
                    }

                    Data.ServiceRequestNumber = ServiceRequest.ServiceRequestData.ServiceRequestNumber;
                    Data.ServiceLocation = ServiceRequest.ServiceRequestData.ServiceLocation;
                    Data.Contact = ServiceRequest.ServiceRequestData.Contact;
                    Data.JobName = ServiceRequest.ServiceRequestData.JobName;
                    Data.Assign = ServiceRequest.ServiceRequestData.Assign;
                    Data.WorkRequest = ServiceRequest.ServiceRequestData.WorkRequest;
                    Data.ActionTaken = ServiceRequest.ServiceRequestData.ActionTaken;
                    Data.CompletedDate = ServiceRequest.ServiceRequestData.CompletedDate;
                    Data.DueDate = ServiceRequest.ServiceRequestData.DueDate;
                    Data.CustomerId = ServiceRequest.ServiceRequestData.CustomerId;
                    Data.SRTypeId = ServiceRequest.ServiceRequestData.SRTypeId;
                    Data.SRStatusId = ServiceRequest.ServiceRequestData.SRStatusId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = ServiceRequest.ServiceRequestData.isActive;
                    DB.Entry(Data);
                    DB.SaveChanges();

                    //List<tblSRItem> ConList = DB.tblSRItems.Where(x => x.SRId == ServiceRequest.ServiceRequestData.ServiceRequestId).ToList();
                    //if (ConList != null && ConList.Count != 0)
                    //{
                    //    DB.tblSRItems.RemoveRange(ConList);
                    //    DB.SaveChanges();
                    //}

                    //if (ServiceRequest.ServiceRequestData.tblSRItems != null && ServiceRequest.ServiceRequestData.tblSRItems.Count != 0)
                    //{
                    //    tblSRItem ConData = null;

                    //    foreach (var item in ServiceRequest.ServiceRequestData.tblSRItems)
                    //    {
                    //        ConData = new tblSRItem();
                    //        ConData = item;
                    //        ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    //        ConData.CreatedBy = UserId;
                    //        ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    //        ConData.EditBy = UserId;
                    //        ConData.isActive = item.isActive;
                    //        ConData.SRId = Data.ServiceRequestId;
                    //        DB.tblSRItems.Add(ConData);
                    //        DB.SaveChanges();
                    //    }
                    //}

                    List<tblSRFile> ConFList = DB.tblSRFiles.Where(x => x.SRId == ServiceRequest.ServiceRequestData.ServiceRequestId).ToList();
                    if (ConFList != null && ConFList.Count != 0)
                    {
                        DB.tblSRFiles.RemoveRange(ConFList);
                        DB.SaveChanges();
                    }

                    if (ServiceRequest.Files != null && ServiceRequest.Files.Count != 0)
                    {
                        tblSRFile FileData = null;
                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        int NameCount = 1;
                        foreach (var item in ServiceRequest.Files)
                        {
                            FileData = new tblSRFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("UploadFile" + Data.ServiceRequestId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading", Path.GetFileName("UploadFile" + Data.ServiceRequestId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            FileData.FileName = Path.GetFileName(item.FileName);
                            FileData.Caption = "";
                            FileData.FilePath = path;
                            DB.tblSRFiles.Add(FileData);
                            DB.SaveChanges();
                            NameCount++;
                        }
                    }


                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Update Estimate";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();

                    return Ok("Service Request has been Update successfully.");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult DeleteServiceRequest(int id)
        {
            tblServiceRequest Data = new tblServiceRequest();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            int CUserId = 2;
            try
            {
                Data = DB.tblServiceRequests.Select(r => r).Where(x => x.ServiceRequestId == id).FirstOrDefault();

                if (Data == null)
                {
                    return NotFound(); // 404 - Customer not found
                }

                List<tblSRItem> ConList = DB.tblSRItems.Where(x => x.SRId == id).ToList();
                if (ConList != null && ConList.Count != 0)
                {
                    DB.tblSRItems.RemoveRange(ConList);
                    DB.SaveChanges();
                }

                List<tblSRFile> ConFList = DB.tblSRFiles.Where(x => x.SRId == id).ToList();
                if (ConFList != null && ConFList.Count != 0)
                {
                    DB.tblSRFiles.RemoveRange(ConFList);
                    DB.SaveChanges();
                }


                DB.Entry(Data).State = EntityState.Deleted;
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = CUserId;
                LogData.Action = "Delete Service Request";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("Service Request has been deleted successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpPost]
        public IHttpActionResult UpdateAllSelectedServiceRequestStatus(UpdateStatus ParaData)
        {
            tblServiceRequest Data = new tblServiceRequest();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            int CUserId = 2;
            try
            {
                foreach (var item in ParaData.id)
                {

                    Data = DB.tblServiceRequests.Select(r => r).Where(x => x.ServiceRequestId == item).FirstOrDefault();
                    Data.SRStatusId = ParaData.StatusId;
                    DB.Entry(Data);
                    DB.SaveChanges();
                }

                tblLog LogData = new tblLog();
                LogData.UserId = CUserId;
                LogData.Action = "Update All Selected Service Request status";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("All selected Service Request status has been updated successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult DeleteAllSelectedServiceRequest(DeleteSelected ParaData)
        {
            tblServiceRequest Data = new tblServiceRequest();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            int CUserId = 2;
            try
            {
                foreach (var item in ParaData.id)
                {
                    List<tblSRItem> ConList = DB.tblSRItems.Where(x => x.SRId == item).ToList();
                    if (ConList != null && ConList.Count != 0)
                    {
                        DB.tblSRItems.RemoveRange(ConList);
                        DB.SaveChanges();
                    }
                    List<tblSRFile> ConFList = DB.tblSRFiles.Where(x => x.SRId == item).ToList();
                    if (ConFList != null && ConFList.Count != 0)
                    {
                        DB.tblSRFiles.RemoveRange(ConFList);
                        DB.SaveChanges();
                    }
                    Data = DB.tblServiceRequests.Select(r => r).Where(x => x.ServiceRequestId == item).FirstOrDefault();
                    DB.Entry(Data).State = EntityState.Deleted;
                    DB.SaveChanges();
                }

                tblLog LogData = new tblLog();
                LogData.UserId = CUserId;
                LogData.Action = "Delete All Selected Service Request";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("All selected Service Request has been deleted successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public string GetUserIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (securityToken != null)
            {
                var userIdClaim = securityToken.Claims.FirstOrDefault(claim => claim.Type == "userid");

                if (userIdClaim != null)
                {
                    return userIdClaim.Value;
                }
            }

            return null; // User ID not found in the token
        }
    }
}
