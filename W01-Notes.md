# W01 Assignment: .NET Applications

## Environment

- .NET SDK: 8.0.427
- Target framework: net8.0
- Editor: Visual Studio Code

## Component 1: ContosoPizza Web API

The API uses ASP.NET Core controllers and an in-memory PizzaService.
Pepperoni was added to the initial seed data with ID 3.
The initial pizzas are restored whenever the application restarts.

### GET: Initial pizzas and additional seed pizza

Request:

```http
GET http://localhost:5135/pizza
Accept: application/json
```

Response status: 200 OK

Response body:

```json
[
  {"id":1,"name":"Classic Italian","isGlutenFree":false},
  {"id":2,"name":"Veggie","isGlutenFree":true},
  {"id":3,"name":"Pepperoni","isGlutenFree":false}
]
```

### POST: Create a pizza

Request:

```http
POST http://localhost:5135/pizza
Content-Type: application/json

{"name":"Hawaii","isGlutenFree":false}
```

Response status: 201 Created

Location header: http://localhost:5135/Pizza/4

Response body:

```json
{"id":4,"name":"Hawaii","isGlutenFree":false}
```

### PUT: Update the pizza

Request:

```http
PUT http://localhost:5135/pizza/4
Content-Type: application/json

{"id":4,"name":"Hawaiian","isGlutenFree":false}
```

Response status: 204 No Content

Response body: Empty.

Verification request:

```http
GET http://localhost:5135/pizza/4
```

Verification response status: 200 OK

Verification response body:

```json
{"id":4,"name":"Hawaiian","isGlutenFree":false}
```

### DELETE: Remove the pizza

Request:

```http
DELETE http://localhost:5135/pizza/4
```

Request body: Empty.

Response status: 204 No Content

Response body: Empty.

Verification request:

```http
GET http://localhost:5135/pizza/4
```

Verification response status: 404 Not Found

Verification response body:

```json
{
  "type":"https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title":"Not Found",
  "status":404,
  "traceId":"00-a788a53510964081a63d47acd1885b13-2bfd70dfc415cd6a-00"
}
```

## Component 2: Sales Summary Report

Project: mslearn-dotnet-files

Generated report: mslearn-dotnet-files/salesTotalDir/sales-summary.txt

The program reads the Total field from each sales.json file.
It uses decimal values for monetary calculations.
Files named salestotals.json use a different field, OverallTotal,
and are excluded from this report.

Verified report:

```text
Sales Summary
----------------------------
 Total Sales: $2,012.20

 Details:
 201\sales.json: $501.22
 202\sales.json: $1,234.22
 203\sales.json: $99.00
 204\sales.json: $88.88
 sales.json: $88.88
```

### Working sales summary function

This function uses the ReadSalesTotal helper shown below.
System.Globalization and System.Text are imported in Program.cs.

```csharp
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
```

Supporting helper and data model, using Newtonsoft.Json:

```csharp
decimal ReadSalesTotal(string filePath)
{
    string salesJson = File.ReadAllText(filePath);

    SalesData data = JsonConvert.DeserializeObject<SalesData>(salesJson)
        ?? throw new InvalidDataException(
            $"Could not read sales data from: {filePath}");

    return data.Total;
}

record SalesData(
    [property: JsonProperty(Required = Required.Always)] decimal Total);
```