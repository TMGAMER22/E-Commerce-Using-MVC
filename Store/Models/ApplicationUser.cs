using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace Store.Models
{
    public class ApplicationUser:IdentityUser
    {
        public Guid? CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
