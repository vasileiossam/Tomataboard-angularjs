using System.ComponentModel.DataAnnotations;

namespace Tomataboard.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}