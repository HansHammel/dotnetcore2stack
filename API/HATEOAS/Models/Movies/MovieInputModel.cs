using RefArc.Api.HATEOAS.Filter;
using System.ComponentModel.DataAnnotations;

namespace RefArc.Api.HATEOAS.Models.Movies
{
    // TODO: Overposting https://docs.microsoft.com/en-us/aspnet/web-api/overview/formats-and-model-binding/model-validation-in-aspnet-web-api
    // TODO: Friendly Exceptions https://github.com/semack/AspNetCore.FriendlyExceptions
    public class MovieInputModel
    {
        [Key]
        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "ID")]
        // To force clients to set a value, make the property nullable and set the Required attribute, side effect: otherwise no id attribute would insert as id=0
        public int? Id { get; set; }
        [Required(ErrorMessage = "Title is Required")]
        [RegularExpression("^[A-Za-z0-9öäüß-]+$", ErrorMessage = "Must be Latin")]
        [MinLength(2)]
        [MaxLength(256, ErrorMessage = "Must be Maximum 256 Characters")]
        [DataType(DataType.Text)]
        //[StringLength(256)]
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Release Year")]
        public int ReleaseYear { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Summary")]
        public string Summary { get; set; }
    }




}
