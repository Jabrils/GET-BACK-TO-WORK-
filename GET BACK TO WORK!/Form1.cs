using System;
using System.Drawing;
using System.Diagnostics;
using System.Speech.Synthesis;
using System.Threading;
using System.Windows.Forms;

namespace GET_BACK_TO_WORK_
{
    public partial class Form1 : Form
    {
        static bool run;
        static int msgTime = 5000;
        static SpeechSynthesizer _sS = new SpeechSynthesizer();
        static string prev = "";

        public Form1()
        {
            // Initializes
            InitializeComponent();

            // Minimized the window at start (because it doesn't have any UI anyway)
            WindowState = FormWindowState.Minimized;

            // Removes it from the taskbar because this is a background app
            ShowInTaskbar = false;
            Opacity = 0;

            // This makes the app hide in the tab views
            FormBorderStyle = FormBorderStyle.FixedToolWindow;

            // This sets the notification hover over text
            SetIconText();
        }

            // This sets the notification hover over text
        private void SetIconText()
        {
            notifyIcon1.Text = "Get Back To Work!\n" + (run ? "Running" : "Not Running");
        }

        private static void ResetPrev()
        {
            while (run)
            {
                prev = "";
                Thread.Sleep(10000);
            }
        }
            // This is our main program, aka our checker to see if youre using browsers or not.
        private static void GetBackToWork()
        {
            while (run)
            {
                // Gets an array of all applications currently running
                Process[] AllProcesses = Process.GetProcesses();

                // For each of those applications in this array do this:
                foreach (var process in AllProcesses)
                {
                    // If the Window Title isn't blank, then:
                    if (process.MainWindowTitle != "")
                    {
                        // Lowercase the name of the application
                        string s = process.ProcessName.ToLower();

                        // If the application is any of these browsers:
                        if (s == "iexplore" || s == "iexplorer" || s == "microsoftedgecp" || s == "microsoftedge" || s == "chrome" || s == "firefox" || s == "safari" || s == "opera")
                        {
                            // Kill them immediately, with fire
                            process.CloseMainWindow();

                            // This yells at the user for trying to use whatever browser
                            YellAtUser(s);

                            // Log that a launch has been attempted
                            Console.WriteLine(s + " was ATTEMPTED!");

                            //MessageBox.Show(msg, "WHAT ARE YOU DOING!?", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }
                }

                // Log watching updates
                Console.WriteLine("Watching...");

                // Delay watching updates because it's not that serious yo.
                Thread.Sleep(500);
            }
        }

        // This yells at the user for trying to use whatever browser
        private static void YellAtUser(string s)
        {
            // Ommiting if they said the same browser last time, or if the process is
            // Microsoft Edge CP (because it say's it weird & twice for some reason
            // then don't speak.
            if (s != prev && s != "microsoftedgecp")
            {
                // Template message
                string msg = "Get off ov" + s + " & get back to work!";

                // Speak template message
                _sS.SpeakAsync(msg);

                // Remember the last browser you said
                prev = s;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            run = false;
            Application.Exit();
        }

        private void StopApp()
        {
            runToolStripMenuItem.Text = "Run App";
            notifyIcon1.ShowBalloonTip(msgTime, "Freedom!", "You may now open any internet browser that you'd like.", ToolTipIcon.Info);
            run = false;
            SetIconText();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (run)
            {
                StopApp();
            }
            else
            {
                RunApp();
            }
        }

        private void RunApp()
        {
            runToolStripMenuItem.Text = "Stop App";
            notifyIcon1.ShowBalloonTip(msgTime, "Time To Work!", "You will now be stopped from opening internet browser windows.", ToolTipIcon.Info);
            run = true;
            SetIconText();

            Thread checker = new Thread(new ThreadStart(GetBackToWork));
            checker.Start();

            Thread checker2 = new Thread(new ThreadStart(ResetPrev));
            checker2.Start();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
        }
    }
}
