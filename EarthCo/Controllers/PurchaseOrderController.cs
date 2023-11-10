﻿using EarthCo.Models;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Security.Claims;

namespace EarthCo.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class PurchaseOrderController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();
        [HttpGet]
        public IHttpActionResult GetPurchaseOrderList()
        {
            try
            {
                List<tblPurchaseOrder> Data = new List<tblPurchaseOrder>();
                List<PurchaseOrderList> Result = new List<PurchaseOrderList>();
                Data = DB.tblPurchaseOrders.Where(x => x.isDelete != true).ToList();

                if (Data == null || Data.Count == 0)
                {
                    return NotFound(); // 404 - No data found
                }
                else
                {
                    foreach (tblPurchaseOrder item in Data)
                    {
                        PurchaseOrderList Temp = new PurchaseOrderList();
                        Temp.SupplierName = item.tblUser.FirstName + " " + item.tblUser.LastName;
                        Temp.Date =(DateTime) item.Date;
                        Temp.Status =item.tblPurchaseOrderStatu.Status;
                        Temp.RegionalManager = item.tblUser1.FirstName + " " + item.tblUser1.LastName;
                        Temp.RequestedBy = item.tblUser2.FirstName + " " + item.tblUser2.LastName;
                        Temp.EstimateNumber = item.EstimateNumber;
                        Temp.BillNumber = item.BillNumber;
                        Temp.InvoiceNumber = item.InvoiceNumber;
                        Temp.Amount = item.Amount;
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
        public IHttpActionResult GetPurchaseOrder(int id)
        {
            try
            {
                SPGetPurchaseOrderData_Result Data = new SPGetPurchaseOrderData_Result();
                List<SPGetPurchaseOrderItemData_Result> ItemData = new List<SPGetPurchaseOrderItemData_Result>();
                List<SPGetPurchaseOrderFileData_Result> FileData = new List<SPGetPurchaseOrderFileData_Result>();
                Data = DB.SPGetPurchaseOrderData(id).FirstOrDefault();
                ItemData = DB.SPGetPurchaseOrderItemData(id).ToList();
                FileData = DB.SPGetPurchaseOrderFileData(id).ToList();


                GetPurchaseOrderData GetData = new GetPurchaseOrderData();
                if (Data == null)
                {
                    //DB.Configuration.ProxyCreationEnabled = false;
                    //Data = new tblPurchaseOrder();
                    //Data.tblPurchaseOrderItems = null;
                    //Data.tblInvoices = null;
                    //Data.tblEstimate = null;
                    //Data.tblBills = null;
                    //Data.tblPurchaseOrderFiles = null;
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
            catch (Exception ex)
            {
                // Log the exception
                // You may also choose to return a more specific error response (e.g., 500 - Internal Server Error) here.
                return InternalServerError(ex);
            }

        }

        [HttpGet]
        public IHttpActionResult GetPurchaseOrderStatus()
        {
            try
            {
                DB.Configuration.ProxyCreationEnabled = false;
                List<tblPurchaseOrderStatu> Data = new List<tblPurchaseOrderStatu>();
                Data = DB.tblPurchaseOrderStatus.ToList();
                if (Data == null || Data.Count == 0)
                {
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                    return ResponseMessage(responseMessage);
                }

                return Ok(Data); // 200 - Successful response with data
            }
            catch (Exception ex)
            {
                // Log the exception
                // You may also choose to return a more specific error response (e.g., 500 - Internal Server Error) here.
                return InternalServerError(ex);
            }

        }

        [HttpPost]
        public IHttpActionResult AddPurchaseOrder()
        {
            var Data1 = HttpContext.Current.Request.Params.Get("PurchaseOrderData");
            HttpPostedFile file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;

            PurchaseOrderFile PurchaseOrder = new PurchaseOrderFile();
            PurchaseOrder.Files = new List<HttpPostedFile>();
            for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
            {
                PurchaseOrder.Files.Add(HttpContext.Current.Request.Files[i]); ;
            }

            PurchaseOrder.PurchaseOrderData = JsonSerializer.Deserialize<tblPurchaseOrder>(Data1);

            tblPurchaseOrder Data = new tblPurchaseOrder();
            try
            {
                //HttpCookie cookieObj = Request.Cookies["User"];
                //int UserId = Int32.Parse(cookieObj["UserId"]);
                //int RoleId = Int32.Parse(cookieObj["RoleId"]);

                var userIdClaim = User.Identity as ClaimsIdentity;
                int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);

                if (PurchaseOrder.PurchaseOrderData.PurchaseOrderId == 0)
                {

                    if (PurchaseOrder.PurchaseOrderData.tblPurchaseOrderItems != null && PurchaseOrder.PurchaseOrderData.tblPurchaseOrderItems.Count != 0)
                    {
                        foreach (var item in PurchaseOrder.PurchaseOrderData.tblPurchaseOrderItems)
                        {
                            item.CreatedBy = UserId;
                            item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.EditBy = UserId;
                            item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.Amount = item.Qty * item.Rate;
                            item.PurchaseOrderId = Data.PurchaseOrderId;
                            item.isActive = true;
                            item.isDelete = false;
                        }
                    }

                    Data = PurchaseOrder.PurchaseOrderData;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = true;
                    Data.isDelete = false;
                    DB.tblPurchaseOrders.Add(Data);
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

                    if (PurchaseOrder.Files != null && PurchaseOrder.Files.Count != 0)
                    {
                        tblPurchaseOrderFile FileData = null;
                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        int NameCount = 1;
                        foreach (var item in PurchaseOrder.Files)
                        {
                            FileData = new tblPurchaseOrderFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("UploadFile" + Data.PurchaseOrderId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading", Path.GetFileName("UploadFile" + Data.PurchaseOrderId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            FileData.FileName = Path.GetFileName(item.FileName);
                            FileData.Caption = FileData.FileName;
                            FileData.FilePath = path;
                            FileData.PurchaseOrderId = Data.PurchaseOrderId;
                            FileData.CreatedBy = UserId;
                            FileData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.EditBy = UserId;
                            FileData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.isActive = true;
                            DB.tblPurchaseOrderFiles.Add(FileData);
                            DB.SaveChanges();
                            NameCount++;
                        }
                    }

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Add Purchase Order";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();
                    //return Ok("Estimate has been added successfully.");
                    return Ok(new { Id = Data.PurchaseOrderId, Message = "Purchase Order has been added successfully." });
                }
                else
                {
                    Data = DB.tblPurchaseOrders.Select(r => r).Where(x => x.PurchaseOrderId == PurchaseOrder.PurchaseOrderData.PurchaseOrderId).FirstOrDefault();
                    if (Data == null)
                    {
                        return NotFound(); // Customer not found.
                    }

                    List<tblPurchaseOrderItem> ConList = DB.tblPurchaseOrderItems.Where(x => x.PurchaseOrderId == PurchaseOrder.PurchaseOrderData.PurchaseOrderId).ToList();
                    if (ConList != null && ConList.Count != 0)
                    {
                        DB.tblPurchaseOrderItems.RemoveRange(ConList);
                        DB.SaveChanges();
                    }



                    if (PurchaseOrder.PurchaseOrderData.tblPurchaseOrderItems != null && PurchaseOrder.PurchaseOrderData.tblPurchaseOrderItems.Count != 0)
                    {
                        foreach (var item in PurchaseOrder.PurchaseOrderData.tblPurchaseOrderItems)
                        {
                            item.CreatedBy = UserId;
                            item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.EditBy = UserId;
                            item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.Amount = item.Qty * item.Rate;
                            item.PurchaseOrderId = Data.PurchaseOrderId;
                            item.isActive = true;
                            item.isDelete = false;
                        }
                    }


                    Data.SupplierId = PurchaseOrder.PurchaseOrderData.SupplierId;
                    Data.PurchaseOrderNumber = PurchaseOrder.PurchaseOrderData.PurchaseOrderNumber;
                    Data.Tags = PurchaseOrder.PurchaseOrderData.Tags;
                    Data.Date = PurchaseOrder.PurchaseOrderData.Date;
                    Data.DueDate = PurchaseOrder.PurchaseOrderData.DueDate;
                    Data.RegionalManager = PurchaseOrder.PurchaseOrderData.RegionalManager;
                    Data.TermId = PurchaseOrder.PurchaseOrderData.TermId;
                    Data.Requestedby = PurchaseOrder.PurchaseOrderData.Requestedby;
                    Data.StatusId = PurchaseOrder.PurchaseOrderData.StatusId;
                    Data.InvoiceNumber = PurchaseOrder.PurchaseOrderData.InvoiceNumber;
                    Data.InvoiceId = PurchaseOrder.PurchaseOrderData.InvoiceId;
                    Data.BillNumber = PurchaseOrder.PurchaseOrderData.BillNumber;
                    Data.BillId = PurchaseOrder.PurchaseOrderData.BillId;
                    Data.EstimateNumber = PurchaseOrder.PurchaseOrderData.EstimateNumber;
                    Data.EstimateId = PurchaseOrder.PurchaseOrderData.EstimateId;
                    Data.MemoInternal = PurchaseOrder.PurchaseOrderData.MemoInternal;
                    Data.Message = PurchaseOrder.PurchaseOrderData.Message;
                    Data.ShipTo = PurchaseOrder.PurchaseOrderData.ShipTo;
                    Data.Amount = PurchaseOrder.PurchaseOrderData.Amount;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = true;
                    Data.isDelete = false;
                    DB.Entry(Data);
                    DB.SaveChanges();

                    if (PurchaseOrder.PurchaseOrderData.tblPurchaseOrderItems != null && PurchaseOrder.PurchaseOrderData.tblPurchaseOrderItems.Count != 0)
                    {
                        tblPurchaseOrderItem ConData = null;

                        foreach (var item in PurchaseOrder.PurchaseOrderData.tblPurchaseOrderItems)
                        {
                            ConData = new tblPurchaseOrderItem();
                            ConData = item;
                            ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.CreatedBy = UserId;
                            ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.EditBy = UserId;
                            ConData.isActive = true;
                            ConData.isDelete = false;
                            ConData.PurchaseOrderId = Data.PurchaseOrderId;
                            DB.tblPurchaseOrderItems.Add(ConData);
                            DB.SaveChanges();
                        }
                    }

                    List<tblPurchaseOrderFile> ConFList = DB.tblPurchaseOrderFiles.Where(x => x.PurchaseOrderId == PurchaseOrder.PurchaseOrderData.PurchaseOrderId).ToList();
                    if (ConFList != null && ConFList.Count != 0)
                    {
                        DB.tblPurchaseOrderFiles.RemoveRange(ConFList);
                        DB.SaveChanges();
                    }

                    if (PurchaseOrder.Files != null && PurchaseOrder.Files.Count != 0)
                    {
                        tblPurchaseOrderFile FileData = null;
                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        int NameCount = 1;
                        foreach (var item in PurchaseOrder.Files)
                        {
                            FileData = new tblPurchaseOrderFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("UploadFile" + Data.PurchaseOrderId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading", Path.GetFileName("UploadFile" + Data.PurchaseOrderId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            FileData.FileName = Path.GetFileName(item.FileName);
                            FileData.Caption = "";
                            FileData.FilePath = path;
                            FileData.PurchaseOrderId = Data.PurchaseOrderId;
                            FileData.CreatedBy = UserId;
                            FileData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.EditBy = UserId;
                            FileData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.isActive = true;
                            DB.tblPurchaseOrderFiles.Add(FileData);
                            DB.SaveChanges();
                            NameCount++;
                        }
                    }


                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Update Purchase Order";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();

                    //return Ok("Purchase Order has been Update successfully.");
                    return Ok(new { Id = Data.PurchaseOrderId, Message = "Purchase Order has been Update successfully." });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            return NotFound();
        }

        [HttpGet]
        public IHttpActionResult DeletePurchaseOrder(int id)
        {
            tblPurchaseOrder Data = new tblPurchaseOrder();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);

            var userIdClaim = User.Identity as ClaimsIdentity;
            int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
            try
            {
                Data = DB.tblPurchaseOrders.Select(r => r).Where(x => x.PurchaseOrderId == id).FirstOrDefault();
                if (Data == null)
                {
                    return NotFound(); // 404 - Customer not found
                }


                //List<tblPurchaseOrderFile> ConFile = DB.tblPurchaseOrderFiles.Where(x => x.PurchaseOrderId == id).ToList();
                //if (ConFile != null && ConFile.Count != 0)
                //{
                //    DB.tblPurchaseOrderFiles.RemoveRange(ConFile);
                //    DB.SaveChanges();
                //}

                //List<tblPurchaseOrderItem> ConList = DB.tblPurchaseOrderItems.Where(x => x.PurchaseOrderId == id).ToList();
                //if (ConList != null && ConList.Count != 0)
                //{
                //    DB.tblPurchaseOrderItems.RemoveRange(ConList);
                //    DB.SaveChanges();
                //}


                Data.isDelete = true;
                Data.EditBy = UserId;
                Data.EditDate = DateTime.Now;
                DB.Entry(Data);
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = UserId;
                LogData.Action = "Delete Purchase Order";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("Purchase Order has been deleted successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}