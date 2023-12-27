using EarthCo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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
    public class IrrigationAuditReportController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();
        [HttpGet]
        public IHttpActionResult GetIrrigationAuditReportServerSideList(string Search, int DisplayStart = 0, int DisplayLength = 10, bool isAscending = false)
        {
            try
            {
                DB.Configuration.ProxyCreationEnabled = false;
                List<tblIrrigationAuditReport> Data = new List<tblIrrigationAuditReport>();
                //List<IrrigationList> Result = new List<IrrigationList>();

                var totalRecords = DB.tblIrrigationAuditReports.Count(x => !x.isDelete);
                DisplayStart = (DisplayStart - 1) * DisplayLength;
                if (Search == null)
                {
                    Search = "\"\"";
                }
                Search = JsonSerializer.Deserialize<string>(Search);
                if (!string.IsNullOrEmpty(Search) && Search != "")
                {
                    Data = DB.tblIrrigationAuditReports.Where(x => x.CustomerName.ToLower().Contains(Search.ToLower())
                                                    || x.Title.ToLower().Contains(Search.ToLower())
                                                    || x.ContactEmail.ToLower().Contains(Search.ToLower())
                                                  || x.ContactCompany.ToLower().Contains(Search.ToLower())).ToList();
                    totalRecords = Data.Count(x => !x.isDelete);
                    Data = Data.Where(x => !x.isDelete).OrderBy(o => isAscending ? o.IrrigationAuditReportId : -o.IrrigationAuditReportId).Skip(DisplayStart).Take(DisplayLength).ToList();
                }
                else
                {
                    Data = DB.tblIrrigationAuditReports.Where(x => !x.isDelete).OrderBy(o => isAscending ? o.IrrigationAuditReportId : -o.IrrigationAuditReportId).Skip(DisplayStart).Take(DisplayLength).ToList();
                }

                if (Data == null || Data.Count == 0)
                {
                    return NotFound(); // 404 - No data found
                }
                return Ok(new { totalRecords = totalRecords, Data = Data }); // 200 - Successful response with data
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
        public IHttpActionResult GetIrrigationAuditReport(int id)
        {
            try
            {

                SPGetIrrigationAuditReportData_Result Data = new SPGetIrrigationAuditReportData_Result();
                List<SPGetIrrigationControllerAuditReportData_Result> ControllerData = new List<SPGetIrrigationControllerAuditReportData_Result>();
                List<SPGetControllerAuditReportFileData_Result> FileData = new List<SPGetControllerAuditReportFileData_Result>();
                Data = DB.SPGetIrrigationAuditReportData(id).FirstOrDefault();
                ControllerData = DB.SPGetIrrigationControllerAuditReportData(id).ToList();
                //int ControllerId = 0;
                //if(ControllerData!=null && ControllerData.Count!=0)
                //{
                //    ControllerId = ControllerData.FirstOrDefault().ControllerAuditReportId;
                //}
                //FileData = DB.SPGetControllerAuditReportFileData(ControllerId).ToList();


                List<IrrigationControllerAuditReportClass> Result = new List<IrrigationControllerAuditReportClass>();

                if (Data == null)
                {
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                    return ResponseMessage(responseMessage);
                }
                else
                {
                    foreach (var item in ControllerData)
                    {
                        IrrigationControllerAuditReportClass Temp = new IrrigationControllerAuditReportClass();
                        Temp.Data = Data;
                        Temp.ControllerData = item;
                        Temp.FileData = DB.SPGetControllerAuditReportFileData(item.ControllerAuditReportId).ToList();
                        Result.Add(Temp);
                    }
                    
                }

                //var Result = new
                //{
                //    Data = Data,
                //    ControllerData = ControllerData,
                //    FileData = FileData
                //};

                return Ok(Result);
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
        public IHttpActionResult AddIrrigationAuditReport([FromBody] tblIrrigationAuditReport ParaData)
        {
            tblIrrigationAuditReport Data = new tblIrrigationAuditReport();
            tblIrrigationAuditReport CheckData = new tblIrrigationAuditReport();
            try
            {
                var userIdClaim = User.Identity as ClaimsIdentity;
                int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                if (ParaData.IrrigationAuditReportId == 0)
                {

                    Data = ParaData;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = true;
                    Data.isDelete = false;
                    DB.tblIrrigationAuditReports.Add(Data);
                    DB.SaveChanges();

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Add Irrigation Audit Report";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();
                    return Ok(new { Id = Data.IrrigationAuditReportId, Message = "Irrigation audit report has been added successfully." });
                }
                else
                {
                    Data = DB.tblIrrigationAuditReports.Select(r => r).Where(x => x.IrrigationAuditReportId == ParaData.IrrigationAuditReportId).FirstOrDefault();
                    if (Data == null)
                    {
                        return NotFound(); // Customer not found.
                    }

                    Data.Title = ParaData.Title;
                    Data.CustomerId = ParaData.CustomerId;
                    Data.CustomerName = ParaData.CustomerName;
                    Data.ContactId = ParaData.ContactId;
                    Data.ContactName = ParaData.ContactName;
                    Data.ContactCompany = ParaData.ContactCompany;
                    Data.ContactEmail = ParaData.ContactEmail;
                    Data.RegionalManagerId = ParaData.RegionalManagerId;
                    Data.ReportDate = ParaData.ReportDate;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = true;
                    Data.isDelete = false;
                    DB.Entry(Data);
                    DB.SaveChanges();

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Update Irrigation Audit Report";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();

                    return Ok(new { Id = Data.IrrigationAuditReportId, Message = "Irrigation audit report has been update successfully." });
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
        public IHttpActionResult AddControllerAuditReport()
        {
            try
            {
                var Data1 = HttpContext.Current.Request.Params.Get("IrrigationControllerAuditReportData");
                //HttpPostedFile file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;

                ControllerAuditReportClass ParaData = new ControllerAuditReportClass();
                //Punchlist.Files = new HttpPostedFile();
                ParaData = JsonSerializer.Deserialize<ControllerAuditReportClass>(Data1);
                if (HttpContext.Current.Request.Files["Photo"] != null)
                {
                    ParaData.Photo = HttpContext.Current.Request.Files["Photo"];
                }
                if (HttpContext.Current.Request.Files["ControllerPhoto"] != null)
                {
                    ParaData.ControllerPhoto = HttpContext.Current.Request.Files["ControllerPhoto"];
                }
                if (HttpContext.Current.Request.Files["MorePhoto"] != null)
                {
                    HttpFileCollection files = HttpContext.Current.Request.Files;
                    ParaData.MoreFiles = files.AllKeys.Where(key => key == "MorePhoto").Select(key => files[key]).ToList();
                }
                var userIdClaim = User.Identity as ClaimsIdentity;
                int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                if (ParaData.ControllerAuditReportId == 0)
                {
                    tblControllerAuditReport ConData = null;
                    ConData = new tblControllerAuditReport();
                    ConData.ControllerName = ParaData.ControllerName;
                    ConData.BrokenValve = ParaData.BrokenValve;
                    ConData.BrokenLaterals = ParaData.BrokenLaterals;
                    ConData.BrokenHeads = ParaData.BrokenHeads;
                    ConData.HowMany = ParaData.HowMany;
                    ConData.RepairMadeOrNeeded = ParaData.RepairMadeOrNeeded;
                    ConData.MorePhotos = ParaData.MorePhotos;
                    ConData.IrrigationAuditReportId = ParaData.IrrigationAuditReportId;
                    ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    ConData.CreatedBy = UserId;
                    ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    ConData.EditBy = UserId;
                    ConData.isActive = true;
                    ConData.isDelete = false;

                    string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading/IrrigationAuditReport"));
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    if (ParaData.ControllerPhoto != null)
                    {
                        string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading/IrrigationAuditReport"), Path.GetFileName("ControllerPhoto" + ParaData.IrrigationAuditReportId.ToString() + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(ParaData.ControllerPhoto.FileName)));
                        ParaData.ControllerPhoto.SaveAs(path);
                        path = Path.Combine("\\Uploading\\IrrigationAuditReport", Path.GetFileName("ControllerPhoto" + ParaData.IrrigationAuditReportId.ToString() + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(ParaData.ControllerPhoto.FileName)));
                        ConData.ControllerPhotoPath = path;
                        
                    }
                    if (ParaData.Photo != null)
                    {
                        string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading/IrrigationAuditReport"), Path.GetFileName("Photo" + ParaData.IrrigationAuditReportId.ToString() + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(ParaData.Photo.FileName)));
                        ParaData.Photo.SaveAs(path);
                        path = Path.Combine("\\Uploading\\IrrigationAuditReport", Path.GetFileName("Photo" + ParaData.IrrigationAuditReportId.ToString() + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(ParaData.Photo.FileName)));
                        ConData.PhotoPath = path;
                    }

                    DB.tblControllerAuditReports.Add(ConData);
                    DB.SaveChanges();

                    int Count = 1;
                    if (ParaData.MoreFiles != null && ParaData.MoreFiles.Count != 0)
                    {
                        foreach (var item in ParaData.MoreFiles)
                        {
                            tblControllerAuditReportFile Temp = new tblControllerAuditReportFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading/IrrigationAuditReport"), Path.GetFileName("MorePhoto" + ConData.ControllerAuditReportId.ToString() + Count + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading\\IrrigationAuditReport", Path.GetFileName("MorePhoto" + ConData.ControllerAuditReportId.ToString() + Count + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            Temp.FilePath = path;
                            Temp.FileName = "MorePhoto" + ConData.ControllerAuditReportId.ToString() + Count + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName);
                            Temp.Caption = item.FileName;
                            Temp.ControllerAuditReportId = ConData.ControllerAuditReportId;
                            Temp.CreatedBy = UserId;
                            Temp.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            Temp.EditBy = UserId;
                            Temp.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            Temp.isActive = true;
                            Temp.isDelete = false;
                            DB.tblControllerAuditReportFiles.Add(Temp);
                            Count++;
                        }

                        DB.SaveChanges();
                    }

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Add Controller Audit Report";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();
                    return Ok(new { Id = ConData.ControllerAuditReportId, Message = "Controller audit report has been added successfully." });
                }
                else
                {
                    tblControllerAuditReport ConData = new tblControllerAuditReport();
                    ConData = DB.tblControllerAuditReports.Select(r => r).Where(x => x.ControllerAuditReportId == ParaData.ControllerAuditReportId).FirstOrDefault();
                    if (ConData == null)
                    {
                        return NotFound(); // Customer not found.
                    }



                    ConData.ControllerName = ParaData.ControllerName;
                    ConData.BrokenValve = ParaData.BrokenValve;
                    ConData.BrokenLaterals = ParaData.BrokenLaterals;
                    ConData.BrokenHeads = ParaData.BrokenHeads;
                    ConData.HowMany = ParaData.HowMany;
                    ConData.RepairMadeOrNeeded = ParaData.RepairMadeOrNeeded;
                    ConData.MorePhotos = ParaData.MorePhotos;
                    ConData.IrrigationAuditReportId = ParaData.IrrigationAuditReportId;
                    ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    ConData.CreatedBy = UserId;
                    ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    ConData.EditBy = UserId;
                    ConData.isActive = true;
                    ConData.isDelete = false;

                    string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading/IrrigationAuditReport"));
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    if (ParaData.ControllerPhoto != null)
                    {
                        string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading/IrrigationAuditReport"), Path.GetFileName("ControllerPhoto" + ParaData.IrrigationAuditReportId.ToString() + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(ParaData.ControllerPhoto.FileName)));
                        ParaData.ControllerPhoto.SaveAs(path);
                        path = Path.Combine("\\Uploading\\IrrigationAuditReport", Path.GetFileName("ControllerPhoto" + ParaData.IrrigationAuditReportId.ToString() + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(ParaData.ControllerPhoto.FileName)));
                        ConData.ControllerPhotoPath = path;
                    }
                    if (ParaData.Photo != null)
                    {
                        string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading/IrrigationAuditReport"), Path.GetFileName("Photo" + ParaData.IrrigationAuditReportId.ToString() + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(ParaData.Photo.FileName)));
                        ParaData.Photo.SaveAs(path);
                        path = Path.Combine("\\Uploading\\IrrigationAuditReport", Path.GetFileName("Photo" + ParaData.IrrigationAuditReportId.ToString() + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(ParaData.Photo.FileName)));
                        ConData.PhotoPath = path;
                    }
                    DB.Entry(ConData);
                    DB.SaveChanges();

                    int Count = 1;
                    if (ParaData.MoreFiles != null && ParaData.MoreFiles.Count != 0)
                    {
                        foreach (var item in ParaData.MoreFiles)
                        {
                            tblControllerAuditReportFile Temp = new tblControllerAuditReportFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading/IrrigationAuditReport"), Path.GetFileName("MorePhoto" + ConData.ControllerAuditReportId.ToString() + Count + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading\\IrrigationAuditReport", Path.GetFileName("MorePhoto" + ConData.ControllerAuditReportId.ToString() + Count + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            Temp.FilePath = path;
                            Temp.FileName = "MorePhoto" + ConData.ControllerAuditReportId.ToString() + Count + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName);
                            Temp.Caption = item.FileName;
                            Temp.ControllerAuditReportId = ConData.ControllerAuditReportId;
                            Temp.CreatedBy = UserId;
                            Temp.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            Temp.EditBy = UserId;
                            Temp.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            Temp.isActive = true;
                            Temp.isDelete = false;
                            DB.tblControllerAuditReportFiles.Add(Temp);
                            Count++;
                        }

                        DB.SaveChanges();
                    }

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Update Controller Audit Report";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();

                    return Ok(new { Id = ConData.ControllerAuditReportId, Message = "Controller audit report has been update successfully." });
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
        public IHttpActionResult DeleteControllerAuditReport(int id)
        {
            tblControllerAuditReport Data = new tblControllerAuditReport();
            var userIdClaim = User.Identity as ClaimsIdentity;
            int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
            try
            {
                Data = DB.tblControllerAuditReports.Select(r => r).Where(x => x.ControllerAuditReportId == id).FirstOrDefault();

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
                LogData.Action = "Delete Controller Audit Report";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("Controller audit report has been deleted successfully.");
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
        public IHttpActionResult DeleteIrrigationAuditReport(int id)
        {
            tblIrrigationAuditReport Data = new tblIrrigationAuditReport();
            var userIdClaim = User.Identity as ClaimsIdentity;
            int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
            try
            {
                Data = DB.tblIrrigationAuditReports.Select(r => r).Where(x => x.IrrigationAuditReportId == id).FirstOrDefault();

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
                LogData.Action = "Delete Irrigation Audit Report";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("Irrigation audit report has been deleted successfully.");
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
