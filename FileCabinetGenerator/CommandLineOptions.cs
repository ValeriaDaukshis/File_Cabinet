using CommandLine;

namespace FileCabinetGenerator
{
    public class CommandLineOptions
    {
        [Option('t', "output-type", Required = false, HelpText = "Output format type (csv, xml)")]
        public string OutputFormat { get; set; }

        [Option('o', "output-file", Required = false, HelpText = "Output file name.")]
        public string OutputFileName { get; set; }

        [Option('a', "records-amount", Required = false, HelpText = "Amount of generated records.")]
        public int RecordsAmount { get; set; }

        [Option('i', "start-id", Required = false, HelpText = "ID value to start.")]
        public int StartId { get; set; }
    }
}