using DiseaseApp.Entity;

namespace DiseaseApp.Data.Abstract
{
    public interface IPostRepository
    {
        IQueryable<Post> Posts { get; }

        void CreatePost(Post post);

        void EditPost(Post post,int[] TagIds);

        public void UpdatePost(Post post);
        void DeletePost(int postId, int userId, bool isAdmin);
    }
}