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
        public string Folder;
        public float Mem;
        public Mainform()
        {
            InitializeComponent();
            Mem = GetTotalMemoryInBytes() / 1048576000; //GB?
        }
        public void FindFile()
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Minecraft.exe finder";
            fdlg.InitialDirectory = @"c:\";
            fdlg.Filter = "Minecraft.exe|Minecraft.exe";
            fdlg.FilterIndex = 1;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                Path = fdlg.FileName;
                textBox1.Text = fdlg.FileName;
            }
        }
        public void FindFolder()
        {
            FolderBrowserDialog fdlg = new FolderBrowserDialog();
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                Folder = fdlg.SelectedPath;
                textBox2.Text = fdlg.SelectedPath;
            }
        }

        static ulong GetTotalMemoryInBytes()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FindFile();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FindFolder();
        }
    }
    

}
