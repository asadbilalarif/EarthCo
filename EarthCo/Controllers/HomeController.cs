using EarthCo.Models;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.OAuth2PlatformClient;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
//using System.Text.Json;
using System.Web;
using System.Web.Mvc;
using static EarthCo.Models.EstimateQB;
using static EarthCo.Models.QBErrorClass;
using static EarthCo.Models.PurchaseOrderQB;
using static EarthCo.Models.QBPurchaseOrderCUClass;
using static EarthCo.Models.QBInvoiceCUClass;
using static EarthCo.Models.QBBillCUClass;
using static EarthCo.Models.QBItemCUClass;
using static EarthCo.Models.QBStaffCUClass;
using static EarthCo.Models.QBCustomerCUClass;

namespace EarthCo.Controllers
{
    public class HomeController : Controller
    {


        earthcoEntities DB = new earthcoEntities();
        public static string clientid = ConfigurationManager.AppSettings["clientid"];
        public static string clientsecret = ConfigurationManager.AppSettings["clientsecret"];
        public static string redirectUrl = ConfigurationManager.AppSettings["redirectUrl"];
        public static string environment = ConfigurationManager.AppSettings["appEnvironment"];

        public static OAuth2Client auth2Client = new OAuth2Client(clientid, clientsecret, redirectUrl, environment);
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Session.Clear();
            Session.Abandon();
            Request.GetOwinContext().Authentication.SignOut("Cookies");

            return View();
        }

