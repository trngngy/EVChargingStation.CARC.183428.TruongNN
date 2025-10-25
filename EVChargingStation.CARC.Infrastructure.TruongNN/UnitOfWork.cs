using EVChargingStation.CARC.Infrastructure.TruongNN.Interfaces;
using EVChargingStation.CARC.Domain.TruongNN;
using EVChargingStation.CARC.Domain.TruongNN.Entities;

namespace EVChargingStation.CARC.Infrastructure.TruongNN;

public class UnitOfWork : IUnitOfWork
{
    private readonly FA25_SWD392_SE183428_G6_EvChargingStation _dbContext;

    public UnitOfWork(FA25_SWD392_SE183428_G6_EvChargingStation dbContext,
        IGenericRepository<User> userRepository,
        IGenericRepository<Session> sessionRepository,
        IGenericRepository<InvoiceTruongNN> invoiceRepository,
        IGenericRepository<StationAnhDHV> stationRepository,
        IGenericRepository<ReservationLongLQ> reservationRepository,
        IGenericRepository<VehicleHuyPD> vehicleRepository,
        IGenericRepository<Plan> planRepository,
        IGenericRepository<UserPlanHoaHTT> userPlanRepository)
    {
        _dbContext = dbContext;
        Users = userRepository;
        Sessions = sessionRepository;
        Invoices = invoiceRepository;
        Stations = stationRepository;
        Vehicles = vehicleRepository;
        Reservations = reservationRepository;
        Plans = planRepository;
        UserPlans = userPlanRepository;
    }

    public IGenericRepository<User> Users { get; }

    public IGenericRepository<Session> Sessions { get; }

    public IGenericRepository<InvoiceTruongNN> Invoices { get; }

    public IGenericRepository<StationAnhDHV> Stations { get; }

    public IGenericRepository<VehicleHuyPD> Vehicles { get; }

    public IGenericRepository<ReservationLongLQ> Reservations { get; }

    public IGenericRepository<Plan> Plans { get; }

    public IGenericRepository<UserPlanHoaHTT> UserPlans { get; }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}