using System.Dynamic;
using System.Text.Json;

namespace ECommPay.PaymentPage.SDK;

public class Callback: DynamicObject, ICallback, IDynamicObject
{
    // The inner dictionary.
    private readonly IDictionary<string, object> _dictionary;
    private readonly ISignatureHandler _handler;
    private readonly string? _signature;

    // This property returns the number of elements
    // in the inner dictionary.
    public int Count => _dictionary.Count;

    public Callback(IDictionary<string, object> data, ISignatureHandler handler)
    {
        _handler = handler;
        _dictionary = data;
        _signature = SetSignature();
    }

    /// <summary>
    /// Create payment object from JSON data and return.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    /// <exception cref="JsonException"></exception>
    public static ICallback FromJson(string data, ISignatureHandler handler)
    {
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(data)
                         ??  throw new JsonException("Deserialize failed.");
        return new Callback(dictionary, handler);
    }

    private string SetSignature()
    {
        if (_dictionary.TryGetValue("signature", out var signature)
            || _dictionary.TryGetValue("general.signature", out signature))
        {
            return signature.ToString() ?? throw new InvalidOperationException("Signature is undefined");
        }

        throw new InvalidOperationException("Signature not found in callback data");
    }

    public IDictionary<string, object> Dictionary()
    {
        return _dictionary;
    }

    public object GetPayment()
    {
        if (_dictionary.TryGetValue("payment", out var payment))
        {
            return payment;
        }
        
        throw new Exception("Payment is not available.");
    }

    public string GetPaymentStatus()
    {
        var payment = GetPayment();

        if (payment is not JsonElement {ValueKind: JsonValueKind.Object} element)
        {
            throw new InvalidOperationException("Payment status is not available.");
        }

        foreach (var property in element.EnumerateObject().Where(property => property.Name == "status"))
        {
            return property.Value.ToString();
        }

        throw new InvalidOperationException("Payment status is undefined.");
    }

    public string GetPaymentId()
    {
        var payment = GetPayment();

        if (payment is not JsonElement {ValueKind: JsonValueKind.Object} element)
        {
            throw new InvalidOperationException("Payment identifier is not available.");
        }

        foreach (var property in element.EnumerateObject().Where(property => property.Name == "id"))
        {
            return property.Value.ToString();
        }

        throw new InvalidOperationException("Payment identifier is undefined.");
    }
    
    public string GetSignature()
    {
        if (_signature == null)
        {
            throw new Exception("Signature is not available.");
        }

        return _signature;
    }

    public bool CheckSignature()
    {
        return _handler.Check(this, this.GetSignature());
    }
    
    public string ToJson()
    {
        return JsonSerializer.Serialize(_dictionary);
    }
}