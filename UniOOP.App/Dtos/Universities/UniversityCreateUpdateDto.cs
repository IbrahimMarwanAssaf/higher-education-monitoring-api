using System.ComponentModel.DataAnnotations;

namespace UNIOOP.App.Dtos.Universities
{
    public partial class UniversityCreateUpdateDto
    {
        [Required]
        [MaxLength(150)]
        public string UniversityName { get; set; } = string.Empty;
    }
}