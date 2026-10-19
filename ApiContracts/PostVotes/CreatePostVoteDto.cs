namespace ApiContracts.PostVotes;

public class CreatePostVoteDto
{
    public int UserId { get; set; }
    public bool IsLike { get; set; }
}