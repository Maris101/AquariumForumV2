using Microsoft.AspNetCore.Mvc;
using AquariumForum_2.Data;
using AquariumForum_2.Models;
using System.Threading.Tasks;

namespace AquariumForum_2.Controllers
{
    public class CommentsController : Controller
    {
        private readonly AquariumForum_2Context _context;

        public CommentsController(AquariumForum_2Context context)
        {
            _context = context;
        }

        // GET: Comments/Create
        public IActionResult Create(int discussionId)
        {
            ViewBag.DiscussionId = discussionId;  // Pass discussionId to the view
            return View();
        }

        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        

        public async Task<IActionResult> Create([Bind("Content,DiscussionId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                // No need to set CreateDate manually, it's automatically handled
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Discussions", new { id = comment.DiscussionId });
            }

            return View(comment);
        }




    }
}
