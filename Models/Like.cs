using System.ComponentModel.DataAnnotations;
namespace Core2Request.Models;
#pragma warning disable CS8618


public class Like{
    [Key]
    public int LikeId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
   
    public int PostId { get; set; }
   
    public User? UseriQePelqen { get; set;} 
    public Post? PostiQePelqehet { get; set;}

    

}