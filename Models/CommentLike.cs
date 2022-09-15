using System.ComponentModel.DataAnnotations;
namespace Core2Request.Models;
#pragma warning disable CS8618

public class CommentLike{
    [Key]
    public int CommentId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int PostId { get; set;}

    public int LikeId { get; set; }

    public User? UseriQePelqen { get; set;} 

    public Post? PostiQePelqehet { get; set;}

    

}