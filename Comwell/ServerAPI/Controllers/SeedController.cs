using Microsoft.AspNetCore.Mvc;
using Core.Models;
using ServerAPI.Repositories;

namespace ServerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly IQuizRepository _quizRepo;
    private readonly IAchievementRepository _achievementRepo;

    public SeedController(IQuizRepository quizRepo, IAchievementRepository achievementRepo)
    {
        _quizRepo = quizRepo;
        _achievementRepo = achievementRepo;
    }

    [HttpPost("quiz-data")]
    public async Task<IActionResult> SeedQuizData([FromQuery] bool clearExisting = false)
    {
        // Check if achievements already exist
        var existingAchievements = await _achievementRepo.GetAllAsync();
        if (!existingAchievements.Any())
        {
            // Create achievements
            var achievements = new List<Achievement>
            {
                new Achievement
                {
                    Name = "Første Skridt",
                    Description = "Gennemfør din første quiz",
                    Icon = "🎯",
                    Requirements = "Gennemfør 1 quiz",
                    IsActive = true
                },
                new Achievement
                {
                    Name = "Perfektionist",
                    Description = "Få 100% i en quiz",
                    Icon = "💯", 
                    Requirements = "Opnå 100% score",
                    IsActive = true
                },
                new Achievement
                {
                    Name = "Quiz Mester",
                    Description = "Gennemfør 5 quizzer",
                    Icon = "🏆",
                    Requirements = "Gennemfør 5 quizzer",
                    IsActive = true
                },
                new Achievement
                {
                    Name = "Lynhurtig",
                    Description = "Gennemfør en quiz på under 2 minutter",
                    Icon = "⚡",
                    Requirements = "Gennemfør quiz under 2 min",
                    IsActive = true
                },
                new Achievement
                {
                    Name = "Akademiker",
                    Description = "Opnå gennemsnitlig 85%+ over 3+ quizzer",
                    Icon = "🎓",
                    Requirements = "85%+ gennemsnit",
                    IsActive = true
                }
            };

            foreach (var achievement in achievements)
            {
                await _achievementRepo.CreateAsync(achievement);
            }
        }

        // Check if quizzes already exist
        var existingQuizzes = await _quizRepo.GetAllAsync();
        if (!existingQuizzes.Any() || clearExisting)
        {
            var quizzes = new List<Quiz>
            {
                // Quiz 1: Grundlæggende Køkken Viden
                new Quiz
                {
                    Title = "Grundlæggende Køkken Viden",
                    Description = "Test din viden om grundlæggende køkkenprincipper og teknikker",
                    CreatedByUserId = 1,
                    TimeLimitMinutes = 5,
                    Difficulty = "Let",
                    IsActive = true,
                    Questions = new List<QuizQuestion>
                    {
                        new QuizQuestion
                        {
                            QuestionText = "Ved hvilken temperatur skal kød opbevares i køleskabet?",
                            QuestionType = "MultipleChoice",
                            Points = 10,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "Under 5°C", IsCorrect = true },
                                new QuizAnswer { AnswerText = "Under 10°C", IsCorrect = false },
                                new QuizAnswer { AnswerText = "Under 15°C", IsCorrect = false },
                                new QuizAnswer { AnswerText = "Det spiller ingen rolle", IsCorrect = false }
                            }
                        },
                        new QuizQuestion
                        {
                            QuestionText = "Skal du vaske hænderne før du håndterer mad?",
                            QuestionType = "TrueFalse",
                            Points = 10,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "Ja", IsCorrect = true },
                                new QuizAnswer { AnswerText = "Nej", IsCorrect = false }
                            }
                        },
                        new QuizQuestion
                        {
                            QuestionText = "Hvor længe kan tilberedt mad opbevares i køleskabet?",
                            QuestionType = "MultipleChoice",
                            Points = 10,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "1 dag", IsCorrect = false },
                                new QuizAnswer { AnswerText = "3-4 dage", IsCorrect = true },
                                new QuizAnswer { AnswerText = "1 uge", IsCorrect = false },
                                new QuizAnswer { AnswerText = "2 uger", IsCorrect = false }
                            }
                        },
                        new QuizQuestion
                        {
                            QuestionText = "Er det sikkert at optø mad ved stuetemperatur?",
                            QuestionType = "TrueFalse",
                            Points = 10,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "Ja", IsCorrect = false },
                                new QuizAnswer { AnswerText = "Nej", IsCorrect = true }
                            }
                        },
                        new QuizQuestion
                        {
                            QuestionText = "Hvilken temperatur skal fjerkræ nå indvendigt for at være sikkert at spise?",
                            QuestionType = "MultipleChoice",
                            Points = 10,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "65°C", IsCorrect = false },
                                new QuizAnswer { AnswerText = "72°C", IsCorrect = false },
                                new QuizAnswer { AnswerText = "74°C", IsCorrect = true },
                                new QuizAnswer { AnswerText = "80°C", IsCorrect = false }
                            }
                        }
                    }
                },

                // Quiz 2: Kundeservice Excellence
                new Quiz
                {
                    Title = "Kundeservice Excellence",
                    Description = "Test din viden om fremragende kundeservice i hotel- og servicebranchen",
                    CreatedByUserId = 1,
                    TimeLimitMinutes = 7,
                    Difficulty = "Medium",
                    IsActive = true,
                    Questions = new List<QuizQuestion>
                    {
                        new QuizQuestion
                        {
                            QuestionText = "Hvad er den vigtigste regel ved kundeservice?",
                            QuestionType = "MultipleChoice",
                            Points = 10,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "Altid smil", IsCorrect = false },
                                new QuizAnswer { AnswerText = "Lyt aktivt til kunden", IsCorrect = true },
                                new QuizAnswer { AnswerText = "Tag aldrig kritik personligt", IsCorrect = false },
                                new QuizAnswer { AnswerText = "Vær hurtig", IsCorrect = false }
                            }
                        },
                        new QuizQuestion
                        {
                            QuestionText = "Skal du altid sige undskyld, selv når det ikke er din fejl?",
                            QuestionType = "TrueFalse",
                            Points = 10,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "Ja", IsCorrect = true },
                                new QuizAnswer { AnswerText = "Nej", IsCorrect = false }
                            }
                        },
                        new QuizQuestion
                        {
                            QuestionText = "Hvad gør du hvis en kunde klager over noget du ikke kan løse?",
                            QuestionType = "MultipleChoice",
                            Points = 10,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "Fortæl dem at det ikke er dit problem", IsCorrect = false },
                                new QuizAnswer { AnswerText = "Eskaler til en leder/kollega", IsCorrect = true },
                                new QuizAnswer { AnswerText = "Giv op og gå væk", IsCorrect = false },
                                new QuizAnswer { AnswerText = "Bliv sur på kunden", IsCorrect = false }
                            }
                        },
                        new QuizQuestion
                        {
                            QuestionText = "Er kroppssprog vigtigt i kundeservice?",
                            QuestionType = "TrueFalse",
                            Points = 10,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "Ja", IsCorrect = true },
                                new QuizAnswer { AnswerText = "Nej", IsCorrect = false }
                            }
                        },
                        new QuizQuestion
                        {
                            QuestionText = "Hvor hurtigt bør du reagere på en kundehenvendelse?",
                            QuestionType = "MultipleChoice",
                            Points = 10,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "Inden for 24 timer", IsCorrect = false },
                                new QuizAnswer { AnswerText = "Så hurtigt som muligt", IsCorrect = true },
                                new QuizAnswer { AnswerText = "Inden for en uge", IsCorrect = false },
                                new QuizAnswer { AnswerText = "Kun hvis det er vigtigt", IsCorrect = false }
                            }
                        }
                    }
                },

                // Quiz 3: Hotel Operations
                new Quiz
                {
                    Title = "Hotel Operations",
                    Description = "Test din viden om hoteloperationer og procedurer",
                    CreatedByUserId = 1,
                    TimeLimitMinutes = 8,
                    Difficulty = "Medium",
                    IsActive = true,
                    Questions = new List<QuizQuestion>
                    {
                        new QuizQuestion
                        {
                            QuestionText = "Hvad er standard check-in tid på de fleste hoteller?",
                            QuestionType = "MultipleChoice",
                            Points = 10,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "12:00", IsCorrect = false },
                                new QuizAnswer { AnswerText = "14:00", IsCorrect = false },
                                new QuizAnswer { AnswerText = "15:00", IsCorrect = true },
                                new QuizAnswer { AnswerText = "16:00", IsCorrect = false }
                            }
                        },
                        new QuizQuestion
                        {
                            QuestionText = "Hvad er standard check-out tid?",
                            QuestionType = "MultipleChoice",
                            Points = 10,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "10:00", IsCorrect = false },
                                new QuizAnswer { AnswerText = "11:00", IsCorrect = true },
                                new QuizAnswer { AnswerText = "12:00", IsCorrect = false },
                                new QuizAnswer { AnswerText = "13:00", IsCorrect = false }
                            }
                        },
                        new QuizQuestion
                        {
                            QuestionText = "Skal du altid bede om ID ved check-in?",
                            QuestionType = "TrueFalse",
                            Points = 10,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "Ja", IsCorrect = true },
                                new QuizAnswer { AnswerText = "Nej", IsCorrect = false }
                            }
                        },
                        new QuizQuestion
                        {
                            QuestionText = "Hvad gør du hvis en gæst mister sit nøglekort?",
                            QuestionType = "MultipleChoice",
                            Points = 10,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "Giv dem et nyt med det samme", IsCorrect = false },
                                new QuizAnswer { AnswerText = "Verificer deres identitet først", IsCorrect = true },
                                new QuizAnswer { AnswerText = "Bed dem betale for et nyt", IsCorrect = false },
                                new QuizAnswer { AnswerText = "Send dem til en manager", IsCorrect = false }
                            }
                        },
                        new QuizQuestion
                        {
                            QuestionText = "Er det vigtigt at kende hotellets faciliteter godt?",
                            QuestionType = "TrueFalse",
                            Points = 10,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "Ja", IsCorrect = true },
                                new QuizAnswer { AnswerText = "Nej", IsCorrect = false }
                            }
                        }
                    }
                },

                // Quiz 4: Arbejdsmiljø og Sikkerhed
                new Quiz
                {
                    Title = "Arbejdsmiljø og Sikkerhed",
                    Description = "Test din viden om sikkerhed og arbejdsmiljø på arbejdspladsen",
                    CreatedByUserId = 1,
                    TimeLimitMinutes = 6,
                    Difficulty = "Let",
                    IsActive = true,
                    Questions = new List<QuizQuestion>
                    {
                        new QuizQuestion
                        {
                            QuestionText = "Hvad skal du gøre hvis du ser noget vådt på gulvet?",
                            QuestionType = "MultipleChoice",
                            Points = 10,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "Ignorere det", IsCorrect = false },
                                new QuizAnswer { AnswerText = "Tørre det op eller sætte advarselsskilt", IsCorrect = true },
                                new QuizAnswer { AnswerText = "Fortælle det til kolleger senere", IsCorrect = false },
                                new QuizAnswer { AnswerText = "Vente på at rengøring kommer", IsCorrect = false }
                            }
                        },
                        new QuizQuestion
                        {
                            QuestionText = "Skal du rapportere alle ulykker, uanset hvor små de er?",
                            QuestionType = "TrueFalse",
                            Points = 10,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "Ja", IsCorrect = true },
                                new QuizAnswer { AnswerText = "Nej", IsCorrect = false }
                            }
                        },
                        new QuizQuestion
                        {
                            QuestionText = "Hvad er den maksimale vægt du bør løfte alene?",
                            QuestionType = "MultipleChoice",
                            Points = 10,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "50 kg", IsCorrect = false },
                                new QuizAnswer { AnswerText = "25 kg", IsCorrect = false },
                                new QuizAnswer { AnswerText = "23 kg", IsCorrect = true },
                                new QuizAnswer { AnswerText = "30 kg", IsCorrect = false }
                            }
                        },
                        new QuizQuestion
                        {
                            QuestionText = "Er det okay at arbejde når du er syg?",
                            QuestionType = "TrueFalse",
                            Points = 10,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "Ja", IsCorrect = false },
                                new QuizAnswer { AnswerText = "Nej", IsCorrect = true }
                            }
                        }
                    }
                },

                // Quiz 5: Avanceret Køkken Teknikker
                new Quiz
                {
                    Title = "Avanceret Køkken Teknikker",
                    Description = "Test din viden om avancerede køkkenteknikker og madlavning",
                    CreatedByUserId = 1,
                    TimeLimitMinutes = 10,
                    Difficulty = "Svær",
                    IsActive = true,
                    Questions = new List<QuizQuestion>
                    {
                        new QuizQuestion
                        {
                            QuestionText = "Hvad er sous-vide teknikken?",
                            QuestionType = "MultipleChoice",
                            Points = 15,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "Hurtig stegning ved høj varme", IsCorrect = false },
                                new QuizAnswer { AnswerText = "Langsom tilberedning i vakuum ved lav temperatur", IsCorrect = true },
                                new QuizAnswer { AnswerText = "Marinering i syre", IsCorrect = false },
                                new QuizAnswer { AnswerText = "Røgning af kød", IsCorrect = false }
                            }
                        },
                        new QuizQuestion
                        {
                            QuestionText = "Hvad betyder 'brunoise' i køkkensprog?",
                            QuestionType = "MultipleChoice",
                            Points = 15,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "Store skiver", IsCorrect = false },
                                new QuizAnswer { AnswerText = "Fine terninger (2x2mm)", IsCorrect = true },
                                new QuizAnswer { AnswerText = "Strimler", IsCorrect = false },
                                new QuizAnswer { AnswerText = "Hakket groft", IsCorrect = false }
                            }
                        },
                        new QuizQuestion
                        {
                            QuestionText = "Kan du bruge samme skærebræt til kød og grøntsager?",
                            QuestionType = "TrueFalse",
                            Points = 10,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "Ja", IsCorrect = false },
                                new QuizAnswer { AnswerText = "Nej", IsCorrect = true }
                            }
                        },
                        new QuizQuestion
                        {
                            QuestionText = "Hvad er den optimale temperatur for chokolade tempering?",
                            QuestionType = "MultipleChoice",
                            Points = 15,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "25-30°C", IsCorrect = false },
                                new QuizAnswer { AnswerText = "31-32°C", IsCorrect = true },
                                new QuizAnswer { AnswerText = "35-40°C", IsCorrect = false },
                                new QuizAnswer { AnswerText = "20-25°C", IsCorrect = false }
                            }
                        },
                        new QuizQuestion
                        {
                            QuestionText = "Er det nødvendigt at hvile kød efter stegning?",
                            QuestionType = "TrueFalse",
                            Points = 10,
                            Answers = new List<QuizAnswer>
                            {
                                new QuizAnswer { AnswerText = "Ja", IsCorrect = true },
                                new QuizAnswer { AnswerText = "Nej", IsCorrect = false }
                            }
                        }
                    }
                }
            };

            foreach (var quiz in quizzes)
            {
                await _quizRepo.CreateAsync(quiz);
            }
        }

        return Ok(new { message = "Quiz data seeded successfully" });
    }
} 