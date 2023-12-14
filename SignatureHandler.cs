using System.Security.Cryptography;

namespace ECommPay.PaymentPage.SDK;

public class SignatureHandler : ISignatureHandler
{
    private readonly string _secretKey;
    private bool _sortParams = true;
    private readonly IList<string> _ignoredKeys;

    public SignatureHandler(string secretKey)
    {
        _secretKey = secretKey;
        _ignoredKeys = new List<string>();
        _ignoredKeys.Add("frame_mode");
        _ignoredKeys.Add("signature");
    }

    public ISignatureHandler SetSortParams(bool sort)
    {
        _sortParams = sort;
        return this;
    }

    public bool Check(IDynamicObject parameters, string signature)
    {
        var check = Sign(parameters);
        return Sign(parameters) == signature;
    }

    public string Sign(IDynamicObject parameters)
    {
        var options = string.Join(ISignatureHandler.ItemsDelimiter, GetParamsToSign(parameters.Dictionary(), _ignoredKeys));
        var encoding = new System.Text.ASCIIEncoding();
        var hash = new HMACSHA512(encoding.GetBytes(_secretKey));

        return Convert.ToBase64String(hash.ComputeHash(encoding.GetBytes(options)));
    }

    private string[] GetParamsToSign(
        IDictionary<string, object> parameters,
        ICollection<string> ignoreKeys
    )
    {
        var param = GetListParamsToSign(parameters, ignoreKeys);
        return _sortParams ? param.OrderBy(s => s).ToArray() : param.ToArray();
    }

    private static IEnumerable<string> GetListParamsToSign(
        IDictionary<string, object> parameters,
        ICollection<string> ignoreKeys,
        string prefix = ""
    )
    {
        IList<string> result = new List<string>();

        foreach (var (key, value) in parameters)
        {
            if (ignoreKeys.Contains(key))
            {
                continue;
            }

            var paramKey = (prefix != "" ? prefix +  ISignatureHandler.KeyValueDelimiter : "") + key.Replace(":", "::");
            var realValue = value;

            if (bool.TryParse(realValue.ToString(), out var boolVal))
            {
                realValue = boolVal ? "1" : "0";
            }

            result.Add(paramKey + ISignatureHandler.KeyValueDelimiter + realValue);
        }
        
        return result;
    }
}