namespace SuggestionAppLibrary.Models;

public class SuggestionModel
{
   [BsonId]
   [BsonRepresentation(BsonType.ObjectId)]
   public string Id { get; set; }
   public string Suggestion { get; set; }
   public string Description { get; set; }
   public DateTime DateCreated { get; set; } = DateTime.UtcNow;
   public CategoryModel Category { get; set; }
   public BasicUserModel Author { get; set; }
   public HashSet<string> UserVotes { get; set; } = new(); //List with unique values
   public StatusModel SuggestionStatus { get; set; }
   public string OwnerNotes { get; set; }
   public bool ApprovedForReleased { get; set; } = false;
   public bool Archived { get; set; } = false;
   public bool Rejected { get; set; } = false;
}
