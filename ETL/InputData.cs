using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ETL;

public class InputData
{
    public string? FirstName { get; set; }
    public string? SecondName { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? Flat { get; set; }
    public decimal Payment { get; set; }
    public DateTime Date { get; set; }
    public long AccountNumber { get; set; }
    public string? ServiceName { get; set; }

    public static bool TryParse(string line, out InputData? inputData)
    {
        var regex = new Regex(@"((\b[^,]+\b)((?<=\.\w).,)?)");
        var matches = regex.Matches(line);
        if (matches.Count == 9)
        {
            inputData = new InputData();
            inputData.FirstName = matches[0].Value;
            inputData.SecondName = matches[1].Value;
            inputData.City = matches[2].Value;
            inputData.Street = matches[3].Value;
            inputData.Flat = matches[4].Value;
            if (Decimal.TryParse(matches[5].Value, NumberStyles.Any,CultureInfo.InvariantCulture, out var payment))
                inputData.Payment = payment;
            else
            {
                inputData = null;
                return false;
            }
            if (DateTime.TryParseExact(matches[6].Value,"yyyy-dd-mm",CultureInfo.InvariantCulture,DateTimeStyles.None,out var date))
                inputData.Date = date;
            else
            {
                inputData = null;
                return false;
            }
            if (Int64.TryParse(matches[7].Value,out var accountNumber))
                inputData.AccountNumber = accountNumber;
            else
            {
                inputData = null;
                return false;
            }
            inputData.ServiceName = matches[8].Value;
        }
        else
        {
            inputData = null;
            return false;
        }
        return true;
    }
}