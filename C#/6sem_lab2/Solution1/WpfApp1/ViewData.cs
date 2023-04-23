using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary1;
using System.Windows;
using System.Globalization;
using System.Windows.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using LiveCharts.Wpf;
using LiveCharts;

namespace WpfApp1
{
    internal class ViewData : INotifyPropertyChanged, IDataErrorInfo
    {
        private SeriesCollection seriesCollection;
        public SeriesCollection SeriesCollection
        {
            get { return seriesCollection; }
            set { seriesCollection = value; OnPropertyChanged(); }
        }
        private double a;
        public double A
        {
            get { return a; }
            set
            {
                a = value;
                OnPropertyChanged("A");
            }
        }
        private double b;
        public double B
        {
            get { return b; }
            set
            {
                b = value;
                OnPropertyChanged("B");
            }
        }
        private int numPoints;
        public int NumPoints
        {
            get { return numPoints; }
            set
            {
                numPoints = value;
                OnPropertyChanged("NumPoints");
            }
        }
        private bool isUniformGrid;
        public bool IsUniformGrid
        {
            get { return isUniformGrid; }
            set
            {
                isUniformGrid = value;
                OnPropertyChanged("IsUniformGrid");
            }
        }
        private int numSplines;
        public int NumSplines
        {
            get { return numSplines; }
            set
            {
                numSplines = value;
                OnPropertyChanged("NumSplines");
            }
        }
        public FRawEnum fRawEnum { get; set; } // Способ 1
        public List<FRaw> listFRaw { get; set; }  // Способ 2. Список делегатов
        public FRaw fRaw { get; set; }  // для способа 2
        private double lsdd;
        public double lsd
        {
            get { return lsdd; }
            set
            {
                lsdd = value;
                OnPropertyChanged("lsd");
            }
        }
        private double rsdd;
        public double rsd
        {
            get { return rsdd; }
            set
            {
                rsdd = value;
                OnPropertyChanged("rsd");
            }
        }
        private RawData rawdata;
        public RawData rawData
        {
            get { return rawdata; }
            set
            {
                rawdata = value;
                OnPropertyChanged("rawData");
            }
        }
        private SplineData splinedata;
        public SplineData splineData
        {
            get { return splinedata; }
            set
            {
                splinedata = value;
                OnPropertyChanged("splineData");
            }
        }
        public ViewData()
        {
            A = 0;
            B = 10;
            NumPoints = 5;
            NumSplines = 20;
            lsd = 0;
            rsd = 2;
            IsUniformGrid = false;
            //fRawEnum = FRawEnum.Linear; // Способ 1

            listFRaw = new List<FRaw>();       // для способа 2
            listFRaw.Add(RawData.Linear);      // для способа 2
            listFRaw.Add(RawData.Quadratic);   // для способа 2
            listFRaw.Add(RawData.Cubic); // для способа 2
            listFRaw.Add(RawData.Random);
            fRaw = listFRaw[1];                // для способа 2
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public string Error { get; set; }
        public string this[string property]
        {
            get
            {
                string Error = null;
                switch (property)
                {
                    case "NumPoints":
                        if (NumPoints < 2)
                            Error = "Число узлов сетки должно быть больше или равно 2.";
                        break;
                    case "NumSplines":
                        if (NumSplines < 3)
                            Error = "Число узлов равномерной сетки должно быть больше 2.";
                        break;
                    case "A":
                        if (A >= B)
                            Error = "Левый конец отрезка интерполяции должен быть меньше, чем правый конец отрезка.";
                        break;
                    case "B":
                        if (B <= A)
                            Error = "Левый конец отрезка интерполяции должен быть меньше, чем правый конец отрезка.";
                        break;
                }
                return Error;
            }
        }
        public void ExecuteSplines()
        {
            try
            {
                //// для способа 1
                //if (fRawEnum == FRawEnum.Quadratic) fRaw = RawData.Quadratic;
                //else if (fRawEnum == FRawEnum.Polynomial3) fRaw = RawData.Polynomial3;
                //else fRaw = RawData.Linear;

                rawData = new RawData(A, B, NumPoints, IsUniformGrid, fRaw);
                splineData = new SplineData(rawData, lsd, rsd, NumSplines);
                splineData.DoSplines();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void ExecuteSplinesFromFile()
        {
            try
            {
                //// для способа 1
                //if (fRawEnum == FRawEnum.Quadratic) fRaw = RawData.Quadratic;
                //else if (fRawEnum == FRawEnum.Polynomial3) fRaw = RawData.Polynomial3;
                //else fRaw = RawData.Linear;

                splineData = new SplineData(rawData, lsd, rsd, NumSplines);
                splineData.DoSplines();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public override string ToString()
        {
            return $"leftEnd = {A}\n" +
                   $"rightEnd = {B}\n" +
                   $"NumPoints = {NumPoints}\n" +
                   $"LeftSecondDerivative = {lsd}\n" +
                   $"RightSecondDerivative = {rsd}\n" +
                   $"NumSplines = {NumSplines}\n" +
                   $"fRaw = {fRaw.Method.Name}\n";
        }
        public void Save(string filename)
        {
            try
            {
                rawData.Save(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Load(string filename)
        {
            try
            {
                RawData rData;
                if(RawData.Load(filename, out rData))
                {
                    A = rData.A;
                    B = rData.B;
                    NumPoints = rData.NumPoints;
                    IsUniformGrid = rData.IsUniformGrid;
                    rawData = rData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
