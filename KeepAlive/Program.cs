using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeepAlive
{
    /// <summary>
    /// This is a simple app to ping the website in order to prevent the worker process from being recycled.
    /// The host, GoDaddy recycles their worker processes after five minutes of inactivity.  This means that
    /// without pinging the site, everything has to be reloaded, making the site very slow.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
