using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenYS
{
    public partial class SettingsGUI_Form : Form
    {
        public SettingsGUI_Form()
        {
            InitializeComponent();
        }

        private void Reload_Click(object sender, EventArgs e)
        {
            SettingsGUI.Load();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            SettingsGUI.Save();
        }
    }
}
