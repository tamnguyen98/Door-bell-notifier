using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Properties;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.SizeChanged += new EventHandler(Form1_ResizeEvent);
            notifyIcon1.DoubleClick += new System.EventHandler(notifyIcon1_DoubleClick);
            // Disable resize
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            // Disable Maximize button
            this.MaximizeBox = false;
            // Center position
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // read setting
            string setting1 = (string)Settings.Default.ServerUrl;
            textBox1.Text = setting1;
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
            }
        }

        private void notifyIcon1_DoubleClick(object Sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void UpdateBtn_Click(object sender, EventArgs e)
        {
            // save setting
            Properties.Settings.Default.ServerUrl = textBox1.Text;
            // force save
            Properties.Settings.Default.Save();
            
        }

        private void resetBtn_Click(object sender, EventArgs e)
        {
            textBox1.Text = Properties.Settings.Default.ServerUrl;
        }

        private void textBox1_MouseLeave(object sender, EventArgs e)
        {
            // save setting
            Properties.Settings.Default.ServerUrl = textBox1.Text;
        }
    }
}
