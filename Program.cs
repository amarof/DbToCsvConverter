using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace DbToCsvConverter
{
    class Program
    {
        static string connectionString = "Server=(localdb)\\mssqllocaldb;Database=simple-db;Trusted_Connection=True;MultipleActiveResultSets=true";
        static string query = "select id as Id, name as Name, isactive as IsActive, birthdate as BirthDate from person";
        static string csvFileName = @"person.csv";
        static void Main(string[] args)
        {
            // get records (dynamically) based on the query from database
            ICollection<Record> records = GetRecords();
            // convert records to csv file
            ConvertRecordsToCsv(records);
        }
        private static  ICollection<Record> GetRecords()
        {
            List<Record> records = new List<Record>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    int columnCount = reader.FieldCount;
                    while (reader.Read())
                    {
                        Record record = new Record();
                        for (int i = 0; i < columnCount; i++)
                        {
                            RecordColumn recordColumn = new RecordColumn();
                            recordColumn.ColumnName = reader.GetName(i);
                            recordColumn.Value = reader[i].ToString(); 
                            record.RecordColumns.Add(recordColumn);
                        }
                        records.Add(record);
                    }
                }
            }
            return records;
        }
        private static void ConvertRecordsToCsv (ICollection<Record> records)
        {
            StringBuilder sb = new StringBuilder();
            string header = string.Join(",", records.First().RecordColumns.Select(rc => rc.ColumnName).ToList()); // set csv header
            sb.AppendLine(header);
            // set csv lines
            foreach (Record r in records)
            {
                string line = string.Join(",", r.RecordColumns.Select(rc => rc.Value).ToList());
                sb.AppendLine(line);
            }
            File.WriteAllText(csvFileName, sb.ToString());
        }

    }
    /// <summary>
    /// this class represent a column of a record (row)
    /// </summary>
    class RecordColumn
    {
        public string ColumnName { get; set; }
        public string Value { get; set; }
    }
    /// <summary>
    /// this class represent a row 
    /// </summary>
    class Record
    {
        public List<RecordColumn> RecordColumns { get; set; } = new List<RecordColumn>();
    }
}
