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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OstiumLauncher.Program
{
    class util
    {
        public static void injectPortalWars()
        {
            downloadDll();
            // temp sleep until dll injects on main menu
            Thread.Sleep(25000);
            foreach (var process in Process.GetProcessesByName("anticheat.x64.equ8"))
            {
                process.Kill();
            }
            bool injectSuccess = Injector.InjectDllIntoProcess("PortalWars-Win64-Shipping", $@"{Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}\Ostium.dll", out int errorCode);
            if (!injectSuccess)
            {
                MessageBox.Show("Injection failed please retry.", "Ostium - Error");
            }
        }

        private static void downloadDll()
        {
            // download dll and save it in temp folder under /Content/Ostium.dll
            // https://github.com/OstiumDev/Ostium/releases/latest/download/Ostium.dll
            try
            {
                WebClient Client = new WebClient();
                Client.DownloadFile("https://github.com/OstiumDev/Ostium/releases/download/DLL/Ostium.dll", $@"{Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}\Ostium.dll");
            } 
            catch (WebException we)
            {
                MessageBox.Show(we.ToString());
            }
        }
    }
}
