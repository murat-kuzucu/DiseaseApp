using System.ComponentModel.DataAnnotations;
using DiseaseApp.Entity;

namespace DiseaseApp.Models
{
    public class PostCreateViewModel
    {
        
        public int PostId { get; set; } // Id of the post
        
        [Required]
        [Display(Name = "Title")]
        public string? Title { get; set; } // Title of the post

        [Display(Name = "Content")]
        public string? Content { get; set; } // Content of the post

        [Required]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Tags")]
        public List <Tag> Tags { get; set; } = new();
        
        public IFormFile? PostImage { get; set; } // Image of the post
    }
}
