using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq.Expressions;
using Mathematical.Expressions;

namespace DynamicFormula
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load (object sender, EventArgs e)
        {
            List<MyMath> list = new List<MyMath>();
            list.Add(new MyMath() { a = 2.1, b = 3, c = 4.5 });
            list.Add(new MyMath() { a = 3.1, b = 2, c = 1.5 });
            list.Add(new MyMath() { a = 1.1, b = 1, c = 5.5 });
            dataGridView1.DataSource = list;
        }

        private void dataGridView1_CellContentClick (object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnCalculate_Click (object sender, EventArgs e)
        {
            List <MyMath> list = dataGridView1.DataSource as List<MyMath>;

            if (list != null)
            {
                foreach (MyMath math in list)
                {
                    ExpressionEval eval = new ExpressionEval(math.formula);
                    eval.SetVariable("a", math.a);
                    eval.SetVariable("b", math.b);
                    eval.SetVariable("c", math.c);
                    
                    math.result = (double)eval.Evaluate();
                }

                dataGridView1.DataSource = list;
                dataGridView1.Refresh();
            }
        }
    }
}
