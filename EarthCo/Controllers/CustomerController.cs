using EarthCo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EarthCo.Controllers
{
    //[Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CustomerController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();

        //[HttpGet]
        //public List<GetCustomerContact> GetCustomersList()
        //{
        //    List<GetCustomerContact> Data = new List<GetCustomerContact>();
        //    GetCustomerContact Temp = null;
        //    List<tblCustomer> CusData = new List<tblCustomer>();
        //    tblContact ConData = new tblContact();
        //    CusData = DB.tblCustomers.ToList();
        //    foreach (var item in CusData)
        //    {
        //        Temp = new GetCustomerContact();
        //        ConData = item.tblContacts.Where(x => x.isPrimary == true).FirstOrDefault();
                
        //        Temp.CustomerId = item.CustomerId;
        //        Temp.CustomerName = item.CustomerName;
        //        if (ConData != null)
        //        {
        //            Temp.ContactId = ConData.ContactId;
        //            Temp.ContactName = ConData.FirstName + " " + ConData.FirstName;
        //            Temp.ContactCompany = ConData.CompanyName;
        //            Temp.ContactEmail = ConData.Email;
        //        }
                    
        //        Data.Add(Temp);
        //    }

        //    return Data;
        //}

        [HttpGet]
        public IHttpActionResult GetCustomersList()
        {
            try
            {
                List<GetCustomerContact> Data = new List<GetCustomerContact>();
                GetCustomerContact Temp = null;
                List<tblCustomer> CusData = new List<tblCustomer>();
                tblContact ConData = new tblContact();
                CusData = DB.tblCustomers.ToList();
                if (CusData == null || CusData.Count == 0)
                {
                    return NotFound(); // 404 - No data found
                }

                foreach (var item in CusData)
                {
                    Temp = new GetCustomerContact();
                    ConData = item.tblContacts.Where(x => x.isPrimary == true).FirstOrDefault();

                    Temp.CustomerId = item.CustomerId;
                    Temp.CustomerName = item.CustomerName;
                    if (ConData != null)
                    {
                        Temp.ContactId = ConData.ContactId;
                        Temp.ContactName = ConData.FirstName + " " + ConData.FirstName;
                        Temp.ContactCompany = ConData.CompanyName;
                        Temp.ContactEmail = ConData.Email;
                    }

                    Data.Add(Temp);
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

        //[HttpGet]
        //public GetCustomerContact GetCustomer(int id)
        //{
        //    GetCustomerContact Data = new GetCustomerContact();
        //    tblCustomer CusData = new tblCustomer();
        //    tblContact ConData = new tblContact();
        //    CusData = DB.tblCustomers.Where(x => x.CustomerId == id).FirstOrDefault();
        //    if(CusData==null)
        //    {
        //        return null;
        //    }
        //    ConData = CusData.tblContacts.Where(x => x.isPrimary == true).FirstOrDefault();
        //    Data.CustomerId = CusData.CustomerId;
        //    Data.CustomerName = CusData.CustomerName;
        //    Data.ContactId = ConData.ContactId;
        //    Data.ContactName = ConData.FirstName + " " + ConData.FirstName;
        //    Data.ContactCompany = ConData.CompanyName;
        //    Data.ContactEmail = ConData.Email;

        //    return Data;
        //}

        [HttpGet]
        public IHttpActionResult GetCustomer(int id)
        {
            try
            {
                tblCustomer CusData = new tblCustomer();
                CusData = DB.tblCustomers.Where(x => x.CustomerId == id).FirstOrDefault();
                if (CusData == null)
                {
                    CustomerContacts Data = new CustomerContacts();
                    CusData = new tblCustomer();
                    Data.CustomerData = CusData;
                    List<tblContact> ConData = new List<tblContact>();
                    tblContact temp = new tblContact();
                    ConData.Add(temp);
                    Data.ContactData= ConData;
                    string userJson = JsonConvert.SerializeObject(Data);
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                    responseMessage.Content = new StringContent(userJson, Encoding.UTF8, "application/json");
                    return ResponseMessage(responseMessage);
                }

                return Ok(CusData); // 200 - Successful response with data
            }
            catch (Exception ex)
            {
                // Log the exception
                // You may also choose to return a more specific error response (e.g., 500 - Internal Server Error) here.
                return InternalServerError(ex);
            }

        }

        [HttpGet]
        public IHttpActionResult GetContact(int id)
        {
            try
            {
                List<tblContact> ConData = new List<tblContact>();
                ConData = DB.tblContacts.Where(x => x.CustomerId == id).ToList();
                if (ConData == null || ConData.Count==0)
                {
                    ConData = new List<tblContact>();
                    string userJson = JsonConvert.SerializeObject(ConData);
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                    responseMessage.Content = new StringContent(userJson, Encoding.UTF8, "application/json");
                    return ResponseMessage(responseMessage);
                }

                return Ok(ConData); // 200 - Successful response with data
            }
            catch (Exception ex)
            {
                // Log the exception
                // You may also choose to return a more specific error response (e.g., 500 - Internal Server Error) here.
                return InternalServerError(ex);
            }

        }


        //[HttpPost]
        //public String AddCustomer([FromBody] CustomerContacts Customer)
        //{
        //    tblCustomer Data = new tblCustomer();
        //    try
        //    {
        //        //HttpCookie cookieObj = Request.Cookies["User"];
        //        //int UserId = Int32.Parse(cookieObj["UserId"]);
        //        //int RoleId = Int32.Parse(cookieObj["RoleId"]);
        //        int UserId = 2;
        //        if (Customer.CustomerData.CustomerId== 0)
        //        {
        //            Data.CustomerName = Customer.CustomerData.CustomerName;
        //            Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        //            Data.CreatedBy = UserId;
        //            Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        //            Data.EditBy = UserId;
        //            Data.isActive = Customer.CustomerData.isActive;
        //            DB.tblCustomers.Add(Data);
        //            DB.SaveChanges();

        //            if(Customer.ContactData!=null && Customer.ContactData.Count != 0)
        //            {
        //                tblContact ConData = null;

        //                foreach (var item in Customer.ContactData)
        //                {
        //                    ConData = new tblContact();
        //                    ConData = item;
        //                    ConData.CustomerName = Data.CustomerName;
        //                    ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        //                    ConData.CreatedBy = UserId;
        //                    ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        //                    ConData.EditBy = UserId;
        //                    ConData.isActive = item.isActive;
        //                    ConData.isPrimary = item.isPrimary;
        //                    ConData.CustomerId = Data.CustomerId;
        //                    DB.tblContacts.Add(ConData);
        //                    DB.SaveChanges();
        //                }

        //            }

        //            tblLog LogData = new tblLog();
        //            LogData.UserId = UserId;
        //            LogData.Action = "Add Customer";
        //            LogData.CreatedDate = DateTime.Now;
        //            DB.tblLogs.Add(LogData);
        //            DB.SaveChanges();
        //            return "Customer has been added successfully.";
        //        }
        //        else
        //        {
        //            Data = DB.tblCustomers.Select(r => r).Where(x => x.CustomerId == Customer.CustomerData.CustomerId).FirstOrDefault();

        //            Data.CustomerName = Customer.CustomerData.CustomerName;
        //            Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        //            Data.EditBy = UserId;
        //            Data.isActive = Customer.CustomerData.isActive;
        //            DB.Entry(Data);
        //            DB.SaveChanges();

        //            List<tblContact> ConList = DB.tblContacts.Where(x => x.CustomerId == Customer.CustomerData.CustomerId).ToList();
        //            if(ConList!=null && ConList.Count!=0)
        //            {
        //                DB.tblContacts.RemoveRange(ConList);
        //                DB.SaveChanges();
        //            }

        //            if (Customer.ContactData != null && Customer.ContactData.Count != 0)
        //            {
        //                tblContact ConData = null;

        //                foreach (var item in Customer.ContactData)
        //                {
        //                    ConData = new tblContact();
        //                    ConData = item;
        //                    ConData.CustomerName = Data.CustomerName;
        //                    ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        //                    ConData.CreatedBy = UserId;
        //                    ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        //                    ConData.EditBy = UserId;
        //                    ConData.isActive = item.isActive;
        //                    ConData.isPrimary = item.isPrimary;
        //                    ConData.CustomerId = Data.CustomerId;
        //                    DB.tblContacts.Add(ConData);
        //                    DB.SaveChanges();
        //                }

        //            }

        //            tblLog LogData = new tblLog();
        //            LogData.UserId = UserId;
        //            LogData.Action = "Update Customer";
        //            LogData.CreatedDate = DateTime.Now;
        //            DB.tblLogs.Add(LogData);
        //            DB.SaveChanges();

        //            return "Customer has been Update successfully.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}

        [HttpPost]
        public IHttpActionResult AddCustomer([FromBody] CustomerContacts Customer)
        {
            try
            {
                //var userIdClaim = User.Identity as ClaimsIdentity;
                //int userId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                int userId = 2; // Replace with your authentication mechanism to get the user's ID.

                if (Customer.CustomerData.CustomerId == 0)
                {
                    // Creating a new customer.
                    var newCustomer = new tblCustomer
                    {
                        CustomerName = Customer.CustomerData.CustomerName,
                        CreatedDate = DateTime.Now,
                        CreatedBy = userId,
                        EditDate = DateTime.Now,
                        EditBy = userId,
                        isActive = Customer.CustomerData.isActive
                    };

                    DB.tblCustomers.Add(newCustomer);
                    DB.SaveChanges();

                    if (Customer.ContactData != null && Customer.ContactData.Count != 0)
                    {
                        tblContact ConData = null;

                        foreach (var item in Customer.ContactData)
                        {
                            ConData = new tblContact();
                            ConData = item;
                            ConData.CustomerName = newCustomer.CustomerName;
                            ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.CreatedBy = userId;
                            ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.EditBy = userId;
                            ConData.isActive = item.isActive;
                            ConData.isPrimary = item.isPrimary;
                            ConData.CustomerId = newCustomer.CustomerId;
                            DB.tblContacts.Add(ConData);
                            DB.SaveChanges();
                        }

                    }

                    var logData = new tblLog
                    {
                        UserId = userId,
                        Action = "Add Customer",
                        CreatedDate = DateTime.Now
                    };

                    DB.tblLogs.Add(logData);
                    DB.SaveChanges();

                    return Ok("Customer has been added successfully.");
                }
                else
                {
                    // Updating an existing customer.
                    var existingCustomer = DB.tblCustomers.SingleOrDefault(c => c.CustomerId == Customer.CustomerData.CustomerId);

                    if (existingCustomer == null)
                    {
                        return NotFound(); // Customer not found.
                    }

                    existingCustomer.CustomerName = Customer.CustomerData.CustomerName;
                    existingCustomer.EditDate = DateTime.Now;
                    existingCustomer.EditBy = userId;
                    existingCustomer.isActive = Customer.CustomerData.isActive;

                    // Remove existing contacts
                    var existingContacts = DB.tblContacts.Where(c => c.CustomerId == existingCustomer.CustomerId).ToList();
                    DB.tblContacts.RemoveRange(existingContacts);

                    // Add new contacts
                    if (Customer.ContactData != null && Customer.ContactData.Count != 0)
                    {
                        tblContact ConData = null;

                        foreach (var item in Customer.ContactData)
                        {
                            ConData = new tblContact();
                            ConData = item;
                            ConData.CustomerName = existingCustomer.CustomerName;
                            ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.CreatedBy = userId;
                            ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.EditBy = userId;
                            ConData.isActive = item.isActive;
                            ConData.isPrimary = item.isPrimary;
                            ConData.CustomerId = existingCustomer.CustomerId;
                            DB.tblContacts.Add(ConData);
                            DB.SaveChanges();
                        }

                    }

                    var logData = new tblLog
                    {
                        UserId = userId,
                        Action = "Update Customer",
                        CreatedDate = DateTime.Now
                    };

                    DB.tblLogs.Add(logData);
                    DB.SaveChanges();

                    return Ok("Customer has been updated successfully.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                return InternalServerError(ex); // 500 - Internal Server Error
            }
        }



        //[HttpGet]
        //public string DeleteCustomer(int id)
        //{
        //    tblCustomer Data = new tblCustomer();
        //    //HttpCookie cookieObj = Request.Cookies["User"];
        //    //int CUserId = Int32.Parse(cookieObj["UserId"]);
        //    int CUserId = 2;
        //    try
        //    {

        //        List<tblContact> ConList = DB.tblContacts.Where(x => x.CustomerId == id).ToList();
        //        if (ConList != null && ConList.Count != 0)
        //        {
        //            DB.tblContacts.RemoveRange(ConList);
        //            DB.SaveChanges();
        //        }

        //        Data = DB.tblCustomers.Select(r => r).Where(x => x.CustomerId== id).FirstOrDefault();
        //        DB.Entry(Data).State = EntityState.Deleted;
        //        DB.SaveChanges();

        //        tblLog LogData = new tblLog();
        //        LogData.UserId = CUserId;
        //        LogData.Action = "Delete Customer";
        //        LogData.CreatedDate = DateTime.Now;
        //        DB.tblLogs.Add(LogData);
        //        DB.SaveChanges();
        //        return "Customer has been deleted successfully.";
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}



        [HttpGet]
        public IHttpActionResult DeleteCustomer(int id)
        {
            try
            {
                int userId = 2; // Replace with your authentication mechanism to get the user's ID.

                // Check if the customer with the specified ID exists

                tblEstimate CheckEstimate = DB.tblEstimates.Where(x => x.CustomerId == id).FirstOrDefault();
                tblServiceRequest CheckServices = DB.tblServiceRequests.Where(x => x.CustomerId == id).FirstOrDefault();
                tblPunchlist CheckPunchlist = DB.tblPunchlists.Where(x => x.CustomerId == id).FirstOrDefault();

                if (CheckEstimate!=null || CheckServices!=null || CheckPunchlist!=null)
                {
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.Conflict);
                    responseMessage.Content = new StringContent("Cannot delete the record due to related data.");
                    return ResponseMessage(responseMessage);
                }

                var customerToDelete = DB.tblCustomers.FirstOrDefault(c => c.CustomerId == id);

                if (customerToDelete == null)
                {
                    return NotFound(); // 404 - Customer not found
                }

                // Remove associated contacts
                var contactsToDelete = DB.tblContacts.Where(c => c.CustomerId == id).ToList();
                DB.tblContacts.RemoveRange(contactsToDelete);

                // Remove the customer
                DB.Entry(customerToDelete).State = EntityState.Deleted;
                DB.SaveChanges();

                // Log the action
                var logData = new tblLog
                {
                    UserId = userId,
                    Action = "Delete Customer",
                    CreatedDate = DateTime.Now
                };

                DB.tblLogs.Add(logData);
                DB.SaveChanges();

                return Ok("Customer has been deleted successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception
                return InternalServerError(ex); // 500 - Internal Server Error
            }
        }

        [HttpGet]
        public IHttpActionResult SentInvite(InviteEmployee ParaData)
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
                return Ok("Invite has been sent successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
