namespace Atomicity;

public interface IDurability
{
    int GetStartOperation(Guid transactionId);
    
    void Save(Guid transactionId);
}