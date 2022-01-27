using SuggestionsAppUI.Models;

namespace SuggestionsAppUI.Pages
{
   public partial class Create
    {
        private CreateSuggestionModel suggestion = new();
        private List<CategoryModel> categories;
        private UserModel loggedInUser;
        protected async override Task OnInitializedAsync()
        {
            categories = await categoryData.GetAllCategories();
            loggedInUser = await authProvider.GetUserFromAuth(userData);
        }

        private void ClosePage()
        {
            navManager.NavigateTo("/");
        }

        private async Task CreateSuggestion()
        {
            SuggestionModel s = new()
            {Suggestion = suggestion.Suggestion, Description = suggestion.Description, Category = categories.FirstOrDefault(c => c.Id == suggestion.CategoryId), Author = new BasicUserModel(loggedInUser)};
            if (s.Category is null)
            {
                suggestion.CategoryId = "";
                return;
            }

            await suggestionData.CreateSuggestion(s);
            suggestion = new();
            ClosePage();
        }
    }
}