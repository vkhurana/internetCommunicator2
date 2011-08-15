using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using InterComm;

namespace InterComm.Helpers
{
    public static class Logger
    {
        static readonly object _fileLock = new object();

        /// <summary>
        /// tell the logger about the uiElements
        /// </summary>
        /// <param name="form">ui interface</param>
        public static void Initialize(frmMain form)
        {
            _uiElements = form;
        }

        private static frmMain _uiElements;
        private static frmMain UI_Elements
        {
            get
            {
                if (_uiElements == null) throw new NullReferenceException("UI handle not set");
                else
                {
                    return _uiElements;
                }
            }
        }

        private static StreamWriter _writer;
        private static StreamWriter Writer
        {
            get
            {
                if (_writer == null) _writer = new StreamWriter(SessionSettings.logfile, true);
                return _writer;
            }
        }
        
        /// <summary>
        /// basic <timestamp> - <message> formatted log information.
        /// </summary>
        /// <param name="logInfo">the string to log</param>
        public static void LogInformation(string logInfo)
        {
            //lock (_fileLock)
            {
                string logLine = "[" + DateTime.Now.ToString() + "] - " + logInfo;
                Writer.WriteLine(logLine);
                //bug: UI_Elements.AddToListBox(logLine); todo(1)
                Writer.Flush();
            }
        }

        /// <summary>
        /// This logs the exception information, it also recursively traverses the innerexceptions,
        /// recording the message at each level.
        /// </summary>
        /// <param name="ex"></param>
        public static void LogException(Exception ex)
        {
            Exception recurException = ex;
            StringBuilder sb = new StringBuilder();
            sb.Append(ex.Message);

            while (recurException.InnerException != null)
            {
                recurException = recurException.InnerException;
                sb.Append("\n~~~~~~~~~~~~~~~~~~~\n");
                sb.Append(recurException.Message);
            }
            LogInformation(sb.ToString());
        }
    }
}
