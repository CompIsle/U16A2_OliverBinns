using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace U16A2Library
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = "U16A2Task2Data.csv";
            string newFilePath = "BooksCode.csv";

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csv.Context.RegisterClassMap<BookMap>();

                var records = csv.GetRecords<Book>().ToList();

                int validRecordsCount = 0;

                ICodeAssigner codeAssigner = new MD5CodeAssigner();

                using (var writer = new StreamWriter(newFilePath))
                using (var csvw = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {

                    foreach (var record in records)
                    {
                        record.Code = codeAssigner.GenerateHashCode(record);

                        csvw.WriteField(record.Code);
                        csvw.WriteField(record.Name);
                        csvw.WriteField(record.Title);
                        csvw.WriteField(record.Place);
                        csvw.WriteField(record.Publisher);
                        csvw.WriteField(record.Date);
                        
                        csvw.NextRecord();
                        validRecordsCount++;
                    }
                }

                Console.WriteLine($"{records.Count} lines of input read, {validRecordsCount} valid records created.");
            }

            Console.ReadLine();
        }
        public interface ICodeAssigner
        {
            string GenerateHashCode(Book book);
        }
        public class MD5CodeAssigner : ICodeAssigner
        {
            public string GenerateHashCode(Book book)
            {
                string bookString = $"{book.Name}-{book.Title}-{book.Place}-{book.Publisher}-{book.Date}";

                using (MD5 md5 = MD5.Create())
                {
                    byte[] inputBytes = Encoding.ASCII.GetBytes(bookString);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("X2"));
                    }

                    string truncatedHash = "AX" + sb.ToString().Substring(sb.Length - 6);

                    return truncatedHash;
                }
            }
        }

        public class Book
        {
            public string Name { get; set; }
            public string Title { get; set; }
            public string Place { get; set; }
            public string Publisher { get; set; }
            public string Date { get; set; }
            public string Code { get; set; }
        }

        public sealed class BookMap : ClassMap<Book>
        {
            public BookMap()
            {
                Map(m => m.Name).Index(0);
                Map(m => m.Title).Index(1);
                Map(m => m.Place).Index(2);
                Map(m => m.Publisher).Index(3);
                Map(m => m.Date).Index(4);
            }
        }
    }
}

