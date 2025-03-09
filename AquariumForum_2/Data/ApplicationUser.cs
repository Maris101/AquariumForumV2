using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AquariumForum_2.Data
{
    public class ApplicationUser : IdentityUser
    {
        // Name field (required)
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        // Location field (optional)
        public string Location { get; set; } = string.Empty;

        // ImageFilename (stores the file name or path)
        public string ImageFilename { get; set; } = string.Empty;

        // ImageFile (not mapped to the database)
        [NotMapped] 
        public IFormFile ImageFile { get; set; } // The file to upload
    }
}
