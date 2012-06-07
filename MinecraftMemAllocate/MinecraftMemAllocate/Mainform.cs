using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualBasic.Devices;

namespace MinecraftMemAllocate
{
    public partial class Mainform : Form
    {
        string Path;
        float Mem;
        int MemMin = 500;
        int MemMax = 1024;
        object xmlLock = new object();
        string AppPath = System.IO.Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.ApplicationData)) + "/MMA";
        string ConfigFile = "Config.xml";
        string javadir;

        public Mainform()
        {
            InitializeComponent();
            Mem = GetTotalMemoryInBytes() / 1048576;//ToMB
            textBox5.Text = Mem.ToString() + "MB";
            readAppConfig();
            LocateJava();
        }

        void LocateJava()
        {
            String path = Environment.GetEnvironmentVariable("path");
            String[] folders = path.Split(';');
            foreach (String folder in folders)
            {
                if (File.Exists(folder + "java.exe"))
                {
                    this.javadir = folder + "/java.exe";
                    return;
                }
                else if (File.Exists(folder + "\\java.exe"))
                {
                    this.javadir = folder + "/java.exe";
                    return;
                }
            }
            if (javadir == null)
            {
                MessageBox.Show("Unable to find your Java.exe (Not good)");
            }
        }

        bool _myTextBoxChanging = false;
        void validateText(TextBox box)
        {
            // stop multiple changes;
            if (_myTextBoxChanging)
                return;
            _myTextBoxChanging = true;

            string text = box.Text;
            if(box.Text.Length == 0)
                return;
            string validText = "";
            int pos = box.SelectionStart;
            for (int i = 0; i < text.Length; i++)
            {
                bool badChar = false;
                char s = text[i];
                if (s == '.')
                {
                    badChar = true;
                }
                else if (s < '0' || s > '9')
                    badChar = true;

                if (!badChar)
                    validText += s;
                else
                {
                    if (i <= pos)
                        pos--;
                }
            }
            while (validText.Length >= 2 && validText[0] == '0')
            {
                if (validText[1] != '.')
                {
                    validText = validText.Substring(1);
                    if (pos < 2)
                        pos--;
                }
                else
                    break;
            }

            if (pos > validText.Length)
                pos = validText.Length;
            box.Text = validText;
            box.SelectionStart = pos;
            _myTextBoxChanging = false;
        }

        public void readAppConfig()
        {
            lock (xmlLock)
            {
                if (File.Exists(AppPath + "/" + ConfigFile))
                {
                    XmlTextReader xmlReader = new XmlTextReader(AppPath + "/" + ConfigFile);
                    while (xmlReader.Read())
                    {
                        if (xmlReader.Name == "GeneralSettings")
                        {
                            Path = xmlReader.GetAttribute("Path").ToString(); textBox1.Text = Path;
                            MemMin = Int32.Parse(xmlReader.GetAttribute("MemMin")); textBox3.Text = MemMin.ToString();
                            MemMax = Int32.Parse(xmlReader.GetAttribute("MemMax")); textBox4.Text = MemMax.ToString();
                        }
                    }
                    xmlReader.Close();
                }
            }
        }

        void FindFile()
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
            return new ComputerInfo().AvailablePhysicalMemory;
        }

        void button1_Click(object sender, EventArgs e)
        {
            FindFile();
        }

        void button3_Click(object sender, EventArgs e)
        {
            SaveConfig();
            double Num;
            bool isNum = double.TryParse(textBox3.Text.Trim(), out Num);
            if (!isNum)
            {
                MessageBox.Show("Invalid number at Minimum Memory");
                return;
            }
            isNum = double.TryParse(textBox4.Text.Trim(), out Num);
            if (!isNum)
            {
                MessageBox.Show("Invalid number at Maximum Memory");
                return;
            }
            if (MemMin < 10)
            {
                MessageBox.Show("Minimum Memory cannot be less than 10mb");
                return;
            }
            if (MemMin > MemMax)
            {
                MessageBox.Show("Minimum Memory cannot exceed the maximum");
                return;
            }
            if (MemMax > Mem || MemMin > Mem)
            {
                MessageBox.Show("Memory values cannot exceed available RAM");
                return;
            }
            OpenCmdWithCmds();
            label2.Visible = true;
        }

        void SaveConfig()
        {
            lock (xmlLock)
            {
                if (!Directory.Exists(AppPath))
                {
                    Directory.CreateDirectory(AppPath);
                }
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                XmlWriter writer = XmlWriter.Create(AppPath + "/" + ConfigFile, settings);
                writer.WriteStartDocument();
                writer.WriteComment("This file is generated by the program.");
                writer.WriteStartElement("GeneralSettings");
                writer.WriteAttributeString("Path", Path);
                writer.WriteAttributeString("MemMin", MemMin.ToString());
                writer.WriteAttributeString("MemMax", MemMax.ToString());
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
            }
        }

        void OpenCmdWithCmds()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C " + javadir + " " + " -Xmx" + MemMax + "m -Xms" + MemMin + "m" + " -jar " + "\"" + Path + "\"";
            //MessageBox.Show(startInfo.Arguments);
            process.StartInfo = startInfo;
            process.Start();
        }

        void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.TextLength > 0)
            {
                validateText(textBox3);
                MemMin = int.Parse(this.textBox3.Text);
            }
        }

        void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.TextLength > 0)
            {
                validateText(textBox4);
                MemMax = int.Parse(this.textBox4.Text);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
