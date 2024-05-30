using DiseaseApp.Entity;

namespace DiseaseApp.Data.Abstract
{
    public interface ICommentRepository{
        IQueryable<Comment> Comments{get;}

        void CreateComment(Comment Comment);

    }
}