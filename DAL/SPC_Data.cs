using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL
{
    public class SPC_Data
    {
        public DateTime Date
        {
            get;
            set;
        }			// Scan Date/Time
        public string[] Blade
        {
            get
            {
                return this.name.ToArray();
            }
        }		// Name
        public double[] Value
        {
            get
            {
                return this.data.ToArray();
            }
        }		// Thickness
        public double[] Range
        {
            get
            {
                double[] returnValue = new double[this.data.Count];
                if (this.data.Count > 0)
                {
                    double lastVal = this.data[0];

                    for (int i = 0; i < this.data.Count; i++)
                    {
                        returnValue[i] = Math.Max(this.data[i], lastVal) - Math.Min(this.data[i], lastVal);
                        lastVal = this.data[i];
                    }
                }
                return returnValue;
            }
        }		// Difference between this and last thickness
        public double[] Max
        {
            get
            {
                double[] returnValue = new double[this.data.Count];
                for (int i = 0; i < this.data.Count; i++)
                {
                    returnValue[i] = this.MaxT;
                }

                return returnValue;
            }
        }			// Max Tolerance
        public double[] Min
        {
            get
            {
                double[] returnValue = new double[this.data.Count];
                for (int i = 0; i < this.data.Count; i++)
                {
                    returnValue[i] = this.MinT;
                }

                return returnValue;
            }
        }			// Min Tolerance
        public double[] Nom
        {
            get
            {
                double[] returnValue = new double[this.data.Count];
                for (int i = 0; i < this.data.Count; i++)
                {
                    returnValue[i] = this.NomT;
                }

                return returnValue;
            }
        }			// Nom Tolerance
        public double[] AvgValue
        {
            get
            {
                double[] returnValue = new double[this.data.Count];
                double Value = this.AverageValue();
                for (int i = 0; i < this.data.Count; i++)
                {
                    returnValue[i] = Value;
                }

                return returnValue;
            }
        }		// Average Thickness
        public double[] AvgRange
        {
            get
            {
                double[] returnValue = new double[this.data.Count];
                double Value = this.AverageRange();
                for (int i = 0; i < this.data.Count; i++)
                {
                    returnValue[i] = Value;
                }

                return returnValue;
            }
        }		// Average Range
        public double[] UCL
        {
            get
            {
                double[] returnValue = new double[this.data.Count];
                double Value = this.AverageValue() + (0.5f * natTol());
                for (int i = 0; i < this.data.Count; i++)
                {
                    returnValue[i] = Value;
                }

                return returnValue;
            }
        }			// Average(Thickness) + (0.5f * natTol)
        public double[] LCL
        {
            get
            {
                double[] returnValue = new double[this.data.Count];
                double Value = this.AverageValue() - (0.5f * natTol());
                for (int i = 0; i < this.data.Count; i++)
                {
                    returnValue[i] = Value;
                }

                return returnValue;
            }
        }			// Average(Thickness) - (0.5f * natTol)
        public double[] OutOfControl
        {
            get
            {
                double[] returnValue = new double[this.data.Count];
                double Value;
                double[] lcl = this.LCL;
                double[] ucl = this.UCL;

                for (int i = 0; i < this.data.Count; i++)
                {
                    Value = this.data[i];
                    if ((Value > ucl[i]) || (Value < lcl[i]))
                    {
                        returnValue[i] = Value;
                    }
                    else
                    {
                        returnValue[i] = 0;
                    }
                }

                return returnValue;
            }
        }	// Anything outside UCL || LCL

        public double CPK
        {
            get
            {
                return Math.Min(cpkU(), cpkL());
            }
        }			// Min(cpkU, cpkL)

        private double MaxT = 0f;
        private double MinT = 0f;
        private double NomT = 0f;
        private List<double> data = new List<double>();
        private List<string> name = new List<string>();

        public void SetTolerances(double MaxTol, double MinTol, double NomTol)
        {
            this.MaxT = MaxTol;
            this.MinT = MinTol;
            this.NomT = NomTol;
        }

        public void AddValue(string name, double value)
        {
            this.name.Add(name);
            this.data.Add(value);
        }

        private double natTol()
        {
            return ((6f * AverageRange()) / 1.128f);
        }

        private double cpkU()
        {
            return ((MaxT - AverageValue()) / (0.5f * natTol()));
        }

        private double cpkL()
        {
            return ((AverageValue() - MinT) / (0.5f * natTol()));
        }

        private double AverageValue()
        {
            return (double)(this.Sum(this.Value) / this.Value.Length);
        }

        private double AverageRange()
        {
            return (double)(this.Sum(this.Range) / this.Range.Length);
        }

        private double Sum(double[] values)
        {
            double total = 0f;
            foreach (double item in values)
            {
                total += item;
            }

            return total;
        }
    }
}
