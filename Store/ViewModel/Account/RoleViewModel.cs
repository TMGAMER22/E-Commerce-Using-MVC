using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Store.ViewModel.Account
{
    public class RoleViewModel
    {
        [Display(Name = "Role Name")]
        public string Name { get; set; }

        public Guid? SelectedCategoryId { get; set; }
        public List<SelectListItem>? Categories { get; set; }
    }
}
