using EarthCo.Models;
using System;
using System.Collections.Generic;
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
    public class InvoiceController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();
        [HttpGet]
        public IHttpActionResult GetInvoiceList()
        {
            try
            {
                List<tblInvoice> Data = new List<tblInvoice>();
                List<InvoiceList> Result = new List<InvoiceList>();
                Data = DB.tblInvoices.Where(x => x.isDelete != true).ToList();

                if (Data == null || Data.Count == 0)
                {
                    return NotFound(); // 404 - No data found
                }
                else
                {
                    foreach (tblInvoice item in Data)
                    {
                        InvoiceList Temp = new InvoiceList();
                        Temp.InvoiceNumber = item.InvoiceNumber;
                        Temp.CustomerName = item.tblUser.FirstName + " " + item.tblUser.LastName;
                        Temp.IssueDate = (DateTime)item.IssueDate;
                        Temp.TotalAmount = item.TotalAmount;
                        Temp.BalanceAmount = item.BalanceAmount;
                        Temp.ProfitPercentage = item.ProfitPercentage;
                        Result.Add(Temp);
                    }
                }

                return Ok(Result); // 200 - Successful response with data
            }
            catch (Exception ex)
            {
                // Log the exception
                // You may also choose to return a more specific error response (e.g., 500 - Internal Server Error) here.
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetInvoice(int id)
        {
            try
            {
                SPGetInvoiceData_Result Data = new SPGetInvoiceData_Result();
                List<SPGetInvoiceItemData_Result> ItemData = new List<SPGetInvoiceItemData_Result>();
                List<SPGetInvoiceItemData_Result> CostItemData = new List<SPGetInvoiceItemData_Result>();
                List<SPGetInvoiceFileData_Result> FileData = new List<SPGetInvoiceFileData_Result>();
                Data = DB.SPGetInvoiceData(id).FirstOrDefault();
                ItemData = DB.SPGetInvoiceItemData(id).Where(x => x.isCost == false).ToList();
                CostItemData = DB.SPGetInvoiceItemData(id).Where(x => x.isCost == true).ToList();
                FileData = DB.SPGetInvoiceFileData(id).ToList();

                GetInvoiceData GetData = new GetInvoiceData();

                if (Data == null)
                {
                    //DB.Configuration.ProxyCreationEnabled = false;
                    //Data = new tblInvoice();
                    //Data.tblInvoiceItems = null;
                    //Data.tblInvoices = null;
                    //Data.tblEstimate = null;
                    //Data.tblInvoices = null;
                    //Data.tblInvoiceFiles = null;
                    //Data.tblUser = null;
                    //string userJson = JsonConvert.SerializeObject(Data);
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                    //responseMessage.Content = new StringContent(userJson, Encoding.UTF8, "application/json");
                    return ResponseMessage(responseMessage);
                }
                else
                {
                    GetData.Data = Data;
                    GetData.ItemData = ItemData;
                    GetData.CostItemData = CostItemData;
                    GetData.FileData = FileData;
                }

                return Ok(GetData); // 200 - Successful response with data
            }
            catch (Exception ex)
            {
                // Log the exception
                // You may also choose to return a more specific error response (e.g., 500 - Internal Server Error) here.
                return InternalServerError(ex);
            }

        }

        [HttpPost]
        public IHttpActionResult AddInvoice()
        {
            var Data1 = HttpContext.Current.Request.Params.Get("InvoiceData");
            HttpPostedFile file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;

            InvoiceFile Invoice = new InvoiceFile();
            Invoice.Files = new List<HttpPostedFile>();
            for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
            {
                Invoice.Files.Add(HttpContext.Current.Request.Files[i]); ;
            }

            Invoice.InvoiceData = JsonSerializer.Deserialize<tblInvoice>(Data1);

            tblInvoice Data = new tblInvoice();
            try
            {
                //HttpCookie cookieObj = Request.Cookies["User"];
                //int UserId = Int32.Parse(cookieObj["UserId"]);
                //int RoleId = Int32.Parse(cookieObj["RoleId"]);

                var userIdClaim = User.Identity as ClaimsIdentity;
                int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);

                if (Invoice.InvoiceData.InvoiceId == 0)
                {

                    if (Invoice.InvoiceData.tblInvoiceItems != null && Invoice.InvoiceData.tblInvoiceItems.Count != 0)
                    {
                        foreach (var item in Invoice.InvoiceData.tblInvoiceItems)
                        {
                            item.CreatedBy = UserId;
                            item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.EditBy = UserId;
                            item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.Amount = item.Qty * item.Rate;
                            item.InvoiceId = Data.InvoiceId;
                            item.isActive = true;
                            item.isDelete = false;
                        }
                    }

                    Data = Invoice.InvoiceData;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = true;
                    Data.isDelete = false;
                    DB.tblInvoices.Add(Data);
                    DB.SaveChanges();

                    //if (Estimate.EstimateData.tblEstimateItems!= null && Estimate.EstimateData.tblEstimateItems.Count != 0)
                    //{
                    //    tblEstimateItem ConData = null;

                    //    foreach (var item in Estimate.EstimateData.tblEstimateItems)
                    //    {
                    //        ConData = new tblEstimateItem();
                    //        ConData = item;
                    //        ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    //        ConData.CreatedBy = UserId;
                    //        ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    //        ConData.EditBy = UserId;
                    //        ConData.isActive = item.isActive;
                    //        ConData.EstimateId = Data.EstimateId;
                    //        DB.tblEstimateItems.Add(ConData);
                    //        DB.SaveChanges();
                    //    }
                    //}

                    if (Invoice.Files != null && Invoice.Files.Count != 0)
                    {
                        tblInvoiceFile FileData = null;
                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        int NameCount = 1;
                        foreach (var item in Invoice.Files)
                        {
                            FileData = new tblInvoiceFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("UploadFile" + Data.InvoiceId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading", Path.GetFileName("UploadFile" + Data.InvoiceId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            FileData.FileName = Path.GetFileName(item.FileName);
                            FileData.Caption = FileData.FileName;
                            FileData.FilePath = path;
                            FileData.InvoiceId = Data.InvoiceId;
                            FileData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.CreatedBy = UserId;
                            FileData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.EditBy = UserId;
                            FileData.isActive = true;
                            FileData.isDelete = false;
                            DB.tblInvoiceFiles.Add(FileData);
                            DB.SaveChanges();
                            NameCount++;
                        }
                    }

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Add Invoice";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();
                    //return Ok("Estimate has been added successfully.");
                    return Ok(new { Id = Data.InvoiceId, Message = "Invoice has been added successfully." });
                }
                else
                {
                    Data = DB.tblInvoices.Select(r => r).Where(x => x.InvoiceId == Invoice.InvoiceData.InvoiceId).FirstOrDefault();
                    if (Data == null)
                    {
                        return NotFound(); // Customer not found.
                    }

                    List<tblInvoiceItem> ConList = DB.tblInvoiceItems.Where(x => x.InvoiceId == Invoice.InvoiceData.InvoiceId).ToList();
                    if (ConList != null && ConList.Count != 0)
                    {
                        DB.tblInvoiceItems.RemoveRange(ConList);
                        DB.SaveChanges();
                    }



                    if (Invoice.InvoiceData.tblInvoiceItems != null && Invoice.InvoiceData.tblInvoiceItems.Count != 0)
                    {
                        foreach (var item in Invoice.InvoiceData.tblInvoiceItems)
                        {
                            item.CreatedBy = UserId;
                            item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.EditBy = UserId;
                            item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.Amount = item.Qty * item.Rate;
                            item.InvoiceId = Data.InvoiceId;
                            item.isActive = true;
                            item.isDelete = false;
                        }
                    }


                    Data.CustomerId = Invoice.InvoiceData.CustomerId;
                    Data.ServiceLocationId = Invoice.InvoiceData.ServiceLocationId;
                    Data.ContactId = Invoice.InvoiceData.ContactId;
                    Data.InvoiceNumber = Invoice.InvoiceData.InvoiceNumber;
                    Data.IssueDate = Invoice.InvoiceData.IssueDate;
                    Data.DueDate = Invoice.InvoiceData.DueDate;
                    Data.EstimateId = Invoice.InvoiceData.EstimateId;
                    Data.EstimateNumber = Invoice.InvoiceData.EstimateNumber;
                    Data.CustomerMessage = Invoice.InvoiceData.CustomerMessage;
                    Data.PrivateNotes = Invoice.InvoiceData.PrivateNotes;
                    Data.TotalAmount = Invoice.InvoiceData.TotalAmount;
                    Data.BalanceAmount = Invoice.InvoiceData.BalanceAmount;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = true;
                    DB.Entry(Data);
                    DB.SaveChanges();

                    if (Invoice.InvoiceData.tblInvoiceItems != null && Invoice.InvoiceData.tblInvoiceItems.Count != 0)
                    {
                        tblInvoiceItem ConData = null;

                        foreach (var item in Invoice.InvoiceData.tblInvoiceItems)
                        {
                            ConData = new tblInvoiceItem();
                            ConData = item;
                            ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.CreatedBy = UserId;
                            ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.EditBy = UserId;
                            ConData.isActive = true;
                            ConData.isDelete = false;
                            ConData.isCost = item.isCost;
                            ConData.InvoiceId = Data.InvoiceId;
                            DB.tblInvoiceItems.Add(ConData);
                            DB.SaveChanges();
                        }
                    }

                    List<tblInvoiceFile> ConFList = DB.tblInvoiceFiles.Where(x => x.InvoiceId == Invoice.InvoiceData.InvoiceId).ToList();
                    if (ConFList != null && ConFList.Count != 0)
                    {
                        DB.tblInvoiceFiles.RemoveRange(ConFList);
                        DB.SaveChanges();
                    }

                    if (Invoice.Files != null && Invoice.Files.Count != 0)
                    {
                        tblInvoiceFile FileData = null;
                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        int NameCount = 1;
                        foreach (var item in Invoice.Files)
                        {
                            FileData = new tblInvoiceFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("UploadFile" + Data.InvoiceId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading", Path.GetFileName("UploadFile" + Data.InvoiceId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            FileData.FileName = Path.GetFileName(item.FileName);
                            FileData.Caption = "";
                            FileData.FilePath = path;
                            FileData.InvoiceId = Data.InvoiceId;
                            FileData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.CreatedBy = UserId;
                            FileData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.EditBy = UserId;
                            FileData.isActive = true;
                            FileData.isDelete = false;
                            DB.tblInvoiceFiles.Add(FileData);
                            DB.SaveChanges();
                            NameCount++;
                        }
                    }


                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Update Invoice";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();

                    //return Ok("Purchase Order has been Update successfully.");
                    return Ok(new { Id = Data.InvoiceId, Message = "Invoice has been Update successfully." });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            return NotFound();
        }

        [HttpGet]
        public IHttpActionResult DeleteInvoice(int id)
        {
            tblInvoice Data = new tblInvoice();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);

            var userIdClaim = User.Identity as ClaimsIdentity;
            int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
            try
            {
                Data = DB.tblInvoices.Select(r => r).Where(x => x.InvoiceId == id).FirstOrDefault();
                if (Data == null)
                {
                    return NotFound(); // 404 - Customer not found
                }


                //List<tblInvoiceFile> ConFile = DB.tblInvoiceFiles.Where(x => x.InvoiceId == id).ToList();
                //if (ConFile != null && ConFile.Count != 0)
                //{
                //    DB.tblInvoiceFiles.RemoveRange(ConFile);
                //    DB.SaveChanges();
                //}

                //List<tblInvoiceItem> ConList = DB.tblInvoiceItems.Where(x => x.InvoiceId == id).ToList();
                //if (ConList != null && ConList.Count != 0)
                //{
                //    DB.tblInvoiceItems.RemoveRange(ConList);
                //    DB.SaveChanges();
                //}


                Data.isDelete = true;
                Data.EditBy = UserId;
                Data.EditDate = DateTime.Now;
                DB.Entry(Data);
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = UserId;
                LogData.Action = "Delete Invoice";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("Invoice has been deleted successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
