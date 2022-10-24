namespace Atomicity.Persistence;

public interface IPersistenceProvider
{
    int GetStartOperation(Guid transactionId);
    
    void Save(Guid transactionId, string operationName, int operationSequenceNumber);
}