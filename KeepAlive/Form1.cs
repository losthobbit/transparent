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

        double reducer = 0.5;
        double increaser = 1.2;
        double normalizer = 0.95;

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
                    WebException webException = null;
                    string response;
                    stopWatch.Restart();
                    try
                    {
                        response = client.DownloadString(UrlTextBox.Text);
                    }
                    catch (WebException ex)
                    {
                        webException = ex;
                        response = ex.Message;
                    }
                    stopWatch.Stop();
                    Output(String.Format("Time: {0}  Ping interval: {1}   Download time: {2}   Response: {3}",
                        DateTime.Now.TimeOfDay, PingTimer.Interval, stopWatch.Elapsed, response));
                    if (webException != null || stopWatch.ElapsedMilliseconds > RequiredResponseTime)
                    {
                        PingTimer.Interval = (int)((double)PingTimer.Interval * reducer);
                    }
                    else
                    {
                        PingTimer.Interval = (int)((double)PingTimer.Interval * increaser);
                    }
                }
                Normalize(ref reducer);
                Normalize(ref increaser);
            }
            finally
            {
                PingTimer.Enabled = true;
            }
        }

        /// <summary>
        /// Make the number closer to 1.
        /// </summary>
        /// <param name="value"></param>
        private void Normalize(ref double value)
        {
            if (value == 1)
                return;
            value = 1 + (value - 1) * normalizer;
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
