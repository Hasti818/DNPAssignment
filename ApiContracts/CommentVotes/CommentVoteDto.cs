namespace ApiContracts.CommentVotes;

public class CommentVoteDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CommentId { get; set; }
    public bool IsLike { get; set; }
}