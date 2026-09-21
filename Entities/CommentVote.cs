namespace Entities;

public class CommentVote
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }
    public int CommentId { get; set; }
    public bool IsLike { get; set; }
}