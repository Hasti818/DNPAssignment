using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class CommentVoteInMemoryRepository: ICommentVoteRepository
{
   private readonly List<CommentVote> commentVotes = new();
   public CommentVoteInMemoryRepository()
   {
      commentVotes.Add(new CommentVote
      {
         Id = 1,
         UserId = 1,
         CommentId = 1,
         IsLike = true
      });

      commentVotes.Add(new CommentVote
      {
         Id = 2,
         UserId = 2,
         CommentId = 2,
         IsLike = true
      });

      commentVotes.Add(new CommentVote
      {
         Id = 3,
         UserId = 3,
         CommentId = 3,
         IsLike = false
      });
   }
   public Task<CommentVote> AddAsync(CommentVote commentVote)
   {
      commentVote.Id = commentVotes.Any()
         ? commentVotes.Max(vote => vote.Id) + 1
         : 1;

      commentVotes.Add(commentVote);

      return Task.FromResult(commentVote);
   }

   public Task UpdateAsync(CommentVote commentVote)
   {
      CommentVote? existingCommentVote =
         commentVotes.SingleOrDefault(vote => vote.Id == commentVote.Id);

      if (existingCommentVote is null)
      {
         throw new InvalidOperationException(
            $"CommentVote with ID '{commentVote.Id}' not found");
      }

      commentVotes.Remove(existingCommentVote);
      commentVotes.Add(commentVote);

      return Task.CompletedTask;
   }

   public Task DeleteAsync(int id)
   {
      CommentVote? commentVoteToRemove= 
         commentVotes.SingleOrDefault(vote => vote.Id == id);
      if(commentVoteToRemove is null){
         throw new InvalidOperationException(
            $"CommentVote with ID '{id}' not found");
      }
      commentVotes.Remove(commentVoteToRemove);
      return Task.CompletedTask;
   }

   public Task<CommentVote> GetSingleAsync(int id)
   {
      CommentVote? commentVote =
         commentVotes.SingleOrDefault(vote => vote.Id == id);

      if (commentVote is null)
      {
         throw new InvalidOperationException(
            $"CommentVote with ID '{id}' not found");
      }

      return Task.FromResult(commentVote);
   }

   public IQueryable<CommentVote> GetMany()
   {
      return commentVotes.AsQueryable();
   }
}