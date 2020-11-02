using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.SizeChanged += new EventHandler(Form1_ResizeEvent);
            notifyIcon1.DoubleClick += new System.EventHandler(notifyIcon1_DoubleClick);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_ResizeEvent(object sender, System.EventArgs e)
        {
            //if the form is minimized  
            //hide it from the task bar  
            //and show the system tray icon (represented by the NotifyIcon control)  
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
                Console.WriteLine("hide");
            }
        }

        private void notifyIcon1_DoubleClick(object Sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }
    }
}
