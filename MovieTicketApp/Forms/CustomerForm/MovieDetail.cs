using MovieTicketApp.Models;
using MovieTicketApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieTicketApp.Forms.CustomerForm
{
    public partial class MovieDetail : UserControl
    {
        private readonly Movie _movie;
        private decimal totalPrice = 0;
        private readonly TicketService _ticketService;

        public MovieDetail(Movie movie)
        {
            InitializeComponent();
            _movie = movie;
            _ticketService = new TicketService();

            // Configure form
            LoadMovieDetails();
            SetupEventHandlers();
        }

        private void LoadMovieDetails()
        {
            // Set movie title
            titleTxtBox.Text = _movie.Title;
            titleTxtBox.ReadOnly = true;

            // Set price
            priceTxtBox.Text = $"{_movie.Price:N0} VNĐ";
            priceTxtBox.ReadOnly = true;

            // Set description
            descTxt.Text = _movie.Description;
            descTxt.ReadOnly = true;

            // Configure datetime picker
            datetimeView.MinDate = DateTime.Today;
            datetimeView.Value = DateTime.Today;

            // Configure number seats
            numberSeats.Minimum = 1;
            numberSeats.Maximum = 10;
            numberSeats.Value = 1;

            // Calculate initial total price
            UpdateTotalPrice();

            // Load movie image
            if (!string.IsNullOrEmpty(_movie.ImageUrl))
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        var imageData = client.DownloadData(_movie.ImageUrl);
                        using (var ms = new MemoryStream(imageData))
                        {
                            pictureMovie.Image = Image.FromStream(ms);
                            pictureMovie.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                    }
                }
                catch
                {
                    pictureMovie.Image = null;
                }
            }
        }

        private void SetupEventHandlers()
        {
            // Add handlers
            numberSeats.ValueChanged += (s, e) => UpdateTotalPrice();
            backBtn.Click += BackBtn_Click;
            btnBook.Click += BtnBook_Click;
        }

        private void UpdateTotalPrice()
        {
            totalPrice = (decimal)_movie.Price * (decimal)numberSeats.Value;
            toalPrice.Text = $"{totalPrice:N0} VNĐ";
            toalPrice.ReadOnly = true;
        }

        private void BackBtn_Click(object sender, EventArgs e)
        {
            // Return to HomeForm
            var customerForm = this.ParentForm as CustomerForm;
            if (customerForm?.mainPanel != null)
            {
                customerForm.mainPanel.Controls.Clear();
                var homeForm = new HomeForm();
                customerForm.mainPanel.Controls.Add(homeForm);
            }
        }

        private async void BtnBook_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                $"Xác nhận đặt {numberSeats.Value} vé xem phim {_movie.Title}\n" +
                $"Ngày xem: {datetimeView.Value:dd/MM/yyyy}\n" +
                $"Tổng tiền: {totalPrice:N0} VNĐ",
                "Xác nhận đặt vé",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                // MessageBox.Show("Đặt vé thành công!",
                //     "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                try
                {
                    btnBook.Enabled = false;
                    btnBook.Text = "Đang xử lý...";
                    Cursor = Cursors.WaitCursor;

                    // Create ticket request
                    var ticket = new TicketCreate
                    {
                        seat_numbers = string.Join(",", Enumerable.Range(1, (int)numberSeats.Value)),
                        user_id = Program.CurrentUser.Id,
                        movie_id = _movie.Id
                    };

                    // Get payment URL from API
                    var response = await _ticketService.CreateTicketAsync(ticket);

                    // Show payment form
                    var paymentForm = new PaymentForm(response.payment_url);
                    paymentForm.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi đặt vé: {ex.Message}",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    btnBook.Enabled = true;
                    btnBook.Text = "Đặt vé";
                    Cursor = Cursors.Default;
                }        
            }
        }
    }
}
