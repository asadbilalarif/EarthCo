using EarthCo.Models;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EarthCo.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class WeeklyReportController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();

        [HttpGet]
        public IHttpActionResult GetWeeklyReportList()
        {
            try
            {
                List<SPGetWeeklyReportListData_Result> Data = new List<SPGetWeeklyReportListData_Result>();
                Data = DB.SPGetWeeklyReportListData().ToList();
                if (Data == null || Data.Count == 0)
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

        [HttpGet]
        public IHttpActionResult GetWeeklyReport(int id, int CustomerId = 0, int Year = 0, int Month = 0)
        {
            try
            {
                SPGetWeeklyReportData_Result Data = new SPGetWeeklyReportData_Result();
                List<SPGetWeeklyReportFileData_Result> FileData = new List<SPGetWeeklyReportFileData_Result>();
                Data = DB.SPGetWeeklyReportData(id, CustomerId, Year, Month).FirstOrDefault();
                FileData = DB.SPGetWeeklyReportFileData(Data.WeeklyReportId).ToList();
                if (Data == null)
                {
                    //Data = new tblMonthlyLandsacpe();
                    //string userJson = JsonConvert.SerializeObject(Data);
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                    //responseMessage.Content = new StringContent(userJson, Encoding.UTF8, "application/json");
                    return ResponseMessage(responseMessage);
                }

                GetWeeklyReportFiles Result = new GetWeeklyReportFiles();
                Result.Data = Data;
                Result.FileData = FileData;

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
        public IHttpActionResult AddWeeklyReport()
        {
            
            try
            {
                var userIdClaim = User.Identity as ClaimsIdentity;
                int userId = int.Parse(userIdClaim.FindFirst("userid")?.Value);

                var Data1 = HttpContext.Current.Request.Params.Get("WeeklyReportData");
                //HttpPostedFile file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;

                WeeklyReportFile WeeklyReport = new WeeklyReportFile();
                WeeklyReport.Files = new List<HttpPostedFile>();
                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    WeeklyReport.Files.Add(HttpContext.Current.Request.Files[i]); ;
                }

                tblWeeklyReport Data = new tblWeeklyReport();

                WeeklyReport.WeeklyReportData = JsonSerializer.Deserialize<tblWeeklyReport>(Data1);

                if (WeeklyReport.WeeklyReportData.WeeklyReportId == 0)
                {
                    Data = WeeklyReport.WeeklyReportData;
                    Data.CreatedDate = DateTime.Now;
                    Data.CreatedBy = userId;
                    Data.EditDate = DateTime.Now;
                    Data.EditBy = userId;
                    Data.isActive = true;
                    Data.isDelete = false;

                    DB.tblWeeklyReports.Add(Data);
                    DB.SaveChanges();


                    if (WeeklyReport.Files != null && WeeklyReport.Files.Count != 0)
                    {
                        tblWeeklyReportFile FileData = null;
                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading/WeeklyReport"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        int NameCount = 1;
                        foreach (var item in WeeklyReport.Files)
                        {
                            FileData = new tblWeeklyReportFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading/WeeklyReport"), Path.GetFileName("WeeklyReport" + Data.WeeklyReportId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading\\WeeklyReport", Path.GetFileName("WeeklyReport" + Data.WeeklyReportId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            FileData.FileName = "WeeklyReport" + Data.WeeklyReportId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName);
                            FileData.Caption = FileData.FileName;
                            FileData.FilePath = path;
                            FileData.WeeklyReportId = Data.WeeklyReportId;
                            FileData.CreatedBy = userId;
                            FileData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.EditBy = userId;
                            FileData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.isActive = true;
                            FileData.isDelete = false;
                            DB.tblWeeklyReportFiles.Add(FileData);
                            DB.SaveChanges();
                            NameCount++;
                        }
                    }

                    var logData = new tblLog
                    {
                        UserId = userId,
                        Action = "Add Weekly report",
                        CreatedDate = DateTime.Now
                    };

                    DB.tblLogs.Add(logData);
                    DB.SaveChanges();

                    //return Ok("Weekly report has been added successfully.");
                    return Ok(new { Id = Data.WeeklyReportId, Message = "Weekly report has been added successfully." });
                }
                else
                {
                    // Updating an existing customer.
                    Data = DB.tblWeeklyReports.SingleOrDefault(c => c.WeeklyReportId == WeeklyReport.WeeklyReportData.WeeklyReportId);

                    if (Data == null)
                    {
                        return NotFound(); // Customer not found.
                    }

                    Data.CustomerId = WeeklyReport.WeeklyReportData.CustomerId;
                    Data.ServiceLocationId = WeeklyReport.WeeklyReportData.ServiceLocationId;
                    Data.ContactId = WeeklyReport.WeeklyReportData.ContactId;
                    Data.JobName = WeeklyReport.WeeklyReportData.JobName;
                    Data.Notes = WeeklyReport.WeeklyReportData.Notes;
                    Data.AssignTo = WeeklyReport.WeeklyReportData.AssignTo;
                    Data.ReportForWeekOf = WeeklyReport.WeeklyReportData.ReportForWeekOf;
                    Data.Thisweekrotation = WeeklyReport.WeeklyReportData.Thisweekrotation;
                    Data.Nextweekrotation = WeeklyReport.WeeklyReportData.Nextweekrotation;
                    Data.ProposalsCompleted = WeeklyReport.WeeklyReportData.ProposalsCompleted;
                    Data.ProposalsSubmitted = WeeklyReport.WeeklyReportData.ProposalsSubmitted;
                    Data.ProposalsNotes = WeeklyReport.WeeklyReportData.ProposalsNotes;
                    Data.EditDate = DateTime.Now;
                    Data.EditBy = userId;
                    Data.isActive = true;
                    Data.isDelete = false;

                    List<tblWeeklyReportFile> ConFList = DB.tblWeeklyReportFiles.Where(x => x.WeeklyReportId == WeeklyReport.WeeklyReportData.WeeklyReportId).ToList();
                    if (ConFList != null && ConFList.Count != 0)
                    {
                        DB.tblWeeklyReportFiles.RemoveRange(ConFList);
                        DB.SaveChanges();
                    }

                    if (WeeklyReport.Files != null && WeeklyReport.Files.Count != 0)
                    {
                        tblWeeklyReportFile FileData = null;
                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading/WeeklyReport"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        int NameCount = 1;
                        foreach (var item in WeeklyReport.Files)
                        {
                            FileData = new tblWeeklyReportFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading/WeeklyReport"), Path.GetFileName("WeeklyReport" + Data.WeeklyReportId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading\\WeeklyReport", Path.GetFileName("WeeklyReport" + Data.WeeklyReportId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            FileData.FileName = "WeeklyReport" + Data.WeeklyReportId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName);
                            FileData.Caption = FileData.FileName;
                            FileData.FilePath = path;
                            FileData.WeeklyReportId = Data.WeeklyReportId;
                            FileData.CreatedBy = userId;
                            FileData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.EditBy = userId;
                            FileData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.isActive = true;
                            FileData.isDelete = false;
                            DB.tblWeeklyReportFiles.Add(FileData);
                            DB.SaveChanges();
                            NameCount++;
                        }
                    }

                    var logData = new tblLog
                    {
                        UserId = userId,
                        Action = "Update Weekly report",
                        CreatedDate = DateTime.Now
                    };

                    DB.tblLogs.Add(logData);
                    DB.SaveChanges();

                    //return Ok("Weekly report has been updated successfully.");
                    return Ok(new { Id = Data.WeeklyReportId, Message = "Weekly report has been updated successfully." });
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
        public IHttpActionResult DeleteWeeklyReport(int id)
        {
            tblWeeklyReport Data = new tblWeeklyReport();
            var userIdClaim = User.Identity as ClaimsIdentity;
            int CUserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
            //int CUserId = 2;
            try
            {
                Data = DB.tblWeeklyReports.Select(r => r).Where(x => x.WeeklyReportId == id).FirstOrDefault();

                if (Data == null)
                {
                    return NotFound(); // 404 - Customer not found
                }


                //List<tblWeeklyReportFile> ConFList = DB.tblWeeklyReportFiles.Where(x => x.WeeklyReportId == id).ToList();
                //if (ConFList != null && ConFList.Count != 0)
                //{
                //    DB.tblWeeklyReportFiles.RemoveRange(ConFList);
                //    DB.SaveChanges();
                //}

                Data.isDelete = true;
                Data.EditBy = CUserId;
                Data.EditDate = DateTime.Now;
                DB.Entry(Data);
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = CUserId;
                LogData.Action = "Delete Weekly report";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("Weekly report has been deleted successfully.");
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
