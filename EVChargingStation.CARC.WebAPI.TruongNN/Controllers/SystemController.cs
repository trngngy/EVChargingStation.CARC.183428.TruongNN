using EVChargingStation.CARC.Application.TruongNN.Interfaces.Commons;
using EVChargingStation.CARC.Application.TruongNN.Utils;
using EVChargingStation.CARC.Domain.TruongNN;
using EVChargingStation.CARC.Domain.TruongNN.Entities;
using EVChargingStation.CARC.Domain.TruongNN.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EVChargingStation.CARC.WebAPI.TruongNN.Controllers
{

    [ApiController]
    [Route("api/system")]
    public class SystemController : ControllerBase
    {
        private readonly FA25_SWD392_SE183428_G6_EvChargingStation _context;
        private readonly ILoggerService _logger;

        public SystemController(FA25_SWD392_SE183428_G6_EvChargingStation context, ILoggerService logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("seed-all-data")]
        public async Task<IActionResult> SeedData()
        {
            try
            {
                await ClearDatabase(_context);
                await SeedUserAsync();

                return Ok(ApiResult<object>.Success(new
                {
                    Message = "Data seeded successfully."
                }));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.Error("Database update error during data seeding: " + dbEx.Message);
                return StatusCode(500, "Database update error occurred while seeding data.");
            }
            catch (Exception ex)
            {
                _logger.Error("Error during data seeding: " + ex.Message);
                return StatusCode(500, "An error occurred while seeding data.");
            }
        }

        private async Task ClearDatabase(FA25_SWD392_SE183428_G6_EvChargingStation context)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                await context.Users.ExecuteDeleteAsync();

                await transaction.CommitAsync();
                _logger.Success("Deleted data in database successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.Error("Error clearing database: " + ex.Message);
                throw;
            }
        }

        //Seed methods go here
        private async Task SeedUserAsync()
        {
            var passwordHasher = new PasswordHasher();

            //Seed User
            var users = new List<User>
            {
                new()
                {
                    FirstName = "Admin",
                    LastName = "User",
                    PasswordHash = passwordHasher.HashPassword("Admin@123"),
                    DateOfBirth = DateTime.UtcNow.AddYears(-30),
                    Gender = Gender.Male,
                    Email = "Admin@gmail.com",
                    Phone = "1234567890",
                    Address = "123 Admin St, City, Country",
                    Role = RoleType.Admin,
                    Status = UserStatus.Active
                }
            };
            _logger.Info("Seeding users with roles...");

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();
            _logger.Success("Users seeded successfully.");
        }
    }
}
