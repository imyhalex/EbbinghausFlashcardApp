using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

/*
 * Acknmowledgement: This code is based on the following part: "SignalR" from the following reference.
 * @reference: https://learn.microsoft.com/en-us/aspnet/core/blazor/tutorials/signalr-blazor?view=aspnetcore-8.0&tabs=visual-studio-code
 */
namespace EbbinghausFlashcardApp.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task AddFlashcardSetToReviewList(int flashcardSetId, string flashcardSetName)
        {
            await Clients.All.SendAsync("AddFlashcardSet", flashcardSetId, flashcardSetName);
        }

        public async Task RemoveFlashcardSetFromReviewList(int flashcardSetId)
        {
            await Clients.All.SendAsync("RemoveFlashcardSet", flashcardSetId);
        }
    }
}
