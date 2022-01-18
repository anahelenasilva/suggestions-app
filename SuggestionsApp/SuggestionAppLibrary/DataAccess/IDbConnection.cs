using MongoDB.Driver;

namespace SuggestionAppLibrary.DataAccess;

public interface IDbConnection
{
   IMongoCollection<CategoryModel> CategoryCollection { get; }
   string CategoryColletionName { get; }
   MongoClient Client { get; }
   string DbName { get; }
   IMongoCollection<StatusModel> StatusCollection { get; }
   string StatusColletionName { get; }
   IMongoCollection<SuggestionModel> SuggestionCollection { get; }
   string SuggestionColletionName { get; }
   IMongoCollection<UserModel> UserCollection { get; }
   string UserColletionName { get; }
}