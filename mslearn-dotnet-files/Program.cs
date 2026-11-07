using Newtonsoft.Json;
using System.Globalization;
using System.Text;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

var salesFiles = FindFiles(storesDirectory)
    .OrderBy(file => file)
    .ToList();

var salesTotal = CalculateSalesTotal(salesFiles);

File.AppendAllText(
    Path.Combine(salesTotalDir, "totals.txt"),
    $"{salesTotal.ToString(CultureInfo.InvariantCulture)}{Environment.NewLine}");

var reportPath = Path.Combine(salesTotalDir, "sales-summary.txt");

GenerateSalesSummary(salesFiles, storesDirectory, reportPath);

Console.WriteLine(
    $"Total Sales: {salesTotal.ToString("C", CultureInfo.GetCultureInfo("en-US"))}");

Console.WriteLine($"Report saved to: {reportPath}");

IEnumerable<string> FindFiles(string folderName)
{
    return Directory.EnumerateFiles(
        folderName,
        "sales.json",
        SearchOption.AllDirectories);
}

decimal ReadSalesTotal(string filePath)
{
    string salesJson = File.ReadAllText(filePath);

    SalesData data = JsonConvert.DeserializeObject<SalesData>(salesJson)
        ?? throw new InvalidDataException(
            $"Could not read sales data from: {filePath}");

    return data.Total;
}

decimal CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    decimal total = 0;

    foreach (var file in salesFiles)
    {
        total += ReadSalesTotal(file);
    }

    return total;
}

void GenerateSalesSummary(
    IEnumerable<string> salesFiles,
    string storesDirectory,
    string reportPath)
{
    var currencyCulture = CultureInfo.GetCultureInfo("en-US");
    var details = new StringBuilder();
    decimal grandTotal = 0;

    foreach (var file in salesFiles)
    {
        decimal fileTotal = ReadSalesTotal(file);
        grandTotal += fileTotal;

        string relativePath = Path.GetRelativePath(storesDirectory, file);

        details.AppendLine(
            $" {relativePath}: {fileTotal.ToString("C", currencyCulture)}");
    }

    var report = new StringBuilder();

    report.AppendLine("Sales Summary");
    report.AppendLine("----------------------------");
    report.AppendLine(
        $" Total Sales: {grandTotal.ToString("C", currencyCulture)}");
    report.AppendLine();
    report.AppendLine(" Details:");
    report.Append(details);

    File.WriteAllText(reportPath, report.ToString());
}

record SalesData(
    [property: JsonProperty(Required = Required.Always)] decimal Total);