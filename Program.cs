// This program accepts a parameter for either "on" or "off" to toggle the screensaver
// "off" will disable the screensaver
// "on" will turn on the blank screensaver

using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ConsoleSetScreenSaver
{
    class Program
    {
        const int SPI_SETSCREENSAVEACTIVE = 0x0011;

        [DllImport("user32", CharSet = CharSet.Auto)]
        unsafe public static extern short SystemParametersInfo(int uiAction, int uiParam, int* pvParam, int fWinIni);

        const string KEY_SCREENSAVER = "SCRNSAVE.EXE";
        const string KEY_SCREENSAVER_ACTIVE = "ScreenSaveActive";
        const string BLANK_SCREENSAVER = @"C:\Windows\system32\scrnsave.scr";
        static void Main(string[] args)
        {
            try
            {
                RegistryKey oKey = Registry.CurrentUser.OpenSubKey("Control Panel",
                true);
                oKey = oKey.OpenSubKey("desktop", true);
                if (null != oKey)
                {
                    string screensaver = (string)oKey.GetValue(KEY_SCREENSAVER);
                    if (null == screensaver)
                    {
                        screensaver = "(null)";
                    }

                    string active = (string)oKey.GetValue(KEY_SCREENSAVER_ACTIVE);
                    if (null == active)
                    {
                        active = "(null)";
                    }

                    Console.WriteLine("Screen Saver: {0}", screensaver);
                    Console.WriteLine("Active: {0}", active);

                    //off
                    //Screen Saver: (null)
                    //Active: 1

                    //on
                    //Screen Saver: C:\Windows\system32\scrnsave.scr
                    //Active: 1

                    bool enable = false;
                    if (args.Length > 0)
                    {
                        switch (args[0])
                        {
                            case "on":
                                enable = true;
                                break;

                            case "off":
                                enable = false;
                                break;

                            default:
                                return;
                        }
                    }
                    else
                    {
                        return;
                    }

                    if (enable)
                    {
                        oKey.SetValue(KEY_SCREENSAVER, BLANK_SCREENSAVER);
                    }
                    else
                    {
                        oKey.DeleteValue(KEY_SCREENSAVER);
                    }

                    oKey.SetValue(KEY_SCREENSAVER_ACTIVE, "1");

                    unsafe
                    {
                        int nX = 1;
                        SystemParametersInfo(
                        SPI_SETSCREENSAVEACTIVE,
                        0,
                        &nX,
                        0
                        );
                    }

                    oKey.Close();

                    if (enable)
                    {
                        ProcessStartInfo psi = new ProcessStartInfo(BLANK_SCREENSAVER, string.Empty);
                        using (Process p = new Process())
                        {
                            p.StartInfo = psi;
                            p.Start();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }
    }
}
