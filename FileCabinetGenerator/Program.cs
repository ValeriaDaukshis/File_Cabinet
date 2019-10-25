using System;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using CommandLine;
using FileCabinetApp;
using FileCabinetGenerator.FileImporters;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Enter point.
    /// </summary>
    class Program
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private static IFileCabinetSerializer serializer;
        private static string _outputFormat;
        private static string _outputFileName;
        private static int _recordsAmount;
        private static int _startId;
        private static string firstName = "FirstName";
        private static string lastName = "LastName";
        private static int durationMinValue = 6;
        private static int durationMaxValue = 120;
        private static int creditSumMinValue = 10;
        private static int creditSumMaxValue = 5000;
        private static DateTime birthDayMinValue = new DateTime(1950, 01, 01);
        
        [XmlElement("record")]
        public decimal CreditSum { get; private set; }
        private static FileCabinetRecords fileCabinetRecords = new FileCabinetRecords();
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
        
        /// <summary>
        /// Main.
        /// </summary>
        /// <param name="args"></param>
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
            GenerateFileCabinetRecordsFields();
            ExportToFile();
            Console.WriteLine($"{_recordsAmount} records were written to {_outputFileName}");
        }
        
        private static void ExportToFile()
        {
            try
            {
                using (StreamWriter fs = new StreamWriter(_outputFileName))
                {
                    if (_outputFormat == "csv")
                    {
                        serializer = new ImportToCsv(fs);
                    }
                    else if (_outputFormat == "xml")
                    {
                        serializer = new ImportToXml(fs);
                    }
                    serializer.Serialize(fileCabinetRecords);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        private static void GenerateFileCabinetRecordsFields()
        {
            int userId = _startId;
            for (int i = 0; i < _recordsAmount; i++)
            {
                GenerateFields(i, out string firstName, out string lastName, out char gender, out DateTime dateOfBirth, out decimal credit, out short duration);
                Record record = new Record(userId, firstName, lastName, gender, dateOfBirth, credit, duration);
                fileCabinetRecords.Record.Add(record);
                userId++;
            }
        }
        
        private static void GenerateFields(int i,out string firstName, out string lastName, out char gender, out DateTime dateOfBirth, out decimal credit, out short duration)
        {
            firstName = Program.firstName + i;
            lastName = Program.lastName + i;
            gender = i % 5 == 0 ? 'F' : 'M';
            dateOfBirth = RandomBirthDay();
            credit = new Random().Next(creditSumMinValue, creditSumMaxValue);
            duration = (short)new Random().Next(durationMinValue, durationMaxValue);
        }
        private static DateTime RandomBirthDay()
        {
            DateTime start = birthDayMinValue;
            Random rand = new Random();
            int range = (DateTime.Today - start).Days;
            return start.AddDays(rand.Next(range)); 
        }
    }
}