using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeepAlive
{
    public partial class Form1 : Form
    {
        Stopwatch stopWatch = new Stopwatch();

        public Form1()
        {
            InitializeComponent();
        }

        private int RequiredResponseTime
        {
            get
            {
                return Int32.Parse(ResponseTimeTextBox.Text);
            }
            set
            {
                ResponseTimeTextBox.Text = value.ToString();
            }
        }

        private void PingTimer_Tick(object sender, EventArgs e)
        {
            PingTimer.Enabled = false;
            try
            {
                using (var client = new WebClient())
                {
                    stopWatch.Restart();
                    var response = client.DownloadString(UrlTextBox.Text);
                    stopWatch.Stop();
                    Output(String.Format("Ping interval: {0}   Download time: {1}   Response: {2}",
                        PingTimer.Interval, stopWatch.Elapsed, response));
                    if (stopWatch.ElapsedMilliseconds > RequiredResponseTime)
                    {
                        PingTimer.Interval = (int)((float)PingTimer.Interval * 0.9);
                    }
                    else
                    {
                        PingTimer.Interval = (int)((float)PingTimer.Interval * 1.005);
                    }
                }
            }
            finally
            {
                PingTimer.Enabled = true;
            }
        }

        private void Output(string message)
        {
            if (OutputTextBox.TextLength > 10000)
            {
                OutputTextBox.Text = 
                    String.Join(Environment.NewLine, 
                    OutputTextBox.Text.Split(new []{Environment.NewLine}, StringSplitOptions.None).Skip(10));
            }
            OutputTextBox.AppendText(message + Environment.NewLine);
        }

        private void OutputTextBox_VisibleChanged(object sender, EventArgs e)
        {
            if (OutputTextBox.Visible)
            {
                OutputTextBox.SelectionStart = OutputTextBox.TextLength;
                OutputTextBox.ScrollToCaret();
            }
        }
    }
}
