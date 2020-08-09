using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace FYPDraft.Models
{
    public class ResetPassword
    {
        [Required(ErrorMessage = "Please enter Email")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter Password")]
        [StringLength(20, MinimumLength = 7, ErrorMessage = "Password must be 7 characters or more")]
        [DataType(DataType.Password)]
        public string UserPw { get; set; }

        [DataType(DataType.Password)]
        [Compare("UserPw", ErrorMessage = "Passwords do not match")]
        public string UserPw2 { get; set; }
    }
}