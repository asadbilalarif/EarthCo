using EarthCo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using static EarthCo.Models.EstimateQB;
using static EarthCo.Models.PurchaseOrderQB;
using static EarthCo.Models.QBBillCUClass;
using static EarthCo.Models.QBErrorClass;
using static EarthCo.Models.QBInvoiceCUClass;
using static EarthCo.Models.QBItemCUClass;
using static EarthCo.Models.QBPurchaseOrderCUClass;
using static EarthCo.Models.SyncQB;

namespace EarthCo.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SyncQBController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();

        [HttpPost]
        public IHttpActionResult SyncDataAPI()
        {
            try
            {
                List<tblSyncLog> SyncLogData = DB.tblSyncLogs.Where(x => x.isSync != true).ToList();

                foreach (tblSyncLog item in SyncLogData)
                {
                    if(item.Name=="Estimate")
                    {
                        SyncEstimateAsync(item);
                    }
                    else if(item.Name == "PurchaseOrder")
                    {
                        SyncPurchaseOrderAsync(item);
                    }
                    else if(item.Name == "Invoice")
                    {
                        SyncInvoiceAsync(item);
                    }
                    else if(item.Name == "Item")
                    {
                        SyncItemAsync(item);
                    }
                }
                
                return Ok("Webhook request is valid.");
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
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return ResponseMessage(responseMessage);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An exception occurred: {ex.Message}");
                // Additional handling for generic exceptions

                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = ex.InnerException != null && ex.InnerException.InnerException != null ? new StringContent(ex.InnerException.InnerException.Message) : new StringContent(ex.Message);
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return ResponseMessage(responseMessage);
            }

        }


        [HttpPost]
        public async Task SyncEstimateAsync(tblSyncLog SyncLog)
        {
            try
            {
                tblEstimate Data = new tblEstimate();
                List<tblEstimateItem> ItemData = new List<tblEstimateItem>();
                List<tblEstimateFile> FileData = new List<tblEstimateFile>();
                if(SyncLog.Operation=="Create")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblEstimates.Where(x => x.EstimateId == SyncLog.Id).FirstOrDefault();
                        ItemData = DB.tblEstimateItems.Where(x => x.EstimateId == SyncLog.Id).ToList();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            DB = new earthcoEntities();
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/"+TokenData.realmId+"/estimate?minorversion=23";

                        QBEstimateClass EsitimateData = new QBEstimateClass();
                        Models.EstimateQB.Line LineData = new Models.EstimateQB.Line();
                        EsitimateData.BillEmail = new BillEmail();
                        EsitimateData.BillEmail.Address = Data.tblUser.Email;
                        EsitimateData.TotalAmt =(decimal) Data.TotalAmount;
                        //EsitimateData.SyncToken = "0";
                        //EsitimateData.Id = "1103";
                        EsitimateData.CustomerRef = new EstimateQB.CustomerRef();
                        EsitimateData.CustomerRef.value = Data.tblUser.QBId.ToString();
                        //EsitimateData.CustomerRef.name = "Cool Cars";

                        foreach (var item in ItemData)
                        {
                            //LineData.Id = "1";
                            LineData.Description = item.Description;
                            LineData.Amount =Convert.ToDecimal(item.Amount);
                            LineData.DetailType = item.tblItem.Type;
                            LineData.SalesItemLineDetail = new Models.EstimateQB.SalesItemLineDetail();
                            LineData.SalesItemLineDetail.ItemRef = new EstimateQB.ItemRef();
                            LineData.SalesItemLineDetail.ItemRef.value = item.tblItem.QBId.ToString();
                            //LineData.SalesItemLineDetail.ItemRef.name = "Pest Control";
                            LineData.SalesItemLineDetail.UnitPrice =Convert.ToDecimal(item.Rate);
                            LineData.SalesItemLineDetail.Qty = (int)item.Qty;
                            EsitimateData.Line = new List<Models.EstimateQB.Line>();
                            EsitimateData.Line.Add(LineData);
                        }
                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(EsitimateData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                string jsonResponse = await response.Content.ReadAsStringAsync();
                                dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                                // Now you can access the data using the model
                                var QBId = estimateModel["Estimate"]["Id"];
                                var SyncToken = estimateModel["Estimate"]["SyncToken"];

                                Data.QBId = SyncToken;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId =Convert.ToInt32(QBId);
                                SyncLogData.isSync =true;
                                SyncLogData.EditDate =DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                string errorMessage = await response.Content.ReadAsStringAsync();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = errorMessage;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }


                        //foreach (tblEstimateFile item in FileData)
                        //{
                        //    string baseUrl = "https://sandbox-quickbooks.api.intuit.com";

                        //    // Endpoint
                        //    //string endpoint = "/v3/company/<realmID>/upload"; // Replace <realmID> with the actual realm ID

                        //    // Complete URL
                        //    //apiUrl = baseUrl + endpoint;

                        //    // Request body
                        //    var requestBody = new
                        //    {
                        //        AttachableRef = new[]
                        //        {
                        //        new
                        //        {
                        //            EntityRef = new
                        //            {
                        //                type = "Estimate",
                        //                value = "95"
                        //            }
                        //        }
                        //    },
                        //        //ContentType = "image/jpg",
                        //        //FileName = "receipt_nov15.jpg"
                        //    };

                        //    // Serialize the request body to JSON
                        //    string jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

                        //    using (var httpClient = new HttpClient())
                        //    using (var content = new MultipartFormDataContent())
                        //    {
                        //        // Add AttachableRef as a JSON string
                        //        //var attachableRefJson = "{\"AttachableRef\":[{\"EntityRef\":{\"type\":\"Invoice\",\"value\":\"95\"}}],\"ContentType\":\"image/jpg\",\"FileName\":\"receipt_nov15.jpg\"}";
                        //        content.Add(new StringContent(jsonRequestBody), "application/json");

                        //        // Load the file from a URL
                        //        var fileUrl = "https://earthcoapi.yehtohoga.com/"+item.FilePath+""; // Replace with the actual file URL
                        //        var fileBytes = await httpClient.GetByteArrayAsync(fileUrl);
                        //        var fileContent = new ByteArrayContent(fileBytes);
                        //        content.Add(fileContent, "file", "receipt_nov15.jpg");

                        //        // Make the POST request
                        //        apiUrl = $"/v3/company/" + TokenData.realmId + "/upload";
                        //        var requestUrl = $"{baseUrl}{apiUrl}";

                        //        var response = await httpClient.PostAsync(requestUrl, content);

                        //        if (response.IsSuccessStatusCode)
                        //        {
                        //            // Handle success
                        //            Console.WriteLine("File uploaded successfully!");
                        //        }
                        //        else
                        //        {
                        //            // Handle failure
                        //            Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        //        }
                        //    }
                        //}

                        

                    }
                    else
                    {
                        Data = DB.tblEstimates.Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if(Data!=null)
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Convert.ToInt32(Data.EstimateId);
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblToken TokenData = DB.tblTokens.FirstOrDefault();
                            string AccessToken = "";

                            var diffOfDates = DateTime.Now - TokenData.EditDate;
                            do
                            {
                                TokenData = DB.tblTokens.FirstOrDefault();
                                diffOfDates = DateTime.Now - TokenData.EditDate;
                                if (diffOfDates.Value.Hours >= 1)
                                {
                                    HomeController.GetAuthTokensUsingRefreshTokenAsync();
                                }
                            } while (diffOfDates.Value.Hours >= 1);

                            AccessToken = TokenData.AccessToken;

                            using (HttpClient client = new HttpClient())
                            {
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                                client.DefaultRequestHeaders.Accept.Clear();
                                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                                // Make the GET request
                                HttpResponseMessage response = await client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/estimate/" + SyncLog.QBId + "?minorversion=23");

                                if (response.IsSuccessStatusCode)
                                {
                                    var readTask = response.Content.ReadAsStringAsync();
                                    readTask.Wait();
                                    string Test = readTask.Result;
                                    QBEstimateResponseClass.EstimateMain ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBEstimateResponseClass.EstimateMain>(Test);

                                    Data.QBId = Convert.ToInt32(ResponseData.Estimate.Id);
                                    Data.EstimateNumber = ResponseData.Estimate.DocNumber;
                                    int QBId = Convert.ToInt32(ResponseData.Estimate.CustomerRef.value); ;
                                    int CustomerId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.CustomerId = CustomerId;
                                    //Data.ServiceLocationId = Estimate.EstimateData.ServiceLocationId;
                                    //Data.Email = ResponseData.Estimate.Domain;
                                    //Data.IssueDate = DateTime.Now;
                                    //Data.ContactId = Estimate.EstimateData.ContactId;
                                    //Data.RegionalManagerId = Estimate.EstimateData.RegionalManagerId;
                                    //Data.AssignTo = Estimate.EstimateData.AssignTo;
                                    //Data.RequestedBy = Estimate.EstimateData.RequestedBy;
                                    if (ResponseData.Estimate.TxnStatus.Contains("Pending"))
                                    {
                                        Data.EstimateStatusId = 4;
                                    }
                                    else if (ResponseData.Estimate.TxnStatus.Contains("Accepted"))
                                    {
                                        Data.EstimateStatusId = 1;
                                    }
                                    else if (ResponseData.Estimate.TxnStatus.Contains("Rejected"))
                                    {
                                        Data.EstimateStatusId = 5;
                                    }
                                    else if (ResponseData.Estimate.TxnStatus.Contains("Closed"))
                                    {
                                        Data.EstimateStatusId = 2;
                                    }

                                    Data.EstimateNotes = ResponseData.Estimate.CustomerMemo.value;
                                    Data.SyncToken = ResponseData.Estimate.SyncToken;
                                    //Data.Tax = Estimate.EstimateData.Tax;
                                    //Data.Tags = Estimate.EstimateData.Tags;
                                    //Data.Discount = Estimate.EstimateData.Discount;
                                    //Data.Shipping = Estimate.EstimateData.Shipping;
                                    //Data.Profit = Estimate.EstimateData.Profit;
                                    //Data.ProfitPercentage = Estimate.EstimateData.ProfitPercentage;
                                    Data.TotalAmount = (double?)ResponseData.Estimate.TotalAmt;
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    //Data.CreatedBy = UserId;
                                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("E").FirstOrDefault());
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblEstimates.Add(Data);
                                    DB.SaveChanges();


                                    foreach (QBEstimateResponseClass.Line item in ResponseData.Estimate.Line)
                                    {
                                        tblEstimateItem Item = new tblEstimateItem();
                                        Item.Description = item.Description;
                                        Item.Qty = Convert.ToInt32(item.SalesItemLineDetail.Qty);
                                        Item.Rate = Convert.ToDouble(item.SalesItemLineDetail.UnitPrice);
                                        Item.Amount = Convert.ToDouble(item.Amount);
                                        QBId = Convert.ToInt32(item.SalesItemLineDetail.ItemRef.value);
                                        int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                        Item.ItemId = ItemId;
                                        Item.EstimateId = Data.EstimateId;
                                        //Item.CreatedBy = UserId;
                                        Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        //Item.EditBy = UserId;
                                        Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        Item.isCost = false;
                                        Item.isActive = true;
                                        Item.isDelete = false;
                                        DB.tblEstimateItems.Add(Item);
                                        DB.SaveChanges();
                                    }


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.EstimateId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();

                                }
                                else
                                {
                                    string errorMessage = await response.Content.ReadAsStringAsync();

                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = errorMessage;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                        }
                        



                    }
                }
                else if (SyncLog.Operation == "Update")
                {
                    if(SyncLog.isQB!=true)
                    {
                        Data = DB.tblEstimates.Where(x => x.EstimateId == SyncLog.Id).FirstOrDefault();
                        ItemData = DB.tblEstimateItems.Where(x => x.EstimateId == SyncLog.Id).ToList();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/estimate?minorversion=23";

                        QBEstimateClass EsitimateData = new QBEstimateClass();
                        Models.EstimateQB.Line LineData = new Models.EstimateQB.Line();
                        EsitimateData.BillEmail = new BillEmail();
                        EsitimateData.BillEmail.Address = Data.tblUser.Email;
                        EsitimateData.TotalAmt = (decimal)Data.TotalAmount;
                        EsitimateData.SyncToken = Data.SyncToken;
                        EsitimateData.Id = Data.QBId.ToString();
                        EsitimateData.CustomerRef = new EstimateQB.CustomerRef();
                        EsitimateData.CustomerRef.value = Data.tblUser.QBId.ToString();
                        //EsitimateData.CustomerRef.name = "Cool Cars";

                        foreach (var item in ItemData)
                        {
                            //LineData.Id = "1";
                            LineData.Description = item.Description;
                            LineData.Amount = Convert.ToDecimal(item.Amount);
                            LineData.DetailType = item.tblItem.Type;
                            LineData.SalesItemLineDetail = new Models.EstimateQB.SalesItemLineDetail();
                            LineData.SalesItemLineDetail.ItemRef = new EstimateQB.ItemRef();
                            LineData.SalesItemLineDetail.ItemRef.value = item.tblItem.QBId.ToString();
                            //LineData.SalesItemLineDetail.ItemRef.name = "Pest Control";
                            LineData.SalesItemLineDetail.UnitPrice = Convert.ToDecimal(item.Rate);
                            LineData.SalesItemLineDetail.Qty = (int)item.Qty;
                            EsitimateData.Line = new List<Models.EstimateQB.Line>();
                            EsitimateData.Line.Add(LineData);
                        }
                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(EsitimateData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                string jsonResponse = await response.Content.ReadAsStringAsync();
                                dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                                // Now you can access the data using the model
                                var QBId = estimateModel["Estimate"]["Id"];
                                var SyncToken = estimateModel["Estimate"]["SyncToken"];

                                Data.QBId = QBId;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                string errorMessage = await response.Content.ReadAsStringAsync();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = errorMessage;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;

                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            // Make the GET request
                            HttpResponseMessage response = await client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/estimate/" + SyncLog.QBId + "?minorversion=23");

                            if (response.IsSuccessStatusCode)
                            {
                                var readTask = response.Content.ReadAsStringAsync();
                                readTask.Wait();
                                string Test = readTask.Result;
                                QBEstimateResponseClass.EstimateMain ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBEstimateResponseClass.EstimateMain>(Test);


                                int QBId= Convert.ToInt32(ResponseData.Estimate.Id);

                                Data = DB.tblEstimates.Where(x => x.QBId == QBId).FirstOrDefault();
                                if(Data!=null)
                                {
                                    Data.QBId = Convert.ToInt32(ResponseData.Estimate.Id);
                                    Data.EstimateNumber = ResponseData.Estimate.DocNumber;
                                    QBId = Convert.ToInt32(ResponseData.Estimate.CustomerRef.value); ;
                                    int CustomerId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.CustomerId = CustomerId;
                                    //Data.ServiceLocationId = Estimate.EstimateData.ServiceLocationId;
                                    //Data.Email = ResponseData.Estimate.Domain;
                                    //Data.IssueDate = DateTime.Now;
                                    //Data.ContactId = Estimate.EstimateData.ContactId;
                                    //Data.RegionalManagerId = Estimate.EstimateData.RegionalManagerId;
                                    //Data.AssignTo = Estimate.EstimateData.AssignTo;
                                    //Data.RequestedBy = Estimate.EstimateData.RequestedBy;
                                    if (ResponseData.Estimate.TxnStatus.Contains("Pending"))
                                    {
                                        Data.EstimateStatusId = 4;
                                    }
                                    else if (ResponseData.Estimate.TxnStatus.Contains("Accepted"))
                                    {
                                        Data.EstimateStatusId = 1;
                                    }
                                    else if (ResponseData.Estimate.TxnStatus.Contains("Rejected"))
                                    {
                                        Data.EstimateStatusId = 5;
                                    }
                                    else if (ResponseData.Estimate.TxnStatus.Contains("Closed"))
                                    {
                                        Data.EstimateStatusId = 2;
                                    }

                                    Data.EstimateNotes = ResponseData.Estimate.CustomerMemo.value;
                                    Data.SyncToken = ResponseData.Estimate.SyncToken;
                                    //Data.Tax = Estimate.EstimateData.Tax;
                                    //Data.Tags = Estimate.EstimateData.Tags;
                                    //Data.Discount = Estimate.EstimateData.Discount;
                                    //Data.Shipping = Estimate.EstimateData.Shipping;
                                    //Data.Profit = Estimate.EstimateData.Profit;
                                    //Data.ProfitPercentage = Estimate.EstimateData.ProfitPercentage;
                                    Data.TotalAmount = (double?)ResponseData.Estimate.TotalAmt;
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    //Data.CreatedBy = UserId;
                                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("E").FirstOrDefault());
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.Entry(Data);
                                    DB.SaveChanges();

                                    ItemData = DB.tblEstimateItems.Where(x => x.EstimateId == Data.EstimateId).ToList();
                                    if (ItemData != null && ItemData.Count != 0)
                                    {
                                        DB.tblEstimateItems.RemoveRange(ItemData);
                                    }

                                    foreach (QBEstimateResponseClass.Line item in ResponseData.Estimate.Line)
                                    {
                                        tblEstimateItem Item = new tblEstimateItem();
                                        Item.Description = item.Description;
                                        Item.Qty = Convert.ToInt32(item.SalesItemLineDetail.Qty);
                                        Item.Rate = Convert.ToDouble(item.SalesItemLineDetail.UnitPrice);
                                        Item.Amount = Convert.ToDouble(item.Amount);
                                        QBId = Convert.ToInt32(item.SalesItemLineDetail.ItemRef.value);
                                        int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                        Item.ItemId = ItemId;
                                        Item.EstimateId = Data.EstimateId;
                                        //Item.CreatedBy = UserId;
                                        Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        //Item.EditBy = UserId;
                                        Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        Item.isCost = false;
                                        Item.isActive = true;
                                        Item.isDelete = false;
                                        DB.tblEstimateItems.Add(Item);
                                        DB.SaveChanges();
                                    }


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.EstimateId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    Data.QBId = Convert.ToInt32(ResponseData.Estimate.Id);
                                    Data.EstimateNumber = ResponseData.Estimate.DocNumber;
                                    QBId = Convert.ToInt32(ResponseData.Estimate.CustomerRef.value); ;
                                    int CustomerId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.CustomerId = CustomerId;
                                    //Data.ServiceLocationId = Estimate.EstimateData.ServiceLocationId;
                                    //Data.Email = ResponseData.Estimate.Domain;
                                    //Data.IssueDate = DateTime.Now;
                                    //Data.ContactId = Estimate.EstimateData.ContactId;
                                    //Data.RegionalManagerId = Estimate.EstimateData.RegionalManagerId;
                                    //Data.AssignTo = Estimate.EstimateData.AssignTo;
                                    //Data.RequestedBy = Estimate.EstimateData.RequestedBy;
                                    if (ResponseData.Estimate.TxnStatus.Contains("Pending"))
                                    {
                                        Data.EstimateStatusId = 4;
                                    }
                                    else if (ResponseData.Estimate.TxnStatus.Contains("Accepted"))
                                    {
                                        Data.EstimateStatusId = 1;
                                    }
                                    else if (ResponseData.Estimate.TxnStatus.Contains("Rejected"))
                                    {
                                        Data.EstimateStatusId = 5;
                                    }
                                    else if (ResponseData.Estimate.TxnStatus.Contains("Closed"))
                                    {
                                        Data.EstimateStatusId = 2;
                                    }

                                    Data.EstimateNotes = ResponseData.Estimate.CustomerMemo.value;
                                    Data.SyncToken = ResponseData.Estimate.SyncToken;
                                    //Data.Tax = Estimate.EstimateData.Tax;
                                    //Data.Tags = Estimate.EstimateData.Tags;
                                    //Data.Discount = Estimate.EstimateData.Discount;
                                    //Data.Shipping = Estimate.EstimateData.Shipping;
                                    //Data.Profit = Estimate.EstimateData.Profit;
                                    //Data.ProfitPercentage = Estimate.EstimateData.ProfitPercentage;
                                    Data.TotalAmount = (double?)ResponseData.Estimate.TotalAmt;
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    //Data.CreatedBy = UserId;
                                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("E").FirstOrDefault());
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblEstimates.Add(Data);
                                    DB.SaveChanges();

                                    ItemData = DB.tblEstimateItems.Where(x => x.EstimateId == Data.EstimateId).ToList();
                                    if (ItemData != null && ItemData.Count != 0)
                                    {
                                        DB.tblEstimateItems.RemoveRange(ItemData);
                                    }

                                    foreach (QBEstimateResponseClass.Line item in ResponseData.Estimate.Line)
                                    {
                                        tblEstimateItem Item = new tblEstimateItem();
                                        Item.Description = item.Description;
                                        Item.Qty = Convert.ToInt32(item.SalesItemLineDetail.Qty);
                                        Item.Rate = Convert.ToDouble(item.SalesItemLineDetail.UnitPrice);
                                        Item.Amount = Convert.ToDouble(item.Amount);
                                        QBId = Convert.ToInt32(item.SalesItemLineDetail.ItemRef.value);
                                        int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                        Item.ItemId = ItemId;
                                        Item.EstimateId = Data.EstimateId;
                                        //Item.CreatedBy = UserId;
                                        Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        //Item.EditBy = UserId;
                                        Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        Item.isCost = false;
                                        Item.isActive = true;
                                        Item.isDelete = false;
                                        DB.tblEstimateItems.Add(Item);
                                        DB.SaveChanges();
                                    }


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.EstimateId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                            else
                            {
                                string errorMessage = await response.Content.ReadAsStringAsync();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = errorMessage;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                            }
                        }
                    }
                }
                else if (SyncLog.Operation == "Delete")
                {
                    if (SyncLog.isQB != true)
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/estimate?operation=delete";

                        QBDeleteClass DeleteData = new QBDeleteClass();
                        Data = DB.tblEstimates.Where(x => x.EstimateId == SyncLog.Id).FirstOrDefault();
                        DeleteData.Id = Data.QBId.ToString();
                        DeleteData.SyncToken = Data.SyncToken;



                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(DeleteData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                string jsonResponse = await response.Content.ReadAsStringAsync();
                                dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                                // Now you can access the data using the model
                                var QBId = estimateModel["Estimate"]["Id"];


                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                string errorMessage = await response.Content.ReadAsStringAsync();
                                var errorResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(errorMessage);
                                if(errorResponse.Fault.Error.FirstOrDefault().Message.Contains("Object Not Found"))
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = errorResponse.Fault.Error.FirstOrDefault().Message;
                                    SyncLogData.EditDate = DateTime.Now;
                                    SyncLogData.isSync = true;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = errorMessage;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                
                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        Data = DB.tblEstimates.Select(r => r).Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                       if(Data!=null)
                        {
                            Data.isDelete = true;
                            Data.EditDate = DateTime.Now;
                            DB.Entry(Data);
                            DB.SaveChanges();

                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Data.EstimateId;
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Message = "Object Not Found";
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        
                    }
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
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                //return ResponseMessage(responseMessage);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An exception occurred: {ex.Message}");
                // Additional handling for generic exceptions

                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = ex.InnerException != null && ex.InnerException.InnerException != null ? new StringContent(ex.InnerException.InnerException.Message) : new StringContent(ex.Message);
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                //return ResponseMessage(responseMessage);
            }

        }

        [HttpPost]
        public async Task SyncPurchaseOrderAsync(tblSyncLog SyncLog)
        {
            try
            {
                tblPurchaseOrder Data = new tblPurchaseOrder();
                List<tblPurchaseOrderItem> ItemData = new List<tblPurchaseOrderItem>();
                List<tblPurchaseOrderFile> FileData = new List<tblPurchaseOrderFile>();
                if (SyncLog.Operation == "Create")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblPurchaseOrders.Where(x => x.PurchaseOrderId == SyncLog.Id).FirstOrDefault();
                        ItemData = DB.tblPurchaseOrderItems.Where(x => x.PurchaseOrderId == SyncLog.Id).ToList();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            DB = new earthcoEntities();
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/purchaseorder?minorversion=23";

                        QBPurchaseOrderClass PurchaseOrderData = new QBPurchaseOrderClass();
                        Models.QBPurchaseOrderCUClass.LineDetail LineData = new Models.QBPurchaseOrderCUClass.LineDetail();
                        PurchaseOrderData.APAccountRef = new QBPurchaseOrderCUClass.APAccountRef();
                        PurchaseOrderData.APAccountRef.value = "33";
                        PurchaseOrderData.TotalAmt = (decimal)Data.Amount;
                        //EsitimateData.SyncToken = "0";
                        //EsitimateData.Id = "1103";
                        PurchaseOrderData.VendorRef = new QBPurchaseOrderCUClass.VendorRef();
                        PurchaseOrderData.VendorRef.value = Data.tblUser.QBId.ToString();
                        if (Data.StatusId==1)
                        {
                            PurchaseOrderData.POStatus = "Open";
                        }
                        else if (Data.StatusId == 2)
                        {
                            PurchaseOrderData.POStatus = "Closed";
                        }
                        

                        foreach (var item in ItemData)
                        {
                            //LineData.Id = "1";
                            //LineData.Description = item.Description;
                            LineData.Amount = Convert.ToDecimal(item.Amount);
                            LineData.DetailType = item.tblItem.Type;
                            LineData.ItemBasedExpenseLineDetail = new Models.QBPurchaseOrderCUClass.ItemBasedExpenseLineDetail();
                            LineData.ItemBasedExpenseLineDetail.ItemRef = new QBPurchaseOrderCUClass.ItemRef();
                            LineData.ItemBasedExpenseLineDetail.ItemRef.value = item.tblItem.QBId.ToString();
                            //LineData.SalesItemLineDetail.ItemRef.name = "Pest Control";
                            LineData.ItemBasedExpenseLineDetail.UnitPrice = Convert.ToDecimal(item.Rate);
                            LineData.ItemBasedExpenseLineDetail.Qty = (int)item.Qty;
                            PurchaseOrderData.Line = new List<Models.QBPurchaseOrderCUClass.LineDetail>();
                            PurchaseOrderData.Line.Add(LineData);
                        }
                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(PurchaseOrderData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                string jsonResponse = await response.Content.ReadAsStringAsync();
                                dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                                // Now you can access the data using the model
                                var QBId = estimateModel["PurchaseOrder"]["Id"];
                                var SyncToken = estimateModel["PurchaseOrder"]["SyncToken"];

                                Data.QBId = SyncToken;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                            }
                            else
                            {
                                // Handle error
                                string errorMessage = await response.Content.ReadAsStringAsync();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = errorMessage;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }


                        //foreach (tblEstimateFile item in FileData)
                        //{
                        //    string baseUrl = "https://sandbox-quickbooks.api.intuit.com";

                        //    // Endpoint
                        //    //string endpoint = "/v3/company/<realmID>/upload"; // Replace <realmID> with the actual realm ID

                        //    // Complete URL
                        //    //apiUrl = baseUrl + endpoint;

                        //    // Request body
                        //    var requestBody = new
                        //    {
                        //        AttachableRef = new[]
                        //        {
                        //        new
                        //        {
                        //            EntityRef = new
                        //            {
                        //                type = "Estimate",
                        //                value = "95"
                        //            }
                        //        }
                        //    },
                        //        //ContentType = "image/jpg",
                        //        //FileName = "receipt_nov15.jpg"
                        //    };

                        //    // Serialize the request body to JSON
                        //    string jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

                        //    using (var httpClient = new HttpClient())
                        //    using (var content = new MultipartFormDataContent())
                        //    {
                        //        // Add AttachableRef as a JSON string
                        //        //var attachableRefJson = "{\"AttachableRef\":[{\"EntityRef\":{\"type\":\"Invoice\",\"value\":\"95\"}}],\"ContentType\":\"image/jpg\",\"FileName\":\"receipt_nov15.jpg\"}";
                        //        content.Add(new StringContent(jsonRequestBody), "application/json");

                        //        // Load the file from a URL
                        //        var fileUrl = "https://earthcoapi.yehtohoga.com/"+item.FilePath+""; // Replace with the actual file URL
                        //        var fileBytes = await httpClient.GetByteArrayAsync(fileUrl);
                        //        var fileContent = new ByteArrayContent(fileBytes);
                        //        content.Add(fileContent, "file", "receipt_nov15.jpg");

                        //        // Make the POST request
                        //        apiUrl = $"/v3/company/" + TokenData.realmId + "/upload";
                        //        var requestUrl = $"{baseUrl}{apiUrl}";

                        //        var response = await httpClient.PostAsync(requestUrl, content);

                        //        if (response.IsSuccessStatusCode)
                        //        {
                        //            // Handle success
                        //            Console.WriteLine("File uploaded successfully!");
                        //        }
                        //        else
                        //        {
                        //            // Handle failure
                        //            Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        //        }
                        //    }
                        //}



                    }
                    else
                    {
                        Data = DB.tblPurchaseOrders.Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if (Data != null)
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Convert.ToInt32(Data.EstimateId);
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblToken TokenData = DB.tblTokens.FirstOrDefault();
                            string AccessToken = "";

                            var diffOfDates = DateTime.Now - TokenData.EditDate;
                            do
                            {
                                TokenData = DB.tblTokens.FirstOrDefault();
                                diffOfDates = DateTime.Now - TokenData.EditDate;
                                if (diffOfDates.Value.Hours >= 1)
                                {
                                    HomeController.GetAuthTokensUsingRefreshTokenAsync();
                                }
                            } while (diffOfDates.Value.Hours >= 1);

                            AccessToken = TokenData.AccessToken;

                            using (HttpClient client = new HttpClient())
                            {
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                                client.DefaultRequestHeaders.Accept.Clear();
                                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                                // Make the GET request
                                HttpResponseMessage response = await client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/purchaseorder/" + SyncLog.QBId + "?minorversion=23");

                                if (response.IsSuccessStatusCode)
                                {
                                    var readTask = response.Content.ReadAsStringAsync();
                                    readTask.Wait();
                                    string Test = readTask.Result;
                                    PurchaseOrderQB.PurchaseOrderResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<PurchaseOrderQB.PurchaseOrderResponse>(Test);

                                    Data.QBId = Convert.ToInt32(ResponseData.PurchaseOrder.Id);
                                    Data.EstimateNumber = ResponseData.PurchaseOrder.DocNumber;
                                    int QBId = Convert.ToInt32(ResponseData.PurchaseOrder.VendorRef.value); ;
                                    int VendorId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.SupplierId = VendorId;
                                    //Data.ServiceLocationId = Estimate.EstimateData.ServiceLocationId;
                                    //Data.Email = ResponseData.Estimate.Domain;
                                    //Data.IssueDate = DateTime.Now;
                                    //Data.ContactId = Estimate.EstimateData.ContactId;
                                    //Data.RegionalManagerId = Estimate.EstimateData.RegionalManagerId;
                                    //Data.AssignTo = Estimate.EstimateData.AssignTo;
                                    //Data.RequestedBy = Estimate.EstimateData.RequestedBy;
                                    if (ResponseData.PurchaseOrder.POStatus.Contains("Open"))
                                    {
                                        Data.StatusId = 1;
                                    }
                                    else if (ResponseData.PurchaseOrder.POStatus.Contains("Close"))
                                    {
                                        Data.StatusId = 2;
                                    }

                                    Data.SyncToken = ResponseData.PurchaseOrder.SyncToken;
                                    //Data.Tax = Estimate.EstimateData.Tax;
                                    //Data.Tags = Estimate.EstimateData.Tags;
                                    //Data.Discount = Estimate.EstimateData.Discount;
                                    //Data.Shipping = Estimate.EstimateData.Shipping;
                                    //Data.Profit = Estimate.EstimateData.Profit;
                                    //Data.ProfitPercentage = Estimate.EstimateData.ProfitPercentage;
                                    Data.Date = DateTime.Now;
                                    Data.Amount = (double)ResponseData.PurchaseOrder.TotalAmt;
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    //Data.CreatedBy = UserId;
                                    Data.PurchaseOrderNumber = ResponseData.PurchaseOrder.DocNumber;
                                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("P").FirstOrDefault());
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblPurchaseOrders.Add(Data);
                                    DB.SaveChanges();


                                    foreach (PurchaseOrderQB.Line item in ResponseData.PurchaseOrder.Line)
                                    {
                                        tblPurchaseOrderItem Item = new tblPurchaseOrderItem();
                                        //Item.Description = item.Description;
                                        Item.Qty = Convert.ToInt32(item.ItemBasedExpenseLineDetail.Qty);
                                        Item.Rate = Convert.ToDouble(item.ItemBasedExpenseLineDetail.UnitPrice);
                                        Item.Amount = Convert.ToDouble(item.Amount);
                                        QBId = Convert.ToInt32(item.ItemBasedExpenseLineDetail.ItemRef.value);
                                        int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                        Item.ItemId = ItemId;
                                        Item.PurchaseOrderId = Data.PurchaseOrderId;
                                        //Item.CreatedBy = UserId;
                                        Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        //Item.EditBy = UserId;
                                        Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        Item.isActive = true;
                                        Item.isDelete = false;
                                        DB.tblPurchaseOrderItems.Add(Item);
                                        DB.SaveChanges();
                                    }


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.PurchaseOrderId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();

                                }
                                else
                                {
                                    string errorMessage = await response.Content.ReadAsStringAsync();

                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = errorMessage;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                        }




                    }
                }
                else if (SyncLog.Operation == "Update")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblPurchaseOrders.Where(x => x.PurchaseOrderId == SyncLog.Id).FirstOrDefault();
                        ItemData = DB.tblPurchaseOrderItems.Where(x => x.PurchaseOrderId == SyncLog.Id).ToList();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/purchaseorder?minorversion=23";


                        QBPurchaseOrderClass PurchaseOrderData = new QBPurchaseOrderClass();
                        Models.QBPurchaseOrderCUClass.LineDetail LineData = new Models.QBPurchaseOrderCUClass.LineDetail();
                        PurchaseOrderData.APAccountRef = new QBPurchaseOrderCUClass.APAccountRef();
                        PurchaseOrderData.APAccountRef.value = "33";
                        PurchaseOrderData.TotalAmt = (decimal)Data.Amount;
                        PurchaseOrderData.SyncToken = Data.SyncToken;
                        PurchaseOrderData.Id = Data.QBId.ToString();
                        PurchaseOrderData.VendorRef = new QBPurchaseOrderCUClass.VendorRef();
                        PurchaseOrderData.VendorRef.value = Data.tblUser.QBId.ToString();
                        if (Data.StatusId == 1)
                        {
                            PurchaseOrderData.POStatus = "Open";
                        }
                        else if (Data.StatusId == 2)
                        {
                            PurchaseOrderData.POStatus = "Closed";
                        }


                        foreach (var item in ItemData)
                        {
                            //LineData.Id = "1";
                            //LineData.Description = item.Description;
                            LineData.Amount = Convert.ToDecimal(item.Amount);
                            LineData.DetailType = item.tblItem.Type;
                            LineData.ItemBasedExpenseLineDetail = new Models.QBPurchaseOrderCUClass.ItemBasedExpenseLineDetail();
                            LineData.ItemBasedExpenseLineDetail.ItemRef = new QBPurchaseOrderCUClass.ItemRef();
                            LineData.ItemBasedExpenseLineDetail.ItemRef.value = item.tblItem.QBId.ToString();
                            //LineData.SalesItemLineDetail.ItemRef.name = "Pest Control";
                            LineData.ItemBasedExpenseLineDetail.UnitPrice = Convert.ToDecimal(item.Rate);
                            LineData.ItemBasedExpenseLineDetail.Qty = (int)item.Qty;
                            PurchaseOrderData.Line = new List<Models.QBPurchaseOrderCUClass.LineDetail>();
                            PurchaseOrderData.Line.Add(LineData);
                        }


                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(PurchaseOrderData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                string jsonResponse = await response.Content.ReadAsStringAsync();
                                dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                                // Now you can access the data using the model
                               
                                var QBId = estimateModel["PurchaseOrder"]["Id"];
                                var SyncToken = estimateModel["PurchaseOrder"]["SyncToken"];

                                Data.QBId = QBId;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                string errorMessage = await response.Content.ReadAsStringAsync();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = errorMessage;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;

                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            // Make the GET request
                            HttpResponseMessage response = await client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/purchaseorder/" + SyncLog.QBId + "?minorversion=23");

                            if (response.IsSuccessStatusCode)
                            {
                                var readTask = response.Content.ReadAsStringAsync();
                                readTask.Wait();
                                string Test = readTask.Result;
                                PurchaseOrderQB.PurchaseOrderResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<PurchaseOrderQB.PurchaseOrderResponse>(Test);


                                int QBId = Convert.ToInt32(ResponseData.PurchaseOrder.Id);

                                Data = DB.tblPurchaseOrders.Where(x => x.QBId == QBId).FirstOrDefault();
                                if (Data != null)
                                {
                                    Data.QBId = Convert.ToInt32(ResponseData.PurchaseOrder.Id);
                                    Data.EstimateNumber = ResponseData.PurchaseOrder.DocNumber;
                                    QBId = Convert.ToInt32(ResponseData.PurchaseOrder.VendorRef.value); ;
                                    int VendorId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.SupplierId = VendorId;
                                    //Data.ServiceLocationId = Estimate.EstimateData.ServiceLocationId;
                                    //Data.Email = ResponseData.Estimate.Domain;
                                    //Data.IssueDate = DateTime.Now;
                                    //Data.ContactId = Estimate.EstimateData.ContactId;
                                    //Data.RegionalManagerId = Estimate.EstimateData.RegionalManagerId;
                                    //Data.AssignTo = Estimate.EstimateData.AssignTo;
                                    //Data.RequestedBy = Estimate.EstimateData.RequestedBy;
                                    if (ResponseData.PurchaseOrder.POStatus.Contains("Open"))
                                    {
                                        Data.StatusId = 1;
                                    }
                                    else if (ResponseData.PurchaseOrder.POStatus.Contains("Close"))
                                    {
                                        Data.StatusId = 2;
                                    }

                                    Data.SyncToken = ResponseData.PurchaseOrder.SyncToken;
                                    //Data.Tax = Estimate.EstimateData.Tax;
                                    //Data.Tags = Estimate.EstimateData.Tags;
                                    //Data.Discount = Estimate.EstimateData.Discount;
                                    //Data.Shipping = Estimate.EstimateData.Shipping;
                                    //Data.Profit = Estimate.EstimateData.Profit;
                                    //Data.ProfitPercentage = Estimate.EstimateData.ProfitPercentage;
                                    Data.Amount = (double)ResponseData.PurchaseOrder.TotalAmt;
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    //Data.CreatedBy = UserId;
                                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("P").FirstOrDefault());
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.Entry(Data);
                                    DB.SaveChanges();

                                    ItemData = DB.tblPurchaseOrderItems.Where(x => x.PurchaseOrderId == Data.PurchaseOrderId).ToList();
                                    if (ItemData != null && ItemData.Count != 0)
                                    {
                                        DB.tblPurchaseOrderItems.RemoveRange(ItemData);
                                    }

                                    foreach (PurchaseOrderQB.Line item in ResponseData.PurchaseOrder.Line)
                                    {
                                        tblPurchaseOrderItem Item = new tblPurchaseOrderItem();
                                        //Item.Description = item.Description;
                                        Item.Qty = Convert.ToInt32(item.ItemBasedExpenseLineDetail.Qty);
                                        Item.Rate = Convert.ToDouble(item.ItemBasedExpenseLineDetail.UnitPrice);
                                        Item.Amount = Convert.ToDouble(item.Amount);
                                        QBId = Convert.ToInt32(item.ItemBasedExpenseLineDetail.ItemRef.value);
                                        int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                        Item.ItemId = ItemId;
                                        Item.PurchaseOrderId = Data.PurchaseOrderId;
                                        //Item.CreatedBy = UserId;
                                        Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        //Item.EditBy = UserId;
                                        Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        Item.isActive = true;
                                        Item.isDelete = false;
                                        DB.tblPurchaseOrderItems.Add(Item);
                                        DB.SaveChanges();
                                    }


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.PurchaseOrderId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    Data.QBId = Convert.ToInt32(ResponseData.PurchaseOrder.Id);
                                    Data.EstimateNumber = ResponseData.PurchaseOrder.DocNumber;
                                    QBId = Convert.ToInt32(ResponseData.PurchaseOrder.VendorRef.value); ;
                                    int VendorId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.SupplierId = VendorId;
                                    //Data.ServiceLocationId = Estimate.EstimateData.ServiceLocationId;
                                    //Data.Email = ResponseData.Estimate.Domain;
                                    //Data.IssueDate = DateTime.Now;
                                    //Data.ContactId = Estimate.EstimateData.ContactId;
                                    //Data.RegionalManagerId = Estimate.EstimateData.RegionalManagerId;
                                    //Data.AssignTo = Estimate.EstimateData.AssignTo;
                                    //Data.RequestedBy = Estimate.EstimateData.RequestedBy;
                                    if (ResponseData.PurchaseOrder.POStatus.Contains("Open"))
                                    {
                                        Data.StatusId = 1;
                                    }
                                    else if (ResponseData.PurchaseOrder.POStatus.Contains("Close"))
                                    {
                                        Data.StatusId = 2;
                                    }

                                    Data.SyncToken = ResponseData.PurchaseOrder.SyncToken;
                                    //Data.Tax = Estimate.EstimateData.Tax;
                                    //Data.Tags = Estimate.EstimateData.Tags;
                                    //Data.Discount = Estimate.EstimateData.Discount;
                                    //Data.Shipping = Estimate.EstimateData.Shipping;
                                    //Data.Profit = Estimate.EstimateData.Profit;
                                    //Data.ProfitPercentage = Estimate.EstimateData.ProfitPercentage;
                                    Data.Amount = (double)ResponseData.PurchaseOrder.TotalAmt;
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    //Data.CreatedBy = UserId;
                                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("P").FirstOrDefault());
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblPurchaseOrders.Add(Data);
                                    DB.SaveChanges();

                                    ItemData = DB.tblPurchaseOrderItems.Where(x => x.PurchaseOrderId == Data.PurchaseOrderId).ToList();
                                    if (ItemData != null && ItemData.Count != 0)
                                    {
                                        DB.tblPurchaseOrderItems.RemoveRange(ItemData);
                                    }

                                    foreach (PurchaseOrderQB.Line item in ResponseData.PurchaseOrder.Line)
                                    {
                                        tblPurchaseOrderItem Item = new tblPurchaseOrderItem();
                                        //Item.Description = item.Description;
                                        Item.Qty = Convert.ToInt32(item.ItemBasedExpenseLineDetail.Qty);
                                        Item.Rate = Convert.ToDouble(item.ItemBasedExpenseLineDetail.UnitPrice);
                                        Item.Amount = Convert.ToDouble(item.Amount);
                                        QBId = Convert.ToInt32(item.ItemBasedExpenseLineDetail.ItemRef.value);
                                        int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                        Item.ItemId = ItemId;
                                        Item.PurchaseOrderId = Data.PurchaseOrderId;
                                        //Item.CreatedBy = UserId;
                                        Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        //Item.EditBy = UserId;
                                        Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        Item.isActive = true;
                                        Item.isDelete = false;
                                        DB.tblPurchaseOrderItems.Add(Item);
                                        DB.SaveChanges();
                                    }


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.PurchaseOrderId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                            else
                            {
                                string errorMessage = await response.Content.ReadAsStringAsync();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = errorMessage;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                            }
                        }
                    }
                }
                else if (SyncLog.Operation == "Delete")
                {
                    if (SyncLog.isQB != true)
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/purchaseorder?operation=delete";

                        QBDeleteClass DeleteData = new QBDeleteClass();
                        Data = DB.tblPurchaseOrders.Where(x => x.PurchaseOrderId == SyncLog.Id).FirstOrDefault();
                        DeleteData.Id = Data.QBId.ToString();
                        DeleteData.SyncToken = Data.SyncToken;



                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(DeleteData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                string jsonResponse = await response.Content.ReadAsStringAsync();
                                dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                                // Now you can access the data using the model
                                var QBId = estimateModel["PurchaseOrder"]["Id"];


                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                string errorMessage = await response.Content.ReadAsStringAsync();
                                var errorResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(errorMessage);
                                if (errorResponse.Fault.Error.FirstOrDefault().Message.Contains("Object Not Found"))
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = errorResponse.Fault.Error.FirstOrDefault().Message;
                                    SyncLogData.EditDate = DateTime.Now;
                                    SyncLogData.isSync = true;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = errorMessage;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }

                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        Data = DB.tblPurchaseOrders.Select(r => r).Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if (Data != null)
                        {
                            Data.isDelete = true;
                            Data.EditDate = DateTime.Now;
                            DB.Entry(Data);
                            DB.SaveChanges();

                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Data.EstimateId;
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Message = "Object Not Found";
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }

                    }
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
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                //return ResponseMessage(responseMessage);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An exception occurred: {ex.Message}");
                // Additional handling for generic exceptions

                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = ex.InnerException != null && ex.InnerException.InnerException != null ? new StringContent(ex.InnerException.InnerException.Message) : new StringContent(ex.Message);
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                //return ResponseMessage(responseMessage);
            }

        }

        [HttpPost]
        public async Task SyncInvoiceAsync(tblSyncLog SyncLog)
        {
            try
            {
                tblInvoice Data = new tblInvoice();
                List<tblInvoiceItem> ItemData = new List<tblInvoiceItem>();
                List<tblInvoiceFile> FileData = new List<tblInvoiceFile>();
                if (SyncLog.Operation == "Create")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblInvoices.Where(x => x.InvoiceId == SyncLog.Id).FirstOrDefault();
                        ItemData = DB.tblInvoiceItems.Where(x => x.InvoiceId == SyncLog.Id).ToList();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            DB = new earthcoEntities();
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/invoice?minorversion=23";

                        QBInvoiceClass InvoiceData = new QBInvoiceClass();
                        Models.QBInvoiceCUClass.LineDetail LineData = new Models.QBInvoiceCUClass.LineDetail();
                        InvoiceData.CustomerMemo = new QBInvoiceCUClass.CustomerMemo();
                        InvoiceData.CustomerMemo.value = Data.CustomerMessage;
                        //InvoiceData.TotalAmt = (decimal)Data.TotalAmount;
                        InvoiceData.Balance = (decimal)Data.BalanceAmount;
                        InvoiceData.DocNumber = Data.InvoiceNumber;
                        if(Data.DueDate!=null)
                        {
                            InvoiceData.DueDate =(DateTime) Data.DueDate;
                        }
                        
                        //EsitimateData.SyncToken = "0";
                        //EsitimateData.Id = "1103";
                        InvoiceData.CustomerRef = new QBInvoiceCUClass.CustomerRef();
                        InvoiceData.CustomerRef.value = Data.tblUser.QBId.ToString();


                        foreach (var item in ItemData)
                        {
                            LineData.Amount = Convert.ToDecimal(item.Amount);
                            LineData.Description = item.Description;
                            LineData.DetailType = item.tblItem.Type;
                            LineData.SalesItemLineDetail = new Models.QBInvoiceCUClass.SalesItemLineDetail();
                            LineData.SalesItemLineDetail.ItemRef = new QBInvoiceCUClass.ItemRef();
                            LineData.SalesItemLineDetail.ItemRef.value = item.tblItem.QBId.ToString();
                            LineData.SalesItemLineDetail.UnitPrice = Convert.ToDecimal(item.Rate);
                            LineData.SalesItemLineDetail.Qty = (int)item.Qty;
                            InvoiceData.Line = new List<Models.QBInvoiceCUClass.LineDetail>();
                            InvoiceData.Line.Add(LineData);
                        }
                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(InvoiceData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                string jsonResponse = await response.Content.ReadAsStringAsync();
                                dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                                // Now you can access the data using the model
                                var QBId = estimateModel["Invoice"]["Id"];
                                var SyncToken = estimateModel["Invoice"]["SyncToken"];

                                Data.QBId = SyncToken;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                            }
                            else
                            {
                                // Handle error
                                string errorMessage = await response.Content.ReadAsStringAsync();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = errorMessage;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }


                        //foreach (tblEstimateFile item in FileData)
                        //{
                        //    string baseUrl = "https://sandbox-quickbooks.api.intuit.com";

                        //    // Endpoint
                        //    //string endpoint = "/v3/company/<realmID>/upload"; // Replace <realmID> with the actual realm ID

                        //    // Complete URL
                        //    //apiUrl = baseUrl + endpoint;

                        //    // Request body
                        //    var requestBody = new
                        //    {
                        //        AttachableRef = new[]
                        //        {
                        //        new
                        //        {
                        //            EntityRef = new
                        //            {
                        //                type = "Estimate",
                        //                value = "95"
                        //            }
                        //        }
                        //    },
                        //        //ContentType = "image/jpg",
                        //        //FileName = "receipt_nov15.jpg"
                        //    };

                        //    // Serialize the request body to JSON
                        //    string jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

                        //    using (var httpClient = new HttpClient())
                        //    using (var content = new MultipartFormDataContent())
                        //    {
                        //        // Add AttachableRef as a JSON string
                        //        //var attachableRefJson = "{\"AttachableRef\":[{\"EntityRef\":{\"type\":\"Invoice\",\"value\":\"95\"}}],\"ContentType\":\"image/jpg\",\"FileName\":\"receipt_nov15.jpg\"}";
                        //        content.Add(new StringContent(jsonRequestBody), "application/json");

                        //        // Load the file from a URL
                        //        var fileUrl = "https://earthcoapi.yehtohoga.com/"+item.FilePath+""; // Replace with the actual file URL
                        //        var fileBytes = await httpClient.GetByteArrayAsync(fileUrl);
                        //        var fileContent = new ByteArrayContent(fileBytes);
                        //        content.Add(fileContent, "file", "receipt_nov15.jpg");

                        //        // Make the POST request
                        //        apiUrl = $"/v3/company/" + TokenData.realmId + "/upload";
                        //        var requestUrl = $"{baseUrl}{apiUrl}";

                        //        var response = await httpClient.PostAsync(requestUrl, content);

                        //        if (response.IsSuccessStatusCode)
                        //        {
                        //            // Handle success
                        //            Console.WriteLine("File uploaded successfully!");
                        //        }
                        //        else
                        //        {
                        //            // Handle failure
                        //            Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        //        }
                        //    }
                        //}



                    }
                    else
                    {
                        Data = DB.tblInvoices.Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if (Data != null)
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Convert.ToInt32(Data.InvoiceId);
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblToken TokenData = DB.tblTokens.FirstOrDefault();
                            string AccessToken = "";

                            var diffOfDates = DateTime.Now - TokenData.EditDate;
                            do
                            {
                                TokenData = DB.tblTokens.FirstOrDefault();
                                diffOfDates = DateTime.Now - TokenData.EditDate;
                                if (diffOfDates.Value.Hours >= 1)
                                {
                                    HomeController.GetAuthTokensUsingRefreshTokenAsync();
                                }
                            } while (diffOfDates.Value.Hours >= 1);

                            AccessToken = TokenData.AccessToken;

                            using (HttpClient client = new HttpClient())
                            {
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                                client.DefaultRequestHeaders.Accept.Clear();
                                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                                // Make the GET request
                                HttpResponseMessage response = await client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/invoice/" + SyncLog.QBId + "?minorversion=23");

                                if (response.IsSuccessStatusCode)
                                {
                                    var readTask = response.Content.ReadAsStringAsync();
                                    readTask.Wait();
                                    string Test = readTask.Result;
                                    InvoiceQB.InvoiceResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<InvoiceQB.InvoiceResponse>(Test);

                                    Data.QBId = Convert.ToInt32(ResponseData.Invoice.Id);
                                    Data.SyncToken = ResponseData.Invoice.SyncToken;
                                    Data.InvoiceNumber = ResponseData.Invoice.DocNumber;
                                    int QBId = Convert.ToInt32(ResponseData.Invoice.CustomerRef.value); ;
                                    int CustomerId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.CustomerId = CustomerId;

                                    Data.TotalAmount = (double)ResponseData.Invoice.TotalAmt;
                                    Data.BalanceAmount = (double)ResponseData.Invoice.Balance;
                                    Data.CustomerMessage = ResponseData.Invoice.CustomerMemo.value;
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    //Data.CreatedBy = UserId;
                                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("I").FirstOrDefault());
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblInvoices.Add(Data);
                                    DB.SaveChanges();


                                    foreach (InvoiceQB.Line item in ResponseData.Invoice.Line)
                                    {
                                        tblInvoiceItem Item = new tblInvoiceItem();
                                        Item.Description = item.Description;
                                        Item.Qty = Convert.ToInt32(item.SalesItemLineDetail.Qty);
                                        Item.Rate = Convert.ToDouble(item.SalesItemLineDetail.UnitPrice);
                                        Item.Name = item.SalesItemLineDetail.ItemRef.name;
                                        Item.Amount = Convert.ToDouble(item.Amount);
                                        QBId = Convert.ToInt32(item.SalesItemLineDetail.ItemRef.value);
                                        int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                        Item.ItemId = ItemId;

                                        Item.InvoiceId = Data.InvoiceId;

                                        Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        Item.isCost = false;
                                        Item.isActive = true;
                                        Item.isDelete = false;
                                        DB.tblInvoiceItems.Add(Item);
                                        DB.SaveChanges();
                                    }


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.InvoiceId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();

                                }
                                else
                                {
                                    string errorMessage = await response.Content.ReadAsStringAsync();

                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = errorMessage;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                        }




                    }
                }
                else if (SyncLog.Operation == "Update")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblInvoices.Where(x => x.InvoiceId == SyncLog.Id).FirstOrDefault();
                        ItemData = DB.tblInvoiceItems.Where(x => x.InvoiceId == SyncLog.Id).ToList();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/invoice?minorversion=23";


                        QBInvoiceClass InvoiceData = new QBInvoiceClass();
                        Models.QBInvoiceCUClass.LineDetail LineData = new Models.QBInvoiceCUClass.LineDetail();
                        InvoiceData.CustomerMemo = new QBInvoiceCUClass.CustomerMemo();
                        InvoiceData.CustomerMemo.value = Data.CustomerMessage;
                        //InvoiceData.TotalAmt = (decimal)Data.TotalAmount;
                        InvoiceData.Balance = (decimal)Data.BalanceAmount;
                        InvoiceData.DocNumber = Data.InvoiceNumber;
                        if (Data.DueDate != null)
                        {
                            InvoiceData.DueDate = (DateTime)Data.DueDate;
                        }

                        InvoiceData.SyncToken = Data.SyncToken;
                        InvoiceData.Id = Data.QBId.ToString();
                        InvoiceData.CustomerRef = new QBInvoiceCUClass.CustomerRef();
                        InvoiceData.CustomerRef.value = Data.tblUser.QBId.ToString();


                        foreach (var item in ItemData)
                        {
                            LineData.Amount = Convert.ToDecimal(item.Amount);
                            LineData.Description = item.Description;
                            LineData.DetailType = item.tblItem.Type;
                            LineData.SalesItemLineDetail = new Models.QBInvoiceCUClass.SalesItemLineDetail();
                            LineData.SalesItemLineDetail.ItemRef = new QBInvoiceCUClass.ItemRef();
                            LineData.SalesItemLineDetail.ItemRef.value = item.tblItem.QBId.ToString();
                            LineData.SalesItemLineDetail.UnitPrice = Convert.ToDecimal(item.Rate);
                            LineData.SalesItemLineDetail.Qty = (int)item.Qty;
                            InvoiceData.Line = new List<Models.QBInvoiceCUClass.LineDetail>();
                            InvoiceData.Line.Add(LineData);
                        }


                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(InvoiceData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                string jsonResponse = await response.Content.ReadAsStringAsync();
                                dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                                // Now you can access the data using the model

                                var QBId = estimateModel["Invoice"]["Id"];
                                var SyncToken = estimateModel["Invoice"]["SyncToken"];

                                Data.QBId = QBId;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                string errorMessage = await response.Content.ReadAsStringAsync();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = errorMessage;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;

                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            // Make the GET request
                            HttpResponseMessage response = await client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/invoice/" + SyncLog.QBId + "?minorversion=23");

                            if (response.IsSuccessStatusCode)
                            {
                                var readTask = response.Content.ReadAsStringAsync();
                                readTask.Wait();
                                string Test = readTask.Result;
                                InvoiceQB.InvoiceResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<InvoiceQB.InvoiceResponse>(Test);


                                int QBId = Convert.ToInt32(ResponseData.Invoice.Id);

                                Data = DB.tblInvoices.Where(x => x.QBId == QBId).FirstOrDefault();
                                if (Data != null)
                                {
                                    Data.QBId = Convert.ToInt32(ResponseData.Invoice.Id);
                                    Data.SyncToken = ResponseData.Invoice.SyncToken;
                                    Data.InvoiceNumber = ResponseData.Invoice.DocNumber;
                                    QBId = Convert.ToInt32(ResponseData.Invoice.CustomerRef.value); ;
                                    int CustomerId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.CustomerId = CustomerId;

                                    Data.TotalAmount = (double)ResponseData.Invoice.TotalAmt;
                                    Data.BalanceAmount = (double)ResponseData.Invoice.Balance;
                                    Data.CustomerMessage = ResponseData.Invoice.CustomerMemo.value;
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    //Data.CreatedBy = UserId;
                                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("I").FirstOrDefault());
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.Entry(Data);
                                    DB.SaveChanges();

                                    ItemData = DB.tblInvoiceItems.Where(x => x.InvoiceId == Data.InvoiceId).ToList();
                                    if (ItemData != null && ItemData.Count != 0)
                                    {
                                        DB.tblInvoiceItems.RemoveRange(ItemData);
                                    }

                                    foreach (InvoiceQB.Line item in ResponseData.Invoice.Line)
                                    {
                                        tblInvoiceItem Item = new tblInvoiceItem();
                                        Item.Description = item.Description;
                                        Item.Qty = Convert.ToInt32(item.SalesItemLineDetail.Qty);
                                        Item.Rate = Convert.ToDouble(item.SalesItemLineDetail.UnitPrice);
                                        Item.Name = item.SalesItemLineDetail.ItemRef.name;
                                        Item.Amount = Convert.ToDouble(item.Amount);
                                        QBId = Convert.ToInt32(item.SalesItemLineDetail.ItemRef.value);
                                        int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                        Item.ItemId = ItemId;

                                        Item.InvoiceId = Data.InvoiceId;

                                        Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        Item.isCost = false;
                                        Item.isActive = true;
                                        Item.isDelete = false;
                                        DB.tblInvoiceItems.Add(Item);
                                        DB.SaveChanges();
                                    }

                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.InvoiceId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    Data.QBId = Convert.ToInt32(ResponseData.Invoice.Id);
                                    Data.SyncToken = ResponseData.Invoice.SyncToken;
                                    Data.InvoiceNumber = ResponseData.Invoice.DocNumber;
                                    QBId = Convert.ToInt32(ResponseData.Invoice.CustomerRef.value); ;
                                    int CustomerId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.CustomerId = CustomerId;

                                    Data.TotalAmount = (double)ResponseData.Invoice.TotalAmt;
                                    Data.BalanceAmount = (double)ResponseData.Invoice.Balance;
                                    Data.CustomerMessage = ResponseData.Invoice.CustomerMemo.value;
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    //Data.CreatedBy = UserId;
                                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("I").FirstOrDefault());
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblInvoices.Add(Data);
                                    DB.SaveChanges();

                                    ItemData = DB.tblInvoiceItems.Where(x => x.InvoiceId == Data.InvoiceId).ToList();
                                    if (ItemData != null && ItemData.Count != 0)
                                    {
                                        DB.tblInvoiceItems.RemoveRange(ItemData);
                                    }

                                    foreach (InvoiceQB.Line item in ResponseData.Invoice.Line)
                                    {
                                        tblInvoiceItem Item = new tblInvoiceItem();
                                        Item.Description = item.Description;
                                        Item.Qty = Convert.ToInt32(item.SalesItemLineDetail.Qty);
                                        Item.Rate = Convert.ToDouble(item.SalesItemLineDetail.UnitPrice);
                                        Item.Name = item.SalesItemLineDetail.ItemRef.name;
                                        Item.Amount = Convert.ToDouble(item.Amount);
                                        QBId = Convert.ToInt32(item.SalesItemLineDetail.ItemRef.value);
                                        int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                        Item.ItemId = ItemId;

                                        Item.InvoiceId = Data.InvoiceId;

                                        Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        Item.isCost = false;
                                        Item.isActive = true;
                                        Item.isDelete = false;
                                        DB.tblInvoiceItems.Add(Item);
                                        DB.SaveChanges();
                                    }


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.InvoiceId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                            else
                            {
                                string errorMessage = await response.Content.ReadAsStringAsync();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = errorMessage;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                            }
                        }
                    }
                }
                else if (SyncLog.Operation == "Delete")
                {
                    if (SyncLog.isQB != true)
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/invoice?operation=delete";

                        QBDeleteClass DeleteData = new QBDeleteClass();
                        Data = DB.tblInvoices.Where(x => x.InvoiceId == SyncLog.Id).FirstOrDefault();
                        DeleteData.Id = Data.QBId.ToString();
                        DeleteData.SyncToken = Data.SyncToken;



                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(DeleteData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                string jsonResponse = await response.Content.ReadAsStringAsync();
                                dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                                // Now you can access the data using the model
                                var QBId = estimateModel["Invoice"]["Id"];


                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                string errorMessage = await response.Content.ReadAsStringAsync();
                                var errorResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(errorMessage);
                                if (errorResponse.Fault.Error.FirstOrDefault().Message.Contains("Object Not Found"))
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = errorResponse.Fault.Error.FirstOrDefault().Message;
                                    SyncLogData.EditDate = DateTime.Now;
                                    SyncLogData.isSync = true;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = errorMessage;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }

                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        Data = DB.tblInvoices.Select(r => r).Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if (Data != null)
                        {
                            Data.isDelete = true;
                            Data.EditDate = DateTime.Now;
                            DB.Entry(Data);
                            DB.SaveChanges();

                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Data.EstimateId;
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Message = "Object Not Found";
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }

                    }
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
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                //return ResponseMessage(responseMessage);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An exception occurred: {ex.Message}");
                // Additional handling for generic exceptions

                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = ex.InnerException != null && ex.InnerException.InnerException != null ? new StringContent(ex.InnerException.InnerException.Message) : new StringContent(ex.Message);
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                //return ResponseMessage(responseMessage);
            }

        }

        [HttpPost]
        public async Task SyncBillAsync(tblSyncLog SyncLog)
        {
            try
            {
                tblBill Data = new tblBill();
                List<tblBillItem> ItemData = new List<tblBillItem>();
                List<tblBillFile> FileData = new List<tblBillFile>();
                if (SyncLog.Operation == "Create")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblBills.Where(x => x.BillId == SyncLog.Id).FirstOrDefault();
                        ItemData = DB.tblBillItems.Where(x => x.BillId == SyncLog.Id).ToList();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            DB = new earthcoEntities();
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/bill?minorversion=23";

                        QBBillClass BillData = new QBBillClass();
                        Models.QBBillCUClass.LineDetail LineData = new Models.QBBillCUClass.LineDetail();
                        BillData.TotalAmt = (decimal)Data.Amount;
                        BillData.DocNumber = Data.BillNumber;
                        if (Data.DueDate != null)
                        {
                            BillData.DueDate = (DateTime)Data.DueDate;
                        }

                        //EsitimateData.SyncToken = "0";
                        //EsitimateData.Id = "1103";
                        BillData.VendorRef = new QBBillCUClass.VendorRef();
                        BillData.VendorRef.value = Data.tblUser.QBId.ToString();


                        foreach (var item in ItemData)
                        {
                            LineData.Amount = Convert.ToDecimal(item.Amount);
                            LineData.Description = item.Description;
                            LineData.DetailType = item.tblItem.Type;
                            LineData.ItemBasedExpenseLineDetail = new Models.QBBillCUClass.ItemBasedExpenseLineDetail();
                            LineData.ItemBasedExpenseLineDetail.ItemRef = new QBBillCUClass.ItemRef();
                            LineData.ItemBasedExpenseLineDetail.ItemRef.value = item.tblItem.QBId.ToString();
                            LineData.ItemBasedExpenseLineDetail.UnitPrice = Convert.ToDecimal(item.Rate);
                            LineData.ItemBasedExpenseLineDetail.Qty = (int)item.Qty;
                            BillData.Line = new List<Models.QBBillCUClass.LineDetail>();
                            BillData.Line.Add(LineData);
                        }
                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(BillData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                string jsonResponse = await response.Content.ReadAsStringAsync();
                                dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                                // Now you can access the data using the model
                                var QBId = estimateModel["Bill"]["Id"];
                                var SyncToken = estimateModel["Bill"]["SyncToken"];

                                Data.QBId = SyncToken;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                            }
                            else
                            {
                                // Handle error
                                string errorMessage = await response.Content.ReadAsStringAsync();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = errorMessage;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }


                        //foreach (tblEstimateFile item in FileData)
                        //{
                        //    string baseUrl = "https://sandbox-quickbooks.api.intuit.com";

                        //    // Endpoint
                        //    //string endpoint = "/v3/company/<realmID>/upload"; // Replace <realmID> with the actual realm ID

                        //    // Complete URL
                        //    //apiUrl = baseUrl + endpoint;

                        //    // Request body
                        //    var requestBody = new
                        //    {
                        //        AttachableRef = new[]
                        //        {
                        //        new
                        //        {
                        //            EntityRef = new
                        //            {
                        //                type = "Estimate",
                        //                value = "95"
                        //            }
                        //        }
                        //    },
                        //        //ContentType = "image/jpg",
                        //        //FileName = "receipt_nov15.jpg"
                        //    };

                        //    // Serialize the request body to JSON
                        //    string jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

                        //    using (var httpClient = new HttpClient())
                        //    using (var content = new MultipartFormDataContent())
                        //    {
                        //        // Add AttachableRef as a JSON string
                        //        //var attachableRefJson = "{\"AttachableRef\":[{\"EntityRef\":{\"type\":\"Bill\",\"value\":\"95\"}}],\"ContentType\":\"image/jpg\",\"FileName\":\"receipt_nov15.jpg\"}";
                        //        content.Add(new StringContent(jsonRequestBody), "application/json");

                        //        // Load the file from a URL
                        //        var fileUrl = "https://earthcoapi.yehtohoga.com/"+item.FilePath+""; // Replace with the actual file URL
                        //        var fileBytes = await httpClient.GetByteArrayAsync(fileUrl);
                        //        var fileContent = new ByteArrayContent(fileBytes);
                        //        content.Add(fileContent, "file", "receipt_nov15.jpg");

                        //        // Make the POST request
                        //        apiUrl = $"/v3/company/" + TokenData.realmId + "/upload";
                        //        var requestUrl = $"{baseUrl}{apiUrl}";

                        //        var response = await httpClient.PostAsync(requestUrl, content);

                        //        if (response.IsSuccessStatusCode)
                        //        {
                        //            // Handle success
                        //            Console.WriteLine("File uploaded successfully!");
                        //        }
                        //        else
                        //        {
                        //            // Handle failure
                        //            Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        //        }
                        //    }
                        //}



                    }
                    else
                    {
                        Data = DB.tblBills.Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if (Data != null)
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Convert.ToInt32(Data.BillId);
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblToken TokenData = DB.tblTokens.FirstOrDefault();
                            string AccessToken = "";

                            var diffOfDates = DateTime.Now - TokenData.EditDate;
                            do
                            {
                                TokenData = DB.tblTokens.FirstOrDefault();
                                diffOfDates = DateTime.Now - TokenData.EditDate;
                                if (diffOfDates.Value.Hours >= 1)
                                {
                                    HomeController.GetAuthTokensUsingRefreshTokenAsync();
                                }
                            } while (diffOfDates.Value.Hours >= 1);

                            AccessToken = TokenData.AccessToken;

                            using (HttpClient client = new HttpClient())
                            {
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                                client.DefaultRequestHeaders.Accept.Clear();
                                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                                // Make the GET request
                                HttpResponseMessage response = await client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/bill/" + SyncLog.QBId + "?minorversion=23");

                                if (response.IsSuccessStatusCode)
                                {
                                    var readTask = response.Content.ReadAsStringAsync();
                                    readTask.Wait();
                                    string Test = readTask.Result;
                                    QBBill.BillResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBBill.BillResponse>(Test);

                                    Data.QBId = Convert.ToInt32(ResponseData.Bill.Id);
                                    Data.SyncToken = ResponseData.Bill.SyncToken;
                                    Data.BillNumber = ResponseData.Bill.DocNumber;
                                    int QBId = Convert.ToInt32(ResponseData.Bill.VendorRef.value); ;
                                    int VendorId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.SupplierId = VendorId;

                                    Data.DueDate = (DateTime)ResponseData.Bill.DueDate;
                                    Data.BillDate = (DateTime)ResponseData.Bill.TxnDate;
                                    Data.Amount = (double)ResponseData.Bill.TotalAmt;
                                    Data.Currency = "USD";
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    //Data.CreatedBy = UserId;
                                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("B").FirstOrDefault());
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblBills.Add(Data);
                                    DB.SaveChanges();


                                    foreach (QBBill.Line item in ResponseData.Bill.Line)
                                    {
                                        tblBillItem Item = new tblBillItem();
                                        Item.Description = item.Description;
                                        Item.Qty = Convert.ToInt32(item.ItemBasedExpenseLineDetail.Qty);
                                        Item.Rate = Convert.ToDouble(item.ItemBasedExpenseLineDetail.UnitPrice);
                                        Item.Name = item.ItemBasedExpenseLineDetail.ItemRef.name;
                                        Item.Amount = Convert.ToDouble(item.Amount);
                                        QBId = Convert.ToInt32(item.ItemBasedExpenseLineDetail.ItemRef.value);
                                        int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                        Item.ItemId = ItemId;

                                        Item.BillId = Data.BillId;

                                        Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        Item.isActive = true;
                                        Item.isDelete = false;
                                        DB.tblBillItems.Add(Item);
                                        DB.SaveChanges();
                                    }


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.BillId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();

                                }
                                else
                                {
                                    string errorMessage = await response.Content.ReadAsStringAsync();

                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = errorMessage;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                        }




                    }
                }
                else if (SyncLog.Operation == "Update")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblBills.Where(x => x.BillId == SyncLog.Id).FirstOrDefault();
                        ItemData = DB.tblBillItems.Where(x => x.BillId == SyncLog.Id).ToList();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/bill?minorversion=23";


                        QBBillClass BillData = new QBBillClass();
                        Models.QBBillCUClass.LineDetail LineData = new Models.QBBillCUClass.LineDetail();
                        BillData.TotalAmt = (decimal)Data.Amount;
                        BillData.DocNumber = Data.BillNumber;
                        if (Data.DueDate != null)
                        {
                            BillData.DueDate = (DateTime)Data.DueDate;
                        }

                        BillData.SyncToken = Data.SyncToken;
                        BillData.Id = Data.QBId.ToString();
                        BillData.VendorRef = new QBBillCUClass.VendorRef();
                        BillData.VendorRef.value = Data.tblUser.QBId.ToString();


                        foreach (var item in ItemData)
                        {
                            LineData.Amount = Convert.ToDecimal(item.Amount);
                            LineData.Description = item.Description;
                            LineData.DetailType = item.tblItem.Type;
                            LineData.ItemBasedExpenseLineDetail = new Models.QBBillCUClass.ItemBasedExpenseLineDetail();
                            LineData.ItemBasedExpenseLineDetail.ItemRef = new QBBillCUClass.ItemRef();
                            LineData.ItemBasedExpenseLineDetail.ItemRef.value = item.tblItem.QBId.ToString();
                            LineData.ItemBasedExpenseLineDetail.UnitPrice = Convert.ToDecimal(item.Rate);
                            LineData.ItemBasedExpenseLineDetail.Qty = (int)item.Qty;
                            BillData.Line = new List<Models.QBBillCUClass.LineDetail>();
                            BillData.Line.Add(LineData);
                        }


                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(BillData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                string jsonResponse = await response.Content.ReadAsStringAsync();
                                dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                                // Now you can access the data using the model

                                var QBId = estimateModel["Bill"]["Id"];
                                var SyncToken = estimateModel["Bill"]["SyncToken"];

                                Data.QBId = QBId;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                string errorMessage = await response.Content.ReadAsStringAsync();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = errorMessage;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;

                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            // Make the GET request
                            HttpResponseMessage response = await client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/bill/" + SyncLog.QBId + "?minorversion=23");

                            if (response.IsSuccessStatusCode)
                            {
                                var readTask = response.Content.ReadAsStringAsync();
                                readTask.Wait();
                                string Test = readTask.Result;
                                QBBill.BillResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBBill.BillResponse>(Test);


                                int QBId = Convert.ToInt32(ResponseData.Bill.Id);

                                Data = DB.tblBills.Where(x => x.QBId == QBId).FirstOrDefault();
                                if (Data != null)
                                {
                                    Data.QBId = Convert.ToInt32(ResponseData.Bill.Id);
                                    Data.SyncToken = ResponseData.Bill.SyncToken;
                                    Data.BillNumber = ResponseData.Bill.DocNumber;
                                    QBId = Convert.ToInt32(ResponseData.Bill.VendorRef.value); ;
                                    int VendorId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.SupplierId = VendorId;

                                    Data.DueDate = (DateTime)ResponseData.Bill.DueDate;
                                    Data.BillDate = (DateTime)ResponseData.Bill.TxnDate;
                                    Data.Amount = (double)ResponseData.Bill.TotalAmt;
                                    Data.Currency = "USD";
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    //Data.CreatedBy = UserId;
                                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("B").FirstOrDefault());
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblBills.Add(Data);
                                    DB.SaveChanges();

                                    ItemData = DB.tblBillItems.Where(x => x.BillId == Data.BillId).ToList();
                                    if (ItemData != null && ItemData.Count != 0)
                                    {
                                        DB.tblBillItems.RemoveRange(ItemData);
                                    }

                                    foreach (QBBill.Line item in ResponseData.Bill.Line)
                                    {
                                        tblBillItem Item = new tblBillItem();
                                        Item.Description = item.Description;
                                        Item.Qty = Convert.ToInt32(item.ItemBasedExpenseLineDetail.Qty);
                                        Item.Rate = Convert.ToDouble(item.ItemBasedExpenseLineDetail.UnitPrice);
                                        Item.Name = item.ItemBasedExpenseLineDetail.ItemRef.name;
                                        Item.Amount = Convert.ToDouble(item.Amount);
                                        QBId = Convert.ToInt32(item.ItemBasedExpenseLineDetail.ItemRef.value);
                                        int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                        Item.ItemId = ItemId;

                                        Item.BillId = Data.BillId;

                                        Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        Item.isActive = true;
                                        Item.isDelete = false;
                                        DB.tblBillItems.Add(Item);
                                        DB.SaveChanges();
                                    }


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.BillId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    Data.QBId = Convert.ToInt32(ResponseData.Bill.Id);
                                    Data.SyncToken = ResponseData.Bill.SyncToken;
                                    Data.BillNumber = ResponseData.Bill.DocNumber;
                                    QBId = Convert.ToInt32(ResponseData.Bill.VendorRef.value); ;
                                    int VendorId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.SupplierId = VendorId;

                                    Data.DueDate = (DateTime)ResponseData.Bill.DueDate;
                                    Data.BillDate = (DateTime)ResponseData.Bill.TxnDate;
                                    Data.Amount = (double)ResponseData.Bill.TotalAmt;
                                    Data.Currency = "USD";
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    //Data.CreatedBy = UserId;
                                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("B").FirstOrDefault());
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblBills.Add(Data);
                                    DB.SaveChanges();

                                    ItemData = DB.tblBillItems.Where(x => x.BillId == Data.BillId).ToList();
                                    if (ItemData != null && ItemData.Count != 0)
                                    {
                                        DB.tblBillItems.RemoveRange(ItemData);
                                    }

                                    foreach (QBBill.Line item in ResponseData.Bill.Line)
                                    {
                                        tblBillItem Item = new tblBillItem();
                                        Item.Description = item.Description;
                                        Item.Qty = Convert.ToInt32(item.ItemBasedExpenseLineDetail.Qty);
                                        Item.Rate = Convert.ToDouble(item.ItemBasedExpenseLineDetail.UnitPrice);
                                        Item.Name = item.ItemBasedExpenseLineDetail.ItemRef.name;
                                        Item.Amount = Convert.ToDouble(item.Amount);
                                        QBId = Convert.ToInt32(item.ItemBasedExpenseLineDetail.ItemRef.value);
                                        int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                        Item.ItemId = ItemId;

                                        Item.BillId = Data.BillId;

                                        Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        Item.isActive = true;
                                        Item.isDelete = false;
                                        DB.tblBillItems.Add(Item);
                                        DB.SaveChanges();
                                    }


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.BillId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                            else
                            {
                                string errorMessage = await response.Content.ReadAsStringAsync();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = errorMessage;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                            }
                        }
                    }
                }
                else if (SyncLog.Operation == "Delete")
                {
                    if (SyncLog.isQB != true)
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/bill?operation=delete";

                        QBDeleteClass DeleteData = new QBDeleteClass();
                        Data = DB.tblBills.Where(x => x.BillId == SyncLog.Id).FirstOrDefault();
                        DeleteData.Id = Data.QBId.ToString();
                        DeleteData.SyncToken = Data.SyncToken;



                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(DeleteData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                string jsonResponse = await response.Content.ReadAsStringAsync();
                                dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                                // Now you can access the data using the model
                                var QBId = estimateModel["Bill"]["Id"];


                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                string errorMessage = await response.Content.ReadAsStringAsync();
                                var errorResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(errorMessage);
                                if (errorResponse.Fault.Error.FirstOrDefault().Message.Contains("Object Not Found"))
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = errorResponse.Fault.Error.FirstOrDefault().Message;
                                    SyncLogData.EditDate = DateTime.Now;
                                    SyncLogData.isSync = true;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = errorMessage;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }

                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        Data = DB.tblBills.Select(r => r).Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if (Data != null)
                        {
                            Data.isDelete = true;
                            Data.EditDate = DateTime.Now;
                            DB.Entry(Data);
                            DB.SaveChanges();

                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Data.BillId;
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Message = "Object Not Found";
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }

                    }
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
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                //return ResponseMessage(responseMessage);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An exception occurred: {ex.Message}");
                // Additional handling for generic exceptions

                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = ex.InnerException != null && ex.InnerException.InnerException != null ? new StringContent(ex.InnerException.InnerException.Message) : new StringContent(ex.Message);
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                //return ResponseMessage(responseMessage);
            }

        }

        [HttpPost]
        public async Task SyncItemAsync(tblSyncLog SyncLog)
        {
            try
            {
                tblItem Data = new tblItem();
                if (SyncLog.Operation == "Create")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblItems.Where(x => x.ItemId == SyncLog.Id).FirstOrDefault();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            DB = new earthcoEntities();
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/item?minorversion=23";

                        QBItemClass ItemData = new QBItemClass();
                        ItemData.Type = "NonInventory";
                        ItemData.Name = Data.ItemName;
                        ItemData.UnitPrice = (decimal)Data.SalePrice;
                        ItemData.PurchaseCost = (decimal)Data.PurchasePrice;

                        //EsitimateData.SyncToken = "0";
                        //EsitimateData.Id = "1103";
                        ItemData.IncomeAccountRef = new IncomeAccountRef();
                        ItemData.IncomeAccountRef.value = Data.tblAccount.QBId.ToString();
                        ItemData.ExpenseAccountRef = new ExpenseAccountRef();
                        ItemData.ExpenseAccountRef.value = Data.tblAccount1.QBId.ToString();

                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(ItemData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                string jsonResponse = await response.Content.ReadAsStringAsync();
                                dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                                // Now you can access the data using the model
                                var QBId = estimateModel["Item"]["Id"];
                                var SyncToken = estimateModel["Item"]["SyncToken"];

                                Data.QBId = SyncToken;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                            }
                            else
                            {
                                // Handle error
                                string errorMessage = await response.Content.ReadAsStringAsync();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = errorMessage;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        Data = DB.tblItems.Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if (Data != null)
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Convert.ToInt32(Data.ItemId);
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblToken TokenData = DB.tblTokens.FirstOrDefault();
                            string AccessToken = "";

                            var diffOfDates = DateTime.Now - TokenData.EditDate;
                            do
                            {
                                TokenData = DB.tblTokens.FirstOrDefault();
                                diffOfDates = DateTime.Now - TokenData.EditDate;
                                if (diffOfDates.Value.Hours >= 1)
                                {
                                    HomeController.GetAuthTokensUsingRefreshTokenAsync();
                                }
                            } while (diffOfDates.Value.Hours >= 1);

                            AccessToken = TokenData.AccessToken;

                            using (HttpClient client = new HttpClient())
                            {
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                                client.DefaultRequestHeaders.Accept.Clear();
                                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                                // Make the GET request
                                HttpResponseMessage response = await client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/item/" + SyncLog.QBId + "?minorversion=23");

                                if (response.IsSuccessStatusCode)
                                {
                                    var readTask = response.Content.ReadAsStringAsync();
                                    readTask.Wait();
                                    string Test = readTask.Result;
                                    QBItem.ItemResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBItem.ItemResponse>(Test);

                                    Data.QBId = Convert.ToInt32(ResponseData.Item.Id);
                                    Data.SyncToken = ResponseData.Item.SyncToken;

                                    Data.ItemName = ResponseData.Item.Name;
                                    Data.Type = ResponseData.Item.Type;
                                    Data.SalePrice = (double)ResponseData.Item.UnitPrice;
                                    Data.PurchasePrice = (double)ResponseData.Item.PurchaseCost;
                                    int QBId = Convert.ToInt32(ResponseData.Item.IncomeAccountRef.value);
                                    Data.IncomeAccount =DB.tblAccounts.Where(x=>x.QBId==QBId).Select(s=>s.AccountId).FirstOrDefault() ;
                                    QBId = Convert.ToInt32(ResponseData.Item.ExpenseAccountRef.value);
                                    Data.ExpenseAccount =DB.tblAccounts.Where(x=>x.QBId==QBId).Select(s=>s.AccountId).FirstOrDefault() ;
                                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblItems.Add(Data);
                                    DB.SaveChanges();


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.ItemId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();

                                }
                                else
                                {
                                    string errorMessage = await response.Content.ReadAsStringAsync();

                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = errorMessage;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                        }




                    }
                }
                else if (SyncLog.Operation == "Update")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblItems.Where(x => x.ItemId == SyncLog.Id).FirstOrDefault();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/item?minorversion=23";


                        QBItemClass ItemData = new QBItemClass();
                        ItemData.Type = "NonInventory";
                        ItemData.Name = Data.ItemName;
                        ItemData.UnitPrice = (decimal)Data.SalePrice;
                        ItemData.PurchaseCost = (decimal)Data.PurchasePrice;
                        ItemData.Name = Data.ItemName;

                        ItemData.SyncToken = Data.SyncToken;
                        ItemData.Id = Data.QBId.ToString();
                        ItemData.IncomeAccountRef = new IncomeAccountRef();
                        ItemData.IncomeAccountRef.value = Data.tblAccount.QBId.ToString();
                        ItemData.ExpenseAccountRef = new ExpenseAccountRef();
                        ItemData.ExpenseAccountRef.value = Data.tblAccount1.QBId.ToString();


                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(ItemData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                string jsonResponse = await response.Content.ReadAsStringAsync();
                                dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                                // Now you can access the data using the model

                                var QBId = estimateModel["Item"]["Id"];
                                var SyncToken = estimateModel["Item"]["SyncToken"];

                                Data.QBId = QBId;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                string errorMessage = await response.Content.ReadAsStringAsync();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = errorMessage;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;

                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            // Make the GET request
                            HttpResponseMessage response = await client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/item/" + SyncLog.QBId + "?minorversion=23");

                            if (response.IsSuccessStatusCode)
                            {
                                var readTask = response.Content.ReadAsStringAsync();
                                readTask.Wait();
                                string Test = readTask.Result;
                                QBItem.ItemResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBItem.ItemResponse>(Test);


                                int QBId = Convert.ToInt32(ResponseData.Item.Id);

                                Data = DB.tblItems.Where(x => x.QBId == QBId).FirstOrDefault();
                                if (Data != null)
                                {
                                    Data.QBId = Convert.ToInt32(ResponseData.Item.Id);
                                    Data.SyncToken = ResponseData.Item.SyncToken;

                                    Data.ItemName = ResponseData.Item.Name;
                                    Data.Type = ResponseData.Item.Type;
                                    Data.SalePrice = (double)ResponseData.Item.UnitPrice;
                                    Data.PurchasePrice = (double)ResponseData.Item.PurchaseCost;
                                    QBId = Convert.ToInt32(ResponseData.Item.IncomeAccountRef.value);
                                    Data.IncomeAccount = DB.tblAccounts.Where(x => x.QBId == QBId).Select(s => s.AccountId).FirstOrDefault();
                                    QBId = Convert.ToInt32(ResponseData.Item.ExpenseAccountRef.value);
                                    Data.ExpenseAccount = DB.tblAccounts.Where(x => x.QBId == QBId).Select(s => s.AccountId).FirstOrDefault();
                                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.Entry(Data);
                                    DB.SaveChanges();


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.ItemId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    Data.QBId = Convert.ToInt32(ResponseData.Item.Id);
                                    Data.SyncToken = ResponseData.Item.SyncToken;

                                    Data.ItemName = ResponseData.Item.Name;
                                    Data.Type = ResponseData.Item.Type;
                                    Data.SalePrice = (double)ResponseData.Item.UnitPrice;
                                    Data.PurchasePrice = (double)ResponseData.Item.PurchaseCost;
                                    QBId = Convert.ToInt32(ResponseData.Item.IncomeAccountRef.value);
                                    Data.IncomeAccount = DB.tblAccounts.Where(x => x.QBId == QBId).Select(s => s.AccountId).FirstOrDefault();
                                    QBId = Convert.ToInt32(ResponseData.Item.ExpenseAccountRef.value);
                                    Data.ExpenseAccount = DB.tblAccounts.Where(x => x.QBId == QBId).Select(s => s.AccountId).FirstOrDefault();
                                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblItems.Add(Data);
                                    DB.SaveChanges();


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.ItemId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                            else
                            {
                                string errorMessage = await response.Content.ReadAsStringAsync();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = errorMessage;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                            }
                        }
                    }
                }
                else if (SyncLog.Operation == "Delete")
                {
                    if (SyncLog.isQB != true)
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/item?operation=delete";

                        QBDeleteClass DeleteData = new QBDeleteClass();
                        Data = DB.tblItems.Where(x => x.ItemId == SyncLog.Id).FirstOrDefault();
                        DeleteData.Id = Data.QBId.ToString();
                        DeleteData.SyncToken = Data.SyncToken;



                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(DeleteData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                string jsonResponse = await response.Content.ReadAsStringAsync();
                                dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                                // Now you can access the data using the model
                                var QBId = estimateModel["Item"]["Id"];


                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                string errorMessage = await response.Content.ReadAsStringAsync();
                                var errorResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(errorMessage);
                                if (errorResponse.Fault.Error.FirstOrDefault().Message.Contains("Object Not Found"))
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = errorResponse.Fault.Error.FirstOrDefault().Message;
                                    SyncLogData.EditDate = DateTime.Now;
                                    SyncLogData.isSync = true;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = errorMessage;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }

                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        Data = DB.tblItems.Select(r => r).Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if (Data != null)
                        {
                            Data.isDelete = true;
                            Data.EditDate = DateTime.Now;
                            DB.Entry(Data);
                            DB.SaveChanges();

                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Data.ItemId;
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Message = "Object Not Found";
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }

                    }
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
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                //return ResponseMessage(responseMessage);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An exception occurred: {ex.Message}");
                // Additional handling for generic exceptions

                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = ex.InnerException != null && ex.InnerException.InnerException != null ? new StringContent(ex.InnerException.InnerException.Message) : new StringContent(ex.Message);
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                //return ResponseMessage(responseMessage);
            }

        }

        [HttpPost]
        public async Task<IHttpActionResult> SyncData()
        {
            try
            {
                if (!Request.Method.Equals(HttpMethod.Post))
                {
                    return (IHttpActionResult)Request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, "Method Not Allowed");
                }

                //VerifySignature Test = new VerifySignature();
                Dictionary<string, string> headers = Request.Headers.ToDictionary(header => header.Key, header => header.Value.ToString());

                // Replace "yourSecretKeyHere" with your actual secret key
                string verifier = "b4f26f4f-72f2-435f-a00d-af9561b05494";

                string payload = await Request.Content.ReadAsStringAsync();
                // Call IsRequestValid function
                //bool isValid = Test.IsRequestValid(headers, payload, verifier);

                RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(payload);
                tblSyncLog Result = null;
                // Access the data
                

                foreach (var eventNotification in rootObject.EventNotifications)
                {
                    foreach (var entity in eventNotification.DataChangeEvent.Entities)
                    {
                        int ID= Convert.ToInt32(entity.Id); ;
                        Result = DB.tblSyncLogs.Where(x => x.QBId == ID && x.Name== entity.Name).FirstOrDefault();
                        if(Result==null)
                        {
                            Result = new tblSyncLog();
                            Result.QBId = Convert.ToInt32(entity.Id);
                            Result.Name = entity.Name;
                            Result.Operation = entity.Operation;
                            Result.CreatedDate = entity.LastUpdated;
                            Result.isQB = true;
                            Result.isSync = false;
                            Result.Message = payload;
                            DB.tblSyncLogs.Add(Result);
                            DB.SaveChanges();
                        }
                        else
                        {
                            Result.Id = 0;
                            Result.QBId = Convert.ToInt32(entity.Id);
                            Result.Name = entity.Name;
                            Result.Operation = entity.Operation;
                            Result.CreatedDate = entity.LastUpdated;
                            Result.isQB = true;
                            Result.isSync = false;
                            Result.Message = payload;
                            DB.Entry(Result);
                            DB.SaveChanges();
                        }
                        
                    }
                }

                //Result.CreatedDate = DateTime.Now;
               
                
                // Process the valid webhook request...
                return Ok("Webhook request is valid.");

                //if (isValid)
                //{

                //    tblSyncLog Result = new tblSyncLog();
                //    Result.Message = payload;
                //    Result.CreatedDate = DateTime.Now;
                //    DB.tblSyncLogs.Add(Result);
                //    DB.SaveChanges();
                //    // Process the valid webhook request...
                //    return Ok("Webhook request is valid.");
                //}
                //else
                //{
                //    tblSyncLog Result = new tblSyncLog();
                //    Result.Message = "Not Valid " + payload;
                //    Result.CreatedDate = DateTime.Now;
                //    DB.tblSyncLogs.Add(Result);
                //    DB.SaveChanges();
                //    // Handle invalid request...
                //    return BadRequest("Invalid webhook request.");
                //}
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
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return ResponseMessage(responseMessage);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An exception occurred: {ex.Message}");
                // Additional handling for generic exceptions

                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = ex.InnerException != null && ex.InnerException.InnerException != null ? new StringContent(ex.InnerException.InnerException.Message) : new StringContent(ex.Message);
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return ResponseMessage(responseMessage);
            }

        }
    }
}
