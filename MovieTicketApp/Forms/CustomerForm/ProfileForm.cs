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
    public partial class ProfileForm : UserControl
    {
        private readonly UserService _userService;
        private readonly TicketService _ticketService;
        private BindingSource _bindingSource;
        public ProfileForm()
        {
            InitializeComponent();
            _userService = new UserService();
            _ticketService = new TicketService();
            _bindingSource = new BindingSource();

            Dock = DockStyle.Fill;

            SetupDataGridView();
            LoadUserData();

            updateUserAndPassBtn.Click += updateUserAndPassBtn_Click;
            updatePreferencesBtn.Click += updatePreferencesBtn_Click;
        }

        private void SetupDataGridView()
        {
            // Configure DataGridView
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Add columns
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "id",
                Name = "Id",
                HeaderText = "Mã vé",
                Width = 80
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "seat_numbers",
                Name = "SeatNumbers",
                HeaderText = "Số ghế",
                Width = 120
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "total_price",
                Name = "TotalPrice",
                HeaderText = "Tổng tiền",
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Format = "N0 VNĐ",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                },
                Width = 150
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "purchase_date",
                Name = "PurchaseDate",
                HeaderText = "Ngày mua",
                Width = 150
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "status",
                Name = "Status",
                HeaderText = "Trạng thái",
                Width = 100
            });

            // Additional styling
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Bind data source
            dataGridView1.DataSource = _bindingSource;
        }

        private async void LoadUserData()
        {
            try
            {
                // Load user tickets
                var tickets = await _ticketService.GetTicketsByUserAsync(Program.CurrentUser.Id);
                _bindingSource.DataSource = tickets;

                // Load other user data
                usernameTxt.Text = Program.CurrentUser.Username;
                preferencesTxt.Text = Program.CurrentUser.Preferences;
                passwordTxt.PasswordChar = '•';
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void updateUserAndPassBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usernameTxt.Text) || 
                    string.IsNullOrWhiteSpace(passwordTxt.Text))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin!",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    "Bạn có chắc muốn cập nhật thông tin tài khoản?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    updateUserAndPassBtn.Enabled = false;
                    updateUserAndPassBtn.Text = "Đang cập nhật...";
                    Cursor = Cursors.WaitCursor;

                    var updatedUser = await _userService.UpdateUserProfileAsync(
                        Program.CurrentUser.Id,
                        new Models.User
                        {
                            Username = usernameTxt.Text.Trim(),
                            Password = passwordTxt.Text.Trim()
                        }
                    );

                    Program.CurrentUser = updatedUser;
                    MessageBox.Show("Cập nhật thông tin thành công!",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật thông tin: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                updateUserAndPassBtn.Enabled = true;
                updateUserAndPassBtn.Text = "Sửa";
                Cursor = Cursors.Default;
            }
        }

        private async void updatePreferencesBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(preferencesTxt.Text))
                {
                    MessageBox.Show("Vui lòng nhập sở thích của bạn!",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    "Bạn có chắc muốn cập nhật sở thích?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    updatePreferencesBtn.Enabled = false;
                    updatePreferencesBtn.Text = "Đang cập nhật...";
                    Cursor = Cursors.WaitCursor;

                    var updatedUser = await _userService.UpdatePreferencesAsync(
                        Program.CurrentUser.Id,
                        preferencesTxt.Text.Trim()
                    );

                    Program.CurrentUser = updatedUser;
                    MessageBox.Show("Cập nhật sở thích thành công!",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật sở thích: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                updatePreferencesBtn.Enabled = true;
                updatePreferencesBtn.Text = "Sửa";
                Cursor = Cursors.Default;
            }
        }

    }
}
