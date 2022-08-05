// See https://aka.ms/new-console-template for more information


using ETL;

var directoryReader = new DirectoryReader();
//var input = await directoryReader.ReadAsync(@"C:\Users\User\RiderProjects\Radency.ConsoleApp\Radency.ConsoleApp\folder_a");
var a = Directory.Exists($@"..\..\..\folder_b");
Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy"));
await directoryReader.ReadAsync(@"..\..\..\folder_a");