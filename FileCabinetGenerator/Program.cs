using System;
using System.Globalization;
using CommandLine;

namespace FileCabinetGenerator
{
    class Program
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private static string _outputFormat;
        private static string _outputFileName;
        private static int _recordsAmount;
        private static int _startId;
        private class CommandLineOptions
        {
            [Option('t', "output-type", Required = true, HelpText = "Output format type (csv, xml)")]
            public string OutputFormat { get; set; }

            [Option('o', "output", Required = true, HelpText = "Output file name.")]
            public string OutputFileName { get; set; }
            
            [Option('a', "records-amount", Required = true, HelpText = "Amount of generated records.")]
            public int RecordsAmount { get; set; }

            [Option('i', "start-id", Required = true, HelpText = "ID value to start.")]
            public int StartId { get; set; }
        }
        
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(o =>
                {
                    o.OutputFormat = o.OutputFormat.ToLower(Culture);
                    if (o.OutputFormat != "xml" && o.OutputFormat != "csv")
                    {
                        Console.WriteLine($"Invalid output format type {o.OutputFormat}");
                        return;
                    }
                    
                    if (string.IsNullOrEmpty(o.OutputFileName))
                    {
                        Console.WriteLine($"Invalid output file name {o.OutputFileName}");
                        return;
                    }

                    if (o.RecordsAmount <= 0)
                    {
                        Console.WriteLine($"Invalid records amount {o.RecordsAmount}");
                        return;
                    }
                    
                    if (o.StartId <= 0)
                    {
                        Console.WriteLine($"Invalid id {o.StartId}");
                        return;
                    }
                    _outputFormat = o.OutputFormat;
                    _outputFileName = o.OutputFileName;
                    _recordsAmount = o.RecordsAmount;
                    _startId = o.StartId;
                });
        }
    }
}