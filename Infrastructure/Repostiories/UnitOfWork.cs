namespace Infrastructure.Repostiories
{
    public class UnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync() 
        {
            await _context.SaveChangesAsync();
        }

        public async Task BeginTransaction()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollBackAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }
    }
}
