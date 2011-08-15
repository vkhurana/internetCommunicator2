using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InterComm.Helpers;
using InterComm;

namespace InterComm
{
    public partial class frmMain : Form
    {
        BotEngine Engine;
        
        ~frmMain()
        {
            trayIcon.Visible = false;
        }

        public frmMain()
        {
            InitializeComponent();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void Main_Load(object sender, EventArgs e)
        {
            //place the window in the corner
            InitializeUI();
            Logger.Initialize(this);
            Engine = new BotEngine(this);
            Engine.Initialize();
        }

        private void InitializeUI()
        {
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            int spacing = 5;
            this.Location = new Point(
                                workingArea.Width - this.Width - spacing,
                                workingArea.Height - this.Height - spacing
                                );
            botLabel.Text = "000";
            //showToolStripMenuItem_Click(null, null); //hide the main window
        }

        public void UpdateStatusInfo(string text)
        {
            toolStripStatusLabel.Text = text;
        }

        public void AddToListBox(string str)
        {
            {
                logList.Items.Insert(0, str);
            }
            //this.Refresh();
        }

        private void loggerBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void logList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Visible == true)
            {
                trayIconContextMenu.Items[0].Text = "Show";
                this.Visible = false;
                this.ShowInTaskbar = false;
            }
            else
            {
                trayIconContextMenu.Items[0].Text = "Hide";
                this.Visible = true;
                this.ShowInTaskbar = true;
            }
        }
    }
}
