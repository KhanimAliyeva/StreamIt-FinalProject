using System.ComponentModel.DataAnnotations;

namespace STREAMIT.MVC.Models
{
    public class ContactMessageVm
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Message { get; set; }

        public bool SaveInfo { get; set; }
    }
}
