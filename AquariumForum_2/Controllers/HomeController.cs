using System.Diagnostics;
using AquariumForum_2.Data;
using AquariumForum_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AquariumForum_2.Controllers
{
    public class HomeController : Controller
    {
        private readonly AquariumForum_2Context _context;

        public HomeController(AquariumForum_2Context context)
        {
            _context = context;
        }

        // GET: Home/Index
        public async Task<IActionResult> Index()
        {
            var discussions = await _context.Discussion
                .Include(d => d.Comment) // Include comments so you can count them
                .OrderByDescending(d => d.CreateDate)  // Order by CreateDate (newest first)
                .Select(d => new Discussion
                {
                    DiscussionId = d.DiscussionId,
                    Title = d.Title,
                    Content = d.Content,
                    CreateDate = d.CreateDate,
                    ImageFilename = d.ImageFilename,
                    CommentCount = d.Comment.Count() // Calculate the comment count for each discussion
                })
                .ToListAsync();

            return View(discussions);
        }



        // GET: Home/GetDiscussion/5
        public async Task<IActionResult> GetDiscussion(int id)
        {
            // Get the discussion along with its comments
            var discussion = await _context.Discussion
                .Include(d => d.Comment)
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

        public IActionResult CreateComment(int discussionId)
        {
            ViewBag.DiscussionId = discussionId;
            return View("~/Views/Comments/Create.cshtml");
        }




        // POST: Home/CreateComment/5 (Handle comment creation)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment(int discussionId, [Bind("Content")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                // Find the discussion that the comment belongs to
                var discussion = await _context.Discussion.FindAsync(discussionId);

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
