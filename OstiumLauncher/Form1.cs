/*
 * Ostium Launcher, Injects a DLL into another program
 * Copyright (C) 2021 OstiumDev
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OstiumLauncher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
            OpenFileDialog folderBrowser = new OpenFileDialog();
            folderBrowser.ValidateNames = false;
            folderBrowser.CheckFileExists = false;
            folderBrowser.CheckPathExists = true;
            folderBrowser.FileName = "Folder Selection.";
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                string folderPath = Path.GetDirectoryName(folderBrowser.FileName);
                textBox1.Text = folderPath;
            }
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("steam://rungameid/677620") { UseShellExecute = true });
            this.Hide();
            Thread.Sleep(25000);
            foreach (var process in Process.GetProcessesByName("anticheat.x64.equ8"))
            {
                process.Kill();
            }
            bool injectSuccess = Injector.InjectDllIntoProcess("PortalWars-Win64-Shipping", $@"{Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName)}\Ostium.dll", out int errorCode);
            if (!injectSuccess)
            {
                Console.WriteLine("Injection failed please retry.");
            }
            Environment.Exit(0);
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            if(File.Exists($"{textBox1.Text}\\PortalWars.exe"))
            {
                this.Hide();
                Process.Start($"{textBox1.Text}\\PortalWars.exe", "-log");
            }
            else
            {
                MessageBox.Show("Unable to find PortalWars.exe please make sure you are in the right game directory.", "Ostium - Error");
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://discord.gg/R9UHJ4M9QW") { UseShellExecute = true });
        }
    }
}
