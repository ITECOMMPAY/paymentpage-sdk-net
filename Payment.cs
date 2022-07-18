using System.Dynamic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;

namespace ECommPay.PaymentPage.SDK;

/// <summary>
/// The class derived from DynamicObject.
/// 
/// </summary>
public class Payment : DynamicObject, IPayment
{
    // The inner dictionary.
    private readonly IDictionary<string, object> _dictionary = new Dictionary<string, object>();

    // This property returns the number of elements
    // in the inner dictionary.
    public int Count => _dictionary.Count;

    private bool _testMode;
    
    public Payment(int projectId, string? paymentId = null)
    {
        _testMode = false;
        _dictionary.Add("project_id", projectId);
        _dictionary.Add("interface_type", IPayment.InterfaceType);
        // _dictionary.Add("payment_type", IPayment.PayoutType);
        _dictionary.Add("payment_id", paymentId ?? GenerateId());
    }

    public IPayment SetPaymentAmount(int amount)
    {
        _dictionary.Add("payment_amount", amount);

        return this;
    }

    public IPayment SetPaymentCurrency(string currency)
    {
        _dictionary.Add("payment_currency", currency);

        return this;
    }

    public IPayment SetPaymentDescription(string description)
    {
        _dictionary.Add("payment_description", description);

        return this;
    }

    public IPayment SetTestMode()
    {
        _testMode = true;

        return this;
    }

    public bool IsTestMode()
    {
        return _testMode;
    }

    public string PaymentId()
    {
        if (_dictionary.TryGetValue("payment_id", out var value))
        {
            return value.ToString() ?? "undefined";
        }

        return "undefined";
    }

    private Payment(IDictionary<string, object> dictionary)
    {
        _dictionary = dictionary;
    }

    /// <summary>
    /// Create payment object from JSON data and return.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static IPayment FromJson(string data)
    {
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(data)
                         ??  throw new Exception("Deserialize failed.");
        return new Payment(dictionary);
    }

    // If you try to get a value of a property
    // not defined in the class, this method is called.
    public override bool TryGetMember(
        GetMemberBinder binder, out object? result)
    {
        // Converting the property name to lowercase
        // so that property names become case-insensitive.
        var name = binder.Name.ToLower();

        // If the property name is found in a dictionary,
        // set the result parameter to the property value and return true.
        // Otherwise, return false.
        return _dictionary.TryGetValue(name, out result);
    }

    // If you try to set a value of a property that is
    // not defined in the class, this method is called.
    public override bool TrySetMember(
        SetMemberBinder binder, object? value)
    {
        if (value == null)
        {
            return false;
        }

        var name = binder.Name.ToLower();

        // Converting the property name to lowercase
        // so that property names become case-insensitive.
        _dictionary[name] = value;

        // You can always add a value to a dictionary,
        // so this method always returns true.
        return true;
    }

    public IDictionary<string, object> Dictionary()
    {
        return _dictionary;
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(_dictionary);
    }

    public override string ToString()
    {
        var result = new List<string>();
        foreach (var (key, value) in _dictionary)
        {
            result.Add(HttpUtility.UrlEncode(key) + "=" + HttpUtility.UrlEncode(value.ToString() ?? ""));
        }

        var queryString = string.Join('&', result);
        Regex reg = new Regex(@"%[a-f0-9]{2}");
        return reg.Replace(queryString, m => m.Value.ToUpperInvariant());
    }

    private static string GenerateId()
    {
        return Guid.NewGuid().ToString();
    }
}