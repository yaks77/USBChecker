using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using UsbEject.Library;
using System.Security.Principal;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Net;
using System.Net.NetworkInformation;

namespace USBChecker
{
    public class TaskTrayApplicationContext : ApplicationContext
    {
        NotifyIcon _notifyIcon = new NotifyIcon();
        public Configuration _configWindow = new Configuration();
       // private DriveDetector _driveDetector;
        private int _timeRange = 72; // default time range set to 72 hours
        private DateTime _deadline;
        private bool verbose;
        private Timer _timer1;
        Queue<string> _drivesToEject = new Queue<string>();

        public void InitTimer()
        {
            _timer1 = new Timer();
            _timer1.Tick += new EventHandler(timer1_Tick);
            _timer1.Interval = 5000; // in miliseconds
            _timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //
            // Quick search for already plugged USB devices before KioskChecker starts running
            //
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Removable)
                {
                    _configWindow.listBox1.Items.Add("Found a USB drive in " + drive.Name);
                    if (!_drivesToEject.Contains(drive.Name)) // just in case
                        checkTrustStamp(drive.Name);
                }
            }

            // Loop through queue.
            string letter = "";
            while (_drivesToEject.Count >0)
            {
                letter = _drivesToEject.Dequeue();

                VolumeDeviceClass volumeDeviceClass = new VolumeDeviceClass();
                foreach (Volume device in volumeDeviceClass.Devices)
                {
                    // is this volume on USB disks?
                    if (!device.IsUsb)
                        continue;

                    // is this volume a logical disk?
                    if ((device.LogicalDrive == null) || (device.LogicalDrive.Length == 0))
                        continue;

                    if (string.CompareOrdinal(device.LogicalDrive, letter.ToUpper().Replace("\\", "")) == 0)
                    {

                        string resultado = device.Eject(false); // allow Windows to display any relevant UI
                        // Just add report to the listbox
                        string s = "Drive " + letter + " automatically ejected.";
                        _configWindow.listBox1.Items.Add(s);
                        break;

                    }
                }

            }
        }
        
        public TaskTrayApplicationContext(int i_timeRange, DateTime d_deadline,bool b_verbose)
        {
            _timeRange = i_timeRange;
            _deadline = d_deadline;
            verbose = b_verbose;

            InitTimer();

         //   _driveDetector = new DriveDetector();
         //   _driveDetector.DeviceArrived += new DriveDetectorEventHandler(OnDriveArrived);
         //   _driveDetector.DeviceRemoved += new DriveDetectorEventHandler(OnDriveRemoved);

            _notifyIcon.Icon = USBChecker.Properties.Resources.AppIcon;
            _notifyIcon.DoubleClick += new EventHandler(ShowConfig);
            _notifyIcon.MouseMove += new MouseEventHandler(_notifyIcon1_MouseMove);
            _notifyIcon.Visible = true;
            _notifyIcon.BalloonTipTitle = "USB Checker";
            
            // Not contextual menu, chosen shortcut key instead
            // MenuItem configMenuItem = new MenuItem("Configuration", new EventHandler(ShowConfig));
            // MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));
            //  _notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { configMenuItem, exitMenuItem });
            
            // Show in the LOG window the timeRange chosen
            _configWindow.s_labelTimeRange.Text = _timeRange.ToString();
            
            // DEADLINE advisory
            if (DateTime.Today < _deadline)
                ShowMessageOnly("USB Checker is setup to start blocking non-sanitize USB devices on " + _deadline.ToShortDateString() + ". After " + (_deadline - DateTime.Today).TotalDays + " days, you will need to scan any USB mass storage device at least once every " + _timeRange + " hours." );
            else
                ShowMessageOnly("USB Checker has started and it will block any USB mass storage devices not sanitazed for the last " + i_timeRange + " hours. For more information, please contact your local IT support.");
        }
        


        private void _notifyIcon1_MouseMove(Object sender, MouseEventArgs e)  
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version_minor = fvi.ProductMinorPart.ToString();
            string version_major =  fvi.FileMajorPart.ToString();
            _notifyIcon.Text = "USB Checker " + version_major + "." + version_minor ;
        }


        private string getNameDrive(string s_drive)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.Name == s_drive)
                {
                    return drive.VolumeLabel;
                }
            }
            return "NOT FOUND";
        }

        /*private void OnDriveArrived(object sender, DriveDetectorEventArgs e)
        {
            // Report the event in the listbox.
            // e.Drive is the drive letter for the device which just arrived, e.g. "E:\\"
            _configWindow.listBox1.Items.Add("------------------------------------------------------------------------------------------------------------------------------------------------");
            string s = "Drive arrived " + e.Drive;
            _configWindow.listBox1.Items.Add(s);
            _configWindow.listBox1.TopIndex = _configWindow.listBox1.Items.Count - 1;

            checkTrustStamp(e.Drive);


        }*/

        private void checkTrustStamp(string s_drive, bool showAll = true)
        {

            string stamp = "STAMP.kiosk";
            // check the existence of the file in root STAMP.kiosk

            if (!File.Exists(s_drive + stamp))
            {
                _configWindow.listBox1.Items.Add(s_drive + stamp + " does NOT exist.");
                ShowMessageAndEject(s_drive, "Your USB storage device '"+ getNameDrive(s_drive) + "' does not contain the correct stamp, it is considered NOT SAFE and will be ejected.");
            }
         // More other checks should be added here....I removed mine here.

        }


        static string GetSha1Hash(SHA1 sha1Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = sha1Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        private void ShowMessageAndEject(string s_drive, string message)
        {
               if (DateTime.Today < _deadline)
                {
                    message = "This USB '" + getNameDrive(s_drive) + "' has not been sanitized using an USB Checker station, but you can still use it normally. After " + (_deadline - DateTime.Today).TotalDays + " days, in order to use a USB mass storage, you will need to scan the device at least once every " + _timeRange + " hours.";
                    _notifyIcon.BalloonTipIcon = ToolTipIcon.Warning;
                }
                else
                {
                    _drivesToEject.Enqueue(s_drive.ToUpper());
                    
                    _notifyIcon.BalloonTipIcon = ToolTipIcon.Error;
                }

                _notifyIcon.ShowBalloonTip(8000);
                // _notifyIcon.Dispose();
        }

        private void ShowMessageOnly(string message)
        {


            _notifyIcon.Visible = true;
            //_notifyIcon.Icon = SystemIcons.Shield;
            _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            _notifyIcon.ShowBalloonTip(8000);

        }

        // Called by DriveDetector after removable device has been unpluged 
   /*     private void OnDriveRemoved(object sender, DriveDetectorEventArgs e)
        {
            // TODO: do clean up here, etc. Letter of the removed drive is in e.Drive;

            // Just add report to the listbox
            string s = "Drive removed " + e.Drive;
            _configWindow.listBox1.Items.Add(s);
        }
        */

    void ShowConfig(object sender, EventArgs e)
        {
            // If we are already showing the window meerly focus it.
            if (_configWindow.Visible)
                _configWindow.Focus();
            else
            {
                if (Control.ModifierKeys == Keys.Control)
                    _configWindow.ShowDialog();
            }
        }



        void Exit(object sender, EventArgs e)
        {
            // We must manually tidy up and remove the icon before we exit.
            // Otherwise it will be left behind until the user mouses over.
            _notifyIcon.Visible = false;

            Application.Exit();
        }

      
    }
}
