using ClassLibrary1;
using System;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            func.ItemsSource = Enum.GetValues(typeof(FuncEnum));
        }

        private void Click(object sender, RoutedEventArgs e)
        {
            V1DataList obj = new V1DataList(DataInfo.Text, DateTime.Now);
            obj.AddDefaults(int.Parse(nItems.Text), (FuncEnum)func.SelectedItem);
            OutBase.Text = "Str: " + DataInfo.Text + '\n' + "Date: " + DateTime.Now;
            Lists.ItemsSource = obj.node;
        }

        private void Lists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show(Lists.SelectedItem.ToString());
            InfoList.Text = Lists.SelectedItem.ToString();
        }
    }
}
