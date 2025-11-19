using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITP104_FINAL_PROJECT.Data;
using ITP104_FINAL_PROJECT.Models;

namespace ITP104_FINAL_PROJECT
{
    public partial class LoginScreen : Form
    {
        private readonly UserRepository userRepository;
        public static User CurrentUser { get; private set; }

        public LoginScreen()
        {
            InitializeComponent();

            guna2Panel1.FillColor = ColorTranslator.FromHtml("#647FBC");
            userRepository = new UserRepository();

            // Set password char for password textbox
            txtPassword.PasswordChar = '●';
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            // Validation
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please enter your username.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter your password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            // Disable login button during authentication
            btnLogin.Enabled = false;
            btnLogin.Text = "Logging in...";

            try
            {
                // Test database connection first
                bool isConnected = await DatabaseHelper.TestConnectionAsync();
                if (!isConnected)
                {
                    MessageBox.Show("Cannot connect to database. Please check your connection settings.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Authenticate user
                User user = await userRepository.AuthenticateAsync(username, password);

                if (user != null)
                {
                    // Store current user globally
                    CurrentUser = user;

                    MessageBox.Show($"Welcome back, {user.FullName}!", "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Open main dashboard
                    MainDashboard dashboard = new MainDashboard();
                    dashboard.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid username or password. Please try again.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during login:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable login button
                btnLogin.Enabled = true;
                btnLogin.Text = "Login";
            }
        }
    }
}
