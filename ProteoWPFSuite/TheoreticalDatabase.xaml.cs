using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using ProteoformSuiteInternal;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Windows.Input;

namespace ProteoWPFSuite
{
    /// <summary>
    /// Interaction logic for TheoreticalDatabase.xaml
    /// </summary>
    public partial class TheoreticalDatabase : UserControl,ISweetForm,ITabbedMDI
    {
        #region Public Constructor
        public TheoreticalDatabase()
        {
            InitializeComponent();
        }
        #endregion Public Constructor
        #region Private Fields

        OpenFileDialog openAccessionListDialog = new OpenFileDialog();
        bool initial_load = true;

        #endregion Private Fields
        #region Public Property

        public List<DataTable> DataTables { get; private set; }

        #endregion Public Property
        #region Private Methods

        private void TheoreticalDatabase_Load(object sender, EventArgs e)
        {
            InitializeParameterSet();
            initial_load = false;
        }

        private bool SetMakeDatabaseButton()
        {
            bool ready_to_run = ReadyToRunTheGamut();
            btn_downloadUniProtPtmList.IsEnabled = !ready_to_run && Sweet.lollipop.get_files(Sweet.lollipop.input_files, Purpose.PtmList).Count() == 0;
            btn_Make_Databases.IsEnabled = ready_to_run;
            return ready_to_run;
        }
        //new trick in wpf
        private void btn_Make_Databases_Click(object sender, EventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            RunTheGamut(false);
            Mouse.OverrideCursor = null;
        }

