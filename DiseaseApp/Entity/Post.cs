using System.ComponentModel.DataAnnotations;

namespace DiseaseApp.Entity;

public class Post{

    [Key]
    public int PostId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Content { get; set; }

    public string? Url { get; set; }

    public string? PostImage { get; set; }

    public DateTime PublishedOn { get; set; }

    public bool IsActive { get; set; }

    public int UserId { get; set; }
    
    public User User { get; set; } = null!;
    public List<Comment> Comments { get; set; } = new List<Comment>();

    public List<Tag> Tags { get; set; } = new List<Tag>();

}