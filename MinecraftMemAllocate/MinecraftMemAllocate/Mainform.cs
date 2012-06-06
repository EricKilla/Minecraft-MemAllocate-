using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MinecraftMemAllocate
{
    public partial class Mainform : Form
    {
        public string Path;
        public Mainform()
        {
            InitializeComponent();
        }
        public void FindFile()
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Minecraft.exe finder";
            fdlg.InitialDirectory = @"c:\";
            fdlg.Filter = "Minecraft.exe|Minecraft.exe";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                Path = fdlg.FileName;
                textBox1.Text = fdlg.FileName;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FindFile();
        }
    }
    

}
