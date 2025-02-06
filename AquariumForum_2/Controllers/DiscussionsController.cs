using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AquariumForum_2.Data;
using AquariumForum_2.Models;
using Microsoft.AspNetCore.Hosting;  // Required for IWebHostEnvironment

namespace AquariumForum_2.Controllers
{
    public class DiscussionsController : Controller
    {
        //private readonly AquariumForum_2Context _context;

        //public DiscussionsController(AquariumForum_2Context context)
        //{
        //    _context = context;
        //}

        private readonly AquariumForum_2Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;



        //public DiscussionsController(AquariumForum_2Context context, AquariumForum_2Context webHostEnvironment)
        public DiscussionsController(AquariumForum_2Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        //// GET: Discussions
        //public async Task<IActionResult> Index()
        //{

        //    return View(await _context.Discussion.ToListAsync());
        //}

        public async Task<IActionResult> Index()
        {
            var discussions = await _context.Discussion
                .Select(d => new Discussion
                {
                    DiscussionId = d.DiscussionId,
                    Title = d.Title,
                    Content = d.Content,
                    CreateDate = d.CreateDate,
                    ImageFilename = d.ImageFilename,
                    // Get the count of comments associated with each discussion
                    CommentCount = d.Comment.Count()
                })
                .OrderByDescending(d => d.CreateDate)  // Order by CreateDate (oldest to newest)
                .ToListAsync();

            return View(discussions);
        }




        // GET: Discussions/Details/5
        public async Task<IActionResult> Details(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                // Fetch the discussion and include comments
                var discussion = await _context.Discussion
                    .Include(d => d.Comment)  // Include comments associated with the discussion
                    .FirstOrDefaultAsync(d => d.DiscussionId == id);

                if (discussion == null)
                {
                    return NotFound();
                }

                // Pass the discussion to the view
                return View(discussion);
            }

            // The "Add Comment" redirect
            public IActionResult AddComment(int discussionId)
            {
                return RedirectToAction("Create", "Comments", new { discussionId = discussionId });
            }
        
    



    // POST: Discussions/Details/5/Comment
    [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int discussionId, [Bind("Body")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                // Find the discussion the comment belongs to
                var discussion = await _context.Discussion.FindAsync(discussionId);

                if (discussion != null)
                {
                    // Associate the comment with the discussion
                    comment.DiscussionId = discussionId;

                    // Add the comment to the database
                    _context.Add(comment);
                    await _context.SaveChangesAsync();
                }

                // Redirect back to the Details page of the discussion with the new comment
                return RedirectToAction(nameof(Details), new { id = discussionId });
            }

            // If the model state is invalid, return to the details page with an error
            return View();
        }




        

            // GET: Discussions/Create
            public IActionResult Create()
            {
                return View();
            }



        // POST: Discussions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DiscussionId,Title,Content,ImageFile")] Discussion discussion)
        {


            // Handle image file, but it is optional
            if (discussion.ImageFile != null)
            {
                // Generate a unique filename for the image if a file is uploaded
                discussion.ImageFilename = Guid.NewGuid().ToString() + Path.GetExtension(discussion.ImageFile.FileName);

                // Save the image to the disk
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", discussion.ImageFilename);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await discussion.ImageFile.CopyToAsync(fileStream);
                }
            }
            else
            {
                //if not null
                discussion.ImageFilename = "";
            }

            // Proceed if the model is valid
            if (ModelState.IsValid)
            {
                // Add the discussion to the database
                _context.Add(discussion);
                await _context.SaveChangesAsync();

                // Redirect to the Details page for the newly created discussion
                return RedirectToAction(nameof(Details), new { id = discussion.DiscussionId });
            }

            // Return to the view if the model is invalid
            return View(discussion);
        }








        // GET: Discussions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussion = await _context.Discussion.FindAsync(id);
            if (discussion == null)
            {
                return NotFound();
            }
            return View(discussion);
        }


        // POST: Discussions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DiscussionId,Title,Content,ImageFile,ImageFilename,CreateDate")] Discussion discussion)
        {
            if (id != discussion.DiscussionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if a new image file is uploaded
                    if (discussion.ImageFile != null)
                    {
                        // Remove the old image file if it exists
                        if (!string.IsNullOrEmpty(discussion.ImageFilename))
                        {
                            string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", discussion.ImageFilename);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Generate a new unique filename for the uploaded image
                        discussion.ImageFilename = Guid.NewGuid().ToString() + Path.GetExtension(discussion.ImageFile.FileName);

                        // Save the new image to disk
                        string newFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", discussion.ImageFilename);
                        using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                        {
                            await discussion.ImageFile.CopyToAsync(fileStream);
                        }
                    }

                    // Update the discussion in the database with the new image filename
                    _context.Update(discussion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscussionExists(discussion.DiscussionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(discussion);
        }


        // GET: Discussions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussion = await _context.Discussion
                .FirstOrDefaultAsync(m => m.DiscussionId == id);
            if (discussion == null)
            {
                return NotFound();
            }

            return View(discussion);
        }

        // POST: Discussions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discussion = await _context.Discussion.FindAsync(id);
            if (discussion != null)
            {
                _context.Discussion.Remove(discussion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiscussionExists(int id)
        {
            return _context.Discussion.Any(e => e.DiscussionId == id);
        }
    }
}
