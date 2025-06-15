using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MeritJournal.Application.DTOs;
using Xunit;

namespace MeritJournal.IntegrationTests.Endpoints
{
    [Collection("Database collection")]
    public class JournalEntryEndpointsTests
    {
        private readonly HttpClient _client;
        private readonly DatabaseFixture _fixture;
        
        public JournalEntryEndpointsTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            var factory = new TestWebApplicationFactory(fixture.ConnectionString);
            _client = factory.CreateClient();
        }
        
        [Fact]
        public async Task GetAllJournalEntries_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/api/journal-entries");
            
            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var entries = JsonSerializer.Deserialize<List<JournalEntryDto>>(content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            Assert.NotNull(entries);
            Assert.True(entries.Count >= 2); // We seeded at least 2 entries
        }
        
        [Fact]
        public async Task GetJournalEntryById_ReturnsCorrectEntry()
        {
            // Arrange
            var expectedId = 1;
            
            // Act
            var response = await _client.GetAsync($"/api/journal-entries/{expectedId}");
            
            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var entry = JsonSerializer.Deserialize<JournalEntryDto>(content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            Assert.NotNull(entry);
            Assert.Equal(expectedId, entry.Id);
            Assert.Equal("Test Entry 1", entry.Title);
            Assert.NotNull(entry.Tags);
            Assert.Contains("health", entry.Tags);
            Assert.Contains("work", entry.Tags);
        }
        
        [Fact]
        public async Task CreateJournalEntry_ReturnsCreatedEntry()
        {
            // Arrange
            var newEntry = new
            {
                Title = "Integration Test Entry",
                Content = "This is a test entry created during integration testing",
                EntryDate = DateTime.UtcNow.Date.ToString("o"),
                Tags = new[] { "test", "integration" }
            };
            
            var jsonContent = JsonSerializer.Serialize(newEntry);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
            // Act
            var response = await _client.PostAsync("/api/journal-entries", httpContent);
            
            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var createdEntry = JsonSerializer.Deserialize<JournalEntryDto>(content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            Assert.NotNull(createdEntry);
            Assert.Equal("Integration Test Entry", createdEntry.Title);
            Assert.Equal("This is a test entry created during integration testing", createdEntry.Content);
            Assert.NotNull(createdEntry.Tags);
            Assert.Contains("test", createdEntry.Tags);
            Assert.Contains("integration", createdEntry.Tags);
        }
        
        [Fact]
        public async Task UpdateJournalEntry_ReturnsUpdatedEntry()
        {
            // Arrange
            var entryToUpdate = 2;
            var updatedEntry = new
            {
                Title = "Updated Test Entry",
                Content = "This content was updated during integration testing",
                EntryDate = DateTime.UtcNow.Date.ToString("o"),
                Tags = new[] { "updated", "testing" }
            };
            
            var jsonContent = JsonSerializer.Serialize(updatedEntry);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
            // Act
            var response = await _client.PutAsync($"/api/journal-entries/{entryToUpdate}", httpContent);
            
            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JournalEntryDto>(content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            Assert.NotNull(result);
            Assert.Equal(entryToUpdate, result.Id);
            Assert.Equal("Updated Test Entry", result.Title);
            Assert.Equal("This content was updated during integration testing", result.Content);
            Assert.NotNull(result.Tags);
            Assert.Contains("updated", result.Tags);
            Assert.Contains("testing", result.Tags);
            Assert.DoesNotContain("personal", result.Tags); // The original tag should be gone
        }
        
        [Fact]
        public async Task DeleteJournalEntry_RemovesEntry()
        {
            // Arrange
            // First create a new entry to delete
            var newEntry = new
            {
                Title = "Entry to Delete",
                Content = "This entry will be deleted",
                EntryDate = DateTime.UtcNow.Date.ToString("o")
            };
            
            var jsonContent = JsonSerializer.Serialize(newEntry);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/journal-entries", httpContent);
            createResponse.EnsureSuccessStatusCode();
            
            var createContent = await createResponse.Content.ReadAsStringAsync();
            var createdEntry = JsonSerializer.Deserialize<JournalEntryDto>(createContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            Assert.NotNull(createdEntry);
            var entryId = createdEntry.Id;
            
            // Act - Delete the entry
            var deleteResponse = await _client.DeleteAsync($"/api/journal-entries/{entryId}");
            
            // Assert
            deleteResponse.EnsureSuccessStatusCode();
            
            // Verify it's gone
            var getResponse = await _client.GetAsync($"/api/journal-entries/{entryId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
