using Core.Models;
using System.Text.Json;

namespace WebApp.Services;

public class QuizService : IQuizService
{
    private readonly HttpClient _httpClient;

    public QuizService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Quiz management
    public async Task<List<Quiz>> GetAllQuizzesAsync()
    {
        var response = await _httpClient.GetAsync("api/quiz");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Quiz>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Quiz>();
        }
        return new List<Quiz>();
    }

    public async Task<Quiz?> GetQuizByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/quiz/{id}");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Quiz>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        return null;
    }

    public async Task<Quiz> CreateQuizAsync(Quiz quiz)
    {
        var json = JsonSerializer.Serialize(quiz);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/quiz", content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Quiz>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
        throw new Exception("Failed to create quiz");
    }

    public async Task<Quiz> UpdateQuizAsync(Quiz quiz)
    {
        var json = JsonSerializer.Serialize(quiz);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"api/quiz/{quiz.Id}", content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Quiz>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
        throw new Exception("Failed to update quiz");
    }

    public async Task DeleteQuizAsync(int id)
    {
        await _httpClient.DeleteAsync($"api/quiz/{id}");
    }

    // Quiz attempts
    public async Task<QuizAttempt> StartQuizAsync(int quizId, int userId)
    {
        var request = new { QuizId = quizId, UserId = userId };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync("api/quiz/start", content);
        if (response.IsSuccessStatusCode)
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<QuizAttempt>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
        throw new Exception("Failed to start quiz");
    }

    public async Task<QuizAttempt> SubmitQuizAsync(QuizAttempt attempt)
    {
        var json = JsonSerializer.Serialize(attempt);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync("api/quiz/submit", content);
        if (response.IsSuccessStatusCode)
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<QuizAttempt>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
        throw new Exception("Failed to submit quiz");
    }

    public async Task<List<QuizAttempt>> GetUserAttemptsAsync(int userId)
    {
        var response = await _httpClient.GetAsync($"api/quiz/attempts/user/{userId}");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<QuizAttempt>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<QuizAttempt>();
        }
        return new List<QuizAttempt>();
    }

    public async Task<List<QuizAttempt>> GetQuizAttemptsAsync(int quizId)
    {
        var response = await _httpClient.GetAsync($"api/quiz/attempts/quiz/{quizId}");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<QuizAttempt>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<QuizAttempt>();
        }
        return new List<QuizAttempt>();
    }

    // Leaderboard
    public async Task<List<QuizAttempt>> GetLeaderboardAsync(int? quizId = null)
    {
        var url = "api/quiz/leaderboard";
        if (quizId.HasValue)
        {
            url += $"?quizId={quizId.Value}";
        }
        
        var response = await _httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<QuizAttempt>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<QuizAttempt>();
        }
        return new List<QuizAttempt>();
    }

    // Achievements
    public async Task<List<Achievement>> GetAllAchievementsAsync()
    {
        var response = await _httpClient.GetAsync("api/quiz/achievements");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Achievement>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Achievement>();
        }
        return new List<Achievement>();
    }

    public async Task<List<UserAchievement>> GetUserAchievementsAsync(int userId)
    {
        var response = await _httpClient.GetAsync($"api/quiz/achievements/user/{userId}");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<UserAchievement>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<UserAchievement>();
        }
        return new List<UserAchievement>();
    }

    public async Task CheckAndAwardAchievementsAsync(int userId)
    {
        // This is now handled by the API when submitting a quiz
        await Task.CompletedTask;
    }


} 