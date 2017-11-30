using System.ComponentModel.DataAnnotations;

namespace RefArc.Api.HATEOAS.Filter
{
    // Presumably you will have an equivalent user account class with a user name.
    public class User
    {
        [Required]
        public string UserName { get; set; }
    }
}
