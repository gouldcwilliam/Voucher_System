using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Data.OleDb;

namespace Vouchers
{
    class Functions
    {



        /// <summary> FUNCTION FOR EXPORT TO EXCEL
        /// </summary>
        /// <param name="dataTable">data to put into file</param>
        /// <param name="worksheetName">name of worksheet</param>
        /// <param name="saveAsLocation">file path to save</param>
        /// <param name="ReportType">does nothing</param>
        /// <returns></returns>
        public static bool Excel_WriteDataTableToFile(System.Data.DataTable dataTable, string worksheetName, string saveAsLocation, string ReportType)
        {
            Microsoft.Office.Interop.Excel.Application excel;
            Microsoft.Office.Interop.Excel.Workbook excelworkBook;
            Microsoft.Office.Interop.Excel.Worksheet excelSheet;
            Microsoft.Office.Interop.Excel.Range excelCellrange;

            try
            {
                // Start Excel and get Application object.
                excel = new Microsoft.Office.Interop.Excel.Application();

                // for making Excel visible
                excel.Visible = false;
                excel.DisplayAlerts = false;

                // Creation a new Workbook
                excelworkBook = excel.Workbooks.Add(Type.Missing);

                // Workk sheet
                excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelworkBook.ActiveSheet;
                excelSheet.Name = worksheetName;


                excelSheet.Cells[1, 1] = "Export";
                excelSheet.Cells[1, 2] = "Date : " + DateTime.Now.ToShortDateString();

                // loop through each row and add values to our sheet
                int rowcount = 2;

                foreach (System.Data.DataRow datarow in dataTable.Rows)
                {
                    rowcount += 1;
                    for (int i = 1; i <= dataTable.Columns.Count; i++)
                    {
                        // on the first iteration we add the column headers
                        if (rowcount == 3)
                        {
                            excelSheet.Cells[2, i] = dataTable.Columns[i - 1].ColumnName;
                            excelSheet.Cells.Font.Color = System.Drawing.Color.Black;

                        }

                        excelSheet.Cells[rowcount, i] = datarow[i - 1].ToString();

                        //for alternate rows
                        if (rowcount > 3)
                        {
                            if (i == dataTable.Columns.Count)
                            {
                                if (rowcount % 2 == 0)
                                {
                                    excelCellrange = excelSheet.Range[excelSheet.Cells[rowcount, 1], excelSheet.Cells[rowcount, dataTable.Columns.Count]];
                                    Excel_FormatCells(excelCellrange, "#CCCCFF", System.Drawing.Color.Black, false);
                                }

                            }
                        }

                    }

                }

                // now we resize the columns
                excelCellrange = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[rowcount, dataTable.Columns.Count]];
                excelCellrange.EntireColumn.AutoFit();
                Microsoft.Office.Interop.Excel.Borders border = excelCellrange.Borders;
                border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                border.Weight = 2d;


                excelCellrange = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[2, dataTable.Columns.Count]];
                Excel_FormatCells(excelCellrange, "#000099", System.Drawing.Color.White, true);


                //now save the workbook and exit Excel


                excelworkBook.SaveAs(saveAsLocation); ;
                excelworkBook.Close();
                excel.Quit();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                excelSheet = null;
                excelCellrange = null;
                excelworkBook = null;
            }

        }




        /// <summary> FUNCTION FOR FORMATTING EXCEL CELLS
        /// </summary>
        /// <param name="range"></param>
        /// <param name="HTMLcolorCode"></param>
        /// <param name="fontColor"></param>
        /// <param name="IsFontbool"></param>
        public static void Excel_FormatCells(Microsoft.Office.Interop.Excel.Range range, string HTMLcolorCode, System.Drawing.Color fontColor, bool IsFontbool)
        {
            range.Interior.Color = System.Drawing.ColorTranslator.FromHtml(HTMLcolorCode);
            range.Font.Color = System.Drawing.ColorTranslator.ToOle(fontColor);
            if (IsFontbool == true)
            {
                range.Font.Bold = IsFontbool;
            }
        }




/*

        /// <summary> QUERIES THE EXCEL FILE
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="xQuery"></param>
        /// <returns></returns>
        public static DataTable Excel_QuerySheet(string fileName, string xQuery)
        {
            DataTable t = new DataTable();
            OleDbConnection xconn = null;
            OleDbDataAdapter xadapter = null;
            try
            {
                if (fileName.Trim().EndsWith(".xlsx"))
                {
                    xconn = new OleDbConnection(string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", fileName));
                }
                else if (fileName.Trim().EndsWith(".xls"))
                {
                    xconn = new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";", fileName));
                }
                else { return new DataTable(); }
                xadapter = new OleDbDataAdapter(xQuery, xconn);
                xadapter.Fill(t);
                return t;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DataTable();
            }
            finally
            {
                if (t.Rows.Count == 0) { t.Dispose(); }
                if (xconn != null) { xconn.Close(); xconn.Dispose(); }
                if (xadapter != null) { xadapter.Dispose(); }
            }
        }




        /// <summary> IMPORTS THE TABLES NAMES OF THE GIVEN FILE
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static DataTable Excel_GetTables(string fileName)
        {
            DataTable t = new DataTable();
            OleDbConnection xconn = null;
            try
            {
                if (fileName.Trim().EndsWith(".xlsx"))
                {
                    xconn = new OleDbConnection(string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", fileName));
                }
                else if (fileName.Trim().EndsWith(".xls"))
                {
                    xconn = new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";", fileName));
                }
                xconn.Open();
                t = xconn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                return t;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DataTable();
            }
            finally
            {
                if (xconn != null) { xconn.Close(); xconn.Dispose(); }
                if (t.Rows.Count == 0) { t.Dispose(); }
            }
        }




        /// <summary> IMPORTS THE COLUMNS OF THE GIVEN FILE
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public static DataTable Excel_GetColumns(string fileName, string tablename)
        {
            DataTable t = new DataTable();
            OleDbConnection xconn = null;
            try
            {
                if (fileName.Trim().EndsWith(".xlsx"))
                {
                    xconn = new OleDbConnection(string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", fileName));
                }
                else if (fileName.Trim().EndsWith(".xls"))
                {
                    xconn = new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";", fileName));
                }
                xconn.Open();
                t = xconn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, tablename, null });
                return t;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DataTable();
            }
            finally
            {
                if (xconn != null) { xconn.Close(); xconn.Dispose(); }
                if (t.Rows.Count == 0) { t.Dispose(); }
            }
        }
        */



    }
}
