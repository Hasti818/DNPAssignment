using FileRepositories;
using RepositoryContracts;
using CLI.UI;
IUserRepository userRepository = new UserFileRepository();
IPostRepository postRepository = new PostFileRepository();
ICommentRepository commentRepository = new CommentFileRepository();
IPostVoteRepository postVoteRepository = new PostVoteFileRepository();
ICommentVoteRepository commentVoteRepository = new CommentVoteFileRepository();
CliApp app = new CliApp(
    userRepository,
    postRepository,
    commentRepository,
    postVoteRepository,
    commentVoteRepository
);
await app.StartAsync();