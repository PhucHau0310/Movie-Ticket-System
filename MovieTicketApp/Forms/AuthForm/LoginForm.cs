using MovieTicketApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieTicketApp.Forms
{
    public partial class LoginForm : Form
    {
        private readonly UserService _userService;
        public LoginForm()
        {
            InitializeComponent();

            _userService = new UserService();
            // Set default password char
            passwordTxtBox.PasswordChar = '•';
        }

        private void exitLoginFormBtn_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Bạn chắc chắn muốn thoát chương trình?",
                "Xác nhận thoát",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void showPassCkb_CheckedChanged(object sender, EventArgs e)
        {
            passwordTxtBox.PasswordChar = showPassCkb.Checked ? '\0' : '•';
        }

        private async void loginBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usernameTxtBox.Text) ||
                    string.IsNullOrWhiteSpace(passwordTxtBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu.",
                        "Lỗi đăng nhập",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                loginBtn.Enabled = false;
                loginBtn.Text = "Đang đăng nhập...";
                Cursor = Cursors.WaitCursor;

                var user = await _userService.LoginAsync(
                    usernameTxtBox.Text.Trim(),
                    passwordTxtBox.Text);

                Program.CurrentUser = user;

                MessageBox.Show($"Chào mừng {user.Username}!",
                "Đăng nhập thành công",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

                var adminForm = new AdminForm();
                var customerForm = new MovieTicketApp.Forms.CustomerForm.CustomerForm();

                if (user.Role == "admin" || user.Role == "staff")
                {
                    this.Hide();
                    adminForm.Show();
                }
                else if (user.Role == "customer")
                {
                    this.Hide();
                    customerForm.Show();
                }
                else
                {
                    MessageBox.Show("Bạn không có quyền truy cập vào hệ thống.",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login failed: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                loginBtn.Enabled = true;
                loginBtn.Text = "Login";
                Cursor = Cursors.Default;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                var result = MessageBox.Show(
                    "Bạn chắc chắn muốn thoát chương trình?",
                    "Xác nhận thoát",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
                else
                {
                    Application.Exit();
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Hide();
            var registerForm = new RegisterForm();
            registerForm.Show();
        }
    }
}
