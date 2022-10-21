namespace Atomicity;

public interface ITransactionDurability
{
    int GetStartOperation(Guid transactionId);
    
    void Save(Guid transactionId);
}