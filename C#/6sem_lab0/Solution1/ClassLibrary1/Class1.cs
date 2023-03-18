using System.Collections;
using System.Numerics;

namespace ClassLibrary1
{
    public enum FuncEnum
    {
        Func1,
        Func2,
        Func3
    }
    public struct DataItem
    {
        public double x { get; set; }
        public System.Numerics.Complex value { get; set; }
        public DataItem(double a, Complex b)
        {
            x = a;
            value = b;
        }
        public string ToLongString(string format)
        {
            string result = string.Format(format, x) + string.Format(format, value);
            return result;
        }
        public override string ToString()
        {
            string result = x.ToString() + " " + value.ToString();
            return result;
        }
    }

    public abstract class V1Data : IEnumerable<DataItem>
    {
        List<DataItem> node;
        public string str { set; get; }
        public DateTime date { set; get; }
        virtual public double MaxMagnitude { get; set; }
        public V1Data(string s, DateTime dt)
        {
            str = s;
            date = dt;
        }
        public abstract string ToLongString(string format);
        public override string ToString()
        {
            string result = str + " " + date.ToString();
            return result;
        }
        public abstract IEnumerator<DataItem> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
        {
            return node.GetEnumerator();
        }
    }

    public class V1DataList : V1Data
    {
        public List<DataItem> node { get; set; }
        public V1DataList(string s, DateTime dt) : base(s, dt)
        {
            node = new List<DataItem>();
        }
        
        public void AddDefaults(int nItems, FuncEnum F)
        {
            Random rnd = new Random();

            for (int i = 0; i < nItems; i++)
            {
                double x = rnd.Next(0,10);
                Complex y;
                switch (F)
                {
                    case FuncEnum.Func1:
                        y = new Complex(0, x);
                        break;

                    case FuncEnum.Func2:
                        y = new Complex(x, 0);
                        break;

                    case FuncEnum.Func3:
                        y = new Complex(x, x);
                        break;
                    default:
                        throw new ArgumentException("Invalid function");
                }
                DataItem item = new DataItem(x, y);
                node.Add(item);
            }
        }
        public override string ToString()
        {
            string result = System.ComponentModel.TypeDescriptor.GetClassName(this);
            result += " " + str + " " + date.ToString() + " " + string.Format("{0:F2}", MaxMagnitude) + " " + (node.Count).ToString();
            return result;
        }
        public override string ToLongString(string format)
        {
            //string result = str.ToString() + " " + date.ToString() + " " + string.Format("{0:F2}", MaxMagnitude) + " " + (node.Count).ToString() + "\n";
            string result = ToString();
            result += "\n";
            for (int i = 0; i < node.Count; i++)
            {
                result += i.ToString() + ": point = " + string.Format(format, node[i].x) + " with value = ";
                result += string.Format(format, node[i].value) + "\n";
            }
            return result;
        }
        public override IEnumerator<DataItem> GetEnumerator()
        {
            return node.GetEnumerator();
        }
    }
}