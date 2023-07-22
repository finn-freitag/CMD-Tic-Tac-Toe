using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMD_Tic_Tac_Toe
{
    public class Perzeptron
    {
        public double[] input;
        public double[] weights;
        public double Error;

        public static Random Random = new Random();

        public Perzeptron(int inputcount, double bias, int weightMin, int weightMax)
        {
            input = new double[inputcount + 1];
            weights = new double[inputcount + 1];
            input[inputcount] = 1;
            weights[inputcount] = bias / (-1);
            for (int i = 0; i < inputcount; i++)
            {
                weights[i] = Random.Next(weightMin, weightMax);
            }
        }

        public double calc()
        {
            double sum = 0;
            for (int i = 0; i < input.Length; i++)
            {
                sum += input[i] * weights[i];
            }
            if (sum >= 0) return 1; else return 0;
        }

        public double calcSigmoid()
        {
            double sum = 0;
            for (int i = 0; i < input.Length; i++)
            {
                sum += input[i] * weights[i];
            }
            return Sigmoid(sum);
        }

        public double calc(int outputs)
        {
            double sum = 0;
            for (int i = 0; i < input.Length; i++)
            {
                sum += input[i] * weights[i] / outputs;
            }
            double sig = Sigmoid(sum);
            double res = sig * Convert.ToDouble(outputs);
            if (res > outputs) res = outputs;
            return res;
        }

        public double calcSum()
        {
            double sum = 0;
            for (int i = 0; i < input.Length; i++)
            {
                sum += input[i] * weights[i];
            }
            return sum;
        }

        public void trainWithCorrection(double result)
        {
            bool run = true;
            double alpha = 0.1;
            while (run)
            {
                double actuallresult = calc();
                if (GetDistance(actuallresult, result) < alpha) run = false;
                else
                {
                    for (int i = 0; i < input.Length; i++)
                    {
                        weights[i] = weights[i] + alpha * input[i] * (result - actuallresult);
                    }
                }
            }
        }

        public void trainSigmoidWithCorrection(double result)
        {
            bool run = true;
            double alpha = 0.1;
            while (run)
            {
                double actuallresult = calcSigmoid();
                if (GetDistance(actuallresult, result) < alpha) run = false;
                else
                {
                    for (int i = 0; i < input.Length; i++)
                    {
                        weights[i] = weights[i] + alpha * input[i] * (result - actuallresult);
                    }
                }
            }
        }

        public void trainWithCorrection(double result, int outputs)
        {
            bool run = true;
            double bigalpha = 1;
            double smallalpha = 0.00001;
            while (run)
            {
                double actuallresult = calc(outputs);
                if (GetDistance(actuallresult, result) < bigalpha) run = false;
                else
                {
                    for (int i = 0; i < input.Length; i++)
                    {
                        weights[i] = weights[i] + smallalpha * input[i] * (result - actuallresult);
                    }
                }
            }
        }

        private double GetDistance(double d1, double d2)
        {
            double d = d1 - d2;
            if (d < 0) d = d / (-1);
            return d;
        }

        public override string ToString()
        {
            string res = String.Empty;
            foreach (double d in weights)
            {
                res += ";" + Convert.ToString(d);
            }
            res = res.Substring(1);
            return res;
        }

        public void LoadWeightsFromString(string savedstr)
        {
            string[] strs = savedstr.Split(';');
            weights = new double[strs.Length];
            input = new double[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                weights[i] = Convert.ToDouble(strs[i]);
            }
        }

        public static double Sigmoid(double value)
        {
            return (double)(1.0 / (1.0 + Math.Pow(Math.E, -value)));
        }
    }
}
