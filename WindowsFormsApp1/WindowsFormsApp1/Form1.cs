using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Properties;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private static string serverURL = "";
        private static bool successConnection = false;

        private Thread subThread;

        public delegate void MyDelegate(string msg, Form_Alert.enmType type);

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

        private void SpawnThread()
        {
            subThread = new Thread(new ThreadStart(ConnectTo));
            subThread.IsBackground = true;
            subThread.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // read setting
            string setting1 = (string)Settings.Default.ServerUrl;
            //serverURL = setting1;
            if (string.IsNullOrEmpty(setting1) && string.IsNullOrEmpty(serverURL))
                textBox1.Text = serverURL;
            else
                textBox1.Text = setting1;

            if (!string.IsNullOrEmpty(serverURL))
            {
                SpawnThread();
                Console.WriteLine("Thread spawned");
            }
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            if (subThread != null && !subThread.IsAlive)
                subThread.Join();
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
            UpdateSettingDefault();
            // force save
            Properties.Settings.Default.Save();

            Alert("Someone is at the door!", Form_Alert.enmType.Rung);
            
        }

        private void resetBtn_Click(object sender, EventArgs e)
        {
            textBox1.Text = Properties.Settings.Default.ServerUrl;
        }

        private void textBox1_MouseLeave(object sender, EventArgs e)
        {
            UpdateSettingDefault();
        }

        // Saves
        private void UpdateSettingDefault()
        {
            // save setting
            string textboxVal = textBox1.Text;
            if (!String.IsNullOrEmpty(textboxVal))
            {
                Properties.Settings.Default.ServerUrl = textboxVal;
                Console.WriteLine(textboxVal);
                if (subThread == null)
                    SpawnThread();
            }
        }

        // Show the notification on the desktop (Main display)
        public void Alert(string msg, Form_Alert.enmType type)
        {
            Form_Alert frm = new Form_Alert();
            frm.showAlert(msg, type);
        }


        // function for thread
        public void ConnectTo()
        {
            // flag to prevent more than one notification per ring
            bool alerted = false;

            using (var client = new HttpClient())
            {
                Console.WriteLine("Starting connection.");
                client.Timeout = TimeSpan.FromSeconds(2);
                while (true)
                {
                    try
                    {

                        //Console.WriteLine(serverURL);
                        var response = client.GetAsync(serverURL).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            if (!successConnection)
                            {
                                successConnection = true;
                                MyDelegate delInst = new MyDelegate(Alert);
                                this.BeginInvoke(delInst, "Connected to server.", Form_Alert.enmType.Success);
                                Console.WriteLine("Connected");
                            }
                            var responseContent = response.Content;

                            // by calling .Result you are synchronously reading the result
                            int rung = Int16.Parse(responseContent.ReadAsStringAsync().Result);

                            if (rung == 1 && !alerted)
                            {
                                alerted = true;
                                MyDelegate delInst = new MyDelegate(Alert);
                                this.BeginInvoke(delInst, "Someone is at the door!", Form_Alert.enmType.Rung);
                            }
                            else if (rung == 0 && alerted)
                                alerted = false;
                        }
                    }
                    // IF we lose connection to the server
                    catch (System.AggregateException e)
                    {
                        if (successConnection)
                        {
                            Console.WriteLine("Disconnected");
                            MyDelegate delInst = new MyDelegate(Alert);
                            this.BeginInvoke(delInst, "Lost connection to server.", Form_Alert.enmType.Error);
                        }
                        successConnection = false;
                    }
                }
            }
        }
    }
}
