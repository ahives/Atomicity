namespace Atomicity.Data;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("Transactions")]
public class TransactionEntity
{
    [Column("Id"), Key, Required]
    public Guid TransactionId { get; init; }
}