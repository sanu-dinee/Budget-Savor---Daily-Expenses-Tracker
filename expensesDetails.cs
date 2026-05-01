using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace budgetSavour
{
    public partial class expensesDetails : Form
    {
        string connectionString = "Server=localhost\\SQLEXPRESS;Database=dailyExpensesBudgetSaver;Trusted_Connection=True;";
        public expensesDetails()
        {
            InitializeComponent();
            LoadExpensesData();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            LoadExpensesData();
        }
        private void LoadExpensesData()
        {
            int accountNum = SessionManager.CurrentUserAccount;
            float amount = 0.00f;
            string category = "";
            string date = "";
            string description = "";
            NewExpenses showExpense = new NewExpenses(accountNum, amount, category, date, description);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT amount,category,expenseDate,expenseDescription FROM expenses WHERE accountNo=@accountNo";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {

                        cmd.Parameters.AddWithValue("@accountNo", showExpense.AccountNo);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {

                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {

                                    amount = Convert.ToSingle(reader["amount"]);
                                    category = reader["category"].ToString();
                                    date = reader["expenseDate"].ToString();
                                    description = reader["expenseDescription"].ToString();
                                    dataGridView1.Rows.Add(amount, category, date, description);
                                }
                            }
                            else
                            {
                                MessageBox.Show(this,"User Not Found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                        }

                    }

                }

            }
            catch (SqlException ex)
            {
                MessageBox.Show(this, "Database Error:" + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Unexpected error:" + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            expensesManagment ex = new expensesManagment();
            this.Hide();
            
            ex.ShowDialog();
            this.Close();
        }

        private void expensesDetails_Load(object sender, EventArgs e)
        {

        }
    }
    
}
