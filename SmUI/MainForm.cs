using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoreLib;

namespace SmUI
{
    public partial class MainForm : Form
    {
        private const int refreshIntervalMs = 30000;
        private readonly object _lock = new object();
        private bool _stealthMode = false;
        private bool _isOnline = false;
        private DateTime _lastSendedEmail = DateTime.MinValue;

        static System.Windows.Forms.Timer _refreshTimer = new System.Windows.Forms.Timer();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            UpdateFormValues();
            _refreshTimer.Tick += RefreshTimerEventProcessor;
            _refreshTimer.Interval = refreshIntervalMs;
            _refreshTimer.Start();
        }

        private void RefreshTimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            UpdateFormValues();
        }

        private void UpdateFormValues()
        {
            OptimizationServiceSettings settings;

            lock (_lock)
            {
                try
                {
                    settings = WebServiceWrapper.GetSettings();
                }
                catch (Exception)
                {
                    ErrorMode();
                    return;
                }

                _stealthMode = settings.StealthMode;
                _isOnline = true;

                StealthModeBtn.Text = settings.StealthMode ? "Enable Optimization" : "Stealth Mode";
                IsOnlineValue.Text = "true";
                StealthModeBtn.Enabled = true;
            }
        }

        private void ErrorMode()
        {
            IsOnlineValue.Text = "false";
            StealthModeBtn.Text = "Invalid State";
            StealthModeBtn.Enabled = false;
            _isOnline = false;
            SendEmail();
        }

        private void StealthModeBtn_Click(object sender, EventArgs e)
        {
            lock (_lock)
            {
                _stealthMode = !_stealthMode;

                try
                {
                    WebServiceWrapper.SetSettings(_stealthMode);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Can't change WCF service settings! " + ex.Message);
                    ErrorMode();
                }

                UpdateFormValues();
            }
        }

        private void SendEmail()
        {
            if (DateTime.Now - _lastSendedEmail < TimeSpan.FromMinutes(30))
            {
                return;
            }

            try
            {
                var fromAddress = new MailAddress("thesnarb@gmail.com", "From Name");
                var toAddress = new MailAddress("pavel.konovalov@softheme.com", "To Name");
                const string fromPassword = "lmzziwxvnifygzay";
                const string subject = "Sevice is not available!";
                const string body = "Sevice is not available!";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                    Timeout = 20000
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                
                MessageBox.Show("Failed to send e-mail: " + ex.Message);
            }


            _lastSendedEmail = DateTime.Now;
            
        }
    }
}
