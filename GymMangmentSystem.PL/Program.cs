using GymMangmentSystem.BLL.Services.Classes;
using GymMangmentSystem.BLL.Services.InterFaces;
using GymMangmentSystem.DAL.DbContexts;
using GymMangmentSystem.DAL.Models;
using GymMangmentSystem.DAL.Repositories.Classes;
using GymMangmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<ITrainerService, TrainerService>();

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

            if (context.Plans.Any())
                return;

            var now = DateTime.Now;
            context.Plans.AddRange(
                new Plan { Name = "Basic", Description = "Access to gym floor and locker rooms during standard hours.", DurationInDays = 30, Price = 250m, IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Plan { Name = "Standard", Description = "Full gym access plus group classes and sauna.", DurationInDays = 90, Price = 650m, IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Plan { Name = "Premium", Description = "Unlimited access, personal trainer sessions, and nutrition guidance.", DurationInDays = 180, Price = 1200m, IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Plan { Name = "Annual Elite", Description = "Year-round all-inclusive membership with priority booking.", DurationInDays = 365, Price = 2200m, IsActive = true, CreatedAt = now, UpdatedAt = now }
            );

            context.SaveChanges();
        }
    }
}
