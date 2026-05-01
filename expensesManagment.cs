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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace budgetSavour
{
    public partial class expensesManagment : Form
    {
        string connectionString = "Server=localhost\\SQLEXPRESS;Database=dailyExpensesBudgetSaver;Trusted_Connection=True;";
        public expensesManagment()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
           OpenFileDialog openFileDialog = new OpenFileDialog();
            // file types the dialog will show to the user
            openFileDialog.Filter= "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All Files|*.*";
            openFileDialog.Title = "Select an Image File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {   
                    Image tempImage=Image.FromFile(openFileDialog.FileName);//loads the path of the file
                    pictureBox1.Image = new Bitmap(tempImage);
                    tempImage.Dispose();
                    label10.Text = openFileDialog.FileName;
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error loading the image:"+ex,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dashboard db = new dashboard();
            this.Hide();
            db.ShowDialog();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            float amount = float.Parse(textBox1.Text.Trim());
            string expenseCategory =comboBox2.SelectedItem.ToString();
            DateTime selectedDate = dateTimePicker1.Value;
            string uploadReceipt = label10.Text;
            string expenseDescription = richTextBox1.Text.Trim();
            int accountNo = SessionManager.CurrentUserAccount;

            NewExpenses expenses = new NewExpenses(accountNo,amount,expenseCategory, selectedDate.ToString("yyyy-MM-dd"), expenseDescription,uploadReceipt);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO expenses(amount, category, uploadReceipt, expenseDate, expenseDescription, accountNo) VALUES(@amount, @expenseCategory, @uploadReceipt, @expenseDate, @expenseDescription, @accountNo)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {

                        cmd.Parameters.AddWithValue("@amount", expenses.amount);
                        cmd.Parameters.AddWithValue("@expenseCategory", expenses.expenseCategory);
                        cmd.Parameters.AddWithValue("@uploadReceipt", expenses.uploadReceipt);
                        cmd.Parameters.AddWithValue("@expenseDate", expenses.Date);
                        cmd.Parameters.AddWithValue("@expenseDescription", expenses.expenseDescription);
                        cmd.Parameters.AddWithValue("@accountNo", expenses.AccountNo);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show(this, "Expense addded Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearFormFields();

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

        private void ClearFormFields()
        {
            textBox1.Clear();
            label10.Text = "";
            textBox2.Clear();
            textBox3.Clear();
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            dateTimePicker1.Checked = false;
            richTextBox1.Clear();
            pictureBox1.Image = null;

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            expensesDetails expensesDetails = new expensesDetails();
            this.Hide();
           expensesDetails.ShowDialog();
            this.Close();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            float budget = float.Parse(textBox2.Text.Trim());
            float income=float.Parse(textBox3.Text.Trim());
            string month = comboBox1.SelectedItem.ToString();
            int accountNo = SessionManager.CurrentUserAccount;

            Income_Budget Budget = new Income_Budget(budget,income,month,accountNo);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string checkQuery = "SELECT COUNT(*) FROM BudgetIncome WHERE AccountNo = @accountNo AND budgetMonth = @monthName";

                    string query = "INSERT INTO BudgetIncome(budget, income,budgetMonth,accountNo ) VALUES(@budget,@income,@month, @accountNo)";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@accountNo", Budget.AccountNo);
                        checkCmd.Parameters.AddWithValue("@monthName", Budget.Month);

                        int existingCount = (int)checkCmd.ExecuteScalar(); 

                        if (existingCount > 0)
                        {
                            MessageBox.Show(this, "A budget has already been entered for the current month.", "Budget Already Exists", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ClearFormFields();
                            return; 
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {

                        cmd.Parameters.AddWithValue("@budget",Budget.Budget );
                        cmd.Parameters.AddWithValue("@income", Budget.Income);
                        cmd.Parameters.AddWithValue("@month", Budget.Month);
                        cmd.Parameters.AddWithValue("@accountNo", Budget.AccountNo);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show(this, "Budget & Income set Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearFormFields();

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

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void expensesManagment_Load(object sender, EventArgs e)
        {

        }
    }

}
