using EarthCo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace EarthCo.Controllers
{
    public class IrrigationController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();

        [HttpGet]
        public IHttpActionResult GetIrrigationList()
        {
            try
            {
                List<tblIrrigation> Data = new List<tblIrrigation>();
                Data = DB.tblIrrigations.ToList();;
                if (Data == null || Data.Count==0)
                {
                    return NotFound();
                }

                return Ok(Data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            
        }

        [HttpGet]
        public IHttpActionResult GetIrrigation(int id)
        {
            try
            {
                //DB.Configuration.ProxyCreationEnabled = false;
                tblIrrigation Data = new tblIrrigation();
                Data = DB.tblIrrigations.Where(x => x.IrrigationId == id).FirstOrDefault();
                if (Data == null)
                {
                    return NotFound();
                }

                return Ok(Data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            
        }

        [HttpPost]
        public IHttpActionResult AddIrrigation([FromBody] IrrigationControllerClass ParaData)
        {
            tblIrrigation Data = new tblIrrigation();
            try
            {
                //HttpCookie cookieObj = Request.Cookies["User"];
                //int UserId = Int32.Parse(cookieObj["UserId"]);
                //int RoleId = Int32.Parse(cookieObj["RoleId"]);
                int UserId = 2;
                if (ParaData.Irrigation.IrrigationId == 0)
                {
                    Data = ParaData.Irrigation;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = ParaData.Irrigation.isActive;
                    DB.tblIrrigations.Add(Data);
                    DB.SaveChanges();

                    if (ParaData.Controller != null && ParaData.Controller.Count != 0)
                    {
                        tblController ConData = null;

                        foreach (var item in ParaData.Controller)
                        {
                            ConData = new tblController();
                            ConData.MakeAndModel = item.MakeAndModel;
                            ConData.SerialNumber = item.SerialNumber;
                            ConData.isSatelliteBased = item.isSatelliteBased;
                            ConData.TypeofWater = item.TypeofWater;
                            ConData.MeterNumber = item.MeterNumber;
                            ConData.MeterSize = item.MeterSize;
                            ConData.NumberofStation = item.NumberofStation;
                            ConData.NumberofValves = item.NumberofValves;
                            ConData.NumberofBrokenMainLines = item.NumberofBrokenMainLines;
                            ConData.TypeofValves = item.TypeofValves;
                            ConData.LeakingValves = item.LeakingValves;
                            ConData.MalfunctioningValves = item.MalfunctioningValves;
                            ConData.NumberofBrokenLateralLines = item.NumberofBrokenLateralLines;
                            ConData.NumberofBrokenHeads = item.NumberofBrokenHeads;
                            ConData.RepairsMade = item.RepairsMade;
                            ConData.UpgradesMade = item.UpgradesMade;
                            ConData.IrrigationId = item.IrrigationId;
                            ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.CreatedBy = UserId;
                            ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.EditBy = UserId;
                            ConData.isActive = item.isActive;

                            string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading"));
                            if (!Directory.Exists(folder))
                            {
                                Directory.CreateDirectory(folder);
                            }
                            if (item.ControllerPhoto != null)
                            {
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("ControllerPhoto" + item.ControllerId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.ControllerPhoto.FileName)));
                                item.ControllerPhoto.SaveAs(path);
                                path = Path.Combine("\\Uploading", Path.GetFileName("ControllerPhoto" + item.ControllerId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.ControllerPhoto.FileName)));
                                ConData.ControllerPhotoPath = path;
                            }
                            if (item.Photo != null)
                            {
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("Photo" + item.ControllerId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.Photo.FileName)));
                                item.Photo.SaveAs(path);
                                path = Path.Combine("\\Uploading", Path.GetFileName("Photo" + item.Photo.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.Photo.FileName)));
                                ConData.PhotoPath = path;
                            }

                            DB.tblControllers.Add(ConData);
                            DB.SaveChanges();
                        }
                    }

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Add Irrigation";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();
                    return Ok("Irrigation has been added successfully.");
                }
                else
                {
                    Data = DB.tblIrrigations.Select(r => r).Where(x => x.IrrigationId == ParaData.Irrigation.IrrigationId).FirstOrDefault();
                    if (Data == null)
                    {
                        return NotFound(); // Customer not found.
                    }


                    Data.IrrigationNumber = ParaData.Irrigation.IrrigationNumber;
                    Data.CustomerId = ParaData.Irrigation.CustomerId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = ParaData.Irrigation.isActive;
                    DB.Entry(Data);
                    DB.SaveChanges();

                    List<tblController> ConList = DB.tblControllers.Where(x => x.IrrigationId == ParaData.Irrigation.IrrigationId).ToList();
                    if (ConList != null && ConList.Count != 0)
                    {
                        DB.tblControllers.RemoveRange(ConList);
                        DB.SaveChanges();
                    }

                    if (ParaData.Controller != null && ParaData.Controller.Count != 0)
                    {
                        tblController ConData = null;

                        foreach (var item in ParaData.Controller)
                        {
                            ConData = new tblController();
                            ConData.MakeAndModel = item.MakeAndModel;
                            ConData.SerialNumber = item.SerialNumber;
                            ConData.isSatelliteBased = item.isSatelliteBased;
                            ConData.TypeofWater = item.TypeofWater;
                            ConData.MeterNumber = item.MeterNumber;
                            ConData.MeterSize = item.MeterSize;
                            ConData.NumberofStation = item.NumberofStation;
                            ConData.NumberofValves = item.NumberofValves;
                            ConData.NumberofBrokenMainLines = item.NumberofBrokenMainLines;
                            ConData.TypeofValves = item.TypeofValves;
                            ConData.LeakingValves = item.LeakingValves;
                            ConData.MalfunctioningValves = item.MalfunctioningValves;
                            ConData.NumberofBrokenLateralLines = item.NumberofBrokenLateralLines;
                            ConData.NumberofBrokenHeads = item.NumberofBrokenHeads;
                            ConData.RepairsMade = item.RepairsMade;
                            ConData.UpgradesMade = item.UpgradesMade;
                            ConData.IrrigationId = item.IrrigationId;
                            ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.CreatedBy = UserId;
                            ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.EditBy = UserId;
                            ConData.isActive = item.isActive;

                            string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading"));
                            if (!Directory.Exists(folder))
                            {
                                Directory.CreateDirectory(folder);
                            }
                            if (item.ControllerPhoto != null)
                            {
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("ControllerPhoto" + item.ControllerId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.ControllerPhoto.FileName)));
                                item.ControllerPhoto.SaveAs(path);
                                path = Path.Combine("\\Uploading", Path.GetFileName("ControllerPhoto" + item.ControllerId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.ControllerPhoto.FileName)));
                                ConData.ControllerPhotoPath = path;
                            }
                            if (item.Photo != null)
                            {
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("Photo" + item.ControllerId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.Photo.FileName)));
                                item.Photo.SaveAs(path);
                                path = Path.Combine("\\Uploading", Path.GetFileName("Photo" + item.Photo.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.Photo.FileName)));
                                ConData.PhotoPath = path;
                            }

                            DB.tblControllers.Add(ConData);
                            DB.SaveChanges();
                        }
                    }

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Update Irrigation";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();

                    return Ok("Irrigation has been Update successfully.");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult DeleteIrrigation(int id)
        {
            tblIrrigation Data = new tblIrrigation();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            int CUserId = 2;
            try
            {
                Data = DB.tblIrrigations.Select(r => r).Where(x => x.IrrigationId == id).FirstOrDefault();

                if (Data == null)
                {
                    return NotFound(); // 404 - Customer not found
                }

                List<tblController> ConList = DB.tblControllers.Where(x => x.IrrigationId == id).ToList();
                if (ConList != null && ConList.Count != 0)
                {
                    DB.tblControllers.RemoveRange(ConList);
                    DB.SaveChanges();
                }


                
                DB.Entry(Data).State = EntityState.Deleted;
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = CUserId;
                LogData.Action = "Delete Irrigation";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("Irrigation has been deleted successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
