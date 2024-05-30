using DiseaseApp.Data.Abstract;
using DiseaseApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiseaseApp.Entity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace DiseaseApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserRepository _UserRepository;
        //TODO: Implement the following fields
        private readonly string[] _permittedExtensions = { ".jpg", ".jpeg", ".png" }; // Permitted file extensions for profile photo
        private const long _fileSizeLimit = 2 * 1024 * 1024; // 2 MB // File size limit for profile photo

        public UsersController(IUserRepository UserRepository)
        {
            _UserRepository = UserRepository;
        }
        
        public IActionResult Login()
        {
            if(User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Posts");
            }
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            //TODO: Implement the AccessDenied method and page
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
        
        //TODO: Generate a crypt method to hash the password with sha512
        //Sha512 crypter for password
        private string HashPassword(string password)
        {
            using (var sha512 = SHA512.Create())
            {
                byte[] bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if(ModelState.IsValid){
                var user = await _UserRepository.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName || x.Email == model.Email);
                if(user == null){
                    var hashedPassword = HashPassword(model.Password ?? "");
                    _UserRepository.CreateUser(new User{
                        UserName = model.UserName,
                        Name = model.Name,
                        Email = model.Email,
                        Password = hashedPassword,
                        UserRole = model.UserRole.ToString(),
                        IsActive = true
                    });
                    return RedirectToAction("Login");
                }
                else{
                    ModelState.AddModelError("", "User already exists with the given email or username");
                    return View(model);
                } 
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var hashedPassword = HashPassword(model.Password ?? "");
                var isUser = await _UserRepository.Users.FirstOrDefaultAsync(x => x.Email == model.Email && x.Password == hashedPassword);

                if (isUser != null)
                {
                    if (!isUser.IsActive)
                    {
                        ModelState.AddModelError("", "This account is banned. If you consider it a mistake, you can send an email to admin@muratkuzucu.com");
                        return View(model);
                    }

                    var UserClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, isUser.UserId.ToString()),
                        new Claim(ClaimTypes.Name, isUser.UserName ?? ""),
                        new Claim(ClaimTypes.GivenName, isUser.Name ?? ""),
                        new Claim(ClaimTypes.Email, isUser.Email ?? ""),
                        new Claim(ClaimTypes.UserData, isUser.UserImage ?? ""),
                        new Claim(ClaimTypes.Role, isUser.UserRole.ToString() ?? "")
                    };

                    switch (isUser.UserRole)
                    {
                        case "Admin":
                            UserClaims.Add(new Claim(ClaimTypes.Role, "Admin"));
                            break;
                        case "Researcher":
                            UserClaims.Add(new Claim(ClaimTypes.Role, "Researcher"));
                            break;
                        default:
                            UserClaims.Add(new Claim(ClaimTypes.Role, "User"));
                            break;
                    }

                    var Identity = new ClaimsIdentity(UserClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var AuthProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };

                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(Identity), AuthProperties);

                    return RedirectToAction("Index", "Posts");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password");
                }
            }

            return View(model);
        }


        public IActionResult Profile(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return NotFound();
            }
            var user = _UserRepository.Users
                .Include(u => u.Posts)
                .Include(u => u.Comments)
                .ThenInclude(c => c.Post)
                .FirstOrDefault(u => u.UserName == username && u.IsActive);

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }


        
        //Userprofile photo change method
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangeProfilePhoto(IFormFile UserImage)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var user = await _UserRepository.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return NotFound();
            }

            if (UserImage == null || UserImage.Length == 0)
            {
                ModelState.AddModelError("UserImage", "File is required.");
                return RedirectToAction("Profile", new { username });
            }

            // Check file size
            if (UserImage.Length > _fileSizeLimit)
            {
                ModelState.AddModelError("UserImage", "File size exceeds 2 MB.");
                return RedirectToAction("Profile", new { username });
            }

            // Check file extension
            var fileExtension = Path.GetExtension(UserImage.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(fileExtension) || !_permittedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("UserImage", "Invalid file type. Only .jpg, .jpeg, and .png are allowed.");
                return RedirectToAction("Profile", new { username });
            }

            // the user is only changing their own photo
            if (user.UserName != username)
            {
                return Unauthorized();
            }

            // Define the path for the user's profile image
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/lib/img/UserImage", user.UserName ?? "");
            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            }

            // Generate a unique file name to avoid conflicts
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploads, fileName);

            // Save the new profile image
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await UserImage.CopyToAsync(fileStream);
            }

            // Update the user's profile image path
            user.UserImage = fileName;
            _UserRepository.UpdateUser(user);

            // Redirect to the profile page
            return RedirectToAction("Profile", new { username });
        }



    }
}
