using EarthCo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text.Json;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EarthCo.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class EmailController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();
        [HttpGet]
        public IHttpActionResult SendEmail(string Link,int UserId=0,int ContactId=0,bool isVendor=false)
        {
            try
            {
                string Email = null;
                //if (isVendor==false)
                //{
                //    tblContact ContactData = DB.tblContacts.Where(x => x.ContactId == ContactId).FirstOrDefault();
                    
                //    if (ContactData == null)
                //    {
                //        ContactData = DB.tblContacts.Where(x => x.CustomerId == UserId).FirstOrDefault();
                //        Email = ContactData.Email;
                //    }
                //    else
                //    {
                //        Email = ContactData.Email;
                //    }
                //}
                //else
                //{
                //    tblUser ContactData = DB.tblUsers.Where(x => x.UserId == UserId).FirstOrDefault();
                //    Email = ContactData.Email;
                //}

                tblUser ContactData = DB.tblUsers.Where(x => x.UserId == UserId).FirstOrDefault();
                Email = ContactData.Email;

                if (Email == null||Email == ""||Email == "--"||!Email.Contains("@"))
                {
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                    responseMessage.Content = new StringContent("Email not found.");
                    return ResponseMessage(responseMessage);
                }

                Link = JsonSerializer.Deserialize<string>(Link);


                tblSetting setting = DB.tblSettings.Find(1);
                string SenderEmail = setting.Email;
                string SenderPassword = setting.Password;
                SmtpClient Client = new SmtpClient(setting.SMTP, Convert.ToInt32(setting.Port));
                //Client.EnableSsl = false;
                Client.EnableSsl = Convert.ToBoolean(setting.isActive); ;
                Client.Timeout = 100000;
                Client.DeliveryMethod = SmtpDeliveryMethod.Network;
                Client.UseDefaultCredentials = false;
                Client.Credentials = new System.Net.NetworkCredential(SenderEmail, SenderPassword);

                string link = Url.Request.RequestUri.ToString();
                link = "https://earth-co.vercel.app/resetpassword";

                byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(Email);
                string encrypted = Convert.ToBase64String(b);

                byte[] t = System.Text.ASCIIEncoding.ASCII.GetBytes(DateTime.Now.ToString());
                string encryptedTime = Convert.ToBase64String(t);


                string body1 = "";
                body1 += "Welcome to EarthCo!";
                body1 += "<br />To view report, please click on the button below: ";
                body1 += "<br /> <button style='padding: 10px 28px 11px 28px;color: #fff;background:#77993D;'><a style='color:white !important;text-decoration : auto !important;' href = '" + Link + "&isMail=1'>View</a></button>";
                body1 += "<br /><br />Yours,<br />The EarthCo Team";

                string body = "";
                body += "<body  style='background-color:white !important'>";
                body += " <div>";
                body += " <table style='background-color: #f2f3f8; max-width:670px;' width='100%' border='0'  cellpadding='0' cellspacing='0'>";
                body += " <tbody> <tr style='background-color:#77993D;'><td style='padding: 0 35px; background-color:#77993D;text-align: center;'><a><img style='width:15%;padding: 1%;' src='https://earth-co.vercel.app/static/media/earthco_logo.9c2914cd26271bfe62a4.png' style='padding-top: 1%;' alt='Alternate Text' />  </a></td> </tr>";
                body += "<tr style='color:#455056; font-size:15px;line-height:35px;text-align: center;'><td style='padding:6px;text-align: center;'></td></tr><tr style='color:#455056; font-size:15px;line-height:35px;text-align: center;'><td style='padding:6px;text-align: center;'>" + body1 + "</td></tr>";
                body += "  </tbody></table>";
                body += "</body>";


                MailMessage mailMessage = new MailMessage(SenderEmail, Email, "Report preview", body);
                mailMessage.IsBodyHtml = true;
                mailMessage.BodyEncoding = System.Text.UTF8Encoding.UTF8;

                Client.Send(mailMessage);

                return Ok("Email has been sent successfully."); 
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
        public IHttpActionResult SendEmailWithFile()
        {
            try
            {
                var Data1 = HttpContext.Current.Request.Params.Get("EmailData");
                //HttpPostedFile file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;

                EmailFile Email = new EmailFile();
                Email.Files = new List<HttpPostedFile>();
                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    Email.Files.Add(HttpContext.Current.Request.Files[i]); ;
                }

                Email.EmailData = JsonSerializer.Deserialize<EmailClass>(Data1);


                string[] AllEmail = Email.EmailData.Email.Split(',');
                string[] CCEmail = Email.EmailData.CCEmail.Split(',');

                tblSetting setting = DB.tblSettings.Find(1);
                string SenderEmail = setting.Email;
                string SenderPassword = setting.Password;
                //SmtpClient Client = new SmtpClient(setting.SMTP, Convert.ToInt32(setting.Port));
                ////Client.EnableSsl = false;
                //Client.EnableSsl = Convert.ToBoolean(setting.isActive); ;
                //Client.Timeout = 100000;
                //Client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //Client.UseDefaultCredentials = false;
                //Client.Credentials = new System.Net.NetworkCredential(SenderEmail, SenderPassword);

                //Iterate through each of the letters
                foreach (var letter in AllEmail)
                {
                    using (MailMessage mm = new MailMessage(SenderEmail, letter))
                    {
                        //string link = Request.Url.ToString();
                        //link = link.Replace("ForgetPassword", "ChangePassword");
                        mm.Subject = Email.EmailData.Subject;

                        foreach (var item in CCEmail)
                        {
                            mm.CC.Add(new MailAddress(item));
                        }
                       
                        //mm.Headers.Add("In-Reply-To", ccemail);
                        string body = Email.EmailData.Body;


                        
                        mm.Body = body;
                        //mm.Attachments.Add(new Attachment(new MemoryStream(pdfBuffer), LoadNumber + ".pdf"));

                        mm.BodyEncoding = System.Text.Encoding.UTF8;
                        mm.SubjectEncoding = System.Text.Encoding.Default;

                        foreach (HttpPostedFile file in Email.Files)
                        {
                            // Assuming LoadNumber is unique for each file, adjust accordingly
                            mm.Attachments.Add(new Attachment(file.InputStream, file.FileName));
                        }

                        mm.IsBodyHtml = true;
                        SmtpClient Client = new SmtpClient(setting.SMTP, Convert.ToInt32(setting.Port));
                        //Client.EnableSsl = false;
                        Client.EnableSsl = Convert.ToBoolean(setting.isActive); ;
                        Client.Timeout = 100000;
                        Client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        Client.UseDefaultCredentials = false;
                        Client.Credentials = new System.Net.NetworkCredential(SenderEmail, SenderPassword);
                        Client.Send(mm);
                    }
                }




                return Ok("Email has been sent successfully.");
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
