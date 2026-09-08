namespace Entities;

public class PostVote
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }

    public bool IsLiked{ get; set; }
    
}