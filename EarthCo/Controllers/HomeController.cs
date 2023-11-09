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
using System.Security.Claims;
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
        public ActionResult ApiCallService()
        {
            if (Session["realmId"] != null)
            {
                string realmId = Session["realmId"].ToString();
                try
                {
                    var principal = User as ClaimsPrincipal;
                    //OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(principal.FindFirst("access_token").Value);
                    OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator((string)Session["access_token"]);
                    //OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator("eyJlbmMiOiJBMTI4Q0JDLUhTMjU2IiwiYWxnIjoiZGlyIn0..mZ904VehGCHcCHFNgdNSTg.wwDmKFjYHJ33Wsp-1UBTEVYcv08rrOtpEuoVFT-lz5osH6tInjVg1Ba6QOCcBjvZrXs60aPazHc3_BnyYIHhtZgcVeBfx-2Lvxu6UijcA7Gu7zbzeYcuE3hOm45zA-IywTRWOtgMfMUmsHF2ba4RA48LeRMe90nyTuCamGlRAMm0W2FkDkdnStzmCkkDbzwAp9zPdHi9aMj-MtBkFMHZ-bTBZhKRoM0zIQqeMQduu7Cc-iHBn7NFOxVs2Y4zkSaT9s2uthz8nqI53U61V0MnxkgzMFKjAzscET0zlDFPx9oMDquHLJYVQYkZ1bSsqvdM8soEx4L3JAxfMbCE8PLOH-u008kCF_JRYAyo45wPrn8LzwLGaDkCE3tgs-lTxkXlpsx3IiF9fbBkY_UUWgf8N_PL8Hbz_VN71dJt5r1-4HM6P_7uvHAeWYACF4Wv7kX2MxFk2CBMbkNpxFWtVbu8N73UySNp2f6oxEwlE2N4-GOD4h-i3NTcawwyJvKkqxaaQZ_3bzYtEuwH24dyhYtfHXkxHMkppfOSG_CAsCB9vsmGAErWYweJQ8qVSZ_RYhtmUZP9LYr9L4vFIBsqVmtgt2VJPAmq0tmHSue1djjWIvmfPRx7meeBS6Wf-J2UDV59_KBQ1lwviWK0qlCQoqedtJj3HWQGIfggtJguYZZyIz6NjRO9C080T7u1mWqsjS0QeVDlQ7-mhOcC2BKHtbap42kNB9F0-A-uLEF9vVWAhPEsUNACclLCGxOpxJ-lzWSO.SzaObelYr0P4qdhZ4DUH0A");

                    // Create a ServiceContext with Auth tokens and realmId
                    ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
                    serviceContext.IppConfiguration.MinorVersion.Qbo = "23";

                    // Create a QuickBooks QueryService using ServiceContext
                    QueryService<CompanyInfo> querySvc = new QueryService<CompanyInfo>(serviceContext);
                    //QueryService<PurchaseOrder> querySvc = new QueryService<PurchaseOrder>(serviceContext);
                    CompanyInfo companyInfo = querySvc.ExecuteIdsQuery("SELECT * FROM CompanyInfo").FirstOrDefault();
                    //PurchaseOrder companyInfo = querySvc.ExecuteIdsQuery("select * from purchaseorder startposition 1 maxresults 5").FirstOrDefault();

                    string output = "Company Name: " + companyInfo.CompanyName + " Company Address: " + companyInfo.CompanyAddr.Line1 + ", " + companyInfo.CompanyAddr.City + ", " + companyInfo.CompanyAddr.Country + " " + companyInfo.CompanyAddr.PostalCode;
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
