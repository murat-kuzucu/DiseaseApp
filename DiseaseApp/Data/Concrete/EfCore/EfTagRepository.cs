using DiseaseApp.Concrete.EfCore;
using DiseaseApp.Data.Abstract;
using DiseaseApp.Entity;
namespace DiseaseApp.Data.Concrete.EfCore
{
    public class EfTagRepository : ITagRepository
    {
        private DiseaseContext _context;
        public EfTagRepository(DiseaseContext context)
        {
            _context = context;
        }
        public IQueryable<Tag> Tags => _context.Tags;

        public void CreateTag(Tag Tag)
        {
            _context.Tags.Add(Tag);
            _context.SaveChanges();
        }
    }
}