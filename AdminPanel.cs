using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace budgetSavour
{
    public partial class AdminPanel : Form
    {
        string connectionString = "Server=localhost\\SQLEXPRESS;Database=dailyExpensesBudgetSaver;Trusted_Connection=True;";
        public AdminPanel()
        {  
            InitializeComponent();
            loadAdmin();
           
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void AdminPanel_Load(object sender, EventArgs e)
        {
              
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
        private void desginTable()
        {
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersVisible = false;
            dataGridView1.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ReadOnly = true;
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI Variable Text Semibold", 11, FontStyle.Bold);
            dataGridView1.DefaultCellStyle.ForeColor = Color.Teal;
            dataGridView1.BackgroundColor = SystemColors.GradientInactiveCaption;
            dataGridView1.DefaultCellStyle.BackColor = SystemColors.GradientInactiveCaption;

            dataGridView2.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.ColumnHeadersVisible = true;
            dataGridView2.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
            dataGridView2.ReadOnly = true;
            dataGridView2.BackgroundColor = SystemColors.InactiveCaption; ;
            dataGridView2.DefaultCellStyle.BackColor = SystemColors.InactiveCaption;
            dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.InactiveCaption;
            dataGridView2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.DefaultCellStyle.Font = new Font("Segoe UI Variable Text Semibold", 11, FontStyle.Bold);
            dataGridView2.DefaultCellStyle.ForeColor = Color.Teal;
            
            dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.Teal;
            dataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Variable Text Semibold", 10, FontStyle.Bold);
            dataGridView2.GridColor = SystemColors.InactiveCaption; ;
            dataGridView2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


        }
        public void loadAdmin()
        {
            DateTime currentMonth = DateTime.Now;
            string monthName = currentMonth.ToString("MMMM");


            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Amount", typeof(decimal));

            DataTable table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Budget", typeof(decimal));
            table.Columns.Add("Overbudget Amount", typeof(decimal));

            

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string queryOverdue = "SELECT   (a.firstName + ' ' + a.lastName) as name , SUM(d.amount - ISNULL(d.paidAmount, 0)) as debtAmount   FROM accountDetails AS a INNER JOIN debt AS d ON a.AccountNo = d.AccountNo   WHERE  d.status!='Paid'  GROUP BY d.accountNo ,a.firstName, a.lastName";

                    using (SqlCommand cmd = new SqlCommand(queryOverdue, conn))
                    {


                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    string name = reader["name"].ToString();
                                    decimal amount = 0.0m;
                                    if (reader["debtAmount"] != DBNull.Value)
                                    {
                                       
                                        amount = Convert.ToDecimal(reader["debtAmount"]);
                                    }
                                    dt.Rows.Add(name, amount);
                                    string comboBoxItem = $"{name} - {"Debt Overdue"}";

                                    comboBox1.Items.Add(comboBoxItem);
                                }
                            }
                        }
                    }
                    string query = "SELECT (a.firstName + ' ' + a.lastName) as name,  b.budget,  (SUM(e.amount) - b.budget) as overSpent FROM  accountDetails AS a INNER JOIN  expenses AS e ON a.AccountNo = e.accountNo INNER JOIN  BudgetIncome AS b ON a.AccountNo = b.accountNo WHERE  DATENAME(month, e.expenseDate) = @monthName AND b.budgetMonth = @monthName GROUP BY  a.AccountNo, a.firstName, a.lastName, b.budget HAVING  (SUM(e.amount) - b.budget) > 0;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@monthName", monthName);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    string name = reader["name"].ToString();
                                    decimal overSpentAmount = 0.0m;
                                    decimal budget = 0.0m;

                                    if (reader["budget"] != DBNull.Value)
                                    {
                                        budget = Convert.ToDecimal(reader["budget"]);
                                    }
                                    if (reader["overSpent"] != DBNull.Value)
                                    {
                                        overSpentAmount = Convert.ToDecimal(reader["overSpent"]);
                                    }

                                    table.Rows.Add(name,budget ,overSpentAmount);
                                    string comboBoxItem = $"{name} - {"Overbudget"}";

                                    comboBox1.Items.Add(comboBoxItem);
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(this, "Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Unexpected error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            dataGridView1.DataSource = dt;
            dataGridView2.DataSource = table;

            

            desginTable();
        }

        private void button4_Click(object sender, EventArgs e)
        {
             loginPage loginPage = new loginPage();
             this.Hide();
              loginPage.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Warning Send successful", "Send Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
