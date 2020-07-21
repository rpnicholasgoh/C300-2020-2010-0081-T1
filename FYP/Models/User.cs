using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FYPDraft.Models
{
    public class User

    {
        [Required(ErrorMessage = "Please enter User Name")]
        [Remote(action: "VerifyUserID", controller: "Account")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please enter Email")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter Password")]
        [StringLength(20, MinimumLength = 7, ErrorMessage = "Password must be 7 characters or more")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string Password2 { get; set; }

        public DateTime LastLogin { get; set; }

        [Required(ErrorMessage = "Please choose the correct UserRole")]
        public string UserRole { get; set; }
        public DateTime Batch { get; set; }

        [Required(ErrorMessage = "Please enter Company Name")]
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }

        [Required(ErrorMessage = "Please enter Contact No.")]
        public string ContactNo { get; set; }
    }
}
