﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Devices.Bluetooth;

namespace controller_tracker
{
    class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();


        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static ControllersTracker _tracker;

        static void Main(string[] args)
        {
            // print header and app version
            logger.Info("/////////////////////////////////////////");
            logger.Info($"////////////// {Assembly.GetCallingAssembly().GetName().Name} v{Application.ProductVersion} {(Environment.Is64BitProcess ? "x64" : "x86")}");
            logger.Info("/////////////////////////////////////////");

            _handler += new EventHandler(ConsoleExitHandler);
            SetConsoleCtrlHandler(_handler, true);

            // CONFIG
            // init config
            var config = Config.Main;
            if(config == null) {
                Console.ReadKey(); return;
            }
            else {
                logger.Info("Config loaded.");
            }




            _tracker = new ControllersTracker();
            _tracker.Run();

            while (true) { Thread.Sleep(100); }
        }

     


        private static bool ConsoleExitHandler(CtrlType sig)
        {
            logger.Info("Exiting system due to external CTRL-C, or process kill, or shutdown");


            //do your cleanup here
            _tracker.Dispose();

            logger.Info("Cleanup complete");

            logger.Info("APP CLOSED");

            //shutdown right away so there are no lingering threads
            Environment.Exit(-1);

            return true;
        }



    }
}
