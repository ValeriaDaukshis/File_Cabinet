using System;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using CommandLine;
using FileCabinetApp;
using FileCabinetApp.Records;
using FileCabinetGenerator.FileImporters;

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
        private static string outputFormat;
        private static string outputFileName;
        private static int recordsAmount;
        private static int startId;
        private static string firstName = "FirstName";
        private static string lastName = "LastName";
        private static int durationMinValue = 6;
        private static int durationMaxValue = 120;
        private static int creditSumMinValue = 10;
        private static int creditSumMaxValue = 5000;

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
                    outputFormat = o.OutputFormat;
                    outputFileName = o.OutputFileName;
                    recordsAmount = o.RecordsAmount;
                    startId = o.StartId;
                });
            GenerateFileCabinetRecordsFields();
            ExportToFile();
            Console.WriteLine($"{recordsAmount} records were written to {outputFileName}");
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
            firstName = Program.firstName + i;
            lastName = Program.lastName + i;
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