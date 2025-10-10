using EVChargingStation.CARC.Infrastructure.TruongNN.Interfaces;
using EVChargingStation.CARC.Domain.TruongNN;

namespace EVChargingStation.CARC.Infrastructure.TruongNN;

public class UnitOfWork : IUnitOfWork
{
    private readonly FA25_SWD392_SE183428_G6_EvChargingStation _dbContext;

    public UnitOfWork(FA25_SWD392_SE183428_G6_EvChargingStation dbContext)
    {
        _dbContext = dbContext;
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}