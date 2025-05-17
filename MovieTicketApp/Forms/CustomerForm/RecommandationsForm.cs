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

namespace MovieTicketApp.Forms.CustomerForm
{
    public partial class RecommandationsForm : UserControl
    {
        private readonly MovieService _movieService;

        public RecommandationsForm()
        {
            InitializeComponent();
            _movieService = new MovieService();
            
            this.Dock = DockStyle.Fill;
            this.Load += RecommandationsForm_Load;
        }

        private async void RecommandationsForm_Load(object sender, EventArgs e)
        {
            await LoadRecommendations();
        }

        private async Task LoadRecommendations()
        {
            try
            {
                flowLayoutPanel.Controls.Clear();
                
                // Get recommendations for current user
                var recommendations = await _movieService.GetRecommendationsAsync(Program.CurrentUser.Id);

                if (recommendations != null && recommendations.Count > 0)
                {
                    foreach (var movie in recommendations)
                    {
                        var movieCard = new MovieCard(movie)
                        {
                            Width = 230,
                            Height = 400,
                            Margin = new Padding(10)
                        };
                        flowLayoutPanel.Controls.Add(movieCard);
                    }
                }
                else
                {
                    var label = new Label
                    {
                        Text = "Không có đề xuất nào. Hãy cập nhật sở thích của bạn!",
                        AutoSize = true,
                        Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular),
                        Margin = new Padding(10)
                    };
                    flowLayoutPanel.Controls.Add(label);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải đề xuất: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
