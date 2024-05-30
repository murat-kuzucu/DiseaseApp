using DiseaseApp.Entity;

namespace DiseaseApp.Models

{
    public class PostsViewModel
    {
        public List<Post> Posts { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}