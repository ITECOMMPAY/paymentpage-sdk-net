using System.ComponentModel.DataAnnotations;
using System.Net;
using ECommPay.PaymentPage.SDK.interfaces;

namespace ECommPay.PaymentPage.SDK;

public class Gate: IGate
{
    private const string ApiUrl = "https://sdk.ecommpay.com/params/check";
    private const string PpUrl = "https://paymentpage.ecommpay.com/payment";
    private readonly UrlBuilder _urlBuilder;
    private readonly Uri _apiUri;
    private readonly ISignatureHandler _handler;

    public Gate(string secret, string url = PpUrl)
    {
        _handler = new SignatureHandler(secret);
        _urlBuilder = new UrlBuilder(_handler);
        _apiUri = new Uri(ApiUrl);
        SetPaymentBaseUrl(url);
    }
    
    [Obsolete("SetPaymentBaseUrl is deprecated, please use Schema, Host and Port properties instead.")]
    public IGate SetPaymentBaseUrl(string url)
    {
        var uri = new Uri(url);
        _urlBuilder.Path = uri.AbsolutePath;
        _urlBuilder.Host = uri.Host;
        _urlBuilder.Scheme = uri.Scheme;
        return this;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="payment">Payment object</param>
    /// <returns>Uri for open payment form.</returns>
    /// <exception cref="ValidationException">On one or more payment parameters values fail.</exception> 
    public string GetPurchasePaymentPageUrl(IPayment payment)
    {
        if (payment.IsTestMode())
        {
            Validate(payment);
        }

        return _urlBuilder.GetUrl(payment);
    }

    public string GetPayload(IPayment payment)
    {
        if (payment.IsTestMode())
        {
            payment.Dictionary().Add("inner_service", "editor");
            payment.Dictionary().Add("editorAction", "list");
        }

        return payment.ToJson();
    }

    /// <summary>
    /// Return result of check correct parameters values in payment object 
    /// </summary>
    /// <param name="payment"></param>
    /// <exception cref="ValidationException"></exception>
    private void Validate(IPayment payment)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _apiUri);
        request.Content = new StringContent(payment.ToJson());
        var response = new HttpClient().Send(request);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new ValidationException();
        }
    }

    public ICallback HandleCallback(string data)
    {
        return Callback.FromJson(data, _handler);
    }
}