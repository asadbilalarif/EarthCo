using EarthCo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace EarthCo.Controllers
{
    public class CallbackController : Controller
    {
        earthcoEntities DB = new earthcoEntities();
        /// <summary>
        /// Code and realmid/company id recieved on Index page after redirect is complete from Authorization url
        /// </summary>
        /// 

        //public async Task<string> Index(string state, string code= "none", string realmId = "none")
        //{

        //    //code = Request.QueryString["code"] ?? "none";
        //    //realmId = Request.QueryString["realmId"] ?? "none";
        //    if (state.Equals(HomeController.auth2Client.CSRFToken, StringComparison.Ordinal))
        //    {
        //        ViewBag.State = state + " (valid)";
        //    }
        //    else
        //    {
        //        ViewBag.State = state + " (invalid)";
        //    }

        //    await GetAuthTokensAsync(code, realmId);

        //    ViewBag.Error = Request.QueryString["error"] ?? "none";

        //    return "Token successfully generated."; 
        //}

        public async Task<ActionResult> Index()
        {
            //Sync the state info and update if it is not the same
            var state = Request.QueryString["state"];
            if (state.Equals(HomeController.auth2Client.CSRFToken, StringComparison.Ordinal))
            {
                ViewBag.State = state + " (valid)";
            }
            else
            {
                ViewBag.State = state + " (invalid)";
            }

            string code = Request.QueryString["code"] ?? "none";
            string realmId = Request.QueryString["realmId"] ?? "none";
            await GetAuthTokensAsync(code, realmId);

            ViewBag.Error = Request.QueryString["error"] ?? "none";

            return RedirectToAction("ConnectSuccessfull", "Home");
        }

        /// <summary>
        /// Exchange Auth code with Auth Access and Refresh tokens and add them to Claim list
        /// </summary>
        private async Task GetAuthTokensAsync(string code, string realmId)
        {
            tblToken TokenData = new tblToken();
            if (realmId != null)
            {
                 Session["realmId"] = realmId;
            }

            Request.GetOwinContext().Authentication.SignOut("TempState");
            var tokenResponse = await HomeController.auth2Client.GetBearerTokenAsync(code);

            var claims = new List<Claim>();

            if (Session["realmId"] != null)
            {
                claims.Add(new Claim("realmId", Session["realmId"].ToString()));
            }

            if (!string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
            {
                claims.Add(new Claim("access_token", tokenResponse.AccessToken));
                Session["access_token"] = tokenResponse.AccessToken;
                claims.Add(new Claim("access_token_expires_at", (DateTime.Now.AddSeconds(tokenResponse.AccessTokenExpiresIn)).ToString()));
            }

            if (!string.IsNullOrWhiteSpace(tokenResponse.RefreshToken))
            {
                claims.Add(new Claim("refresh_token", tokenResponse.RefreshToken));
                claims.Add(new Claim("refresh_token_expires_at", (DateTime.Now.AddSeconds(tokenResponse.RefreshTokenExpiresIn)).ToString()));
            }
            TokenData = DB.tblTokens.FirstOrDefault();
            if (TokenData == null)
            {
                TokenData = new tblToken();
                TokenData.realmId = realmId;
                TokenData.AccessToken = tokenResponse.AccessToken;
                TokenData.RefreshToken = tokenResponse.RefreshToken;
                TokenData.CreatedDate = DateTime.Now;
                TokenData.EditDate = DateTime.Now;
                DB.tblTokens.Add(TokenData);
                DB.SaveChanges();
            }
            else
            {
                TokenData.realmId = realmId.ToString();
                TokenData.AccessToken = tokenResponse.AccessToken;
                TokenData.RefreshToken = tokenResponse.RefreshToken;
                TokenData.EditDate = DateTime.Now;
                DB.Entry(TokenData);
                DB.SaveChanges();
            }

            var id = new ClaimsIdentity(claims, "Cookies");
            Request.GetOwinContext().Authentication.SignIn(id);
        }
    }
}
