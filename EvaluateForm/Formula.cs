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
// using System.Linq.Expressions;
using org.mariuszgromada.math.mxparser;

namespace EvaluateForm
{

    public partial class Formula : Form
    {
        public Formula()
        {
            InitializeComponent();
        }

        List <FormulaVar> VariableList = new List<FormulaVar>();
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

            MaximizeBox = false;

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
            var newVar = new FormulaVar();

            name1 = Microsoft.VisualBasic.Interaction.InputBox("What should be the name of the variable?", "Title", "", 170, 70);

            if (name1.Length == 0)
            {
                MessageBox.Show("You didn't enter variable name");
                return;
            }
            else if(name1.Length > 1)
            {
                MessageBox.Show("Variable names can't be longer than one letter");
                return;
            }
            else if (char.IsUpper(Convert.ToChar(name1)))
            {
                MessageBox.Show("Use lowercase on variable names");
                return;
            }
            
            else if (double.TryParse(name1, out double output))
            {
                MessageBox.Show("Variable names can't be numbers");
                return;
            }

            //Check if name already excists
            for (int i = 0; i < VariableList.Count(); i++)
            {
                if (name1 == VariableList[i].aTextbox.Name)
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

                Name = $"{name1}"
            }
            );

            latestTextbox = GetControlByName(this.name1);

            newVar.aTextbox = latestTextbox;

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
            newVar.aLabel = latestLabel;

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
                  );

            latestrlabel = GetControlByName($"{name1}rlabel");

            newVar.removeLabel = latestrlabel;

            VariableList.Add(newVar);

            var currentvar = VariableList.Where(x => x.removeLabel.Name == name1 + "rlabel").FirstOrDefault();
            latestrlabel.Click += delegate { removevar(currentvar); };
            y = y + 30;
            this.Height += 30;
        }


        private void removevar (FormulaVar chosenvar)
        {
            chosenvar.aTextbox.Dispose();
            chosenvar.aLabel.Dispose();
            chosenvar.removeLabel.Dispose();


            var id = VariableList.IndexOf(chosenvar);
            var Variable = VariableList.ElementAt(id);
            VariableList.Remove(Variable);

            for (int i = id; i < VariableList.Count(); i++)
            {
                var variable = VariableList[i];
                variable.aTextbox.Location = new Point(variable.aTextbox.Location.X, variable.aTextbox.Location.Y - 30);
                variable.aLabel.Location = new Point(variable.aLabel.Location.X, variable.aLabel.Location.Y - 30);
                variable.removeLabel.Location = new Point(variable.removeLabel.Location.X, variable.removeLabel.Location.Y - 30);
            }

            y = y - 30;
            this.Height -= 30;
        }

        private void btnCalculate_Click (object sender, EventArgs e)
        {

            string parameterlist = "";

            foreach (var item in VariableList)
            {
                parameterlist += $"double {item.aTextbox.Name.ToLower()}";

                if (VariableList.IndexOf(item) != VariableList.Count - 1)
                {
                    parameterlist += ",";
                }
            }

            if (string.IsNullOrEmpty(txtFormula.Text)) txtFormula.Text = "0";

            string formula = txtFormula.Text.ToLower();

            string tempExpression = formula;
            double result;

            //replacing variables with their values
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");

            foreach (var variable in VariableList)
            {
                var variableName = variable.aTextbox.Name;
                var variableValue = variable.aTextbox.Text;

                if (string.IsNullOrEmpty(variableValue)) variableValue = "0";

                if (tempExpression.Contains(variableName))
                {
                    tempExpression = tempExpression.Replace(variableName, variableValue);
                }
            }

            if (tempExpression.Contains("pi"))
            {
                double mathpi = Math.PI;

                tempExpression =  tempExpression.Replace("pi", $"{mathpi}");
            }

            var expression = new Expression (tempExpression);
            result = expression.calculate();

            //Turn the equation into a function.
            string function_text =
            "public static class Evaluator" +
            "{" +                               //  (double x, double y)
            "    public static double Evaluate ( " + parameterlist + " )" +
            "    {" +          // 1 + x
            "        return " + result + ";"+
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

                MessageBox.Show(msg, "Expression Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else
            {
                // Get the Evaluator class type.
                Type evaluator_type = results.CompiledAssembly.GetType("Evaluator");

                // Get a MethodInfo object describing the Evaluate method.
                MethodInfo method_info = evaluator_type.GetMethod("Evaluate");
                
                // Make the parameter list.
                List <object> method_paramslist = new List <object>() { };

                foreach (var item in VariableList)
                {
                    if (string.IsNullOrEmpty(item.aTextbox.Text)) item.aTextbox.Text = "0";

                    object element = 0;

                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");

                    if (double.TryParse(item.aTextbox.Text, out double output))
                    {
                        element = output;
                    }

                    method_paramslist.Add(element);
                }

                object[] method_paramsarray = method_paramslist.ToArray();

                // Execute the method.
                double expression_result = (double)method_info.Invoke(null, method_paramsarray);

                // Display the returned result.
                txtResult.Text = expression_result.ToString();
            }
        }
    }
}