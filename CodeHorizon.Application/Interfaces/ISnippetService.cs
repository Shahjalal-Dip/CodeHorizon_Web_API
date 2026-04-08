using CodeHorizon.Application.DTOs;
using CodeHorizon.Application.DTOs.Snippet;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeHorizon.Application.Interfaces
{
    public interface ISnippetService
    {
        Task<SnippetResponseDto> GetByIdAsync(Guid id, Guid? currentUserId);
        Task<PagedResultDto<SnippetResponseDto>> GetAllFilteredAsync(SnippetFilterDto filter, int page, int pageSize, Guid? currentUserId);
        Task<SnippetResponseDto> CreateAsync(CreateSnippetDto createDto, Guid authorId);
        Task<SnippetResponseDto> UpdateAsync(Guid id, CreateSnippetDto updateDto, Guid userId);
        Task DeleteAsync(Guid id, Guid userId);
    }
}
