namespace ECommPay.PaymentPage.SDK;

public interface IDynamicObject
{
    public IDictionary<string, object> Dictionary();

    public string ToJson();
}