string name = "Wilfredo";
Console.WriteLine($"Hello, {name}!");

DateTime today = DateTime.Today;
DateTime christmas = new DateTime(today.Year, 12, 25);

if (today > christmas)
{
    christmas = christmas.AddYears(1);
}

int daysUntilChristmas = (christmas - today).Days;

Console.WriteLine($"There are {daysUntilChristmas} days until Christmas!");