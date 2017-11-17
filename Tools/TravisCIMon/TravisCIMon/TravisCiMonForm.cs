using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shell32;
using System.Collections;
using TravisCiMon.Properties;
using ToastNotifications;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace TravisCiMon
{

    public partial class TravisCiMonForm : Form
    {
        private string defaultSource = "https://api.travis-ci.org/repos/HansHammel/dotnetcore2stack/cc.xml";
        private string appname = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

        // The path to the key where Windows looks for startup applications
        private RegistryKey regkey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        //private RegistryKey regkey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);


        internal static class NativeMethods
        {
            [FlagsAttribute]
            public enum EXECUTION_STATE : uint
            {
                ES_SYSTEM_REQUIRED = 0x00000001,
                ES_DISPLAY_REQUIRED = 0x00000002,
                // Legacy flag, should not be used.
                // ES_USER_PRESENT   = 0x00000004,
                ES_AWAYMODE_REQUIRED = 0x00000040,
                ES_CONTINUOUS = 0x80000000,
            }

            public static class SleepUtil
            {
                [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
                public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
            }

            /// <summary>Shows a Window</summary>
            /// <remarks>
            /// <para>To perform certain special effects when showing or hiding a 
            /// window, use AnimateWindow.</para>
            ///<para>The first time an application calls ShowWindow, it should use 
            ///the WinMain function's nCmdShow parameter as its nCmdShow parameter. 
            ///Subsequent calls to ShowWindow must use one of the values in the 
            ///given list, instead of the one specified by the WinMain function's 
            ///nCmdShow parameter.</para>
            ///<para>As noted in the discussion of the nCmdShow parameter, the 
            ///nCmdShow value is ignored in the first call to ShowWindow if the 
            ///program that launched the application specifies startup information 
            ///in the structure. In this case, ShowWindow uses the information 
            ///specified in the STARTUPINFO structure to show the window. On 
            ///subsequent calls, the application must call ShowWindow with nCmdShow 
            ///set to SW_SHOWDEFAULT to use the startup information provided by the 
            ///program that launched the application. This behavior is designed for 
            ///the following situations: </para>
            ///<list type="">
            ///    <item>Applications create their main window by calling CreateWindow 
            ///    with the WS_VISIBLE flag set. </item>
            ///    <item>Applications create their main window by calling CreateWindow 
            ///    with the WS_VISIBLE flag cleared, and later call ShowWindow with the 
            ///    SW_SHOW flag set to make it visible.</item>
            ///</list></remarks>
            /// <param name="hWnd">Handle to the window.</param>
            /// <param name="nCmdShow">Specifies how the window is to be shown. 
            /// This parameter is ignored the first time an application calls 
            /// ShowWindow, if the program that launched the application provides a 
            /// STARTUPINFO structure. Otherwise, the first time ShowWindow is called, 
            /// the value should be the value obtained by the WinMain function in its 
            /// nCmdShow parameter. In subsequent calls, this parameter can be one of 
            /// the WindowShowStyle members.</param>
            /// <returns>
            /// If the window was previously visible, the return value is nonzero. 
            /// If the window was previously hidden, the return value is zero.
            /// </returns>
            [DllImport("user32.dll")]
            public static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);

            /// <summary>Enumeration of the different ways of showing a window using 
            /// ShowWindow</summary>
            public enum WindowShowStyle : uint
            {
                /// <summary>Hides the window and activates another window.</summary>
                /// <remarks>See SW_HIDE</remarks>
                Hide = 0,
                /// <summary>Activates and displays a window. If the window is minimized 
                /// or maximized, the system restores it to its original size and 
                /// position. An application should specify this flag when displaying 
                /// the window for the first time.</summary>
                /// <remarks>See SW_SHOWNORMAL</remarks>
                ShowNormal = 1,
                /// <summary>Activates the window and displays it as a minimized window.</summary>
                /// <remarks>See SW_SHOWMINIMIZED</remarks>
                ShowMinimized = 2,
                /// <summary>Activates the window and displays it as a maximized window.</summary>
                /// <remarks>See SW_SHOWMAXIMIZED</remarks>
                ShowMaximized = 3,
                /// <summary>Maximizes the specified window.</summary>
                /// <remarks>See SW_MAXIMIZE</remarks>
                Maximize = 3,
                /// <summary>Displays a window in its most recent size and position. 
                /// This value is similar to "ShowNormal", except the window is not 
                /// actived.</summary>
                /// <remarks>See SW_SHOWNOACTIVATE</remarks>
                ShowNormalNoActivate = 4,
                /// <summary>Activates the window and displays it in its current size 
                /// and position.</summary>
                /// <remarks>See SW_SHOW</remarks>
                Show = 5,
                /// <summary>Minimizes the specified window and activates the next 
                /// top-level window in the Z order.</summary>
                /// <remarks>See SW_MINIMIZE</remarks>
                Minimize = 6,
                /// <summary>Displays the window as a minimized window. This value is 
                /// similar to "ShowMinimized", except the window is not activated.</summary>
                /// <remarks>See SW_SHOWMINNOACTIVE</remarks>
                ShowMinNoActivate = 7,
                /// <summary>Displays the window in its current size and position. This 
                /// value is similar to "Show", except the window is not activated.</summary>
                /// <remarks>See SW_SHOWNA</remarks>
                ShowNoActivate = 8,
                /// <summary>Activates and displays the window. If the window is 
                /// minimized or maximized, the system restores it to its original size 
                /// and position. An application should specify this flag when restoring 
                /// a minimized window.</summary>
                /// <remarks>See SW_RESTORE</remarks>
                Restore = 9,
                /// <summary>Sets the show state based on the SW_ value specified in the 
                /// STARTUPINFO structure passed to the CreateProcess function by the 
                /// program that started the application.</summary>
                /// <remarks>See SW_SHOWDEFAULT</remarks>
                ShowDefault = 10,
                /// <summary>Windows 2000/XP: Minimizes a window, even if the thread 
                /// that owns the window is hung. This flag should only be used when 
                /// minimizing windows from a different thread.</summary>
                /// <remarks>See SW_FORCEMINIMIZE</remarks>
                ForceMinimized = 11
            }
        }

        public void PreventSleep()
        {
            if (TravisCiMon.TravisCiMonForm.NativeMethods.SleepUtil.SetThreadExecutionState(TravisCiMon.TravisCiMonForm.NativeMethods.EXECUTION_STATE.ES_CONTINUOUS
                | TravisCiMon.TravisCiMonForm.NativeMethods.EXECUTION_STATE.ES_DISPLAY_REQUIRED
                | TravisCiMon.TravisCiMonForm.NativeMethods.EXECUTION_STATE.ES_SYSTEM_REQUIRED
                | TravisCiMon.TravisCiMonForm.NativeMethods.EXECUTION_STATE.ES_AWAYMODE_REQUIRED) == 0) //Away mode for Windows >= Vista
                TravisCiMon.TravisCiMonForm.NativeMethods.SleepUtil.SetThreadExecutionState(TravisCiMon.TravisCiMonForm.NativeMethods.EXECUTION_STATE.ES_CONTINUOUS
                    | TravisCiMon.TravisCiMonForm.NativeMethods.EXECUTION_STATE.ES_DISPLAY_REQUIRED
                    | TravisCiMon.TravisCiMonForm.NativeMethods.EXECUTION_STATE.ES_SYSTEM_REQUIRED); //Windows < Vista, forget away mode
        }

        public TravisCiMonForm()
        {
            this.ShowInTaskbar = false;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            exitApp();
            // System.AppDomain.CurrentDomain.FriendlyName;
            //System.Diagnostics.Process.GetCurrentProcess().ProcessName; // Returns the filename without extension (e.g. MyApp).
            //string appname = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            //System.IO.Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            //System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName

            // Check to see the current state (running at startup or not)
            if (regkey.GetValue(appname) == null)
            {
                // The value doesn't exist, the application is not set to run at startup
                checkBox1.Checked = false;
            }
            else
            {
                // The value exists, the application is set to run at startup
                checkBox1.Checked = true;
            }

            //if (TravisCiMon.Properties.Settings.Default.list != null )
            //listBox2.Items.AddRange(TravisCiMon.Properties.Settings.Default.list.ToArray());

            SystemEvents.PowerModeChanged += OnPowerChange;

            // Create a timer and set a two second interval.
            //timer1 = new System.Timers.Timer();
            timer1.Interval = 30 * 60 * 1000;

            // Hook up the Elapsed event for the timer. 
            timer1.Tick += OnTimedEvent;

            // Have the timer fire repeated events (true is the default)
            //timer1.AutoReset = true;

            // Start the timer
            timer1.Enabled = true;

            // If the timer is declared in a long-running method, use KeepAlive to prevent garbage collection
            // from occurring before the method ends. 
            GC.KeepAlive(timer1);

            if (String.IsNullOrEmpty(Settings.Default["source"].ToString())) TravisCiMon.Properties.Settings.Default["source"] = defaultSource;
            TravisCiMon.Properties.Settings.Default.Save(); // Saves settings in application configuration file
            textBox1.Text = TravisCiMon.Properties.Settings.Default["source"].ToString();

        }

        private void OnPowerChange(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend)
            {
                exitApp();
            }

            if (e.Mode == PowerModes.Resume)
            {
            }
        }

        private void OnTimedEvent(object sender, EventArgs e)
        {
            Notify(false);
        }

        private void Notify(Boolean onSuccess)
        {
            try
            {
                var doc = XDocument.Load(listBox2.Items[0].ToString());
                var project = from el in doc.Descendants("Project")
                              select new
                              {
                                  status = el.Attribute("lastBuildStatus").Value,
                                  name = el.Attribute("name").Value,
                                  time = el.Attribute("lastBuildTime").Value,
                                  build = el.Attribute("lastBuildLabel").Value,
                                  progress = el.Attribute("activity").Value
                              };

                foreach (var el in project)
                {

                    {
                        if (el.status == "Success" && onSuccess) // Failure Error Unknown
                        {
                            DateTime date;
                            date = DateTime.TryParse(el.time, out date) ? DateTime.Parse(el.time) : DateTime.Now;
                            var toast = new Notification("TravisCI Build Status", el.name + ": " + el.status + "\r\n" + date.ToString() + " Build " + el.build + ": " + el.progress, 10, FormAnimator.AnimationMethod.Slide, FormAnimator.AnimationDirection.Up);
                            toast.BackgroundImage = Resources.green;
                            toast.Show();
                        }
                        else
                        {
                            DateTime date;
                            date = DateTime.TryParse(el.time, out date) ? DateTime.Parse(el.time) : DateTime.Now;
                            var toast = new Notification("TravisCI Build Status", el.name + ": " + el.status + "\r\n" + date.ToString() + " Build " + el.build + ": " + el.progress, 10, FormAnimator.AnimationMethod.Slide, FormAnimator.AnimationDirection.Up);
                            if (el.status == "Success")  {
                                toast.BackgroundImage = Resources.green;
                            }
                            else {
                                toast.BackgroundImage = Resources.red;
                                PlayNotificationSound(Resources.sonata);
                            }
                            toast.Show();
                        }

                    }
                }
            } catch (Exception ex)
            {
                if (ex is NotSupportedException)
                    MessageBox.Show(listBox2.Items[0].ToString(), ex.Message);
                if (ex is System.Net.WebException)
                {
                    Console.WriteLine(ex);
                    //todo: set timer to shorter period
                    return; //retry next time
                }
                else
                    throw; //rethrow
            }
        }

        [ComImport,
Guid("56fdf342-fd6d-11d0-958a-006097c9a090"),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITaskbarList
        {
            /// <summary>
            /// Initializes the taskbar list object. This method must be called before any other ITaskbarList methods can be called.
            /// </summary>
            int HrInit();

            /// <summary>
            /// Adds an item to the taskbar.
            /// </summary>
            /// <param name="hWnd">A handle to the window to be added to the taskbar.</param>
            int AddTab([In] IntPtr hWnd);

            /// <summary>
            /// Deletes an item from the taskbar.
            /// </summary>
            /// <param name="hWnd">A handle to the window to be deleted from the taskbar.</param>
            int DeleteTab([In] IntPtr hWnd);

            /// <summary>
            /// Activates an item on the taskbar. The window is not actually activated; the window's item on the taskbar is merely displayed as active.
            /// </summary>
            /// <param name="hWnd">A handle to the window on the taskbar to be displayed as active.</param>
            int ActivateTab([In] IntPtr hWnd);

            /// <summary>
            /// Marks a taskbar item as active but does not visually activate it.
            /// </summary>
            /// <param name="hWnd">A handle to the window to be marked as active.</param>
            int SetActiveAlt([In] IntPtr hWnd);
        }

        [ComImport]
        [Guid("56fdf344-fd6d-11d0-958a-006097c9a090")]
        public class CoTaskbarList
        {
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
                Hide();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {



        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        public static void exitApp()
        {
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            exitApp();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //TravisCiMon.Properties.Settings.Default.list = new ArrayList(listBox2.Items);
            if (String.IsNullOrEmpty(textBox1.Text)) textBox1.Text = defaultSource;
            TravisCiMon.Properties.Settings.Default["source"] = textBox1.Text;

            TravisCiMon.Properties.Settings.Default.Save(); // Saves settings in application configuration file
            if (checkBox1.Checked)
            {
                // Add the value in the registry so that the application runs at startup
                regkey.SetValue(appname, Application.ExecutablePath);
            }
            else
            {
                // Remove the value from the registry so that the application doesn't start
                regkey.DeleteValue(appname, false);
            }
            exitApp();


        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            exitApp();
            //if (System.Windows.Forms.Application.MessageLoop)
            {
                // WinForms app
                System.Windows.Forms.Application.Exit();
            }
            //else
            {
                // Console app
                System.Environment.Exit(1);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                listBox2.Items.Add(((TextBox)sender).Text);
                int index = listBox2.FindString(((TextBox)sender).Text);
                // Determine if a valid index is returned. Select the item if it is valid.
                if (index != -1)
                    listBox2.SetSelected(index, true);
                //textBlock1.Text = "You Entered: " + textBox1.Text;
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = listBox2.GetItemText(listBox2.SelectedItem);
        }

        private static void PlayNotificationSound(UnmanagedMemoryStream sound)
        {
            using (var player = new System.Media.SoundPlayer(sound))
            {
                player.Play();
            }
        }

        private static void PlayNotificationSound(string sound)
        {
            var soundsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds");
            var soundFile = Path.Combine(soundsFolder, sound + ".wav");

            using (var player = new System.Media.SoundPlayer(soundFile))
            {
                player.Play();
            }
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            Notify(true);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Show();
                WindowState = FormWindowState.Normal;
                BringToFront();
                Activate();
            }
        }
    }
}
