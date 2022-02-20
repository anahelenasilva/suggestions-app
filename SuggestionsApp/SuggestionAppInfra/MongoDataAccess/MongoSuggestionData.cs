using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using SuggestionAppLibrary.DataAccess;
using SuggestionAppLibrary.Models;

namespace SuggestionAppInfra.MongoDataAccess;

public class MongoSuggestionData : ISuggestionData
{
   private readonly IDbConnection _db;
   private readonly IUserData _userData;
   private readonly IMemoryCache _cache;
   private readonly IMongoCollection<SuggestionModel> _suggestions;
   private readonly string CacheName = "SuggestionData";

   public MongoSuggestionData(IDbConnection db, IUserData userData, IMemoryCache cache)
   {
      _db = db;
      _userData = userData;
      _cache = cache;

      _suggestions = _db.SuggestionCollection;
   }

   public async Task<List<SuggestionModel>> GetAllSuggestions()
   {
      var output = _cache.Get<List<SuggestionModel>>(CacheName);

      if (output is not null)
      {
         return output;
      }

      var results = await _suggestions.FindAsync(s => s.Archived == false);
      output = results.ToList();

      _cache.Set(CacheName, output, TimeSpan.FromMinutes(1));

      return output;
   }

   public async Task<List<SuggestionModel>> GetAllAppprovedSuggestions()
   {
      var output = await GetAllSuggestions();
      return output.Where(s => s.ApprovedForReleased).ToList();
   }

   public async Task<SuggestionModel> GetSuggestion(string id)
   {
      var results = await _suggestions.FindAsync(s => s.Id == id);
      return results.FirstOrDefault();
   }

   public async Task<List<SuggestionModel>> GetAllSuggestionsWaitingForApproval()
   {
      var output = await GetAllSuggestions();
      return output.Where(s => s.ApprovedForReleased == false && s.Rejected == false).ToList();
   }

   public async Task UpdateSuggestion(SuggestionModel suggestion)
   {
      await _suggestions.ReplaceOneAsync(s => s.Id == suggestion.Id, suggestion);
      _cache.Remove(CacheName);
   }

   public async Task UpvoteSuggestion(string suggestionId, string userId)
   {
      var client = _db.Client;

      using var session = await client.StartSessionAsync();

      session.StartTransaction();

      try
      {
         var db = client.GetDatabase(_db.DbName);
         var suggestionsInTransation = db.GetCollection<SuggestionModel>(_db.SuggestionColletionName);
         var suggestion = (await suggestionsInTransation.FindAsync(s => s.Id == suggestionId)).First();

         bool isUpvote = suggestion.UserVotes.Add(userId);
         if (isUpvote == false)
         {
            suggestion.UserVotes.Remove(userId);
         }

         await suggestionsInTransation.ReplaceOneAsync(session, s => s.Id == suggestionId, suggestion);

         var usersInTransaction = db.GetCollection<UserModel>(_db.UserColletionName);
         var user = await _userData.GetUserAsync(userId);

         if (isUpvote)
         {
            user.VotedOnSuggestions.Add(new BasicSuggestionModel(suggestion));
         }
         else
         {
            var suggestionToRemove = user.VotedOnSuggestions.Where(s => s.Id == suggestionId).First();
            user.VotedOnSuggestions.Remove(suggestionToRemove);
         }

         await usersInTransaction.ReplaceOneAsync(session, u => u.Id == userId, user);

         await session.CommitTransactionAsync();

         _cache.Remove(CacheName);
      }
      catch (Exception ex)
      {
         //TODO add log
         await session.AbortTransactionAsync();
         throw;
      }
   }

   public async Task CreateSuggestion(SuggestionModel suggestion)
   {
      var client = _db.Client;

      using var session = await client.StartSessionAsync();

      session.StartTransaction();

      try
      {
         var db = client.GetDatabase(_db.DbName);
         var suggestionsInTransation = db.GetCollection<SuggestionModel>(_db.SuggestionColletionName);
         await suggestionsInTransation.InsertOneAsync(session,suggestion);

         var usersInTransaction = db.GetCollection<UserModel>(_db.UserColletionName);
         var user = await _userData.GetUserAsync(suggestion.Author.Id);
         user.AuthoredSuggestions.Add(new BasicSuggestionModel(suggestion));
         await usersInTransaction.ReplaceOneAsync(session, u => u.Id == user.Id, user);

         await session.CommitTransactionAsync();
      }
      catch (Exception ex)
      {
         //TODO add log
         await session.AbortTransactionAsync();
         throw;
      }
   }

   public async Task<List<SuggestionModel>> GetUsersSuggestions(string userId)
   {
      var output = _cache.Get<List<SuggestionModel>>(userId);
      if (output is null)
      {
         var results = await _suggestions.FindAsync(s => s.Author.Id == userId);
         output = results.ToList();

         _cache.Set(userId, output, TimeSpan.FromMinutes(1));
      }

      return output;
   }
}
