using System.ComponentModel.DataAnnotations;

namespace FYPDraft.Models
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Please enter User Name")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
