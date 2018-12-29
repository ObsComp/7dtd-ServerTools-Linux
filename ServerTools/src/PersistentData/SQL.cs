using System.Data;

namespace ServerTools
{
    public class SQL
    {
        public static int Sql_version = 6;

        public static void Connect()
        {
            MySqlDatabase.SetConnection();
        }

        public static void FastQuery(string _sql)
        {
            MySqlDatabase.FastQuery(_sql);
        }

        public static DataTable TQuery(string _sql)
        {
            DataTable dt = MySqlDatabase.TQuery(_sql);
            return dt;
        }

        public static string EscapeString(string _string)
        {
            string _str = MySqlDatabase.EscapeString(_string);
            return _str;
        }
    }
}
