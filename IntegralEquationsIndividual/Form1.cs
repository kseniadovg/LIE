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

namespace IntegralEquationsIndividual
{
    public partial class Form1 : Form
    {
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
namespace MyNamespace
{
    public delegate double Del(double t);
    public static class LambdaCreator
    {
        public static Del Create()
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

        static void CreateF(string text, ref Delegate del)
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
        static void CreateFFromTwoVariables(string text, ref Delegate del)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            parameters.ReferencedAssemblies.Add("System.dll");
            CompilerResults results;
            results = provider.CompileAssemblyFromSource(parameters, begin1 + text + end);
            var cls = results.CompiledAssembly.GetType("MyNamespace.LambdaCreator");
            var method = cls.GetMethod("Create", BindingFlags.Static | BindingFlags.Public);
            del = method.Invoke(null, null) as Delegate;
        }

        private double GreenFunction(Vector<Delegate> y, Vector<double> x, double t)
        {
            double GreenFunction;
            double a;
            double b;
            //Green Function Variables
            double R = double.Parse(textBox1.Text);
            double rZero = Math.Sqrt((Math.Pow((double)y.a.DynamicInvoke(t), 2) + Math.Pow((double)y.a.DynamicInvoke(t), 2)));
            double rAStarP = Math.Sqrt(Math.Pow(x.a - (Math.Pow(R, 2) / Math.Pow(rZero, 2)) * (double)y.a.DynamicInvoke(t),2)
                + Math.Pow(x.b - (Math.Pow(R, 2) / Math.Pow(rZero, 2)) * (double)y.b.DynamicInvoke(t), 2));
            //Constructing elements of the vector
            a = -((double)y.a.DynamicInvoke(t) / Math.Pow(rZero, 2))
                + ((Math.Pow(R, 2) / Math.Pow(rZero, 2)) * (x.a - (Math.Pow(R, 2) / Math.Pow(rZero, 2)) * (double)y.a.DynamicInvoke(t)) - (x.a - (double)y.a.DynamicInvoke(t))) / Math.Pow(rAStarP, 2);
            b = -((double)y.b.DynamicInvoke(t) / Math.Pow(rZero, 2))
                + ((Math.Pow(R, 2) / Math.Pow(rZero, 2)) * (x.b - (Math.Pow(R, 2) / Math.Pow(rZero, 2)) * (double)y.b.DynamicInvoke(t)) - (x.b - (double)y.b.DynamicInvoke(t))) / Math.Pow(rAStarP, 2);
            //Count Mju function
            Vector<double> mju = Mju(t);
            //Count Green function
            GreenFunction = 1 / (2 * Math.PI) * (a * mju.a + b * mju.b);
            return GreenFunction;
        }

        private double Integral(Delegate fZero, Vector<Delegate> y, Vector<double> x)
        {
            double a = (double)fZero.DynamicInvoke((double)y.a.DynamicInvoke(0), (double)y.b.DynamicInvoke(0)) * GreenFunction(y, x, 0);
            double sum = 0;
            for (double i = 0.05; i < 2*Math.PI; i+=0.05)
            {
                sum += (double)fZero.DynamicInvoke((double)y.a.DynamicInvoke(i), (double)y.b.DynamicInvoke(i)) * GreenFunction(y, x, i);
            }
            double b = (double)fZero.DynamicInvoke((double)y.a.DynamicInvoke(2 * Math.PI), (double)y.b.DynamicInvoke(2 * Math.PI)) * GreenFunction(y, x, 2 * Math.PI);
            return a + sum + b;
        }


        private Vector<double> Mju(double t)
        {
            double KsiPrime = - 2 * Math.Cos(t);
            double PhiPrime = 2 * Math.Sin(t);
            double a = KsiPrime / Math.Sqrt(Math.Pow(KsiPrime, 2) + Math.Pow(PhiPrime, 2));
            double b = PhiPrime / Math.Sqrt(Math.Pow(KsiPrime, 2) + Math.Pow(PhiPrime, 2));
            Vector<double> Mju = new Vector<double>(a, b);
            return Mju;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // how to use
            CreateF("Math.Sin(t)", ref f);
            CreateFFromTwoVariables(textBox7.Text, ref fZero);
            CreateF(textBox1.Text + "*Math.Cos(t)", ref y1);
            CreateF(textBox2.Text + "*Math.Sin(t)", ref y2);
            Vector<Delegate> y = new Vector<Delegate>(y1, y2);
            Vector<double> x = new Vector<double>(0, 1);

            double I = Integral(fZero, y, x);

            double d = (double)f.DynamicInvoke(0.5 * Math.PI);
        }        
    }
}
