using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvHelperTutorial
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ReadBase();
            ReadHeaderLower();
            ReadHeaderLess();
            ReadMapping();
            WriteBase();
            WriteByHand();
        }

        static void ReadBase()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
            };
            using (var reader = new StreamReader("file.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<Foo>();
                foreach (var record in records)
                {
                    Console.WriteLine(String.Format("[ReadBase] {0} {1}", record.Id,record.Name));
                }
            }
            
        }
        static void ReadHeaderLower()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower(),
            };
            using (var reader = new StreamReader("file.lower.csv"))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<Foo>();
                foreach (var record in records)
                {
                    Console.WriteLine(String.Format("[ReadHeaderLower] {0} {1}", record.Id, record.Name));
                }
            }
        }
        static void ReadHeaderLess()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using (var reader = new StreamReader("file.headerless.csv"))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<Foo>();
                foreach (var record in records)
                {
                    Console.WriteLine(String.Format("[ReadHeaderLess] {0} {1}", record.Id, record.Name));
                }
            }
        }
        static void ReadMapping()
        {
            using (var reader = new StreamReader("file.lower.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<FooMap>();
                var records = csv.GetRecords<Foo>();
                foreach (var record in records)
                {
                    Console.WriteLine(String.Format("[ReadMapping] {0} {1}", record.Id, record.Name));
                }
            }
        }
        static void WriteBase()
        {
            var records = new List<Foo>
            {
                new Foo { Id = 1, Name = "one" },
                new Foo { Id = 2, Name = "two" },
            };
            using (var writer = new StreamWriter("file.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
            }
        }
        static void WriteByHand()
        {
            var records = new List<Foo>
            {
                new Foo { Id = 1, Name = "one" },
                new Foo { Id = 2, Name = "two" },
            };
            using (var writer = new StreamWriter("filehand.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<Foo>();
                csv.NextRecord();
                foreach (var record in records)
                {
                    csv.WriteRecord(record);
                    csv.NextRecord();
                }
            }
        }
    }
}
