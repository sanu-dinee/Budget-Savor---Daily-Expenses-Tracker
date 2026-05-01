using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient; 
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace budgetSavour
{
    public partial class monthlySummary : Form
    {
        string connectionString = "Server=localhost\\SQLEXPRESS;Database=dailyExpensesBudgetSaver;Trusted_Connection=True;";
        //change the color state of the progress bar by sending a Windows message via SendMessage()
        private const int PBM_SETSTATE = 1040;
        private const int PBST_NORMAL = 1;
        private const int PBST_ERROR = 2;
        private const int PBST_PAUSED = 3;

        //Allows calling the Windows API SendMessage function to manipulate native controls
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);

        public monthlySummary()
        {
            InitializeComponent();
            
            
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            loginPage login = new loginPage();
            this.Hide();
            login.ShowDialog();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            expensesManagment expense = new expensesManagment();
            this.Hide();
            expense.ShowDialog();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            debtManagement debts = new debtManagement();
            this.Hide();
            debts.ShowDialog();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            categorizedExpenses cExpense = new categorizedExpenses();
            this.Hide();
            cExpense.ShowDialog();
            this.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void monthlySummary_Load(object sender, EventArgs e)
        {
            comboBox1.Text = "Select a Month......";
            
        }

        private void LoadSummary(){

            
            int accountNum = SessionManager.CurrentUserAccount;
            float totalBudget = 0.0f;
            float totalExpenses = 0.0f;
            float totalDebt = 0.0f;
            float budgetPercentage = 0.0f;
            string monthName = comboBox1.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(monthName))
            {
                MessageBox.Show(this, "Please select a month.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int monthNumber;
            try
            {
                monthNumber = DateTime.ParseExact(monthName, "MMMM", System.Globalization.CultureInfo.CurrentCulture).Month;
            }
            catch
            {
                MessageBox.Show(this, "Invalid month selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string queryBudget = "SELECT b.budget as Budget, SUM(e.amount) as TotalExpenses FROM BudgetIncome AS b LEFT JOIN expenses AS e ON b.AccountNo = e.AccountNo AND MONTH(e.expenseDate) = @monthNumber WHERE b.accountNo = @accountNo AND b.budgetMonth = @month GROUP BY b.budget";

                    using (SqlCommand cmd = new SqlCommand(queryBudget, conn))
                    {
                        cmd.Parameters.AddWithValue("@accountNo", accountNum);
                        cmd.Parameters.AddWithValue("@month", monthName); 
                        cmd.Parameters.AddWithValue("@monthNumber", monthNumber); 

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader["Budget"] != DBNull.Value)
                                {
                                    totalBudget = Convert.ToSingle(reader["Budget"]);
                                }
                                if (reader["TotalExpenses"] != DBNull.Value)
                                {
                                    totalExpenses = Convert.ToSingle(reader["TotalExpenses"]);
                                }
                            }
                        }
                    }

                   
                    string queryDebt = "SELECT SUM(amount - ISNULL(paidAmount, 0)) as debt FROM debt WHERE accountNo=@accountNo AND MONTH(dueDate)=@monthNumber AND status!='Paid' ";
                    using (SqlCommand cmd = new SqlCommand(queryDebt, conn))
                    {
                        cmd.Parameters.AddWithValue("@accountNo", accountNum);
                        cmd.Parameters.AddWithValue("@monthNumber", monthNumber);
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            totalDebt = Convert.ToSingle(result);
                        }
                    }

                    
                    string queryCategories = "SELECT category, SUM(amount) as TotalExpenses FROM expenses WHERE accountNo=@accountNo AND MONTH(expenseDate)=@monthNumber GROUP BY category";


                    chart1.Series["pie"].Points.Clear();
                    chart1.Series["pie"].IsValueShownAsLabel = true;
                    chart1.Series["pie"].Label = "#PERCENT{P0}";

                    StringBuilder categorizedOutput = new StringBuilder();
                    using (SqlCommand cmd = new SqlCommand(queryCategories, conn))
                    {
                        cmd.Parameters.AddWithValue("@accountNo", accountNum);
                        cmd.Parameters.AddWithValue("@monthNumber", monthNumber);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    string category = reader["category"].ToString();
                                    float cAmount = Convert.ToSingle(reader["TotalExpenses"]);
                                    categorizedOutput.AppendLine($"{category} - Rs.{cAmount}");


                                    var pointIndex = chart1.Series["pie"].Points.AddXY(category, cAmount);
                                    chart1.Series["pie"].Points[pointIndex].LegendText = category;
                                }
                            }
                           
                        }
                    }

                    
                    if (totalBudget > 0)
                    {
                        budgetPercentage = (totalExpenses / totalBudget) * 100;
                    }
                    else
                    {
                        budgetPercentage = 0;
                    }

                    label9.Text = $"{budgetPercentage:F2}%";
                    label8.Text = categorizedOutput.ToString();
                    if (totalExpenses > 0.0f && totalBudget > 0.0f)
                    {
                        label3.Text = $"Expenses   Rs.{totalExpenses}\n    (Budget - Rs.{totalBudget}) ";
                    }
                    else
                    {
                        if (totalExpenses > 0.0f)
                        {
                            label3.Text = $"Expenses   Rs.{totalExpenses}";
                        }
                        else if (totalBudget>0.0f)
                        {
                            label3.Text = $" (Budget - Rs.{totalBudget}) ";
                        }
                        
                    }

                    if (totalDebt > 0)
                    {
                        label6.Text = $"Debt         Rs.{totalDebt} ";
                    }
                    if (budgetPercentage > 100)
                    {
                        progressBar2.Value = 100;
                        SendMessage(progressBar2.Handle, PBM_SETSTATE, (IntPtr)PBST_ERROR, IntPtr.Zero);
                        label9.ForeColor = Color.Red;
                    }
                    else
                    {
                        progressBar2.Value = (int)budgetPercentage;
                        label9.ForeColor = Color.ForestGreen;
                        SendMessage(progressBar2.Handle, PBM_SETSTATE, (IntPtr)PBST_NORMAL, IntPtr.Zero);
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
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label3.Text = "";
            label6.Text = "";
            LoadSummary();

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        private void pieChart()
        {
            



           

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
