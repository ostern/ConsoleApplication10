using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Converter
{
    class DictionaryQ<TType> : Dictionary<string, Dictionary<string, TType>>
    {
        // Add elements do dictionary
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

        public Dictionary<string, double> FindValue(string input)
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

    class Input
    {
        internal int koef { get; set; }
        internal int qty { get; set; }
        internal string valued { get; set; }

        public Input(int qty, int koef, string valued)
        {
            this.koef = koef;
            this.qty = qty;
            this.valued = valued;
        }
    }


    class Converter : DictionaryQ<double>
    {
        // constructor with elements. logicaly is better to pass a whole dictionary here.
        //change to (string input, string findthis) to use parameters
        public Converter()
        {
            Add("Length", "inch", 39.3701);
            Add("Length", "meter", 1);
            Add("Length", "foot", 3.2808);
            Add("Bigdata", "byte", 8);
            Add("Bigdata", "bit", 1);

            // Run(input, findthis);
        }

        // Dictionay of behaviours for calculation. Search by first name of dictionary

        private Dictionary<string, Func<double, double, int, int, double>> _operations =
            new Dictionary<string, Func<double, double, int, int, double>>
            {
                {"Length", (unit, target, qty, koef) => 1/unit*target*qty*Math.Pow(10, koef)},
                {"Bigdata", (unit, target, qty, koef) => qty*unit*Math.Pow(10, koef)/target},
            };

        public enum Prefixes
        {
            yotta = 24,
            zetta = 21,
            exa = 18,
            peta = 15,
            tera = 12,
            giga = 9,
            mega = 6,
            kilo = 3,
            hecto = 2,
            deca = 1,
            deci = -1,
            centi = -2,
            milli = -3,
            micro = -6,
            nano = -9,
            pico = -12,
            femto = -15,
            atto = -18,
            zepto = -21,
            yocto = -24
        }

        // add operation to Dictionay of behaviours for calculation.
        public void AddOperation(string op, Func<double, double, int, int, double> body)
        {
            if (_operations.ContainsKey(op))
                throw new ArgumentException(string.Format("Operation {0} already exists", op), "op");
            _operations.Add(op, body);
        }

        public Input ParseString(string input)
        {
            string pat = string.Join("|", Enum.GetNames(typeof (Prefixes)));
            var pattern = (@"(\d*)\s*(" + pat + @")*(\w+?)s*$");
            Match match = Regex.Match(input.ToLower(), pattern);
            if (!match.Success)
            {
                throw new ArgumentException("Malformed string");
            }
            var koef = 0;
            var qty = int.Parse(match.Groups[1].Value);
            var valued = match.Groups[3].Value;
            if (match.Groups[2].Value != "")
            {
                koef = (int) (Prefixes) Enum.Parse(typeof (Prefixes), match.Groups[2].Value);
            }
            return new Input(qty, koef, valued);
        }

        public void Calculate(Dictionary<string, double> from, Dictionary<string, double> to, Input input, string output)
        {
            if (!_operations.ContainsKey(from.First().Key))
            {
                throw new ArgumentException(string.Format("Operation is invalid"), "op");
            }

            Console.WriteLine("{0:F} {1}",
                _operations[from.First().Key](from.First().Value, to.First().Value, input.qty, input.koef), output);
        }

        // start method 
        public void Convert(string input, string output)
        {
            var nInput = ParseString(input);

            var unit = FindValue(nInput.valued);
            var target = FindValue(output);
            if (unit == null || target == null)
            {
                throw new ArgumentException("invalid Vilue", "dictionary");
            }
            if (unit.First().Key != target.First().Key)
            {
                throw new ArgumentException("invalid convertion", "dictionary");
            }
            this.Calculate(unit, target, nInput, output);
        }

        static void Main(string[] args)
        {
            Converter testProgram = new Converter();

            testProgram.Convert("10 kilometers", "inch");
            testProgram.Convert("10 foots", "inch");
            testProgram.Convert("15 kilofoots", "inch");
            testProgram.Convert("64 bit", "byte");
            testProgram.Convert("64 bytes", "bit");
            testProgram.Add("Length", "parcel", 0.005);
            testProgram.Convert("1 parcels", "inch");
        }
    }
}
