using System.Text.Json.Serialization;

namespace ETL;

public class OutputData
{
    public string City { get; set; } = String.Empty;
    public List<Service> Services { get; set; } = new();
    public decimal Total { get; set; }

    public static OutputData Map(List<InputData>? inputData)
    {
        if (inputData.Count == 0)
            return new OutputData();
        var data = new OutputData();
        data.City = inputData.First().City;
        data.Services = inputData
            .DistinctBy(x => x.ServiceName)
            .Select(x => new Service
        {
            Name = x.ServiceName,
            Payers = inputData
                .Where(s => s.ServiceName == x.ServiceName)
                .Select(p => new Payer
                {
                    Date = p.Date,
                    Name = $"{p.FirstName} {p.SecondName}",
                    Payment = p.Payment,
                    AccountNumber = p.AccountNumber
                }).ToList(),
            Total = inputData
                .Where(s => s.ServiceName == x.ServiceName)
                .Sum(pay => pay.Payment)
        }).ToList();
        data.Total = data.Services.Sum(x => x.Total);
        return data;
    }
}

public class Service
{
    public string Name { get; set; } = String.Empty;
    public List<Payer> Payers { get; set; } = new();
    public decimal Total { get; set; }
}

public class Payer
{
    public string Name { get; set; } = String.Empty;
    public decimal Payment { get; set; }
    public DateTime Date { get; set; }
    [JsonPropertyName("account_number")]
    public long AccountNumber { get; set; }
}