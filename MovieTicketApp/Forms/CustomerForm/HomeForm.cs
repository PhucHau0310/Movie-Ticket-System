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
    public partial class HomeForm : UserControl
    {
        private readonly MovieService _movieService;

        public HomeForm()
        {
            InitializeComponent();
            _movieService = new MovieService();

            // Configure form
            Dock = DockStyle.Fill;
            flowLayoutPanel1.AutoScroll = true;

            // Load movies when form loads
            this.Load += HomeForm_Load;
        }

        private async void HomeForm_Load(object sender, EventArgs e)
        {
            await LoadMovies();
        }

        private async Task LoadMovies()
        {
            try
            {
                var movies = await _movieService.GetAllMoviesAsync();
                DisplayMovies(movies);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách phim: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayMovies(List<Movie> movies)
        {
            // Clear existing panels
            flowLayoutPanel1.Controls.Clear();

            // Create new panels for each movie
            foreach (var movie in movies)
            {
                var panel = CreateMoviePanel(movie);
                flowLayoutPanel1.Controls.Add(panel);
            }
        }

        private Panel CreateMoviePanel(Movie movie)
        {
            // Create panel
            var panel = new Panel
            {
                BackColor = Color.PowderBlue,
                Size = new Size(250, 320), 
                Margin = new Padding(8)
            };

            // Create picture box
            var pictureBox = new PictureBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                // Dock = DockStyle.Top,
                Size = new Size(250, 180), 
                Location = new Point(0, 0),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            // Create label
            var label = new Label
            {
                AutoSize = false,        
                TextAlign = ContentAlignment.MiddleCenter, 
                Font = new Font("Microsoft Sans Serif", 11, FontStyle.Bold),
                Size = new Size(250, 40), 
                Location = new Point(0, 190), 
                Text = movie.Title,
            };

            // Create button
            var button = new Button
            {
                BackColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Bold),
                Size = new Size(120, 40),
                Location = new Point((250 - 120) / 2, 240),
                UseVisualStyleBackColor = false,
                Text = "Mua Vé",
            };

            // Load movie image
            if (!string.IsNullOrEmpty(movie.ImageUrl))
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        var imageData = client.DownloadData(movie.ImageUrl);
                        using (var ms = new MemoryStream(imageData))
                        {
                            pictureBox.Image = Image.FromStream(ms);
                        }
                    }
                }
                catch
                {
                    pictureBox.Image = null;
                }
            }

            // Add click handler
            button.Tag = movie;
            button.Click += BuyButton_Click;

            // Add controls to panel
            panel.Controls.Add(pictureBox);
            panel.Controls.Add(label);
            panel.Controls.Add(button);

            return panel;
        }

        private void BuyButton_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is Movie movie)
            {
                // Create MovieDetail control
                var movieDetail = new MovieDetail(movie);
                movieDetail.Dock = DockStyle.Fill;

                // Get parent form and replace content
                var customerForm = this.ParentForm as CustomerForm;
                if (customerForm?.mainPanel != null)
                {
                    customerForm.mainPanel.Controls.Clear();
                    customerForm.mainPanel.Controls.Add(movieDetail);
                }
            }
        }
    }
}
