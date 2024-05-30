using System.ComponentModel.DataAnnotations;

namespace DiseaseApp.Entity;

public class User{

    [Key]
    public int UserId { get; set; }

    public string UserRole { get; set; } = "";
    public string? UserName { get; set; }

    public string? UserImage { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public DateTime RegisteredOn { get; set; } = DateTime.Now;

    public bool IsActive { get; set; }

    public List<Post> Posts { get; set; } = new List<Post>();

    public List<Tag> Tags { get; set; } = new List<Tag>();

    public List<Comment> Comments { get; set; } = new List<Comment>();

}