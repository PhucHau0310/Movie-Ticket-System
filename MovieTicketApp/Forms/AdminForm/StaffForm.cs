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
    public partial class StaffForm : UserControl
    {
        private readonly UserService _userService;
        private int? selectedUserId;
        public StaffForm()
        {
            InitializeComponent();
            _userService = new UserService();

            dataGridView1.CellClick += dataGridView1_CellClick;
            InitializeForm();
        }

        private async void InitializeForm()
        {
            roleBox.SelectedIndex = 0; // Select "staff" by default
            await LoadStaffData();
            SetButtonStates(false);
        }

        private async Task LoadStaffData()
        {
            try
            {
                var staffList = await _userService.GetUsersByRoleAsync("staff");
                dataGridView1.DataSource = staffList;

                dataGridView1.Columns["Password"].Visible = false;
                dataGridView1.Columns["Id"].Width = 50;
                dataGridView1.Columns["Username"].Width = 150;
                dataGridView1.Columns["Role"].Width = 100;

                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.AllowUserToAddRows = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách nhân viên: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void addBtn_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            try
            {
                await _userService.RegisterAsync(
                    usernameTxt.Text.Trim(),
                    passwordTxt.Text,
                    roleBox.SelectedItem.ToString().ToLower()
                );

                MessageBox.Show("Thêm nhân viên thành công!",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearInputs();
                await LoadStaffData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm nhân viên: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void updateBtn_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs() || !selectedUserId.HasValue) return;

            try
            {
                await _userService.UpdateUserProfileAsync(
                    selectedUserId.Value,
                    new Models.User
                    {
                        Username = usernameTxt.Text.Trim(),
                        Password = passwordTxt.Text,
                        Role = roleBox.SelectedItem.ToString().ToLower(),
                    }
                );

                MessageBox.Show("Cập nhật nhân viên thành công!",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearInputs();
                await LoadStaffData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật nhân viên: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (!selectedUserId.HasValue) return;

            var result = MessageBox.Show(
                "Bạn có chắc muốn xóa nhân viên này?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    await _userService.DeleteUserAsync(selectedUserId.Value);
                    MessageBox.Show("Xóa nhân viên thành công!",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ClearInputs();
                    await LoadStaffData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi xóa nhân viên: {ex.Message}",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dataGridView1.Rows[e.RowIndex];

                selectedUserId = Convert.ToInt32(row.Cells["Id"].Value);

                usernameTxt.Text = row.Cells["Username"].Value.ToString();
                passwordTxt.Text = string.Empty;
                roleBox.SelectedItem = row.Cells["Role"].Value.ToString().ToLower();

                SetButtonStates(true);
            }
        }

        private void ClearInputs()
        {
            usernameTxt.Clear();
            passwordTxt.Clear();
            selectedUserId = null;
            SetButtonStates(false);
        }

        private void SetButtonStates(bool isEditing)
        {
            updateBtn.Enabled = isEditing;
            deleteBtn.Enabled = isEditing;
            addBtn.Enabled = !isEditing;
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(usernameTxt.Text) ||
                string.IsNullOrWhiteSpace(passwordTxt.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

    }
}
