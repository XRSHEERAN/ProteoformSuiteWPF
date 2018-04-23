/**
 * Changes made: DatagridView->dataGrid; dgv.datasource -> dg.itemssource;  dgvColumn
 * 
 */
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
namespace ProteoformSuiteWPF
{
    public class ExcelWriter
    {
        #region Private Field

        private XLWorkbook workbook = new XLWorkbook();

        #endregion Private Field


        #region Public Methods

        public void ExportToExcel(List<DataGrid> dgs, string sheet_prefix)
        {
            if (dgs == null)
                return;

            List<DataTable> datatables = new List<DataTable>();
            foreach (DataGrid dg in dgs)
            {
                if (dg.ItemsSource == null || dg.Columns.Count == 0 || dg.Items.Count == 0)
                    continue;

                DataTable dt = new DataTable(dg.Name);
                
                foreach (DataGridColumn col in dg.Columns)
                {
                    if (col.Visibility==Visibility.Collapsed)
                        dt.Columns.Add(col.Header.ToString());
                }

                var dgRows = get_rows(dg);
                /*foreach (DataGridRow row in dgRows)
                {
                    DataRow new_row = dt.NewRow();
                    int column_index = 0;
                    if (row == null)
                        continue;//avoid null cases
                    
                    foreach (DataGridColumn col in dg.Columns)
                    {
                        DataGridCell cell = col.GetCellContent(row);
                        if (dg.Columns[cell.ColumnIndex].Visible)
                            new_row[column_index++] = cell.Value == null || cell.Value.ToString() == "NaN" ? "" : cell.Value;
                    }
                    dt.Rows.Add(new_row);
                }*/
                foreach(DataGridRow dgr in dgRows)
                {
                    DataRow new_row = dt.NewRow();
                    int col = 0;
                    for(int j = 0; j < dg.Columns.Count; j++)
                    {
                        DataGridCell cell = get_cell(dg, dgr, j);
                        if (cell.Column.Visibility == Visibility.Visible)
                        {
                            TextBlock tb = cell.Content as TextBlock;
                            new_row[col++] = (tb == null || tb.Text == "NaN") ? "" : tb.Text;
                        }
                    }
                    dt.Rows.Add(new_row);
                }
            }

            ExportToExcel(datatables, sheet_prefix);
        }

        public void ExportToExcel(List<DataTable> datatables, string sheet_prefix)
        {
            if (datatables == null)
                return;

            foreach (DataTable dt in datatables)
            {
                if (dt.Rows.Count == 0)
                    continue;

                IXLWorksheet worksheet = null;
                lock (workbook)
                {
                    worksheet = workbook.Worksheets.Add(sheet_name(sheet_prefix, dt.TableName));
                }

                // speedup by doing this in parallel
                worksheet.Cell(1, 1).InsertTable(dt);

                foreach (var col in worksheet.Columns())
                {
                    try
                    {
                        col.Cells(2, worksheet.LastRowUsed().RowNumber()).DataType =
                            Double.TryParse(worksheet.Row(2).Cell(col.ColumnNumber()).Value.ToString(), out double is_number) ?
                            XLDataType.Number :
                            XLDataType.Text;
                    }
                    catch
                    {
                        col.Cells(2, worksheet.LastRowUsed().RowNumber()).DataType = XLDataType.Text;
                    }
                }
            }
        }

        public void BuildHyperlinkSheet(List<Tuple<ISweetForm, List<DataTable>>> sheets)
        {
            var ws = workbook.Worksheets.Add("Contents");
            int row = 1;
            foreach (Tuple<ISweetForm, List<DataTable>> x in sheets)
            {
                if (x.Item2 == null) continue;
                foreach (DataTable dt in x.Item2)
                {
                    ws.Cell(row, 1).Value = "Table S" + row.ToString();
                    ws.Cell(row, 2).Value = sheet_name((x.Item1 as Window).Name, dt.TableName);
                    ws.Cell(row++, 2).Hyperlink = new XLHyperlink("'" + sheet_name((x.Item1 as Window).Name, dt.TableName) + "'!A1");
                }
            }
        }

        public string SaveToExcel(string filename)
        {
            if (workbook.Worksheets.Count > 0)
            {
                workbook.SaveAs(filename);
                return "Successfully exported table.";
            }
            else
            {
                return "There were no tables to export.";
            }
        }

        #endregion Public Methods

        #region Private Method

        /**
         * This method get an Ienumerable list for the rows of datagrid,
         * which does not have Rows property any more
         **/
        private IEnumerable<DataGridRow> get_rows(DataGrid inp)
        {
            IEnumerable<DataGridRow> src = inp.ItemsSource as IEnumerable<DataGridRow>;
            if (null == src) yield return null; //Null exception? maybe actual return
            foreach (var item in src)
            {
                var row = inp.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (null != row) yield return row;
            }
        }

        private static string sheet_name(string sheet_prefix, string table_name)
        {
            return sheet_prefix.Substring(0, Math.Min(sheet_prefix.Length, 30 - table_name.Length)) + "_" + table_name;
        }
        /*
         * get cells from a location
         */
        private static DataGridCell get_cell(DataGrid dg, DataGridRow row, int col)
        {
            if (row != null)
            {
                DataGridCellsPresenter cellPresenter = get_child<DataGridCellsPresenter>(row);

                DataGridCell dcl = (DataGridCell)cellPresenter.ItemContainerGenerator.ContainerFromIndex(col);
                if (dcl == null)
                {
                    dg.ScrollIntoView(row, dg.Columns[col]);
                    dcl = (DataGridCell)cellPresenter.ItemContainerGenerator.ContainerFromIndex(col);
                }
                return dcl;
            }
            return null;
        }
        private static T get_child<T>(Visual root) where T:Visual
        {
            T child = default(T);
            int numOfChildren = VisualTreeHelper.GetChildrenCount(root);
            for(int i = 0; i < numOfChildren; i++)
            {
                Visual vChild = (Visual)VisualTreeHelper.GetChild(root, i);
                child = vChild as T;//will not throw exception with as
                if (child == null)//the child is not T typed
                {
                    child = get_child<T>(vChild);
                }
                else
                {
                    break;
                }
            }
            return child;
        }
        #endregion Private Method
    }
}
