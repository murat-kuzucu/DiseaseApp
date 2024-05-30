using DiseaseApp.Data.Abstract;
using DiseaseApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiseaseApp.Entity;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace DiseaseApp.Controllers
{
    public class PostsController : Controller
    {
        private const int PageSize = 10;
        private IPostRepository _PostRepository;
        private ITagRepository _TagRepository;
        private ICommentRepository _CommentRepository;

        private readonly string[] _permittedExtensions = { ".jpg", ".jpeg", ".png" };
        private const long _fileSizeLimit = 3 * 1024 * 1024; 
        
        public PostsController(IPostRepository PostRepository, ITagRepository TagRepository, ICommentRepository CommentRepository)
        {
            _PostRepository = PostRepository;
            _TagRepository = TagRepository;
            _CommentRepository = CommentRepository;
        }

        public async Task<IActionResult> Index(string? tag, int page = 1)
        {
            var posts = _PostRepository.Posts.Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(tag))
            {
                posts = posts.Where(p => p.Tags.Any(t => t.Url == tag));
            }

            var totalPosts = await posts.CountAsync();
            var totalPages = (int)Math.Ceiling(totalPosts / (double)PageSize);

            var postsToShow = await posts
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var viewModel = new PostsViewModel
            {
                Posts = postsToShow,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(string? url)
        {
            return View(await _PostRepository.Posts
            .Include(u => u.User)
            .Include(x => x.Tags)
            .Include(c => c.Comments)
            .ThenInclude(u => u.User)
            .FirstOrDefaultAsync(m => m.Url == url));
        }

        [Authorize]
        [HttpPost]
        public JsonResult AddComment(int PostId, string Text)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var UserName = User.FindFirstValue(ClaimTypes.Name);
            var UserAvatar = User.FindFirstValue(ClaimTypes.UserData);

            var Entity = new Comment()
            {
                Text = Text,
                PublishedOn = DateTime.Now,
                PostId = PostId,
                UserId = int.Parse(UserId ?? "")
            };
            _CommentRepository.CreateComment(Entity);

            return Json(new {
                UserName,
                Text,
                Entity.PublishedOn,
                UserAvatar
            });
        }

        [Authorize(Roles = "Admin,Researcher")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        public string GenerateSlug(string title)
        {
            title = title.ToLowerInvariant();
            title = Regex.Replace(title, @"[^a-z0-9\s-]", "");
            title = Regex.Replace(title, @"\s+", "-").Trim();
            title = title.Substring(0, Math.Min(title.Length, 45)).Trim();
            title = title.Trim('-');
            return title;
        }

        [Authorize(Roles = "Admin,Researcher")]
        [HttpPost]
        public IActionResult Create(PostCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var newPost = new Post
                {
                    Title = model.Title,
                    Content = model.Content,
                    Url = GenerateSlug(model.Title ?? ""),
                    Description = model.Description,
                    UserId = int.Parse(userId ?? ""),
                    PublishedOn = DateTime.Now,
                    IsActive = false,
                    Tags = model.Tags
                };

                _PostRepository.CreatePost(newPost);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin,Researcher")]
        public async Task<IActionResult> List()
        {
            var UserRole = User.FindFirstValue(ClaimTypes.Role) ?? "";
            var UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");

            var Posts = _PostRepository.Posts;

            if (UserRole != "Admin")
            {
                Posts = Posts.Where(p => p.UserId == UserId);
            }
            return View(await Posts.ToListAsync());
        }

        [Authorize(Roles = "Admin,Researcher")]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = _PostRepository.Posts.Include(p => p.Tags).FirstOrDefault(p => p.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            ViewBag.Tags = _TagRepository.Tags.ToList();

            var model = new PostEditViewModel
            {
                PostId = post.PostId,
                Title = post.Title,
                Content = post.Content,
                Description = post.Description,
                IsActive = post.IsActive,
                TagIds = post.Tags.Select(t => t.TagId).ToList()
            };

            return View(model);
        }

        [Authorize(Roles = "Admin,Researcher")]
        [HttpPost]
        public async Task<IActionResult> Edit(PostEditViewModel model, int[] TagIds, IFormFile? postImage)
        {
            if (ModelState.IsValid)
            {
                var post = _PostRepository.Posts.Include(p => p.Tags).FirstOrDefault(p => p.PostId == model.PostId);
                if (post == null)
                {
                    return NotFound();
                }

                post.Title = model.Title;
                post.Content = model.Content;
                post.Description = model.Description;
                post.IsActive = model.IsActive;
                post.Url = GenerateSlug(model.Title ?? "");

                // Ensure TagIds is not null
                TagIds ??= Array.Empty<int>();

                // Update tags
                post.Tags.Clear();
                if (TagIds.Length > 0)
                {
                    post.Tags = _TagRepository.Tags.Where(t => TagIds.Contains(t.TagId)).ToList();
                }

                // Handle post image upload
                if (postImage != null && postImage.Length > 0)
                {
                    // Check file size
                    if (postImage.Length > _fileSizeLimit)
                    {
                        ModelState.AddModelError("PostImage", "File size exceeds 3 MB.");
                        return RedirectToAction("Edit", new { id = post.PostId });
                    }

                    // Check file extension
                    var fileExtension = Path.GetExtension(postImage.FileName).ToLowerInvariant();
                    if (string.IsNullOrEmpty(fileExtension) || !_permittedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("PostImage", "Invalid file type. Only .jpg, .jpeg, and .png are allowed.");
                        return RedirectToAction("Edit", new { id = post.PostId });
                    }

                    // Define the path for the post's image
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/lib/img/PostImage", post.PostId.ToString());
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }

                    // Generate a unique file name to avoid conflicts
                    var fileName = $"{Guid.NewGuid()}{fileExtension}";
                    var filePath = Path.Combine(uploads, fileName);

                    // Save the new post image
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await postImage.CopyToAsync(fileStream);
                    }

                    // Update the post's image path
                    post.PostImage = fileName;
                }

                _PostRepository.EditPost(post, TagIds);
                return RedirectToAction("List");
            }

            ViewBag.Tags = _TagRepository.Tags.ToList();
            return View(model);
        }

        [Authorize(Roles = "Admin,Researcher")]
        [HttpPost]
        public IActionResult Delete(int postId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var isAdmin = User.IsInRole("Admin");

            _PostRepository.DeletePost(postId, userId, isAdmin);
            return RedirectToAction("List");
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Researcher")]
        public async Task<IActionResult> ChangePostPhoto(int postId, IFormFile postImage)
        {
            if (postImage == null || postImage.Length == 0)
            {
                ModelState.AddModelError("PostImage", "File is required.");
                return RedirectToAction("Edit", new { id = postId });
            }

            var post = _PostRepository.Posts.FirstOrDefault(p => p.PostId == postId);
            if (post == null)
            {
                return NotFound();
            }

            // Check file size
            if (postImage.Length > _fileSizeLimit)
            {
                ModelState.AddModelError("PostImage", "File size exceeds 3 MB.");
                return RedirectToAction("Edit", new { id = postId });
            }

            // Check file extension
            var fileExtension = Path.GetExtension(postImage.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(fileExtension) || !_permittedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("PostImage", "Invalid file type. Only .jpg, .jpeg, and .png are allowed.");
                return RedirectToAction("Edit", new { id = postId });
            }

            // Define the path for the post's image
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/lib/img/PostImage", postId.ToString());
            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            }

            // Generate a unique file name to avoid conflicts
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploads, fileName);

            // Save the new post image
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await postImage.CopyToAsync(fileStream);
            }

            // Update the post's image path
            post.PostImage = fileName;
            _PostRepository.UpdatePost(post);

            // Redirect to the edit page of the post
            return RedirectToAction("Edit", new { id = postId });
        }
        

    }
}
