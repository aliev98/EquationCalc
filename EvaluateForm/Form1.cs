using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Threading;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace EvaluateForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int y = 88;
        List<Control> textboxlist = new List<Control>();
        //List<textboxandlabel> textboxlist2 = new List<textboxandlabel>();
        string name1 = "";

        private void Form1_Load (object sender, EventArgs e)
        {
            Font myfont = new Font("Microsoft sant serif", 11.0f, FontStyle.Bold);
            txtFormula.Font = myfont;
            txtResult.Font = myfont;
            txtResult.ReadOnly = true;
            txtResult.BackColor = Color.White;

            btnCalculate.FlatStyle = FlatStyle.Flat;
            btnCalculate.BackColor = Color.Transparent;
            btnCalculate.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnCalculate.FlatAppearance.MouseOverBackColor = Color.Gray;
            btnCalculate.FlatAppearance.BorderColor = Color.FromName("Control");

            variable_add.FlatStyle = FlatStyle.Flat;
            variable_add.BackColor = Color.Transparent;
            variable_add.FlatAppearance.MouseDownBackColor = Color.Transparent;
            variable_add.FlatAppearance.MouseOverBackColor = Color.Gray;
            variable_add.FlatAppearance.BorderColor = Color.FromName("Control");
        }

        Control GetControlByName (string Name)
        {
            foreach (Control c in this.Controls)
                if (c.Name == Name)
                    return c;

            return null;
        }

        Control latestTextbox;
        private void variable_add_Click (object sender, EventArgs e)
        {

            name1 = Microsoft.VisualBasic.Interaction.InputBox ("Vad ska variabeln heta?", "Title", "", 170, 70);

            //var latestTextbox = GetControlByName (this.name1); // får fram den nyaste textboxen

            //foreach (var item in textboxlist)
            //{
            //    if (item.name == name1)
            //    {
            //        messagebox.show("this variable already exists", "", messageboxbuttons.ok, messageboxicon.error);

            //        int lastindex = textboxlist.count() - 1;

            //        return;
            //    }

            //    if (item.name.tolower() == name1.tolower())
            //    {
            //        messagebox.show("this variable already exists", "", messageboxbuttons.ok, messageboxicon.error);
            //        return;
            //    }
            //}

            if (name1.Length == 0)
            {
                MessageBox.Show("You didn't enter variable name");
                return;
            }

            if (double.TryParse (name1, out double output))
            {
                MessageBox.Show ("Variable names can't be numbers");
                return;
            }

            for (int i = 0; i < textboxlist.Count(); i++)
            {
                if (name1 == textboxlist[i].Name)
                {
                    MessageBox.Show("This variable already excists");
                    return;
                }
            }

            Controls.Add(
            new TextBox()
            {
                Location = new Point(70, y),

                Size = new Size(56, 22),

                Name = $"{name1}" // viktigt
            }
            );

           latestTextbox = GetControlByName(this.name1);
           textboxlist.Add (latestTextbox); // lägger till den nya textboxen i listan

            Controls.Add(
                new Label()
                {
                    Location = new System.Drawing.Point(16, y),
                    Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                    Text = $"{name1}",
                    Size = new System.Drawing.Size(52, 17),
                }
                );
            
            // new stuff
            Controls.Add(
                  new Label()
                  {
                      Location = new System.Drawing.Point(130, y),
                      Font = new System.Drawing.Font("Microsoft Sans Serif", 9.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                      Text = $"X",
                      Name = $"{name1}label",
                      ForeColor = Color.Red,
                      Size = new System.Drawing.Size(52, 17),
                  }
                  ) ;

            var latestlabel = GetControlByName($"{name1}label");
            labellist.Add(latestlabel);
            //latestlabel.Click += delegate { helloworld(name1, latestlabel.Name); };
          
            //new stuff

            y = y + 30;
            
            this.Height += 30;
        }

        List<Control> labellist = new List<Control>() { };
        void btnPrint_Click (object sender, EventArgs e)
        {
            var latesttextbox = GetControlByName($"{name1}");
            latestTextbox.Dispose();
        }

        private void removevar (string textname, string labelname)
        {
            bool booltext = false;

            var righttextbox = GetControlByName(textname);

            if (righttextbox != null) booltext = true;

            bool boollabel = false;

            var rightlabel = GetControlByName(labelname);

            if (rightlabel != null) boollabel = true;

            if (booltext && boollabel)
            {
                righttextbox.Dispose();
            }
        }

        object ConvertToAny (string input)
        {
            int i;

            if (int.TryParse(input, out i))
                return i;

            double d;

            if (double.TryParse(input, out d))
                return d;
  
            return input;
        }

        private void btnCalculate_Click (object sender, EventArgs e)
        {
            var list1 = textboxlist;

            string parameterlist = "";

            foreach (var item in list1)
            {

                parameterlist += $"double {item.Name.ToLower()}";

                if (list1.IndexOf(item) != list1.Count - 1)
                {
                    parameterlist += ",";
                }
            }

            string s = txtFormula.Text;

            string[] words = s.Split('^');

            double result = 0;

            if (words.Count() > 1)
            {
                result = Math.Pow (double.Parse(words[0]), double.Parse(words[1]));
            }



            //char[] charray = s.ToCharArray();

            foreach (var item in s)
            {
                char j = item;

                if (s.IndexOf(j) == s.Length - 1)
                {
                    if (!double.TryParse(j.ToString(), out double output))
                    {
                         break;
                    }

                   if (item != '0')
                   {
                        //txtFormula.Text += '.';
                        //txtFormula.Text += '0';
                   }
                }
            }

            var replace = result.ToString();
            
            for (int i = 0; i < words.Count(); i++)
            {

            }

            // Turn the equation into a function.
            string function_text = (result == 0) ?
            "public static class Evaluator" +
            "{" +                               //  (double x, double y)
            "    public static double Evaluate ( " + parameterlist + " )" +
            "    {" +          // 1 + x
            "        return " + txtFormula.Text + ";" +
            "    }" +
            "}"
            :
            "public static class Evaluator" +
            "{" +                               //  (double x, double y)
            "    public static double Evaluate ( " + parameterlist + " )" +
            "    {" +          // 1 + x
            "        return " + result + ";" +
            "    }" +
            "}";


            // Compile the function.

            CodeDomProvider code_provider = CodeDomProvider.CreateProvider("C#");

           // Generate a non-executable assembly in memory.
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            parameters.GenerateExecutable = false;
    

            // Compile the code.
            CompilerResults results = code_provider.CompileAssemblyFromSource (parameters, function_text);
            
                // If there are errors, display them.
                if (results.Errors.Count > 0)
                {
                    string msg = "Error compiling the expression.";

                    foreach (CompilerError compiler_error in results.Errors)
                    {
                         msg += "\n" + compiler_error.ErrorText;
                    }

                    MessageBox.Show (msg, "Expression Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                else
                {
                    // Get the Evaluator class type.
                    Type evaluator_type = results.CompiledAssembly.GetType("Evaluator");

                    // Get a MethodInfo object describing the Evaluate method.
                    MethodInfo method_info = evaluator_type.GetMethod("Evaluate");
                    // Make the parameter list.

                    //object[] method_params = new object[]
                    //{
                    //    double.Parse (txtX.Text),
                    //    double.Parse (txtY.Text),
                    //};

                    //  lista med argument som skickas som parametrar, som sedan görs om till en array
                    List <object> method_paramslist = new List<object>() { };

                    foreach (var item in list1)
                    {
                        if (item.Text.Length == 0)
                        {
                            item.Text = "0";
                        }

                        object element = 0;
                        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");

                        if (double.TryParse(item.Text, out double output))
                        {
                            //element = double.Parse (item.Text);
                            element = output;
                        }

                        else
                        {

                        }

                        method_paramslist.Add(element);
                    }

                object[] method_paramsarray = method_paramslist.ToArray();

                // Execute the method.
                double expression_result = (double) method_info.Invoke(null, method_paramsarray);

                // Display the returned result.
                txtResult.Text = expression_result.ToString();
            }
        }
    }
}