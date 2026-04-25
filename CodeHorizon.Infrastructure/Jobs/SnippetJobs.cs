using CodeHorizon.Application.Interfaces;
using CodeHorizon.Application.Jobs;
using Microsoft.Extensions.Logging;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeHorizon.Infrastructure.Jobs
{
    public class SnippetJobs : ISnippetJobs
    {
        private readonly ISnippetRepository _snippetRepository;
        private readonly ILogger<SnippetJobs> _logger;

        public SnippetJobs(ISnippetRepository snippetRepository, ILogger<SnippetJobs> logger)
        {
            _snippetRepository = snippetRepository;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 2)]
        public async Task CleanupOldSnippetsAsync()
        {
            _logger.LogInformation("Starting cleanup of old snippets");
            // we can implement logic to archive/delete old snippets
            await Task.CompletedTask;
        }

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] {60, 120, 300})]
        public async Task GenerateSnippetPreviewAsync(Guid snippetId)
        {
            try
            {
                _logger.LogInformation("Generating preview for snippet {SnippetId}", snippetId);

                var snippet = await _snippetRepository.GetByIdAsync(snippetId);
                if(snippet == null)
                {
                    _logger.LogWarning("Snippet {SnippetId} not found", snippetId);

                    return;
                }

                // Generate a clean preview (remove extra whitespace, first 200 chars)

                var cleanContent = System.Text.RegularExpressions.Regex.Replace(
                    snippet.Content,
                    @"\s+",
                    " ").Trim();

                snippet.CodePreview = cleanContent.Length > 200
                    ? cleanContent.Substring(0, 200) + "..."
                    : cleanContent;

                await _snippetRepository.UpdateAsync(snippet);

                _logger.LogInformation("Preview generated for snippet {SnippetId}", snippetId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating preview for snippet {SnippetId}", snippetId);
                throw;
            }
        }

        public async Task SendSnippetCreatedNotificationAsync(Guid snippetId, Guid authorId)
        {
            _logger.LogInformation("Sending notification for new snippet {SnippetId} from user {AuthorId}",
               snippetId, authorId);

            // Here you would integrate with email service, push notifications, etc.
            await Task.CompletedTask;
        }

        public async Task UpdateSnippetStatisticsAsync(Guid snippetId)
        {
            try
            {
                var snippet = await _snippetRepository.GetByIdAsync(snippetId);
                if (snippet == null) return;

                // can be extended with more metrics
                _logger.LogInformation("Updated statistics for snippet {SnippetId}", snippetId);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating statistics for snippet {SnippetId}", snippetId);
            }
        }
    }
}
