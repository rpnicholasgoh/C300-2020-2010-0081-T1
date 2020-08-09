using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FYPDraft.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FYPDraft.Controllers
{
    [Route("api/[controller]")]
    public class CalEvController : ControllerBase
    {
        private readonly SchedulerContext _context;
        public CalEvController(SchedulerContext context)
        {
            _context = context;
        }

        // GET: api/events
        [Authorize(Roles = "Admin, Alumni, Startup")]
        [HttpGet]
        public IEnumerable<WebAPIEvent> Get()
        {
            return _context.Event.ToList().Select(e => (WebAPIEvent)e);
        }

        // GET api/events/5
        [Authorize(Roles = "Admin, Alumni, Startup")]
        [HttpGet("{id}")]
        public WebAPIEvent Get(int id)
        {
            if (User.IsInRole("Alumni"))
            {
                if (_context.Event.Find(id).Type == "Alumni")
                {
                    return (WebAPIEvent)_context.Event.Find(id);
                }
            }
            else if (User.IsInRole("Startup"))
            {
                if (_context.Event.Find(id).Type == "Startup")
                {
                    return (WebAPIEvent)_context.Event.Find(id);
                }
            }
            return (WebAPIEvent)_context.Event.Find(id);
        }

        // POST api/events
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ObjectResult Post([FromForm] WebAPIEvent apiEvent)
        {
            var newEvent = (SchedulerEvent)apiEvent;

            _context.Event.Add(newEvent);
            _context.SaveChanges();

            var ev = _context.Event.Find(newEvent.Id);
            if ((ev.Id == newEvent.Id) && (newEvent.Type == "Startup"))
            {
                string select = "SELECT * FROM Users WHERE UserRole='Startup'";
                DataTable dt = DBUtl.GetTable(select);

                foreach (DataRow row in dt.Rows)
                {
                    string fname = row.Field<string>("FullName");
                    string email = row.Field<string>("Email");
                    string template = @"Hi {0},<br/><br/>
                               You are invited to the event - <b>{1}</b><br/><br/>
                               Start Date and Time  : <b>{2}</b><br/><br/>
                               End Date and Time    : <b>{3}</b><br/><br/>";
                    string title = "Invitation to Attend Program";
                    string message = String.Format(template, fname, newEvent.Title, newEvent.StartDate, newEvent.EndDate);
                    string result;
                    EmailUtl.SendEmail(email, title, message, out result);
                }
            }
            else if ((ev.Id == newEvent.Id) && (newEvent.Type == "Alumni"))
            {
                string select = "SELECT * FROM Users WHERE UserRole='Alumni'";
                DataTable dt = DBUtl.GetTable(select);

                foreach (DataRow row in dt.Rows)
                {
                    string fname = row.Field<string>("FullName");
                    string email = row.Field<string>("Email");
                    string template = @"Hi {0},<br/><br/>
                               You are invited to the event - <b>{1}</b><br/><br/>
                               Start Date and Time  : <b>{2}</b><br/><br/>
                               End Date and Time    : <b>{3}</b><br/><br/>
                               Venue                : <b>{4}</b>";
                    string title = "Invitation to Attend Program";
                    string message = String.Format(template, fname, newEvent.Title, newEvent.StartDate, newEvent.EndDate, newEvent.Venue);
                    string result;
                    EmailUtl.SendEmail(email, title, message, out result);
                }
            } else if ((ev.Id == newEvent.Id) && (newEvent.Type == "Both"))
            {
                string select = "SELECT * FROM Users WHERE UserRole='Startup' AND UserRole='Alumni'";
                DataTable dt = DBUtl.GetTable(select);

                foreach (DataRow row in dt.Rows)
                {
                    string fname = row.Field<string>("FullName");
                    string email = row.Field<string>("Email");
                    string template = @"Hi {0},<br/><br/>
                               You are invited to the event - <b>{1}</b><br/><br/>
                               Start Date and Time  : <b>{2}</b><br/><br/>
                               End Date and Time    : <b>{3}</b><br/><br/>";
                    string title = "Invitation to Attend Program";
                    string message = String.Format(template, fname, newEvent.Title, newEvent.StartDate, newEvent.EndDate);
                    string result;
                    EmailUtl.SendEmail(email, title, message, out result);
                }
            }

            // delete a single occurrence from a recurring series
            var resultAction = "inserted";
            if (newEvent.RecType == "none")
            {
                resultAction = "deleted";
            }

            return Ok(new
            {
                tid = newEvent.Id,
                action = resultAction
            });
        }

        // PUT api/events/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public ObjectResult Put(int id, [FromForm] WebAPIEvent apiEvent)
        {
            var updatedEvent = (SchedulerEvent)apiEvent;
            var dbEvent = _context.Event.Find(id);
            dbEvent.Title = updatedEvent.Title;
            dbEvent.Description = updatedEvent.Description;
            dbEvent.StartDate = updatedEvent.StartDate;
            dbEvent.EndDate = updatedEvent.EndDate;
            dbEvent.Venue = updatedEvent.Venue;
            dbEvent.Type = updatedEvent.Type;
            dbEvent.Category = updatedEvent.Category;
            dbEvent.EventPID = updatedEvent.EventPID;
            dbEvent.RecType = updatedEvent.RecType;
            dbEvent.EventLength = updatedEvent.EventLength;
            _context.SaveChanges();

            if (!string.IsNullOrEmpty(updatedEvent.RecType) && updatedEvent.RecType != "none")
            {
                //all modified occurrences must be deleted when we update a recurring series
                //https://docs.dhtmlx.com/scheduler/server_integration.html#recurringevents

                _context.Event.RemoveRange(
                    _context.Event.Where(e => e.EventPID == id)
                );
            }

            _context.SaveChanges();

            return Ok(new
            {
                action = "updated"
            });
        }

        // DELETE api/events/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ObjectResult DeleteEvent(int id)
        {
            var e = _context.Event.Find(id);
            if (e != null)
            {
                _context.Event.Remove(e);
                _context.SaveChanges();
            }

            return Ok(new
            {
                action = "deleted"
            });
        }
    }
}