using AdventOfCode2023.Days;
using System.Reflection;

string rootPath = Directory.GetCurrentDirectory() + @"\";
bool TESTING = true;

string[] arguments = Console.ReadLine().Split(" ");
string day = "Day" + arguments[0];
if (arguments.Length > 1 && arguments[1] == "R")
    TESTING = false;

string targetFile = rootPath + (TESTING ? $"TestData\\{day}.txt" : $"Data\\{day}.txt");
string input = File.ReadAllText(targetFile);

Type? type = Assembly.GetExecutingAssembly().GetType($"AdventOfCode2023.Days.{day}");
MethodInfo? method = type?.GetMethod("Run");
Console.WriteLine(method?.Invoke(null, new object[] { input }));