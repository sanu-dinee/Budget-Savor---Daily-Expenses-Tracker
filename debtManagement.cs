using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace budgetSavour
{
    public partial class debtManagement : Form
    {
        string connectionString = "Server=localhost\\SQLEXPRESS;Database=dailyExpensesBudgetSaver;Trusted_Connection=True;";
        public debtManagement()
        {
            InitializeComponent();
            LoadDebtDetails();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            dashboard db = new dashboard();
            this.Hide();
            db.ShowDialog();
            this.Close();
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            string status = "Pending";
            DateTime currentDateTime = DateTime.Now;
            string lender = textBox1.Text.Trim();


            try
            {
                DateTime lendDate = dateTimePicker1.Value;

                DateTime dueDate = dateTimePicker2.Value;

                int accountNo = SessionManager.CurrentUserAccount;



                if (!string.IsNullOrWhiteSpace(textBox3.Text))

                {

                    if (!float.TryParse(textBox3.Text.Trim(), out float newPayment))

                    {

                        MessageBox.Show(this, "Please enter a valid number for the paid amount.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        return;

                    }



                    float storedPaidAmount = 0.0f;
                    float samount = 0.0f;
                    using (SqlConnection conn = new SqlConnection(connectionString))

                    {

                        conn.Open();

                        Debt checkDebt = new Debt(accountNo, storedPaidAmount, lender, samount);
                        string queryDisplay = "SELECT amount, paidAmount FROM debt WHERE lenderName=@lName AND accountNo=@account";

                        using (SqlCommand cmd = new SqlCommand(queryDisplay, conn))

                        {

                            cmd.Parameters.AddWithValue("@account", checkDebt.AccountNo);

                            cmd.Parameters.AddWithValue("@lName", checkDebt.LenderName);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    if (reader["paidAmount"] != DBNull.Value)
                                    {
                                        storedPaidAmount = Convert.ToSingle(reader["paidAmount"]);
                                    }
                                    if (reader["amount"] != DBNull.Value)
                                    {
                                        samount = Convert.ToSingle(reader["amount"]);
                                    }
                                }
                            }

                            float newTotalPaidAmount = storedPaidAmount + newPayment;

                            if (newTotalPaidAmount == samount)

                            {

                                status = "Paid";

                            }

                            else
                            {

                                if (currentDateTime.Date > dueDate.Date)

                                {

                                    status = "Overdue";


                                }

                                else

                                {

                                    status = "Pending";

                                }
                            }

                            Debt UpdateDebt = new Debt(accountNo, newTotalPaidAmount, status, lender);


                            string queryUpdate = "UPDATE debt SET paidAmount=@paidAmount, status=@status WHERE lenderName=@lName AND accountNo=@account";

                            using (SqlCommand cmd1 = new SqlCommand(queryUpdate, conn))

                            {

                                cmd1.Parameters.AddWithValue("@paidAmount", UpdateDebt.PaidAmount);

                                cmd1.Parameters.AddWithValue("@status", UpdateDebt.Status);

                                cmd1.Parameters.AddWithValue("@lName", UpdateDebt.LenderName);

                                cmd1.Parameters.AddWithValue("@account", UpdateDebt.AccountNo);

                                cmd1.ExecuteNonQuery();



                                MessageBox.Show(this, "Debt updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            }
                        }
                    }
                }

                else

                {



                    using (SqlConnection conn = new SqlConnection(connectionString))



                    {
                        if (!float.TryParse(textBox2.Text.Trim(), out float amount))
                        {
                            MessageBox.Show(this, "Please enter a valid number for the lent amount.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        Debt debt = new Debt(lender, amount, lendDate.ToString("yyyy-MM-dd"), dueDate.ToString("yyyy-MM-dd"), status, accountNo);

                        conn.Open();

                        string query = "INSERT INTO debt(lenderName, amount, lendDate, dueDate, status, accountNo, paidAmount) VALUES(@lender, @amount, @lendDate, @dueDate, @status, @accountno, @paidAmount)";

                        using (SqlCommand cmd = new SqlCommand(query, conn))

                        {
                            cmd.Parameters.AddWithValue("@lender", debt.LenderName);
                            cmd.Parameters.AddWithValue("@amount", debt.Amount);
                            cmd.Parameters.AddWithValue("@lendDate", debt.LendDate);
                            cmd.Parameters.AddWithValue("@dueDate", debt.DueDate);
                            cmd.Parameters.AddWithValue("@status", debt.Status);
                            cmd.Parameters.AddWithValue("@accountno", debt.AccountNo);
                            cmd.Parameters.AddWithValue("@paidAmount", 0.0f);
                            cmd.ExecuteNonQuery();

                            MessageBox.Show(this, "Debt added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }

                ClearFormFields();
            }

            catch (SqlException ex)

            {
                MessageBox.Show(this, $"Database Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            catch (Exception ex)

            {

                MessageBox.Show(this, $"Unexpected error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }
        private void ClearFormFields()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            dateTimePicker1.Checked = false;
            dateTimePicker2.Checked = false;

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void LoadDebtDetails()
        {
            int accountNum = SessionManager.CurrentUserAccount;
            float amount = 0.00f;
            string dueDate = "";
            float paidAmount = 0.0f;
            string status = "";
            string lender = "";



            Debt DisplayDebt = new Debt(lender, amount, dueDate, status, paidAmount, accountNum);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT lenderName,amount,dueDate,status,paidAmount FROM debt WHERE accountNo=@accountNo";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {

                        cmd.Parameters.AddWithValue("@accountNo", DisplayDebt.AccountNo);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {

                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {

                                    amount = Convert.ToSingle(reader["amount"]);
                                    lender = reader["lenderName"].ToString();
                                    status = reader["status"].ToString();
                                    dueDate = reader["dueDate"].ToString();
                                    paidAmount = Convert.ToSingle(reader["paidAmount"]);
                                    float percentage = (amount > 0) ? (paidAmount / amount) * 100 : 0;



                                    dataGridView1.Rows.Add(lender, amount, dueDate, status, paidAmount);
                                }
                            }
                            else
                            {
                                MessageBox.Show(this, "User Not Found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void debtManagement_Load(object sender, EventArgs e)
        {

        }
    }
}

