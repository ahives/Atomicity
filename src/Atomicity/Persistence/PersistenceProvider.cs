namespace Atomicity.Persistence;

public class PersistenceProvider :
    IPersistenceProvider
{
    public int GetStartOperation(Guid transactionId)
    {
        return 0;
    }

    public void Save(Guid transactionId, string operationName, int operationSequenceNumber)
    {
    }
}