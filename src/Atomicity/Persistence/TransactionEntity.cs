namespace Atomicity.Persistence;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Transactions")]
public class TransactionEntity
{
    [Column("Id"), Key, Required]
    public Guid Id { get; init; }
    
    [Column("OperationId")]
    public Guid OperationId { get; init; }
    
    [Column("CreationTimestamp")]
    public DateTimeOffset CreationTimestamp { get; init; }
}