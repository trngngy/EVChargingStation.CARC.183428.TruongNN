using EVChargingStation.CARC.Domain.TruongNN.Entities;

namespace EVChargingStation.CARC.Infrastructure.TruongNN.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<User> Users { get; }
    IGenericRepository<Session> Sessions { get; }
    IGenericRepository<InvoiceTruongNN> Invoices { get; }
    IGenericRepository<StationAnhDHV> Stations { get; }
    IGenericRepository<VehicleHuyPD> Vehicles { get; }
    IGenericRepository<ReservationLongLQ> Reservations { get; }
    IGenericRepository<Plan> Plans { get; }
    IGenericRepository<UserPlanHoaHTT> UserPlans { get; }

    Task<int> SaveChangesAsync();
}