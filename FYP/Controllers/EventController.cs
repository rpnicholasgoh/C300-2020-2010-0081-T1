using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using FYPDraft.Models;
using System.Data;

namespace FYPDraft.Controllers
{
    public class EventController : Controller
    {
        public IActionResult Events()
        {
            string sql = "SELECT * FROM Event";
            DataTable dt = DBUtl.GetTable(sql);
            return View(dt.Rows);
        }

        #region "Event Add"
        public IActionResult EventAdd()
        {
            return View("Create");
        }

        [HttpPost]
        public IActionResult EventAdd(Event ev, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("Create");
            }
            else
            {
                string filename = DoFileUpload(ev.File);
                string insert = @"INSERT INTO Event(Title, Description, StartDate, EndDate, Venue, Type, Category, FileName, EventPID, EventLength) VALUES('{0}', '{1}', '{2:yyyy-MM-dd HH:mm}','{3:yyyy-MM-dd HH:mm}', '{4}', '{5}', '{6}', '{7}',{8} ,{9})";
                int res = DBUtl.ExecSQL(insert, ev.Title, ev.Description, ev.StartDate, ev.EndDate, ev.Venue, ev.Type,ev.Category,filename, 0, 0);
                if (res == 1)
                {
                    TempData["Message"] = "Event Successfully Added";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
                return RedirectToAction("Events");
            }
        }
        #endregion

        #region "Event Delete"

        public IActionResult Delete(string id)
        {
            string select = @"SELECT * FROM Event WHERE Id={0}";
            DataTable ds = DBUtl.GetTable(select, id);
            if(ds.Rows.Count != 1)
            {
                TempData["Message"] = "Event does not exist";
                TempData["MsgType"] = "warning";
            }
            else
            {
                string delete = "DELETE FROM Event WHERE Id={0}";
                int res = DBUtl.ExecSQL(delete, id);
                if(res ==1)
                {
                    TempData["Message"] = "Event Deleted";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
            }
            return RedirectToAction("Events");
        }
        #endregion

        #region "Event Update"
        public IActionResult Update(string id)
        {
            string select = "SELECT * FROM Event WHERE Id={0}";
            List<Event> list = DBUtl.GetList<Event>(select, id);
            if (list.Count == 1)
            {
                return View(list[0]);
            }
            else
            {
                TempData["Message"] = "Event not found";
                TempData["MsgType"] = "warning";
                return RedirectToAction("Events");
            }

        }
        [HttpPost]
        public IActionResult Update(Event ev)
        {
            ModelState.Remove("File"); //No need to validate "File"
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("Create");
            }
            else
            {
                string update = @"UPDATE Event SET Title ='{1}',
                                            Description = '{2}',
                                            StartDate = '{3:yyyy-MM-dd HH:mm}',
                                            EndDate = '{4:yyyy-MM-dd HH:mm}',
                                            Venue = '{5}',
                                            Type = '{6}',
                                            Category = '{7}'
                                            WHERE Id ={0}";
                int res = DBUtl.ExecSQL(update, ev.Id, ev.Title, ev.Description, ev.StartDate, ev.EndDate, ev.Venue,ev.Type, ev.Category);
                if(res == 1)
                {
                    TempData["Message"] = "Event Updated";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
                return RedirectToAction("Events");
            }
        }
        #endregion

        
        #region "Upload File"
       private string DoFileUpload(IFormFile file)
        {
            string fext = Path.GetExtension(file.FileName);
            string uname = Guid.NewGuid().ToString();
            string fname = uname + fext;
            string fullpath = Path.Combine(_env.WebRootPath, "files/" + fname);
            using (FileStream fs = new FileStream(fullpath, FileMode.Create))
            {
                file.CopyTo(fs);
            }
            return fname;

        }

        private IWebHostEnvironment _env;
        public EventController(IWebHostEnvironment environment)
        {
            _env = environment;
        }

    

        #region "Download File"
        public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filename);

            var memory = new MemoryStream();
            using(var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }
        #endregion
         
        #region"GetContentType"
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        #endregion
       
        #region "Dictionary"
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
        #endregion

    }
}
#endregion