        public async Task<ActionResult> SignIn()
        {

            string apiUrl = "https://rest.method.me/api/v1/tables/Customer";


            var requestData = new
            {
                Email = "david1@landscaping.com",
                Name = "David Henderson1",
                Title = "Ms.",
                LastName = "Henderson1",
                FamilyName = "Test1",
                Phone = "1112223333",
                FirstName = "David1"
            };
            string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);


            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "APIKey NjU2NDk0MWFkMDUzMDRhN2Q3MzY2NDI3LjcxRjNEMEY2MTVDQzRBOTVBNzFCMUVGOEIwRTExRjIw ");

                // Create the request content
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");



                var responseTask = client.GetAsync("https://rest.method.me/api/v1/tables/Customer?skip=0&top=10&select=RecordID,Email,FirstName,LastName,BillAddressCountry,TotalBalance&orderby=LastModifiedDate desc");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    //var readTask = result.Content.ReadAsAsync<Bill>();
                    //readTask.Wait();
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    string Test = readTask.Result;
                    dynamic Data = Newtonsoft.Json.JsonConvert.DeserializeObject(Test);
                    //BillInfo = JsonSerializer.Deserialize<Bill>(readTask.Result);
                    var Tst = Data.value;
                    foreach (var item in Tst)
                    {
                        var Tst1 = (int)item["RecordID"];
                    }

                    //var Tst1 = Tst[0].RecordID;

                }

                


                // Make the POST request
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Parse and use the response data as needed
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    // Process jsonResponse
                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle error
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    // Handle error message
                    return View("Error");
                }
            }



            


            //byte[] nonceBytes = new byte[16]; // 16 bytes will result in a 128-bit nonce
            //using (var rng = RandomNumberGenerator.Create())
            //{
            //    rng.GetBytes(nonceBytes);
            //}

            //// Convert the byte array to a hexadecimal string
            //string nonce = BitConverter.ToString(nonceBytes).Replace("-", "").ToLower();

            //string methodSignInUrl = "https://auth.method.me/connect/authorize" +
            //    "?client_id=NjU2NDk0MWFkMDUzMDRhN2Q3MzY2NDI3LjcxRjNEMEY2MTVDQzRBOTVBNzFCMUVGOEIwRTExRjIw" +
            //    "&nonce="+nonce+"" +
            //    "&redirect_uri=https://localhost:44363/Home/MethodCallback" +
            //    "&response_type=code" +
            //    "&scope=api" +
            //    "&state=";

            //return Redirect(methodSignInUrl);
                }

        public ActionResult MethodCallback(string code, string state)
        {
            // Use the authorization code to get an access token from Method Identity Server
            // Perform a POST request to the token endpoint
            int I = 0;
            // Construct the request body
            //string requestBody = $"code={code}" +
            //                     $"&redirect_uri={your_redirect_url}" +
            //                     $"&grant_type=authorization_code" +
            //                     $"&client_id={your_client_id}" +
            //                     $"&client_secret={your_client_secret}";

            //using (var client = new HttpClient())
            //{
            //    var content = new StringContent(requestBody, Encoding.UTF8, "application/x-www-form-urlencoded");
            //    var response = client.PostAsync("https://auth.methodlocal.com/connect/token", content).Result;

            //    if (response.IsSuccessStatusCode)
            //    {
            //        var result = response.Content.ReadAsStringAsync().Result;
            //        // Parse the result to get access token, ID token, and refresh token
            //        // Store tokens securely for future API calls
            //        // Redirect or perform additional actions as needed
            //    }
            //    else
            //    {
            //        // Handle error response from Method Identity Server
            //        // Redirect or display an error message
            //    }
            //}

            return View("CallbackView"); // You can create a view for callback handling
        }

        public  static async System.Threading.Tasks.Task<bool> GetAuthTokensUsingRefreshTokenAsync()
        {
            var claims = new List<Claim>();

            earthcoEntities db = new earthcoEntities();
            tblToken TokenData = new tblToken();
            string previousRefreshToken = db.tblTokens.Select(s => s.RefreshToken).FirstOrDefault();
            //var tokenResponse = await auth2Client.RefreshTokenAsync(previousRefreshToken);


            var tokenResponse = auth2Client.RefreshTokenAsync(previousRefreshToken);
            tokenResponse.Wait();
            var data = tokenResponse.Result;

            if(data.AccessToken==null)
            {
                return false;
            }
            TokenData = db.tblTokens.FirstOrDefault();
            if (TokenData == null)
            {
                TokenData = new tblToken();
                TokenData.AccessToken = data.AccessToken;
                TokenData.RefreshToken = data.RefreshToken;
                TokenData.CreatedDate = DateTime.Now;
                TokenData.EditDate = DateTime.Now;
                db.tblTokens.Add(TokenData);
                db.SaveChanges();
            }
            else
            {
                TokenData.AccessToken = data.AccessToken;
                TokenData.RefreshToken = data.RefreshToken;
                TokenData.EditDate = DateTime.Now;
                db.Entry(TokenData);
                db.SaveChanges();
            }
            return true;
        }
        //public async Task<string> InitiateAuth(string submitButton)
        //{

        //    //string RFT = DB.tblTokens.Select(s => s.RefreshToken).FirstOrDefault();

        //    //if (RFT != null && RFT != "")
        //    //{
        //    //    Task<bool> TokenResponse = GetAuthTokensUsingRefreshTokenAsync();
        //    //    bool result = await TokenResponse;
        //    //    if (result == true)
        //    //    {
        //    //        return "Tokens";
        //    //    }

        //    //}


        //    switch (submitButton)
        //    {
        //        case "Connect to QuickBooks":
        //            List<OidcScopes> scopes = new List<OidcScopes>();
        //            scopes.Add(OidcScopes.Accounting);
        //            string authorizeUrl = auth2Client.GetAuthorizationURL(scopes);
        //            return authorizeUrl;
        //        default:
        //            return "Error";
        //    }
        //}

        public async Task<ActionResult> InitiateAuth(string submitButton)
        {

            string RFT = DB.tblTokens.Select(s => s.RefreshToken).FirstOrDefault();

            if (RFT != null && RFT != "")
            {
                Task<bool> TokenResponse = GetAuthTokensUsingRefreshTokenAsync();
                bool result = await TokenResponse;
                if (result == true)
                {
                    return RedirectToAction("ConnectSuccessfull", "Home");
                }

            }


            switch (submitButton)
            {
                case "Connect to QuickBooks":
                    List<OidcScopes> scopes = new List<OidcScopes>();
                    scopes.Add(OidcScopes.Accounting);
                    string authorizeUrl = auth2Client.GetAuthorizationURL(scopes);

                    // Use JavaScript to open the URL in a new window or tab
                    string script = $"window.open('{authorizeUrl}', '_blank');";
                    return Content("<script type='text/javascript'>" + script + "</script>");
                //return Redirect(authorizeUrl);
                default:
                    return (View());
            }
        }

        /// <summary>
        /// QBO API Request
        /// </summary>
        public async Task<ActionResult> ApiCallService()
        {
            //if (Session["realmId"] != null)
            //{

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


            string realmId = TokenData.realmId;
                try
                {




                    var principal = User as ClaimsPrincipal;
                    //OAuth2RequestValidator oauthValidator1 = new OAuth2RequestValidator(principal.FindFirst("access_token").Value);
                    OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator((string)TokenData.AccessToken);
                    ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
                    serviceContext.IppConfiguration.MinorVersion.Qbo = "23";
                    QueryService<Bill> querySvc = new QueryService<Bill>(serviceContext);
                    Bill BillInfo = querySvc.ExecuteIdsQuery("Select * from Bill startposition 1 maxresults 5").FirstOrDefault();
                //OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator("eyJlbmMiOiJBMTI4Q0JDLUhTMjU2IiwiYWxnIjoiZGlyIn0..mZ904VehGCHcCHFNgdNSTg.wwDmKFjYHJ33Wsp-1UBTEVYcv08rrOtpEuoVFT-lz5osH6tInjVg1Ba6QOCcBjvZrXs60aPazHc3_BnyYIHhtZgcVeBfx-2Lvxu6UijcA7Gu7zbzeYcuE3hOm45zA-IywTRWOtgMfMUmsHF2ba4RA48LeRMe90nyTuCamGlRAMm0W2FkDkdnStzmCkkDbzwAp9zPdHi9aMj-MtBkFMHZ-bTBZhKRoM0zIQqeMQduu7Cc-iHBn7NFOxVs2Y4zkSaT9s2uthz8nqI53U61V0MnxkgzMFKjAzscET0zlDFPx9oMDquHLJYVQYkZ1bSsqvdM8soEx4L3JAxfMbCE8PLOH-u008kCF_JRYAyo45wPrn8LzwLGaDkCE3tgs-lTxkXlpsx3IiF9fbBkY_UUWgf8N_PL8Hbz_VN71dJt5r1-4HM6P_7uvHAeWYACF4Wv7kX2MxFk2CBMbkNpxFWtVbu8N73UySNp2f6oxEwlE2N4-GOD4h-i3NTcawwyJvKkqxaaQZ_3bzYtEuwH24dyhYtfHXkxHMkppfOSG_CAsCB9vsmGAErWYweJQ8qVSZ_RYhtmUZP9LYr9L4vFIBsqVmtgt2VJPAmq0tmHSue1djjWIvmfPRx7meeBS6Wf-J2UDV59_KBQ1lwviWK0qlCQoqedtJj3HWQGIfggtJguYZZyIz6NjRO9C080T7u1mWqsjS0QeVDlQ7-mhOcC2BKHtbap42kNB9F0-A-uLEF9vVWAhPEsUNACclLCGxOpxJ-lzWSO.SzaObelYr0P4qdhZ4DUH0A");

                // Create a ServiceContext with Auth tokens and realmId

                // Create a QuickBooks QueryService using ServiceContext
                //QueryService<CompanyInfo> querySvc = new QueryService<CompanyInfo>(serviceContext);
                //QueryService<PurchaseOrder> querySvc = new QueryService<PurchaseOrder>(serviceContext);
                //CompanyInfo companyInfo = querySvc.ExecuteIdsQuery("SELECT * FROM CompanyInfo").FirstOrDefault();
                //PurchaseOrder purchaseorderInfo = querySvc.ExecuteIdsQuery("select * from purchaseorder startposition 1 maxresults 5").FirstOrDefault();
                //Bill BillInfo = new Bill();
                //BillPayment BillInfo1 = new BillPayment();

                //using (var client = new HttpClient())
                //{
                //    client.BaseAddress = new Uri("https://sandbox-quickbooks.api.intuit.com/");

                //    //HTTP POST
                //    var postTask = client.PostAsJsonAsync<PurchaseOrder>("student", student);
                //    postTask.Wait();

                //    var result = postTask.Result;
                //    if (result.IsSuccessStatusCode)
                //    {
                //        return RedirectToAction("Index");
                //    }
                //}

                // API endpoint URL
                string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/4620816365351126570/estimate?minorversion=23";
                //string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/4620816365351126570/purchaseorder?minorversion=23";
                //string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/4620816365351126570/customer?minorversion=23";
                //string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/4620816365351126570/estimate?operation=delete";

                // Request body data
                //var requestData = new
                //{
                //    TaxIdentifier = "99-5688293",
                //    AcctNum = "35372649",
                //    Title = "Ms.",
                //    GivenName = "Test",
                //    FamilyName = "Test",
                //    Suffix = "Sr.",
                //    CompanyName = "Dianne's Auto Shop6",
                //    DisplayName = "Dianne's Auto Shop6",
                //    PrintOnCheckName = "Dianne's Auto Shop1"
                //};


                //Vendor billAddr = new Vendor
                //{
                //    TaxIdentifier = "99-5688293",
                //    AcctNum = "35372649",
                //    Title = "Ms.",
                //    GivenName = "Dianne12",
                //    FamilyName = "Bradley12",
                //    Suffix = "Sr.",
                //    CompanyName = "Dianne's Auto Shop6",
                //    DisplayName = "Dianne's Auto Shop6",
                //    PrintOnCheckName = "Dianne's Auto Shop6"
                //};

                QBEstimateClass EsitimateData = new QBEstimateClass();
                Models.EstimateQB.Line LineData = new Models.EstimateQB.Line();
                EsitimateData.BillEmail = new BillEmail();
                //EsitimateData.BillEmail.Address = "Cool_Cars@intuit.com";
                EsitimateData.TotalAmt = 105;
                //EsitimateData.SyncToken = "2";
                //EsitimateData.Id = "1103";
                EsitimateData.CustomerRef = new EstimateQB.CustomerRef();
                EsitimateData.CustomerRef.value = "3";
                //EsitimateData.CustomerRef.name = "Cool Cars";
                //LineData.Id = "1";
                LineData.Description = "Test";
                LineData.Amount = 105;
                LineData.DetailType = "SalesItemLineDetail";
                LineData.SalesItemLineDetail = new Models.EstimateQB.SalesItemLineDetail();
                LineData.SalesItemLineDetail.ItemRef = new EstimateQB.ItemRef();
                LineData.SalesItemLineDetail.ItemRef.value = "9";
                //LineData.SalesItemLineDetail.ItemRef.name = "Pest Control";
                LineData.SalesItemLineDetail.UnitPrice = 35;
                LineData.SalesItemLineDetail.Qty = 3;
                EsitimateData.Line = new List<Models.EstimateQB.Line>();
                EsitimateData.Line.Add(LineData);

                //QBDeleteClass DeleteData = new QBDeleteClass();
                ////Data = DB.tblEstimates.Where(x => x.EstimateId == SyncLog.Id).FirstOrDefault();
                //DeleteData.Id = "1108";
                //DeleteData.SyncToken = "0";

                QBPurchaseOrderClass PurchaseOrderData = new QBPurchaseOrderClass();
                //Models.QBPurchaseOrderCUClass.LineDetail LineData = new Models.QBPurchaseOrderCUClass.LineDetail();
                //PurchaseOrderData.APAccountRef = new QBPurchaseOrderCUClass.APAccountRef();
                //PurchaseOrderData.APAccountRef.value = "33";
                //PurchaseOrderData.TotalAmt =Convert.ToDecimal(100);
                ////PurchaseOrderData.TxnDate = DateTime.Now;
                //PurchaseOrderData.SyncToken = "0";
                //PurchaseOrderData.Id = "1110";
                //PurchaseOrderData.VendorRef = new QBPurchaseOrderCUClass.VendorRef();
                //PurchaseOrderData.VendorRef.value = "41";
                //PurchaseOrderData.POStatus = "Open";
                //LineData.Amount = 100;
                //LineData.DetailType = "ItemBasedExpenseLineDetail";
                //LineData.ItemBasedExpenseLineDetail = new Models.QBPurchaseOrderCUClass.ItemBasedExpenseLineDetail();
                //LineData.ItemBasedExpenseLineDetail.ItemRef = new QBPurchaseOrderCUClass.ItemRef();
                //LineData.ItemBasedExpenseLineDetail.ItemRef.value = "5";
                ////LineData.SalesItemLineDetail.ItemRef.name = "Pest Control";
                //LineData.ItemBasedExpenseLineDetail.UnitPrice = 50;
                //LineData.ItemBasedExpenseLineDetail.Qty = 2;
                //PurchaseOrderData.Line = new List<Models.QBPurchaseOrderCUClass.LineDetail>();
                //PurchaseOrderData.Line.Add(LineData);


                QBInvoiceClass InvoiceData = new QBInvoiceClass();
                //Models.QBInvoiceCUClass.LineDetail LineData = new Models.QBInvoiceCUClass.LineDetail();
                //InvoiceData.CustomerMemo = new QBInvoiceCUClass.CustomerMemo();
                //InvoiceData.CustomerMemo.value = "Thank you for your business and have a great day!";
                ////InvoiceData.TotalAmt = 150;
                //InvoiceData.Balance = 50;
                //InvoiceData.DocNumber = "9874";
                //InvoiceData.DueDate = DateTime.Now.AddDays(20);

                //InvoiceData.SyncToken = "1";
                //InvoiceData.Id = "1115";
                //InvoiceData.CustomerRef = new QBInvoiceCUClass.CustomerRef();
                //InvoiceData.CustomerRef.value = "1";

                //LineData.Amount = 275;
                //LineData.Description = "Rock Fountain";
                //LineData.DetailType = "SalesItemLineDetail";
                //LineData.SalesItemLineDetail = new Models.QBInvoiceCUClass.SalesItemLineDetail();
                //LineData.SalesItemLineDetail.ItemRef = new QBInvoiceCUClass.ItemRef();
                //LineData.SalesItemLineDetail.ItemRef.value = "1";
                //LineData.SalesItemLineDetail.UnitPrice = 275;
                //LineData.SalesItemLineDetail.Qty = 1;
                //InvoiceData.Line = new List<Models.QBInvoiceCUClass.LineDetail>();
                //InvoiceData.Line.Add(LineData);


                QBBillClass BillData = new QBBillClass();
                //Models.QBBillCUClass.LineDetail LineData = new Models.QBBillCUClass.LineDetail();
                //BillData.TotalAmt = (decimal)103.55;
                //BillData.DocNumber = "9876";
                //BillData.DueDate = DateTime.Now.AddDays(10);
                //BillData.SyncToken = "0";
                //BillData.Id = "1119";
                //BillData.VendorRef = new QBBillCUClass.VendorRef();
                //BillData.VendorRef.value = "56";
                //LineData.Amount = Convert.ToDecimal(125);
                //LineData.Description = "Lumber";
                //LineData.DetailType = "ItemBasedExpenseLineDetail";
                //LineData.ItemBasedExpenseLineDetail = new Models.QBBillCUClass.ItemBasedExpenseLineDetail();
                //LineData.ItemBasedExpenseLineDetail.ItemRef = new QBBillCUClass.ItemRef();
                //LineData.ItemBasedExpenseLineDetail.ItemRef.value = "7";
                //LineData.ItemBasedExpenseLineDetail.UnitPrice = Convert.ToDecimal(125);
                //LineData.ItemBasedExpenseLineDetail.Qty = (int)1;
                //BillData.Line = new List<Models.QBBillCUClass.LineDetail>();
                //BillData.Line.Add(LineData);


                QBItemClass ItemData = new QBItemClass();
                //ItemData.Type = "NonInventory";
                //ItemData.Name = "Garden Supplies3";
                //ItemData.UnitPrice = (decimal)150;
                //ItemData.PurchaseCost = (decimal)110;

                //ItemData.SyncToken = "0";
                //ItemData.Id = "66";
                //ItemData.IncomeAccountRef = new IncomeAccountRef();
                //ItemData.IncomeAccountRef.value = "79";
                //ItemData.ExpenseAccountRef = new ExpenseAccountRef();
                //ItemData.ExpenseAccountRef.value = "80";

                QBStaffClass StaffData = new QBStaffClass();
                //StaffData.GivenName = "John";
                //StaffData.FamilyName = "Meuller";
                //StaffData.PrimaryPhone = new QBStaffCUClass.PrimaryPhone();
                //StaffData.PrimaryPhone.FreeFormNumber = "408-525-1234";
                //StaffData.Title = "Manager2";
                //StaffData.SyncToken = "0";
                //StaffData.Id = "252";

                QBCustomerClass CustomerData = new QBCustomerClass();
                //CustomerData.GivenName = "Bill1";
                //CustomerData.FamilyName = "Lucchini";
                //CustomerData.CompanyName = "Bill's Windsurf Shop";
                //CustomerData.PrimaryPhone = new QBCustomerCUClass.PrimaryPhone();
                //CustomerData.PrimaryPhone.FreeFormNumber = "(555) 555-5555";
                //CustomerData.PrimaryEmailAddr = new QBCustomerCUClass.PrimaryEmailAddr();
                //CustomerData.PrimaryEmailAddr.Address = "jdrew@myemail.com";
                //CustomerData.SyncToken = "0";
                //CustomerData.Id = "254";

                // Convert the request body to JSON
                //string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(EsitimateData);
                //string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(PurchaseOrderData);
                string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(EsitimateData);
                //string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(DeleteData);
                // Create HttpClient
                using (HttpClient client = new HttpClient())
                    {
                        // Set headers
                        //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                        //client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJlbmMiOiJBMTI4Q0JDLUhTMjU2IiwiYWxnIjoiZGlyIn0..BpQ1lQxl8z4LjJ6oU40OHQ.BqrX891OtD-9dPReTlj4SsOWwF2d7YC_PxWcpV6XGXmKavq9oJrDaMVqfdbbdZJA7UlhZ-bmxRrNIjiV3MJZ_LVRt3HJI83zswBI9EFwOBF-rKFSbeHRKhgT7IJGqZiUZTY7LfuChVwKllXqVUihQSDqsd5aAsRTmsGgFCNyvYPXHZLMWkA-RXt42rfsGTWCYRUJuCnlC0SjSjdPEuwm9chat0NbC7TBhaZ0ZBS71XxKx9QqT3gg6PI1ldNRbXAIHvNEfW3A_vsXdttPcd7nPts8xa6Yp7i9a2FYDmSWrdn5cn0dWe-0ebKRoBxRUt98jUv4qF5P0zHgZeU-zJ_Qc8AK0aLr2k6ngoMkEwh-jgJNs3whIhmgaawlK3cW5_yM2E7Q3EUSXqh1x3MSy3m37uq_iUyTDSDbpeqRvQoGwoPX8fG_2G7BkUocr6WGKuoTgVEC6VHi8hSVjH9Ha6QNdalfGsfGp6uQaiyB1juzg47-wXdz2jnxJNsgJJg0c6wFMoBg18k6szJHv_Ir4R3mDPOE1wsl-v8rQpBTXE5Ei-kOdeMACsnZQuVj_WPV6orsd9dlMlAjDnNK9qyGyAJSOQ_K-jdJc37CWcliTs6PmqIYw-NdgSwtsjbv-ahxt7LU2vfYURJ49I-1QoE395EnWqy7Qv8zQARgMpA2O6RxtgFXmK-QFyLXonFOomTwJmFT1R3AAPtnFxLHtJGMs2bBuVkJkk5AVcKp8QVCYAUP7LYLfBQOXpdt3i8ib2uTB9v_.M6GZiSm2HcYA2IVQLaFniQ");
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer "+ oauthValidator.AccessToken);
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
                        //var estimateId = estimateModel["Estimate"]["Id"];
                        //var SyncToken = estimateModel["Estimate"]["SyncToken"];
                        //var estimateId = estimateModel["PurchaseOrder"]["Id"];
                        //var SyncToken = estimateModel["PurchaseOrder"]["SyncToken"];
                        var estimateId = estimateModel["Estimate"]["Id"];
                        var SyncToken = estimateModel["Estimate"]["SyncToken"];
                        // Process jsonResponse
                        return View();
                        }
                        else
                        {
                            // Handle error
                            string errorMessage = await response.Content.ReadAsStringAsync();
                        var errorResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(errorMessage);

                        // Handle error message
                        //return View("Error");
                    }
                }


                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    // Make the GET request
                    HttpResponseMessage response = await client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/customer/" + 254 + "?minorversion=23");
                    //HttpResponseMessage response = await client.GetAsync("http://103.73.231.56/clickcut/api/Login/recoverpassword?Email=abdulqadeerkhan070@gmail.com");
                    //var responseTask = await client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/Estimate/" + 1108 + "?minorversion=23");
                    //var visibilityresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/4620816365351126570/vendor?minorversion=23", content);
                    //responseTask.Wait();

                    //var result = responseTask.Result;
                    if (response.IsSuccessStatusCode)
                    {
                        //var readTask = result.Content.ReadAsAsync<Bill>();
                        //readTask.Wait();
                        var readTask = response.Content.ReadAsStringAsync();
                        readTask.Wait();
                        string Test = readTask.Result;
                        QBCustomer.CustomerResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBCustomer.CustomerResponse>(Test);
                        //BillInfo = JsonSerializer.Deserialize<Bill>(readTask.Result);


                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                    }
                }




                string baseUrl = "https://sandbox-quickbooks.api.intuit.com";

                // Endpoint
                //string endpoint = "/v3/company/<realmID>/upload"; // Replace <realmID> with the actual realm ID

                // Complete URL
                //apiUrl = baseUrl + endpoint;

                // Request body
                var requestBody = new
                {
                    AttachableRef = new[]
                    {
                                new
                                {
                                    EntityRef = new
                                    {
                                        type = "Estimate",
                                        value = "1108"
                                    }
                                }
                            },
                    //ContentType = "image/jpg",
                    //FileName = "receipt_nov15.jpg"
                };

                // Serialize the request body to JSON
                string jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

                using (var httpClient = new HttpClient())
                using (var content = new MultipartFormDataContent())
                {
                    // Add AttachableRef as a JSON string
                    //var attachableRefJson = "{\"AttachableRef\":[{\"EntityRef\":{\"type\":\"Invoice\",\"value\":\"95\"}}],\"ContentType\":\"image/jpg\",\"FileName\":\"receipt_nov15.jpg\"}";
                    content.Add(new StringContent(jsonRequestBody), "application/json");

                    // Load the file from a URL
                    var fileUrl = "https://earthcoapi.yehtohoga.com/Uploading/Estimate/Estimate24108122023110150.png"; // Replace with the actual file URL
                    //var fileUrl = src ={`https://earthcoapi.yehtohoga.com/${file.FilePath}`};
                        var fileBytes = await httpClient.GetByteArrayAsync(fileUrl);
                    var fileContent = new ByteArrayContent(fileBytes);
                    content.Add(fileContent, "file", "receipt_nov15.jpg");

                    // Make the POST request
                    apiUrl = $"/v3/company/" + TokenData.realmId + "/upload";
                    var requestUrl = $"{baseUrl}{apiUrl}";

                    var response = await httpClient.PostAsync(requestUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Handle success
                        Console.WriteLine("File uploaded successfully!");
                    }
                    else
                    {
                        // Handle failure
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");

                        string errorMessage = await response.Content.ReadAsStringAsync();
                    }
                }


                apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/4620816365351126570/upload";
                //apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/4620816365351126570/estimate?minorversion=23";

                using (HttpClient client = new HttpClient())
                using (MultipartFormDataContent formData = new MultipartFormDataContent())
                {
                    var httpClient = new HttpClient();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + oauthValidator.AccessToken);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    // Convert JSON metadata to StringContent
                    string jsonMetadata = "{\"AttachableRef\":[{\"EntityRef\":{\"type\":\"Estimate\",\"value\":\"1107\"}},\"FileName\":\"receipt_nov16.jpg\",\"ContentType\":\"image/jpg\"}]";
                    StringContent metadataContent = new StringContent(jsonMetadata, Encoding.UTF8, "application/json");

                    // Add file metadata to the form data
                    formData.Add(metadataContent, "file_metadata_0");

                    // Convert base64-encoded file content to ByteArrayContent
                    //string base64FileContent = "/9j/4AAQSkZJRgABAQEAlgCWAAD/4ge4SUNDX1BST0ZJTEUAAQEAAAeoYXBwbAIgAABtbnRyUkdC"+
                    //                            "IFhZWiAH2QACABkACwAaAAthY3NwQVBQTAAAAABhcHBsAAAAAAAAAAAAAAAAAAAAAAAA9tYAAQAA"+
                    //                            "AADTLWFwcGwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAtk"+
                    //                            "ZXNjAAABCAAAAG9kc2NtAAABeAAABWxjcHJ0AAAG5AAAADh3dHB0AAAHHAAAABRyWFlaAAAHMAAA"+
                    //                            "ABRnWFlaAAAHRAAAABRiWFlaAAAHWAAAABRyVFJDAAAHbAAAAA5jaGFkAAAHfAAAACxiVFJDAAAH"+
                    //                            "bAAAAA5nVFJDAAAHbAAAAA5kZXNjAAAAAAAAABRHZW5lcmljIFJHQiBQcm9maWxlAAAAAAAAAAAA"+
                    //                            "AAAUR2VuZXJpYyBSR0IgUHJvZmlsZQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"+
                    //                            "AAAAAAAAAAAAAAAAAAAAbWx1YwAAAAAAAAAeAAAADHNrU0sAAAAoAAABeGhySFIAAAAoAAABoGNh"+
                    //                            "RVMAAAAkAAAByHB0QlIAAAAmAAAB7HVrVUEAAAAqAAACEmZyRlUAAAAoAAACPHpoVFcAAAAWAAAC"+
                    //                            "ZGl0SVQAAAAoAAACem5iTk8AAAAmAAAComtvS1IAAAAWAAACyGNzQ1oAAAAiAAAC3mhlSUwAAAAe"+
                    //                            "AAADAGRlREUAAAAsAAADHmh1SFUAAAAoAAADSnN2U0UAAAAmAAAConpoQ04AAAAWAAADcmphSlAA"+
                    //                            "AAAaAAADiHJvUk8AAAAkAAADomVsR1IAAAAiAAADxnB0UE8AAAAmAAAD6G5sTkwAAAAoAAAEDmVz"+
                    //                            "RVMAAAAmAAAD6HRoVEgAAAAkAAAENnRyVFIAAAAiAAAEWmZpRkkAAAAoAAAEfHBsUEwAAAAsAAAE"+
                    //                            "pHJ1UlUAAAAiAAAE0GFyRUcAAAAmAAAE8mVuVVMAAAAmAAAFGGRhREsAAAAuAAAFPgBWAWEAZQBv"+
                    //                            "AGIAZQBjAG4A/QAgAFIARwBCACAAcAByAG8AZgBpAGwARwBlAG4AZQByAGkBDQBrAGkAIABSAEcA"+
                    //                            "QgAgAHAAcgBvAGYAaQBsAFAAZQByAGYAaQBsACAAUgBHAEIAIABnAGUAbgDoAHIAaQBjAFAAZQBy"+
                    //                            "AGYAaQBsACAAUgBHAEIAIABHAGUAbgDpAHIAaQBjAG8EFwQwBDMEMAQ7BEwEPQQ4BDkAIAQ/BEAE"+
                    //                            "PgREBDAEOQQ7ACAAUgBHAEIAUAByAG8AZgBpAGwAIABnAOkAbgDpAHIAaQBxAHUAZQAgAFIAVgBC"+
                    //                            "kBp1KAAgAFIARwBCACCCcl9pY8+P8ABQAHIAbwBmAGkAbABvACAAUgBHAEIAIABnAGUAbgBlAHIA"+
                    //                            "aQBjAG8ARwBlAG4AZQByAGkAcwBrACAAUgBHAEIALQBwAHIAbwBmAGkAbMd8vBgAIABSAEcAQgAg"+
                    //                            "1QS4XNMMx3wATwBiAGUAYwBuAP0AIABSAEcAQgAgAHAAcgBvAGYAaQBsBeQF6AXVBeQF2QXcACAA"+
                    //                            "UgBHAEIAIAXbBdwF3AXZAEEAbABsAGcAZQBtAGUAaQBuAGUAcwAgAFIARwBCAC0AUAByAG8AZgBp"+
                    //                            "AGwAwQBsAHQAYQBsAOEAbgBvAHMAIABSAEcAQgAgAHAAcgBvAGYAaQBsZm6QGgAgAFIARwBCACBj"+
                    //                            "z4/wZYdO9k4AgiwAIABSAEcAQgAgMNcw7TDVMKEwpDDrAFAAcgBvAGYAaQBsACAAUgBHAEIAIABn"+
                    //                            "AGUAbgBlAHIAaQBjA5MDtQO9A7kDugPMACADwAPBA78DxgOvA7sAIABSAEcAQgBQAGUAcgBmAGkA"+
                    //                            "bAAgAFIARwBCACAAZwBlAG4A6QByAGkAYwBvAEEAbABnAGUAbQBlAGUAbgAgAFIARwBCAC0AcABy"+
                    //                            "AG8AZgBpAGUAbA5CDhsOIw5EDh8OJQ5MACAAUgBHAEIAIA4XDjEOSA4nDkQOGwBHAGUAbgBlAGwA"+
                    //                            "IABSAEcAQgAgAFAAcgBvAGYAaQBsAGkAWQBsAGUAaQBuAGUAbgAgAFIARwBCAC0AcAByAG8AZgBp"+
                    //                            "AGkAbABpAFUAbgBpAHcAZQByAHMAYQBsAG4AeQAgAHAAcgBvAGYAaQBsACAAUgBHAEIEHgQxBEkE"+
                    //                            "OAQ5ACAEPwRABD4ERAQ4BDsETAAgAFIARwBCBkUGRAZBACAGKgY5BjEGSgZBACAAUgBHAEIAIAYn"+
                    //                            "BkQGOQYnBkUARwBlAG4AZQByAGkAYwAgAFIARwBCACAAUAByAG8AZgBpAGwAZQBHAGUAbgBlAHIA"+
                    //                            "ZQBsACAAUgBHAEIALQBiAGUAcwBrAHIAaQB2AGUAbABzAGV0ZXh0AAAAAENvcHlyaWdodCAyMDA3"+
                    //                            "IEFwcGxlIEluYy4sIGFsbCByaWdodHMgcmVzZXJ2ZWQuAFhZWiAAAAAAAADzUgABAAAAARbPWFla"+
                    //                            "IAAAAAAAAHRNAAA97gAAA9BYWVogAAAAAAAAWnUAAKxzAAAXNFhZWiAAAAAAAAAoGgAAFZ8AALg2"+
                    //                            "Y3VydgAAAAAAAAABAc0AAHNmMzIAAAAAAAEMQgAABd7///MmAAAHkgAA/ZH///ui///9owAAA9wA"+
                    //                            "AMBs/+EAgEV4aWYAAE1NACoAAAAIAAUBEgADAAAAAQABAAABGgAFAAAAAQAAAEoBGwAFAAAAAQAA"+
                    //                            "AFIBKAADAAAAAQACAACHaQAEAAAAAQAAAFoAAAAAAAA6mQAAAGQAADqZAAAAZAACoAIABAAAAAEA"+
                    //                            "AAAuoAMABAAAAAEAAAAUAAAAAP/bAEMAAgEBAgEBAgIBAgICAgIDBQMDAwMDBgQEAwUHBgcHBwYG"+
                    //                            "BgcICwkHCAoIBgYJDQkKCwsMDAwHCQ0ODQwOCwwMC//bAEMBAgICAwIDBQMDBQsIBggLCwsLCwsL"+
                    //                            "CwsLCwsLCwsLCwsLCwsLCwsLCwsLCwsLCwsLCwsLCwsLCwsLCwsLCwsLC//AABEIABQALgMBIgAC"+
                    //                            "EQEDEQH/xAAfAAABBQEBAQEBAQAAAAAAAAAAAQIDBAUGBwgJCgv/xAC1EAACAQMDAgQDBQUEBAAA"+
                    //                            "AX0BAgMABBEFEiExQQYTUWEHInEUMoGRoQgjQrHBFVLR8CQzYnKCCQoWFxgZGiUmJygpKjQ1Njc4"+
                    //                            "OTpDREVGR0hJSlNUVVZXWFlaY2RlZmdoaWpzdHV2d3h5eoOEhYaHiImKkpOUlZaXmJmaoqOkpaan"+
                    //                            "qKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4eLj5OXm5+jp6vHy8/T19vf4+fr/xAAfAQAD"+
                    //                            "AQEBAQEBAQEBAAAAAAAAAQIDBAUGBwgJCgv/xAC1EQACAQIEBAMEBwUEBAABAncAAQIDEQQFITEG"+
                    //                            "EkFRB2FxEyIygQgUQpGhscEJIzNS8BVictEKFiQ04SXxFxgZGiYnKCkqNTY3ODk6Q0RFRkdISUpT"+
                    //                            "VFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqCg4SFhoeIiYqSk5SVlpeYmZqio6Slpqeoqaqys7S1tre4"+
                    //                            "ubrCw8TFxsfIycrS09TV1tfY2dri4+Tl5ufo6ery8/T19vf4+fr/2gAMAwEAAhEDEQA/APkb41eO"+
                    //                            "NYtfi94oSDVdRVF1a74FzJgfvn96j+Kmj+O/gj/wj3/C159Q0MeK9Et/EekmfU1IvdPuN/kzrtkO"+
                    //                            "0N5b/K2GGOQKzvjnz8YPFX/YVvP/AEc9fePiv9qXwn4K+Eut674A8S+Br3xl4e/Zd8MaZ4dF0LPU"+
                    //                            "HtvENte3TtbwwThla8h3RuYipI+UspFfueIryw6p8kOa+n5Ja62310Z+V4XDRxU5RlPl8z4O0S/8"+
                    //                            "YeJfB+t+IfD1xrN7oPhpbZtV1GG7ZrXTxczeRb+bJvwDLLlEAyWYHA4OK/hTxP4i8ceKdI0Xwrq9"+
                    //                            "1eanr13FY6fANS2fappZFjRVdnCgF2UFiQozkkDmv0r+KX7VPhO/8JftJaX8I/HPwu0qXxf4c8C+"+
                    //                            "JZIA1hDb63cpldfjtwI2WS7NvFGnkINyyMpXY7Fqq/GDUvhB4d+IXxP17TPHvwc1nTfiB8a/h/4o"+
                    //                            "0O10zVbW4lstChubNbxpoto+zxARztLHyAgZnADc8NPN5ydpUbXtbfqob6dOZ9vha8zu/smElpXX"+
                    //                            "W+3eW2vku+5+fXjvw38RPhlZm58eRa7ptp/a97oCXT3m+3nv7Jtt3BDKkhWUwtwzISgPG4mux/ZD"+
                    //                            "8Y6tefEO++06nqL7dOkxm6k4/exf7VfRX/BRD9oTTfiz+xBaaD8NPGfgK803wz8XvFf2nRLOWzjv"+
                    //                            "30+XU5pNKntIUQSPbeU+8yxkKUK7i+AB8x/sc/8AJQ7/AP7Bz/8Ao2KuqlWnicLOdSKTu1b0em/d"+
                    //                            "anFVpRw+IjCnK60d/X0O/wDil+zfoWofE7xJNcXOqb31W7JxJHj/AF7/APTOsH/hmPw//wA/Gqf9"+
                    //                            "/I//AI3RRXRTk+VamM4rmegv/DMfh/H/AB8ap/38j/8AjdB/Zj8Pn/l41Trn/WR9f+/dFFVzPuRZ"+
                    //                            "XEP7Mfh89bjVOP8AppFx/wCQ69I/Zf8A2d9F0z4hXP2S61Qb9Olzl4z0lh/6Z+9FFY4mT9lLU1oJ"+
                    //                            "c6P /2Q==";

                    var fileUrl = "https://earthcoapi.yehtohoga.com/Uploading/Estimate/Estimate122122023150737.jpg";
                    var fileBytes = await httpClient.GetByteArrayAsync(fileUrl);
                    //string base64String = Convert.ToBase64String(fileBytes);
                    var fileContent = new ByteArrayContent(fileBytes);
                    //byte[] fileBytes = Convert.FromBase64String(base64FileContent);
                    //ByteArrayContent fileContent = new ByteArrayContent(fileBytes);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                    // Add file content to the form data
                    formData.Add(fileContent, "file_content_0", "398535758.jpg");

                    // Send the POST request
                    HttpResponseMessage response = await client.PostAsync(apiUrl, formData);

                    if(response.IsSuccessStatusCode)
                    {
                        // Handle success
                        Console.WriteLine("File uploaded successfully!");

                        var jsonResponse = response.Content.ReadAsStringAsync();
                        jsonResponse.Wait();
                        var Result = jsonResponse.Result;
                    }
                    else
                    {
                        // Handle failure
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");

                        string errorMessage = await response.Content.ReadAsStringAsync();
                    }
                    // Read and return the response content
                    //return await response.Content.ReadAsStringAsync();
                }



                //using (var client = new HttpClient())
                //{
                //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthValidator.AccessToken);
                //    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //    Vendor billAddr = new Vendor
                //    {
                //        TaxIdentifier = "99-5688293",
                //        AcctNum = "35372649",
                //        Title = "Ms.",
                //        GivenName = "Dianne12",
                //        FamilyName = "Bradley12",
                //        Suffix = "Sr.",
                //        CompanyName = "Dianne's Auto Shop9",
                //        DisplayName = "Dianne's Auto Shop9",
                //        PrintOnCheckName = "Dianne's Auto Shop9"
                //    };
                //    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthValidator.AccessToken);
                //    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //    string message;
                //    var json = Newtonsoft.Json.JsonConvert.SerializeObject(billAddr);
                //    var content = new StringContent(json, Encoding.UTF8, "application/json");
                //    var visibilityresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/4620816365351126570/vendor?minorversion=23", content);
                //    if (visibilityresponse.IsSuccessStatusCode)
                //    {
                //        var responseContent = await visibilityresponse.Content.ReadAsStringAsync();
                //        message = Convert.ToString(responseContent);
                //    }


                //    ////client.BaseAddress = new Uri("https://sandbox-quickbooks.api.intuit.com/v3/company/4620816365351126570/purchaseorder/178?minorversion=23");
                //    ////HTTP GET
                //    var responseTask = client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/4620816365351126570/bill/1?minorversion=23");
                //    responseTask.Wait();

                //    var result = responseTask.Result;
                //    if (result.IsSuccessStatusCode)
                //    {
                //        //var readTask = result.Content.ReadAsAsync<Bill>();
                //        //readTask.Wait();
                //        var readTask = result.Content.ReadAsStringAsync();
                //        readTask.Wait();
                //        string Test = readTask.Result;
                //        BillInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<Bill>(Test);
                //        //BillInfo = JsonSerializer.Deserialize<Bill>(readTask.Result);


                //    }
                //    //else //web api sent error response 
                //    //{
                //    //    //log response status here..

                //    //    //purchaseorderInfo = Enumerable.Empty<PurchaseOrder>();

                //    //    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                //    //}
                //}


                string output = "Vendor Name: " + BillInfo.VendorRef.name+ " purchase order Number: " + BillInfo.VendorRef.name + ", " + BillInfo.VendorRef.name + ", " + BillInfo.VendorRef.name + " " + BillInfo.VendorRef.name;
                    //string output = "";
                    return View("ApiCallService", (object)("QBO API call Successful!! Response: " + output));
                }
                catch (Exception ex)
                {
                    return View("ApiCallService", (object)("QBO API call Failed!" + " Error message: " + ex.Message));
                }
            //}
            //else
            //    return View("ApiCallService", (object)"QBO API call Failed!");
        }



        public async Task<ActionResult> GetItemsFromQB(string Type)
        {
            //if (Session["realmId"] != null)
            //{

            tblToken TokenData = DB.tblTokens.FirstOrDefault();


            string realmId = TokenData.realmId;
            try
            {
                var principal = User as ClaimsPrincipal;
                //OAuth2RequestValidator oauthValidator1 = new OAuth2RequestValidator(principal.FindFirst("access_token").Value);
                OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator((string)TokenData.AccessToken);
                ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
                serviceContext.IppConfiguration.MinorVersion.Qbo = "23";

                if(Type.Contains("Item"))
                {
                    QueryService<Item> querySvc = new QueryService<Item>(serviceContext);
                    List<Item> ItemInfo = querySvc.ExecuteIdsQuery("select * from item").ToList();

                    tblItem ItemData = null;

                    foreach (Item item in ItemInfo)
                    {
                        int ID = Convert.ToInt32(item.Id);
                        ItemData = DB.tblItems.Where(x => x.QBId == ID).FirstOrDefault();
                        if(ItemData==null)
                        {
                            ItemData = new tblItem();
                            ItemData.QBId = Convert.ToInt32(item.Id);
                            ItemData.SyncToken = item.SyncToken;
                            ItemData.ItemName = item.Name;
                            ItemData.SaleDescription = item.Description;
                            ItemData.PurchaseDescription = item.Description;
                            ItemData.Type = item.Type.ToString();
                            ItemData.PurchasePrice = Convert.ToDouble(item.PurchaseCost);
                            ItemData.SalePrice = Convert.ToDouble(item.UnitPrice);
                            ItemData.isActive = true;
                            ItemData.isDelete = false;
                            ItemData.CreatedDate = DateTime.Now;
                            ItemData.EditDate = DateTime.Now;
                            DB.tblItems.Add(ItemData);
                        }
                        else
                        {
                            ItemData = new tblItem();
                            ItemData.QBId = Convert.ToInt32(item.Id);
                            ItemData.SyncToken = item.SyncToken;
                            ItemData.ItemName = item.Name;
                            ItemData.SaleDescription = item.Description;
                            ItemData.PurchaseDescription = item.Description;
                            ItemData.Type = item.Type.ToString();
                            ItemData.PurchasePrice = Convert.ToDouble(item.PurchaseCost);
                            ItemData.SalePrice = Convert.ToDouble(item.UnitPrice);
                            ItemData.isActive = true;
                            ItemData.isDelete = false;
                            ItemData.CreatedDate = DateTime.Now;
                            ItemData.EditDate = DateTime.Now;
                            DB.Entry(ItemData);
                        }
                        
                    }
                    DB.SaveChanges();
                }
                

                if(Type.Contains("Customer"))
                {
                    QueryService<Customer> CustomerquerySvc = new QueryService<Customer>(serviceContext);
                    List<Customer> CustomerInfo = CustomerquerySvc.ExecuteIdsQuery("select * from Customer").ToList();

                    tblUser CustomerData = null;

                    foreach (Customer item in CustomerInfo)
                    {
                        int ID = Convert.ToInt32(item.Id);
                        CustomerData = DB.tblUsers.Where(x => x.QBId == ID && x.UserTypeId==2).FirstOrDefault();
                        if(CustomerData==null)
                        {
                            CustomerData = new tblUser();
                            CustomerData.QBId = Convert.ToInt32(item.Id);
                            CustomerData.SyncToken = item.SyncToken;
                            CustomerData.FirstName = item.GivenName != null ? item.GivenName : "";
                            CustomerData.LastName = item.FamilyName != null ? item.FamilyName : "";
                            CustomerData.CompanyName = item.CompanyName;
                            CustomerData.Email = item.PrimaryEmailAddr != null ? item.PrimaryEmailAddr.Address : "";
                            CustomerData.Phone = item.PrimaryPhone != null ? item.PrimaryPhone.FreeFormNumber : "";
                            CustomerData.RoleId = 2;
                            CustomerData.UserTypeId = 2;
                            CustomerData.isLoginAllow = false;
                            CustomerData.isActive = true;
                            CustomerData.isDelete = false;
                            CustomerData.CreatedDate = DateTime.Now;
                            CustomerData.EditDate = DateTime.Now;
                            DB.tblUsers.Add(CustomerData);
                        }
                        else
                        {
                            CustomerData = new tblUser();
                            CustomerData.QBId = Convert.ToInt32(item.Id);
                            CustomerData.SyncToken = item.SyncToken;
                            CustomerData.FirstName = item.GivenName != null ? item.GivenName : "";
                            CustomerData.LastName = item.FamilyName != null ? item.FamilyName : "";
                            CustomerData.CompanyName = item.CompanyName;
                            CustomerData.Email = item.PrimaryEmailAddr != null ? item.PrimaryEmailAddr.Address : "";
                            CustomerData.Phone = item.PrimaryPhone != null ? item.PrimaryPhone.FreeFormNumber : "";
                            CustomerData.RoleId = 2;
                            CustomerData.UserTypeId = 2;
                            CustomerData.isLoginAllow = false;
                            CustomerData.isActive = true;
                            CustomerData.isDelete = false;
                            CustomerData.CreatedDate = DateTime.Now;
                            CustomerData.EditDate = DateTime.Now;
                            DB.Entry(CustomerData);
                        }
                        
                    }
                    DB.SaveChanges();
                }
                

                if(Type.Contains("Vendor"))
                {
                    QueryService<Vendor> VendorquerySvc = new QueryService<Vendor>(serviceContext);
                    List<Vendor> VendorInfo = VendorquerySvc.ExecuteIdsQuery("select * from Vendor").ToList();

                    tblUser VendorData = null;

                    foreach (Vendor item in VendorInfo)
                    {
                        int ID = Convert.ToInt32(item.Id);
                        VendorData = DB.tblUsers.Where(x => x.QBId == ID && x.UserTypeId == 3).FirstOrDefault();
                        if(VendorData==null)
                        {
                            VendorData = new tblUser();
                            VendorData.QBId = Convert.ToInt32(item.Id);
                            VendorData.SyncToken = item.SyncToken;
                            VendorData.FirstName = item.GivenName != null ? item.GivenName : "";
                            VendorData.LastName = item.FamilyName != null ? item.FamilyName : "";
                            VendorData.CompanyName = item.CompanyName;
                            VendorData.Email = item.PrimaryEmailAddr != null ? item.PrimaryEmailAddr.Address : "";
                            VendorData.Phone = item.PrimaryPhone != null ? item.PrimaryPhone.FreeFormNumber : "";
                            VendorData.UserTypeId = 3;
                            VendorData.isLoginAllow = false;
                            VendorData.isActive = true;
                            VendorData.isDelete = false;
                            VendorData.CreatedDate = DateTime.Now;
                            VendorData.EditDate = DateTime.Now;
                            DB.tblUsers.Add(VendorData);
                        }
                        else
                        {
                            VendorData = new tblUser();
                            VendorData.QBId = Convert.ToInt32(item.Id);
                            VendorData.SyncToken = item.SyncToken;
                            VendorData.FirstName = item.GivenName != null ? item.GivenName : "";
                            VendorData.LastName = item.FamilyName != null ? item.FamilyName : "";
                            VendorData.CompanyName = item.CompanyName;
                            VendorData.Email = item.PrimaryEmailAddr != null ? item.PrimaryEmailAddr.Address : "";
                            VendorData.Phone = item.PrimaryPhone != null ? item.PrimaryPhone.FreeFormNumber : "";
                            VendorData.UserTypeId = 3;
                            VendorData.isLoginAllow = false;
                            VendorData.isActive = true;
                            VendorData.isDelete = false;
                            VendorData.CreatedDate = DateTime.Now;
                            VendorData.EditDate = DateTime.Now;
                            DB.Entry(VendorData);
                        }
                       
                    }
                    DB.SaveChanges();
                }

                if (Type.Contains("Employee"))
                {
                    QueryService<Employee> EmployeequerySvc = new QueryService<Employee>(serviceContext);
                    List<Employee> EmployeeInfo = EmployeequerySvc.ExecuteIdsQuery("select * from Employee").ToList();

                    tblUser EmployeeData = null;

                    foreach (Employee item in EmployeeInfo)
                    {
                        int ID = Convert.ToInt32(item.Id);
                        EmployeeData = DB.tblUsers.Where(x => x.QBId == ID && x.UserTypeId == 1).FirstOrDefault();
                        if(EmployeeData==null)
                        {
                            EmployeeData = new tblUser();
                            EmployeeData.QBId = Convert.ToInt32(item.Id);
                            EmployeeData.SyncToken = item.SyncToken;
                            EmployeeData.FirstName = item.GivenName != null ? item.GivenName : "";
                            EmployeeData.LastName = item.FamilyName != null ? item.FamilyName : "";
                            EmployeeData.Email = item.PrimaryEmailAddr != null ? item.PrimaryEmailAddr.Address : "";
                            EmployeeData.Phone = item.PrimaryPhone != null ? item.PrimaryPhone.FreeFormNumber : "";
                            EmployeeData.UserTypeId = 1;
                            EmployeeData.RoleId = 4;
                            EmployeeData.CreatedDate = DateTime.Now;
                            EmployeeData.isLoginAllow = false;
                            EmployeeData.isActive = true;
                            EmployeeData.isDelete = false;
                            EmployeeData.CreatedDate = DateTime.Now;
                            EmployeeData.EditDate = DateTime.Now;
                            DB.tblUsers.Add(EmployeeData);
                        }
                        else
                        {
                            EmployeeData = new tblUser();
                            EmployeeData.QBId = Convert.ToInt32(item.Id);
                            EmployeeData.SyncToken = item.SyncToken;
                            EmployeeData.FirstName = item.GivenName != null ? item.GivenName : "";
                            EmployeeData.LastName = item.FamilyName != null ? item.FamilyName : "";
                            EmployeeData.Email = item.PrimaryEmailAddr != null ? item.PrimaryEmailAddr.Address : "";
                            EmployeeData.Phone = item.PrimaryPhone != null ? item.PrimaryPhone.FreeFormNumber : "";
                            EmployeeData.UserTypeId = 1;
                            EmployeeData.RoleId = 4;
                            EmployeeData.CreatedDate = DateTime.Now;
                            EmployeeData.isLoginAllow = false;
                            EmployeeData.isActive = true;
                            EmployeeData.isDelete = false;
                            EmployeeData.CreatedDate = DateTime.Now;
                            EmployeeData.EditDate = DateTime.Now;
                            DB.Entry(EmployeeData);
                        }
                        
                    }
                    DB.SaveChanges();
                }

                if (Type.Contains("Estimate"))
                {
                    QueryService<Estimate> EstimatequerySvc = new QueryService<Estimate>(serviceContext);
                    List<Estimate> EstimateInfo = EstimatequerySvc.ExecuteIdsQuery("select * from Estimate").ToList();

                    tblEstimate EstimateData = null;

                    foreach (Estimate item in EstimateInfo)
                    {
                        int ID = Convert.ToInt32(item.Id);
                        EstimateData = DB.tblEstimates.Where(x => x.QBId == ID ).FirstOrDefault();
                        if(EstimateData==null)
                        {
                            EstimateData = new tblEstimate();
                            EstimateData.QBId = Convert.ToInt32(item.Id);
                            EstimateData.SyncToken = item.SyncToken;
                            EstimateData.EstimateNumber = item.DocNumber;
                            int QBId = Convert.ToInt32(item.CustomerRef.Value);
                            int CustomerId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                            EstimateData.CustomerId = CustomerId;
                            if (item.TxnStatus.Contains("Pending"))
                            {
                                EstimateData.EstimateStatusId = 4;
                            }
                            else if (item.TxnStatus.Contains("Accepted"))
                            {
                                EstimateData.EstimateStatusId = 1;
                            }
                            else if (item.TxnStatus.Contains("Rejected"))
                            {
                                EstimateData.EstimateStatusId = 5;
                            }
                            else if (item.TxnStatus.Contains("Closed"))
                            {
                                EstimateData.EstimateStatusId = 2;
                            }
                            EstimateData.TotalAmount = (double?)item.TotalAmt;

                            EstimateData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            EstimateData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            EstimateData.DocNumber = Convert.ToString(DB.SPGetNumber("E").FirstOrDefault());
                            EstimateData.isActive = true;
                            EstimateData.isDelete = false;
                            DB.tblEstimates.Add(EstimateData);
                            DB.SaveChanges();
                            foreach (var itemLine in item.Line)
                            {
                                tblEstimateItem Item = new tblEstimateItem();
                                Item.Description = itemLine.Description;
                                itemLine.AnyIntuitObject = new Intuit.Ipp.Data.SalesItemLineDetail();
                                Intuit.Ipp.Data.SalesItemLineDetail LI = (Intuit.Ipp.Data.SalesItemLineDetail)itemLine.AnyIntuitObject;
                                Item.Qty = Convert.ToInt32(LI.Qty);
                                Item.Rate = Convert.ToDouble(LI.AnyIntuitObject);
                                Item.Amount = Convert.ToDouble(itemLine.Amount);
                                QBId = Convert.ToInt32(LI.ItemRef.Value);
                                int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                Item.ItemId = ItemId;
                                Item.EstimateId = EstimateData.EstimateId;
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
                        }
                        else
                        {
                            EstimateData = new tblEstimate();
                            EstimateData.QBId = Convert.ToInt32(item.Id);
                            EstimateData.SyncToken = item.SyncToken;
                            EstimateData.EstimateNumber = item.DocNumber;
                            int QBId = Convert.ToInt32(item.CustomerRef.Value);
                            int CustomerId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                            EstimateData.CustomerId = CustomerId;
                            if (item.TxnStatus.Contains("Pending"))
                            {
                                EstimateData.EstimateStatusId = 4;
                            }
                            else if (item.TxnStatus.Contains("Accepted"))
                            {
                                EstimateData.EstimateStatusId = 1;
                            }
                            else if (item.TxnStatus.Contains("Rejected"))
                            {
                                EstimateData.EstimateStatusId = 5;
                            }
                            else if (item.TxnStatus.Contains("Closed"))
                            {
                                EstimateData.EstimateStatusId = 2;
                            }
                            EstimateData.TotalAmount = (double?)item.TotalAmt;

                            EstimateData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            EstimateData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            EstimateData.DocNumber = Convert.ToString(DB.SPGetNumber("E").FirstOrDefault());
                            EstimateData.isActive = true;
                            EstimateData.isDelete = false;
                            DB.Entry(EstimateData);
                            DB.SaveChanges();
                            List<tblEstimateItem> ItemData = DB.tblEstimateItems.Where(x => x.EstimateId == EstimateData.EstimateId).ToList();
                            if(ItemData!=null && ItemData.Count!=0)
                            {
                                DB.tblEstimateItems.RemoveRange(ItemData);
                            }

                            foreach (var itemLine in item.Line)
                            {
                                tblEstimateItem Item = new tblEstimateItem();
                                Item.Description = itemLine.Description;
                                itemLine.AnyIntuitObject = new Intuit.Ipp.Data.SalesItemLineDetail();
                                Intuit.Ipp.Data.SalesItemLineDetail LI = (Intuit.Ipp.Data.SalesItemLineDetail)itemLine.AnyIntuitObject;
                                Item.Qty = Convert.ToInt32(LI.Qty);
                                Item.Rate = Convert.ToDouble(LI.AnyIntuitObject);
                                Item.Amount = Convert.ToDouble(itemLine.Amount);
                                QBId = Convert.ToInt32(LI.ItemRef.Value);
                                int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                Item.ItemId = ItemId;
                                Item.EstimateId = EstimateData.EstimateId;
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
                        }
                        

                    }

                }

                if (Type.Contains("PurchaseOrder"))
                {
                    QueryService<PurchaseOrder> PurchaseOrderquerySvc = new QueryService<PurchaseOrder>(serviceContext);
                    List<PurchaseOrder> PurchaseOrderInfo = PurchaseOrderquerySvc.ExecuteIdsQuery("select * from PurchaseOrder").ToList();

                    tblPurchaseOrder PurchaseOrderData = null;

                    foreach (PurchaseOrder item in PurchaseOrderInfo)
                    {
                        int ID = Convert.ToInt32(item.Id);
                        PurchaseOrderData = DB.tblPurchaseOrders.Where(x => x.QBId == ID).FirstOrDefault();
                        if (PurchaseOrderData == null)
                        {
                            PurchaseOrderData = new tblPurchaseOrder();
                            PurchaseOrderData.QBId = Convert.ToInt32(item.Id);
                            PurchaseOrderData.SyncToken = item.SyncToken;
                            PurchaseOrderData.PurchaseOrderNumber = item.DocNumber;
                            int QBId = Convert.ToInt32(item.VendorRef.Value);
                            int VendorId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                            PurchaseOrderData.SupplierId = VendorId;
                            if (item.POStatus.ToString().Contains("Open"))
                            {
                                PurchaseOrderData.StatusId = 1;
                            }
                            else if (item.POStatus.ToString().Contains("Close"))
                            {
                                PurchaseOrderData.StatusId = 2;
                            }
                            PurchaseOrderData.Date = DateTime.Now;
                            PurchaseOrderData.Amount = (double)item.TotalAmt;
                            PurchaseOrderData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            PurchaseOrderData.PurchaseOrderNumber = item.DocNumber;
                            PurchaseOrderData.DocNumber = Convert.ToString(DB.SPGetNumber("P").FirstOrDefault());
                            PurchaseOrderData.isActive = true;
                            PurchaseOrderData.isDelete = false;
                            DB.tblPurchaseOrders.Add(PurchaseOrderData);
                            DB.SaveChanges();
                            foreach (var itemLine in item.Line)
                            {
                                tblPurchaseOrderItem Item = new tblPurchaseOrderItem();
                                //Item.Description = item.Description;
                                Intuit.Ipp.Data.ItemBasedExpenseLineDetail LI = (Intuit.Ipp.Data.ItemBasedExpenseLineDetail)itemLine.AnyIntuitObject;
                                Item.Qty = Convert.ToInt32(LI.Qty);
                                Item.Rate = Convert.ToDouble(LI.AnyIntuitObject);
                                Item.Amount = Convert.ToDouble(itemLine.Amount);
                                QBId = Convert.ToInt32(LI.ItemRef.Value);
                                int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                Item.ItemId = ItemId;
                                Item.PurchaseOrderId = PurchaseOrderData.PurchaseOrderId;
                                //Item.CreatedBy = UserId;
                                Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                //Item.EditBy = UserId;
                                Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                Item.isActive = true;
                                Item.isDelete = false;
                                DB.tblPurchaseOrderItems.Add(Item);
                                DB.SaveChanges();
                            }
                        }
                        else
                        {
                            PurchaseOrderData = new tblPurchaseOrder();
                            PurchaseOrderData.QBId = Convert.ToInt32(item.Id);
                            PurchaseOrderData.SyncToken = item.SyncToken;
                            PurchaseOrderData.PurchaseOrderNumber = item.DocNumber;
                            int QBId = Convert.ToInt32(item.VendorRef.Value);
                            int VendorId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                            PurchaseOrderData.SupplierId = VendorId;
                            if (item.POStatus.ToString().Contains("Open"))
                            {
                                PurchaseOrderData.StatusId = 1;
                            }
                            else if (item.POStatus.ToString().Contains("Close"))
                            {
                                PurchaseOrderData.StatusId = 2;
                            }
                            PurchaseOrderData.Date = DateTime.Now;
                            PurchaseOrderData.Amount = (double)item.TotalAmt;
                            PurchaseOrderData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            PurchaseOrderData.PurchaseOrderNumber = item.DocNumber;
                            PurchaseOrderData.DocNumber = Convert.ToString(DB.SPGetNumber("P").FirstOrDefault());
                            PurchaseOrderData.isActive = true;
                            PurchaseOrderData.isDelete = false;
                            DB.Entry(PurchaseOrderData);
                            DB.SaveChanges();

                            List<tblPurchaseOrderItem> ItemData = DB.tblPurchaseOrderItems.Where(x => x.PurchaseOrderId == PurchaseOrderData.PurchaseOrderId).ToList();
                            if (ItemData != null && ItemData.Count != 0)
                            {
                                DB.tblPurchaseOrderItems.RemoveRange(ItemData);
                            }

                            foreach (var itemLine in item.Line)
                            {
                                tblPurchaseOrderItem Item = new tblPurchaseOrderItem();
                                //Item.Description = item.Description;
                                Intuit.Ipp.Data.ItemBasedExpenseLineDetail LI = (Intuit.Ipp.Data.ItemBasedExpenseLineDetail)itemLine.AnyIntuitObject;
                                Item.Qty = Convert.ToInt32(LI.Qty);
                                Item.Rate = Convert.ToDouble(LI.AnyIntuitObject);
                                Item.Amount = Convert.ToDouble(itemLine.Amount);
                                QBId = Convert.ToInt32(LI.ItemRef.Value);
                                int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                Item.ItemId = ItemId;
                                Item.PurchaseOrderId = PurchaseOrderData.PurchaseOrderId;
                                //Item.CreatedBy = UserId;
                                Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                //Item.EditBy = UserId;
                                Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                Item.isActive = true;
                                Item.isDelete = false;
                                DB.tblPurchaseOrderItems.Add(Item);
                                DB.SaveChanges();
                            }
                        }
                       
                    }
                }

                if (Type.Contains("Bill"))
                {
                    QueryService<Bill> BillquerySvc = new QueryService<Bill>(serviceContext);
                    List<Bill> BillInfo = BillquerySvc.ExecuteIdsQuery("select * from Bill").ToList();

                    tblBill BillData = null;

                    foreach (Bill item in BillInfo)
                    {
                        int ID = Convert.ToInt32(item.Id);
                        BillData = DB.tblBills.Where(x => x.QBId == ID).FirstOrDefault();
                        if (BillData == null)
                        {
                            BillData = new tblBill();
                            BillData.QBId = Convert.ToInt32(item.Id);
                            BillData.SyncToken = item.SyncToken;
                            BillData.BillNumber = item.DocNumber;
                            int QBId = Convert.ToInt32(item.VendorRef.Value); ;
                            int VendorId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                            BillData.SupplierId = VendorId;

                            BillData.DueDate = (DateTime)item.DueDate;
                            BillData.BillDate = (DateTime)item.TxnDate;
                            BillData.Amount = (double)item.TotalAmt;
                            BillData.Currency = "USD";
                            //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                            BillData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            //Data.CreatedBy = UserId;
                            BillData.DocNumber = Convert.ToString(DB.SPGetNumber("B").FirstOrDefault());
                            BillData.isActive = true;
                            BillData.isDelete = false;
                            DB.tblBills.Add(BillData);
                            DB.SaveChanges();

                            foreach (var itemLine in item.Line)
                            {
                                tblBillItem Item = new tblBillItem();
                                Item.Description = itemLine.Description;
                                Intuit.Ipp.Data.ItemBasedExpenseLineDetail LI = (Intuit.Ipp.Data.ItemBasedExpenseLineDetail)itemLine.AnyIntuitObject;
                                Item.Qty = Convert.ToInt32(LI.Qty);
                                Item.Rate = Convert.ToDouble(LI.AnyIntuitObject);
                                Item.Name = LI.ItemRef.name;
                                Item.Amount = Convert.ToDouble(itemLine.Amount);
                                QBId = Convert.ToInt32(LI.ItemRef.Value);
                                int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                Item.ItemId = ItemId;

                                Item.BillId = BillData.BillId;

                                Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                Item.isActive = true;
                                Item.isDelete = false;
                                DB.tblBillItems.Add(Item);
                                DB.SaveChanges();
                            }
                        }
                        else
                        {
                            BillData = new tblBill();
                            BillData.QBId = Convert.ToInt32(item.Id);
                            BillData.SyncToken = item.SyncToken;
                            BillData.BillNumber = item.DocNumber;
                            int QBId = Convert.ToInt32(item.VendorRef.Value); ;
                            int VendorId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                            BillData.SupplierId = VendorId;

                            BillData.DueDate = (DateTime)item.DueDate;
                            BillData.BillDate = (DateTime)item.TxnDate;
                            BillData.Amount = (double)item.TotalAmt;
                            BillData.Currency = "USD";
                            //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                            BillData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            //Data.CreatedBy = UserId;
                            BillData.DocNumber = Convert.ToString(DB.SPGetNumber("B").FirstOrDefault());
                            BillData.isActive = true;
                            BillData.isDelete = false;
                            DB.Entry(BillData);
                            DB.SaveChanges();

                            List<tblBillItem> ItemData = DB.tblBillItems.Where(x => x.BillId == BillData.BillId).ToList();
                            if (ItemData != null && ItemData.Count != 0)
                            {
                                DB.tblBillItems.RemoveRange(ItemData);
                            }

                            foreach (var itemLine in item.Line)
                            {
                                tblBillItem Item = new tblBillItem();
                                Item.Description = itemLine.Description;
                                Intuit.Ipp.Data.ItemBasedExpenseLineDetail LI = (Intuit.Ipp.Data.ItemBasedExpenseLineDetail)itemLine.AnyIntuitObject;
                                Item.Qty = Convert.ToInt32(LI.Qty);
                                Item.Rate = Convert.ToDouble(LI.AnyIntuitObject);
                                Item.Name = LI.ItemRef.name;
                                Item.Amount = Convert.ToDouble(itemLine.Amount);
                                QBId = Convert.ToInt32(LI.ItemRef.Value);
                                int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                Item.ItemId = ItemId;

                                Item.BillId = BillData.BillId;

                                Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                Item.isActive = true;
                                Item.isDelete = false;
                                DB.tblBillItems.Add(Item);
                                DB.SaveChanges();
                            }
                        }
                        
                    }
                }

                if (Type.Contains("Invoice"))
                {
                    QueryService<Invoice> InvoicequerySvc = new QueryService<Invoice>(serviceContext);
                    List<Invoice> InvoiceInfo = InvoicequerySvc.ExecuteIdsQuery("select * from Invoice").ToList();

                    tblInvoice InvoiceData = null;

                    foreach (Invoice item in InvoiceInfo)
                    {
                        int ID = Convert.ToInt32(item.Id);
                        InvoiceData = DB.tblInvoices.Where(x => x.QBId == ID).FirstOrDefault();
                        if (InvoiceData == null)
                        {
                            InvoiceData = new tblInvoice();
                            InvoiceData.QBId = Convert.ToInt32(item.Id);
                            InvoiceData.SyncToken = item.SyncToken;
                            InvoiceData.InvoiceNumber = item.DocNumber;
                            int QBId = Convert.ToInt32(item.CustomerRef.Value); ;
                            int CustomerId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                            InvoiceData.CustomerId = CustomerId;

                            InvoiceData.TotalAmount = (double)item.TotalAmt;
                            InvoiceData.BalanceAmount = (double)item.Balance;
                            InvoiceData.CustomerMessage = item.CustomerMemo.Value;
                            //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                            InvoiceData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            //Data.CreatedBy = UserId;
                            InvoiceData.DocNumber = Convert.ToString(DB.SPGetNumber("I").FirstOrDefault());
                            InvoiceData.isActive = true;
                            InvoiceData.isDelete = false;
                            DB.tblInvoices.Add(InvoiceData);
                            DB.SaveChanges();

                            foreach (var itemLine in item.Line)
                            {
                                tblInvoiceItem Item = new tblInvoiceItem();
                                Item.Description = itemLine.Description;
                                Intuit.Ipp.Data.SalesItemLineDetail LI = (Intuit.Ipp.Data.SalesItemLineDetail)itemLine.AnyIntuitObject;
                                Item.Qty = Convert.ToInt32(LI.Qty);
                                Item.Rate = Convert.ToDouble(LI.AnyIntuitObject);
                                Item.Name = LI.ItemRef.name;
                                Item.Amount = Convert.ToDouble(itemLine.Amount);
                                QBId = Convert.ToInt32(LI.ItemRef.Value);
                                int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                Item.ItemId = ItemId;

                                Item.InvoiceId = InvoiceData.InvoiceId;

                                Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                Item.isCost = false;
                                Item.isActive = true;
                                Item.isDelete = false;
                                DB.tblInvoiceItems.Add(Item);
                                DB.SaveChanges();
                            }
                        }
                        else
                        {
                            InvoiceData = new tblInvoice();
                            InvoiceData.QBId = Convert.ToInt32(item.Id);
                            InvoiceData.SyncToken = item.SyncToken;
                            InvoiceData.InvoiceNumber = item.DocNumber;
                            int QBId = Convert.ToInt32(item.CustomerRef.Value); ;
                            int CustomerId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                            InvoiceData.CustomerId = CustomerId;

                            InvoiceData.TotalAmount = (double)item.TotalAmt;
                            InvoiceData.BalanceAmount = (double)item.Balance;
                            InvoiceData.CustomerMessage = item.CustomerMemo.Value;
                            //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                            InvoiceData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            //Data.CreatedBy = UserId;
                            InvoiceData.DocNumber = Convert.ToString(DB.SPGetNumber("I").FirstOrDefault());
                            InvoiceData.isActive = true;
                            InvoiceData.isDelete = false;
                            DB.Entry(InvoiceData);
                            DB.SaveChanges();

                            List<tblInvoiceItem> ItemData = DB.tblInvoiceItems.Where(x => x.InvoiceId == InvoiceData.InvoiceId).ToList();
                            if (ItemData != null && ItemData.Count != 0)
                            {
                                DB.tblInvoiceItems.RemoveRange(ItemData);
                            }

                            foreach (var itemLine in item.Line)
                            {
                                tblInvoiceItem Item = new tblInvoiceItem();
                                Item.Description = itemLine.Description;
                                Intuit.Ipp.Data.SalesItemLineDetail LI = (Intuit.Ipp.Data.SalesItemLineDetail)itemLine.AnyIntuitObject;
                                Item.Qty = Convert.ToInt32(LI.Qty);
                                Item.Rate = Convert.ToDouble(LI.AnyIntuitObject);
                                Item.Name = LI.ItemRef.name;
                                Item.Amount = Convert.ToDouble(itemLine.Amount);
                                QBId = Convert.ToInt32(LI.ItemRef.Value);
                                int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                Item.ItemId = ItemId;

                                Item.InvoiceId = InvoiceData.InvoiceId;

                                Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                Item.isCost = false;
                                Item.isActive = true;
                                Item.isDelete = false;
                                DB.tblInvoiceItems.Add(Item);
                                DB.SaveChanges();
                            }
                        }
                        
                    }
                }

                if (Type.Contains("Account"))
                {
                    QueryService<Account> AccountquerySvc = new QueryService<Account>(serviceContext);
                    List<Account> AccountInfo = AccountquerySvc.ExecuteIdsQuery("select * from Account where AccountType IN ('Expense', 'Income')").ToList();

                    tblAccount AccountData = null;

                    foreach (Account item in AccountInfo)
                    {
                        int ID = Convert.ToInt32(item.Id);
                        AccountData = DB.tblAccounts.Where(x => x.QBId == ID).FirstOrDefault();
                        if (AccountData == null)
                        {
                            AccountData = new tblAccount();
                            AccountData.QBId = Convert.ToInt32(item.Id);
                            AccountData.SyncToken = item.SyncToken;
                            //AccountData.Code = item.AcctNum;

                            AccountData.Name = item.Name;
                            if(item.AccountType!=null && item.AccountType.ToString().ToLower()== "expense")
                            {
                                AccountData.Type = "Expense Account";
                            }
                            if (item.AccountType != null && item.AccountType.ToString().ToLower() == "income")
                            {
                                AccountData.Type = "Income Account";
                            }

                            AccountData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            AccountData.isActive = true;
                            AccountData.isDelete = false;
                            DB.tblAccounts.Add(AccountData);
                            DB.SaveChanges();

                        }
                        else
                        {
                            AccountData = new tblAccount();
                            AccountData.QBId = Convert.ToInt32(item.Id);
                            AccountData.SyncToken = item.SyncToken;
                            //AccountData.Code = item.AcctNum;

                            AccountData.Name = item.Name;
                            if (item.AccountType != null && item.AccountType.ToString().ToLower() == "expense")
                            {
                                AccountData.Type = "Expense Account";
                            }
                            if (item.AccountType != null && item.AccountType.ToString().ToLower() == "income")
                            {
                                AccountData.Type = "Income Account";
                            }
                            AccountData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            AccountData.isActive = true;
                            AccountData.isDelete = false;
                            DB.Entry(AccountData);
                            DB.SaveChanges();
                        }
                        
                    }
                }

                return RedirectToAction("Tokens");
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

                return View("ApiCallService", (object)("QBO API call Failed!" + " Error message: " + dbEx.Message));
            }
            catch (Exception ex)
            {
                return View("ApiCallService", (object)("QBO API call Failed!" + " Error message: " + ex.Message));
            }
            //}
            //else
            //    return View("ApiCallService", (object)"QBO API call Failed!");
        }
        /// <summary>
        /// Use the Index page of App controller to get all endpoints from discovery url
        /// </summary>
        public ActionResult Error()
        {
            return View("Error");
        }

        /// <summary>
        /// Action that takes redirection from Callback URL
        /// </summary>
        public ActionResult Tokens()
        {
            return View("Tokens");
        }
        public ActionResult ConnectSuccessfull()
        {
            return View();
        }
    }
}
