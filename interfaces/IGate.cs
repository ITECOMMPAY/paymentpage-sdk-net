namespace ECommPay.PaymentPage.SDK.interfaces;

public interface IGate
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public IGate SetPaymentBaseUrl(string url = "");

    /// <summary>
    /// 
    /// </summary>
    /// <param name="payment"></param>
    /// <returns></returns>
    public string GetPurchasePaymentPageUrl(IPayment payment);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="payment"></param>
    /// <returns></returns>
    public string GetPayload(IPayment payment);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public ICallback HandleCallback(string data);
}