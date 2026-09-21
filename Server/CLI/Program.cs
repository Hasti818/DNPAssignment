using InMemoryRepositories;
using RepositoryContracts;
using CLI.UI;
IUserRepository userRepository= new UserInMemoryRepository();
IPostRepository postRepository= new PostInMemoryRepository();
ICommentRepository commentRepository = new CommentInMemoryRepository();
IPostVoteRepository postVoteRepository = new PostVoteInMemoryRepository();
ICommentVoteRepository commentVoteRepository = new CommentVoteInMemoryRepository();
CliApp app = new CliApp(
    userRepository,
    postRepository,
    commentRepository,
    postVoteRepository,
    commentVoteRepository
);
await app.StartAsync();