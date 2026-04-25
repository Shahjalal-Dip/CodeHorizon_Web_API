using System;
using System.Threading.Tasks;

namespace CodeHorizon.Application.Jobs
{
    public interface ISnippetJobs
    {
        Task GenerateSnippetPreviewAsync(Guid snippetId);
        Task UpdateSnippetStatisticsAsync(Guid snippetId);
        Task CleanupOldSnippetsAsync();
        Task SendSnippetCreatedNotificationAsync(Guid snippetId, Guid authorId);
    }
}