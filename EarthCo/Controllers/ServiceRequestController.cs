using EarthCo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace EarthCo.Controllers
{
    public class ServiceRequestController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();
        [HttpPost]
        public String AddServiceRequest([FromBody] tblServiceRequest ServiceRequest, HttpPostedFile[] Files)
        {
            tblServiceRequest Data = new tblServiceRequest();
            try
            {
                //HttpCookie cookieObj = Request.Cookies["User"];
                //int UserId = Int32.Parse(cookieObj["UserId"]);
                //int RoleId = Int32.Parse(cookieObj["RoleId"]);
                int UserId = 2;
                if (ServiceRequest.ServiceRequestId == 0)
                {
                    Data = ServiceRequest;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = ServiceRequest.isActive;
                    DB.tblServiceRequests.Add(Data);
                    DB.SaveChanges();

                    if (ServiceRequest.tblSRItems != null && ServiceRequest.tblSRItems.Count != 0)
                    {
                        tblSRItem ConData = null;

                        foreach (var item in ServiceRequest.tblSRItems)
                        {
                            ConData = new tblSRItem();
                            ConData = item;
                            ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.CreatedBy = UserId;
                            ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.EditBy = UserId;
                            ConData.isActive = item.isActive;
                            ConData.SRId = Data.ServiceRequestId;
                            DB.tblSRItems.Add(ConData);
                            DB.SaveChanges();
                        }
                    }

                    if(Files != null && Files.Length!=0)
                    {
                        tblSRFile FileData = null;
                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        int NameCount = 1;
                        foreach (var item in Files)
                        {
                            FileData = new tblSRFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("UploadFile" + Data.ServiceRequestId.ToString()+ NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
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
                    LogData.Action = "Add Service Request";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();
                    return "Service Request has been added successfully.";
                }
                else
                {
                    Data = DB.tblServiceRequests.Select(r => r).Where(x => x.ServiceRequestId == ServiceRequest.ServiceRequestId).FirstOrDefault();


                    Data.ServiceRequestNumber = ServiceRequest.ServiceRequestNumber;
                    Data.ServiceLocation = ServiceRequest.ServiceLocation;
                    Data.Contact = ServiceRequest.Contact;
                    Data.JobName = ServiceRequest.JobName;
                    Data.Assign = ServiceRequest.Assign;
                    Data.WorkRequest = ServiceRequest.WorkRequest;
                    Data.ActionTaken = ServiceRequest.ActionTaken;
                    Data.CompletedDate = ServiceRequest.CompletedDate;
                    Data.DueDate = ServiceRequest.DueDate;
                    Data.CustomerId = ServiceRequest.CustomerId;
                    Data.SRTypeId = ServiceRequest.SRTypeId;
                    Data.SRStatusId = ServiceRequest.SRStatusId;
                    Data.SRStatusId = ServiceRequest.SRStatusId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = ServiceRequest.isActive;
                    DB.Entry(Data);
                    DB.SaveChanges();

                    List<tblSRItem> ConList = DB.tblSRItems.Where(x => x.SRId == ServiceRequest.ServiceRequestId).ToList();
                    if (ConList != null && ConList.Count != 0)
                    {
                        DB.tblSRItems.RemoveRange(ConList);
                        DB.SaveChanges();
                    }

                    if (ServiceRequest.tblSRItems != null && ServiceRequest.tblSRItems.Count != 0)
                    {
                        tblSRItem ConData = null;

                        foreach (var item in ServiceRequest.tblSRItems)
                        {
                            ConData = new tblSRItem();
                            ConData = item;
                            ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.CreatedBy = UserId;
                            ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.EditBy = UserId;
                            ConData.isActive = item.isActive;
                            ConData.SRId = Data.ServiceRequestId;
                            DB.tblSRItems.Add(ConData);
                            DB.SaveChanges();
                        }
                    }

                    List<tblSRFile> ConFList = DB.tblSRFiles.Where(x => x.SRId == ServiceRequest.ServiceRequestId).ToList();
                    if (ConFList != null && ConFList.Count != 0)
                    {
                        DB.tblSRFiles.RemoveRange(ConFList);
                        DB.SaveChanges();
                    }

                    if (Files != null && Files.Length != 0)
                    {
                        tblSRFile FileData = null;
                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        int NameCount = 1;
                        foreach (var item in Files)
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

                    return "Estimate has been Update successfully.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpDelete]
        public string DeleteServiceRequest(int id)
        {
            tblServiceRequest Data = new tblServiceRequest();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            int CUserId = 2;
            try
            {

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

                Data = DB.tblServiceRequests.Select(r => r).Where(x => x.ServiceRequestId == id).FirstOrDefault();
                DB.Entry(Data).State = EntityState.Deleted;
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = CUserId;
                LogData.Action = "Delete Service Request";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return "Service Request has been deleted successfully.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
