using DiseaseApp.Concrete.EfCore;
using DiseaseApp.Data.Abstract;
using DiseaseApp.Entity;
namespace DiseaseApp.Data.Concrete.EfCore
{
    public class EfCommentRepository : ICommentRepository
    {
        private DiseaseContext _context;
        public EfCommentRepository(DiseaseContext context)
        {
            _context = context;
        }
        public IQueryable<Comment> Comments => _context.Comments;

        public void CreateComment(Comment Comment)
        {
            _context.Comments.Add(Comment);
            _context.SaveChanges();
        }
    }
}