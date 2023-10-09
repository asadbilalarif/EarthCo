﻿using EarthCo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EarthCo.Controllers
{
    public class CustomerController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();

        [HttpGet]
        public List<tblContant> GetCustomersList()
        {
            DB.Configuration.ProxyCreationEnabled = false;
            List<tblContant> Data = new List<tblContant>();
            Data = DB.tblContants.ToList();
            return Data;
        }

        [HttpGet]
        public List<tblContant> GetCustomer(int id)
        {
            DB.Configuration.ProxyCreationEnabled = false;
            List<tblContant> Data = new List<tblContant>();
            Data = DB.tblContants.Where(x=>x.CustomerId==id).ToList();
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

        [HttpDelete]
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
    }
}
