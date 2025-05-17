using MovieTicketApp.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieTicketApp.Forms
{
    public partial class ManageCustomerForm : UserControl
    {
        private readonly UserService _userService;
        private int? selectedUserId;

        public ManageCustomerForm()
        {
            InitializeComponent();
            _userService = new UserService();

            // Add event handlers
            dataGridView1.CellClick += dataGridView1_CellClick;
            this.Load += ManageCustomerForm_Load;
            addBtn.Click += addBtn_Click;
            updateBtn.Click += updateBtn_Click;
            deleteBtn.Click += deleteBtn_Click;
            clearBtn.Click += clearBtn_Click;

            InitializeForm();
        }

        private async void InitializeForm()
        {
            await LoadCustomerData();
            SetButtonStates(false);
        }

        private async Task LoadCustomerData()
        {
            try
            {
                var customers = await _userService.GetUsersByRoleAsync("customer");
                dataGridView1.DataSource = customers;

                // Configure grid view
                dataGridView1.Columns["Password"].Visible = false;
                dataGridView1.Columns["Id"].Width = 50;
                dataGridView1.Columns["Username"].Width = 150;
                dataGridView1.Columns["Preferences"].Width = 200;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.AllowUserToAddRows = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách khách hàng: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void addBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateInputs() == false) return;

                var user = await _userService.RegisterAsync(
                    usernameTxt.Text.Trim(),
                    passwordTxt.Text,
                    "customer"
                );

                MessageBox.Show("Thêm khách hàng thành công!", 
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                await LoadCustomerData();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm khách hàng: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void updateBtn_Click(object sender, EventArgs e)
        {
            if (!selectedUserId.HasValue || ValidateInputs() == false) return;

            try
            {
                await _userService.UpdateUserProfileAsync(
                    selectedUserId.Value,
                    new Models.User
                    {
                        Username = usernameTxt.Text.Trim(),
                        Password = passwordTxt.Text,
                        Role = "customer"
                    }
                );

                MessageBox.Show("Cập nhật khách hàng thành công!",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                await LoadCustomerData();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật khách hàng: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (!selectedUserId.HasValue) return;

            var result = MessageBox.Show(
                "Bạn có chắc muốn xóa khách hàng này?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    await _userService.DeleteUserAsync(selectedUserId.Value);
                    MessageBox.Show("Xóa khách hàng thành công!",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    await LoadCustomerData(); // Refresh data after deleting
                    ClearSelection();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi xóa khách hàng: {ex.Message}",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            ClearSelection();
        }

        private void ClearSelection()
        {
            dataGridView1.ClearSelection();
            selectedUserId = null;
            SetButtonStates(false);
        }

        private void ManageCustomerForm_Load(object sender, EventArgs e)
        {
            InitializeForm();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dataGridView1.Rows[e.RowIndex];
                selectedUserId = Convert.ToInt32(row.Cells["Id"].Value);
                usernameTxt.Text = row.Cells["Username"].Value.ToString();
                //emailTxt.Text = row.Cells["Email"].Value?.ToString();
                passwordTxt.Text = string.Empty; // Clear password for security
                SetButtonStates(true);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(usernameTxt.Text) ||
                string.IsNullOrWhiteSpace(passwordTxt.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin khách hàng!",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void ClearInputs()
        {
            usernameTxt.Clear();
            passwordTxt.Clear();
            selectedUserId = null;
            SetButtonStates(false);
        }

        private void SetButtonStates(bool isSelected)
        {
            updateBtn.Enabled = isSelected;
            deleteBtn.Enabled = isSelected;
            addBtn.Enabled = !isSelected;
        }
    }
}
