using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace USBChecker
{
    public partial class Configuration : Form
    {
        public Configuration()
        {
            InitializeComponent();

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version_minor = fvi.ProductMinorPart.ToString();
            string version_major = fvi.FileMajorPart.ToString();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        /*
private void LoadSettings(object sender, EventArgs e)
{
  //showMessageCheckBox.Checked = TaskTrayApplication.Properties.Settings.Default.ShowMessage;
}

private void SaveSettings(object sender, FormClosingEventArgs e)
{
  // If the user clicked "Save"
  if (this.DialogResult == DialogResult.OK)
  {
      //  TaskTrayApplication.Properties.Settings.Default.ShowMessage = showMessageCheckBox.Checked;
      KioskCheckerTray.Properties.Settings.Default.Save();
  }
}

private void Configuration_Load(object sender, EventArgs e)
{

}
*/
    }
}