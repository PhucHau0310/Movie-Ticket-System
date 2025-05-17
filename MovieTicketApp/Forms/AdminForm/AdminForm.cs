using MovieTicketApp.Forms;
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

namespace MovieTicketApp
{
    public partial class AdminForm : Form
    {
        private readonly MovieService _movieService;
        public AdminForm()
        {
            InitializeComponent();
            _movieService = new MovieService();

            DashboardForm dashboard = new DashboardForm();
            mainPanel.Controls.Add(dashboard);

            currentUser.Text = $"Xin chào, {Program.CurrentUser.Username} !";
        }
        private async void MainForm_Load(object sender, EventArgs e)
        {
            //var movies = await _movieService.GetMoviesAsync();
            //moviesListBox.DataSource = movies;
            //moviesListBox.DisplayMember = "Title";
        }

        private void LoadDashboard()
        {
            mainPanel.Controls.Clear();

            DashboardForm dashboard = new DashboardForm();

            mainPanel.Controls.Add(dashboard);
        }

        private void LoadStaff()
        {
            mainPanel.Controls.Clear();

            StaffForm staff = new StaffForm();

            mainPanel.Controls.Add(staff);
        }

        private void LoadManageCustomer()
        {
            mainPanel.Controls.Clear();

            ManageCustomerForm cus = new ManageCustomerForm();

            mainPanel.Controls.Add(cus);
        }

        private void LoadMovie()
        {
            mainPanel.Controls.Clear();

            MovieForm movie = new MovieForm();

            mainPanel.Controls.Add(movie);
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

        private void dashboardBtn_Click(object sender, EventArgs e)
        {
            LoadDashboard();
        }

        private void movieBtn_Click(object sender, EventArgs e)
        {
            LoadMovie();
        }

        private void userBtn_Click(object sender, EventArgs e)
        {
            LoadStaff();
        }

        private void customerBtn_Click(object sender, EventArgs e)
        {
            LoadManageCustomer();
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
    }
}
