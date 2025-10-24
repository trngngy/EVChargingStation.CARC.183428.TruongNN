using EVChargingStation.CARC.Domain.TruongNN.Entities;

namespace EVChargingStation.CARC.Infrastructure.TruongNN.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<User> Users { get; }

    Task<int> SaveChangesAsync();
}