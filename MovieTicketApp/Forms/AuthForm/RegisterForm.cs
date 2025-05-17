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
    public partial class RegisterForm : Form
    {
        private readonly UserService _userService;
        public RegisterForm()
        {
            InitializeComponent();

            _userService = new UserService();

            // Set password char for password textbox
            passwordTxtBox.PasswordChar = '•';
            confirmPassTxt.PasswordChar = '•';
        }

        private void showPassCkb_CheckedChanged(object sender, EventArgs e)
        {
            passwordTxtBox.PasswordChar = showPassCkb.Checked ? '\0' : '•';
            confirmPassTxt.PasswordChar = showPassCkb.Checked ? '\0' : '•';
        }

        private async void loginBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(usernameTxtBox.Text) ||
                    string.IsNullOrWhiteSpace(passwordTxtBox.Text)
                    )
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin!",
                        "Lỗi đăng ký",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (passwordTxtBox.Text != confirmPassTxt.Text)
                {
                    MessageBox.Show("Mật khẩu xác nhận không khớp!",
                        "Lỗi đăng ký",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Show loading state
                loginBtn.Enabled = false;
                loginBtn.Text = "Đang đăng ký...";
                Cursor = Cursors.WaitCursor;

                // Call register service
                var user = await _userService.RegisterAsync(
                    usernameTxtBox.Text.Trim(),
                    passwordTxtBox.Text,
                    "customer"
                );

                MessageBox.Show("Đăng ký thành công! Vui lòng đăng nhập.",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Close register form and show login form
                var loginForm = new LoginForm();
                this.Hide();
                loginForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đăng ký thất bại: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                loginBtn.Enabled = true;
                loginBtn.Text = "ĐĂNG KÝ";
                Cursor = Cursors.Default;
            }
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

        private void label5_Click(object sender, EventArgs e)
        {
            this.Hide();
            var loginForm = new LoginForm();
            loginForm.Show();
        }
    }
}
