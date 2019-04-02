using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Vouchers
{
    class SQL
    {


        public static string connString = "Data Source=ricvchdbp01\\sqlexpress;" +
                                  "Initial Catalog=Voucher System SQL LocSQL;" +
                                  "Integrated Security=SSPI;";
        public static SqlConnection conn = new SqlConnection(connString);


        public static bool TestConnection()
        {
            try
            {
                conn.Open();
            }
            catch (Exception) { return false; }
            finally
            {
                conn.Close();
            }
            return true;
        }


        public static DataTable SelectDPIReport(string startDate, string endDate)
        {
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            string queryString =
                                @"SELECT [NATIONAL ACCOUNTS1].[GROUP]
	                                , [NATIONAL ACCOUNTS1].RAILROAD AS [National Account]
	                                , VOUCHERS.[INVOICE #]
	                                , VOUCHERS.[VOUCHER #]
	                                , VOUCHERS.[SHIPTO NAME]
	                                , VOUCHERS.SHIPTO
	                                , VOUCHERS.[CUST INVC]
	                                , CAST(VOUCHERS.[PURCHASE DATE] as date) as [Purchase Date]
	                                --, CAST(VOUCHERS.[INVC DATE] as varchar)
	                                , SUBSTRING(CAST(VOUCHERS.[INVC DATE] as varchar),5,2) + '-' + SUBSTRING(CAST(VOUCHERS.[INVC DATE] as varchar),7,2) + '-' + SUBSTRING(CAST(VOUCHERS.[INVC DATE] as varchar),1,4) AS [INVOICE DATE]
	                                , [VOUCHERS DETAIL].[Quantity]
	                                , [VOUCHERS DETAIL].[Total]
	                                , [VOUCHERS DETAIL].[Subsidized Amount]
	                                , [VOUCHERS DETAIL].[Prepayment Amount]
	                                , [VOUCHERS DETAIL].[Payroll Deduction]
	                                --, IIf([MISC6] Is Null,'blank',CAST([MISC6] AS DATE)) AS [MISC 6]
	                                , IIf([MISC6] Is Null or upper([MISC6]) like '%BLANK%','blank',[MISC6]) AS [MISC 6]
                                FROM ([VOUCHERS DETAIL] 
	                                INNER JOIN VOUCHERS 
		                                ON ([VOUCHERS DETAIL].[Invoice #] = VOUCHERS.[INVOICE #]) 
		                                AND ([VOUCHERS DETAIL].[Voucher#] = VOUCHERS.[VOUCHER #]) 
		                                AND ([VOUCHERS DETAIL].[Brand ID] = VOUCHERS.[BRAND ID]) 
		                                AND ([VOUCHERS DETAIL].[System ID] = VOUCHERS.[SYSTEM ID])) 
	                                INNER JOIN [NATIONAL ACCOUNTS1] 
	                                ON VOUCHERS.[SYSTEM ID] = [NATIONAL ACCOUNTS1].[SYSTEM ID]
                                WHERE VOUCHERS.[INVC DATE] Between @startDate And @endDate;";
            SqlDataAdapter da = new SqlDataAdapter(queryString, conn);
            da.SelectCommand.Parameters.AddWithValue("@startDate", startDate);
            da.SelectCommand.Parameters.AddWithValue("@endDate", endDate);
            try
            {
                conn.Open();
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            catch (Exception) {; }
            finally { conn.Close(); }
            return dt;
        }

        /// <summary>
        /// List all tables
        /// </summary>
        /// <returns></returns>
        static public DataTable ListTables()
        {
            return Select("SELECT Distinct TABLE_NAME FROM information_schema.TABLES");
        }



        /// <summary>
        /// executes select statement
        /// </summary>
        /// <param name="selectSQL">statement to execute</param>
        /// <param name="paramList">parameters to add to sql string</param>
        /// <returns>DataTable containing results</returns>
        static public DataTable Select(string selectSQL, List<SqlParameter> paramList)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(selectSQL, conn);

            if (paramList.Count > 0)
            {
                adapter.SelectCommand.Parameters.AddRange(paramList.ToArray());
            }
            try
            {
                adapter.Fill(ds);
                return ds.Tables[0];
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Errors);
                return new DataTable();
            }
            finally { conn.Close(); }
        }

        /// <summary>
        /// <seealso cref="Select(string, List{SqlParameter})"/>
        /// </summary>
        /// <param name="selectSQL"></param>
        /// <returns></returns>
		static public DataTable Select(string selectSQL)
        {
            return Select(selectSQL, new List<SqlParameter>());
        }

        /// <summary>
        /// <seealso cref="Select(string, List{SqlParameter})"/>
        /// </summary>
        /// <param name="selectSQL"></param>
        /// <param name="sqlParam"></param>
        /// <returns></returns>
		static public DataTable Select(string selectSQL, SqlParameter sqlParam)
        {
            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(sqlParam);
            return Select(selectSQL, paramList);
        }
    }
}
