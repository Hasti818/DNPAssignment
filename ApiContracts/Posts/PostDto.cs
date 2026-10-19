namespace ApiContracts.Posts;

public class PostDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public int UserId{ get; set; }
    public String UserName { get; set; }
    public DateTime CreatedAt{ get; set; }
}