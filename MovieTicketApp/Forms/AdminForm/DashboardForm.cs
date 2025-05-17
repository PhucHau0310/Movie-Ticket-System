using MovieTicketApp.Models;
using MovieTicketApp.Services;
using System;
using System.Windows.Forms;
using System.Linq;

namespace MovieTicketApp.Forms
{
    public partial class DashboardForm : UserControl
    {
        private readonly MovieService _movieService;
        private readonly UserService _userService;
        private readonly TicketService _ticketService;

        public DashboardForm()
        {
            InitializeComponent();
            _movieService = new MovieService();
            _userService = new UserService();
            _ticketService = new TicketService();

            Dock = DockStyle.Fill;
            SetupDataGridViews();
            this.Load += DashboardForm_Load;
        }

        private void SetupDataGridViews()
        {
            // Setup New Movies DataGridView
            newMovieDataViewGrid.AutoGenerateColumns = false;
            newMovieDataViewGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Title",
                HeaderText = "Tên phim",
                Width = 150
            });
            newMovieDataViewGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Genre",
                HeaderText = "Thể loại",
                Width = 100
            });
            newMovieDataViewGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ReleaseDate",
                HeaderText = "Ngày phát hành",
                Width = 120
            });

            // Setup Purchase History DataGridView
            purchaseHistoryDataViewGrid.AutoGenerateColumns = false;
            purchaseHistoryDataViewGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "id",
                HeaderText = "Mã vé",
                Width = 80
            });
            purchaseHistoryDataViewGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "total_price",
                HeaderText = "Tổng tiền",
                Width = 120
            });
            purchaseHistoryDataViewGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "purchase_date",
                HeaderText = "Ngày mua",
                Width = 120
            });
            purchaseHistoryDataViewGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "status",
                HeaderText = "Trạng thái",
                Width = 80
            });
        }

        private async void DashboardForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Load statistics  
                var movies = await _movieService.GetAllMoviesAsync();
                totalMovie.Text = movies.Count.ToString();

                var users = await _userService.GetAllUsersAsync();
                totalStaff.Text = users.Count(u => u.Role == "staff").ToString();

                var tickets = await _ticketService.GetAllTicketsAsync();
                var totalRevenueValue = tickets
                    //.Where(t => t.Status == "paid")  
                    .Sum(t => t.total_price);
                totalRevenue.Text = totalRevenueValue.ToString("N0") + " VNĐ";

                var todayRevenue = tickets
                    .Where(t =>
                        //t.Status == "paid" &&  
                        DateTime.TryParse(t.purchase_date, out var purchaseDate) &&
                        purchaseDate.Date == DateTime.Today)
                    .Sum(t => t.total_price);
                revenueToday.Text = todayRevenue.ToString("N0") + " VNĐ";

                // Load new movies (last 10)  
                var newMovies = movies
                    .OrderByDescending(m => m.ReleaseDate)
                    .Take(10)
                    .ToList();
                newMovieDataViewGrid.DataSource = newMovies;

                // Load recent purchases (last 10)  
                var recentTickets = tickets
                    .OrderByDescending(t => DateTime.TryParse(t.purchase_date, out var purchaseDate) ? purchaseDate : DateTime.MinValue)
                    .Take(10)
                    .ToList();
                purchaseHistoryDataViewGrid.DataSource = recentTickets;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
