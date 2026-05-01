using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace budgetSavour
{
    public partial class forgotPassword : Form
    {
        string connectionString = "Server=localhost\\SQLEXPRESS;Database=dailyExpensesBudgetSaver;Trusted_Connection=True;";
        public forgotPassword()
        {
            InitializeComponent();
        }

        private void forgotPassword_Load(object sender, EventArgs e)
        {

        }
        private bool IsEmailValid(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, emailPattern);
        }

        private bool IsValidPassword(string password)
        {
            string validPassword = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            return Regex.IsMatch(password, validPassword);
        }

        

        private void button1_Click(object sender, EventArgs e)
        {

            string Email = textBox1.Text;
            string Password = textBox2.Text.Trim();
           


            if ( string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) )
            {
                MessageBox.Show(this, "Please fill in all fields", " Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsEmailValid(Email))
            {
                MessageBox.Show(this, "Please enter a valid email address.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
                return;
            }
            

            if (!IsValidPassword(Password))

            {
                MessageBox.Show(this, "Password must be at least 8 characters long and \n include at least one uppercase letter, one lowercase letter,\n one number, and one special character (@$!%*?&).", "Invalid Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2.Clear();
                textBox5.Clear();
              
                return;
            }


            if (textBox2.Text != textBox5.Text)
            {
                MessageBox.Show(this, "Password mismatch", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2.Clear();
                textBox5.Clear();
                
                return;
            }


            string salt;
            string hashedPassword = PasswordHasher.HashPassword(Password, out salt);

            


            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE accountDetails SET passwordHash=@hashedPassword, salt=@salt WHERE email=@email";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@hashedPassword", hashedPassword);
                        cmd.Parameters.AddWithValue("@salt", salt);
                        cmd.Parameters.AddWithValue("@email", Email);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show(this, "Password Updated Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearFormFields();

                            this.Hide();
                            loginPage login = new loginPage();
                            login.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show(this, "No account found with that email.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void button2_Click(object sender, EventArgs e)
        {
            loginPage loginPage = new loginPage();
            this.Hide();
            loginPage.ShowDialog();
        }

        public void ClearFormFields()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox5.Clear();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            if (textBox2.UseSystemPasswordChar == true)
            {
                textBox2.UseSystemPasswordChar = false;
            }
            else
            {
                textBox2.UseSystemPasswordChar = true;
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            if (textBox5.UseSystemPasswordChar == true)
            {
                textBox5.UseSystemPasswordChar = false;
            }
            else
            {
                textBox5.UseSystemPasswordChar = true;
            }
        }
    }
}
