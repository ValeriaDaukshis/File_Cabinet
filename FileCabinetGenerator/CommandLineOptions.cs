using CommandLine;

namespace FileCabinetGenerator
{
    public class CommandLineOptions
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
}