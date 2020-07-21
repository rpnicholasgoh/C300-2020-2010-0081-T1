using System;

namespace FYPDraft.Models
{
    public class SchedulerEvent
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Venue { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string FileGuid { get; set; }
        public string FileName { get; set; }
        public int EventPID { get; set; }
        public string RecType { get; set; }
        public long EventLength { get; set; }
    }
}