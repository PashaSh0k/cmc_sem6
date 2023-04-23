using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Forms.Integration;
using System.Windows.Forms.Design;
using System.Windows.Forms.DataVisualization.Charting;
using LiveCharts;
using LiveCharts.Wpf;
using SeriesCollection = LiveCharts.SeriesCollection;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiveCharts.Defaults;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class GridValue
    {
        public double Point { get; set; }
        public double Value { get; set; }
        public override string ToString()
        {
            return $"{Point:F3} , {Value:F3}";
        }
    }

    public partial class MainWindow : Window
    {
        ViewData viewData = new ViewData();
        /* SeriesCollection seriesCollection;
        public SeriesCollection SeriesCollection
        {
            get { return seriesCollection; }
            set { seriesCollection = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        */
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = viewData;
            //winFormsHost.Child = chart;
            //comboBox_Enum.ItemsSource = Enum.GetValues(typeof(ClassLibrary1.FRawEnum));
        }
        /*private void TextBox_Error(object sender, ValidationErrorEventArgs e)
        {
            MessageBox.Show(e.Error.ErrorContent.ToString());
        }*/

        private void ToSave(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "RawData";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                viewData.Save(filename);
            }
        }

        private void RawDataFromControlsButton_Click(object sender, RoutedEventArgs e)
        {
            viewData.ExecuteSplines();
            //integralTextBlock.Text = viewData.splineData.Integral.ToString();
            //ListsRawData.ItemsSource = (System.Collections.IEnumerable)viewData.rawData;
        }

        private void RawDataFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                viewData.Load(filename);
                viewData.ExecuteSplinesFromFile();
            }
        }
        private void RDFCC(object sender, ExecutedRoutedEventArgs e)
        {
            viewData.ExecuteSplines();

            ChartValues<ObservablePoint> seriesFunc = new ChartValues<ObservablePoint>() { };
            for(int i = 0; i < viewData.NumPoints; i++)
            {
                seriesFunc.Add(new ObservablePoint(viewData.rawData.Points[i], viewData.rawData.Values[i]));
            }
            ChartValues<ObservablePoint> seriesSpline = new ChartValues<ObservablePoint>() { };
            for (int i = 0; i < viewData.NumSplines; i++)
            {
                seriesSpline.Add(new ObservablePoint(viewData.splineData.SplineDataItems[i].Point, viewData.splineData.SplineDataItems[i].SplineValue));
            }

            LineSeries lineSeriesFunc = new LineSeries
            {
                Title = "Function",
                Values = seriesFunc,
                Stroke = Brushes.Blue,
                StrokeThickness = 1
            };
            LineSeries lineSeriesSpline = new LineSeries
            {
                Title = "Spline",
                Values = seriesSpline,
                Stroke = Brushes.Orange,
                StrokeThickness = 1,
                LineSmoothness = 0
            };
            // Добавление LineSeries в коллекцию SeriesCollection
            LiveCharts.SeriesCollection seriesCollection = new LiveCharts.SeriesCollection();
            seriesCollection.Add(lineSeriesFunc);
            seriesCollection.Add(lineSeriesSpline);
            viewData.SeriesCollection = seriesCollection;
        }
        private void CanRDFCC(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Validation.GetHasError(textBox_A) == true)
            {
                e.CanExecute = false;
                return;
            }
            if (Validation.GetHasError(textBox_B) == true)
            {
                e.CanExecute = false;
                return;
            }
            if (Validation.GetHasError(textBox_NumSplines) == true)
            {
                e.CanExecute = false;
                return;
            }
            if (Validation.GetHasError(textBox_NumPoints) == true)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = true;
        }
        
        private void RDFFC(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                viewData.Load(filename);

                if (!((viewData != null) && (viewData.NumPoints >= 2) && (viewData.NumSplines > 2) && (viewData.A < viewData.B)))
                {
                    MessageBox.Show("not correct values");
                    return;
                }
                viewData.ExecuteSplinesFromFile();
                ChartValues<ObservablePoint> seriesFunc = new ChartValues<ObservablePoint>() { };
                for (int i = 0; i < viewData.NumPoints; i++)
                {
                    seriesFunc.Add(new ObservablePoint(viewData.rawData.Points[i], viewData.rawData.Values[i]));
                }
                ChartValues<ObservablePoint> seriesSpline = new ChartValues<ObservablePoint>() { };
                for (int i = 0; i < viewData.NumSplines; i++)
                {
                    seriesSpline.Add(new ObservablePoint(viewData.splineData.SplineDataItems[i].Point, viewData.splineData.SplineDataItems[i].SplineValue));
                }

                LineSeries lineSeriesFunc = new LineSeries
                {
                    Title = "Function",
                    Values = seriesFunc,
                    Stroke = Brushes.Blue,
                    StrokeThickness = 1
                };
                LineSeries lineSeriesSpline = new LineSeries
                {
                    Title = "Spline",
                    Values = seriesSpline,
                    Stroke = Brushes.Orange,
                    StrokeThickness = 1,
                    LineSmoothness = 0
                };
                // Добавление LineSeries в коллекцию SeriesCollection
                LiveCharts.SeriesCollection seriesCollection = new LiveCharts.SeriesCollection();
                seriesCollection.Add(lineSeriesFunc);
                seriesCollection.Add(lineSeriesSpline);
                viewData.SeriesCollection = seriesCollection;
            }
        }

        private void CanRDFFC(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void CanSave(object sender, CanExecuteRoutedEventArgs e)
        {
            if (viewData.rawData != null)
            {
                if (Validation.GetHasError(textBox_A) == true)
                {
                    e.CanExecute = false;
                    return;
                }
                if (Validation.GetHasError(textBox_B) == true)
                {
                    e.CanExecute = false;
                    return;
                }
                if (Validation.GetHasError(textBox_NumSplines) == true)
                {
                    e.CanExecute = false;
                    return;
                }
                if (Validation.GetHasError(textBox_NumPoints) == true)
                {
                    e.CanExecute = false;
                    return;
                }
                e.CanExecute = true;
            }
            else e.CanExecute = false;
        }

        private void SaveRawData(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "RawData";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                viewData.Save(filename);
            }
        }
    }
    public class TwoValuesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{values[0]} {values[1]}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var str = (string)value;
            var values = str.Split(' ');
            return new object[] { double.Parse(values[0]), double.Parse(values[1]) };
        }
    }
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return value;
        }
    }
    public class RawDataToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RawData rawData)
            {
                List<GridValue> gridValues = new List<GridValue>();
                for (int i = 0; i < rawData.NumPoints; i++)
                {
                    GridValue gridValue = new()
                    {
                        Point = rawData.Points[i],
                        Value = rawData.Values[i]
                    };
                    gridValues.Add(gridValue);
                }

                return gridValues;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class SplineDataItemTostringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SplineDataItem sItem)
            {
                return $"Point: {sItem.Point:F2}\nSplineValue: {sItem.SplineValue:F2}\nFirstDerivative: {sItem.FirstDerivative:F2}\nSecondDerivative: {sItem.SecondDerivative:F2}";
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public static class CustomCommands
    {
        public static RoutedCommand RawDataFromControlsCommand = new RoutedCommand("RawDataFromControlsCommand", typeof(WpfApp1.CustomCommands));
        public static RoutedCommand RawDataFromFileCommand = new RoutedCommand("RawDataFromFileCommand", typeof(WpfApp1.CustomCommands));
    }
}

