using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FYPDraft.Models
{
    public class ResetPwd
    {

        [Required(ErrorMessage = "Please enter your Username")]
        public string Username { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your old Password")]
        [DataType(DataType.Password)]
        public string currentPwd { get; set; }

        [Required(ErrorMessage = "Please enter new Password")]
        [StringLength(20, MinimumLength = 7, ErrorMessage = "Password must be 7 characters or more")]
        [DataType(DataType.Password)]
        public string UserPw { get; set; }

        [DataType(DataType.Password)]
        [Compare("UserPw", ErrorMessage = "Passwords do not match")]
        public string UserPw2 { get; set; }

        public string Token { get; set; }
    }
}
