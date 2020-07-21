using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FYPDraft.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter Title")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title 1-200 chars")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter Description")]
        [StringLength(400, MinimumLength = 1, ErrorMessage = "Description 1-400 chars")]
        public string Description { get; set; }


        [Required(ErrorMessage = "Please enter Start Date/Time")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Please enter End Date/Time")]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Please enter Venue")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Venue 1-200 chars")]
        public string Venue { get; set; }

        [Required(ErrorMessage = "Please select User Role")]
        [StringLength(45, MinimumLength = 1, ErrorMessage = "Type 1-45 chars")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Please select the Category")]
        [StringLength(45, MinimumLength = 1, ErrorMessage = "Type 1-45 chars")]
        public string Category { get; set; }

        [Required]
        public IFormFile File { get; set; }

        public string FileGuid { get; set; }

        public string FileName { get; set; }

        public int EventPID { get; set; }

        public string RecType { get; set; }
        public long EventLength { get; set; }
    }
}

