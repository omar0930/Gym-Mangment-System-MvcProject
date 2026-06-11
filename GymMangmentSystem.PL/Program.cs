using GymMangmentSystem.BLL;
using GymMangmentSystem.BLL.Services.Classes;
using GymMangmentSystem.BLL.Services.InterFaces;
using GymMangmentSystem.DAL.DbContexts;
using GymMangmentSystem.DAL.Models;
using GymMangmentSystem.DAL.Repositories.Classes;
using GymMangmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GymMangmentSystem.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<GymDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ISessionRepository, SessionRepository>();
            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<ITrainerService, TrainerService>();
            builder.Services.AddScoped<ISessionService, SessionService>();
            builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
            builder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfiles()));

            var app = builder.Build();

            SeedPlans(app);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }

        private static void SeedPlans(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            context.Database.Migrate();

            // Idempotent: only seed when the Plans table is empty.
            if (context.Plans.Any())
                return;

            var filePath = Path.Combine(app.Environment.ContentRootPath, "Data", "plans.json");
            if (!File.Exists(filePath))
                return;

            var json = File.ReadAllText(filePath);
            var plans = JsonSerializer.Deserialize<List<Plan>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (plans is null || plans.Count == 0)
                return;

            var now = DateTime.Now;
            foreach (var plan in plans)
            {
                plan.CreatedAt = now;
                plan.UpdatedAt = now;
            }

            context.Plans.AddRange(plans);
            context.SaveChanges();
        }
    }
}
