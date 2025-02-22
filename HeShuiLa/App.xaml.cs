using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HeShuiLa
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex = null;
        public MainVM MainVM { get; } = new MainVM();

        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "HeShuiLa";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                MessageBox.Show("应用程序已经在运行中", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                Current.Shutdown();
                return;
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_mutex != null)
            {
                try
                {
                    _mutex.ReleaseMutex();
                    _mutex.Dispose();
                }
                catch (ApplicationException) { }
            }
            base.OnExit(e);
        }
    }
}
