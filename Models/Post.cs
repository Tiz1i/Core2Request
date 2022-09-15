using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Core2Request.Models;
#pragma warning disable CS8618


public class Post {

[Key]
public int PostId { get;set; }
public string Description { get;set; }

public int UserId { get; set; }
public int CommentId { get; set; }
public int LikeId { get; set; }
public List<Like> Likes { get; set; } = new List<Like>();

public User? Creator { get; set; }
public DateTime CreatedAt { get; set; } = DateTime.Now;
public DateTime UpdatedAt { get; set; } = DateTime.Now;

}