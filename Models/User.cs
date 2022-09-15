#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace Core2Request.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{	
    [Key]
    public int UserId { get; set; }

    [Display(Name = "First Name")]
    [Required]
    [MinLength(2)]
    
    public string Firstname { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    [MinLength(2)]
      
    public string Lastname { get; set; }

    [Required]
    [Display(Name = "Username")]
    [MinLength(3)]
    [MaxLength(15)]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password must be 8 characters or longer!")]
    public string Password { get; set; }

    [NotMapped]
    [Compare("Password")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Pw")]
    public string ConfirmPassword { get; set;}
    public User? CreatedPost { get; set;}
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

   public List<Request> Requests {get;set;} = new List<Request>();
    
    public List<Post> Posts {get;set;} = new List<Post>();
    
    public List<Comment> Comments { get; set; } = new List<Comment>();
    public List<Request> ReciverRequests {get;set;} = new List<Request>();
    public List<Request> SenderRequests {get;set;} = new List<Request>();

    public List<Request> Sender {get;set;} = new List<Request>();
    public List<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();
}

    public class LoginUser
{
    // No other fields!
    [Required]
    public string Username { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
}
