using System;
using System.Windows;
using System.Windows.Controls;
using ProteoformSuiteInternal;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ProteoWPFSuite
{
    /// <summary>
    /// Interaction logic for testWin.xaml
    /// </summary>
    public partial class testWin : Window
    {
        public testWin()
        {
            InitializeComponent();
            
        }

        private void Dissoci_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void testFunc(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            this.test.cmb_loadTable.Text = "test"+rnd.Next();
        }
    }
}
