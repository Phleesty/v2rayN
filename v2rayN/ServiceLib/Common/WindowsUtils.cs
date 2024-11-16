using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace ServiceLib.Common
{
    internal static class WindowsUtils
    {
        public static string? RegReadValue(string path, string name, string def)
        {
            RegistryKey? regKey = null;
            try
            {
                regKey = Registry.CurrentUser.OpenSubKey(path, false);
                var value = regKey?.GetValue(name) as string;
                return Utils.IsNullOrEmpty(value) ? def : value;
            }
            catch (Exception ex)
            {
                Logging.SaveLog(ex.Message, ex);
            }
            finally
            {
                regKey?.Close();
            }
            return def;
        }

        public static void RegWriteValue(string path, string name, object value)
        {
            RegistryKey? regKey = null;
            try
            {
                regKey = Registry.CurrentUser.CreateSubKey(path);
                if (Utils.IsNullOrEmpty(value.ToString()))
                {
                    regKey?.DeleteValue(name, false);
                }
                else
                {
                    regKey?.SetValue(name, value);
                }
            }
            catch (Exception ex)
            {
                Logging.SaveLog(ex.Message, ex);
            }
            finally
            {
                regKey?.Close();
            }
        }

        public static void RemoveTunDevice()
        {
            try
            {
                var sum = MD5.HashData(Encoding.UTF8.GetBytes("wintunsingbox_tun"));
                var guid = new Guid(sum);
                string pnputilPath = @"C:\Windows\System32\pnputil.exe";
                string arg = $$""" /remove-device  "SWD\Wintun\{{{guid}}}" """;

                // Try to remove the device
                Process proc = new()
                {
                    StartInfo = new()
                    {
                        FileName = pnputilPath,
                        Arguments = arg,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                proc.Start();
                var output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
            }
            catch
            {
            }
        }

    }
}