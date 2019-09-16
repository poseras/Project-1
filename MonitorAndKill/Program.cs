using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Diagnostics;
using System.Threading;
using Xceed.Wpf.Toolkit;
using Amazon.Lambda;
using System.IO;

namespace MonitorAndKill
{
    class Program
    {
        static void Main(string[] args)
        {
            // Show all processes on the local computer.
            Process[] processes = Process.GetProcesses();
            // Display count.
            Console.WriteLine("Count: {0}", processes.Length);
            // Loop over processes.
            foreach (Process process in processes)
            {
                Console.WriteLine(process.Id);
                Console.WriteLine(process.ProcessName);   
            }

            // Omit the exe part.
            Console.WriteLine("Enter the name of the process you want to kill and press enter");
            string toKill = Console.ReadLine();
            Console.WriteLine("Enter time of execution in minutes that this porcess should be killed and press enter");
            string strMinutes = Console.ReadLine();
            Console.WriteLine("Enter monitoring frequency in minutes");
            string strFrequency = Console.ReadLine();
            double minutes = 0;
            double cFrequency = 0;
            //validate minutes input
            if (!Double.TryParse(strMinutes, out minutes))
            {
                Console.WriteLine("Invalid minutes number entered");
            }
            else
            {
                TimeSpan time = TimeSpan.FromMinutes(minutes);        
  
            try
            {
                foreach (Process proc in Process.GetProcessesByName(toKill))
                {
                    TimeSpan runtime = DateTime.Now - proc.StartTime;
                        if (runtime >= time)
                        {
                            proc.Kill();
                            Logger(proc.ProcessName + " Has been terminated");
                        }
                        else
                        {

                            //validate frequency input
                            if (!Double.TryParse(strFrequency, out cFrequency))
                            {
                                Console.WriteLine("Invalid frequency number entered");
                            }
                            else
                            {
                                TimeSpan finalFrequency = TimeSpan.FromMinutes(cFrequency);
                                Console.WriteLine("Monitoring started, please wait...");
                                while (runtime < time)
                                {
                                    Thread.Sleep(finalFrequency);
                                    runtime = DateTime.Now - proc.StartTime;
                                    Console.WriteLine("Monitoring again");
                                    Logger("Will monitor again in" + finalFrequency);
                                }
                               

                                proc.Kill();
                                Logger(proc.ProcessName + " Has been terminated");
                            }
                        }
                    
                }
            }
            catch (Exception ex)
            {
                    Console.WriteLine("Process does not exist anymore or was never active");
            }

        }
           
            Console.WriteLine(toKill+ " has been terminated");


        }

        //Create folder and file for log
        public static void VerifyDir(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                if (!dir.Exists)
                {
                    dir.Create();
                }
            }
            catch { }
        }

        public static void Logger(string lines)
        {
            string path = "C:/Log/";
            VerifyDir(path);
            string fileName = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "_Logs.txt";
            try
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(path + fileName, true);
                file.WriteLine(DateTime.Now.ToString() + ": " + lines);
                file.Close();
            }
            catch (Exception) { }
        }
    }
}
