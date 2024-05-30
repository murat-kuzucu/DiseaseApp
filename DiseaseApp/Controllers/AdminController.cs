using DiseaseApp.Data.Abstract;
using DiseaseApp.Data.Concrete.EfCore;
using DiseaseApp.Entity;
using DiseaseApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DiseaseApp.Controllers
{
    
    
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminRepository _adminRepository;
        private readonly DiseaseContext _context;
        private const int PageSize = 10;

        public AdminController(IAdminRepository adminRepository, DiseaseContext context)
        {
            _adminRepository = adminRepository;
            _context = context; // Initialize DiseaseContext
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult UserManager(int page = 1)
        {
            const int pageSize = 10;
            var users = _adminRepository.GetUsers()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalItemCount = _adminRepository.GetUsers().Count();
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPage = page;

            return View(users);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult PostManager(int page = 1)
        {
            const int pageSize = 10;
            var posts = _adminRepository.GetPosts()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalItemCount = _adminRepository.GetPosts().Count();
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPage = page;

            return View(posts);
        }

        [HttpGet]
        [Authorize(Roles="Admin")]
        public IActionResult CommentManager(int page = 1)
        {
            const int pageSize = 10;
            var comments = _adminRepository.GetComments() // Get all comments from the database
            .Skip((page -1) * pageSize)
            .Take(pageSize)
            .ToList();
        
            ViewBag.TotalItemCount = _adminRepository.GetComments().Count();
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPage = page;

            return View(comments);
        }
        [HttpGet]
        [Authorize(Roles="Admin")]
        public IActionResult DeleteComment(int commentId){
            var comment = _adminRepository.GetComments().FirstOrDefault(c => c.CommentId == commentId);
            if(comment == null) return NotFound(); // Comment not found
            return View(comment);
        }

        [HttpPost]
        [ActionName("DeleteComment")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCommentConfirmed(int commentId)
        {
            var comment = _adminRepository.GetComments().FirstOrDefault(c => c.CommentId == commentId);
            if (comment == null)
            {
                return NotFound(); // Comment not found
            }
            _adminRepository.DeleteComment(comment.CommentId); // Assuming DeleteComment method expects commentId
            return RedirectToAction("CommentManager");
        }
        
        [HttpPost]
        [Authorize(Roles="Admin")]
        public IActionResult CreateUser(CreateUserViewModel model){
            if(ModelState.IsValid){
                var user = new User{
                    UserName = model.UserName,
                    Name = model.Name,
                    Email = model.Email,
                    Password = model.Password,
                    UserRole = model.UserRole,
                    IsActive = model.IsActive // Default value is true
                };
                _adminRepository.CreateUser(user);          
            }
            return View();
        }
        [HttpGet]
        [Authorize(Roles="Admin")]
        public IActionResult CreateUser(){
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(int UserId)
        {
            try
            {
                var user = _context.Users
                                    .Include(u => u.Comments)
                                    .Include(u => u.Posts)
                                    .FirstOrDefault(u => u.UserId == UserId);

                if (user == null)
                {
                    return NotFound(); // User not found
                }

                // Delete user comments
                if (user.Comments != null && user.Comments.Any())
                {
                    _context.Comments.RemoveRange(user.Comments);
                }

                // Delete user posts
                if (user.Posts != null && user.Posts.Any())
                {
                    _context.Posts.RemoveRange(user.Posts);
                }

                // Delete user
                _context.Users.Remove(user);

                // Save changes
                _context.SaveChanges();

                return RedirectToAction("UserManager"); // Redirect only if successful
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while deleting user: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the user. Please try again later."); // Internal server error
            }
        }
        
           [HttpGet]
           [Authorize(Roles = "Admin")]
            public IActionResult UserEdit(int? UserId)
            {
                if (UserId == null)
                {
                    return BadRequest("User ID is required");
                }

                var user = _context.Users.FirstOrDefault(u => u.UserId == UserId);
                if (user == null)
                {
                    ViewBag.ErrorMessage = "User not found";
                    return RedirectToAction("UserManager");
                }

                var model = new UserEditViewModel
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Name = user.Name,
                    Email = user.Email,
                    UserRole = user.UserRole,
                    IsActive = user.IsActive
                };

                return View(model);
            }

            [HttpPost]
            [Authorize(Roles = "Admin")]
            public IActionResult UserEdit(UserEditViewModel model)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var user = _context.Users.FirstOrDefault(u => u.UserId == model.UserId);
                        if (user == null)
                        {
                            return NotFound("User not found");
                        }

                        user.UserName = model.UserName;
                        user.Name = model.Name;
                        user.Email = model.Email;
                        user.UserRole = model.UserRole ?? "User";
                        user.IsActive = model.IsActive;

                        _context.SaveChanges();

                        return RedirectToAction("UserManager");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while updating user: {ex.Message}");
                        ModelState.AddModelError(string.Empty, "An error occurred while updating the user. Please try again later.");
                    }
                }
                return View(model);
            }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult ChangeUserStatus(int UserId, bool isActive)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == UserId);
            if (user != null)
            {
                if (user.UserRole == "Admin" && !isActive)
                {
                    TempData["ErrorMessage"] = "Admin accounts cannot be deactivated.";
                    return RedirectToAction("UserManager");
                }

                user.IsActive = isActive;
                _context.SaveChanges();
            }
            return RedirectToAction("UserManager");
        }


    }
}
