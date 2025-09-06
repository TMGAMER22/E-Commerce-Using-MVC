using System.ComponentModel.DataAnnotations;

namespace Store.ViewModel.Account
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name ="Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "Confrim Password didn't Match The New one")]
        public string ConfirmPassword { get; set; }
    }
}
