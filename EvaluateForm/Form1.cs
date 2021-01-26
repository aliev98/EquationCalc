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
//using System.Linq.Expressions;
using org.mariuszgromada.math.mxparser;

namespace EvaluateForm
{

    public class newvar
    {
        public Control aTextbox { get; set; }
        public Control aLabel { get; set; }
        public Control removeLabel { get; set; }
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<newvar> listofvars = new List<newvar>();

        int y = 88;

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
        Control latestLabel;
        Control latestrlabel;

        private void variable_add_Click (object sender, EventArgs e)
        {
            var aVar = new newvar();

            name1 = Microsoft.VisualBasic.Interaction.InputBox ("What should be the name of the variable", "Title", "", 170, 70);

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

            for (int i = 0; i < listofvars.Count(); i++)
            {
                if (name1 == listofvars[i].aTextbox.Name)
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
            
            aVar.aTextbox = latestTextbox;

           //textboxlist.Add (latestTextbox); // lägger till den nya textboxen i listan

            Controls.Add(
                new Label()
                {
                    Location = new System.Drawing.Point(16, y),
                    Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                    Text = $"{name1}",
                    Name = $"{name1}label",
                    Size = new System.Drawing.Size(52, 17),
                }
                );
            
            latestLabel = GetControlByName($"{name1}label");
            aVar.aLabel = latestLabel;

            // new stuff
            Controls.Add(
                  new Label()
                  {
                      Location = new System.Drawing.Point(130, y),
                      Font = new System.Drawing.Font("Microsoft Sans Serif", 9.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                      Text = "X",
                      Name = $"{name1}rlabel",
                      ForeColor = Color.Red,
                      Size = new System.Drawing.Size(52, 17),
                  }
                  ) ;

            latestrlabel = GetControlByName($"{name1}rlabel");
            //labellist.Add(latestlabel);
            aVar.removeLabel = latestrlabel;

            listofvars.Add(aVar);

            var currentvar = listofvars.Where (x => x.removeLabel.Name == name1 + "rlabel").FirstOrDefault();
            latestrlabel.Click += delegate { removevar (currentvar); };
          
            //new stuff

            y = y + 30;
            
            this.Height += 30;
        }


        private void removevar (newvar chosenvar)
        {
            chosenvar.aTextbox.Dispose();
            chosenvar.aLabel.Dispose();
            chosenvar.removeLabel.Dispose();

            var id = listofvars.IndexOf(chosenvar);
            var hello = listofvars.ElementAt(id);
            listofvars.Remove(hello);

            y = y - 30;

            this.Height -= 30;
        }


        //object ConvertToAny (string input)
        //{
        //    int i;

        //    if (int.TryParse(input, out i))
        //        return i;

        //    double d;

        //    if (double.TryParse(input, out d))
        //        return d;

        //    return input;
        //}

     
        private void btnCalculate_Click (object sender, EventArgs e)
        {

            var list1 = listofvars;

            string parameterlist = "";

            foreach (var item in list1)
            {
                parameterlist += $"double {item.aTextbox.Name.ToLower()}";

                if (list1.IndexOf(item) != list1.Count - 1)
                {
                    parameterlist += ",";
                }
            }

            if (string.IsNullOrEmpty(txtFormula.Text)) txtFormula.Text = "0";

            string s = txtFormula.Text.ToLower();


            string[] words = s.Split('^');
            double result = 0;

            if (words.Count() > 1)
            {

                string newExpression = "";
                string oldExpression = s;
                
                for (int i = 0; i < listofvars.Count(); i++)
                {
                    newExpression = oldExpression.Replace($"{listofvars[i].aTextbox.Name}", $"{listofvars[i].aTextbox.Text}");
                }

                var expression = new Expression(newExpression);
                result = expression.calculate();

            }


            // Turn the equation into a function.
            string function_text = (result == 0) ?
            "public static class Evaluator" +
            "{" +                               //  (double x, double y)
            "    public static double Evaluate ( " + parameterlist + " )" +
            "    {" +          // 1 + x
            "        return " + s + ";" +
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
            CodeDomProvider code_provider = CodeDomProvider.CreateProvider ("C#");

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
                        //if (item.aTextbox.Text.Length == 0)
                        //{
                        //    item.aTextbox.Text = "0";
                        //}

                        if (string.IsNullOrEmpty(item.aTextbox.Text)) item.aTextbox.Text = "0";

                        object element = 0;

                        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");

                        if (double.TryParse (item.aTextbox.Text, out double output))
                        {
                            //element = double.Parse (item.Text);
                            element = output;
                        }

                        else
                        {
                            
                        }

                        method_paramslist.Add (element);
                    }

                object[] method_paramsarray = method_paramslist.ToArray();

                // Execute the method.
                double expression_result = (double) method_info.Invoke (null, method_paramsarray);

                // Display the returned result.
                txtResult.Text = expression_result.ToString();
            }
        }
    }
}