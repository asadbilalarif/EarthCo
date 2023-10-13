using EarthCo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EarthCo.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CustomerController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();

        [HttpGet]
        public List<tblCustomer> GetCustomersList()
        {
            List<tblCustomer> Data = new List<tblCustomer>();
            Data = DB.tblCustomers.ToList();
            return Data;
        }

        [HttpGet]
        public tblCustomer GetCustomer(int id)
        {
            tblCustomer Data = new tblCustomer();
            Data = DB.tblCustomers.Where(x=>x.CustomerId==id).FirstOrDefault();
            return Data;
        }

        [HttpPost]
        public String AddCustomer([FromBody] CustomerContacts Customer)
        {
            tblCustomer Data = new tblCustomer();
            try
            {
                //HttpCookie cookieObj = Request.Cookies["User"];
                //int UserId = Int32.Parse(cookieObj["UserId"]);
                //int RoleId = Int32.Parse(cookieObj["RoleId"]);
                int UserId = 2;
                if (Customer.CustomerData.CustomerId== 0)
                {
                    Data.CustomerName = Customer.CustomerData.CustomerName;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = Customer.CustomerData.isActive;
                    DB.tblCustomers.Add(Data);
                    DB.SaveChanges();

                    if(Customer.ContactData!=null&& Customer.ContactData.Count != 0)
                    {
                        tblContant ConData = null;

                        foreach (var item in Customer.ContactData)
                        {
                            ConData = new tblContant();
                            ConData = item;
                            ConData.CustomerName = Data.CustomerName;
                            ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.CreatedBy = UserId;
                            ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.EditBy = UserId;
                            ConData.isActive = item.isActive;
                            ConData.CustomerId = Data.CustomerId;
                            DB.tblContants.Add(ConData);
                            DB.SaveChanges();
                        }

                    }

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Add Customer";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();
                    return "Customer has been added successfully.";
                }
                else
                {
                    Data = DB.tblCustomers.Select(r => r).Where(x => x.CustomerId == Customer.CustomerData.CustomerId).FirstOrDefault();

                    Data.CustomerName = Customer.CustomerData.CustomerName;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = Customer.CustomerData.isActive;
                    DB.Entry(Data);
                    DB.SaveChanges();

                    List<tblContant> ConList = DB.tblContants.Where(x => x.CustomerId == Customer.CustomerData.CustomerId).ToList();
                    if(ConList!=null && ConList.Count!=0)
                    {
                        DB.tblContants.RemoveRange(ConList);
                        DB.SaveChanges();
                    }

                    if (Customer.ContactData != null && Customer.ContactData.Count != 0)
                    {
                        tblContant ConData = null;

                        foreach (var item in Customer.ContactData)
                        {
                            ConData = new tblContant();
                            ConData = item;
                            ConData.CustomerName = Data.CustomerName;
                            ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.CreatedBy = UserId;
                            ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.EditBy = UserId;
                            ConData.isActive = item.isActive;
                            ConData.CustomerId = Data.CustomerId;
                            DB.tblContants.Add(ConData);
                            DB.SaveChanges();
                        }

                    }

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Update Customer";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();

                    return "Customer has been Update successfully.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpGet]
        public string DeleteCustomer(int id)
        {
            tblCustomer Data = new tblCustomer();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            int CUserId = 2;
            try
            {

                List<tblContant> ConList = DB.tblContants.Where(x => x.CustomerId == id).ToList();
                if (ConList != null && ConList.Count != 0)
                {
                    DB.tblContants.RemoveRange(ConList);
                    DB.SaveChanges();
                }

                Data = DB.tblCustomers.Select(r => r).Where(x => x.CustomerId== id).FirstOrDefault();
                DB.Entry(Data).State = EntityState.Deleted;
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = CUserId;
                LogData.Action = "Delete Customer";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return "Customer has been deleted successfully.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpGet]
        public string SentInvite(InviteEmployee ParaData)
        {
            tblCustomer Data = new tblCustomer();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            int CUserId = 2;
            try
            {

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
                //string link = "";
                link = link.Replace("ForgetPassword", "ChangeForgetPassword");

                byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(ParaData.Email);
                string encrypted = Convert.ToBase64String(b);

                byte[] t = System.Text.ASCIIEncoding.ASCII.GetBytes(DateTime.Now.ToString());
                string encryptedTime = Convert.ToBase64String(t);


                string body1 = "";
                body1 += "Welcome to EarthCo!";
                body1 += "<br />"+ParaData.FirstName+" "+ ParaData.LastName+ "";
                body1 += "<br />"+ParaData.InvitaionEmail+"";
                body1 += "<br /><br />Yours,<br />The EarthCo Team";

                string body = "";
                body += "<body  style='background-color:white !important'>";
                body += " <div>";
                //body += "<h3>Hello " + sa.ReceiveName + ",</h3>";
                body += " <table style='background-color: #f2f3f8; max-width:670px;' width='100%' border='0'  cellpadding='0' cellspacing='0'>";
                body += " <tbody> <tr style='background-color:#333333;'><td style='padding: 0 35px; background-color:#333333;text-align: center;'><a><img src='https://ci6.googleusercontent.com/proxy/Ia8xyYsLq6FtQcWzOyAOvF7XpZC5N9JGdMFlTO2LwH6Q_PSpKXU2LVHg6bmHoSGjTN1EKugOuHt6dFMCU82XXyTadS1p1EfV7a70vjNPbIkMB7z9H6h_9hgZNRA9bAJNWW-fi4jazw=s0-d-e1-ft#https://automatische-gartenberegnung.de/wp-content/uploads/2020/05/logo-1_200x50.png' style='padding-top: 1%;' alt='Alternate Text' />  </a></td> </tr>";
                body += "<tr style='color:#455056; font-size:15px;line-height:35px;text-align: center;'><td style='padding:6px;text-align: center;'></td></tr><tr style='color:#455056; font-size:15px;line-height:35px;text-align: center;'><td style='padding:6px;text-align: center;'>" + body1 + "</td></tr>";
                body += "  </tbody></table>";
                body += "</body>";


                MailMessage mailMessage = new MailMessage(SenderEmail, ParaData.Email, "Forget Password Email", body);
                mailMessage.IsBodyHtml = true;
                mailMessage.BodyEncoding = System.Text.UTF8Encoding.UTF8;

                Client.Send(mailMessage);


                tblLog LogData = new tblLog();
                LogData.UserId = CUserId;
                LogData.Action = "Sent invite";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return "Invite has been sent successfully.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
