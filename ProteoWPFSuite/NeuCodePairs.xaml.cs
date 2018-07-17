using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Controls;
using System.Drawing;
using System.Windows;
namespace ProteoWPFSuite
{
    /// <summary>
    /// Interaction logic for NeuCodePairs.xaml
    /// </summary>
    public partial class NeuCodePairs : UserControl, ISweetForm
    {
        public NeuCodePairs()
        {
            InitializeComponent();

            //chart1 init
            this.ct_LysineCount.Cursor = System.Windows.Forms.Cursors.No;

        }

        public List<DataTable> DataTables => throw new NotImplementedException();

        public void ClearListsTablesFigures(bool clear_following_forms)
        {
            throw new NotImplementedException();
        }

        public void FillTablesAndCharts()
        {
            throw new NotImplementedException();
        }

        public void InitializeParameterSet()
        {
            throw new NotImplementedException();
        }

        public bool ReadyToRunTheGamut()
        {
            throw new NotImplementedException();
        }

        public void RunTheGamut(bool full_run)
        {
            throw new NotImplementedException();
        }

        public List<DataTable> SetTables()
        {
            throw new NotImplementedException();
        }

        System.Drawing.Point? ct_intensityRatio_prevPosition = null;
        System.Windows.Forms.ToolTip ct_intensityRatio_tt = new System.Windows.Forms.ToolTip();
        void ct_IntensityRatio_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                DisplayUtility.tooltip_graph_display(ct_intensityRatio_tt, e, this.ct_IntensityRatio, ct_intensityRatio_prevPosition);
            }
        }

        System.Drawing.Point? ct_LysineCount_prevPosition = null;
        System.Windows.Forms.ToolTip ct_LysineCount_tt = new System.Windows.Forms.ToolTip();
        private void ct_LysineCount_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                
                DisplayUtility.tooltip_graph_display(ct_LysineCount_tt, e, ct_LysineCount, ct_LysineCount_prevPosition);
            }
        }
    }
}
