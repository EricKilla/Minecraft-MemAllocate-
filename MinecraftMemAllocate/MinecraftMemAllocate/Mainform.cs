using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace MinecraftMemAllocate
{
    public partial class Mainform : Form
    {
        public string Path;
        public string Folder;
        public float Mem;
        public int MemMin = 500;
        public int MemMax = 1024;
        string AppPath = System.IO.Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.ApplicationData)) + "/MMA";
        string ConfigFile = "MMAData.txt";
        
        public Mainform()
        {
            InitializeComponent();
            Mem = GetTotalMemoryInBytes() / 1048576;//ToMB
            textBox5.Text = Mem.ToString() + "MB";
            readAppConfig();
            textBox3.Text = MemMin.ToString();
            textBox4.Text = MemMax.ToString();
        }

        public void readAppConfig()
        {
            IEnumerable<string> data = File.ReadLines(AppPath + "/" + ConfigFile);
            textBox1.Text = data.ElementAt(0);
            textBox3.Text = data.ElementAt(1);
            textBox4.Text = data.ElementAt(2);
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

        static ulong GetTotalMemoryInBytes()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FindFile();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<string> contents = new List<string>();
            if (!Directory.Exists(AppPath)){
                Directory.CreateDirectory(AppPath );
            }
            if (File.Exists(AppPath + "/" + ConfigFile))
            {
                File.Delete(AppPath + "/" + ConfigFile);
            }
            contents.Add(Path);
            contents.Add(MemMin.ToString());
            contents.Add(MemMax.ToString());
            File.WriteAllLines(AppPath+"/"+ConfigFile, contents);
            OpenCmdWithCmds();
        }

        public void OpenCmdWithCmds()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C java " + "-Xms" + MemMin + "m" + " -Xmx" + MemMax + "m -jar " + "\""+Path+"\"";
            process.StartInfo = startInfo;
            process.Start();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            MemMin = int.Parse(this.textBox3.Text);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            MemMin = int.Parse(this.textBox4.Text);
        }
    }
    

}
