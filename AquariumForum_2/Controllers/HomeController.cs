using System.Diagnostics;
using AquariumForum_2.Data;
using AquariumForum_2.Models;
using AquariumForum_2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace AquariumForum_2.Controllers
{
    public class HomeController : Controller
    {
        private readonly AquariumForum_2Context _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(AquariumForum_2Context context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Home/Index
        public async Task<IActionResult> Index()
        {
            var discussions = await _context.Discussion
                .Include(d => d.ApplicationUser)  // Include ApplicationUser to access Name
                .Include(d => d.Comment)  // Include comments to count them
                .OrderByDescending(d => d.CreateDate)  // Order by CreateDate (newest first)
                .Select(d => new Discussion
                {
                    DiscussionId = d.DiscussionId,
                    Title = d.Title,
                    Content = d.Content,
                    CreateDate = d.CreateDate,
                    ImageFilename = d.ImageFilename,
                    CommentCount = d.Comment.Count(),
                    ApplicationUserId = d.ApplicationUser.Id,// Calculate the comment count for each discussion
                    AuthorName = d.ApplicationUser.Name // Adding the Author's Name directly here
                })
                .ToListAsync();

            return View(discussions);
        }




        public async Task<IActionResult> Profile(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound(); // If no id is passed, return Not Found
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(); // Return a 404 if the user is not found
            }

            // Fetch the discussions for this user
            var discussions = await _context.Discussion
                .Where(d => d.ApplicationUserId == id)
                .ToListAsync();

            // For each discussion, calculate the comment count by querying the Comment table
            foreach (var discussion in discussions)
            {
                // Count the comments for each discussion
                discussion.CommentCount = await _context.Comment
                    .Where(c => c.DiscussionId == discussion.DiscussionId)
                    .CountAsync();
            }

            var model = new UserProfileViewModel
            {
                User = user,
                Discussions = discussions
            };

            return View(model);
        }




        public async Task<IActionResult> GetDiscussion(int id)
        {
            // Get the discussion along with its comments and the associated user (ApplicationUser)
            var discussion = await _context.Discussion
                .Include(d => d.ApplicationUser)  // Include the ApplicationUser for the discussion (author of the discussion)
                .Include(d => d.Comment)  // Include comments associated with the discussion
                .ThenInclude(c => c.ApplicationUser)  // Include the ApplicationUser for each comment (author of the comment)
                .FirstOrDefaultAsync(d => d.DiscussionId == id);

            if (discussion == null)
            {
                return NotFound();
            }

            return View(discussion);
        }



        // GET: Home/CreateComment/5 (Display the comment creation page)
        //public IActionResult CreateComment(int discussionId)
        //{
        //    // Pass the discussion ID to the view to be used in the form
        //    ViewBag.DiscussionId = discussionId;
        //    return View();
        //}

        [Authorize]
        public IActionResult CreateComment(int discussionId)
        {
            ViewBag.DiscussionId = discussionId;
            return View("~/Views/Comments/Create.cshtml");
        }




        // POST: Home/CreateComment/5 (Handle comment creation)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CreateComment(int discussionId, [Bind("Content")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                // Find the discussion that the comment belongs to
                var discussion = await _context.Discussion
                    .FindAsync(discussionId);
                comment.ApplicationUserId = _userManager.GetUserId(User);

                if (discussion != null)
                {
                    // Set the discussion ID for the new comment
                    comment.DiscussionId = discussionId;
                    comment.CreateDate = DateTime.Now;

                    // Add the comment to the database
                    _context.Add(comment);
                    await _context.SaveChangesAsync();
                }

                // Redirect the user back to the discussion page after the comment is created
                return RedirectToAction("GetDiscussion", new { id = discussionId });
            }

            // If the model state is invalid, stay on the CreateComment page
            return View(comment);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }


}
