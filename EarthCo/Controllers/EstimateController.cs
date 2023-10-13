using EarthCo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EarthCo.Controllers
{
    public class EstimateController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();

        [HttpGet]
        public List<tblEstimate> GetEstimateList()
        {
            List<tblEstimate> Data = new List<tblEstimate>();
            Data = DB.tblEstimates.ToList();
            return Data;
        }

        [HttpGet]
        public tblEstimate GetEstimate(int id)
        {
            //DB.Configuration.ProxyCreationEnabled = false;
            tblEstimate Data = new tblEstimate();
            List<tblEstimateItem> EstimateItems = new List<tblEstimateItem>();
            Data = DB.tblEstimates.Where(x => x.EstimateId == id).FirstOrDefault();
            return Data;
        }

        [HttpPost]
        public String AddEstimate([FromBody] tblEstimate Estimate)
        {
            tblEstimate Data = new tblEstimate();
            try
            {
                //HttpCookie cookieObj = Request.Cookies["User"];
                //int UserId = Int32.Parse(cookieObj["UserId"]);
                //int RoleId = Int32.Parse(cookieObj["RoleId"]);
                int UserId = 2;
                if (Estimate.EstimateId == 0)
                {

                    if (Estimate.tblEstimateItems != null && Estimate.tblEstimateItems.Count != 0)
                    {
                        foreach (var item in Estimate.tblEstimateItems)
                        {
                            item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.EstimateId = Data.EstimateId;
                        }
                    }

                    Data = Estimate;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = Estimate.isActive;
                    DB.tblEstimates.Add(Data);
                    DB.SaveChanges();

                    //if (Estimate.tblEstimateItems!= null && Estimate.tblEstimateItems.Count != 0)
                    //{
                    //    tblEstimateItem ConData = null;

                    //    foreach (var item in Estimate.tblEstimateItems)
                    //    {
                    //        ConData = new tblEstimateItem();
                    //        ConData = item;
                    //        ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    //        ConData.CreatedBy = UserId;
                    //        ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    //        ConData.EditBy = UserId;
                    //        ConData.isActive = item.isActive;
                    //        ConData.EstimateId = Data.EstimateId;
                    //        DB.tblEstimateItems.Add(ConData);
                    //        DB.SaveChanges();
                    //    }
                    //}

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Add Estimate";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();
                    return "Estimate has been added successfully.";
                }
                else
                {
                    List<tblEstimateItem> ConList = DB.tblEstimateItems.Where(x => x.EstimateId == Estimate.EstimateId).ToList();
                    if (ConList != null && ConList.Count != 0)
                    {
                        DB.tblEstimateItems.RemoveRange(ConList);
                        DB.SaveChanges();
                    }

                    Data = DB.tblEstimates.Select(r => r).Where(x => x.EstimateId == Estimate.EstimateId).FirstOrDefault();

                    if (Estimate.tblEstimateItems != null && Estimate.tblEstimateItems.Count != 0)
                    {
                        foreach (var item in Estimate.tblEstimateItems)
                        {
                            item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.EstimateId = Data.EstimateId;
                        }
                    }


                    Data.EstimateNumber = Estimate.EstimateNumber;
                    Data.ServiceLocation = Estimate.ServiceLocation;
                    Data.Email = Estimate.Email;
                    Data.IssueDate = DateTime.Now;
                    Data.EstimateStatusId = Estimate.EstimateStatusId;
                    Data.CustomerId = Estimate.CustomerId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = Estimate.isActive;
                    DB.Entry(Data);
                    DB.SaveChanges();

                    

                    //if (Estimate.tblEstimateItems != null && Estimate.tblEstimateItems.Count != 0)
                    //{
                    //    tblEstimateItem ConData = null;

                    //    foreach (var item in Estimate.tblEstimateItems)
                    //    {
                    //        ConData = new tblEstimateItem();
                    //        ConData = item;
                    //        ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    //        ConData.CreatedBy = UserId;
                    //        ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    //        ConData.EditBy = UserId;
                    //        ConData.isActive = item.isActive;
                    //        ConData.EstimateId = Data.EstimateId;
                    //        DB.tblEstimateItems.Add(ConData);
                    //        DB.SaveChanges();
                    //    }
                    //}

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Update Estimate";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();

                    return "Estimate has been Update successfully.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpGet]
        public string DeleteEstimate(int id)
        {
            tblEstimate Data = new tblEstimate();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            int CUserId = 2;
            try
            {

                List<tblEstimateItem> ConList = DB.tblEstimateItems.Where(x => x.EstimateId == id).ToList();
                if (ConList != null && ConList.Count != 0)
                {
                    DB.tblEstimateItems.RemoveRange(ConList);
                    DB.SaveChanges();
                }

                Data = DB.tblEstimates.Select(r => r).Where(x => x.EstimateId == id).FirstOrDefault();
                DB.Entry(Data).State = EntityState.Deleted;
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = CUserId;
                LogData.Action = "Delete Estimate";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return "Estimate has been deleted successfully.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public string UpdateAllSelectedEstimateStatus(UpdateStatus ParaData)
        {
            tblEstimate Data = new tblEstimate();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            int CUserId = 2;
            try
            {
                foreach (var item in ParaData.id)
                {
                    Data = DB.tblEstimates.Select(r => r).Where(x => x.EstimateId == item).FirstOrDefault();
                    Data.EstimateStatusId = ParaData.StatusId;
                    DB.Entry(Data);
                    DB.SaveChanges();
                }

                tblLog LogData = new tblLog();
                LogData.UserId = CUserId;
                LogData.Action = "Update All Selected Estimate status";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return "All selected Estimate status has been updated successfully.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        [HttpPost]
        public string DeleteAllSelectedEstimate(DeleteSelected ParaData)
        {
            tblEstimate Data = new tblEstimate();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            int CUserId = 2;
            try
            {
                foreach (var item in ParaData.id)
                {
                    List<tblEstimateItem> ConList = DB.tblEstimateItems.Where(x => x.EstimateId == item).ToList();
                    if (ConList != null && ConList.Count != 0)
                    {
                        DB.tblEstimateItems.RemoveRange(ConList);
                        DB.SaveChanges();
                    }
                    Data = DB.tblEstimates.Select(r => r).Where(x => x.EstimateId == item).FirstOrDefault();
                    DB.Entry(Data).State = EntityState.Deleted;
                    DB.SaveChanges();
                }

                tblLog LogData = new tblLog();
                LogData.UserId = CUserId;
                LogData.Action = "Delete All Selected Estimate";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return "All selected Estimate has been deleted successfully.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
