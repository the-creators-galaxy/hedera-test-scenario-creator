using Hashgraph;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;

namespace create_token_test_environment
{
    /// <summary>
    /// Orchestrates the display of the command line user interface.  
    /// Each instance of this object is an individual grouping of values.
    /// </summary>
    public class ConsoleInterface
    {
        /// <summary>
        /// Title for section grouping.
        /// </summary>
        private string _title;
        /// <summary>
        /// Main color for the section grouping.
        /// </summary>
        private Color _color;
        /// <summary>
        /// List of data elements to display on the screen for this grouping.
        /// </summary>
        private List<(string key, string title, string value)> _data = new();
        /// <summary>
        /// Main entry point for drawing the main screen and list of currently 
        /// input values for a scenario creation.
        /// </summary>
        /// <param name="config">
        /// The input configuration as currently configured by the user.
        /// </param>
        /// <returns>
        /// An enumerable of renderable components for consumption by the 
        /// Spectre framework.  The sum total of  which describe how to 
        /// display the configuration information for the user interface.
        /// </returns>
        public static IRenderable CreateConfigRendering(InputConfiguration config)
        {
            var left = new List<Panel>();
            left.Add(new ConsoleInterface("Token", Color.Purple)
                .Add("1", "Symbol", config.TokenSymbol)
                .Add("2", "Name", config.TokenName)
                .Add("3", "Memo", config.TokenMemo)
                .Add("4", "Circulation", config.TokenCirculation)
                .Add("5", "Decimal Places", config.TokenDecimalPlaces)
                .Generate());
            left.Add(new ConsoleInterface("Recipients", Color.DarkTurquoise)
                .Add("6", "No. of Recipients", config.RecipientCount)
                .Add("7", "Initial Balance", config.RecipientInitialCryptoBalance)
                .Add("8", "Maximum Distribution", config.MaximumDistribution)
                .Add("9", "Minimum Distribution", config.MiniumDistribution)
                .Generate());

            var right = new List<Panel>();
            right.Add(new ConsoleInterface("Treasury", Color.SteelBlue)
                .Add("A", "Total Number of Keys", config.TreasuryTotalKeyCount)
                .Add("B", "Required Number of Keys", config.TreasuryRequiredSignatureCount)
                .Add("C", "Initial Balance", config.TreasuryInitialCryptoBalance)
                .Generate());
            right.Add(new ConsoleInterface("Distributions Payer", Color.DarkGoldenrod)
                .Add("D", "Initial Balance", config.DistributionPayerInitialCryptoBalance)
                .Generate());
            right.Add(new ConsoleInterface("Scheduling Payer", Color.Blue)
                .Add("E", "Initial Balance", config.SchedulingPayerInitialCryptoBalance)
                .Generate());

            var combined = new List<IRenderable>();
            combined.Add(new Rows(left));
            combined.Add(new Rows(right));

            var sections = new List<IRenderable>();
            sections.Add(new Columns(combined).Expand());
            sections.Add(new ConsoleInterface("Network Details", Color.Chartreuse3)
                .Add("F", "Hedera Node Address", config.Gateway.Url)
                .Add("G", "Hedera Node Account", config.Gateway)
                .Add("H", "Payer Account", config.Payer)
                .Add("J", "Payer Private Key", config.PrivateKey)
                .Generate());
            sections.Add(new ConsoleInterface("Outputs", Color.Yellow3)
                .Add("K", "CSV Distribution File", config.OutputCsvFile)
                .Add("M", "Generated Secrets File", config.OutputSecretsFile)
                .Generate());
            return new Columns(sections);
        }
        /// <summary>
        /// Private constructor.
        /// </summary>
        /// <param name="title">Title of the section</param>
        /// <param name="color">Theme Color for the section.</param>
        private ConsoleInterface(string title, Color color)
        {
            _title = title;
            _color = color;
        }
        /// <summary>
        /// Adds a string value to the section grouping.
        /// </summary>
        /// <param name="key">
        /// The “key” value for the data item, this is the value 
        /// the user types at the keyboard when they indicate 
        /// they want to modify the value of this setting.
        /// </param>
        /// <param name="title">
        /// The name/description of this data value item.
        /// </param>
        /// <param name="value">
        /// The current string value of this item.
        /// </param>
        /// <returns>
        /// A reference to this grouping instance, 
        /// to support fluent code patterns.
        /// </returns>
        private ConsoleInterface Add(string key, string title, string value)
        {
            _data.Add((key, title, value));
            return this;
        }
        /// <summary>
        /// Adds a long value to the section grouping.
        /// </summary>
        /// <param name="key">
        /// The “key” value for the data item, this is the value 
        /// the user types at the keyboard when they indicate 
        /// they want to modify the value of this setting.
        /// </param>
        /// <param name="title">
        /// The name/description of this data value item.
        /// </param>
        /// <param name="value">
        /// The current long value of this item.
        /// </param>
        /// <returns>
        /// A reference to this grouping instance, 
        /// to support fluent code patterns.
        /// </returns>
        private ConsoleInterface Add(string key, string title, long value)
        {
            _data.Add((key, title, value.ToString()));
            return this;
        }
        /// <summary>
        /// Adds a ulong value to the section grouping.
        /// </summary>
        /// <param name="key">
        /// The “key” value for the data item, this is the value 
        /// the user types at the keyboard when they indicate 
        /// they want to modify the value of this setting.
        /// </param>
        /// <param name="title">
        /// The name/description of this data value item.
        /// </param>
        /// <param name="value">
        /// The current ulong value of this item.
        /// </param>
        /// <returns>
        /// A reference to this grouping instance, 
        /// to support fluent code patterns.
        /// </returns>
        private ConsoleInterface Add(string key, string title, ulong value)
        {
            _data.Add((key, title, value.ToString()));
            return this;
        }
        /// <summary>
        /// Adds a hedera address value to the section grouping.
        /// </summary>
        /// <param name="key">
        /// The “key” value for the data item, this is the value 
        /// the user types at the keyboard when they indicate 
        /// they want to modify the value of this setting.
        /// </param>
        /// <param name="title">
        /// The name/description of this data value item.
        /// </param>
        /// <param name="value">
        /// The current hedera address value of this item.
        /// </param>
        /// <returns>
        /// A reference to this grouping instance, 
        /// to support fluent code patterns.
        /// </returns>
        private ConsoleInterface Add(string key, string title, Address address)
        {
            _data.Add((key, title, $"{address.AsString()}"));
            return this;
        }
        /// <summary>
        /// Adds a string value to the section grouping.
        /// </summary>
        /// <param name="key">
        /// The “key” byte array for the data item, this is the value 
        /// the user types at the keyboard when they indicate 
        /// they want to modify the value of this setting.
        /// </param>
        /// <param name="title">
        /// The name/description of this data value item.
        /// </param>
        /// <param name="value">
        /// The current byte array value of this item.
        /// </param>
        /// <returns>
        /// A reference to this grouping instance, 
        /// to support fluent code patterns.
        /// </returns>
        private ConsoleInterface Add(string key, string title, ReadOnlyMemory<byte> bytes)
        {
            _data.Add((key, title, Hex.FromBytes(bytes)));
            return this;
        }
        /// <summary>
        /// Generates the Spectre primitive components describing
        /// the layout and drawing of console user interface components 
        /// representing the data contained by this grouping.
        /// </summary>
        /// <returns></returns>
        private Panel Generate()
        {
            var color = _color.ToHex();
            var table = new Table() { ShowHeaders = false, Border = TableBorder.None };
            table.AddColumns(string.Empty, string.Empty, string.Empty);
            foreach (var row in _data)
            {
                table.AddRow($"[gray][[{row.key}]][/]", $"[#{color}]{row.title}[/]", $"[white]{row.value}[/]");
            }
            return new Panel(table)
                  .Header($"[#{color}]{_title}[/]")
                  .BorderColor(_color)
                  .Expand();
        }
        /// <summary>
        /// Main entry point for evaluating user input for the root 
        /// application decision tree for command and control.  
        /// It either dispatches a command for editing the configuration 
        /// or starts a simulation generation.
        /// </summary>
        /// <param name="config">
        /// Object holding the current state of the user entered 
        /// simulation configuration.
        /// </param>
        /// <param name="code">
        /// The char code entered by the user from the command line.
        /// </param>
        public static void DispatchEditRequest(InputConfiguration config, char code)
        {
            switch (char.ToUpper(code))
            {
                case '1':
                    Ask($"Please Enter a [white]token symbol[/]: ", config.TokenSymbol, value =>
                    {
                        config.TokenSymbol = value;
                        return true;
                    });
                    break;
                case '2':
                    Ask($"Please Enter a [white]token name[/]: ", config.TokenName, value =>
                    {
                        config.TokenName = value;
                        return true;
                    });
                    break;
                case '3':
                    Ask($"Please Enter a [white]token memo[/]: ", config.TokenMemo, value =>
                    {
                        config.TokenMemo = value;
                        return true;
                    });
                    break;
                case '4':
                    Ask($"Please Enter the initial token [white]circulation[/] of coins: ", config.TokenCirculation, value =>
                    {
                        if (ulong.TryParse(value, out ulong circulation) && circulation > 0)
                        {
                            config.TokenCirculation = circulation;
                            return true;
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Initial Token Circulation must be a valid whole number greater than zero.[/]");
                            return false;
                        }
                    });
                    break;
                case '5':
                    Ask($"Please Enter the initial token [white]decimal places[/]: ", config.TokenDecimalPlaces, value =>
                    {
                        if (uint.TryParse(value, out uint places))
                        {
                            config.TokenDecimalPlaces = places;
                            return true;
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Initial Token Circulation must be a valid whole number zero or larger.[/]");
                            return false;
                        }
                    });
                    break;
                case '6':
                    Ask($"Please Enter the [white]number of distribution recipients[/]: ", config.RecipientCount, value =>
                    {
                        if (long.TryParse(value, out long count) && count > 0)
                        {
                            config.RecipientCount = count;
                            return true;
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]The number of recipient acocunts must be a valid whole number greater than zero.[/]");
                            return false;
                        }
                    });
                    break;
                case '7':
                    Ask($"Please Enter the [white]initial crypto balance[/] for recipients: ", config.RecipientInitialCryptoBalance, value =>
                    {
                        if (ulong.TryParse(value, out ulong balance))
                        {
                            config.RecipientInitialCryptoBalance = balance;
                            return true;
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Initial Crypto Balance for Recipients must be a valid whole number zero or larger.[/]");
                            return false;
                        }
                    });
                    break;
                case '8':
                    Ask($"Please Enter the [white]maximum distribution[/] a recipient may receive: ", config.MaximumDistribution, value =>
                    {
                        if (long.TryParse(value, out long maximum) && maximum > 0 && maximum >= config.MiniumDistribution)
                        {
                            config.MaximumDistribution = maximum;
                            return true;
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]The Maximum Recipient Distribution must be greater or equal to the minimum distribution.[/]");
                            return false;
                        }
                    });
                    break;
                case '9':
                    Ask($"Please Enter the [white]minimum distribution[/] a recipient may receive: ", config.MiniumDistribution, value =>
                    {
                        if (long.TryParse(value, out long minimum) && minimum > 0 && minimum <= config.MaximumDistribution)
                        {
                            config.MiniumDistribution = minimum;
                            return true;
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]The Minimum Recipient Distribution must be greater or equal to zero and less than or equal to the maximum distribution.[/]");
                            return false;
                        }
                    });
                    break;
                case 'A':
                    Ask($"Please Enter the total [white]number of keys[/] associated with the treasury account: ", config.TreasuryTotalKeyCount, value =>
                    {
                        if (int.TryParse(value, out int count) && count > 0 && count > config.TreasuryRequiredSignatureCount)
                        {
                            config.TreasuryTotalKeyCount = count;
                            return true;
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]The Total Number of Keys must be a whole number greater than zero and equal to or greater than the Requred Number of Keys for signing.[/]");
                            return false;
                        }
                    });
                    break;
                case 'B':
                    Ask($"Please Enter the [white]number of signatures[/] required for signing treasury transactions: ", config.TreasuryRequiredSignatureCount, value =>
                    {
                        if (uint.TryParse(value, out uint count) && count > 0 && count <= config.TreasuryTotalKeyCount)
                        {
                            config.TreasuryRequiredSignatureCount = count;
                            return true;
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]The number of required signatures must be a valid whole number greater than zero and not more than the total number of treasury signing keys.[/]");
                            return false;
                        }
                    });
                    break;
                case 'C':
                    Ask($"Please Enter the [white]initial crypto balance[/] for the treasury account: ", config.TreasuryInitialCryptoBalance, value =>
                    {
                        if (ulong.TryParse(value, out ulong balance))
                        {
                            config.TreasuryInitialCryptoBalance = balance;
                            return true;
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Initial Crypto Balance for the Treasury Account must be a valid whole number zero or larger.[/]");
                            return false;
                        }
                    });
                    break;
                case 'D':
                    Ask($"Please Enter the [white]initial crypto balance[/] for the distributions payer account: ", config.DistributionPayerInitialCryptoBalance, value =>
                    {
                        if (ulong.TryParse(value, out ulong balance))
                        {
                            config.DistributionPayerInitialCryptoBalance = balance;
                            return true;
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Initial Crypto Balance for the Treasury Account must be a valid whole number zero or larger.[/]");
                            return false;
                        }
                    });
                    break;
                case 'E':
                    Ask($"Please Enter the [white]initial crypto balance[/] for the scheduling payer account: ", config.SchedulingPayerInitialCryptoBalance, value =>
                    {
                        if (ulong.TryParse(value, out ulong balance))
                        {
                            config.SchedulingPayerInitialCryptoBalance = balance;
                            return true;
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Initial Crypto Balance for the Scheduling Payer Account must be a valid whole number zero or larger.[/]");
                            return false;
                        }
                    });
                    break;
                case 'F':
                    Ask($"Please Enter the URL for the [white]Gateway[/] node in the form of [blue]<address:port>[/]: ", config.Gateway.Url, value =>
                    {
                        config.Gateway = new Gateway(value, config.Gateway);
                        return true;
                    });
                    break;
                case 'G':
                    Ask($"Please Enter the wallet address for the [white]Gateway[/] node in the form of [blue]<shard.realm.num>[/]: ", $"{config.Gateway.ShardNum}.{config.Gateway.RealmNum}.{config.Gateway.AccountNum}", value =>
                    {
                        var address = ParseAddress(value);
                        if (address != Address.None)
                        {
                            config.Gateway = new Gateway(config.Gateway.Url, address);
                            return true;
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Not a valid Hedera Wallet Address, it should be in the form of [yellow]<shard>.<realm>.<number>[/].[/]");
                            return false;
                        }
                    });
                    break;
                case 'H':
                    Ask($"Please Enter the wallet address for the [white]Payer Account[/] paying the fees to create the scenario assets: ", $"{config.Payer.AsString()}", value =>
                    {
                        var address = ParseAddress(value);
                        if (address != Address.None)
                        {
                            config.Payer = address;
                            return true;
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Not a valid Hedera Wallet Address, it should be in the form of [yellow]<shard>.<realm>.<number>[/].[/]");
                            return false;
                        }
                    });
                    break;
                case 'J':
                    Ask($"Please Enter [white]Private Key[/] for the account paying the fees to create the scenario assets: ", Hex.FromBytes(config.PrivateKey), value =>
                    {
                        try
                        {
                            var bytes = Hex.ToBytes(value);
                            new Signatory(bytes);
                            config.PrivateKey = bytes;
                            return true;
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"[red]Parsing Error: {ex.Message}[/]");
                            return false;
                        }
                    });
                    break;
                case 'K':
                    Ask($"Please Enter the path to the [white]CSV Distribution[/] file: ", config.OutputCsvFile, value =>
                    {
                        config.OutputCsvFile = value;
                        return true;
                    });
                    break;
                case 'M':
                    Ask($"Please Enter the path to the [white]Generated Secrets[/] file: ", config.OutputSecretsFile, value =>
                    {
                        config.OutputSecretsFile = value;
                        return true;
                    });
                    break;
            }
        }
        /// <summary>
        /// Helper function to parse a hedera address from user input.
        /// </summary>
        /// <param name="value">
        /// A string that should represent an hedera address in 
        /// shard.realm.num form.
        /// </param>
        /// <returns>
        /// An hedera address if parse-able, otherwise the None value. 
        /// </returns>
        private static Address ParseAddress(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var parts = value.Split('.');
                if (parts.Length == 3 &&
                    long.TryParse(parts[0], out long shard) && shard >= 0 &&
                    long.TryParse(parts[1], out long realm) && realm >= 0 &&
                    long.TryParse(parts[2], out long num) && num >= 0)
                {
                    return new Address(shard, realm, num);
                }
            }
            return Address.None;
        }
        /// <summary>
        /// Prompts the user for a new value for a ulong data item.
        /// </summary>
        /// <param name="message">
        /// The message prompt.
        /// </param>
        /// <param name="currentValue">
        /// The current value of the data item.
        /// </param>
        /// <param name="accept">
        /// Callback function that returns true if the newly input 
        /// data is valid and can be accepted.
        /// </param>
        static void Ask(string message, ulong currentValue, Func<string, bool> accept)
        {
            Ask(message, currentValue.ToString(), accept);
        }
        /// <summary>
        /// Prompts the user for a new value for a long data item.
        /// </summary>
        /// <param name="message">
        /// The message prompt.
        /// </param>
        /// <param name="currentValue">
        /// The current value of the data item.
        /// </param>
        /// <param name="accept">
        /// Callback function that returns true if the newly input 
        /// data is valid and can be accepted.
        /// </param>
        static void Ask(string message, long currentValue, Func<string, bool> accept)
        {
            Ask(message, currentValue.ToString(), accept);
        }
        /// <summary>
        /// Prompts the user for a new value for a string data item.
        /// </summary>
        /// <param name="message">
        /// The message prompt.
        /// </param>
        /// <param name="currentValue">
        /// The current value of the data item.
        /// </param>
        /// <param name="accept">
        /// Callback function that returns true if the newly input 
        /// data is valid and can be accepted.
        /// </param>
        static void Ask(string message, string currentValue, Func<string, bool> accept)
        {
            while (true)
            {
                var test = AnsiConsole.Ask(message, currentValue);
                if (!string.IsNullOrWhiteSpace(test))
                {
                    if (accept(test))
                    {
                        return;
                    }
                }
            }
        }
    }
}
