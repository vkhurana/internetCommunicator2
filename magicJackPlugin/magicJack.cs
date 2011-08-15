using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginSDK;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace magicJackPlugin
{
    [StatelessChatPluginAttribute("manages the magic jack process")]
    class magicJack: IStatelessChatInterface
    {
        private const string helpfile = "plugins\\magicJackFiles\\commandsHelp.txt";
        private const string startupPathFile = "plugins\\magicJackFiles\\startupPath.ini";
        
        private string magicJackStartupFileName
        {
            get
            {
                if (File.Exists(startupPathFile))
                {
                    string path = File.ReadAllText(startupPathFile);
                    path = path.Trim();
                    if (File.Exists(path))
                    {
                        return path;
                    }
                    else
                    {
                        MessageBox.Show("Invalid file path in " + startupPathFile + "!\nThe file does not exist", "Error!",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                }
                return @"C:\Documents and Settings\vkhurana\Application Data\mjusbsp\magicJackLoader.exe";
            }
        }

        private Process GetMagicJackProcess()
        {
            Process mJ = null;
            try
            {
                mJ = System.Diagnostics.Process.GetProcessesByName("magicjack").FirstOrDefault();
            }
            catch { mJ = null; }
            return mJ;
        }

        private void KillProcess(Process p)
        {
            p.Kill();
        }

        private Process StartProcess(string path)
        {
            Process p = new Process();
            p.StartInfo.FileName = path;
            p.Start();
            return p;
        }

        public string GetCommandResult(string cmdArgs)
        {
            string returnMessage = "null";
            string command = cmdArgs;
            command = command.Trim();
            command = command.ToLower();
            switch (command)
            {
                case "start":
                    {
                        Process mJackOld = GetMagicJackProcess();
                        if (mJackOld == null)
                        {
                            try
                            {
                                Process mJack = StartProcess(magicJackStartupFileName);
                                returnMessage = "Process started... [" + mJack.Id + "]";
                            }
                            catch (Exception ex)
                            {
                                returnMessage = "Unable to start process... (" + ex.Message + ")";
                            }
                        }
                        else
                        {
                            returnMessage = "Process already running! (try restart)";
                        }
                        break;
                    }
                case "kill":
                    {
                        Process mJackOld = GetMagicJackProcess();
                        if (mJackOld != null)
                        {
                            //its running, kill it!
                            try
                            {
                                KillProcess(mJackOld);
                                returnMessage = "Process successfully killed.";
                            }
                            catch
                            {
                                returnMessage = "Error killing process!";
                                break;
                            }
                        }
                        else
                        {
                            returnMessage = "Process not running!";
                        }
                        break;
                    }
                case "restart":
                    {
                        Process mJackOld = GetMagicJackProcess();
                        if (mJackOld == null)
                        {
                            returnMessage = "Process not running! (try start)";
                        }
                        else
                        {
                            //its running, lets kill it!
                            try
                            {
                                KillProcess(mJackOld);
                            }
                            catch (Exception ex)
                            {
                                returnMessage = "Error killing process! (" + ex.Message + ")";
                            }
                            //killed, now lets start it
                            try
                            {
                                Process mJack = StartProcess(magicJackStartupFileName);
                                returnMessage = "Process restarted... [" + mJack.Id + "]";
                            }
                            catch (Exception ex)
                            {
                                returnMessage = "Unable to restart process... (" + ex.Message + ")";
                            }
                        }
                        break;
                    }
                case "status":
                    {
                        Process mJ = GetMagicJackProcess();
                        if (mJ != null)
                        {
                            StringBuilder sb = new StringBuilder("magicJack Process running...");
                            sb.Append("\nPID=" + mJ.Id.ToString());
                            sb.Append("\nStartTime=" + mJ.StartTime.ToString());
                            sb.Append("\nProcess=" + mJ.MainModule.FileName);
                            sb.Append("\nTitle=" + mJ.MainWindowTitle);
                            sb.Append("\nMemUsage=" + mJ.WorkingSet64);
                            sb.Append("\nProcessorTime=" +
                                        mJ.TotalProcessorTime.Days + ":" +
                                        mJ.TotalProcessorTime.Hours + ":" +
                                        mJ.TotalProcessorTime.Minutes + ":" +
                                        mJ.TotalProcessorTime.Seconds + ":" +
                                        mJ.TotalProcessorTime.Milliseconds);
                            returnMessage = sb.ToString();
                        }
                        else
                        {
                            returnMessage = "Process not running!";
                        }
                        
                        break;
                    }
                default:
                    {
                        try
                        {
                            returnMessage = File.ReadAllText(helpfile);
                        }
                        catch
                        {
                            returnMessage = "Invalid Command!\nHelp file not found!";
                        }
                        break;
                    }
            }
            return returnMessage;
        }
        public string GetCommandTrigger()
        {
            return "magicJack";
        }
    }
}
