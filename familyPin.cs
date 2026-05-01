using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Data.SqlClient;
using System.Security.Principal;

namespace budgetSavour
{
    public partial class familyPin : Form
    {
        string connectionString = "Server=localhost\\SQLEXPRESS;Database=dailyExpensesBudgetSaver;Trusted_Connection=True;";

       
       
        public familyPin()
        {
            InitializeComponent();
            label1.Text = string.Empty;

            button1.Click += NUmberButton_Click;
            button2.Click += NUmberButton_Click;
            button3.Click += NUmberButton_Click;
            button4.Click += NUmberButton_Click;
            button5.Click += NUmberButton_Click;
            button6.Click += NUmberButton_Click;
            button7.Click += NUmberButton_Click;
            button8.Click += NUmberButton_Click;
            button9.Click += NUmberButton_Click;
            button12.Click += NUmberButton_Click;

            

        }
        
        

        private void familyPin_Load(object sender, EventArgs e)
        {
            label1.Text = string.Empty;
        }

       

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void label1_Click(object sender, EventArgs e)
        {
           
        }
        private void NUmberButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button clickedButton = (System.Windows.Forms.Button)sender;

            if (label1.Text.Length < 4)
            {
                label1.Text += clickedButton.Text;
            }

        }


        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                if (label1.Text.Length == 4)


                {
                    int familyPin = int.Parse(label1.Text);
                    int accountNum = 0;

                    familyAccountUser familyUser = new familyAccountUser(accountNum, familyPin);


                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "select accountNo from accountDetails WHERE familyPin = @familyPin";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@familyPin", familyUser.FamilyPin);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    accountNum = Convert.ToInt32(reader["accountNo"]);
                                  
                                }
                                else
                                {
                                    MessageBox.Show(this, "Family Pin Not Found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                            }

                        }
                        
                    }
                    SessionManager.CurrentUserAccount = accountNum;
                    MessageBox.Show(this,"Login Successful", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();

                }
                else
                {
                    MessageBox.Show("Please enter the 4-digit Family Pin");
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
            

        private void button10_Click(object sender, EventArgs e)
        {
            if (label1.Text.Length > 0)
            {
                label1.Text = label1.Text.Substring(0, label1.Text.Length - 1);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button9_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button12_Click(object sender, EventArgs e)
        {
             
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        private void button5_Click(object sender, EventArgs e)
        {
           
        }

        private void button8_Click(object sender, EventArgs e)
        {
           
        }

        private void button7_Click(object sender, EventArgs e)
        {
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
        }

        private void button13_Click(object sender, EventArgs e)
        {
            loginPage login = new loginPage();
            this.Hide();
            login.ShowDialog();
            
        }
    }
}
