using EVChargingStation.CARC.Application.TruongNN.Interfaces.Commons;
using EVChargingStation.CARC.Application.TruongNN.Utils;
using EVChargingStation.CARC.Domain.TruongNN;
using EVChargingStation.CARC.Domain.TruongNN.Entities;
using EVChargingStation.CARC.Domain.TruongNN.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

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
                await SeedVehicleAsync();
                await SeedPlanAsync();
                await SeedUserPlanAsync();
                await SeedLocationAsync();
                await SeedStationAsync();
                await SeedConnectorAsync();
                await SeedSessionAsync();
                await SeedInvoiceAsync();

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
                //Delete data in order to avoid foreign key constraint issues
                await context.InvoiceTruongNN.ExecuteDeleteAsync();
                await context.Sessions.ExecuteDeleteAsync();
                await context.Connectors.ExecuteDeleteAsync();
                await context.StationAnhDHV.ExecuteDeleteAsync();
                await context.Locations.ExecuteDeleteAsync();
                await context.UserPlanHoaHTT.ExecuteDeleteAsync();
                await context.Plans.ExecuteDeleteAsync();
                await context.VehicleHuyPD.ExecuteDeleteAsync();
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

        private async Task SeedVehicleAsync()
        {
            _logger.Info("Seeding vehicles...");

            //Get Admin User
            var adminUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == "Admin@gmail.com");

            if (adminUser == null)
            {
                _logger.Error("Admin user not found for vehicle seeding.");
                return;
            }

            //Create vehicles for Admin
            var vehicles = new List<VehicleHuyPD>
            {
                new()
                {
                    Make = "Tesla",
                    Model = "Model 3",
                    Year = 2023,
                    LicensePlate = "30A-1345",
                    ConnectorType = ConnectorType.CCS,
                    UserId = adminUser.TruongNNID
                },
                new()
                {
                    Make = "VinFast",
                    Model = "VF e34",
                    Year = 2024,
                    LicensePlate = "29B-67890",
                    ConnectorType = ConnectorType.CHAdeMO,
                    UserId = adminUser.TruongNNID
                },
                new()
                {
                    Make = "BMW",
                    Model = "i4",
                    Year = 2023,
                    LicensePlate = "31C-54321",
                    ConnectorType = ConnectorType.AC,
                    UserId = adminUser.TruongNNID
                }
            };

            await _context.VehicleHuyPD.AddRangeAsync(vehicles);
            await _context.SaveChangesAsync();
            _logger.Success($"Seeded {vehicles.Count} vehicles seeded successfully.");
        }

        private async Task SeedPlanAsync()
        {
            _logger.Info("Seeding plans...");

            //Create plans with different types
            var plans = new List<Plan>
            {
                new()
                {
                    Name = "Basic Prepaid",
                    Description = "Pay as you go - Top up your account and charge at standard rates",
                    Type = PlanType.Prepaid,
                    Price = 0m, //No monthly fee
                    MaxDailyKwh = 50m
                },
                new()
                {
                    Name = "Standard Postpaid",
                    Description ="Monthly billing with competitive rates for regular users",
                    Type = PlanType.Postpaid,
                    Price = 99000m, //Monthly fee per moth
                    MaxDailyKwh = 100m
                },
                 new()
                {
                    Name = "Premium Postpaid",
                    Description = "Enhanced postpaid plan with higher daily limits and priority support",
                    Type = PlanType.Postpaid,
                    Price = 199000m, // 199,000 VND per month
                    MaxDailyKwh = 200m
                },
                new()
                {
                    Name = "VIP Unlimited",
                    Description = "Exclusive VIP plan with unlimited charging, priority access, and premium benefits",
                    Type = PlanType.VIP,
                    Price = 499000m, // 499,000 VND per month
                    MaxDailyKwh = null // Unlimited
                }
            };

            await _context.Plans.AddRangeAsync(plans);
            await _context.SaveChangesAsync();
            _logger.Success($"Seeded {plans.Count} plans successfully.");
        }

        private async Task SeedUserPlanAsync()
        {
            _logger.Info("Seeding user plans...");
            //Get Admin User
            var adminUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == "Admin@gmail.com");

            if (adminUser == null)
            {
                _logger.Error("Admin user not found for user plan seeding.");
                return;
            }

            //Get VIP Plan
            var vipPlan = await _context.Plans
                .FirstOrDefaultAsync(p => p.Type == PlanType.VIP);

            if (vipPlan == null)
            {
                _logger.Error("VIP plan not found for user plan seeding.");
                return;
            }

            //Assign VIP plan to Admin user
            var userPlans = new List<UserPlanHoaHTT>
            {
                new()
                {
                UserId = adminUser.TruongNNID,
                PlanId = vipPlan.TruongNNID,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddYears(1) // 1 year validity
                }
            };

            await _context.UserPlanHoaHTT.AddRangeAsync(userPlans);
            await _context.SaveChangesAsync();
            _logger.Success($"Seeded {userPlans.Count} user plans successfully.");
        }

        private async Task SeedLocationAsync()
        {
            _logger.Info("Seeding locations...");

            var locations = new List<Location>
            {
                new()
                {
                    Name = "FPT University HCM",
                    Address = "Lô E2a-7, Đường D1, Khu Công nghệ cao, P.Long Thạnh Mỹ, TP. Thủ Đức, TP.HCM",
                    Latitude = 10.8411m,
                    Longitude = 106.8098m,
                    City = "Ho Chi Minh City",
                    StateProvince = "Ho Chi Minh",
                    Country = "Vietnam",
                    Timezone = "Asia/Ho_Chi_Minh"
                },
                new()
                {
                    Name = "Vincom Center Landmark 81",
                    Address = "720A Điện Biên Phủ, Vinhomes Tân Cảng, Bình Thạnh, TP.HCM",
                    Latitude = 10.7946m,
                    Longitude = 106.7218m,
                    City = "Ho Chi Minh City",
                    StateProvince = "Ho Chi Minh",
                    Country = "Vietnam",
                    Timezone = "Asia/Ho_Chi_Minh"
                },
                new()
                {
                    Name = "Saigon Centre",
                    Address = "65 Lê Lợi, Bến Nghé, Quận 1, TP.HCM",
                    Latitude = 10.7769m,
                    Longitude = 106.7009m,
                    City = "Ho Chi Minh City",
                    StateProvince = "Ho Chi Minh",
                    Country = "Vietnam",
                    Timezone = "Asia/Ho_Chi_Minh"
                }
            };

            await _context.Locations.AddRangeAsync(locations);
            await _context.SaveChangesAsync();
            _logger.Success($"Seeded {locations.Count} locations successfully.");
        }

        private async Task SeedStationAsync()
        {
            _logger.Info("Seeding stations...");

            //Get Locations
            var locations = await _context.Locations.ToListAsync();

            if (!locations.Any())
            {
                _logger.Error("No locations found for station seeding.");
                return;
            }
            var stations = new List<StationAnhDHV>
            {
                new()
                {
                    Name = "FPT Fast Charging Hub",
                    LocationId = locations[0].TruongNNID,
                    Status = StationStatus.Online
                },
                new()
                {
                    Name = "Landmark 81 EV Station",
                    LocationId = locations[1].TruongNNID,
                    Status = StationStatus.Online
                },
                new()
                {
                    Name = "Saigon Centre Power Point",
                    LocationId = locations[2].TruongNNID,
                    Status = StationStatus.Online
                }
            };
            await _context.StationAnhDHV.AddRangeAsync(stations);
            await _context.SaveChangesAsync();
            _logger.Success($"Seeded {stations.Count} stations successfully.");
        }

        private async Task SeedConnectorAsync()
        {
            _logger.Info("Seeding connectors...");

            var stations = await _context.StationAnhDHV.ToListAsync();

            if (!stations.Any())
            {
                _logger.Error("No stations found for connector seeding.");
                return;
            }

            var connectors = new List<Connector>();

            // Add multiple connectors for each station
            foreach (var station in stations)
            {
                connectors.AddRange(new List<Connector>
                {
                    new()
                    {
                        StationAnhDHVId = station.TruongNNID,
                        ConnectorType = ConnectorType.CCS,
                        PowerKw = 150m,
                        Status = ConnectorStatus.Free,
                        PricePerKwh = 4500m
                    },
                    new()
                    {
                        StationAnhDHVId = station.TruongNNID,
                        ConnectorType = ConnectorType.CHAdeMO,
                        PowerKw = 100m,
                        Status = ConnectorStatus.Free,
                        PricePerKwh = 4000m
                    },
                    new()
                    {
                        StationAnhDHVId = station.TruongNNID,
                        ConnectorType = ConnectorType.AC,
                        PowerKw = 22m,
                        Status = ConnectorStatus.Free,
                        PricePerKwh = 3000m
                    }
                });
            }

            await _context.Connectors.AddRangeAsync(connectors);
            await _context.SaveChangesAsync();
            _logger.Success($"Seeded {connectors.Count} connectors successfully.");
        }

        private async Task SeedSessionAsync()
        {
            _logger.Info("Seeding sessions...");

            var adminUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == "Admin@gmail.com");
            var connectors = await _context.Connectors.ToListAsync();

            if (adminUser == null || !connectors.Any())
            {
                _logger.Error("Admin user or connectors not found for session seeding.");
                return;
            }

            var sessions = new List<Session>();
            var random = new Random();

            // Tạo 15 sessions với thời gian khác nhau trong 30 ngày qua
            for (int i = 0; i < 15; i++)
            {
                var daysAgo = random.Next(0, 30);
                var startHour = random.Next(1, 5);
                var connector = connectors[random.Next(connectors.Count)];
                var energyKwh = random.Next(20, 80);
                var cost = energyKwh * connector.PricePerKwh;

                sessions.Add(new Session
                {
                    ConnectorId = connector.TruongNNID,
                    UserId = adminUser.TruongNNID,
                    StartTime = DateTime.UtcNow.AddDays(-daysAgo).AddHours(-startHour),
                    EndTime = DateTime.UtcNow.AddDays(-daysAgo).AddHours(-startHour + random.Next(1, 4)),
                    Status = SessionStatus.Stopped,
                    SocStart = random.Next(10, 40),
                    SocEnd = random.Next(70, 100),
                    EnergyKwh = energyKwh,
                    Cost = cost
                });
            }

            await _context.Sessions.AddRangeAsync(sessions);
            await _context.SaveChangesAsync();
            _logger.Success($"Seeded {sessions.Count} sessions successfully.");
        }

        private async Task SeedInvoiceAsync()
        {
            _logger.Info("Seeding invoices...");

            var adminUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == "Admin@gmail.com");
            var sessions = await _context.Sessions
                .Where(s => !s.InvoiceTruongNNId.HasValue) // Chỉ lấy sessions chưa có invoice
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            if (adminUser == null || !sessions.Any())
            {
                _logger.Error("Admin user or uninvoiced sessions not found for invoice seeding.");
                return;
            }

            var invoices = new List<InvoiceTruongNN>();
            var random = new Random();

            // Tạo 10 invoices, mỗi invoice liên kết với 1 session duy nhất
            var invoiceCount = Math.Min(10, sessions.Count);

            for (int i = 0; i < invoiceCount; i++)
            {
                var session = sessions[i];

                if (!session.Cost.HasValue || !session.EndTime.HasValue)
                {
                    _logger.Warn($"Session {session.TruongNNID} skipped - missing cost or end time.");
                    continue;
                }

                var subtotalAmount = session.Cost.Value;
                var taxAmount = subtotalAmount * 0.1m; // VAT 10%
                var totalAmount = subtotalAmount + taxAmount;
                var isPaid = i < 7; // 7 invoices đầu đã thanh toán

                var invoice = new InvoiceTruongNN
                {
                    UserId = adminUser.TruongNNID,
                    SessionId = session.TruongNNID,
                    PeriodStart = session.StartTime,
                    PeriodEnd = session.EndTime.Value,
                    SubtotalAmount = subtotalAmount,
                    TaxAmount = taxAmount,
                    TotalAmount = totalAmount,
                    AmountPaid = isPaid ? totalAmount : 0,
                    Status = isPaid ? InvoiceStatus.Paid : InvoiceStatus.Outstanding,
                    DueDate = session.EndTime.Value.AddDays(7),
                    IssuedAt = session.EndTime.Value
                };

                invoices.Add(invoice);
                await _context.InvoiceTruongNN.AddAsync(invoice);
                await _context.SaveChangesAsync();

                // Cập nhật InvoiceId cho session
                session.InvoiceTruongNNId = invoice.TruongNNID;
            }

            await _context.SaveChangesAsync();

            _logger.Success($"Seeded {invoices.Count} invoices successfully. " +
                           $"Each invoice is linked to 1 session. " +
                           $"{sessions.Count - invoices.Count} sessions remain uninvoiced.");
        }
    }
}