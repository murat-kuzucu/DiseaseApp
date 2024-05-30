using DiseaseApp.Concrete.EfCore;
using DiseaseApp.Data.Abstract;
using DiseaseApp.Entity;
using Microsoft.EntityFrameworkCore;
namespace DiseaseApp.Data.Concrete.EfCore
{
    public class EfPostRepository : IPostRepository
    {
        private DiseaseContext _context;
        public EfPostRepository(DiseaseContext context)
        {
            _context = context;
        }
        public IQueryable<Post> Posts => _context.Posts;

        public void CreatePost(Post post)
        {
            _context.Posts.Add(post);
            _context.SaveChanges();
        }

        public void EditPost(Post post)
        {
            var entity = _context.Posts.FirstOrDefault(p => p.PostId == post.PostId);
            
            if(entity != null)
            {
                entity.Title = post.Title;
                entity.Content = post.Content;
                entity.Description = post.Description;
                entity.IsActive = post.IsActive;
                entity.Url = post.Url;
            };
            _context.SaveChanges();
        }

        public void EditPost(Post post, int[] TagIds)
        {
            var entity = _context.Posts.Include(p => p.Tags).FirstOrDefault(p => p.PostId == post.PostId);

            if (entity != null)
            {
                entity.Title = post.Title;
                entity.Content = post.Content;
                entity.Description = post.Description;
                entity.IsActive = post.IsActive;
                entity.Url = post.Url;

                // Ensure TagIds is not null
                TagIds ??= Array.Empty<int>();

                // Update tags
                entity.Tags.Clear();
                if (TagIds.Length > 0)
                {
                    entity.Tags = _context.Tags.Where(tag => TagIds.Contains(tag.TagId)).ToList();
                }

                _context.SaveChanges();
            }
        }

        public void DeletePost(int postId, int userId, bool isAdmin)
        {
            var entity = _context.Posts.Include(p => p.Comments).FirstOrDefault(p => p.PostId == postId);
            if (entity != null)
            {
                if (isAdmin || entity.UserId == userId)
                {
                    // Delete related comments first
                    _context.Comments.RemoveRange(entity.Comments);

                    // Then delete the post
                    _context.Posts.Remove(entity);
                    _context.SaveChanges();
                }
            }
        }

        public void UpdatePost(Post post)
        {
            _context.Posts.Update(post);
            _context.SaveChanges();
        }

        

    }
}