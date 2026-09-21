using RepositoryContracts;
using Entities;

namespace CLI.UI;

public class CliApp
{
    private readonly IUserRepository userRepository;
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IPostVoteRepository postVoteRepository;
    private readonly ICommentVoteRepository commentVoteRepository;

    public CliApp(IUserRepository userRepository , IPostRepository postRepository, ICommentRepository commentRepository
    , IPostVoteRepository postVoteRepository, ICommentVoteRepository commentVoteRepository)
    {
        this.userRepository = userRepository;
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
        this.postVoteRepository = postVoteRepository;
        this.commentVoteRepository = commentVoteRepository;
    }
    private async Task CreateUserAsync()
    {
        Console.Write("Enter username: ");
        string? userName = Console.ReadLine();
        Console.Write("Enter password: ");
        string? password = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Username and password cannot be empty.");
            return;
        }
        bool userExists = userRepository
            .GetMany()
            .Any(user => user.UserName == userName);

        if (userExists)
        {
            Console.WriteLine("Username is already taken.");
            return;
        }
        User user = new User
        {
            UserName = userName,
            Password = password
        };
        User createdUser = await userRepository.AddAsync(user);
        Console.WriteLine($"User created successfully with ID {createdUser.Id}");
    }

    private async Task CreatePostAsync()
    {
        Console.Write("Enter post title  ");
        string? title = Console.ReadLine();
        Console.Write("Enter post body");
        string?  body = Console.ReadLine();
        Console.WriteLine("Enter user id");
        string? userIdInput = Console.ReadLine();
        if (!int.TryParse(userIdInput, out int userId))
        {
            Console.WriteLine("Invalid user ID.");
            return;
        }
        bool userExists= userRepository.GetMany().Any(user => user.Id == userId);
        if (!userExists)
        {
            Console.WriteLine("User not found.");
            return;
        }

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(body))
        {
            Console.WriteLine("Title and body cannot be empty.");
            return;
        }

        Post post = new Post
        {
            Title = title,
            Body = body,
            UserId = userId,
            CreatedAt = DateTime.Now
        };
        Post createdPost = await postRepository.AddAsync(post);

        Console.WriteLine($"Post created successfully with ID {createdPost.Id}");
    }

    private async Task CreateCommentAsync()
    {
        Console.Write("Enter comment body");
        string?body = Console.ReadLine();
        Console.Write("Enter user ID");
        string? userIdInput = Console.ReadLine();
        if (!int.TryParse(userIdInput, out int userId)){
            Console.WriteLine("Invalid user ID.");
            return;
        }
        bool userExists= userRepository.GetMany().Any(user => user.Id == userId);
        if (!userExists){
            Console.WriteLine("User not found.");
            return;
        }

        Console.WriteLine(" Enter post Id");
        string? postIdInput = Console.ReadLine();
        if (!int.TryParse(postIdInput, out int postId))
        {
            Console.WriteLine("Invalid post ID.");
            return;
        }
        bool postExists= postRepository.GetMany().Any(post => post.Id == postId);
        if (!postExists)
        {
            Console.WriteLine("Post not found.");
            return;
        }

        Comment comment = new Comment
        {
            Body = body,
            UserId = userId,
            PostId = postId,
            CreatedAt = DateTime.Now
        };
        Comment createdComment =
            await commentRepository.AddAsync(comment);

        Console.WriteLine(
            $"Comment created successfully with ID {createdComment.Id}");
    }
    private void ViewPosts()
    {
        var posts = postRepository.GetMany();

        if (!posts.Any())
        {
            Console.WriteLine("No posts found.");
            return;
        }

        Console.WriteLine("Posts:");

        foreach (Post post in posts)
        {
            Console.WriteLine($"[{post.Id}] {post.Title}");
        }
    }

    private async Task ViewPostAsync()
    {
        Console.Write("Enter post ID: ");
        string? postIdInput = Console.ReadLine();

        if (!int.TryParse(postIdInput, out int postId))
        {
            Console.WriteLine("Invalid post ID.");
            return;
        }

        Post post;

        try
        {
            post = await postRepository.GetSingleAsync(postId);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("Post not found.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"Title: {post.Title}");
        Console.WriteLine($"Body: {post.Body}");
        Console.WriteLine($"Created at: {post.CreatedAt}");

        var comments = commentRepository
            .GetMany()
            .Where(comment => comment.PostId == postId);

        Console.WriteLine();
        Console.WriteLine("Comments:");

        if (!comments.Any())
        {
            Console.WriteLine("No comments yet.");
            return;
        }

        foreach (Comment comment in comments)
        {
            Console.WriteLine(
                $"[{comment.Id}] User {comment.UserId}: {comment.Body}");
        }
    }
    private async Task VoteOnPostAsync()
    {
        Console.Write("Enter user ID: ");
        string? userIdInput = Console.ReadLine();

        if (!int.TryParse(userIdInput, out int userId))
        {
            Console.WriteLine("Invalid user ID.");
            return;
        }

        bool userExists = userRepository
            .GetMany()
            .Any(user => user.Id == userId);

        if (!userExists)
        {
            Console.WriteLine("User not found.");
            return;
        }

        Console.Write("Enter post ID: ");
        string? postIdInput = Console.ReadLine();

        if (!int.TryParse(postIdInput, out int postId))
        {
            Console.WriteLine("Invalid post ID.");
            return;
        }

        bool postExists = postRepository
            .GetMany()
            .Any(post => post.Id == postId);

        if (!postExists)
        {
            Console.WriteLine("Post not found.");
            return;
        }

        Console.WriteLine("1. Like");
        Console.WriteLine("2. Dislike");
        Console.Write("Choose: ");

        string? voteChoice = Console.ReadLine();

        bool isLike;

        if (voteChoice == "1")
        {
            isLike = true;
        }
        else if (voteChoice == "2")
        {
            isLike = false;
        }
        else
        {
            Console.WriteLine("Invalid choice.");
            return;
        }

        PostVote? existingVote = postVoteRepository
            .GetMany()
            .SingleOrDefault(vote =>
                vote.UserId == userId &&
                vote.PostId == postId);

        if (existingVote is not null)
        {
            existingVote.IsLike = isLike;

            await postVoteRepository.UpdateAsync(existingVote);

            Console.WriteLine("Vote updated successfully.");
            return;
        }

        PostVote postVote = new PostVote
        {
            UserId = userId,
            PostId = postId,
            IsLike = isLike
        };

        await postVoteRepository.AddAsync(postVote);

        Console.WriteLine("Vote added successfully.");
    }
    private async Task VoteOnCommentAsync()
    {
        Console.Write("Enter user ID: ");
        string? userIdInput = Console.ReadLine();

        if (!int.TryParse(userIdInput, out int userId))
        {
            Console.WriteLine("Invalid user ID.");
            return;
        }

        bool userExists = userRepository
            .GetMany()
            .Any(user => user.Id == userId);

        if (!userExists)
        {
            Console.WriteLine("User not found.");
            return;
        }

        Console.Write("Enter comment ID: ");
        string? commentIdInput = Console.ReadLine();

        if (!int.TryParse(commentIdInput, out int commentId))
        {
            Console.WriteLine("Invalid comment ID.");
            return;
        }

        bool commentExists = commentRepository
            .GetMany()
            .Any(comment => comment.Id == commentId);

        if (!commentExists)
        {
            Console.WriteLine("Comment not found.");
            return;
        }

        Console.WriteLine("1. Like");
        Console.WriteLine("2. Dislike");
        Console.Write("Choose: ");

        string? voteChoice = Console.ReadLine();

        bool isLike;

        if (voteChoice == "1")
        {
            isLike = true;
        }
        else if (voteChoice == "2")
        {
            isLike = false;
        }
        else
        {
            Console.WriteLine("Invalid choice.");
            return;
        }

        CommentVote? existingVote = commentVoteRepository
            .GetMany()
            .SingleOrDefault(vote =>
                vote.UserId == userId &&
                vote.CommentId == commentId);

        if (existingVote is not null)
        {
            existingVote.IsLike = isLike;

            await commentVoteRepository.UpdateAsync(existingVote);

            Console.WriteLine("Vote updated successfully.");
            return;
        }

        CommentVote commentVote = new CommentVote
        {
            UserId = userId,
            CommentId = commentId,
            IsLike = isLike
        };

        await commentVoteRepository.AddAsync(commentVote);

        Console.WriteLine("Vote added successfully.");
    }

    public async Task StartAsync()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Welcome to the Forum CLI!");
            Console.WriteLine("1. Create user");
            Console.WriteLine("2. Create post");
            Console.WriteLine("3. Add comment");
            Console.WriteLine("4. View posts");
            Console.WriteLine("5. View specific post");
            Console.WriteLine("6. Like/dislike post");
            Console.WriteLine("7. Like/dislike comment");
            Console.WriteLine("0. Exit");

            Console.Write("Choose an option: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await CreateUserAsync();
                    break;

                case "2":
                    await CreatePostAsync();
                    break;

                case "3":
                    await CreateCommentAsync();
                    break;

                case "4":
                    ViewPosts();
                    break;

                case "5":
                    await ViewPostAsync();
                    break;
                case "6":
                    await VoteOnPostAsync();
                    break;
                case "7":
                    await VoteOnCommentAsync();
                    break;

                case "0":
                    Console.WriteLine("Goodbye!");
                    return;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }
    
}