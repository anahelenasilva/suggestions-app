
namespace SuggestionAppLibrary.DataAccess;

public interface ISuggestionData
{
   Task CreateSuggestion(SuggestionModel suggestion);
   Task<List<SuggestionModel>> GetAllAppprovedSuggestions();
   Task<List<SuggestionModel>> GetAllSuggestions();
   Task<List<SuggestionModel>> GetAllSuggestionsWaitingForApproval();
   Task<List<SuggestionModel>> GetUsersSuggestions(string userId);
   Task<SuggestionModel> GetSuggestion(string id);
   Task UpdateSuggestion(SuggestionModel suggestion);
   Task UpvoteSuggestion(string suggestionId, string userId);
}