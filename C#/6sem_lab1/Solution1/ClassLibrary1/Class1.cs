using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClassLibrary1
{
    public struct GridValue
    {
        public double Point { get; set; }
        public double Value { get; set; }
    }
    public delegate double FRaw(double x);

    public enum FRawEnum
    {
        Linear,
        Cubic,
        Random
    }
    public class RawData
    {
        public double A { get; set; }
        public double B { get; set; }
        public int NumPoints { get; set; }
        public bool IsUniformGrid { get; set; }
        public FRaw F { get; set; }
        public double[] Points { get; set; }
        public double[] Values { get; set; }
        public List<GridValue> gridValues = new List<GridValue>();

        public RawData(double a, double b, int numPoints, bool isUniformGrid, FRaw fRaw)
        {
            A = a;
            B = b;
            NumPoints = numPoints;
            IsUniformGrid = isUniformGrid;
            F = fRaw;
            Points = new double[numPoints];
            Values = new double[numPoints];

            if (IsUniformGrid)
            {
                double h = (b - a) / (numPoints - 1);
                for(int i = 0; i < numPoints; i++)
                {
                    Points[i] = a + i * h;
                }
            }
            else
            {
                double h = (b - a) / (numPoints - 1);
                Points[0] = a;
                Points[numPoints - 1] = b;
                for (int i = 1; i < numPoints - 1; i++)
                {
                    Random randomize = new Random();
                    Points[i] = a + h * (i + randomize.NextDouble());
                }
            }

            for (int i = 0; i < numPoints; i++)
            {
                Values[i] = F(Points[i]);
            }
        }

        public RawData(string fileName)
        {
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(fileName);
                string line = reader.ReadLine();
                string[] arr = line.Split(' ');
                A = double.Parse(arr[0]);
                B = double.Parse(arr[1]);
                NumPoints = int.Parse(arr[2]);
                IsUniformGrid = bool.Parse(arr[3]);
                //string fRawString = reader.ReadLine();
                //F = (x) => double.Parse(fRawString.Replace("x", x.ToString()));

                Points = new double[NumPoints];
                Values = new double[NumPoints];

                for (int i = 0; i < NumPoints; i++)
                {
                    line = reader.ReadLine();
                    arr = line.Split(' ');
                    Points[i] = double.Parse(arr[0]);
                    Values[i] = double.Parse(arr[1]);
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while reading the file.", e);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        public static double[] InitValues(FRaw fRaw, double[] points)
        {
            double[] values = new double[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                values[i] = fRaw(points[i]);
            }
            return values;
        }
        public static double[] InitValues(FRawEnum fRawEnum, double[] points)
        {
            double[] values = new double[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                switch (fRawEnum)
                {
                    case FRawEnum.Linear:
                        values[i] = points[i];
                        break;
                    case FRawEnum.Cubic:
                        values[i] = points[i] * points[i] * points[i];
                        break;
                    case FRawEnum.Random:
                        Random rand = new Random();
                        values[i] = rand.NextDouble();
                        break;
                    default:
                        throw new ArgumentException("Invalid FRawEnum value");
                }
            }
            return values;
        }

        public void Save(string filename)
        {
            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter(filename);
                writer.WriteLine($"{A} {B} {NumPoints} {IsUniformGrid}");
                //writer.WriteLine($"{F}");
                for (int i = 0; i < NumPoints; i++)
                {
                    writer.WriteLine($"{Points[i]} {Values[i]}");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }
        public static bool Load(string filename, out RawData rawData)
        {
            rawData = null;
            try
            {
                rawData = new RawData(filename);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to read data from file {filename}. {e.Message}");
                return false;
            }
        }
        public static double Linear(double x)
        { return x; }
        public static double Quadratic(double x)
        { return x * (x - 1) + 2; }
        public static double Cubic(double x)
        { return x * (x - 1) * (x - 2) + 2; }
        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < NumPoints; i++)
            {
                str += Points[i] + Values[i] + '\n';
            }
            return str;
        }
    }
    public struct SplineDataItem
    {
        public double Point { get; set; }
        public double SplineValue { get; set; }
        public double FirstDerivative { get; set; }
        public double SecondDerivative { get; set; }

        public SplineDataItem(double point, double splineValue, double firstDerivative, double secondDerivative)
        {
            Point = point;
            SplineValue = splineValue;
            FirstDerivative = firstDerivative;
            SecondDerivative = secondDerivative;
        }

        public override string ToString()
        {
            //return System.String.Format("{0:F2}, {1:F2}, {2:F2}, {3:F2}", Point, SplineValue, FirstDerivative, SecondDerivative);
            return $"{Point} , {SplineValue},  {FirstDerivative}, {SecondDerivative} ";
        }

        public string ToString(string format)
        {
            return $"{Point.ToString(format)}, {SplineValue.ToString(format)}, {FirstDerivative.ToString(format)}, {SecondDerivative.ToString(format)}";
        }
    }
    
    public class SplineData : INotifyPropertyChanged
    {
        public RawData rawData { get; set; }
        public int NumNodes { get; set; }

        public double lsd { get; set; }
        public double rsd { get; set; }
        private List<SplineDataItem> splineDataItems;
        public List<SplineDataItem> SplineDataItems
        {
            get{ return splineDataItems; }
            set
            {
                splineDataItems = value;
                OnPropertyChanged("SplineDataItems");
            }
        }
        private double integ;
        public double Integral
        {
            get { return integ; }
            set
            {
                integ = value;
                OnPropertyChanged("Integral");
            }
        }
        public SplineData(RawData data, double leftSecondDerivative, double rightSecondDerivative, int nodeCount)
        {
            rawData = data;
            NumNodes = nodeCount;
            lsd = leftSecondDerivative;
            rsd = rightSecondDerivative;
            Integral = 0;
            SplineDataItems = new List<SplineDataItem>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public void DoSplines()
        {
            double[] ldrd = { lsd, rsd };
            double[] coeff = new double[4 * (rawData.NumPoints - 1)];
            double[] res = new double[3 * NumNodes];
            double[] integral = new double[1];
            double result = func(NumNodes, rawData.NumPoints, rawData.Points, rawData.Values, rawData.IsUniformGrid, ldrd, coeff, res, integral);
            Integral = integral[0];
            double step = (rawData.B - rawData.A) / (NumNodes - 1);
            for (int i = 0; i < NumNodes; i++)
            {
                SplineDataItem sdi = new SplineDataItem()
                {
                    Point = rawData.A + step * i,
                    SplineValue = res[3 * i],
                    FirstDerivative = res[3 * i + 1],
                    SecondDerivative = res[3 * i + 2]
                };
                SplineDataItems.Add(sdi);
            }
        }
        [DllImport("D:\\С#\\6sem_lab1\\Solution1\\x64\\Debug\\Dll1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double func(int NumSplines, int NumNodes, double[] Points, double[] Values, bool isUniform, double[] ldrd, double[] coeff, double[] res, double[] integral);
    }
}