using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLHelper;
using System.Data.Sql;
using System.Data;

namespace SQLHelper.Test
{
    public class Program
    {
        private const string connectionString = "Server=DESKTOP-INPDFMT\\SSMSTESTSERVER;Database=TestBase;User Id=sa;Password=test123!;";

        private static SQLHandler handler;

        private static void Main(string[] args)
        {
            handler = new SQLHandler(true);
            handler.ConnectionString = connectionString;

            string sqlInsert = $"INSERT INTO Person (P_Name, P_BD) VALUES ('John', '{DateTime.Now.ToShortDateString()}')";
            string sqlInsert2 = "INSERT INTO Person (P_Name, P_BD) VALUES ('John', '1990-01-01')";
            handler.ExecuteNonQuerySql(connectionString,sqlInsert);
            Console.WriteLine($"Executed sqlInsert1: {sqlInsert}");
            handler.ExecuteNonQuerySql(connectionString,sqlInsert2);
            Console.WriteLine($"Executed sqlInsert2: {sqlInsert2}");
            string sqlSelect = $"SELECT COUNT(*) FROM Person;";
            int count = handler.GetSingleResult<int>(connectionString,sqlSelect);
            Console.WriteLine($"Executed GetSingleEntry: {count}");
            string sqlSelectAll = $"Select * from Person;";
            DataTable results = handler.SqlGetData(connectionString, sqlSelectAll);

            for (int i = 0; i < results.Columns.Count; i++)
            {
                Console.Write(results.Columns[i].ColumnName+"  ");
            }

            foreach (DataRow row in results.Rows)
            {
                foreach (DataColumn col in results.Columns)
                {
                    Console.WriteLine($"{row[col]}");
                }
            }

            Console.ReadKey();
        }
    }
}
