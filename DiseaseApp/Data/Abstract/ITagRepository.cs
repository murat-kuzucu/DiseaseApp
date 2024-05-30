using DiseaseApp.Entity;

namespace DiseaseApp.Data.Abstract
{
    public interface ITagRepository{
        IQueryable<Tag> Tags { get; }

        void CreateTag(Tag tag);
    }
}