using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConvertToFrom
{
    class DictionaryQ<TType> : Dictionary<string, Dictionary<string, TType>>
    {
       

    public void Add(string dictionaryKey, string key, TType value)
    {
      

            if (!ContainsKey(dictionaryKey))
                Add(dictionaryKey, new Dictionary<string, TType>());

            this[dictionaryKey].Add(key, value);
        }

        public TType Get(string dictionaryKey, string key)
        {
       
            return this[dictionaryKey][key];
        }

        public Dictionary<string, double> FindValue( string input)
        {
            
            foreach (var t in this.Keys)
            {
                if (this[t].ContainsKey(input))
                {
                    var result = new Dictionary<string, double>();
                    result.Add(t, double.Parse(this[t][input].ToString()));
                    return result;
                }
            }

            return null;
        }

    }


class Program : DictionaryQ<double>
    {   

        private int koef { get; set; }
        private int qty { get; set; }
        private string valued { get; set; }


        public enum Prefixes
        {
            yotta = 24, zetta = 21,
            exa = 18, peta = 15, tera = 12, giga = 9,mega = 6, kilo = 3, hecto = 2, deca = 1, deci = -1, centi = -2,
            milli = -3, micro = -6, nano = -9, pico = -12, femto = -15, atto = -18, zepto = -21, yocto = -24

        }
        private Dictionary<string, Func<double, double, int, int, double>> _operations =
            new Dictionary<string, Func<double, double, int, int, double>>
            {
                { "Length", (unit, target, qty, koef) => 1/unit*target*qty*Math.Pow(10, koef) },
                { "Bigdata", (unit, target, qty, koef) => qty*unit*Math.Pow(10, koef)/target },
            };

        public double PerformOperation(string op, double unit, double target, int qty, int koef)
            {
                if (!_operations.ContainsKey(op))
                   throw new ArgumentException(string.Format("Operation is invalid"), "op");
            return _operations[op](unit, target, qty, koef);
            }
        public void AddOperation(string op, Func<double, double, int, int, double> body)
            {
        if (_operations.ContainsKey(op))
            throw new ArgumentException(string.Format("Operation {0} already exists", op), "op");
            _operations.Add(op, body);
            }
        public void ParseString(string input)
        {
            string pat = string.Join("|", Enum.GetNames(typeof(Prefixes)));
            var pattern = (@"(\d*)\s*(" + pat + @")*(\w+?)s*$");
            Match match = Regex.Match(input.ToLower(), pattern );
            if (match.Success)
                {
                    qty = int.Parse(match.Groups[1].Value);
                    valued = match.Groups[3].Value;
                    if (match.Groups[2].Value == "")
                    {
                        koef = 0;
                    }
                    else
                    {
                        koef = (int) (Prefixes) Enum.Parse(typeof (Prefixes), match.Groups[2].Value);
                    }                      
                }

        }

        public void Calculate(string op, double unit, double target, int qty, int koef)
        {
            Console.WriteLine("{0:F}",PerformOperation(op, unit, target, qty, koef));
        }

        public void Magic(string input, string findthis)
        {
            this.ParseString(input);

            var unit = FindValue(valued);
            var target = FindValue(findthis);
            if (unit == null || target==null)
            { throw new ArgumentException("invalid Vilue", "dictionary"); }


            if (unit.First().Key == target.First().Key)
            {
                this.Calculate(unit.First().Key, unit.First().Value, target.First().Value, qty, koef);
            }
            else
            {
                throw new ArgumentException("invalid convertion", "dictionary");
            }
        }
   
    
        static void Main(string[] args)
        {


            Program testProgram = new Program();
  
            
              testProgram.Add("Length",  "inch", 39.3701);
                testProgram.Add("Length", "meter", 1);
                testProgram.Add("Length", "foot", 3.2808);
                testProgram.Add("Bigdata", "byte", 8);
                testProgram.Add("Bigdata", "bit", 1);
                testProgram.Add("Length", "parcel", 0.005);
                     

            testProgram.Magic("10 kilometers", "inch");
            testProgram.Magic("10 foots", "inch");
            testProgram.Magic("15 kilofoots", "inch");
            testProgram.Magic( "64 bit", "byte");
            testProgram.Magic("64 bytes", "bit");
 
            testProgram.Magic("1 parcels", "inch");

  


        }
    }
}
