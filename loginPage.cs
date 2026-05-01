using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace budgetSavour
{
    public partial class loginPage : Form
    {
        string connectionString = "Server=localhost\\SQLEXPRESS;Database=dailyExpensesBudgetSaver;Trusted_Connection=True;";
        public loginPage()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }    

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string email = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();
            string storedSalt = "";
            string storedPassword = "";
            int accountNo = 0;
            string adminPassword = "";

            appUser loggedUser = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string adminQuery = "SELECT uPassword FROM admin WHERE email = @Email";
                    using (SqlCommand cmd = new SqlCommand(adminQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                               
                                adminPassword = reader["uPassword"].ToString();

                                if (adminPassword == password)
                                {
                                    MessageBox.Show(this, "Admin login successful!", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    AdminPanel adminMenu = new AdminPanel();
                                    this.Hide();
                                    adminMenu.Show();
                                  
                                }
                                else
                                {
                                   
                                    MessageBox.Show(this, "Invalid password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    textBox2.Clear();
                                    return; 
                                }
                            }
                        }
                    }


                    string query = "select accountNo, passwordHash,salt from accountDetails WHERE email = @Email";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                storedPassword = reader["passwordHash"].ToString();
                                storedSalt = reader["salt"].ToString();
                                if (reader["accountNo"] != DBNull.Value)
                                {
                                    accountNo = Convert.ToInt32(reader["accountNo"]);
                                }

                                loggedUser = new appUser(accountNo,storedPassword,storedSalt,email);
                            }
                            else
                            {
                                MessageBox.Show(this, "Email Not Found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                textBox1.Clear();
                                return;
                            }
                            
                        }

                    }
                    
                }
               

                if (loggedUser != null)
                {

                    if (PasswordHasher.verifyPassword(password, storedPassword, storedSalt))
                    {
                        SessionManager.CurrentUserAccount = accountNo;
                        MessageBox.Show(this, "Login Successful", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        dashboard menu = new dashboard();
                        this.Hide();
                        menu.Show();
                       

                    }
                    else
                    {
                        MessageBox.Show(this, "Incorrect password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox2.Clear();

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
            signUp register = new signUp();
            this.Hide();
             register.ShowDialog();
          



        }

        private void button3_Click(object sender, EventArgs e)
        {   
           
            this.BackColor = Color.Black;
          
            
            familyPin familypin = new familyPin();

            DialogResult result = familypin.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                
                dashboard menu= new dashboard();
                this.Hide();
                menu.ShowDialog();
                this.Close();

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

        private void label4_Click(object sender, EventArgs e)
        {
             forgotPassword forgotPassword = new forgotPassword();
            this.Hide();
            forgotPassword.ShowDialog();
        }
    }
}
