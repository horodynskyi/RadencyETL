// See https://aka.ms/new-console-template for more information

using System.Globalization;
using System.Text.RegularExpressions;
using ETL;
using Newtonsoft.Json;

var line = "John, Doe, “Lviv, Kleparivska 35, 4”,  500.0, 2022-27-01, 1234567, Water";
var dir = AppDomain.CurrentDomain;

var directoryReader = new DirectoryReader();
var input = await directoryReader.ReadAsync(@"C:\Users\User\RiderProjects\Radency.ConsoleApp\Radency.ConsoleApp\folder_a");
var output = OutputData.Map(input);
Console.WriteLine(JsonConvert.SerializeObject(output));