using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace FilesCompare.CompareHelper
{
    class FileTypeRelative
    {
        public void Relative(string type)
        {
            if (null != Registry.ClassesRoot.OpenSubKey(type))
                return;

            WindowsIdentity wi = WindowsIdentity.GetCurrent();
            WindowsPrincipal wp = new WindowsPrincipal(wi);

            if (!wp.IsInRole(WindowsBuiltInRole.Administrator))
            {
                System.Diagnostics.ProcessStartInfo start = new
                System.Diagnostics.ProcessStartInfo();
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                start.WorkingDirectory = System.Windows.Forms.Application.StartupPath;
                start.Verb = "runas";
                start.FileName = System.Windows.Forms.Application.ExecutablePath;
                System.Diagnostics.Process.Start(start);
                Application.Exit();
                return;
            }

            RegistryKey key = Registry.ClassesRoot.OpenSubKey(type);
            if (key == null)
            {
                key = Registry.ClassesRoot.CreateSubKey(type);
                key.SetValue("", "Jeebook.Reader.jb");
                key.SetValue("Content Type", "application/jb");

                key = Registry.ClassesRoot.CreateSubKey("Jeebook.Reader.jb");
                key.SetValue("", "Jeebook Document");

                RegistryKey keySub = key.CreateSubKey("DefaultIcon");
                keySub.SetValue("", System.Windows.Forms.Application.StartupPath + "Jeebook.ico");
                keySub = key.CreateSubKey("shell//open//command");
                keySub.SetValue("", "/" + System.Windows.Forms.Application.ExecutablePath + @"//%1/");
            }
        }
    }
}
