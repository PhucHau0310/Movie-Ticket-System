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
    public partial class PaymentForm : Form
    {
        public PaymentForm(string paymentUrl)
        {
            InitializeComponent();

            // Configure form
            this.Size = new Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Thanh toán VNPay";

            // Create WebBrowser control
            var webBrowser = new WebBrowser
            {
                Dock = DockStyle.Fill,
                ScriptErrorsSuppressed = true
            };

            // Navigate to payment URL
            webBrowser.Navigate(paymentUrl);

            // Add to form
            this.Controls.Add(webBrowser);

            // Handle document completed to check for payment result
            webBrowser.DocumentCompleted += (s, e) =>
            {
                if (webBrowser.Url.ToString().Contains("vnpay_return"))
                {
                    MessageBox.Show("Thanh toán hoàn tất!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            };
        }
    }
}
