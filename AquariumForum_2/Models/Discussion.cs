using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AquariumForum_2.Data;

namespace AquariumForum_2.Models
{
    public class Discussion
    {
        // Primary Key for the discussion
        public int DiscussionId { get; set; } 

        // Title of the discussion
        public string Title { get; set; } = string.Empty;

        // Content of the discussion
        public string Content { get; set; } = string.Empty;

        // Image filename related to the discussion
        public string ImageFilename { get; set; } = string.Empty;

        // Property for file upload, not mapped in EF
        [NotMapped]
        [Display(Name = "Photograph")]
        public IFormFile? ImageFile { get; set; } // nullable! 

        // Add this property to hold the number of comments
        [NotMapped] // This prevents EF from trying to map it to the database
        public int CommentCount { get; set; }

        // Date and time when the discussion was created
        public DateTime CreateDate { get; set; } = DateTime.Now;

        // Navigation property: A collection of comments related to this discussion
        public ICollection<Comment>? Comment { get; set; } //Made it nullible



        //Keys for authication

        // Foreign key (AspNetUsers table)
        public string ApplicationUserId { get; set; } = string.Empty;
        // Navigation property
        public ApplicationUser? ApplicationUser { get; set; } // nullable!!!



        // Add this property to hold the author's name
        public string AuthorName { get; set; } = string.Empty; // This is where the author's name will be stored



    }
}
