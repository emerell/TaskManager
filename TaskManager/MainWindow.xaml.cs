using FontAwesome.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace TaskManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ImageAwesome _loader;
        private ProcessListView _processesListView;

        public MainWindow()
        {
            InitializeComponent();
            ShowProcessesListView();
        }

        private void ShowLoader(bool isShow)
        {
            Loader.OnRequestLoader(MainGrid, ref _loader, isShow);
        }

        private void ShowProcessesListView()
        {
            MainGrid.Children.Clear();
            if (_processesListView == null)
                _processesListView = new ProcessListView(ShowLoader);
            MainGrid.Children.Add(_processesListView);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _processesListView?.Close();
            ProcessData.Close();
            base.OnClosing(e);
        }
    }
}
