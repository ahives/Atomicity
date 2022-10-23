namespace Atomicity.Persistence;

public class DurableTransactionProvider :
    IDurableTransactionProvider
{
    public int GetStartOperation(Guid transactionId)
    {
        return 0;
    }

    public void Save(Guid transactionId, string operationName, int operationSequenceNumber)
    {
    }
}