using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.DirectoryServices.ActiveDirectory;
using System.Net;


namespace USBChecker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            // Control to dont run the checker twice
            string procName = Process.GetCurrentProcess().ProcessName;
            // get the list of all processes by the "procName"       
            Process[] processes = Process.GetProcessesByName(procName);

            if (processes.Length > 1)
            {
                // MessageBox.Show(procName + " already running");
                return;
            }


            Regex cmdRegEx = new Regex(@"/(?<name>.+?):(?<val>.+)");

            Dictionary<string, string> cmdArgs = new Dictionary<string, string>();
            foreach (string s in args)
            {
                Match m = cmdRegEx.Match(s);
                if (m.Success)
                {
                    cmdArgs.Add(m.Groups[1].Value.ToUpper(), m.Groups[2].Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in cmdArgs)
            {
                //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }

            // Extract the TIME RANGE value  DEFAULT value = 72
            string s_timeRange;
            int i_timeRange;
            if (!cmdArgs.TryGetValue("TIMERANGE", out s_timeRange))
                i_timeRange = 72;
            else // check if the value is a integer
            {

                if (Int32.TryParse(s_timeRange, out i_timeRange))
                    Console.WriteLine(i_timeRange);
                else
                    i_timeRange = 72;
            }
            // Extract the VISIBLE value    DEFAULT = false
            string s_visibleExec;
            bool b_visibleExec;
            if (!cmdArgs.TryGetValue("VISIBLE", out s_visibleExec))
                b_visibleExec = false;
            else // check if the value is a bool
            {
                if (Boolean.TryParse(s_visibleExec, out b_visibleExec))
                    Console.WriteLine(b_visibleExec);
                else
                    b_visibleExec = false;
            }

            // Extract the DEADLINE value    DEFAULT = DateTime.Today
            string s_deadline;
            DateTime d_deadline;
            if (!cmdArgs.TryGetValue("DEADLINE", out s_deadline))
                d_deadline = DateTime.Today;
            else // check if the value is a DateTime
            {
                if (DateTime.TryParse(s_deadline, out d_deadline))
                    Console.WriteLine(d_deadline);
                else
                    d_deadline = DateTime.Today;
            }

            // Extract the VERBOSE value    DEFAULT = true
            string s_verbose;
            bool b_verbose;
            if (!cmdArgs.TryGetValue("VERBOSE", out s_verbose))
                b_verbose = true;
            else // check if the value is a bool
            {
                if (Boolean.TryParse(s_verbose, out b_verbose))
                    Console.WriteLine(b_verbose);
                else
                    b_verbose = true;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Instead of running a form, we run an ApplicationContext.
            TaskTrayApplicationContext task_Kiosk = new TaskTrayApplicationContext(i_timeRange, d_deadline, b_verbose);
            
            Application.Run(task_Kiosk);



            
        }
    }
}