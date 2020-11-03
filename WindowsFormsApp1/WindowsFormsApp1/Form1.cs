using System;
using System.Net.Http;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private static Uri serverURL;
        private static bool successConnection = false;
        private bool firstTimeUser = true;

        private Thread subThread;

        public delegate void AlertDelegate(string msg, Form_Alert.enmType type);
        public delegate void ErrorDelegate(bool b);

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
            Properties.Settings.Default.Upgrade();

            // read setting
            string setting1 = Properties.Settings.Default.ServerUrl;

            if (!string.IsNullOrEmpty(setting1))
            {
                Uri.TryCreate(setting1, UriKind.RelativeOrAbsolute, out serverURL);
                textBox1.Text = setting1;
                firstTimeUser = false;
            }
            else if (serverURL != null && string.IsNullOrEmpty(serverURL.AbsolutePath))
                textBox1.Text = serverURL.AbsolutePath;

            if (serverURL != null && !string.IsNullOrEmpty(serverURL.AbsolutePath))
            {
                SpawnThread();
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
                notifyIcon1.BalloonTipTitle = "Doorbell Notifications";
                notifyIcon1.BalloonTipText = "I'll hide down here in the tray. Double click my (bell) icon if you want to update/close me!";
                notifyIcon1.ShowBalloonTip(1000);
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
            Properties.Settings.Default.Reload();

        }

        private void resetBtn_Click(object sender, EventArgs e)
        {
            textBox1.Text = Properties.Settings.Default.ServerUrl;
        }

        // Saves
        private void UpdateSettingDefault()
        {
            // save setting
            string textboxVal = textBox1.Text;
            if (!String.IsNullOrEmpty(textboxVal))
            {
                if (Uri.TryCreate(textboxVal, UriKind.RelativeOrAbsolute, out serverURL) == true)
                {
                    Properties.Settings.Default.ServerUrl = textboxVal;
                    Console.WriteLine(textboxVal);
                    successConnection = false;
                    if (subThread == null)
                        SpawnThread();
                }
            }
        }

        // Show the notification on the desktop (Main display)
        public void Alert(string msg, Form_Alert.enmType type)
        {
            Form_Alert frm = new Form_Alert();
            frm.showAlert(msg, type);
        }

        public void ShowURLError(bool b)
        {
            this.errorLabel.Visible = b;
        }


        // function for thread
        public void ConnectTo()
        {
            // flag to prevent more than one notification per ring
            bool alerted = false;
            ErrorDelegate erDel = new ErrorDelegate(ShowURLError);
            AlertDelegate delInst = new AlertDelegate(Alert);

            using (var client = new HttpClient())
            {
                Console.WriteLine("Starting connection.");
                client.Timeout = TimeSpan.FromSeconds(3);
                while (true)
                {
                    try
                    {

                        //Console.WriteLine(serverURL);
                        var response = client.GetAsync(serverURL).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            this.BeginInvoke(erDel, false);
                            if (!successConnection)
                            {
                                successConnection = true;
                                this.BeginInvoke(delInst, "Connected to server.", Form_Alert.enmType.Success);
                                Console.WriteLine("Connected");
                            }
                            var responseContent = response.Content;

                            // by calling .Result you are synchronously reading the result
                            int rung = Int16.Parse(responseContent.ReadAsStringAsync().Result);

                            if (rung == 1 && !alerted)
                            {
                                alerted = true;
                                //AlertDelegate delInst = new AlertDelegate(Alert);
                                this.BeginInvoke(delInst, "Someone is at the door!", Form_Alert.enmType.Rung);
                            }
                            else if (rung == 0 && alerted)
                                alerted = false;
                        }
                    }
                    // IF we lose connection to the server
                    catch
                    {
                        this.BeginInvoke(erDel, true);
                        if (successConnection || firstTimeUser)
                        {
                            firstTimeUser = false;
                            Console.WriteLine("Disconnected");
                            //AlertDelegate delInst = new AlertDelegate(Alert);
                            this.BeginInvoke(delInst, "Lost connection to server.", Form_Alert.enmType.Error);
                        }
                        successConnection = false;
                    }
                }
            }
        }
    }
}
