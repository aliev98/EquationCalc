using System;
using System.Windows.Forms;

namespace FormulaWinF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string[] formulatext;

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click (object sender, EventArgs e)
        {
            formulatext = txtFormula.Text.Split();
            
            for (int i = 0; i < formulatext.Length; i++)
            {
                if (formulatext[i] == "varA".ToLower())
                {
                    formulatext[i] = varA.Text;
                }

                if (formulatext[i] == "varB".ToLower())
                {
                    formulatext[i] = varB.Text;
                }
            }
            
            string formulastring = String.Join (" ", formulatext);

            NCalc.Expression expr = new NCalc.Expression (formulastring);
            int result = (int)expr.Evaluate();
            string resultstring = result.ToString();

            MessageBox.Show(resultstring);

        }
    }
}
