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

        #region FunctionTemplates
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
        #endregion

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

        private void button1_Click(object sender, EventArgs e)
        {
            InitializeProblem();

            solver = new Solver(p, double.Parse(textBox14.Text));

            MessageBox.Show("Done!");
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
            solver.step = double.Parse(textBox14.Text);
            solver.GenerateMesh();
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
            PointPairList list3 = new PointPairList();

            for (double t = 0; t <= Math.PI * 2; t += 0.05)
            {
                list.Add(solver.p.Gamma0(t).a, solver.p.Gamma0(t).b);
                list1.Add((double)solver.p.Gamma1.a.DynamicInvoke(t), (double)solver.p.Gamma1.b.DynamicInvoke(t));
                list2.Add((double)solver.p.Gamma2.a.DynamicInvoke(t), (double)solver.p.Gamma2.b.DynamicInvoke(t));
            }

            foreach (var v in solver.p.mesh)
            {
                list3.Add(v.a, v.b);
            }

            LineItem myCurve = pane.AddCurve("", list, Color.DarkRed, SymbolType.None);
            myCurve = pane.AddCurve("", list1, Color.Blue, SymbolType.None);
            myCurve = pane.AddCurve("", list2, Color.Green, SymbolType.None);

            LineItem myCurve1 = pane.AddCurve("", list3, Color.Black, SymbolType.Star);
            myCurve1.Line.IsVisible = false;
            myCurve1.Symbol.Fill.Color = Color.Black;
            myCurve1.Symbol.Fill.Type = FillType.Solid;
            myCurve1.Symbol.Size = 7;

            pane.Title.Text = "Graph";
            pane.XAxis.Title.Text = "x";
            pane.YAxis.Title.Text = "y";
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }
    }
}
