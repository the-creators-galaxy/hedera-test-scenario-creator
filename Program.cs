using Spectre.Console;
using System;
using System.Threading.Tasks;

namespace create_token_test_environment
{
    /// <summary>
    /// Main entry class for the token scenario creator.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main entry point for the token scenario creator.
        /// </summary>
        static async Task Main()
        {
            try
            {
                var config = new InputConfiguration();
                while (true)
                {
                    AnsiConsole.Clear();
                    AnsiConsole.Write(new Rule() { Border = BoxBorder.Double, Style = Style.Parse("#082") });
                    AnsiConsole.Write(new FigletText("Test Distribution Scenario Creator") { Color = Color.Aqua });
                    AnsiConsole.Write(ConsoleInterface.CreateConfigRendering(config));
                    AnsiConsole.Write(new Rule() { Border = BoxBorder.Double, Style = Style.Parse("#082") });
                    AnsiConsole.Write(new Markup("Enter [red][[Esc]][/] to quit, [yellow][[1-M]][/] to edit a configuration, then press [green][[Enter]][/] to generate:"));
                    AnsiConsole.WriteLine();
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        AnsiConsole.WriteLine();
                        var result = await Generator.CreateTestEnvironment(config);
                        AnsiConsole.MarkupLine("[white]Completed.[/]");
                        break;
                    }
                    else
                    {
                        ConsoleInterface.DispatchEditRequest(config, key.KeyChar);
                    }
                    AnsiConsole.Clear();
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks);
                Console.WriteLine("Exit due to Error.");
                Environment.Exit(1);
            }
        }
    }
}
