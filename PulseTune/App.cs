using Microsoft.VisualBasic.ApplicationServices;
using PulseTune.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PulseTune
{
    internal class App : WindowsFormsApplicationBase, IDisposable
    {
        // 非公開フィールド
        private readonly MainWindow mainWindow;

        // コンストラクタ
        public App()
        {
            this.mainWindow = new MainWindow();

            this.IsSingleInstance = !Debugger.IsAttached;
            this.MainForm = this.mainWindow;
        }

        public void Dispose()
        {
            if (this.mainWindow != null)
            {
                this.mainWindow.Dispose();
            }
        }

        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
        {
            base.OnStartupNextInstance(eventArgs);

            var transportCommandLineArgs = new List<string>();
            foreach (string argument in eventArgs.CommandLine)
            {
                transportCommandLineArgs.Add(argument);
            }
            transportCommandLineArgs.Add("/play-last-track-in-playlist");

            this.mainWindow.ProcessCommandLineArgs(transportCommandLineArgs.ToArray());
        }
    }
}
