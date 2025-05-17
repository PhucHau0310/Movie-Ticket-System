using MovieTicketApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieTicketApp.Forms.CustomerForm
{
    public partial class MovieCard : UserControl
    {
        private readonly Movie _movie;
        private Panel mainPanel;
        private PictureBox posterPictureBox;
        private Label titleLabel;
        private Label genreLabel;
        private Label releaseDateLabel;

        public MovieCard(Movie movie)
        {
            _movie = movie;
            InitializeComponents();
            LoadData();
        }

        private void InitializeComponents()
        {
            // Main panel
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            // Poster picture box
            posterPictureBox = new PictureBox
            {
                Width = 200,
                Height = 300,
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(15, 15)
            };

            // Title label
            titleLabel = new Label
            {
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Width = 200,
                Height = 25,
                Location = new Point(15, 320)
            };

            // Genre label
            genreLabel = new Label
            {
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 10),
                ForeColor = Color.Gray,
                Width = 200,
                Height = 20,
                Location = new Point(15, 345)
            };

            // Release date label
            releaseDateLabel = new Label
            {
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 10),
                ForeColor = Color.Gray,
                Width = 200,
                Height = 20,
                Location = new Point(15, 365)
            };

            // Add controls
            mainPanel.Controls.Add(posterPictureBox);
            mainPanel.Controls.Add(titleLabel);
            mainPanel.Controls.Add(genreLabel);
            mainPanel.Controls.Add(releaseDateLabel);
            Controls.Add(mainPanel);

            // Add hover effect
            this.MouseEnter += MovieCard_MouseEnter;
            this.MouseLeave += MovieCard_MouseLeave;
            foreach (Control control in mainPanel.Controls)
            {
                control.MouseEnter += MovieCard_MouseEnter;
                control.MouseLeave += MovieCard_MouseLeave;
            }
        }

        private void LoadData()
        {
            // Set data
            titleLabel.Text = _movie.Title;
            genreLabel.Text = _movie.Genre;
            releaseDateLabel.Text = DateTime.Parse(_movie.ReleaseDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            // Load poster image
            try
            {
                if (!string.IsNullOrEmpty(_movie.ImageUrl))
                {
                    using (var webClient = new System.Net.WebClient())
                    {
                        byte[] imageBytes = webClient.DownloadData(_movie.ImageUrl);
                        using (var ms = new System.IO.MemoryStream(imageBytes))
                        {
                            posterPictureBox.Image = Image.FromStream(ms);
                        }
                    }
                }
            }
            catch
            {
                // Use placeholder image if loading fails
                posterPictureBox.Image = Properties.Resources.cinema;
            }
        }

        private void MovieCard_MouseEnter(object sender, EventArgs e)
        {
            mainPanel.BackColor = Color.FromArgb(240, 240, 240);
            Cursor = Cursors.Hand;
        }

        private void MovieCard_MouseLeave(object sender, EventArgs e)
        {
            mainPanel.BackColor = Color.White;
            Cursor = Cursors.Default;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Add border
            using (Pen pen = new Pen(Color.LightGray))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            }
        }
    }
}
