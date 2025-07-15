using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;

namespace kurss
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<double> xv = new List<double>();
        List<double> yv = new List<double>();
        private void Form1_Load(object sender, EventArgs e)
        {

            this.dataGridView2.Columns[0].Width = 70;
            this.dataGridView2.Columns[1].Width = 75;
            this.dataGridView2.Columns[2].Width = 78;
            this.dataGridView2.Columns[3].Width = 78;
            this.dataGridView2.Columns[4].Width = 78;
            chart1.Series[0].LegendText = "Исходный график";
            chart1.Series[1].LegendText = "График по Лагранжу";
            chart1.Series[2].LegendText = "График по Ньютону";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                xv.Clear();
                yv.Clear();
                dataGridView1.Rows.Clear();
                double a = double.Parse(textBox1.Text);
                double b = double.Parse(textBox2.Text);
                Random random = new Random();
                for (double x = a; x <= b; x += random.NextDouble() * (0.5 - 0.1) + 0.1)
                {
                    double y = 1.8 * Math.Pow(x, 2) - Math.Sin(10 * x);
                    dataGridView1.Rows.Add(Math.Round(x, 3), Math.Round(y, 3));
                    xv.Add(Math.Round(x, 3));
                    yv.Add(Math.Round(y, 3));
                }
            }
            catch
            {
                MessageBox.Show("Wrong");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart1.Series[2].Points.Clear();
            for (int i = 0; i < xv.Count; i++)
            {
                double x = xv[i];
                double y = yv[i];
                chart1.Series[0].Points.AddXY(x, y);
            }
        }
        static double[,] CalculateDividedDifferences(List<double> x, List<double> y)
        {
            int n = x.Count;
            double[,] dividedDifferences = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                dividedDifferences[i, 0] = y[i];
            }

            for (int j = 1; j < n; j++)
            {
                for (int i = 0; i < n - j; i++)
                {
                    dividedDifferences[i, j] = (dividedDifferences[i + 1, j - 1] - dividedDifferences[i, j - 1]) / (x[i + j] - x[i]);
                }
            }

            return dividedDifferences;
        }

        static double Interpolatte(double[,] dividedDifferences, List<double> x, double targetX)
        {
            int n = x.Count;
            double interpolatedY = dividedDifferences[0, 0];

            for (int i = 1; i < n; i++)
            {
                double term = dividedDifferences[0, i];
                for (int j = 0; j < i; j++)
                {
                    term *= (targetX - x[j]);
                }
                interpolatedY += term;
            }

            return interpolatedY;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart1.Series[2].Points.Clear();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            double arg = double.Parse(textBox3.Text);
            double[,] dividedDifferences = CalculateDividedDifferences(xv, yv);
            double ans = Interpolatte(dividedDifferences, xv, arg);
            for (int i = 0; i < xv.Count; i++)
            {   
                double interpolatedY = Interpolatte(dividedDifferences, xv, xv[i]);
                chart1.Series[2].Points.AddXY(xv[i], interpolatedY);
            }
            dataGridView2.Rows[0].Cells[0].Value = double.Parse(textBox3.Text);
            dataGridView2.Rows[0].Cells[1].Value = Math.Round(ans, 6);
            stopwatch.Stop();
            dataGridView2.Rows[0].Cells[2].Value = stopwatch.ElapsedMilliseconds;

        }
        static double[] Interpolate(List<double> x, List<double> y)
        {
            int n = x.Count;
            double[] interpolatedValues = new double[n];

            for (int i = 0; i < n; i++)
            {
                double sum = 0;

                for (int j = 0; j < n; j++)
                {
                    double product = 1;

                    for (int k = 0; k < n; k++)
                    {
                        if (k != j)
                        {
                            product *= (x[i] - x[k]) / (x[j] - x[k]);
                        }
                    }

                    sum += y[j] * product;
                }

                interpolatedValues[i] = sum;
            }

            return interpolatedValues;
        }
        static double LagrangeInterpolation(List<double> x, List<double> y, double xi)
        {
            int n = x.Count;
            double sum = 0;
            for (int j = 0; j < n; j++)
            {
                double product = 1;

                for (int k = 0; k < n; k++)
                {
                    if (k != j)
                    {
                        product *= (xi - x[k]) / (x[j] - x[k]);
                    }
                }

                sum += y[j] * product;
            }
            return sum;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart1.Series[2].Points.Clear();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            double arg = double.Parse(textBox3.Text);
            double ans = LagrangeInterpolation(xv, yv, arg);
            double[] interpolatedValues = Interpolate(xv, yv);
          
            for (int i = 0; i < xv.Count; i++)
            {
                chart1.Series[1].Points.AddXY(xv[i], interpolatedValues[i]);
            }
            dataGridView2.Rows[0].Cells[0].Value = double.Parse(textBox3.Text);
            dataGridView2.Rows[0].Cells[3].Value = Math.Round(ans,6);
            stopwatch.Stop();
            dataGridView2.Rows[0].Cells[4].Value = stopwatch.ElapsedMilliseconds;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
