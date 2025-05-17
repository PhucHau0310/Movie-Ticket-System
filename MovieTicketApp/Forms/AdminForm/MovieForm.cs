using MovieTicketApp.Models;
using MovieTicketApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieTicketApp.Forms
{
    public partial class MovieForm : UserControl
    {
        private readonly MovieService _movieService;
        private int? selectedMovieId;
        private PictureBox moviePictureBox;

        public MovieForm()
        {
            InitializeComponent();
            _movieService = new MovieService();

            // Add PictureBox for movie image
            SetupPictureBox();

            // Add event handlers
            dataGridView1.CellClick += dataGridView1_CellClick;
            //addBtn.Click += addBtn_Click;
            //updateBtn.Click += updateBtn_Click;
            //deleteBtn.Click += deleteBtn_Click;
            //clearBtn.Click += clearBtn_Click;

            LoadMovies();
        }

        private void SetupPictureBox()
        {
            moviePictureBox = new PictureBox
            {
                Size = new Size(185, 200),
                Location = new Point(30, 480),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };
            panel3.Controls.Add(moviePictureBox);
        }

        private async Task LoadMovies()
        {
            try
            {
                var movies = await _movieService.GetAllMoviesAsync();
                dataGridView1.DataSource = movies;
                ConfigureDataGridView();

                if (dataGridView1.SelectedRows.Count > 0)
                {
                    var imageUrl = dataGridView1.SelectedRows[0].Cells["ImageUrl"].Value?.ToString();
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        await LoadMovieImage(imageUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách phim: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureDataGridView()
        {
            dataGridView1.Columns["Id"].Width = 50;
            dataGridView1.Columns["Title"].Width = 150;
            dataGridView1.Columns["Description"].Width = 200;
            dataGridView1.Columns["Genre"].Width = 100;
            dataGridView1.Columns["Duration"].Width = 80;
            dataGridView1.Columns["ReleaseDate"].Width = 100;
            dataGridView1.Columns["Price"].Width = 80;
            dataGridView1.Columns["Rating"].Width = 80;
            dataGridView1.Columns["ImageUrl"].Width = 200;

            dataGridView1.Columns["ReleaseDate"].HeaderText = "Ngày chiếu";
            dataGridView1.Columns["ImageUrl"].HeaderText = "URL Ảnh";

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }

        private async void addBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInputs()) return;

                var movie = new Movie
                {
                    Title = titleTxt.Text.Trim(),
                    Description = descTxt.Text.Trim(),
                    Genre = genreTxt.Text.Trim(),
                    Duration = int.Parse(durationTxt.Text),
                    ReleaseDate = releaseDateTxt.Text.Trim(),
                    Price = float.Parse(priceTxt.Text),
                    Rating = float.Parse(ratingTxt.Text),
                    ImageUrl = imageUrlTxt.Text.Trim()
                };

                await _movieService.CreateMovieAsync(movie);
                MessageBox.Show("Thêm phim thành công!",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                await LoadMovies();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm phim: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void updateBtn_Click(object sender, EventArgs e)
        {
            if (!selectedMovieId.HasValue || !ValidateInputs()) return;

            try
            {
                var movie = new Movie
                {
                    Id = selectedMovieId.Value,
                    Title = titleTxt.Text.Trim(),
                    Description = descTxt.Text.Trim(),
                    Genre = genreTxt.Text.Trim(),
                    Duration = int.Parse(durationTxt.Text),
                    ReleaseDate = releaseDateTxt.Text.Trim(),
                    Price = float.Parse(priceTxt.Text),
                    Rating = float.Parse(ratingTxt.Text),
                    ImageUrl = imageUrlTxt.Text.Trim()
                };

                await _movieService.UpdateMovieAsync(selectedMovieId.Value, movie);
                MessageBox.Show("Cập nhật phim thành công!",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                await LoadMovies();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật phim: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (!selectedMovieId.HasValue) return;

            var result = MessageBox.Show(
                "Bạn có chắc muốn xóa phim này?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    await _movieService.DeleteMovieAsync(selectedMovieId.Value);
                    MessageBox.Show("Xóa phim thành công!",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    await LoadMovies();
                    ClearInputs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi xóa phim: {ex.Message}",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dataGridView1.Rows[e.RowIndex];
                selectedMovieId = Convert.ToInt32(row.Cells["Id"].Value);

                titleTxt.Text = row.Cells["Title"].Value.ToString();
                descTxt.Text = row.Cells["Description"].Value.ToString();
                genreTxt.Text = row.Cells["Genre"].Value.ToString();
                durationTxt.Text = row.Cells["Duration"].Value.ToString();
                priceTxt.Text = row.Cells["Price"].Value.ToString();
                ratingTxt.Text = row.Cells["Rating"].Value?.ToString();
                imageUrlTxt.Text = row.Cells["ImageUrl"].Value?.ToString();
                releaseDateTxt.Text = row.Cells["ReleaseDate"].Value.ToString();

                LoadMovieImage(imageUrlTxt.Text);
                SetButtonStates(true);
            }
        }

        private async 
        Task
LoadMovieImage(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl)) return;

                using (var client = new WebClient())
                {
                    var imageData = await client.DownloadDataTaskAsync(imageUrl);
                    using (var ms = new System.IO.MemoryStream(imageData))
                    {
                        moviePictureBox.Image = Image.FromStream(ms);
                    }
                }
            }
            catch
            {
                moviePictureBox.Image = null;
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(titleTxt.Text) ||
                string.IsNullOrWhiteSpace(descTxt.Text) ||
                string.IsNullOrWhiteSpace(genreTxt.Text) ||
                string.IsNullOrWhiteSpace(durationTxt.Text) ||
                string.IsNullOrWhiteSpace(priceTxt.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin phim!",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!float.TryParse(priceTxt.Text, out _))
            {
                MessageBox.Show("Giá phải là số!",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(durationTxt.Text, out _))
            {
                MessageBox.Show("Thời lượng phải là số nguyên!",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void ClearInputs()
        {
            titleTxt.Clear();
            descTxt.Clear();
            genreTxt.Clear();
            durationTxt.Clear();
            priceTxt.Clear();
            ratingTxt.Clear();
            imageUrlTxt.Clear();
            releaseDateTxt.Clear();
            moviePictureBox.Image = null;
            selectedMovieId = null;
            SetButtonStates(false);
        }

        private void SetButtonStates(bool isSelected)
        {
            updateBtn.Enabled = isSelected;
            deleteBtn.Enabled = isSelected;
            addBtn.Enabled = !isSelected;
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

    }
}
