using DiseaseApp.Data.Abstract;
using DiseaseApp.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DiseaseApp.Data.Concrete.EfCore
{
    public class EfAdminRepository : IAdminRepository
    {
        private readonly DiseaseContext _context;

        public EfAdminRepository(DiseaseContext context)
        {
            _context = context;
        }

        public IQueryable<User> GetUsers()
        {
            return _context.Users.AsQueryable();
        }

        public IQueryable<Post> GetPosts()
        {
            return _context.Posts.Include(p => p.User).AsQueryable();
        }

        public IQueryable<Comment> GetComments()
        {
            return _context.Comments.Include(c => c.User).Include(c => c.Post).AsQueryable();
        }

        public void CreateUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void DeleteUser(int UserId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == UserId);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
        public void DeleteComment(int CommentId)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.CommentId == CommentId);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                _context.SaveChanges();
            }
        }
    }
}
