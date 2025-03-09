using Microsoft.AspNetCore.Mvc;
using AquariumForum_2.Data;
using AquariumForum_2.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AquariumForum_2.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly AquariumForum_2Context _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CommentsController(AquariumForum_2Context context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Comments/Create
        [Authorize]
        public IActionResult Create(int discussionId)
        {
            ViewBag.DiscussionId = discussionId;  // Pass discussionId to the view
            return View();
        }

        // POST: Comments/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]


        //public async Task<IActionResult> Create([Bind("Content,DiscussionId")] Comment comment)
        //{


        //if (ModelState.IsValid)
        //{
        //    comment.ApplicationUserId = _userManager.GetUserId(User);
        //    comment.CreateDate = DateTime.Now;
        //    // No need to set CreateDate manually, it's automatically handled
        //    _context.Add(comment);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction("Details", "Discussions", new { id = comment.DiscussionId });
        //}



        //    return View(comment);
        //}


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

                //// Redirect the user back to the discussion page after the comment is created
                //return RedirectToAction("GetDiscussion", new { id = discussionId });

                // Redirect the user back to the Discussion details page after the comment is created
                return RedirectToAction("Details", "Discussions", new { id = discussionId });

            }

            // If the model state is invalid, stay on the CreateComment page
            return View(comment);
        }




    }
}
