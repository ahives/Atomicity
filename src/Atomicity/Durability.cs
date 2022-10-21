namespace Atomicity;

public class Durability :
    IDurability
{
    public int GetStartOperation(Guid transactionId)
    {
        return 0;
    }

    public void Save(Guid transactionId)
    {
    }
}