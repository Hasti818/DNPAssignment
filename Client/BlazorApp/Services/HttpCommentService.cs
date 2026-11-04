using System.Net.Http.Json;
using System.Text.Json;
using ApiContracts.Comments;

namespace BlazorApp.Services;

public class HttpCommentService : ICommentService
{
    private readonly HttpClient client;

    public HttpCommentService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<CommentDto> AddCommentAsync(
        int postId,
        CreateCommentDto request)
    {
        HttpResponseMessage httpResponse =
            await client.PostAsJsonAsync(
                $"api/posts/{postId}/comments",
                request);

        string response =
            await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<CommentDto>(
            response,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
    }

    public async Task<List<CommentDto>> GetCommentsForPostAsync(int postId)
    {
        HttpResponseMessage httpResponse =
            await client.GetAsync($"api/comments?postId={postId}");

        string response =
            await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<List<CommentDto>>(
            response,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
    }
}