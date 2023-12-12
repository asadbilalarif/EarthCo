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

        public static async System.Threading.Tasks.Task GetAuthTokensUsingRefreshTokenAsync()
        {
            var claims = new List<Claim>();

            earthcoEntities db = new earthcoEntities();
            tblToken TokenData = new tblToken();
            string previousRefreshToken = db.tblTokens.Select(s => s.RefreshToken).FirstOrDefault();
            //var tokenResponse = await auth2Client.RefreshTokenAsync(previousRefreshToken);


            var tokenResponse = auth2Client.RefreshTokenAsync(previousRefreshToken);
            tokenResponse.Wait();
            var data = tokenResponse.Result;


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

        }


        public ActionResult InitiateAuth(string submitButton)
        {

            string RFT = DB.tblTokens.Select(s => s.RefreshToken).FirstOrDefault();

            if (RFT!=null && RFT!="")
            {
                var TokenResponse=GetAuthTokensUsingRefreshTokenAsync();
                return RedirectToAction("Tokens");
            }


            switch (submitButton)
            {
                case "Connect to QuickBooks":
                    List<OidcScopes> scopes = new List<OidcScopes>();
                    scopes.Add(OidcScopes.Accounting);
                    string authorizeUrl = auth2Client.GetAuthorizationURL(scopes);
                    return Redirect(authorizeUrl);
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
                    //EsitimateData.SyncToken = "0";
                    //EsitimateData.Id = "1103";
                    EsitimateData.CustomerRef = new CustomerRef();
                    EsitimateData.CustomerRef.value = "3";
                    //EsitimateData.CustomerRef.name = "Cool Cars";
                    //LineData.Id = "1";
                    LineData.Description = "Pest Control Services";
                    LineData.Amount = 105;
                    LineData.DetailType = "SalesItemLineDetail";
                    LineData.SalesItemLineDetail = new Models.EstimateQB.SalesItemLineDetail();
                    LineData.SalesItemLineDetail.ItemRef = new ItemRef();
                    LineData.SalesItemLineDetail.ItemRef.value = "10";
                    //LineData.SalesItemLineDetail.ItemRef.name = "Pest Control";
                    LineData.SalesItemLineDetail.UnitPrice = 35;
                    LineData.SalesItemLineDetail.Qty = 3;
                    EsitimateData.Line = new List<Models.EstimateQB.Line>();
                    EsitimateData.Line.Add(LineData);



                    // Convert the request body to JSON
                    string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(EsitimateData);

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
                        var estimateId = estimateModel["Estimate"]["Id"];
                        // Process jsonResponse
                        return View();
                        }
                        else
                        {
                            // Handle error
                            string errorMessage = await response.Content.ReadAsStringAsync();
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
                    HttpResponseMessage response = await client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/estimate/" + 1108 + "?minorversion=23");
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
                        QBEstimateResponseClass.EstimateMain ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBEstimateResponseClass.EstimateMain>(Test);
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



        public async Task<ActionResult> GetItemsFromQB()
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
                QueryService<Item> querySvc = new QueryService<Item>(serviceContext);
                List<Item> ItemInfo = querySvc.ExecuteIdsQuery("select * from item").ToList();

                tblItem ItemData = null;

                foreach (Item item in ItemInfo)
                {
                    ItemData = new tblItem();
                    ItemData.QBId = Convert.ToInt32(item.Id);
                    ItemData.ItemName = item.Name;
                    ItemData.SaleDescription = item.Description;
                    ItemData.PurchaseDescription = item.Description;
                    ItemData.Type = item.Type.ToString();
                    ItemData.PurchasePrice =Convert.ToDouble(item.PurchaseCost);
                    ItemData.SalePrice =Convert.ToDouble(item.UnitPrice);
                    ItemData.isActive = true;
                    ItemData.isDelete = false;
                    ItemData.CreatedDate = DateTime.Now;
                    ItemData.EditDate = DateTime.Now;
                    DB.tblItems.Add(ItemData);
                }
                    DB.SaveChanges();


                QueryService<Customer> CustomerquerySvc = new QueryService<Customer>(serviceContext);
                List<Customer> CustomerInfo = CustomerquerySvc.ExecuteIdsQuery("select * from Customer").ToList();

                tblUser CustomerData = null;

                foreach (Customer item in CustomerInfo)
                {
                    CustomerData = new tblUser();
                    CustomerData.QBId = Convert.ToInt32(item.Id);
                    CustomerData.FirstName = item.GivenName!=null?item.GivenName:"";
                    CustomerData.LastName = item.FamilyName != null ? item.FamilyName : "";
                    CustomerData.CompanyName = item.CompanyName;
                    CustomerData.Email = item.PrimaryEmailAddr!=null? item.PrimaryEmailAddr.Address:"";
                    CustomerData.Phone = item.PrimaryPhone != null ? item.PrimaryPhone.FreeFormNumber:"";
                    CustomerData.RoleId = 2;
                    CustomerData.UserTypeId = 2;
                    CustomerData.isLoginAllow = false;
                    CustomerData.isActive = true;
                    CustomerData.isDelete = false;
                    CustomerData.CreatedDate = DateTime.Now;
                    CustomerData.EditDate = DateTime.Now;
                    DB.tblUsers.Add(CustomerData);
                }
                DB.SaveChanges();


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
    }
}
