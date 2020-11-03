using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form_Alert : Form
    {
        public enum enmAction
        {
            wait,
            start,
            close
        }

        public enum enmType
        {
            Rung,
            Success,
            Error
        }
        private Form_Alert.enmAction action;


        public Form_Alert()
        {
            InitializeComponent();
            button1.Text = "\xE711"; // set exit button icon
            labelIcon.Text = "\xE783"; // set the alert icon
            this.BackColor = ColorTranslator.FromHtml("#1c1c1c");
            this.TopMost = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private int x, y;

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (this.action)
            {
                case enmAction.wait:
                    timer1.Interval = 5000;
                    action = enmAction.close;
                    break;
                case Form_Alert.enmAction.start:
                    this.timer1.Interval = 1;
                    this.Opacity += 0.1;
                    if (this.x < this.Location.X)
                    {
                        this.Left--;
                    }
                    else
                    {
                        if (this.Opacity == 1.0)
                        {
                            action = Form_Alert.enmAction.wait;
                        }
                    }
                    break;
                case enmAction.close:
                    timer1.Interval = 1;
                    this.Opacity -= 0.1;

                    this.Left -= 3;
                    if (base.Opacity == 0.0)
                    {
                        base.Close();
                    }
                    break;
            }
        }

        public void showAlert(string msg, enmType type)
        {
            this.Opacity = 0.0;
            this.StartPosition = FormStartPosition.Manual;
            string fname;

            for (int i = 1; i < 10; i++)
            {
                fname = "alert" + i.ToString();
                Form_Alert frm = (Form_Alert)Application.OpenForms[fname];

                if (frm == null)
                {
                    this.Name = fname;
                    this.x = Screen.PrimaryScreen.WorkingArea.Width - this.Width + 15;
                    this.y = Screen.PrimaryScreen.WorkingArea.Height - this.Height * i - 5 * i;
                    this.Location = new Point(this.x, this.y);
                    break;

                }

            }
            this.x = Screen.PrimaryScreen.WorkingArea.Width - base.Width - 5;

            switch (type)
            {
                // See list of ui symbol at https://docs.microsoft.com/en-us/windows/uwp/design/style/segoe-ui-symbol-font
                case enmType.Success:
                    labelIcon.Text = "\xEB68";
                    labelIcon.ForeColor = ColorTranslator.FromHtml("#3ddc84");
                    //this.BackColor = ColorTranslator.FromHtml("#3ddc84");
                    break;
                case enmType.Error:
                    labelIcon.Text = "\xEB5E";
                    labelIcon.ForeColor = ColorTranslator.FromHtml("#f86734");
                    //this.BackColor = ColorTranslator.FromHtml("#f86734");
                    break;
                case enmType.Rung:
                    labelIcon.Text = "\xEB8C";
                    labelIcon.ForeColor = ColorTranslator.FromHtml("#f1cc81");
                    SystemSounds.Beep.Play();
                    //this.BackColor = ColorTranslator.FromHtml("#f1cc81");
                    break;
            }

            this.alertMsg.Text = msg;
            this.Show();
            this.action = enmAction.start;
            this.timer1.Interval = 1;
            this.timer1.Start();
        }

    }
}
