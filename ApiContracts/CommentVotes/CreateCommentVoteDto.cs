namespace ApiContracts.CommentVotes;

public class CreateCommentVoteDto
{
    public int UserId { get; set; }
    public bool IsLike { get; set; }
}