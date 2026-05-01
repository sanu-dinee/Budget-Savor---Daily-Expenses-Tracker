using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace budgetSavour
{
    public partial class categorizedExpenses : Form
    {
        string connectionString = "Server=localhost\\SQLEXPRESS;Database=dailyExpensesBudgetSaver;Trusted_Connection=True;";

        //  stores data in pairs-class level variables
        private Dictionary<int, decimal> _currentMonthlyExpensesData;
        private Dictionary<int, decimal> _currentMonthlyDebtData;
        private Dictionary<string, decimal> _currentCategoryExpensesData;
        private string _currentReportOption;
        public categorizedExpenses()
        {
            InitializeComponent();
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);

            comboBox2.Visible = false;
            dateTimePicker1.Visible = false;
            
        }

        private void label4_Click(object sender, EventArgs e)
        {

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

        private void button5_Click(object sender, EventArgs e)
        {
            loginPage login = new loginPage();
            this.Hide();
            login.ShowDialog();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            monthlySummary summary = new monthlySummary();
            this.Hide();
            summary.ShowDialog();
            this.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {
           

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void categorizedExpenses_Load(object sender, EventArgs e)
        {
            chart1.Legends[0].Enabled = false;
            chart1.Size = new Size(497, 226);
            chart1.Anchor = AnchorStyles.None;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            chart1.Series["bar"].Points.Clear();
            label2.Text = string.Empty;
            label3.Text = string.Empty;

            string option = comboBox1.SelectedItem?.ToString();
            comboBox2.Visible = (option == "Expenses - for a month");
            dateTimePicker1.Visible = (option == "Expenses - for a day");

            if (comboBox2.Visible)
            {
                string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
                comboBox2.Items.Clear();
                comboBox2.Items.AddRange(months);
            }

            loadExpenses();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadExpenses();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            loadExpenses();
        }

        public void loadExpenses()
        {   

            int accountNum = SessionManager.CurrentUserAccount;
            string option = comboBox1.SelectedItem?.ToString().Trim();
            _currentReportOption = option;

            if (string.IsNullOrEmpty(option))
            {
                return;
            }

            chart1.Series["bar"].Points.Clear();


            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    if (option == "Monthly Expenses")
                    {
                        string[] monthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                        //  dictionary to store total expenses for each month (1 to 12) initialized to 0
                        var monthlyExpenses = new Dictionary<int, decimal>();

                       
                        for (int i = 1; i <= 12; i++)
                        {
                            monthlyExpenses[i] = 0.0m;
                        }

                        string query = @" SELECT MONTH(expenseDate) AS month,  SUM(amount) AS totalExpenses   FROM expenses  WHERE accountNo=@accountNo  GROUP BY MONTH(expenseDate)";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@accountNo", accountNum);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int month = Convert.ToInt32(reader["month"]);
                                    decimal totalExpenses = reader["totalExpenses"] != DBNull.Value
                                        ? Convert.ToDecimal(reader["totalExpenses"])
                                        : 0m;
                                    //updates the dictionary with the total expenses
                                    monthlyExpenses[month] = totalExpenses;
                                }
                            }
                        }


                        _currentMonthlyExpensesData = monthlyExpenses;
                        _currentMonthlyDebtData = null;
                        _currentCategoryExpensesData = null;

                        chart1.Series["bar"].Points.Clear();
                        foreach (var item in monthlyExpenses)
                        {
                            chart1.Series["bar"].Points.AddXY(monthNames[item.Key - 1], item.Value);
                        }

                        chart1.Legends[0].Enabled = true;
                        chart1.Series["bar"].LegendText = "Total Expenses";

                        label2.Text = "Months (Jan-Dec)";
                        label3.Text = "Total Amount";

                        chart1.ChartAreas[0].AxisY.LabelStyle.Format = "#,##0";
                        chart1.ChartAreas[0].AxisX.Interval = 1;
                        chart1.ChartAreas[0].AxisY.Minimum = 0;
                    }
                    else if (option == "Monthly Debt")
                    {
                        string[] monthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                        var monthlyDebt = new Dictionary<int, decimal>();
                        for (int i = 1; i <= 12; i++)
                        {
                            monthlyDebt[i] = 0.0m;
                        }

                        string query1 = "SELECT MONTH(lendDate) AS month, SUM(amount) AS totalDebt FROM debt WHERE accountNo=@accountNo GROUP BY MONTH(lendDate)";
                        using (SqlCommand cmd = new SqlCommand(query1, conn))
                        {
                            cmd.Parameters.AddWithValue("@accountNo", accountNum);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int month = Convert.ToInt32(reader["month"]);
                                    decimal totalDebt = Convert.ToDecimal(reader["totalDebt"]);
                                    monthlyDebt[month] = totalDebt;
                                }
                            }
                        }
                        _currentMonthlyDebtData = monthlyDebt;
                        _currentMonthlyExpensesData = null;
                        _currentCategoryExpensesData = null;
                        chart1.Series["bar"].Points.Clear();
                        foreach (var item in monthlyDebt)
                        {
                            chart1.Series["bar"].Points.AddXY(monthNames[item.Key - 1], item.Value);
                        }
                        chart1.Legends[0].Enabled = true;
                        chart1.Series["bar"].LegendText = "Total Debt";
                        label2.Text = "Months (Jan-Dec)";
                        label3.Text = "Total Amount";
                        chart1.ChartAreas[0].AxisY.LabelStyle.Format = "#,##0";
                        chart1.ChartAreas[0].AxisX.Interval = 1;
                    }
                    else if (option == "Expenses - for a month")
                    {
                        comboBox2.Text = "Select A Month....";
                        if (comboBox2.SelectedItem == null)
                            return;

                        string monthName = comboBox2.SelectedItem.ToString();

                        string[] allCategories = { "Food", "Grocery", "Transportation", "Entertainment", "Utilities", "Shopping", "Labour", "Rent", "Health" };
                        var categoryExpenses = new Dictionary<string, decimal>();
                        foreach (var cat in allCategories)
                        {
                            categoryExpenses[cat] = 0.0m;
                        }

                        string query2 = "SELECT category, SUM(amount) as TotalExpenses FROM expenses WHERE accountNo=@accountNo AND DATENAME(month, expenseDate)=@monthName GROUP BY category";
                        using (SqlCommand cmd = new SqlCommand(query2, conn))
                        {
                            cmd.Parameters.AddWithValue("@accountNo", accountNum);
                            cmd.Parameters.AddWithValue("@monthName", monthName);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        string category = reader["category"].ToString();
                                        decimal totalExpenses = Convert.ToDecimal(reader["TotalExpenses"]);
                                        if (categoryExpenses.ContainsKey(category))
                                        {
                                            categoryExpenses[category] = totalExpenses;
                                        }
                                    }
                                }
                                
                            }
                        }
                        _currentCategoryExpensesData = categoryExpenses;
                        _currentMonthlyExpensesData = null;
                        _currentMonthlyDebtData = null;
                        chart1.Series["bar"].Points.Clear();
                        foreach (var item in categoryExpenses)
                        {
                            chart1.Series["bar"].Points.AddXY(item.Key, item.Value);
                        }
                        chart1.Legends[0].Enabled = true;
                        chart1.Series["bar"].LegendText = "Total Expenses";
                        chart1.ChartAreas[0].AxisY.LabelStyle.Format = "#,##0";
                        label2.Text = "Expense Category";
                        label3.Text = "Amount";
                        chart1.ChartAreas[0].AxisX.Interval = 1;
                    }
                    else if (option == "Expenses - for a day")
                    {
                        DateTime selectedDate = dateTimePicker1.Value.Date;

                        string[] allCategories = { "Food", "Grocery", "Transportation", "Entertainment", "Utilities", "Shopping", "Labour", "Rent", "Health" };
                        var categoryExpenses = new Dictionary<string, decimal>();
                        foreach (var cat in allCategories)
                        {
                            categoryExpenses[cat] = 0.0m;
                        }

                        string query3 = "SELECT category, SUM(amount) as TotalExpenses FROM expenses WHERE accountNo=@accountNo AND expenseDate = @selectedDate GROUP BY category";
                        using (SqlCommand cmd = new SqlCommand(query3, conn))
                        {
                            cmd.Parameters.AddWithValue("@accountNo", accountNum);
                            cmd.Parameters.AddWithValue("@selectedDate", selectedDate);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        string category = reader["category"].ToString();
                                        decimal totalExpenses = Convert.ToDecimal(reader["TotalExpenses"]);
                                        if (categoryExpenses.ContainsKey(category))
                                        {
                                            categoryExpenses[category] = totalExpenses;
                                        }
                                    }
                                }
                               
                            }
                        }
                        //assign the data to the variables
                        _currentCategoryExpensesData = categoryExpenses;
                        _currentMonthlyExpensesData = null;
                        _currentMonthlyDebtData = null;
                        chart1.Series["bar"].Points.Clear();
                        foreach (var item in categoryExpenses)
                        {
                            chart1.Series["bar"].Points.AddXY(item.Key, item.Value);
                        }
                        chart1.Legends[0].Enabled = true;
                        chart1.Series["bar"].LegendText = "Total Expenses";
                        chart1.ChartAreas[0].AxisY.LabelStyle.Format = "#,##0";
                        label2.Text = "Expense Category";
                        label3.Text = "Amount";
                        chart1.ChartAreas[0].AxisX.Interval = 1;
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

        private void button2_Click(object sender, EventArgs e)
        {   
            int number = 0;
            string filePath = "";
            do
            {
                filePath = $@"E:\LNBTI\SE\year2sem1\Visual Application Programming\Project-Budget Tracker\budgetSavour\report{number}.txt";
                number++;
            } while (File.Exists(filePath));


            try
            {
                bool fileExists = File.Exists(filePath);

                using (StreamWriter sw = new StreamWriter(filePath, true))
                {
                    if (!fileExists)
                    {
                        sw.WriteLine("--- Daily Expenses & Budget Tracker Report ---");
                        sw.WriteLine();
                        sw.WriteLine($"Report Generated On: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        sw.WriteLine();
                        sw.WriteLine("--------------------------------------------");
                        sw.WriteLine();
                    }

                    string selectedReportOption = _currentReportOption;

                    sw.WriteLine($"Report Type: {selectedReportOption}");
                    sw.WriteLine();

                    if (selectedReportOption == "Monthly Expenses")
                    {
                        sw.WriteLine("--- Monthly Expenses Breakdown ---");
                        if (_currentMonthlyExpensesData != null)
                        {
                            string[] monthNames = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
                            foreach (var item in _currentMonthlyExpensesData)
                            {
                                if (item.Value > 0)
                                {
                                    sw.WriteLine($"{monthNames[item.Key - 1]}: Rs.{item.Value:N2}");
                                }
                            }
                        }
                        else
                        {
                            sw.WriteLine("No monthly expense data available to report.");
                        }
                        sw.WriteLine("----------------------------------");
                    }
                    else if (selectedReportOption == "Monthly Debt")
                    {
                        sw.WriteLine("--- Monthly Debt Breakdown ---");
                        if (_currentMonthlyDebtData != null)
                        {
                            string[] monthNames = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
                            foreach (var item in _currentMonthlyDebtData)
                            {
                                if (item.Value > 0)
                                {
                                    sw.WriteLine($"{monthNames[item.Key - 1]}: Rs.{item.Value:N2}");
                                }
                            }
                        }
                        else
                        {
                            sw.WriteLine("No monthly debt data available to report.");
                        }
                        sw.WriteLine("----------------------------------");
                    }
                    else if (selectedReportOption == "Expenses - for a month" || selectedReportOption == "Expenses - for a day")
                    {
                        string timeFrame = (selectedReportOption == "Expenses - for a month") ?
                                            "Month: " + (comboBox2.SelectedItem?.ToString() ?? "N/A") :
                                            "Day: " + dateTimePicker1.Value.ToShortDateString();

                        sw.WriteLine($"--- Category Expenses for {timeFrame} ---");
                        if (_currentCategoryExpensesData != null)
                        {
                            foreach (var item in _currentCategoryExpensesData)
                            {
                                if (item.Value > 0)
                                {
                                    sw.WriteLine($"{item.Key}: Rs.{item.Value:N2}");
                                }
                            }
                        }
                        else
                        {
                            sw.WriteLine("No category expense data available to report.");
                        }
                        sw.WriteLine("---------------------------------------");
                    }
                    else
                    {
                        sw.WriteLine("No specific chart data selected or available for reporting.");
                    }

                   
                }

                MessageBox.Show("Report Generated Successfully!", "Report Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (IOException ioEx)
            {
               
                MessageBox.Show(this, $"Error accessing file: {ioEx.Message}\nPlease ensure the file is not open and you have write permissions.",
                                "File Access Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }
            catch (Exception ex)
            {
               
                MessageBox.Show(this, $"An unexpected error occurred: {ex.Message}", "Application Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
               
            }
        }
    
       }
    
}