        private void cmbx_DisplayWhichDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!initial_load)
            {
                string table = cmbx_DisplayWhichDB.SelectedItem.ToString();
                if (table == "Target")
                    DisplayUtility.FillDataGridView(dgv_Database, Sweet.lollipop.target_proteoform_community.theoretical_proteoforms.Select(t => new DisplayTheoreticalProteoform(t)));
                else
                    DisplayUtility.FillDataGridView(dgv_Database, Sweet.lollipop.decoy_proteoform_communities[table].theoretical_proteoforms.Select(t => new DisplayTheoreticalProteoform(t)));
            }
            DisplayTheoreticalProteoform.FormatTheoreticalProteoformTable(dgv_Database);
        }

        private void cmb_empty_TextChanged(object sender, EventArgs e) { }

        #endregion Private Methods

        #region Public Methods

        public void load_dgv()
        {
            DisplayUtility.FillDataGridView(dgv_Database, Sweet.lollipop.target_proteoform_community.theoretical_proteoforms.Select(t => new DisplayTheoreticalProteoform(t)));
            initialize_table_bindinglist();
            DisplayTheoreticalProteoform.FormatTheoreticalProteoformTable(dgv_Database);
        }

        public List<DataTable> SetTables()
        {
            DataTables = new List<DataTable>
            {
                DisplayTheoreticalProteoform.FormatTheoreticalProteoformTable(Sweet.lollipop.target_proteoform_community.theoretical_proteoforms.Select(t => new DisplayTheoreticalProteoform(t)).ToList(), "TargetDatabase"),
                DisplayUnlocalizedModification.FormatUnlocalizedModificationTable(Sweet.lollipop.theoretical_database.unlocalized_lookup.Values.Select(m => new DisplayUnlocalizedModification(m)).ToList(), "UnlocalizedModifications")
            };
            foreach (KeyValuePair<string, ProteoformCommunity> decoy_community in Sweet.lollipop.decoy_proteoform_communities)
            {
                DataTables.Add(DisplayTheoreticalProteoform.FormatTheoreticalProteoformTable(decoy_community.Value.theoretical_proteoforms.Select(t => new DisplayTheoreticalProteoform(t)).ToList(), decoy_community.Key));
            }
            return DataTables;
        }

        public void FillDataBaseTable(string table)
        {
            if (table == "Target")
                DisplayUtility.FillDataGridView(dgv_Database, Sweet.lollipop.target_proteoform_community.theoretical_proteoforms.Select(t => new DisplayTheoreticalProteoform(t)));
            else if (Sweet.lollipop.decoy_proteoform_communities.ContainsKey(table))
                DisplayUtility.FillDataGridView(dgv_Database, Sweet.lollipop.decoy_proteoform_communities[table].theoretical_proteoforms.Select(t => new DisplayTheoreticalProteoform(t)));
            DisplayTheoreticalProteoform.FormatTheoreticalProteoformTable(dgv_Database);
        }

        public void initialize_table_bindinglist()
        {
            List<string> databases = new List<string> { "Target" };
            if (Sweet.lollipop.decoy_proteoform_communities.Keys.Count > 0)
            {
                foreach (string name in Sweet.lollipop.decoy_proteoform_communities.Keys)
                {
                    databases.Add(name);
                }
            }
            cmbx_DisplayWhichDB.ItemsSource = new BindingList<string>(databases);
            cb_useRandomSeed.IsChecked = Sweet.lollipop.useRandomSeed_decoys;
            nud_randomSeed.Value = Sweet.lollipop.randomSeed_decoys;
        }

        public void InitializeParameterSet()
        {
            btn_NeuCode_Lt.IsChecked = Sweet.lollipop.neucode_labeled;
            btn_NaturalIsotopes.IsChecked = !Sweet.lollipop.neucode_labeled;

            nUD_MaxPTMs.Minimum = 0;
            nUD_MaxPTMs.Maximum = 5;
            nUD_MaxPTMs.Value = Sweet.lollipop.max_ptms;

            nUD_NumDecoyDBs.Minimum = 0;
            nUD_NumDecoyDBs.Maximum = 50;
            nUD_NumDecoyDBs.Value = Sweet.lollipop.decoy_databases;

            nUD_MinPeptideLength.Minimum = 0;
            nUD_MinPeptideLength.Maximum = 20;
            nUD_MinPeptideLength.Value = Sweet.lollipop.min_peptide_length;

            ckbx_combineIdenticalSequences.IsChecked = Sweet.lollipop.combine_identical_sequences;
            ckbx_combineTheoreticalsByMass.IsChecked = Sweet.lollipop.combine_theoretical_proteoforms_byMass;
            cb_limitLargePtmSets.IsChecked = Sweet.lollipop.theoretical_database.limit_triples_and_greater;
            cb_useRandomSeed.IsChecked = Sweet.lollipop.useRandomSeed_decoys;
            nud_randomSeed.Value = Sweet.lollipop.randomSeed_decoys;
            ckbx_OxidMeth.IsChecked = Sweet.lollipop.methionine_oxidation;
            ckbx_Meth_Cleaved.IsChecked = Sweet.lollipop.methionine_cleavage;
            ckbx_Carbam.IsChecked = Sweet.lollipop.carbamidomethylation;
            ckbx_combineIdenticalSequences.IsChecked = Sweet.lollipop.combine_identical_sequences;
            ckbx_combineTheoreticalsByMass.IsChecked = Sweet.lollipop.combine_theoretical_proteoforms_byMass;

            tb_modTypesToExclude.Text = String.Join(",", Sweet.lollipop.mod_types_to_exclude);

            tb_tableFilter.TextChanged -= tb_tableFilter_TextChanged;
            tb_tableFilter.Text = "";
            tb_tableFilter.TextChanged += tb_tableFilter_TextChanged;

            tb_modTableFilter.TextChanged -= tb_modTableFilter_TextChanged;
            tb_modTableFilter.Text = "";
            tb_modTableFilter.TextChanged += tb_modTableFilter_TextChanged;

            tb_totalTheoreticalProteoforms.Text = Sweet.lollipop.target_proteoform_community.theoretical_proteoforms.Length.ToString();
        }

        public void RunTheGamut(bool full_run)
        {
            ClearListsTablesFigures(true);
            Sweet.lollipop.theoretical_database.get_theoretical_proteoforms(Environment.CurrentDirectory);
            FillTablesAndCharts();
            if (!full_run && BottomUpReader.bottom_up_PTMs_not_in_dictionary.Count() > 0)
            {
                MessageBox.Show("Warning: the following PTMs in the .mzid file were not matched with any PTMs in the theoretical database: " +
                    String.Join(", ", BottomUpReader.bottom_up_PTMs_not_in_dictionary.Distinct()));
            }
        }

        public bool ReadyToRunTheGamut()
        {
            return Sweet.lollipop.theoretical_database.ready_to_make_database(Environment.CurrentDirectory);
        }

        public void ClearListsTablesFigures(bool clear_following)
        {
            dgv_Database.DataSource = null;
            dgv_Database.Rows.Clear();
            dgv_loadFiles.DataSource = null;
            dgv_loadFiles.Rows.Clear();
            dgv_unlocalizedModifications.DataSource = null;
            dgv_unlocalizedModifications.Rows.Clear();
            tb_modTableFilter.Clear();
            tb_tableFilter.Clear();
            tb_totalTheoreticalProteoforms.Clear();
            if (clear_following)
            {
                for (int i = ((ProteoformSweet)MdiParent).forms.IndexOf(this) + 1; i < ((ProteoformSweet)MdiParent).forms.Count; i++)
                {
                    ISweetForm sweet = ((ProteoformSweet)MdiParent).forms[i];
                    if (sweet as RawExperimentalComponents == null && (Sweet.lollipop.target_proteoform_community.experimental_proteoforms.Any(e => e.topdown_id) || sweet as AggregatedProteoforms == null))
                    {
                        sweet.ClearListsTablesFigures(false);
                    }
                }
            }
        }

        public void FillTablesAndCharts()
        {
            reload_database_list();
            DisplayUtility.FillDataGridView(dgv_Database, Sweet.lollipop.target_proteoform_community.theoretical_proteoforms.Select(t => new DisplayTheoreticalProteoform(t)));
            initialize_table_bindinglist();
            DisplayTheoreticalProteoform.FormatTheoreticalProteoformTable(dgv_Database);
            DisplayUtility.FillDataGridView(dgv_unlocalizedModifications, Sweet.lollipop.theoretical_database.unlocalized_lookup.Values.Select(m => new DisplayUnlocalizedModification(m)));
            DisplayUnlocalizedModification.FormatUnlocalizedModificationTable(dgv_unlocalizedModifications);
            tb_totalTheoreticalProteoforms.Text = Sweet.lollipop.target_proteoform_community.theoretical_proteoforms.Length.ToString();
        }

        #endregion Public Methods

        #region CHECKBOXES Private Methods

        private void ckbx_combineIdenticalSequences_CheckedChanged(object sender, EventArgs e)
        {
            Sweet.lollipop.combine_identical_sequences = ckbx_combineIdenticalSequences.Checked;
        }

        private void ckbx_combineTheoreticalsByMass_CheckedChanged(object sender, EventArgs e)
        {
            Sweet.lollipop.combine_theoretical_proteoforms_byMass = ckbx_combineTheoreticalsByMass.Checked;
        }

        private void ckbx_OxidMeth_CheckedChanged(object sender, EventArgs e)
        {
            Sweet.lollipop.methionine_oxidation = ckbx_OxidMeth.Checked;
        }

        private void ckbx_Carbam_CheckedChanged(object sender, EventArgs e)
        {
            Sweet.lollipop.carbamidomethylation = ckbx_Carbam.Checked;
        }

        private void ckbx_Meth_Cleaved_CheckedChanged(object sender, EventArgs e)
        {
            Sweet.lollipop.methionine_cleavage = ckbx_Meth_Cleaved.Checked;
        }

        private void btn_NaturalIsotopes_CheckedChanged(object sender, EventArgs e)
        {
            Sweet.lollipop.natural_lysine_isotope_abundance = btn_NaturalIsotopes.Checked;
        }

        private void btn_NeuCode_Lt_CheckedChanged(object sender, EventArgs e)
        {
            Sweet.lollipop.neucode_light_lysine = btn_NeuCode_Lt.Checked;
        }

        private void btn_NeuCode_Hv_CheckedChanged(object sender, EventArgs e)
        {
            Sweet.lollipop.neucode_heavy_lysine = btn_NeuCode_Hv.Checked;
        }

        private void nUD_MaxPTMs_ValueChanged(object sender, EventArgs e)
        {
            Sweet.lollipop.max_ptms = Convert.ToInt32(nUD_MaxPTMs.Value);
        }

        private void nUD_NumDecoyDBs_ValueChanged(object sender, EventArgs e)
        {
            Sweet.lollipop.decoy_databases = Convert.ToInt32(nUD_NumDecoyDBs.Value);
        }

        private void nUD_MinPeptideLength_ValueChanged(object sender, EventArgs e)
        {
            Sweet.lollipop.min_peptide_length = Convert.ToInt32(nUD_MinPeptideLength.Value);
        }

        private void cb_limitLargePtmSets_CheckedChanged(object sender, EventArgs e)
        {
            Sweet.lollipop.theoretical_database.limit_triples_and_greater = cb_limitLargePtmSets.Checked;
        }

        #endregion CHECKBOXES Private Methods



        public ProteoformSweet MDIParent { get; set; }
        

        public void OnClosing(ITabbedMDI sender)
        {
            throw new NotImplementedException();
        }
        

        private void cmbx_DisplayWhichDB_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmb_empty_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void dgv_loadFiles_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {

        }

        private void dgv_loadFiles_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {

        }

        private void btn_addFiles_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_downloadUniProtPtmList_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_clearFiles_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_NaturalIsotopes_CheckedChanged(object sender, RoutedEventArgs e)
        {

        }

        private void btn_NeuCode_Lt_CheckedChanged(object sender, RoutedEventArgs e)
        {

        }

        private void btn_NeuCode_Hv_CheckedChanged(object sender, RoutedEventArgs e)
        {

        }

        private void ckbx_Carbam_CheckedChanged(object sender, RoutedEventArgs e)
        {

        }

        private void ckbx_OxidMeth_CheckedChanged(object sender, RoutedEventArgs e)
        {

        }

        private void ckbx_Meth_Cleaved_CheckedChanged(object sender, RoutedEventArgs e)
        {

        }

        private void nud_randomSeed_ValueChanged(object sender, EventArgs e)
        {

        }

        private void nUD_NumDecoyDBs_ValueChanged(object sender, EventArgs e)
        {

        }

        private void cb_useRandomSeed_CheckedChanged(object sender, RoutedEventArgs e)
        {

        }

        private void nUD_MinPeptideLength_ValueChanged(object sender, EventArgs e)
        {

        }

        private void ckbx_combineIdenticalSequences_CheckedChanged(object sender, RoutedEventArgs e)
        {

        }

        private void nUD_MaxPTMs_ValueChanged(object sender, EventArgs e)
        {

        }

        private void cb_limitLargePtmSets_CheckedChanged(object sender, RoutedEventArgs e)
        {

        }

        private void tb_modTypesToExclude_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void tb_modTypesToExclude_TextChanged(object sender, RoutedEventArgs e)
        {

        }

        private void ckbx_combineTheoreticalsByMass_CheckedChanged(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Make_Databases_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_saveModNames_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_amendModNames_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_loadModNames_Click(object sender, RoutedEventArgs e)
        {

        }

        private void tb_modTableFilter_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void tb_tableFilter_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
