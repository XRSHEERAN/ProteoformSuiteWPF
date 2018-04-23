using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
namespace ProteoformSuiteWPF
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ProteoformSweet : UserControl
    {
        #region Private Fields
        ISweetForm current_form;
        SaveFileDialog saveExcelDialog;
        #endregion

        public ProteoformSweet()
        {
            InitializeComponent();
        }

        /**
         * !!!!!!!!!!!!!!Problem: MDI Application look may not be supported; TBA what the application looks
         **/
        private void ExportTables_Click(object sender, RoutedEventArgs e)
        {
            List<DataTable> data_tables = current_form.SetTables();

            if (data_tables == null)
            {
                MessageBox.Show("There is no table on this page to export. Please navigate to another page with the Results tab.");
                return;
            }
            ExcelWriter writer = new ExcelWriter();
            writer.ExportToExcel(data_tables, (current_form as Window).Name);
            //SaveExcelFile(writer, (current_form as Window).mdi)
        }

        private void SaveExcelFile(ExcelWriter writer, string filename)
        {
            saveExcelDialog.FileName = filename;
            
            if (saveExcelDialog.ShowDialog()==true)
            {
                MessageBox.Show(writer.SaveToExcel(saveExcelDialog.FileName));
            }
            else return;
        }
    }
}
