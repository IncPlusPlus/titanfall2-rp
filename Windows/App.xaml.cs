using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using Common;
using log4net;
using titanfall2_rp.updater;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;
using Application = Xamarin.Forms.Application;
using MessageBox = System.Windows.MessageBox;

namespace titanfall2_rp.Windows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        private NotifyIcon? _notifyIcon;
        private RichPresenceManager? _program;
        private bool _isExit;

        protected override void OnStartup(StartupEventArgs e)
        {
            _program = new RichPresenceManager();
            _program.Begin();

            try
            {
                Forms.Init();
            }
            catch (Exception exception)
            {
                Log.Fatal("Failed at Forms.Init()", exception);
                throw;
            }

            try
            {
                base.OnStartup(e);
            }
            catch (Exception exception)
            {
                Log.Fatal("Failed at base.OnStartup(e)", exception);
                throw;
            }

            _notifyIcon = new NotifyIcon();
            _notifyIcon.MouseDoubleClick += NotifyIconOnDoubleClick;
            _notifyIcon.MouseMove += NotifyIconOnMouseMove;
            _notifyIcon.Icon = titanfall2_rp.Windows.Properties.Resources.TrayIcon;
            _notifyIcon.Visible = true;
            _notifyIcon.Text = "Titanfall 2 Discord Rich Presence";

            try
            {
                CreateContextMenu();
            }
            catch (Exception exception)
            {
                Log.Fatal("Failed at CreateContextMenu()", exception);
                throw;
            }
        }

        private void NotifyIconOnDoubleClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                ToggleWindow();
        }

        private void CreateContextMenu()
        {
            _notifyIcon!.ContextMenuStrip =
                new ContextMenuStrip();

            _notifyIcon.ContextMenuStrip.ShowImageMargin = false;

            // TODO: Implement these

            // TODO: This should open Titanfall 2 depending on whether the location of the game is known (and if the user uses steam. This will be set in the config)
            _notifyIcon.ContextMenuStrip.Items.Add("Open Titanfall 2").Click += (_, _) =>
            {
                if (!ProcessUtil.LaunchTitanfall2()) NotifyUserOfError();
            };
            _notifyIcon.ContextMenuStrip.Items.Add("Open the log").Click += (_, _) =>
            {
                if (!ProcessUtil.EditFile("titanfall2-rp.log")) NotifyUserOfError();
            };
            _notifyIcon.ContextMenuStrip.Items.Add("Show the log location").Click += (_, _) =>
            {
                if (!ProcessUtil.ShowFile("titanfall2-rp.log")) NotifyUserOfError();
            };
            _notifyIcon.ContextMenuStrip.Items.Add("Show the log config location").Click += (_, _) =>
            {
                if (!ProcessUtil.ShowFile("log4net.config")) NotifyUserOfError();
            };
            _notifyIcon.ContextMenuStrip.Items.Add("Open settings").Click += (_, _) =>
            {
                if (!ProcessUtil.EditFile(Config.ConfigFileName)) NotifyUserOfError();
            };
            _notifyIcon.ContextMenuStrip.Items.Add("Show settings file location").Click += (_, _) =>
            {
                if (!ProcessUtil.ShowFile(Config.ConfigFileName)) NotifyUserOfError();
            };
            _notifyIcon.ContextMenuStrip.Items.Add("Check for Updates").Click +=
                (_, _) => UpdateHelper.Updater.Update();
            _notifyIcon.ContextMenuStrip.Items.Add("Exit").Click += (s, e) => ExitApplication();
        }

        private static void NotifyUserOfError()
        {
            MessageBox.Show(
                "An error occurred! Check the log for details.", "Titanfall 2 Discord Rich Presence",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static void AlertFeatureNotImplemented()
        {
            MessageBox.Show("This feature isn't available yet!", "Titanfall 2 Discord Rich Presence",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExitApplication()
        {
            _isExit = true;
            if (MainWindow != null)
            {
                MainWindow.Closing -= MainWindow_Closing;
                //MainWindow.LostFocus -= MainWindow_LostFocus;
                MainWindow.Close();
                MainWindow = null;
            }

            _notifyIcon?.Dispose();
            _notifyIcon = null;

            _program?.Stop();

            // Stop the application
            Current.Shutdown();
        }

        /// <summary>
        /// Toggles the window between visible and hidden.
        /// </summary>
        private void ToggleWindow()
        {
            // Create window when it is opened for the first time
            if (MainWindow == null)
            {
                MainWindow = new FormsApplicationPage
                {
                    Title = "TF|2 Rich Presence",
                    Height = 600,
                    Width = 350,
                    Topmost = true,
                    ShowInTaskbar = false,
                    ResizeMode = ResizeMode.NoResize,
                    WindowStyle = WindowStyle.ToolWindow
                };
                ((FormsApplicationPage)MainWindow).LoadApplication(new ActualApp());
                MainWindow.Closing += MainWindow_Closing;
                Application.Current.SendStart();
            }

            // Hide the window when it is visible
            if (MainWindow.IsVisible)
            {
                // Hide!
                MainWindow.Deactivated -= MainWindowOnDeactivated;
                MainWindow.Hide();
                Application.Current.SendSleep();
            }
            // Show the window when it is not visible
            else
            {
                // Position window 
                MainWindow.Left = SystemParameters.WorkArea.Width - MainWindow.Width - 50;
                MainWindow.Top = SystemParameters.WorkArea.Height - MainWindow.Height - 50;
                // Show!
                MainWindow.Show();
                MainWindow.Activate();
                MainWindow.Deactivated += MainWindowOnDeactivated;
                Application.Current.SendResume();
            }
        }

        // Toggle when the window 'X' was clicked
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!_isExit)
            {
                e.Cancel = true;
                // Only hide the window to avoid recreating it when it should get displayed again
                ToggleWindow();
            }
        }

        private System.Drawing.Point? _lastMousePositionInIcon;

        // Store the current position of the mouse on the icon to check if the mouse clicked inside the icon
        // when the window gets deactivated to avoid a duplicated window toggle
        private void NotifyIconOnMouseMove(object? sender, MouseEventArgs e)
        {
            _lastMousePositionInIcon = Control.MousePosition;
        }

        /// <summary>
        /// Called when clicked outside the window.
        /// Toggles the window to get hidden.
        /// </summary> 
        private void MainWindowOnDeactivated(object? sender, EventArgs e)
        {
            // Check if the deactivation came by clicking the icon since this already toggles the window
            if (_lastMousePositionInIcon.HasValue && _lastMousePositionInIcon == Control.MousePosition)
                return;
            ToggleWindow();
        }
    }
}