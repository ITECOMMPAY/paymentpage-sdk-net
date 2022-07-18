namespace ECommPay.PaymentPage.SDK;

public interface IPayment: IDynamicObject
{
    public const string PurchaseType = "purchase";
    public const string PayoutType = "payout";
    public const string RecurringType = "recurring";
    public const string Mode = "modal";
    public const string InterfaceType = "{\"id\":29}";

    public IPayment SetPaymentAmount(int amount);
    public IPayment SetPaymentCurrency(string currency);
    public IPayment SetPaymentDescription(string description);

    public IPayment SetTestMode();

    public bool IsTestMode();

    public string ToString();

    public string PaymentId();
}