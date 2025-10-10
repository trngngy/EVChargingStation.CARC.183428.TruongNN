﻿namespace EVChargingStation.CARC.Infrastructure.TruongNN.Interfaces;

public interface IUnitOfWork : IDisposable
{
    //IGenericRepository<OtpStorage> OtpStorages { get; }
    //IGenericRepository<Movie> Movies { get; }
    //IGenericRepository<ShowTime> ShowTimes { get; }
    //IGenericRepository<Promotion> Promotions { get; }
    //IGenericRepository<CinemaRoom> CinemaRooms { get; }
    //IGenericRepository<AuditLog> AuditLogs { get; }

    Task<int> SaveChangesAsync();
}