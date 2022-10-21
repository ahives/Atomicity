namespace Atomicity;

public class TransactionDurability :
    ITransactionDurability
{
    public int GetStartOperation(Guid transactionId)
    {
        return 0;
    }

    public void Save(Guid transactionId)
    {
    }
}