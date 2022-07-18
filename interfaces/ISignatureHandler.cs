namespace ECommPay.PaymentPage.SDK;

public interface ISignatureHandler
{
    public const string ItemsDelimiter = ";";
    public const string KeyValueDelimiter = ":";
    public const string Algorithm = "sha512";

    public bool Check(IDynamicObject parameters, string signature);
    public string Sign(IDynamicObject parameters);
    public ISignatureHandler SetSortParams(bool sort);
}