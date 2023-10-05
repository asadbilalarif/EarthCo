﻿using EarthCo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Web.Http;

namespace EarthCo.Controllers
{
    public class AccountController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();

        [HttpPost]
        public string Register(tblUser User)
        {
            tblUser Data = new tblUser();
            try
            {
                if (DB.tblUsers.Select(r => r).Where(x => x.Email == User.Email).FirstOrDefault() == null)
                {
                    Data = User;
                    Data.FirstName = User.username;
                    Data.RoleId = 2;
                    byte[] EncDataBtye = new byte[User.Password.Length];
                    EncDataBtye = System.Text.Encoding.UTF8.GetBytes(User.Password);
                    Data.Password = Convert.ToBase64String(EncDataBtye);

                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.isActive = true;
                    DB.tblUsers.Add(Data);
                    DB.SaveChanges();
                    return "User has been added successfully.";
                }
                else
                {
                    //ViewBag.Error = "User Already Exsist!!!";
                    return "User already exsist.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        [HttpPost]
        public IHttpActionResult Login([FromBody] tblUser UserData)
        {
            string pass = null;
            tblLog LogData = new tblLog();
            try
            {
                if (UserData.Password != null)
                {
                    byte[] EncDataBtye = new byte[UserData.Password.Length];
                    EncDataBtye = System.Text.Encoding.UTF8.GetBytes(UserData.Password);
                    pass = Convert.ToBase64String(EncDataBtye);
                }
                var User = DB.tblUsers.Select(r => r).Where(x => x.Email == UserData.Email && x.Password == pass && x.isActive == true).FirstOrDefault();
                if (User != null)
                {
                    //Session["Access"] = DB.tblAccessLevels.Select(r => r).Where(x => x.RoleId == User.RoleId && x.isActive == true).OrderBy(x => x.tblMenu.EditBy).ToList();


                    //var response = new HttpResponseMessage();
                    //string jsonData = JsonConvert.SerializeObject(User);
                    //var cookie = new CookieHeaderValue("User", jsonData);
                    //cookie.Expires = DateTimeOffset.Now.AddDays(1); // Set expiration date
                    //cookie.Path = "/"; // Set the cookie path
                    //response.Headers.AddCookies(new[] { cookie });


                    User.LastLogin = DateTime.Now;
                    DB.Entry(User);
                    DB.SaveChanges();

                    LogData.UserId = User.UserId;
                    LogData.Action = "Login";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();


                    return Ok(new { status = "success"});
                }
                else
                {
                    var UserCheck = DB.tblUsers.Select(r => r).Where(x => x.Email == UserData.Email && x.Password == pass).FirstOrDefault();
                    if (UserCheck != null && (UserCheck.isActive == false || UserCheck.isActive == null))
                    {
                        //ViewBag.Error = "Your account is in-active";
                        return Ok(new { status = "Unauthorized: Your account is in-active." });
                    }
                    else
                    {
                        //ViewBag.Error = "Invalid email or password";
                        return Ok(new { status = "Unauthorized: Invalid email or password." });
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(new { status ="Error: "+ ex.Message });
            }
        }

        [HttpPost]
        public IHttpActionResult CheckEmail([FromBody] tblUser User)
        {
            try
            {
                if (DB.tblUsers.Select(r => r).Where(x => x.Email == User.Email).FirstOrDefault() == null)
                {
                    return Ok(new { status = "Email not exsist." ,isExsist=false});
                }
                else
                {
                    return Ok(new { status = "Email exsist.", isExsist = true });
                }
            }
            catch (Exception ex)
            {

                return Ok(new { status = "Error: " + ex.Message });
            }
        }


        [HttpPost]
        public IHttpActionResult ForgetPassword([FromBody] tblUser User)
        {
            try
            {
                if (DB.tblUsers.Where(x => x.Email == User.Email).FirstOrDefault() != null)
                {


                    tblSetting setting = DB.tblSettings.Find(1);
                    //string SenderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"].ToString();
                    string SenderEmail = setting.Email;
                    //string SenderPassword = System.Configuration.ConfigurationManager.AppSettings["SenderPassword"].ToString();
                    string SenderPassword = setting.Password;
                    //SmtpClient Client = new SmtpClient("yehtohoga.com", 25);
                    SmtpClient Client = new SmtpClient(setting.SMTP, Convert.ToInt32(setting.Port));
                    //Client.EnableSsl = false;
                    Client.EnableSsl = Convert.ToBoolean(setting.isActive); ;
                    Client.Timeout = 100000;
                    Client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    Client.UseDefaultCredentials = false;
                    Client.Credentials = new System.Net.NetworkCredential(SenderEmail, SenderPassword);

                    //string link = Request.Url.ToString();
                    string link = "";
                    link = link.Replace("ForgetPassword", "ChangeForgetPassword");

                    byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(User.Email);
                    string encrypted = Convert.ToBase64String(b);

                    byte[] t = System.Text.ASCIIEncoding.ASCII.GetBytes(DateTime.Now.ToString());
                    string encryptedTime = Convert.ToBase64String(t);


                    string body1 = "";
                    body1 += "Welcome to Automatische!";
                    body1 += "<br />To Change your password, please click on the button below: ";
                    body1 += "<br /> <button style='padding: 10px 28px 11px 28px;color: #fff;background:#333333;'><a style='color:white !important' href = '" + link + "?Email=" + encrypted + "&&Expire=" + encryptedTime + "'>Change Account Password</a></button>";
                    body1 += "<br /><br />Yours,<br />The Automatische Team";

                    string body = "";
                    body += "<body  style='background-color:white !important'>";
                    body += " <div>";
                    //body += "<h3>Hello " + sa.ReceiveName + ",</h3>";
                    body += " <table style='background-color: #f2f3f8; max-width:670px;' width='100%' border='0'  cellpadding='0' cellspacing='0'>";
                    body += " <tbody> <tr style='background-color:#333333;'><td style='padding: 0 35px; background-color:#333333;text-align: center;'><a><img src='https://ci6.googleusercontent.com/proxy/Ia8xyYsLq6FtQcWzOyAOvF7XpZC5N9JGdMFlTO2LwH6Q_PSpKXU2LVHg6bmHoSGjTN1EKugOuHt6dFMCU82XXyTadS1p1EfV7a70vjNPbIkMB7z9H6h_9hgZNRA9bAJNWW-fi4jazw=s0-d-e1-ft#https://automatische-gartenberegnung.de/wp-content/uploads/2020/05/logo-1_200x50.png' style='padding-top: 1%;' alt='Alternate Text' />  </a></td> </tr>";
                    body += "<tr style='color:#455056; font-size:15px;line-height:35px;text-align: center;'><td style='padding:6px;text-align: center;'></td></tr><tr style='color:#455056; font-size:15px;line-height:35px;text-align: center;'><td style='padding:6px;text-align: center;'>" + body1 + "</td></tr>";
                    body += "  </tbody></table>";
                    body += "</body>";


                    MailMessage mailMessage = new MailMessage(SenderEmail, User.Email, "Forget Password Email", body);
                    mailMessage.IsBodyHtml = true;
                    mailMessage.BodyEncoding = System.Text.UTF8Encoding.UTF8;

                    Client.Send(mailMessage);

                    //mailMessage = new MailMessage(SenderEmail, Email, "Thank You Email", "Thank You for Contacting Us!!!");
                    //mailMessage.IsBodyHtml = true;
                    //mailMessage.BodyEncoding = System.Text.UTF8Encoding.UTF8;

                    //Client.Send(mailMessage);

                    //ViewBag.Success = "Email has been sent successfully.";
                    return Ok(new { status = "Email has been sent successfully."});
                }
                else
                {
                    //ViewBag.Error = "User not register";
                    return Ok(new { status = "Email not register."});
                }

            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error: " + ex.Message });
            }
        }

        public string DecrypteEmail([FromBody] tblUser User)
        {
            try
            {
                byte[] b = Convert.FromBase64String(User.Email);
                string decrypted = System.Text.ASCIIEncoding.ASCII.GetString(b);

                return decrypted;
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }


        [HttpPost]
        public IHttpActionResult ChangeForgetPassword(string NewPassword, string ConfirmPassword, string Email)
        {
            string pass = null;
            try
            {


                byte[] EncDataBtye = null;
                tblUser Data = new tblUser();
                Data = DB.tblUsers.Select(r => r).Where(x => x.Email == Email).FirstOrDefault();
                if (Data != null)
                {

                    if (NewPassword == ConfirmPassword)
                    {
                        EncDataBtye = new byte[NewPassword.Length];
                        EncDataBtye = System.Text.Encoding.UTF8.GetBytes(NewPassword);
                        pass = Convert.ToBase64String(EncDataBtye);
                    }
                    else
                    {
                        //ViewBag.PError = "New Password and Confirm Password are not matched!!!";
                        return Ok(new { status = "New Password and Confirm Password are not matched!" });
                    }

                    Data.Password = pass;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    DB.Entry(Data);
                    DB.SaveChanges();

                    //tblSetting setting = DB.tblSettings.Find(1);
                    ////string SenderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"].ToString();
                    //string SenderEmail = setting.Email;
                    ////string SenderPassword = System.Configuration.ConfigurationManager.AppSettings["SenderPassword"].ToString();
                    //string SenderPassword = setting.Password;
                    ////SmtpClient Client = new SmtpClient("yehtohoga.com", 25);
                    //SmtpClient Client = new SmtpClient(setting.SMTP, Convert.ToInt32(setting.Port));
                    ////Client.EnableSsl = false;
                    //Client.EnableSsl = Convert.ToBoolean(setting.isActive); ;
                    //Client.Timeout = 100000;
                    //Client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    //Client.UseDefaultCredentials = false;
                    //Client.Credentials = new System.Net.NetworkCredential(SenderEmail, SenderPassword);

                    ////string link = Request.Url.ToString();
                    //string link = "";
                    //link = link.Replace("ForgetPassword", "ChangeForgetPassword");

                    //byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(Email);
                    //string encrypted = Convert.ToBase64String(b);

                    //byte[] t = System.Text.ASCIIEncoding.ASCII.GetBytes(DateTime.Now.ToString());
                    //string encryptedTime = Convert.ToBase64String(t);


                    //string body1 = "";
                    //body1 += "Welcome to Automatische!";
                    //body1 += "<br />Your password successfully changed";
                    //body1 += "<br /><br />Yours,<br />The Automatische Team";

                    //string body = "";
                    //body += "<body  style='background-color:white !important'>";
                    //body += " <div>";
                    ////body += "<h3>Hello " + sa.ReceiveName + ",</h3>";
                    //body += " <table style='background-color: #f2f3f8; max-width:670px;' width='100%' border='0'  cellpadding='0' cellspacing='0'>";
                    //body += " <tbody> <tr style='background-color: #333333;'><td style='padding: 0 35px; background-color:#333333;text-align: center;'><a><img src='https://ci6.googleusercontent.com/proxy/Ia8xyYsLq6FtQcWzOyAOvF7XpZC5N9JGdMFlTO2LwH6Q_PSpKXU2LVHg6bmHoSGjTN1EKugOuHt6dFMCU82XXyTadS1p1EfV7a70vjNPbIkMB7z9H6h_9hgZNRA9bAJNWW-fi4jazw=s0-d-e1-ft#https://automatische-gartenberegnung.de/wp-content/uploads/2020/05/logo-1_200x50.png' style='padding-top: 1%;' alt='Alternate Text' />  </a></td> </tr>";
                    //body += "<tr style='color:#455056; font-size:15px;line-height:35px;text-align: center;'><td style='padding:6px;text-align: center;'></td></tr><tr style='color:#455056; font-size:15px;line-height:35px;text-align: center;'><td style='padding:6px;text-align: center;'>" + body1 + "</td></tr>";
                    //body += "  </tbody></table>";
                    //body += "</body>";


                    //MailMessage mailMessage = new MailMessage(SenderEmail, Email, "Password alert", body);
                    //mailMessage.IsBodyHtml = true;
                    //mailMessage.BodyEncoding = System.Text.UTF8Encoding.UTF8;

                    //Client.Send(mailMessage);


                    //return RedirectToAction("Login", "Account");
                    return Ok(new { status = "Password changed successfully." });
                }
                else
                {
                    return Ok(new { status = "Email is not correct." });
                }


            }
            catch (Exception ex)
            {

                return Ok(new { status = "Error: " + ex.Message });
            }
        }

    }
}