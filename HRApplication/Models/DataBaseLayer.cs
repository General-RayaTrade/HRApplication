using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace HRApplication.Models
{
    public class DataBaseLayer
    {

        public static SqlConnection GetSqlConnection()
        {
            string szConnectionString = "server=192.168.0.5,34300;database=ARSystem;User ID=sa;Password=DataAdmin2010;TIMEOUT=99999999";
            SqlConnection _SqlConnection = new SqlConnection(szConnectionString);
            return _SqlConnection;
        }
        public static DataTable ExecuteSelectSQLQuery(string szQuery)
        {
            SqlCommand _SqlCommand = new SqlCommand();
            SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter();
            DataTable _DataTable = new DataTable();

            _SqlCommand.Connection = GetSqlConnection();
            _SqlCommand.CommandType = CommandType.Text;
            _SqlCommand.CommandText = szQuery;
            _SqlCommand.CommandTimeout = 0;
            _SqlDataAdapter = new SqlDataAdapter(_SqlCommand);
            _DataTable.Clear();
            try
            {
                _SqlDataAdapter.Fill(_DataTable);
            }
            catch (Exception ErrorMessage)
            {
                StoreErrors(ErrorMessage.Message, "ExecuteSelectSQLQuery With Query: " + szQuery);
            }
            return _DataTable;
        }
        public static int ExecuteSQLNonQuery(string szQuery)
        {
            int nRowsAffected = 0;
            try
            {
                SqlConnection _sqlConnection = GetSqlConnection();
                SqlCommand sqlCommand = new SqlCommand(szQuery, _sqlConnection);
                _sqlConnection.Open();
                nRowsAffected = sqlCommand.ExecuteNonQuery();
                _sqlConnection.Close();
            }
            catch (Exception ErrorMessage)
            {
                StoreErrors(ErrorMessage.Message, "ExecuteSQLNonQuery With Query: " + szQuery);
            }
            return nRowsAffected;
        }


        public static DataTable ExecuteSelectQueryFromExcelSheet(string szQuery, string szPath)
        {
            OleDbCommand _OleDbCommand = new OleDbCommand();
            OleDbDataAdapter _OleDbDataAdapter = new OleDbDataAdapter();
            DataTable _DataTable = new DataTable();

            try
            {

                _OleDbCommand.Connection = GetExcelSheetConnection(szPath);
                _OleDbCommand.CommandType = CommandType.Text;
                _OleDbCommand.CommandText = szQuery;
                _OleDbDataAdapter = new OleDbDataAdapter(_OleDbCommand);
                _DataTable.Clear();
                _OleDbDataAdapter.Fill(_DataTable);
            }
            catch (Exception ErrorMessage)
            {
                StoreErrors(ErrorMessage.Message, "ExecuteScalarSQLQuery With Query: " + szQuery);
            }
            return _DataTable;
        }
        public static OleDbConnection GetExcelSheetConnection(string szPath)
        {
            OleDbConnection oledbConn = new OleDbConnection();
            if (Path.GetExtension(szPath) == ".xls")
                oledbConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + szPath + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"");

            else
                if (Path.GetExtension(szPath) == ".xlsx")
                oledbConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + szPath + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';");
            return oledbConn;
        }
        public static void StoreErrors(string errorplace, string errormessage)
        {
            string strSaveLogHTML = @"D:\Remedy Jobs\RemedyJobs\RemedyJobs\bin\Debug\Logs\" + DateTime.Now.Day.ToString("00") + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Year.ToString("0000");
            string FullPath = strSaveLogHTML + "\\Aman_MerchantJob_Log.html";

            try
            {
                if (!Directory.Exists(strSaveLogHTML))
                    Directory.CreateDirectory(strSaveLogHTML);

                using (StreamWriter sw = File.AppendText(FullPath))
                {
                    sw.WriteLine("<tr><td></td><td></td><td></td></tr>");
                    string DateTimeNow = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
                    sw.WriteLine("<tr><td>" + DateTimeNow + "</td><td>" + errormessage + "</td><td>" + errorplace + "</td></tr>");
                    sw.Flush();
                }
            }
            catch
            {
            }
            finally
            {
            }
        }
        public static string GetNextID(string szQuery)
        {

            int nNext_ID = 1;
            SqlCommand _SqlCommand = new SqlCommand();
            SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter();
            DataTable _DataTable = new DataTable();
            SqlConnection _sqlConnection = GetSqlConnection();
            _SqlCommand.Connection = _sqlConnection;
            _SqlCommand.CommandType = CommandType.Text;
            _SqlCommand.CommandText = szQuery;
            try
            {
                _sqlConnection.Open();
                object Max_ID = _SqlCommand.ExecuteScalar();
                try
                {
                    if (!string.IsNullOrEmpty(Max_ID.ToString()))
                    {
                        if (int.TryParse(Max_ID.ToString(), out nNext_ID) == false)
                            nNext_ID = 1;
                        else
                            nNext_ID = nNext_ID + 1;
                    }
                    else
                        nNext_ID = 1;
                }
                catch
                {
                    nNext_ID = 1;
                }
                if (_sqlConnection.State != ConnectionState.Closed)
                    _sqlConnection.Close();
            }
            catch (Exception ErrorMessage)
            {
                StoreErrors(ErrorMessage.Message, "GetNextID With Query: " + szQuery);
            }
            return nNext_ID.ToString();
        }
        internal static string GetRemedyNextID(string TableName)
        {
            int nNextID = 0;

            object obj_NextID = ExecuteScalarSQLQuery("select nextId from arschema where name ='" + TableName + "'");
            if (int.TryParse(Convert.ToString(obj_NextID), out nNextID))
                ExecuteSQLNonQuery("UPDATE arschema SET nextId = " + (nNextID + 1) + " WHERE name ='" + TableName + "'");

            return nNextID.ToString().PadLeft(15, '0');
        }
        public static object ExecuteScalarSQLQuery(string szQuery)
        {
            object obj = null;
            try
            {
                SqlConnection _sqlConnection = GetSqlConnection();
                SqlCommand sqlCommand = new SqlCommand(szQuery, _sqlConnection);
                _sqlConnection.Open();
                obj = sqlCommand.ExecuteScalar();
                _sqlConnection.Close();
            }
            catch (Exception ErrorMessage)
            {
                StoreErrors(ErrorMessage.Message, "ExecuteScalarSQLQuery With Query: " + szQuery);
            }
            return obj;
        }
    }
}
