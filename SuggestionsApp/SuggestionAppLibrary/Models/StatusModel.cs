namespace SuggestionAppLibrary.Models;

public class StatusModel
{
   [BsonId]
   [BsonRepresentation(BsonType.ObjectId)]
   public string Id { get; set; }
   public string StautsName { get; set; }
   public string StautsDescription { get; set; }
}
