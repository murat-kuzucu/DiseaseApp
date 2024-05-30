using System.ComponentModel.DataAnnotations;
using DiseaseApp.Entity;

namespace DiseaseApp.Models
{
    public class PostEditViewModel
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

        [Required]
        [Display(Name = "IsActive")]
        public bool IsActive { get; set; } // Is the post active?

        public string? Url { get; set; } // Url of the post

        [Display(Name = "Tags")]
        public List <Tag> Tags { get; set; } = new();

        public List<int> TagIds { get; set; } = new(); // List of tag ids to select it in the view

        public IFormFile? PostImage { get; set; } // Image of the post
        
    }
}
