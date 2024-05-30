using DiseaseApp.Entity;

namespace DiseaseApp.Data.Abstract
{
    public interface IAdminRepository
    {
        IQueryable<User> GetUsers();
        IQueryable<Post> GetPosts();
        IQueryable<Comment> GetComments();

        void CreateUser(User user);

        void DeleteUser(int UserId);

        void DeleteComment(int CommentId);

    }
}