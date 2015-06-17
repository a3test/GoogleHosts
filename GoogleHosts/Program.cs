using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;

namespace GoogleHosts
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length > 0 && args[0].ToLower() == "service") //service
            {
                ServiceBase[] ServicesToRun = { new ServiceMain() };
                ServiceBase.Run(ServicesToRun);
            }
            else //console
            {
                const string serviceName = "GoogleHostsUpdate";
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.RedirectStandardOutput = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.UseShellExecute = false;
                Process p;

                Console.WriteLine("-----------服务管理程序，请输入对应的数字-----------");
                Console.WriteLine("[1]安装服务");
                Console.WriteLine("[2]启动服务");
                Console.WriteLine("[3]停止服务");
                Console.WriteLine("[4]卸载服务");
                Console.WriteLine("[5]退出");
                string rl = Console.ReadLine();
                int i;
                if (int.TryParse(rl, out i))
                {
                    switch (i)
                    {
                        case 1:
                            string path = Process.GetCurrentProcess().MainModule.FileName + " service";
                            const string displayName = "Google Hosts Update";
                            psi.FileName = "sc";
                            psi.Arguments = string.Format("create {0} binpath= \"{1}\" displayName= \"{2}\" start= auto", serviceName, path, displayName);
                            p = Process.Start(psi);
                            using (StreamReader sr = p.StandardOutput)
                            {
                                p.WaitForExit();
                                if (p.HasExited)
                                {
                                    Console.WriteLine(sr.ReadToEnd());
                                }
                            }
                            break;
                        case 2:
                            psi.FileName = "net";
                            psi.Arguments = string.Format("start {0}", serviceName);
                            p = Process.Start(psi);
                            using (StreamReader sr = p.StandardOutput)
                            {
                                p.WaitForExit();
                                if (p.HasExited)
                                {
                                    Console.WriteLine(sr.ReadToEnd());
                                }
                            }
                            break;
                        case 3:
                            psi.FileName = "net";
                            psi.Arguments = string.Format("stop {0}", serviceName);
                            p = Process.Start(psi);
                            using (StreamReader sr = p.StandardOutput)
                            {
                                p.WaitForExit();
                                if (p.HasExited)
                                {
                                    Console.WriteLine(sr.ReadToEnd());
                                }
                            }
                            break;
                        case 4:
                            psi.FileName = "sc";
                            psi.Arguments = string.Format("delete {0}", serviceName);
                            p = Process.Start(psi);
                            using (StreamReader sr = p.StandardOutput)
                            {
                                p.WaitForExit();
                                if (p.HasExited)
                                {
                                    Console.WriteLine(sr.ReadToEnd());
                                }
                            }
                            break;
                        case 5:
                            return;
                        default:
                            Console.WriteLine("无法识别的输入\r\n");
                            break;
                    }
                    Main(new[] {""});
                }
                else
                {
                    Console.WriteLine("无法识别的输入\r\n");
                }
            }
        }
    }
}