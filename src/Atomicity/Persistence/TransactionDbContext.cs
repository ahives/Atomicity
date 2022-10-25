namespace Atomicity.Persistence;

using Microsoft.EntityFrameworkCore;

public class TransactionDbContext :
    DbContext
{
    public DbSet<TransactionEntity> Transactions { get; set; }

    public DbSet<OperationEntity> Operations { get; set; }
}