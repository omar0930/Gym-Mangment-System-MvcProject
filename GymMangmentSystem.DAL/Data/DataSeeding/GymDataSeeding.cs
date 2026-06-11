using GymMangmentSystem.DAL.DbContexts;
using GymMangmentSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GymMangmentSystem.DAL.Data.DataSeeding
{
    public static class GymDataSeeding
    {
        public static async Task SeedAsync(GymDbContext dbContext, string seedFilePath, ILogger logger, CancellationToken ct = default)
        {
            try
            {
                if (!await dbContext.Plans.AnyAsync(ct))
                {
                    var Plans = LoadDataFromJsonFile<Plan>("plans.json", seedFilePath);
                    if (Plans.Count > 0)
                    {
                        dbContext.Plans.AddRange(Plans);
                        logger.LogInformation("seeded {Count} plans", Plans.Count);
                    }

                }
                if (dbContext.ChangeTracker.HasChanges())
                {
                    await dbContext.SaveChangesAsync(ct);
                    logger.LogInformation("Database seeding completed successfully.");
                }
                else
                {
                    logger.LogInformation("No new data to seed. Database is already up to date.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private static List<T> LoadDataFromJsonFile<T>(string FileName, string FolderPath)
        {
            var FilePath = Path.Combine(FolderPath, FileName);
            if (!File.Exists(FilePath))
                throw new FileNotFoundException($"The file {FilePath} was not found.");

            var Data = File.ReadAllText(FilePath);
            var Options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            Options.Converters.Add(new JsonStringEnumConverter());
            return JsonSerializer.Deserialize<List<T>>(Data, Options) ?? [];
        }
    }
}
