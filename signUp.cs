using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Text.RegularExpressions;
using System.Drawing.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace budgetSavour
{
    public partial class signUp : Form
    {
        string connectionString = "Server=localhost\\SQLEXPRESS;Database=dailyExpensesBudgetSaver;Trusted_Connection=True;";
        public signUp()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            textBox2.UseSystemPasswordChar = true;
            textBox5.UseSystemPasswordChar = true;
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {
            
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private string GetSelectedAccountType()
        {
            if (Individual.Checked)
            {
                return "Standard";
            }
            else if(radioButton1.Checked)
            {
                return "Family";
            }
            return string.Empty;
        }
        private bool IsEmailValid(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email,emailPattern);
        }

        private bool IsValidPassword(string password)
        {
            string validPassword = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            return Regex.IsMatch(password, validPassword);
        }

        private bool IsEmailAlreadyRegistered(string Email)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "select count(*) from accountDetails WHERE email = @Email";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@email", Email);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;


                }
                
                
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {


            //input validation
            string firstName = textBox4.Text.Trim();
            string lastName = textBox3.Text.Trim();
            string Email = textBox1.Text.Trim();
            string Password = textBox2.Text.Trim();
            string ContactNo = maskedTextBox1.Text.Trim();
            string AccountType = GetSelectedAccountType();
            


            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(AccountType))
            {
                MessageBox.Show(this,"Please fill in all fields", " Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

           if (!IsEmailValid(Email))
            {
                MessageBox.Show(this, "Please enter a valid email address.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
                return;
            }
            if (IsEmailAlreadyRegistered(Email))
            {
                MessageBox.Show(this, "An account with this email address already exists. Please use a different email", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            appUser newUser;
            int familyPin = 0;

            if (AccountType == "Family")
            {
                //generates a random pin
                Random random = new Random();
                familyPin = random.Next(0001, 10000);

                newUser = new familyAccountUser(firstName, lastName, Email, hashedPassword, AccountType, ContactNo, familyPin,salt);
            }
            else
            {

                newUser = new appUser(firstName, lastName, Email, hashedPassword, AccountType, ContactNo,salt);


            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO accountDetails(firstName,lastName,email,passwordHash,accountType,familyPin,contactNo,salt)VALUES(@firstName, @lastName, @Email, @hashedPassword, @AccountType, @familyPin, @ContactNo,@salt)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@firstName", newUser.FName);
                        cmd.Parameters.AddWithValue("@lastName", newUser.LName);
                        cmd.Parameters.AddWithValue("@email", newUser.Email);
                        cmd.Parameters.AddWithValue("@hashedPassword", hashedPassword);
                        cmd.Parameters.AddWithValue("@salt", newUser.Salt);
                        cmd.Parameters.AddWithValue("@accountType", newUser.AccountType);
                        


                        if (newUser is familyAccountUser familyUser)
                        {
                            cmd.Parameters.AddWithValue("@familyPin", familyUser.FamilyPin);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@familyPin", DBNull.Value);
                        }

                        cmd.Parameters.AddWithValue("@contactNo", newUser.ContactNo);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show(this, "Account Created Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearFormFields();

                    }
                    conn.Close();
                }
                loginPage login = new loginPage();
                this.Hide();

                login.ShowDialog();
                this.Close();


            }
            catch (SqlException ex)
            {
                MessageBox.Show(this,"Database Error:"+ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,"Unexpected error:"+ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private void label9_Click(object sender, EventArgs e)
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

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
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
        private void ClearFormFields()
        {
            textBox1.Clear();       
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            maskedTextBox1.Clear();
            radioButton1.Checked = false;
                
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            loginPage loginPage = new loginPage();
            this.Hide();
            loginPage.ShowDialog();
            this.Close();
        }
    }
}
