namespace ECommPay.PaymentPage.SDK;

public class UrlBuilder: UriBuilder
{
    private readonly ISignatureHandler _handler;

    public UrlBuilder(ISignatureHandler handler)
    {
        _handler = handler;
    }
    
    public string GetUrl(IPayment payment)
    {
        if (payment.IsTestMode())
        {
            payment.Dictionary().Add("inner_service", "editor");
            payment.Dictionary().Add("editorAction", "list");
        }

        var signature = _handler.Sign(payment);
        payment.Dictionary().Add("signature", signature);
        Query = payment.ToString();
        return ToString();
    }
}