using Newtonsoft.Json.Linq;

namespace ECommPay.PaymentPage.SDK;

public interface ICallback
{
    public const string SuccessStatus = "succes";
    public const string DeclineStatus = "decline";
    public const string Aw3dsStats = "awaiting 3ds result";
    public const string AwRedStatus = "awaiting redirect result";
    public const string AwCusStatus = "awaiting customer";
    public const string AwClaStatus = "awaiting clarification";
    public const string AwCapStatus = "awaiting capture";
    public const string CancelledStatus = "cancelled";
    public const string RefundedStatus = "refunded";
    public const string PartRefundedStatus = "partially refunded";
    public const string ProcessingStatus = "processing";
    public const string ErrorStatus = "error";
    public const string ReversedStatus = "reversed";

    public object GetPayment();

    public string GetPaymentStatus();

    public string GetPaymentId();

    public string GetSignature();

    public bool CheckSignature();
}