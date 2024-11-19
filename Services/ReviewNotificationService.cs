using Microsoft.Extensions.Hosting;
using EbbinghausFlashcardApp.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using EbbinghausFlashcardApp.Data;
/*
 * Acknowledgement: This code takes reference from the following source:
 * @reference: https://learn.microsoft.com/en-us/dotnet/api/system.idisposable?view=net-8.0
 * @reference: https://www.geeksforgeeks.org/c-sharp-program-to-implement-idisposable-interface/
 * @reference: https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting?view=net-8.0-pp
 * @reference: https://learn.microsoft.com/en-us/aspnet/core/signalr/background-services?view=aspnetcore-8.0
 * @reference: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio
 */
namespace EbbinghausFlashcardApp.Services
{
    public class ReviewNotificationService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ReviewNotificationService(IServiceScopeFactory scopeFactory, IHubContext<NotificationHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CheckFlashcardSetForReview, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private async void CheckFlashcardSetForReview(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                TimeSpan gracePeriod = TimeSpan.FromMinutes(1);
                DateTime cutoffTime = DateTime.UtcNow.Subtract(gracePeriod);
                var flashcardSets = await context.FlashcardSets
                    .Include(f => f.Flashcards).Where(f => f.NextReviewDate <= cutoffTime).ToListAsync();
                
                foreach (var flashcardSet in flashcardSets)
                {
                    await _hubContext.Clients.All.SendAsync("AddFlashcardSet", flashcardSet.Id, flashcardSet.Name);
                    await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", $"Flashcard set '{flashcardSet.Name}' is now ready for review.");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
