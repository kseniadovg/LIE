using Microsoft.CSharp;
using System;
using System.IO;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace IntegralEquationsIndividual
{
    public partial class Form1 : Form
    {
        Problem p;
        Solver solver;

        private static string begin = @"using System;
namespace MyNamespace
{
    public delegate double Del(double t);
    public static class LambdaCreator
    {
        public static Del Create()
        {
            return (t)=>";

        private static string end = @";
        }
    }
}";
        private static string begin1 = @"using System;
namespace DFunction
{
    public delegate double Del(double y1, double y2);
    public static class CreateFunc
    {
        public static Del CreateF()
        {
            return (y1,y2)=>";

        Delegate f;
        Delegate fZero;
        Delegate y1;
        Delegate y2;

        public Form1()
        {
            InitializeComponent();
        }

        static void CreateF(string text, out Delegate del)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            parameters.ReferencedAssemblies.Add("System.dll");
            CompilerResults results;
            results = provider.CompileAssemblyFromSource(parameters, begin + text + end);
            var cls = results.CompiledAssembly.GetType("MyNamespace.LambdaCreator");
            var method = cls.GetMethod("Create", BindingFlags.Static | BindingFlags.Public);
            del = method.Invoke(null, null) as Delegate;
        }
        static void CreateFFromTwoVariables(string text, out Delegate del)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            parameters.ReferencedAssemblies.Add("System.dll");
            CompilerResults results;
            results = provider.CompileAssemblyFromSource(parameters, begin1 + text + end);
            var cls = results.CompiledAssembly.GetType("DFunction.CreateFunc");
            var method = cls.GetMethod("CreateF", BindingFlags.Static | BindingFlags.Public);
            del = method.Invoke(null, null) as Delegate;
        }

        //can't be evaluated on Г0
        private double GreenFunctionNormalDerivativeY(Vector<Delegate> y, Vector<Delegate> yDeriv, Vector<double> x, double t)
        {
            double GreenFunctionDer;
            double a;
            double b;
            Vector<double> yVect = new Vector<double>();
            yVect.a = (double)y.a.DynamicInvoke(t);
            yVect.b = (double)y.b.DynamicInvoke(t);

            //Green Function Variables
            double R = double.Parse(textBox1.Text);
            double rZero = Math.Sqrt((Math.Pow(yVect.a, 2) + Math.Pow(yVect.b, 2)));
            double rAP = Math.Sqrt(Math.Pow(x.a - yVect.a, 2) + Math.Pow(x.b - yVect.b, 2));
            double rAStarP = Math.Sqrt(Math.Pow(x.a - (Math.Pow(R / rZero, 2)) * yVect.a, 2)
                             + Math.Pow(x.b - (Math.Pow(R / rZero, 2)) * yVect.b, 2));

            double rZero2 = rZero * rZero;
            double rAP2 = rAP * rAP;
            double rAStarP2 = rAStarP * rAStarP;

            //Constructing elements of the vector
            if (Math.Abs(rAP) > 1e-10)
            {
                a = -(yVect.a / rZero2)
                    + ((Math.Pow(R / rZero, 2)) * (x.a - (Math.Pow(R / rZero, 2)) * yVect.a)) / rAStarP2
                    - (x.a - yVect.a) / rAP2;
                b = -(yVect.b / rZero2)
                    + ((Math.Pow(R / rZero, 2)) * (x.b - (Math.Pow(R / rZero, 2)) * yVect.b)) / rAStarP2
                    - (x.b - yVect.b) / rAP2;
            }
            else
            {
                double yDerivA = (double)yDeriv.a.DynamicInvoke(yVect.a);
                double yDerivB = (double)yDeriv.a.DynamicInvoke(yVect.b);

                double yDeriv2A = ((double)yDeriv.a.DynamicInvoke(yVect.a + 0.001) - (double)yDeriv.a.DynamicInvoke(yVect.a - 0.001))*2000;
                double yDeriv2B = ((double)yDeriv.b.DynamicInvoke(yVect.b + 0.001) - (double)yDeriv.b.DynamicInvoke(yVect.b - 0.001)) * 2000;
                double yDerivNorm2 = yDerivA * yDerivA + yDerivB * yDerivB;

                a = -(yVect.a / rZero2)
                    + ((Math.Pow(R / rZero, 2)) * (x.a - (Math.Pow(R / rZero, 2)) * yVect.a)) / rAStarP2
                    - (0.5 * yDeriv2A) / yDerivNorm2;
                b = -(yVect.b / rZero2)
                    + ((Math.Pow(R / rZero, 2)) * (x.b - (Math.Pow(R / rZero, 2)) * yVect.b)) / rAStarP2
                    - (0.5 * yDeriv2B) / yDerivNorm2;
            }

            //Count Mju function
            Vector<double> mju = Mju(t);

            //Count Green function
            GreenFunctionDer = (1 / (2 * Math.PI)) * (a * mju.a + b * mju.b);

            return GreenFunctionDer;
        }

        private double IntegralU0(Delegate fZero, Vector<Delegate> y, Vector<double> x)
        {
            double sum = 0;
            int count = 1;
            
            double a = 0.5 * ((double)fZero.DynamicInvoke((double)y.a.DynamicInvoke(0), (double)y.b.DynamicInvoke(0)))
                * GreenFunctionNormalDerivativeY(y,null, x, 0);

            for (double i = 0.05; i < 2 * Math.PI; i += 0.05)
            {
                sum += (double)fZero.DynamicInvoke((double)y.a.DynamicInvoke(i), (double)y.b.DynamicInvoke(i))
                    * GreenFunctionNormalDerivativeY(y,null, x, i);
                count++;
            }

            double b = 0.5 * (double)fZero.DynamicInvoke((double)y.a.DynamicInvoke(2 * Math.PI), (double)y.b.DynamicInvoke(2 * Math.PI))
                * GreenFunctionNormalDerivativeY(y,null, x, 2 * Math.PI);

            //2*pi*R/count
            return -(a + sum + b) * ((4 * Math.PI) / (double)count);
        }

        //mju is dependent on Гi, not only Г0
        private Vector<double> Mju(double t)
        {
            double KsiPrime = 2 * Math.Cos(t);
            double PhiPrime = -2 * Math.Sin(t);
            double a = KsiPrime / Math.Sqrt(Math.Pow(KsiPrime, 2) + Math.Pow(PhiPrime, 2));
            double b = - PhiPrime / Math.Sqrt(Math.Pow(KsiPrime, 2) + Math.Pow(PhiPrime, 2));
            Vector<double> Mju = new Vector<double>(a, b);
            return Mju;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //GaussMethodSLAEsolver gauss=new GaussMethodSLAEsolver() ;
            //double[,] mas = { { 1, 1, 1 }, { 1, 4, 3 }, { 3, 2, 1 } };
            //double[] vec = { 3, 8, 6 };
            //List<double> res = gauss.GaussSolve(mas, vec);
            
            InitializeProblem();
            
            solver = new Solver(p);

            MessageBox.Show("Done!");

            //CreateFFromTwoVariables(textBox7.Text, ref fZero);
            //double d1 = (double)fZero.DynamicInvoke(2, 0);
            //CreateF(textBox1.Text + "*Math.Cos(t)", ref y1);
            //CreateF(textBox2.Text + "*Math.Sin(t)", ref y2);
            //Vector<Delegate> y = new Vector<Delegate>(y1, y2);
            //Vector<double> x = new Vector<double>(1.9, 0);

            //double I = IntegralU0(fZero, y, x);
        }     
   
        void InitializeProblem()
        {
            Delegate Gamma1a, Gamma1b;
            Delegate Gamma2a, Gamma2b;
            Delegate Gamma1DerivativeA, Gamma1DerivativeB;
            Delegate Gamma2DerivativeA, Gamma2DerivativeB;
            Delegate f0, f1, f2;

            double R = double.Parse(textBox1.Text);

            CreateF(textBox3.Text, out Gamma1a);
            CreateF(textBox4.Text, out Gamma1b);
            CreateF(textBox5.Text, out Gamma2a);
            CreateF(textBox6.Text, out Gamma2b);
            CreateF(textBox10.Text, out Gamma1DerivativeA);
            CreateF(textBox11.Text, out Gamma1DerivativeB);
            CreateF(textBox12.Text, out Gamma2DerivativeA);
            CreateF(textBox13.Text, out Gamma2DerivativeB);
            CreateFFromTwoVariables(textBox7.Text, out f0);
            CreateFFromTwoVariables(textBox8.Text, out f1);
            CreateFFromTwoVariables(textBox9.Text, out f2);

            p = new Problem(R, new Vector<Delegate>(Gamma1a, Gamma1b), new Vector<Delegate>(Gamma2a, Gamma2b),
                new Vector<Delegate>(Gamma1DerivativeA, Gamma1DerivativeB),
                new Vector<Delegate>(Gamma2DerivativeA, Gamma2DerivativeB),
                f0, f1, f2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            solver.Solve();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Draw();
        }

        void Draw()
        {
            GraphPane pane = new GraphPane();
            pane = zedGraphControl1.GraphPane;
            pane.CurveList.Clear();
            PointPairList list = new PointPairList();
            PointPairList list1 = new PointPairList();
            PointPairList list2 = new PointPairList();

            for (double t = 0; t <= Math.PI*2; t += 0.05)
            {
                list.Add(solver.p.Gamma0(t).a, solver.p.Gamma0(t).b) ;
                list1.Add((double)solver.p.Gamma1.a.DynamicInvoke(t), (double)solver.p.Gamma1.b.DynamicInvoke(t));
                list2.Add((double)solver.p.Gamma2.a.DynamicInvoke(t), (double)solver.p.Gamma2.b.DynamicInvoke(t));
            }
            LineItem myCurve = pane.AddCurve("", list, Color.DarkRed, SymbolType.None);
            myCurve = pane.AddCurve("", list1, Color.Blue, SymbolType.None);
            myCurve = pane.AddCurve("", list2, Color.Green, SymbolType.None);

            pane.Title.Text = "Graph";
            pane.XAxis.Title.Text = "x";
            pane.YAxis.Title.Text = "f(x)";
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }
    }
}
