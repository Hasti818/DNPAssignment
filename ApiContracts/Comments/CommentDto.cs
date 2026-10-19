namespace ApiContracts.Comments;

public class CommentDto
{
    public int Id { get; set; }
    public string Body { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public int PostId { get; set; }
    public DateTime CreatedAt { get; set; } 
}