using AquariumForum_2.Data;

namespace AquariumForum_2.Models
{
    public class Comment
    {
        //Comment
        //int CommentId
        //string Content
        //datatime CreateDate
        //int DiscussionId(foreign key)


        // Primary Key for the comment
        public int CommentId { get; set; }

        // Content of the comment
        public string Content { get; set; }

        // Date and time when the comment was created
        //public DateTime CreateDate { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now; // Default value set to the current date and time

        // Foreign Key: The ID of the discussion this comment belongs to
        public int DiscussionId { get; set; }

        // Navigation property: The discussion related to this comment
        public Discussion? Discussion { get; set; } //Made it nullible




        //Keys for authication

        // Foreign key (AspNetUsers table)
        public string ApplicationUserId { get; set; } = string.Empty;
        // Navigation property
        public ApplicationUser? ApplicationUser { get; set; } // nullable!!!





    }
}
