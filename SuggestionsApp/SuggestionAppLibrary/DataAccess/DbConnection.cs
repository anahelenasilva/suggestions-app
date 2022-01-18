using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace SuggestionAppLibrary.DataAccess;

public class DbConnection : IDbConnection
{
   private readonly IConfiguration _config;
   private readonly IMongoDatabase _db;
   private string _connectionId = "MongoDB";
   public string DbName { get; private set; }
   public string CategoryColletionName { get; private set; } = "categories";
   public string StatusColletionName { get; private set; } = "statuses";
   public string UserColletionName { get; private set; } = "users";
   public string SuggestionColletionName { get; private set; } = "suggestions";

   public MongoClient Client { get; private set; }
   public IMongoCollection<CategoryModel> CategoryCollection { get; private set; }
   public IMongoCollection<StatusModel> StatusCollection { get; private set; }
   public IMongoCollection<UserModel> UserCollection { get; private set; }
   public IMongoCollection<SuggestionModel> SuggestionCollection { get; private set; }

   public DbConnection(IConfiguration config)
   {
      _config = config;
      Client = new MongoClient(_config.GetConnectionString(_connectionId));
      DbName = _config["DatabaseName"];
      _db = Client.GetDatabase(DbName);

      CategoryCollection = _db.GetCollection<CategoryModel>(CategoryColletionName);
      StatusCollection = _db.GetCollection<StatusModel>(StatusColletionName);
      UserCollection = _db.GetCollection<UserModel>(UserColletionName);
      SuggestionCollection = _db.GetCollection<SuggestionModel>(SuggestionColletionName);
   }
}
