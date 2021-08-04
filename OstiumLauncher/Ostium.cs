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
using System.Diagnostics;
using System.IO;

namespace OstiumLauncher
{
    class Ostium
    {
        static void Main(string[] args)
        {
            Console.Title = "Ostium Beta Testing Launcher";
            Console.WriteLine("Ostium Launcher  Copyright (C) 2021  OstiumDev\nThis program comes with ABSOLUTELY NO WARRANTY;\nThis is free software, and you are welcome to redistribute it\nunder certain conditions;\nFor more information please review our license here: https://github.com/OstiumDev/Launcher");
            Console.WriteLine("Injecting...");
            foreach (var process in Process.GetProcessesByName("anticheat.x64.equ8"))
            {
                process.Kill();
            }
            bool injectSuccess = Injector.InjectDllIntoProcess("PortalWars-Win64-Shipping", $@"{Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName)}\Ostium.dll", out int errorCode);
            if (!injectSuccess)
            {
                Console.WriteLine("Splitgate isn't open, make sure to have Splitgate running before trying to use Ostium!");
            }
            Console.ReadLine();
        }
    }
}
