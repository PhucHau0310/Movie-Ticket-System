using MovieTicketApp.Forms;
using MovieTicketApp.Forms.CustomerForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieTicketApp.Forms.CustomerForm
{
    public partial class CustomerForm : Form
    {
        public CustomerForm()
        {
            InitializeComponent();

            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 70); 
            mainPanel.Size = new Size(ClientSize.Width, ClientSize.Height - 70);

            LoadHomeForm();

            currentUserTxt.Text = $"Xin chào, {Program.CurrentUser.Username}";
        }

        private void LoadHomeForm()
        {
            mainPanel.Controls.Clear();
            HomeForm homeForm = new HomeForm
            {
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(homeForm);
        }

        private void homeBtn_Click(object sender, EventArgs e)
        {
            LoadHomeForm();
        }

        private void profileBtn_Click(object sender, EventArgs e)
        {
            mainPanel.Controls.Clear();
            ProfileForm profileForm = new ProfileForm
            {
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(profileForm);
        }

        private void logoutBtn_Click(object sender, EventArgs e)
        {
            Program.CurrentUser = null;
            this.Hide();
            var loginForm = new LoginForm();
            loginForm.Show();
        }

        private void exitBtn_Click(object sender, EventArgs e)
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

        private void recommandBtn_Click(object sender, EventArgs e)
        {
            mainPanel.Controls.Clear();
            RecommandationsForm recommandationsForm = new RecommandationsForm
            {
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(recommandationsForm);
        }
    }
}
