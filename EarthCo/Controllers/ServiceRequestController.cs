using EarthCo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
        public IHttpActionResult GetServiceRequestServerSideList(int DisplayStart = 0, int DisplayLength = 10, int StatusId = 0)
        {
            try
            {
                var userIdClaim = User.Identity as ClaimsIdentity;
                //int userId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                List<GetServiceRequest> Data = new List<GetServiceRequest>();
                GetServiceRequest Temp = null;
                List<tblServiceRequest> SRData = new List<tblServiceRequest>();
                var totalRecords = 0;
                var totalOpenRecords = 0;
                var totalClosedRecords = 0;
               int RoleId =(int) DB.tblUsers.Where(x => x.UserId == UserId).Select(s => s.RoleId).FirstOrDefault();
                if(RoleId==1)
                {
                    totalRecords = DB.tblServiceRequests.Count(x => !x.isDelete);
                    totalOpenRecords = DB.tblServiceRequests.Where(x => x.SRStatusId == 1).Count(x => !x.isDelete);
                    totalClosedRecords = DB.tblServiceRequests.Where(x => x.SRStatusId == 2).Count(x => !x.isDelete);
                    DisplayStart = (DisplayStart - 1) * DisplayLength;
                    if (StatusId != 0)
                    {
                        SRData = DB.tblServiceRequests.Where(x => !x.isDelete && x.SRStatusId == StatusId).OrderBy(o => o.ServiceRequestId).Skip(DisplayStart).Take(DisplayLength).ToList();
                    }
                    else
                    {
                        SRData = DB.tblServiceRequests.Where(x => !x.isDelete).OrderBy(o => o.ServiceRequestId).Skip(DisplayStart).Take(DisplayLength).ToList();
                    }

                }
                else
                {
                    totalRecords = DB.tblServiceRequests.Count(x => x.Assign == UserId && x.isDelete == false);
                    totalOpenRecords = DB.tblServiceRequests.Where(x => x.SRStatusId == 1).Count(x => x.Assign == UserId && x.isDelete == false);
                    totalClosedRecords = DB.tblServiceRequests.Where(x => x.SRStatusId == 2).Count(x => x.Assign == UserId && x.isDelete == false);
                    DisplayStart = (DisplayStart - 1) * DisplayLength;
                    if (StatusId != 0)
                    {
                        SRData = DB.tblServiceRequests.Where(x => x.Assign == UserId && x.isDelete == false && x.SRStatusId == StatusId).OrderBy(o => o.ServiceRequestId).Skip(DisplayStart).Take(DisplayLength).ToList();
                    }
                    else
                    {
                        SRData = DB.tblServiceRequests.Where(x => x.Assign == UserId && x.isDelete == false).OrderBy(o => o.ServiceRequestId).Skip(DisplayStart).Take(DisplayLength).ToList();
                    }
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
                    if(item.Assign!=null && item.Assign!=0)
                    {
                        Temp.Assign = item.tblUser1.FirstName + " " + item.tblUser1.LastName;
                    }
                    
                    Temp.ServiceRequestNumber = item.ServiceRequestNumber;
                    Temp.Status = item.tblSRStatu.Status;
                    Temp.WorkRequest = item.WorkRequest;
                    Temp.CreatedDate = (DateTime)item.CreatedDate;

                    Data.Add(Temp);
                }

                return Ok(new { totalRecords = totalRecords, totalOpenRecords = totalOpenRecords, totalClosedRecords = totalClosedRecords, Data = Data }); // 200 - Successful response with data
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
                    SRData = DB.tblServiceRequests.Where(x=>x.isDelete==false).ToList();
                }
                else
                {
                    SRData = DB.tblServiceRequests.Where(x => x.Assign == UserId && x.isDelete==false).ToList();
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
                    if(item.Assign!=null && item.Assign!=0)
                    {
                        Temp.Assign = item.tblUser1.FirstName + " " + item.tblUser1.LastName;
                    }
                    
                    Temp.ServiceRequestNumber = item.ServiceRequestNumber;
                    Temp.Status = item.tblSRStatu.Status;
                    Temp.WorkRequest = item.WorkRequest;
                    Temp.CreatedDate = (DateTime)item.CreatedDate;

                    Data.Add(Temp);
                }

                return Ok(Data);
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

        [HttpGet]
        public IHttpActionResult GetServiceRequest(int id)
        {
            try
            {
                SPGetServiceRequestData_Result Data = new SPGetServiceRequestData_Result();
                List<SPGetServiceRequestItemData_Result> ItemData = new List<SPGetServiceRequestItemData_Result>();
                List<SPGetServiceRequestFileData_Result> FileData = new List<SPGetServiceRequestFileData_Result>();
                Data = DB.SPGetServiceRequestData(id).FirstOrDefault();
                ItemData = DB.SPGetServiceRequestItemData(id).ToList();
                FileData = DB.SPGetServiceRequestFileData(id).ToList();

                GetServiceRequestData GetData = new GetServiceRequestData();
                if (Data == null)
                {
                    return NotFound();
                }
                else
                {
                    GetData.Data = Data;
                    GetData.ItemData = ItemData;
                    GetData.FileData = FileData;

                }

                return Ok(GetData);
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


        [HttpGet]
        public IHttpActionResult GetServiceRequestTypes()
        {
            try
            {
                DB.Configuration.ProxyCreationEnabled = false;
                List<tblSRType> Data = new List<tblSRType>();
                Data = DB.tblSRTypes.ToList();
                if (Data == null)
                {
                    return NotFound();
                }

                return Ok(Data);
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

            tblServiceRequest Data = new tblServiceRequest();
            tblServiceRequest CheckData = new tblServiceRequest();
            try
            {
                ServiceRequest.ServiceRequestData = JsonSerializer.Deserialize<tblServiceRequest>(Data1);
                //HttpCookie cookieObj = Request.Cookies["User"];
                //int UserId = Int32.Parse(cookieObj["UserId"]);
                //int RoleId = Int32.Parse(cookieObj["RoleId"]);

                var userIdClaim = User.Identity as ClaimsIdentity;
                int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                if (ServiceRequest.ServiceRequestData.ServiceRequestId == 0)
                {

                    CheckData = DB.tblServiceRequests.Where(x => x.ServiceRequestNumber == ServiceRequest.ServiceRequestData.ServiceRequestNumber && x.ServiceRequestNumber != null && x.ServiceRequestNumber != "").FirstOrDefault();

                    if (CheckData != null)
                    {
                        var responseMessage = new HttpResponseMessage(HttpStatusCode.Conflict);
                        responseMessage.Content = new StringContent("Service Request number already exsist.");
                        return ResponseMessage(responseMessage);
                    }

                    if (ServiceRequest.ServiceRequestData.tblSRItems != null && ServiceRequest.ServiceRequestData.tblSRItems.Count != 0)
                    {
                        foreach (var item in ServiceRequest.ServiceRequestData.tblSRItems)
                        {
                            item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.CreatedBy = UserId;
                            item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.EditBy = UserId;
                            item.isActive = true;
                            item.isDelete = false;
                            item.SRId = Data.ServiceRequestId;
                        }
                    }


                    Data = ServiceRequest.ServiceRequestData;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = true;
                    Data.isDelete = false;
                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("S").FirstOrDefault());
                    if (Data.ServiceRequestNumber == null || Data.ServiceRequestNumber == "")
                    {
                        Data.ServiceRequestNumber = Data.DocNumber;
                    }
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
                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading/ServiceRequest"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        int NameCount = 1;
                        foreach (var item in ServiceRequest.Files)
                        {
                            FileData = new tblSRFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading/ServiceRequest"), Path.GetFileName("ServiceRequest" + Data.ServiceRequestId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading\\ServiceRequest", Path.GetFileName("ServiceRequest" + Data.ServiceRequestId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            FileData.FileName = "ServiceRequest" + Data.ServiceRequestId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName);
                            FileData.Caption = FileData.FileName;
                            FileData.FilePath = path;
                            FileData.SRId = Data.ServiceRequestId;
                            FileData.CreatedBy = UserId;
                            FileData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.EditBy = UserId;
                            FileData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.isActive = true;
                            FileData.isDelete = false;
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
                    //return Ok("Service Request has been added successfully.");
                    return Ok(new { Id = Data.ServiceRequestId, Message = "Service Request has been added successfully." });
                }
                else
                {
                    Data = DB.tblServiceRequests.Select(r => r).Where(x => x.ServiceRequestId == ServiceRequest.ServiceRequestData.ServiceRequestId).FirstOrDefault();
                    if (Data == null)
                    {
                        return NotFound(); // Customer not found.
                    }

                    CheckData = DB.tblServiceRequests.Where(x => x.ServiceRequestNumber == ServiceRequest.ServiceRequestData.ServiceRequestNumber && x.ServiceRequestNumber != null && x.ServiceRequestNumber != "").FirstOrDefault();
                    if (CheckData != null && CheckData.ServiceRequestId != Data.ServiceRequestId)
                    {
                        var responseMessage = new HttpResponseMessage(HttpStatusCode.Conflict);
                        responseMessage.Content = new StringContent("ServiceRequest number already exsist.");
                        return ResponseMessage(responseMessage);
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
                            item.isActive = true;
                            item.isDelete = false;
                            item.SRId = Data.ServiceRequestId;
                        }
                    }

                    Data.ServiceRequestNumber = ServiceRequest.ServiceRequestData.ServiceRequestNumber;
                    Data.ServiceLocationId = ServiceRequest.ServiceRequestData.ServiceLocationId;
                    Data.ContactId = ServiceRequest.ServiceRequestData.ContactId;
                    Data.JobName = ServiceRequest.ServiceRequestData.JobName;
                    Data.Assign = ServiceRequest.ServiceRequestData.Assign;
                    Data.WorkRequest = ServiceRequest.ServiceRequestData.WorkRequest;
                    Data.ActionTaken = ServiceRequest.ServiceRequestData.ActionTaken;
                    Data.CompletedDate = ServiceRequest.ServiceRequestData.CompletedDate;
                    Data.DueDate = ServiceRequest.ServiceRequestData.DueDate;
                    Data.CustomerId = ServiceRequest.ServiceRequestData.CustomerId;
                    Data.SRTypeId = ServiceRequest.ServiceRequestData.SRTypeId;
                    Data.SRStatusId = ServiceRequest.ServiceRequestData.SRStatusId;
                    Data.ServiceLocationId = ServiceRequest.ServiceRequestData.ServiceLocationId;
                    Data.ContactId = ServiceRequest.ServiceRequestData.ContactId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    if (ServiceRequest.ServiceRequestData.ServiceRequestNumber == null || ServiceRequest.ServiceRequestData.ServiceRequestNumber == "")
                    {
                        Data.ServiceRequestNumber = Data.DocNumber;
                    }
                    Data.isActive = true;
                    Data.isDelete = false;
                    DB.Entry(Data);
                    DB.SaveChanges();

                    //List<tblSRItem> ConList = DB.tblSRItems.Where(x => x.SRId == ServiceRequest.ServiceRequestData.ServiceRequestId).ToList();
                    //if (ConList != null && ConList.Count != 0)
                    //{
                    //    DB.tblSRItems.RemoveRange(ConList);
                    //    DB.SaveChanges();
                    //}

                    if (ServiceRequest.ServiceRequestData.tblSRItems != null && ServiceRequest.ServiceRequestData.tblSRItems.Count != 0)
                    {
                        tblSRItem ConData = null;

                        foreach (var item in ServiceRequest.ServiceRequestData.tblSRItems)
                        {
                            ConData = new tblSRItem();
                            ConData = item;
                            ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.CreatedBy = UserId;
                            ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.EditBy = UserId;
                            ConData.isActive = true;
                            ConData.isDelete = false;
                            ConData.SRId = Data.ServiceRequestId;
                            DB.tblSRItems.Add(ConData);
                            DB.SaveChanges();
                        }
                    }

                    //List<tblSRFile> ConFList = DB.tblSRFiles.Where(x => x.SRId == ServiceRequest.ServiceRequestData.ServiceRequestId).ToList();
                    //if (ConFList != null && ConFList.Count != 0)
                    //{
                    //    DB.tblSRFiles.RemoveRange(ConFList);
                    //    DB.SaveChanges();
                    //}

                    if (ServiceRequest.Files != null && ServiceRequest.Files.Count != 0)
                    {
                        tblSRFile FileData = null;
                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading/ServiceRequest"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        int NameCount = 1;
                        foreach (var item in ServiceRequest.Files)
                        {
                            FileData = new tblSRFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading/ServiceRequest"), Path.GetFileName("ServiceRequest" + Data.ServiceRequestId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading\\ServiceRequest", Path.GetFileName("ServiceRequest" + Data.ServiceRequestId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            FileData.FileName = "ServiceRequest" + Data.ServiceRequestId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName);
                            FileData.Caption = FileData.FileName;
                            FileData.FilePath = path;
                            FileData.SRId = Data.ServiceRequestId;
                            FileData.CreatedBy = UserId;
                            FileData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.EditBy = UserId;
                            FileData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.isActive = true;
                            FileData.isDelete = false;
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

                    //return Ok("Service Request has been Update successfully.");
                    return Ok(new { Id = Data.ServiceRequestId, Message = "Service Request has been Update successfully." });
                }
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
                responseMessage.Content = new StringContent(ErrorString) ;

                return ResponseMessage(responseMessage);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An exception occurred: {ex.Message}");
                // Additional handling for generic exceptions

                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = ex.InnerException != null && ex.InnerException.InnerException!=null ? new StringContent(ex.InnerException.InnerException.Message) : new StringContent(ex.Message);

                return ResponseMessage(responseMessage);
            }
            //catch (Exception ex) when (ex is DbEntityValidationException)
            //{
            //    //foreach (var eve in e.EntityValidationErrors)
            //    //{
            //    //    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
            //    //        eve.Entry.Entity.GetType().Name, eve.Entry.State);
            //    //    foreach (var ve in eve.ValidationErrors)
            //    //    {
            //    //        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
            //    //            ve.PropertyName, ve.ErrorMessage);
            //    //    }
            //    //}
            //    string Test = ex.Message;
            //    string Test1 = ex.EntityValidationErrors;
            //    return InternalServerError(ex);
            //}
        }

        [HttpGet]
        public IHttpActionResult DeleteServiceRequest(int id)
        {
            tblServiceRequest Data = new tblServiceRequest();
            var userIdClaim = User.Identity as ClaimsIdentity;
            int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
            try
            {
                Data = DB.tblServiceRequests.Select(r => r).Where(x => x.ServiceRequestId == id).FirstOrDefault();

                if (Data == null)
                {
                    return NotFound(); // 404 - Customer not found
                }

                //List<tblSRItem> ConList = DB.tblSRItems.Where(x => x.SRId == id).ToList();
                //if (ConList != null && ConList.Count != 0)
                //{
                //    DB.tblSRItems.RemoveRange(ConList);
                //    DB.SaveChanges();
                //}

                //List<tblSRFile> ConFList = DB.tblSRFiles.Where(x => x.SRId == id).ToList();
                //if (ConFList != null && ConFList.Count != 0)
                //{
                //    DB.tblSRFiles.RemoveRange(ConFList);
                //    DB.SaveChanges();
                //}

                Data.isDelete = true;
                Data.EditBy = UserId;
                Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                DB.Entry(Data);
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = UserId;
                LogData.Action = "Delete Service Request";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("Service Request has been deleted successfully.");
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


        [HttpPost]
        public IHttpActionResult UpdateAllSelectedServiceRequestStatus(UpdateStatus ParaData)
        {
            tblServiceRequest Data = new tblServiceRequest();
            var userIdClaim = User.Identity as ClaimsIdentity;
            int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
            try
            {
                foreach (var item in ParaData.id)
                {

                    Data = DB.tblServiceRequests.Select(r => r).Where(x => x.ServiceRequestId == item).FirstOrDefault();
                    Data.SRStatusId = ParaData.StatusId;
                    Data.EditBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm")); ;
                    DB.Entry(Data);
                    DB.SaveChanges();
                }

                tblLog LogData = new tblLog();
                LogData.UserId = UserId;
                LogData.Action = "Update All Selected Service Request status";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("All selected Service Request status has been updated successfully.");
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

        [HttpPost]
        public IHttpActionResult DeleteAllSelectedServiceRequest(DeleteSelected ParaData)
        {
            tblServiceRequest Data = new tblServiceRequest();
            var userIdClaim = User.Identity as ClaimsIdentity;
            int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
            try
            {
                foreach (var item in ParaData.id)
                {
                    //List<tblSRItem> ConList = DB.tblSRItems.Where(x => x.SRId == item).ToList();
                    //if (ConList != null && ConList.Count != 0)
                    //{
                    //    DB.tblSRItems.RemoveRange(ConList);
                    //    DB.SaveChanges();
                    //}
                    //List<tblSRFile> ConFList = DB.tblSRFiles.Where(x => x.SRId == item).ToList();
                    //if (ConFList != null && ConFList.Count != 0)
                    //{
                    //    DB.tblSRFiles.RemoveRange(ConFList);
                    //    DB.SaveChanges();
                    //}
                    Data = DB.tblServiceRequests.Select(r => r).Where(x => x.ServiceRequestId == item).FirstOrDefault();
                    Data.isDelete = true;
                    Data.EditBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    DB.Entry(Data);
                    DB.SaveChanges();
                }

                tblLog LogData = new tblLog();
                LogData.UserId = UserId;
                LogData.Action = "Delete All Selected Service Request";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("All selected Service Request has been deleted successfully.");
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

        [HttpGet]
        public IHttpActionResult DeleteServiceRequestFile(int FileId)
        {
            tblSRFile Data = new tblSRFile();

            var userIdClaim = User.Identity as ClaimsIdentity;
            int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
            try
            {
                Data = DB.tblSRFiles.Select(r => r).Where(x => x.SRFileId == FileId).FirstOrDefault();
                if (Data == null)
                {
                    return NotFound(); // 404 - Customer not found
                }

                Data.isDelete = true;
                Data.EditBy = UserId;
                Data.EditDate = DateTime.Now;
                DB.Entry(Data);
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = UserId;
                LogData.Action = "Delete Service Request File";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("Service Request file has been deleted successfully.");
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
