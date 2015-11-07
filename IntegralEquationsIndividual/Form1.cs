using Microsoft.CSharp;
using System;
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

        Delegate f;

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

        private void button1_Click(object sender, EventArgs e)
        {
            // how to use
            CreateF("Math.Sin(t)", ref f);
            double d = (double)f.DynamicInvoke(0.5 * Math.PI);
        }        
    }
}
