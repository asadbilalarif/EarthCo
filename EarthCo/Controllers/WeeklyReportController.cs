using EarthCo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EarthCo.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class WeeklyReportController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();

        [HttpGet]
        public IHttpActionResult GetWeeklyReportList()
        {
            try
            {
                List<tblWeeklyReport> Data = new List<tblWeeklyReport>();
                Data = DB.tblWeeklyReports.ToList();
                if (Data == null || Data.Count == 0)
                {
                    return NotFound();
                }

                return Ok(Data);
            }
            catch (Exception ex)
            {
                // Log the exception
                // You may also choose to return a more specific error response (e.g., 500 - Internal Server Error) here.
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetWeeklyReport(int id)
        {
            try
            {
                tblWeeklyReport Data = new tblWeeklyReport();
                Data = DB.tblWeeklyReports.Where(x => x.WeeklyReportId == id).FirstOrDefault();
                if (Data == null)
                {
                    Data = new tblWeeklyReport();;
                    string userJson = JsonConvert.SerializeObject(Data);
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                    responseMessage.Content = new StringContent(userJson, Encoding.UTF8, "application/json");
                    return ResponseMessage(responseMessage);
                }

                return Ok(Data); 
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        [HttpPost]
        public IHttpActionResult AddWeeklyReport([FromBody] tblWeeklyReport WeeklyReport,List<HttpPostedFileBase> Files)
        {
            tblWeeklyReport Data = new tblWeeklyReport();
            try
            {
                //var userIdClaim = User.Identity as ClaimsIdentity;
                //int userId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                int userId = 2; // Replace with your authentication mechanism to get the user's ID.

                if (WeeklyReport.WeeklyReportId == 0)
                {
                    Data = WeeklyReport;
                    Data.CreatedDate = DateTime.Now;
                    Data.CreatedBy = userId;
                    Data.EditDate = DateTime.Now;
                    Data.EditBy = userId;
                    Data.isActive = WeeklyReport.isActive;

                    DB.tblWeeklyReports.Add(Data);
                    DB.SaveChanges();

                    if (Files != null && Files.Count != 0)
                    {
                        tblWeeklyReportFile FileData = null;
                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        int NameCount = 1;
                        foreach (var item in Files)
                        {
                            FileData = new tblWeeklyReportFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("UploadFile" + Data.WeeklyReportId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading", Path.GetFileName("UploadFile" + Data.WeeklyReportId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            FileData.FileName = Path.GetFileName(item.FileName);
                            FileData.Caption = FileData.FileName;
                            FileData.FilePath = path;
                            FileData.WeeklyReportId = Data.WeeklyReportId;
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

                    return Ok("Weekly report has been added successfully.");
                }
                else
                {
                    // Updating an existing customer.
                    Data = DB.tblWeeklyReports.SingleOrDefault(c => c.WeeklyReportId == WeeklyReport.WeeklyReportId);

                    if (Data == null)
                    {
                        return NotFound(); // Customer not found.
                    }

                    Data.CustomerId = WeeklyReport.CustomerId;
                    Data.ServiceRequestId = WeeklyReport.ServiceRequestId;
                    Data.ContactId = WeeklyReport.ContactId;
                    Data.JobName = WeeklyReport.JobName;
                    Data.Notes = WeeklyReport.Notes;
                    Data.AssignTo = WeeklyReport.AssignTo;
                    Data.ReportForWeekOf = WeeklyReport.ReportForWeekOf;
                    Data.Thisweekrotation = WeeklyReport.Thisweekrotation;
                    Data.Nextweekrotation = WeeklyReport.Nextweekrotation;
                    Data.ProposalsCompleted = WeeklyReport.ProposalsCompleted;
                    Data.ProposalsSubmitted = WeeklyReport.ProposalsSubmitted;
                    Data.ProposalsNotes = WeeklyReport.ProposalsNotes;
                    Data.EditDate = DateTime.Now;
                    Data.EditBy = userId;
                    Data.isActive = WeeklyReport.isActive;

                    List<tblWeeklyReportFile> ConFList = DB.tblWeeklyReportFiles.Where(x => x.WeeklyReportId == WeeklyReport.WeeklyReportId).ToList();
                    if (ConFList != null && ConFList.Count != 0)
                    {
                        DB.tblWeeklyReportFiles.RemoveRange(ConFList);
                        DB.SaveChanges();
                    }

                    if (Files != null && Files.Count != 0)
                    {
                        tblWeeklyReportFile FileData = null;
                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        int NameCount = 1;
                        foreach (var item in Files)
                        {
                            FileData = new tblWeeklyReportFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("UploadFile" + Data.WeeklyReportId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading", Path.GetFileName("UploadFile" + Data.WeeklyReportId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            FileData.FileName = Path.GetFileName(item.FileName);
                            FileData.Caption = "";
                            FileData.FilePath = path;
                            FileData.WeeklyReportId = Data.WeeklyReportId;
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

                    return Ok("Weekly report has been updated successfully.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                return InternalServerError(ex); // 500 - Internal Server Error
            }
        }

        [HttpGet]
        public IHttpActionResult DeleteWeeklyReport(int id)
        {
            tblWeeklyReport Data = new tblWeeklyReport();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            int CUserId = 2;
            try
            {
                Data = DB.tblWeeklyReports.Select(r => r).Where(x => x.WeeklyReportId == id).FirstOrDefault();

                if (Data == null)
                {
                    return NotFound(); // 404 - Customer not found
                }


                List<tblWeeklyReportFile> ConFList = DB.tblWeeklyReportFiles.Where(x => x.WeeklyReportId == id).ToList();
                if (ConFList != null && ConFList.Count != 0)
                {
                    DB.tblWeeklyReportFiles.RemoveRange(ConFList);
                    DB.SaveChanges();
                }


                DB.Entry(Data).State = EntityState.Deleted;
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = CUserId;
                LogData.Action = "Delete Weekly report";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("Weekly report has been deleted successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}
