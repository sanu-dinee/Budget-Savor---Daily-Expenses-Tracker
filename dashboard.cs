using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace budgetSavour
{
    public partial class dashboard : Form
    {
        public dashboard()
        {
            InitializeComponent();
        }

        private void dashboard_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            loginPage login = new loginPage();
            this.Hide();
            login.ShowDialog();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            expensesManagment expenses = new expensesManagment();
            this.Hide();
            expenses.ShowDialog();
            this.Close();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            debtManagement debt = new debtManagement();
            this.Hide();
            debt.ShowDialog();

            this.Close();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            monthlySummary monSummary = new monthlySummary();
            this.Hide();
            monSummary.ShowDialog();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            categorizedExpenses cExpenses = new categorizedExpenses();
            this.Hide();
            cExpenses.ShowDialog();
            this.Close();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
