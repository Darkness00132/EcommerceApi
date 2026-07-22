using Application.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repostiories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync(CancellationToken ct = default) 
        {
            await _context.SaveChangesAsync(ct);
        }


        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
        {
            return await _context.Database.BeginTransactionAsync(ct);
        }
    }
}
