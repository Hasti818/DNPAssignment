namespace ApiContracts.PostVotes;

public class PostVoteDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PostId { get; set; }
    public bool IsLike { get; set; }
}