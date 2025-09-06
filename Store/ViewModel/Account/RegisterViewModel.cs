using System.ComponentModel.DataAnnotations;

namespace Store.ViewModel.Account
{
    public class RegisterViewModel
    {
        public string Name{ get; set; }
        [DataType(DataType.Password)]   
        public string Password{ get; set; }
        [Display(Name ="Confirm Password")]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [StringLength(50,ErrorMessage ="Password Must Be The Same")]
        public string ConfirmPassword{ get; set; }
        public string Role { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
