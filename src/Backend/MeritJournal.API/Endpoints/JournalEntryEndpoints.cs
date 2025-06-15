using MediatR;
using MeritJournal.Application.DTOs;
using MeritJournal.Application.Features.JournalEntries.Commands;
using MeritJournal.Application.Features.JournalEntries.Queries;
using Microsoft.AspNetCore.Mvc;

namespace MeritJournal.API.Endpoints;

/// <summary>
/// Extension methods for configuring journal entry endpoints.
/// </summary>
public static class JournalEntryEndpoints
{    /// <summary>
    /// Registers the journal entry endpoints with the application.
    /// </summary>
    /// <param name="app">The web application to add endpoints to.</param>
    /// <returns>The same web application instance for call chaining.</returns>
    public static WebApplication MapJournalEntryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/journal-entries");
        // TEMPORARY: Authentication disabled for testing
        // .RequireAuthorization();        // Get all journal entries for the current user
        group.MapGet("/", async (IMediator mediator, HttpContext httpContext) =>
        {
            // TEMPORARY: Using a fixed user ID for testing
            var userId = "test-user-id";
            
            // COMMENTED OUT FOR TESTING:
            // var userId = httpContext.User.FindFirst("sub")?.Value ?? 
            //               httpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            // 
            // if (string.IsNullOrEmpty(userId))
            // {
            //     return Results.Unauthorized();
            // }
            
            var query = new GetJournalEntriesQuery(userId);
            var result = await mediator.Send(query);
            
            return Results.Ok(result);
        })
        .WithName("GetJournalEntries")
        .WithOpenApi();        // Create a new journal entry
        group.MapPost("/", async (IMediator mediator, HttpContext httpContext, [FromBody] CreateJournalEntryDto dto) =>
        {
            // TEMPORARY: Using a fixed user ID for testing
            var userId = "test-user-id";
            
            // COMMENTED OUT FOR TESTING:
            // var userId = httpContext.User.FindFirst("sub")?.Value ?? 
            //               httpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            // 
            // if (string.IsNullOrEmpty(userId))
            // {
            //     return Results.Unauthorized();
            // }
              // Create the command with explicit UTC date conversion for PostgreSQL timestamp with time zone
            var command = new CreateJournalEntryCommand
            {
                Title = dto.Title,
                Content = dto.Content,
                EntryDate = DateTime.SpecifyKind(dto.EntryDate, DateTimeKind.Utc),
                Images = dto.Images,
                Tags = dto.Tags,
                UserId = userId
            };
            
            var result = await mediator.Send(command);
            
            return Results.Created($"/api/journal-entries/{result.Id}", result);
        })
        .WithName("CreateJournalEntry")
        .WithOpenApi();

        // Get a journal entry by ID
        group.MapGet("/{id:int}", async (int id, IMediator mediator, HttpContext httpContext) =>
        {
            // TEMPORARY: Using a fixed user ID for testing
            var userId = "test-user-id";
            
            // COMMENTED OUT FOR TESTING:
            // var userId = httpContext.User.FindFirst("sub")?.Value ?? 
            //               httpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            // 
            // if (string.IsNullOrEmpty(userId))
            // {
            //     return Results.Unauthorized();
            // }
            
            var query = new GetJournalEntryByIdQuery(id, userId);
            var result = await mediator.Send(query);
            
            if (result == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(result);
        })
        .WithName("GetJournalEntryById")
        .WithOpenApi();

        // Update an existing journal entry
        group.MapPut("/{id:int}", async (int id, IMediator mediator, HttpContext httpContext, [FromBody] CreateJournalEntryDto dto) =>
        {
            // TEMPORARY: Using a fixed user ID for testing
            var userId = "test-user-id";
            
            // COMMENTED OUT FOR TESTING:
            // var userId = httpContext.User.FindFirst("sub")?.Value ?? 
            //               httpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            // 
            // if (string.IsNullOrEmpty(userId))
            // {
            //     return Results.Unauthorized();
            // }
            
            var command = new UpdateJournalEntryCommand
            {
                Id = id,
                Title = dto.Title,
                Content = dto.Content,
                EntryDate = DateTime.SpecifyKind(dto.EntryDate, DateTimeKind.Utc),
                Images = dto.Images,
                Tags = dto.Tags,
                UserId = userId
            };
            
            try
            {
                var result = await mediator.Send(command);
                return Results.Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        })
        .WithName("UpdateJournalEntry")
        .WithOpenApi();

        // Delete a journal entry
        group.MapDelete("/{id:int}", async (int id, IMediator mediator, HttpContext httpContext) =>
        {
            // TEMPORARY: Using a fixed user ID for testing
            var userId = "test-user-id";
            
            // COMMENTED OUT FOR TESTING:
            // var userId = httpContext.User.FindFirst("sub")?.Value ?? 
            //               httpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            // 
            // if (string.IsNullOrEmpty(userId))
            // {
            //     return Results.Unauthorized();
            // }
            
            var command = new DeleteJournalEntryCommand(id, userId);
            
            try
            {
                await mediator.Send(command);
                return Results.NoContent();
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        })
        .WithName("DeleteJournalEntry")
        .WithOpenApi();

        return app;
    }
}

/// <summary>
/// DTO for creating a journal entry.
/// </summary>
public class CreateJournalEntryDto
{
    /// <summary>
    /// The title of the journal entry.
    /// </summary>
    public required string Title { get; init; }
    
    /// <summary>
    /// The content of the journal entry in HTML format.
    /// </summary>
    public required string Content { get; init; }
    
    /// <summary>
    /// The date to which this journal entry pertains.
    /// </summary>
    public DateTime EntryDate { get; init; }
    
    /// <summary>
    /// The images associated with this journal entry.
    /// </summary>
    public List<JournalImageDto>? Images { get; init; }
    
    /// <summary>
    /// The tags associated with this journal entry.
    /// </summary>
    public List<string>? Tags { get; init; }
}
