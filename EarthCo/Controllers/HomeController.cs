using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.OAuth2PlatformClient;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
//using System.Text.Json;
using System.Web;
using System.Web.Mvc;

namespace EarthCo.Controllers
{
    public class HomeController : Controller
    {
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


        public ActionResult InitiateAuth(string submitButton)
        {
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
            if (Session["realmId"] != null)
            {
                string realmId = Session["realmId"].ToString();
                try
                {
                    var principal = User as ClaimsPrincipal;
                    //OAuth2RequestValidator oauthValidator1 = new OAuth2RequestValidator(principal.FindFirst("access_token").Value);
                    OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator((string)Session["access_token"]);
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
                    string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/4620816365351126570/vendor?minorversion=23";

                    // Request body data
                    var requestData = new
                    {
                        TaxIdentifier = "99-5688293",
                        AcctNum = "35372649",
                        Title = "Ms.",
                        GivenName = "Test",
                        FamilyName = "Test",
                        Suffix = "Sr.",
                        CompanyName = "Dianne's Auto Shop6",
                        DisplayName = "Dianne's Auto Shop6",
                        PrintOnCheckName = "Dianne's Auto Shop1"
                    };

                    Vendor billAddr = new Vendor
                    {
                        TaxIdentifier = "99-5688293",
                        AcctNum = "35372649",
                        Title = "Ms.",
                        GivenName = "Dianne12",
                        FamilyName = "Bradley12",
                        Suffix = "Sr.",
                        CompanyName = "Dianne's Auto Shop6",
                        DisplayName = "Dianne's Auto Shop6",
                        PrintOnCheckName = "Dianne's Auto Shop6"
                    };

                    // Convert the request body to JSON
                    string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);

                    // Create HttpClient
                    using (HttpClient client = new HttpClient())
                    {
                        // Set headers
                        //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                        //client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJlbmMiOiJBMTI4Q0JDLUhTMjU2IiwiYWxnIjoiZGlyIn0..BpQ1lQxl8z4LjJ6oU40OHQ.BqrX891OtD-9dPReTlj4SsOWwF2d7YC_PxWcpV6XGXmKavq9oJrDaMVqfdbbdZJA7UlhZ-bmxRrNIjiV3MJZ_LVRt3HJI83zswBI9EFwOBF-rKFSbeHRKhgT7IJGqZiUZTY7LfuChVwKllXqVUihQSDqsd5aAsRTmsGgFCNyvYPXHZLMWkA-RXt42rfsGTWCYRUJuCnlC0SjSjdPEuwm9chat0NbC7TBhaZ0ZBS71XxKx9QqT3gg6PI1ldNRbXAIHvNEfW3A_vsXdttPcd7nPts8xa6Yp7i9a2FYDmSWrdn5cn0dWe-0ebKRoBxRUt98jUv4qF5P0zHgZeU-zJ_Qc8AK0aLr2k6ngoMkEwh-jgJNs3whIhmgaawlK3cW5_yM2E7Q3EUSXqh1x3MSy3m37uq_iUyTDSDbpeqRvQoGwoPX8fG_2G7BkUocr6WGKuoTgVEC6VHi8hSVjH9Ha6QNdalfGsfGp6uQaiyB1juzg47-wXdz2jnxJNsgJJg0c6wFMoBg18k6szJHv_Ir4R3mDPOE1wsl-v8rQpBTXE5Ei-kOdeMACsnZQuVj_WPV6orsd9dlMlAjDnNK9qyGyAJSOQ_K-jdJc37CWcliTs6PmqIYw-NdgSwtsjbv-ahxt7LU2vfYURJ49I-1QoE395EnWqy7Qv8zQARgMpA2O6RxtgFXmK-QFyLXonFOomTwJmFT1R3AAPtnFxLHtJGMs2bBuVkJkk5AVcKp8QVCYAUP7LYLfBQOXpdt3i8ib2uTB9v_.M6GZiSm2HcYA2IVQLaFniQ");
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer "+ oauthValidator.AccessToken);

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
                            // Process jsonResponse
                            return View();
                        }
                        else
                        {
                            // Handle error
                            string errorMessage = await response.Content.ReadAsStringAsync();
                            // Handle error message
                            return View("Error");
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
            }
            else
                return View("ApiCallService", (object)"QBO API call Failed!");
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
