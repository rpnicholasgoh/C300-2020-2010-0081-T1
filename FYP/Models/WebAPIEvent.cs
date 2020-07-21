using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FYPDraft.Models;

namespace FYPDraft.Models
{
    public class WebAPIEvent
    {
        public int id { get; set; }
        public string text { get; set; }
        public string description { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public int? event_pid { get; set; }
        public string rec_type { get; set; }
        public long? event_length { get; set; }
        public string location { get; set; }
        public string type { get; set; }
        public string category { get; set; }

        public static explicit operator WebAPIEvent(SchedulerEvent schedulerEvent)
        {
            return new WebAPIEvent
            {
                id = schedulerEvent.Id,
                text = HtmlEncoder.Default.Encode(schedulerEvent.Title),
                description = schedulerEvent.Description,
                location = schedulerEvent.Venue,
                type = schedulerEvent.Type,
                start_date = schedulerEvent.StartDate.ToString("yyyy-MM-dd HH:mm"),
                end_date = schedulerEvent.EndDate.ToString("yyyy-MM-dd HH:mm"),
                event_pid = schedulerEvent.EventPID,
                rec_type = schedulerEvent.RecType,
                event_length = schedulerEvent.EventLength
            };
        }

        public static explicit operator SchedulerEvent(WebAPIEvent schedulerEvent)
        {
            return new SchedulerEvent
            {
                Id = schedulerEvent.id,
                Title = schedulerEvent.text,
                Description = schedulerEvent.description,
                Venue = schedulerEvent.location,
                Type = schedulerEvent.type,
                Category = schedulerEvent.category,
                StartDate = DateTime.Parse(
                    schedulerEvent.start_date,
                    System.Globalization.CultureInfo.InvariantCulture),
                EndDate = DateTime.Parse(
                    schedulerEvent.end_date,
                    System.Globalization.CultureInfo.InvariantCulture),
                EventPID = schedulerEvent.event_pid != null ? schedulerEvent.event_pid.Value : 0,
                EventLength = schedulerEvent.event_length != null ? schedulerEvent.event_length.Value : 0,
                RecType = schedulerEvent.rec_type
            };
        }
    }
}
