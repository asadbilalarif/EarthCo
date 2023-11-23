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
    public class BillController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();
        [HttpGet]
        public IHttpActionResult GetBillList()
        {
            try
            {
                List<tblBill> Data = new List<tblBill>();
                List<BillList> Result = new List<BillList>();
                Data = DB.tblBills.Where(x => x.isDelete != true).ToList();

                if (Data == null || Data.Count == 0)
                {
                    return NotFound(); // 404 - No data found
                }
                else
                {
                    foreach (tblBill item in Data)
                    {
                        BillList Temp = new BillList();
                        Temp.BillId = item.BillId;
                        Temp.BillNumber = item.BillNumber;
                        Temp.SupplierName = item.tblUser.FirstName + " " + item.tblUser.LastName;
                        if(item.DueDate!=null)
                        {
                            Temp.DueDate = (DateTime)item.DueDate;
                        }
                        
                        Temp.Amount = item.Amount;
                        if (item.Memo != null)
                        {
                            Temp.Memo = item.Memo;
                        }
                        if (item.Currency != null)
                        {
                            Temp.Currency = item.Currency;
                        }
                        if (item.Tags != null)
                        {
                            Temp.Tags = item.Tags;
                        }
                        
                        
                        
                        Result.Add(Temp);
                    }
                }

                return Ok(Result); // 200 - Successful response with data
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
        public IHttpActionResult GetBill(int id)
        {
            try
            {
                SPGetBillData_Result Data = new SPGetBillData_Result();
                List<SPGetBillItemData_Result> ItemData = new List<SPGetBillItemData_Result>();
                List<SPGetBillFileData_Result> FileData = new List<SPGetBillFileData_Result>();
                Data = DB.SPGetBillData(id).FirstOrDefault();
                ItemData = DB.SPGetBillItemData(id).ToList();
                FileData = DB.SPGetBillFileData(id).ToList();

                GetBillData GetData = new GetBillData();
                if (Data == null)
                {
                    //DB.Configuration.ProxyCreationEnabled = false;
                    //Data = new tblBill();
                    //Data.tblBillItems = null;
                    //Data.tblInvoices = null;
                    //Data.tblEstimate = null;
                    //Data.tblBills = null;
                    //Data.tblBillFiles = null;
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
                    GetData.FileData = FileData;
                }

                return Ok(GetData); // 200 - Successful response with data
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
        public IHttpActionResult AddBill()
        {
            var Data1 = HttpContext.Current.Request.Params.Get("BillData");
            HttpPostedFile file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;

            BillFile Bill = new BillFile();
            Bill.Files = new List<HttpPostedFile>();
            for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
            {
                Bill.Files.Add(HttpContext.Current.Request.Files[i]); ;
            }

            Bill.BillData = JsonSerializer.Deserialize<tblBill>(Data1);

            tblBill Data = new tblBill();
            tblBill CheckData = new tblBill();
            try
            {
                //HttpCookie cookieObj = Request.Cookies["User"];
                //int UserId = Int32.Parse(cookieObj["UserId"]);
                //int RoleId = Int32.Parse(cookieObj["RoleId"]);

                var userIdClaim = User.Identity as ClaimsIdentity;
                int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);

                if (Bill.BillData.BillId == 0)
                {


                    CheckData = DB.tblBills.Where(x => x.BillNumber == Bill.BillData.BillNumber&& x.BillNumber !=null && x.BillNumber != "").FirstOrDefault();

                    if (CheckData != null)
                    {
                        var responseMessage = new HttpResponseMessage(HttpStatusCode.Conflict);
                        responseMessage.Content = new StringContent("Bill number already exsist.");
                        return ResponseMessage(responseMessage);
                    }


                    if (Bill.BillData.tblBillItems != null && Bill.BillData.tblBillItems.Count != 0)
                    {
                        foreach (var item in Bill.BillData.tblBillItems)
                        {
                            item.CreatedBy = UserId;
                            item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.EditBy = UserId;
                            item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.Amount = item.Qty * item.Rate;
                            item.BillId = Data.BillId;
                            item.isActive = true;
                            item.isDelete = false;
                        }
                    }

                    Data = Bill.BillData;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = true;
                    Data.isDelete = false;
                    DB.tblBills.Add(Data);
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

                    if (Bill.Files != null && Bill.Files.Count != 0)
                    {
                        tblBillFile FileData = null;
                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        int NameCount = 1;
                        foreach (var item in Bill.Files)
                        {
                            FileData = new tblBillFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("UploadFile" + Data.BillId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading", Path.GetFileName("UploadFile" + Data.BillId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            FileData.FileName = Path.GetFileName(item.FileName);
                            FileData.Caption = FileData.FileName;
                            FileData.FilePath = path;
                            FileData.BillId = Data.BillId;
                            FileData.CreatedBy = UserId;
                            FileData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.EditBy = UserId;
                            FileData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.isActive = true;
                            FileData.isDelete = false;
                            DB.tblBillFiles.Add(FileData);
                            DB.SaveChanges();
                            NameCount++;
                        }
                    }

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Add Bill";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();
                    //return Ok("Estimate has been added successfully.");
                    return Ok(new { Id = Data.BillId, Message = "Bill has been added successfully." });
                }
                else
                {
                    Data = DB.tblBills.Select(r => r).Where(x => x.BillId == Bill.BillData.BillId).FirstOrDefault();
                    if (Data == null)
                    {
                        return NotFound(); // Customer not found.
                    }


                    CheckData = DB.tblBills.Where(x => x.BillNumber == Bill.BillData.BillNumber && x.BillNumber != null && x.BillNumber != "").FirstOrDefault();
                    if (CheckData != null && CheckData.BillId != Data.BillId)
                    {
                        var responseMessage = new HttpResponseMessage(HttpStatusCode.Conflict);
                        responseMessage.Content = new StringContent("Bill number already exsist.");
                        return ResponseMessage(responseMessage);
                    }


                    List<tblBillItem> ConList = DB.tblBillItems.Where(x => x.BillId == Bill.BillData.BillId).ToList();
                    if (ConList != null && ConList.Count != 0)
                    {
                        DB.tblBillItems.RemoveRange(ConList);
                        DB.SaveChanges();
                    }



                    if (Bill.BillData.tblBillItems != null && Bill.BillData.tblBillItems.Count != 0)
                    {
                        foreach (var item in Bill.BillData.tblBillItems)
                        {
                            item.CreatedBy = UserId;
                            item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.EditBy = UserId;
                            item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.Amount = item.Qty * item.Rate;
                            item.BillId = Data.BillId;
                            item.isDelete = false;
                        }
                    }


                    Data.SupplierId = Bill.BillData.SupplierId;
                    Data.BillNumber = Bill.BillData.BillNumber;
                    Data.Tags = Bill.BillData.Tags;
                    Data.BillDate = Bill.BillData.BillDate;
                    Data.DueDate = Bill.BillData.DueDate;
                    Data.PurchaseOrderId = Bill.BillData.PurchaseOrderId;
                    Data.PurchaseOrderNumber = Bill.BillData.PurchaseOrderNumber;
                    Data.TermId = Bill.BillData.TermId;
                    Data.Memo = Bill.BillData.Memo;
                    Data.Amount = Bill.BillData.Amount;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = true;
                    Data.isDelete = false;
                    DB.Entry(Data);
                    DB.SaveChanges();

                    if (Bill.BillData.tblBillItems != null && Bill.BillData.tblBillItems.Count != 0)
                    {
                        tblBillItem ConData = null;

                        foreach (var item in Bill.BillData.tblBillItems)
                        {
                            ConData = new tblBillItem();
                            ConData = item;
                            ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.CreatedBy = UserId;
                            ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.EditBy = UserId;
                            ConData.isActive = true;
                            ConData.isDelete = false;
                            ConData.BillId = Data.BillId;
                            DB.tblBillItems.Add(ConData);
                            DB.SaveChanges();
                        }
                    }

                    List<tblBillFile> ConFList = DB.tblBillFiles.Where(x => x.BillId == Bill.BillData.BillId).ToList();
                    if (ConFList != null && ConFList.Count != 0)
                    {
                        DB.tblBillFiles.RemoveRange(ConFList);
                        DB.SaveChanges();
                    }

                    if (Bill.Files != null && Bill.Files.Count != 0)
                    {
                        tblBillFile FileData = null;
                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        int NameCount = 1;
                        foreach (var item in Bill.Files)
                        {
                            FileData = new tblBillFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("UploadFile" + Data.BillId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading", Path.GetFileName("UploadFile" + Data.BillId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            FileData.FileName = Path.GetFileName(item.FileName);
                            FileData.Caption = "";
                            FileData.FilePath = path;
                            FileData.BillId = Data.BillId;
                            FileData.CreatedBy = UserId;
                            FileData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.EditBy = UserId;
                            FileData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.isActive = true;
                            FileData.isDelete = false;
                            DB.tblBillFiles.Add(FileData);
                            DB.SaveChanges();
                            NameCount++;
                        }
                    }


                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Update Bill";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();

                    //return Ok("Purchase Order has been Update successfully.");
                    return Ok(new { Id = Data.BillId, Message = "Bill has been Update successfully." });
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
            return NotFound();
        }

        [HttpGet]
        public IHttpActionResult DeleteBill(int id)
        {
            tblBill Data = new tblBill();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);

            var userIdClaim = User.Identity as ClaimsIdentity;
            int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
            try
            {
                Data = DB.tblBills.Select(r => r).Where(x => x.BillId == id).FirstOrDefault();
                if (Data == null)
                {
                    return NotFound(); // 404 - Customer not found
                }


                //List<tblBillFile> ConFile = DB.tblBillFiles.Where(x => x.BillId == id).ToList();
                //if (ConFile != null && ConFile.Count != 0)
                //{
                //    DB.tblBillFiles.RemoveRange(ConFile);
                //    DB.SaveChanges();
                //}

                //List<tblBillItem> ConList = DB.tblBillItems.Where(x => x.BillId == id).ToList();
                //if (ConList != null && ConList.Count != 0)
                //{
                //    DB.tblBillItems.RemoveRange(ConList);
                //    DB.SaveChanges();
                //}


                Data.isDelete = true;
                Data.EditBy = UserId;
                Data.EditDate = DateTime.Now;
                DB.Entry(Data);
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = UserId;
                LogData.Action = "Delete Bill";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("Bill has been deleted successfully.");
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
