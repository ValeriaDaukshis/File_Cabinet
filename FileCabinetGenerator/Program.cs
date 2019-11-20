using System;
using System.Globalization;
using System.IO;
using CommandLine;
using FileCabinetGenerator.FileImporters;
using FileCabinetGenerator.Records;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Enter point.
    /// </summary>
    static class Program
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private static readonly DateTime BirthDayMinValue = new DateTime(1950, 01, 01);
        private static readonly FileCabinetRecords FileCabinetRecords = new FileCabinetRecords();
        private static IFileCabinetSerializer serializer;
        private static string outputFormat = "xml";
        private static string outputFileName = "records.xml";
        private static int recordsAmount = 10;
        private static int startId = 1;
        private static string firstNameTemplate = "FirstName";
        private static string lastNameTemplate = "LastName";
        private static int durationMinValue = 12;
        private static int durationMaxValue = 300;
        private static int creditSumMinValue = 120;
        private static int creditSumMaxValue = 50000;

        /// <summary>
        /// Main.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            bool inputs = true;
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(o =>
                {
                    if (o.OutputFormat is null)
                    {
                        return;
                    }

                    if (o.OutputFormat.ToLower(Culture) != "xml" && o.OutputFormat.ToLower(Culture) != "csv")
                    {
                        Console.WriteLine($"Invalid output format type {o.OutputFormat}");
                        inputs = false;
                        return;
                    }

                    if (string.IsNullOrEmpty(o.OutputFileName))
                    {
                        Console.WriteLine($"Invalid output file name {o.OutputFileName}");
                        inputs = false;
                        return;
                    }

                    if (o.RecordsAmount <= 0)
                    {
                        Console.WriteLine($"Invalid records amount {o.RecordsAmount}");
                        inputs = false;
                        return;
                    }

                    if (o.StartId <= 0)
                    {
                        Console.WriteLine($"Invalid id {o.StartId}");
                        inputs = false;
                        return;
                    }
                    outputFormat = o.OutputFormat;
                    outputFileName = o.OutputFileName;
                    recordsAmount = o.RecordsAmount;
                    startId = o.StartId;
                });

            if (inputs)
            {
                GenerateFileCabinetRecordsFields();
                ExportToFile();
                Console.WriteLine($"{recordsAmount} records were written to {outputFileName}");
            }
        }
        
        private static void ExportToFile()
        {
            try
            {
                using (StreamWriter fs = new StreamWriter(outputFileName))
                {
                    if (outputFormat == "csv")
                    {
                        serializer = new ImportToCsv(fs);
                    }
                    else if (outputFormat == "xml")
                    {
                        serializer = new ImportToXml(fs);
                    }
                    serializer.Serialize(FileCabinetRecords);
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
            int userId = startId;
            for (int i = 0; i < recordsAmount; i++)
            {
                GenerateFields(i, out string firstName, out string lastName, out char gender, out DateTime dateOfBirth, out decimal credit, out short duration);
                Record record = new Record(userId, firstName, lastName, gender, dateOfBirth, credit, duration);
                FileCabinetRecords.Record.Add(record);
                userId++;
            }
        }
        
        private static void GenerateFields(int i,out string firstName, out string lastName, out char gender, out DateTime dateOfBirth, out decimal credit, out short duration)
        {
            firstName = Program.firstNameTemplate + i;
            lastName = Program.lastNameTemplate + i;
            gender = i % 5 == 0 ? 'F' : 'M';
            dateOfBirth = RandomBirthDay();
            credit = new Random().Next(creditSumMinValue, creditSumMaxValue);
            duration = (short)new Random().Next(durationMinValue, durationMaxValue);
        }
        private static DateTime RandomBirthDay()
        {
            DateTime start = BirthDayMinValue;
            Random rand = new Random();
            int range = (DateTime.Today - start).Days;
            return start.AddDays(rand.Next(range)); 
        }
    }
}