using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Microsoft.Win32;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using WinRT.Interop;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompInfoUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public static MainWindow Current;
        Window window;
        public MainWindow()
        {
            try
            {
                this.InitializeComponent();
                LoadRAMInfo();
                CheckforUpdates();
                LoadSoundInfo();
                LoadStuff();
                LoadPrintingInfo();
                LoadDisplayInfo();
                LoadKeyboardInfo();
                LoadPDInfo();
                LoadNetworkInfo();
                LoadMotherBoardInfo();
                LoadDriverInfo();
                LoadServicesInfo();
                LoadProcessesInfo();
                LoadPrintJobsInfo();

                storagedevicestabs.SelectedItem = diskssd;
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Start();
                timer.Tick += Timer_Tick; ;
                window = MainWindow.Current;
                Current = this;
                nvSample.SelectedItem = SysSum;
                application = Application.Current;
                string result = string.Empty;
                string settingsdirectory = Directory.GetCurrentDirectory() + "\\settings.ocs.txt";
                string text = File.ReadAllText(settingsdirectory);
                this.Title = "CompInfo";
                if (text.Contains("COMPInfo Windows Settings File Version 1.0.0 -- NOT TO BE MODIFIED"))
                {
                    if (text.Contains("DefaultLoadPage[]CompInfo_OC == 0"))
                    {
                        DefaultLoad.SelectedIndex = 0;
                    }
                    else if (text.Contains("DefaultLoadPage[]CompInfo_OC == 1"))
                    {
                        DefaultLoad.SelectedIndex = 1;
                    }
                    else if (text.Contains("DefaultLoadPage[]CompInfo_OC == 2"))
                    {
                        DefaultLoad.SelectedIndex = 2;
                    }
                    else if (text.Contains("DefaultLoadPage[]CompInfo_OC == 3"))
                    {
                        DefaultLoad.SelectedIndex = 3;
                    }
                    if (text.Contains("NavigationalStyle[]CompInfo_OC == 0"))
                    {
                        MenuAlignment.SelectedIndex = 0;
                    }
                    else if (text.Contains("NavigationalStyle[]CompInfo_OC == 1"))
                    {
                        MenuAlignment.SelectedIndex = 1;
                    }
                }
                if (text.Contains("COMPInfo Windows Settings File Version 1.0.0 -- NOT TO BE MODIFIED"))
                {
                    if (text.Contains("Theme[]CompInfo_OC == Dark"))
                    {
                        Dark.IsChecked = true;
                    }
                    else if (text.Contains("Theme[]CompInfo_OC == NOTDark"))
                    {
                        Light.IsChecked = true;
                    }
                    else if (text.Contains("Theme[]CompInfo_OC == WindowsDefault"))
                    {
                        Default.IsChecked = true;
                    }
                }

                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
                foreach (ManagementObject os in searcher.Get())
                {
                    result = os["Caption"].ToString();
                    break;
                }
                Osname.Text = result;
                string osversion = string.Empty;
                ManagementObjectSearcher searcher2 = new ManagementObjectSearcher("SELECT Version FROM Win32_OperatingSystem");
                foreach (ManagementObject os in searcher2.Get())
                {
                    osversion = os["Version"].ToString();
                    break;
                }
                Osver.Text = osversion;
                ManagementObjectSearcher searcher3 = new ManagementObjectSearcher("SELECT Manufacturer FROM Win32_OperatingSystem");
                foreach (ManagementObject os in searcher3.Get())
                {
                    Osdev.Text = os["Manufacturer"].ToString();
                    break;
                }
                ManagementObjectSearcher searcher4 = new ManagementObjectSearcher("SELECT OSArchitecture FROM Win32_OperatingSystem");
                foreach (ManagementObject os in searcher4.Get())
                {
                    Osarch.Text = os["OSArchitecture"].ToString();
                    osarc.Text = os["OSArchitecture"].ToString();
                    break;
                }
                ManagementObjectSearcher searcher5 = new ManagementObjectSearcher("SELECT BuildType FROM Win32_OperatingSystem");
                foreach (ManagementObject os in searcher5.Get())
                {
                    Buildtype.Text = os["BuildType"].ToString();
                    break;
                }
                pcname.Text = Environment.MachineName;
                nvSample.PaneTitle = pcname.Text;
                sysname.Text = Environment.MachineName;
                ManagementObjectSearcher searcher6 = new ManagementObjectSearcher("SELECT BuildType FROM Win32_OperatingSystem");
                foreach (ManagementObject os in searcher6.Get())
                {
                    Buildtype.Text = os["BuildType"].ToString();
                    break;
                }
                SetTitleBarColors();

                IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                string sExe = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                System.Drawing.Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(sExe);
                SendMessage(hWnd, WM_SETICON, ICON_BIG, ico.Handle);
                ManagementObjectSearcher searcher7 = new ManagementObjectSearcher("SELECT RegisteredUser FROM Win32_OperatingSystem");
                foreach (ManagementObject os in searcher7.Get())
                {
                    regist.Text = os["RegisteredUser"].ToString();
                    break;
                }
                ManagementObjectSearcher searcher8 = new ManagementObjectSearcher("SELECT SystemDirectory FROM Win32_OperatingSystem");
                foreach (ManagementObject os in searcher8.Get())
                {
                    sysdirec.Text = os["SystemDirectory"].ToString();
                    break;
                }
                LoadSysSummary();
            }
            catch (Exception ex)
            {
                errorlog += DateTime.Now + ":: " + ex.Message + Environment.NewLine;
                File.WriteAllText(@"C:\Users\bnara\Documents\log.txt", errorlog);
            }
        }
        private async void CheckforUpdates()
        {
            try
            {
                WebClient webClient = new WebClient();
                if (webClient.DownloadString("https://pastebin.com/raw/zj9pj0L8").Contains("NEXTVERSIONAVAILABLE"))
                {
                    Process.Start(new ProcessStartInfo("https://github.com/OceanCyanTech/CompInfo/releases") { UseShellExecute = true });
                }
            }
            catch (Exception EX)
            {
                RecordofException(EX);
            }
        }

        private void Dialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Process.Start(new ProcessStartInfo("https://github.com/OceanCyanTech/CompInfo/releases") { UseShellExecute = true });
        }

        string errorlog;
        public int CountDown_Seconds = 2;
        private AppWindow m_AppWindow;
        private void LoadStuff()
        {
            LoadDisks();
            try
            {
                LoadPrintingInfo();
                LoadDisplayInfo();
                LoadKeyboardInfo();
                LoadPDInfo();
                LoadNetworkInfo();
                LoadMotherBoardInfo();
                LoadDriverInfo();
                LoadServicesInfo();
                LoadProcessesInfo();
                LoadPrintJobsInfo();
            }
            catch (Exception ex)
            {
                errorlog += DateTime.Now + ":: " + ex.Message + Environment.NewLine;
                File.WriteAllText(@"C:\Users\bnara\Documents\log.txt", errorlog);
            }
        }
        private void LoadDisks()
        {
            ListDiskDevs.Items.Clear();
            ListLDDevs.Items.Clear();
            ObjectQuery diskquery2 = new ObjectQuery("SELECT * FROM Win32_LogicalDisk");
            ManagementObjectSearcher objectSearcher2 = new ManagementObjectSearcher(diskquery2);
            foreach (ManagementObject item in objectSearcher2.Get())
            {
                ListLDDevs.Items.Add(item["Caption"].ToString());
            }
            ObjectQuery diskquery3 = new ObjectQuery("SELECT * FROM Win32_DiskDrive");
            ManagementObjectSearcher objectSearcher3 = new ManagementObjectSearcher(diskquery3);
            foreach (ManagementObject item in objectSearcher3.Get())
            {
                ListDiskDevs.Items.Add(item["Caption"].ToString());
            }
            ListLDDevs.SelectedIndex = 0;
            ListDiskDevs.SelectedIndex = 0;
        }
        string decrtext;
        private void DecryptStuff(string textdecr)
        {
            try
            {
                string textToDecrypt = textdecr;
                string publickey = "89223012";
                string secretkey = "87654321";
                byte[] secretkeyByte =
     {
                   0x11, 0x02, 0x93, 0x04, 0x05, 0x06, 0x07, 0x08,
                0x19, 0x10, 0x12, 0x12, 0x13, 0x74, 0x15, 0x18
            };
                byte[] publickeybyte = { }; publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = new byte[textToDecrypt.Replace(" ", "+").Length];
                inputbyteArray = Convert.FromBase64String(textToDecrypt.Replace(" ", "+"));
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateDecryptor(publickeybyte, secretkeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    Encoding encoding = Encoding.UTF8;
                    decrtext = encoding.GetString(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                errorlog += DateTime.Now + ":: " + ex.Message + Environment.NewLine;
                File.WriteAllText(@"C:\Users\bnara\Documents\log.txt", errorlog);
            }
        }
        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }
        private void SetTitleBarColors()
        {
            try
            {
                // Check to see if customization is supported.
                // Currently only supported on Windows 11.
                if (AppWindowTitleBar.IsCustomizationSupported())
                {
                    if (m_AppWindow is null)
                    {
                        m_AppWindow = GetAppWindowForCurrentWindow();
                    }
                    var titleBar = m_AppWindow.TitleBar;
                    m_AppWindow.SetIcon(@"C: \Users\bnara\Downloads\CompInfoIcon.ico");
                    titleBar.IconShowOptions = IconShowOptions.ShowIconAndSystemMenu;
                    // Set active window colors
                    if (Dark.IsChecked == true)
                    {
                        titleBar.ForegroundColor = Colors.White;
                        titleBar.BackgroundColor = Colors.Black;
                        titleBar.ButtonForegroundColor = Colors.White;
                        titleBar.ButtonBackgroundColor = Colors.Black;
                        titleBar.ButtonHoverForegroundColor = Colors.White;
                        titleBar.ButtonHoverBackgroundColor = Colors.Black;
                        titleBar.ButtonPressedForegroundColor = Colors.Gray;
                        titleBar.ButtonPressedBackgroundColor = Colors.Black;

                        // Set inactive window colors
                        titleBar.InactiveForegroundColor = Colors.White;
                        titleBar.InactiveBackgroundColor = Colors.Black;
                        titleBar.ButtonInactiveForegroundColor = Colors.White;
                        titleBar.ButtonInactiveBackgroundColor = Colors.Black;
                    }
                    else if (Light.IsChecked == true)
                    {
                        titleBar.ForegroundColor = Colors.Black;
                        titleBar.BackgroundColor = Colors.White;
                        titleBar.ButtonForegroundColor = Colors.Black;
                        titleBar.ButtonBackgroundColor = Colors.White;
                        titleBar.ButtonHoverForegroundColor = Colors.Black;
                        titleBar.ButtonHoverBackgroundColor = Colors.White;
                        titleBar.ButtonPressedForegroundColor = Colors.Gray;
                        titleBar.ButtonPressedBackgroundColor = Colors.White;

                        // Set inactive window colors
                        titleBar.InactiveForegroundColor = Colors.Black;
                        titleBar.InactiveBackgroundColor = Colors.White;
                        titleBar.ButtonInactiveForegroundColor = Colors.Black;
                        titleBar.ButtonInactiveBackgroundColor = Colors.White;
                    }
                    m_AppWindow.SetIcon(@"C: \Users\bnara\Downloads\CompInfoIcon.ico");
                    titleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
                }

                IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                string sExe = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                System.Drawing.Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(sExe);
                SendMessage(hWnd, WM_SETICON, ICON_BIG, ico.Handle);
            }

            catch (Exception ex)
            {
                errorlog += DateTime.Now + ":: " + ex.Message + Environment.NewLine;
                File.WriteAllText(@"C:\Users\bnara\Documents\log.txt", errorlog);
            }
        }
        private void LoadSysSummary()
        {
            try
            {
                //OS Name
                ManagementObjectSearcher searcher8 = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
                foreach (ManagementObject os in searcher8.Get())
                {
                    osname.Text = os["Caption"].ToString();
                    break;
                }
                //Processor
                ManagementObjectSearcher searcher9 = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor");
                foreach (ManagementObject os in searcher9.Get())
                {
                    processor.Text = os["Name"].ToString();
                    CPUName.Text = os["Name"].ToString();
                    break;
                }
                //RAM
                [DllImport("kernel32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);
                long memKb;
                GetPhysicallyInstalledSystemMemory(out memKb);
                ram.Text = (memKb / 1024 / 1024) + " GB";
                TOTALRam.Text = (memKb / 1024 / 1024) + " GB";
                //Graphics
                ManagementObjectSearcher searcher11 = new ManagementObjectSearcher("SELECT * FROM Win32_DisplayConfiguration");
                foreach (ManagementObject os in searcher11.Get())
                {
                    gpusummary.Text = os["Description"].ToString();
                    gpuname.Text = os["Description"].ToString();
                    break;
                }
            }
            catch (Exception ex)
            {
                errorlog += DateTime.Now + ":: " + ex.Message + Environment.NewLine;
                File.WriteAllText(@"C:\Users\bnara\Documents\log.txt", errorlog);
            }
        }
        private void LoadRAMInfo()
        {
            try
            {
                //Total Virtual Memory
                ObjectQuery winQuery = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");

                ManagementObjectSearcher searcher = new ManagementObjectSearcher(winQuery);

                foreach (ManagementObject item in searcher.Get())
                {
                    var virtualmem = (item["TotalVirtualMemorySize"]).ToString();
                    var virtualmemdouble = Double.Parse(virtualmem) / 1e+6;
                    TOTALViRam.Text = virtualmemdouble.ToString("#.#") + " GB";
                }
                //Available Physical Memory
                ObjectQuery winQuery2 = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");

                ManagementObjectSearcher searcher2 = new ManagementObjectSearcher(winQuery2);

                foreach (ManagementObject item in searcher2.Get())
                {
                    var availram = (item["FreePhysicalMemory"]).ToString();
                    var availramdouble = Double.Parse(availram) / 1e+6;
                    AVAILRam.Text = availramdouble.ToString("#.#") + " GB";
                }
                //Available Virtual Memory
                ObjectQuery winQuery3 = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");

                ManagementObjectSearcher searcher3 = new ManagementObjectSearcher(winQuery3);

                foreach (ManagementObject item in searcher3.Get())
                {
                    var availram = (item["FreeVirtualMemory"]).ToString();
                    var availramdouble = Double.Parse(availram) / 1e+6;
                    AVAILViRam.Text = availramdouble.ToString("#.#") + " GB";
                }
                ObjectQuery winq = new ObjectQuery("SELECT * FROM CIM_OperatingSystem");

                ManagementObjectSearcher searcher4 = new ManagementObjectSearcher(winq);
                string mempage = "Unknown";
                foreach (ManagementObject item in searcher4.Get())
                {
                    mempage = item["SizeStoredInPagingFiles"].ToString();
                }
                MemPageSize.Text = (Convert.ToInt32(mempage) / 1e+6).ToString("#.##") + " GB";
            }
            catch (Exception ex)
            {
                errorlog += DateTime.Now + ":: " + ex.Message + Environment.NewLine;
                File.WriteAllText(@"C:\Users\bnara\Documents\log.txt", errorlog);
            }
        }
        private void LoadSoundInfo()
        {
            ListSoundDevs.Items.Clear();
            //Load Sound Devices
            ObjectQuery SoundObject = new ObjectQuery("SELECT * FROM Win32_SoundDevice");
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(SoundObject);
            foreach (ManagementObject obj in managementObjectSearcher.Get())
            {
                ListSoundDevs.Items.Add(obj["Caption"].ToString());
            }
            ListSoundDevs.SelectedIndex = 0;
        }
        private void LoadPrintingInfo()
        {
            try
            {
                ListPrintingDevs.Items.Clear();
                //Load Printer Devices
                ObjectQuery SoundObject = new ObjectQuery("SELECT * FROM Win32_Printer");
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(SoundObject);
                foreach (ManagementObject obj in managementObjectSearcher.Get())
                {
                    ListPrintingDevs.Items.Add(obj["Caption"].ToString());
                }
                ListPrintingDevs.SelectedIndex = 0;
            }
            catch (Exception EX)
            {
                RecordofException(EX);
            }
        }
        private void LoadNetworkInfo()
        {
            try
            {
                Networkitemsname.Items.Clear();
                //Load Printer Devices
                ObjectQuery SoundObject = new ObjectQuery("SELECT * FROM Win32_NetworkAdapterConfiguration");
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(SoundObject);
                foreach (ManagementObject obj in managementObjectSearcher.Get())
                {
                    Networkitemsname.Items.Add(obj["Caption"].ToString());
                }
                Networkitemsname.SelectedIndex = 0;
            }
            catch (Exception EX)
            {
                RecordofException(EX);
            }
        }
        private void LoadDriverInfo()
        {
            try
            {
                Driveritemname.Items.Clear();
                //Load Driver Info
                ObjectQuery SoundObject = new ObjectQuery("SELECT * FROM Win32_SystemDriver");
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(SoundObject);
                foreach (ManagementObject obj in managementObjectSearcher.Get())
                {
                    Driveritemname.Items.Add(obj["Caption"].ToString());
                }
                Driveritemname.SelectedIndex = 0;
            }
            catch (Exception EX)
            {
                RecordofException(EX);
            }
        }
        private void LoadServicesInfo()
        {
            try
            {
                serviceitemname.Items.Clear();
                //Load Services Information
                ObjectQuery SoundObject = new ObjectQuery("SELECT * FROM Win32_Service");
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(SoundObject);
                foreach (ManagementObject obj in managementObjectSearcher.Get())
                {
                    serviceitemname.Items.Add(obj["Caption"].ToString());
                }
                serviceitemname.SelectedIndex = 0;
            }
            catch (Exception EX)
            {
                RecordofException(EX);
            }
        }
        private void LoadProcessesInfo()
        {
            try
            {
                SearchProcesses.Text = "";
                processitemsname.Items.Clear();
                listView.Items.Clear();
                //Load Process Information
                ObjectQuery SoundObject = new ObjectQuery("SELECT * FROM Win32_Process");
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(SoundObject);
                foreach (ManagementObject obj in managementObjectSearcher.Get())
                {
                    processitemsname.Items.Add(obj["Caption"].ToString());
                    listView.Items.Add(obj["Caption"].ToString());
                }
                processitemsname.SelectedIndex = 0;

            }
            catch (Exception EX)
            {
                RecordofException(EX);
            }
        }
        private void LoadPrintJobsInfo()
        {
            try
            {
                SearchPrintJobs.Text = "";
                printerjobitemname.Items.Clear();
                listView2.Items.Clear();
                //Load PrintJobs Information
                ObjectQuery SoundObject = new ObjectQuery("SELECT * FROM Win32_PrintJob");
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(SoundObject);
                foreach (ManagementObject obj in managementObjectSearcher.Get())
                {
                    printerjobitemname.Items.Add(obj["Caption"].ToString());
                    listView2.Items.Add(obj["Caption"].ToString());
                }
                printerjobitemname.SelectedIndex = 0;
                if (printerjobitemname.Items.Count == 0)
                {
                    printerdevice.Text = "There are currently no print jobs!";
                    documentname.Text = "";
                    documentsize.Text = "";
                    owner.Text = "";
                    printstatus.Text = "";
                    pages.Text = "";
                    timesubmitted.Text = "";
                }
            }
            catch (Exception EX)
            {
                RecordofException(EX);
            }
        }
        private void LoadKeyboardInfo()
        {
            try
            {
                ListKeyboardDevs.Items.Clear();
                //Load Keyboard Devices
                ObjectQuery SoundObject = new ObjectQuery("SELECT * FROM CIM_Keyboard");
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(SoundObject);
                foreach (ManagementObject obj in managementObjectSearcher.Get())
                {
                    ListKeyboardDevs.Items.Add(obj["Caption"].ToString());
                }
                ListKeyboardDevs.SelectedIndex = 0;
            }
            catch (Exception EX)
            {
                RecordofException(EX);
            }
        }
        private void LoadPDInfo()
        {
            try
            {
                ListPointingDevs.Items.Clear();
                //Load Printer Devices
                ObjectQuery SoundObject = new ObjectQuery("SELECT * FROM CIM_PointingDevice");
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(SoundObject);
                foreach (ManagementObject obj in managementObjectSearcher.Get())
                {
                    ListPointingDevs.Items.Add(obj["Caption"].ToString());
                }
                ListPointingDevs.SelectedIndex = 0;
            }
            catch (Exception EX)
            {
                RecordofException(EX);
            }
        }
        private void LoadDisplayInfo()
        {
            try
            {
                ListGPUDevs.Items.Clear();
                //Load Printer Devices
                ObjectQuery SoundObject = new ObjectQuery("SELECT * FROM Win32_VideoController");
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(SoundObject);
                foreach (ManagementObject obj in managementObjectSearcher.Get())
                {
                    ListGPUDevs.Items.Add(obj["Caption"].ToString());
                }
                ListGPUDevs.SelectedIndex = 0;
            }
            catch (Exception EX)
            {
                RecordofException(EX);
            }
        }
        private void Timer_Tick(object sender, object e)
        {
            CountDown_Seconds = CountDown_Seconds - 1;
            if (CountDown_Seconds == 0)
            {
                timer.Stop();
                InitialLoader.Visibility = Visibility.Collapsed;
                nvSample.Visibility = Visibility.Visible;
            }
        }

        DispatcherTimer timer = new DispatcherTimer();
        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            //   myButton.Content = "Clicked";
        }
        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
        }

        private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
        {
        }
        private void LoadMotherBoardInfo()
        {
            //Load BIOS Information
            ObjectQuery mbobj = new ObjectQuery("SELECT * FROM Win32_BIOS");
            ManagementObjectSearcher mbobjsearch = new ManagementObjectSearcher(mbobj);
            foreach (ManagementObject obj in mbobjsearch.Get())
            {
                BiosDescription.Text = obj["Description"].ToString();
                BiosCurrentLanguage.Text = obj["CurrentLanguage"].ToString();
                SMBiosversion.Text = obj["SMBIOSBIOSVersion"].ToString();
                Biosversion.Text = obj["Version"].ToString();
            }
            //Load Baseboard Information
            ObjectQuery mbobj2 = new ObjectQuery("SELECT * FROM Win32_BaseBoard");
            ManagementObjectSearcher mbobjsearch2 = new ManagementObjectSearcher(mbobj2);
            foreach (ManagementObject obj in mbobjsearch2.Get())
            {
                motherboardname.Text = obj["Name"].ToString();
                mbserialnumber.Text = obj["SerialNumber"].ToString();
                mbmanufacturer.Text = obj["Manufacturer"].ToString();
                mbproduct.Text = obj["Product"].ToString();
                bool replaceable = (bool)obj["Replaceable"];
                if (replaceable == true)
                {
                    ismbreplace.Text = "Yes";
                }
                else
                {
                    ismbreplace.Text = "No";
                }
                if (mbserialnumber.Text == null)
                {
                    mbserialnumber.Text = "Unable to access the serial number";
                }
            }
        }
        private void LoadCPUInfo()
        {
            string CPUInfo = "Unknown or Invalid Information";
            ManagementObjectSearcher mos =
new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
            foreach (ManagementObject mo in mos.Get())
            {
                CPUInfo = mo["Name"].ToString();
            }
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            //collection to store all management objects
            string CPUManufacturer2 = "Intel";
            ManagementObjectCollection moc = mc.GetInstances();
            if (moc.Count != 0)
            {
                foreach (ManagementObject mo in mc.GetInstances())
                {
                    // display general system information
                    CPUManufacturer2 = mo["Manufacturer"].ToString();
                }
            }
            string LogicalProcessors = Environment.ProcessorCount.ToString();
            int coreCount = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                coreCount += int.Parse(item["NumberOfCores"].ToString());
            }
            if (CPUInfo.ToLower().Contains("intel"))
            {
                CPUManufacturer.Text = "Intel®";
            }
            else if (CPUInfo.ToLower().Contains("amd"))
            {
                CPUManufacturer.Text = "AMD®";
            }
            else if (CPUInfo.ToLower().Contains("qualcomm"))
            {
                CPUManufacturer.Text = "Qualcomm®";
            }
            else
            {
                CPUManufacturer.Text = "Unknown";
            }
            LogicProcessors.Text = LogicalProcessors;
            Cores.Text = coreCount.ToString();
        }
        private void nvSample_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                nvSample.Header = "Settings";
                settingspanel.Visibility = Visibility.Visible;
                osinfo.Visibility = Visibility.Collapsed;
                hardwaree.Visibility = Visibility.Collapsed;
                softwaree.Visibility = Visibility.Collapsed;
                driverinfo.Visibility = Visibility.Collapsed;
                syssummary.Visibility = Visibility.Collapsed;
                raminfo.Visibility = Visibility.Collapsed;
                cpuinfo.Visibility = Visibility.Collapsed;
                soundinfo.Visibility = Visibility.Collapsed;
                Home.Visibility = Visibility.Collapsed;
                inputdevicesinfo.Visibility = Visibility.Collapsed;
                storagedevicesinfo.Visibility = Visibility.Collapsed;
                printinginfo.Visibility = Visibility.Collapsed;
                gpuinfo.Visibility = Visibility.Collapsed;
                gpuinfo.Visibility = Visibility.Collapsed;
                networkinfo.Visibility = Visibility.Collapsed;
                logicboardinfo.Visibility = Visibility.Collapsed;
                servicesinfo.Visibility = Visibility.Collapsed;
                processesinfo.Visibility = Visibility.Collapsed;
                printjobsinfo.Visibility = Visibility.Collapsed;
                SearchSuggestions.Visibility = Visibility.Collapsed;
            }
            else
            {
                NavigationViewItem item = args.SelectedItem as NavigationViewItem;
                nvSample.Header = item.Content.ToString();
                settingspanel.Visibility = Visibility.Collapsed;
                SearchSuggestions.Visibility = Visibility.Collapsed;
                if (nvSample.SelectedItem == SysOS)
                {
                    printjobsinfo.Visibility = Visibility.Collapsed; processesinfo.Visibility = Visibility.Collapsed;
                    driverinfo.Visibility = Visibility.Collapsed;
                    softwaree.Visibility = Visibility.Collapsed;
                    osinfo.Visibility = Visibility.Visible;
                    syssummary.Visibility = Visibility.Collapsed;
                    hardwaree.Visibility = Visibility.Collapsed;
                    cpuinfo.Visibility = Visibility.Collapsed;
                    raminfo.Visibility = Visibility.Collapsed;
                    soundinfo.Visibility = Visibility.Collapsed;
                    Home.Visibility = Visibility.Collapsed;
                    inputdevicesinfo.Visibility = Visibility.Collapsed;
                    storagedevicesinfo.Visibility = Visibility.Collapsed;
                    printinginfo.Visibility = Visibility.Collapsed;
                    gpuinfo.Visibility = Visibility.Collapsed; networkinfo.Visibility = Visibility.Collapsed;
                    networkinfo.Visibility = Visibility.Collapsed;
                    logicboardinfo.Visibility = Visibility.Collapsed;
                    servicesinfo.Visibility = Visibility.Collapsed;
                }
                else if (nvSample.SelectedItem == osinfoo)
                {
                    printjobsinfo.Visibility = Visibility.Collapsed;
                    nvSample.SelectedItem = SysOS;
                }
                else if (nvSample.SelectedItem == SysSum)
                {
                    printjobsinfo.Visibility = Visibility.Collapsed; processesinfo.Visibility = Visibility.Collapsed;
                    driverinfo.Visibility = Visibility.Collapsed;
                    softwaree.Visibility = Visibility.Collapsed;
                    osinfo.Visibility = Visibility.Collapsed;
                    raminfo.Visibility = Visibility.Collapsed;
                    hardwaree.Visibility = Visibility.Collapsed;
                    syssummary.Visibility = Visibility.Visible;
                    cpuinfo.Visibility = Visibility.Collapsed;
                    soundinfo.Visibility = Visibility.Collapsed; Home.Visibility = Visibility.Collapsed;
                    storagedevicesinfo.Visibility = Visibility.Collapsed; logicboardinfo.Visibility = Visibility.Collapsed;
                    inputdevicesinfo.Visibility = Visibility.Collapsed; printinginfo.Visibility = Visibility.Collapsed; gpuinfo.Visibility = Visibility.Collapsed; networkinfo.Visibility = Visibility.Collapsed;
                    servicesinfo.Visibility = Visibility.Collapsed;
                }
                else if (nvSample.SelectedItem == HardCPU)
                {
                    printjobsinfo.Visibility = Visibility.Collapsed; processesinfo.Visibility = Visibility.Collapsed;
                    driverinfo.Visibility = Visibility.Collapsed;
                    softwaree.Visibility = Visibility.Collapsed;
                    LoadCPUInfo();
                    osinfo.Visibility = Visibility.Collapsed;
                    hardwaree.Visibility = Visibility.Collapsed;
                    syssummary.Visibility = Visibility.Collapsed;
                    raminfo.Visibility = Visibility.Collapsed;
                    soundinfo.Visibility = Visibility.Collapsed;
                    cpuinfo.Visibility = Visibility.Visible; Home.Visibility = Visibility.Collapsed; inputdevicesinfo.Visibility = Visibility.Collapsed;
                    storagedevicesinfo.Visibility = Visibility.Collapsed; logicboardinfo.Visibility = Visibility.Collapsed; printinginfo.Visibility = Visibility.Collapsed; gpuinfo.Visibility = Visibility.Collapsed; networkinfo.Visibility = Visibility.Collapsed;
                    servicesinfo.Visibility = Visibility.Collapsed;
                }
                else if (nvSample.SelectedItem == HardRAM)
                {
                    printjobsinfo.Visibility = Visibility.Collapsed; processesinfo.Visibility = Visibility.Collapsed;
                    driverinfo.Visibility = Visibility.Collapsed;
                    softwaree.Visibility = Visibility.Collapsed;
                    LoadRAMInfo();
                    osinfo.Visibility = Visibility.Collapsed;
                    hardwaree.Visibility = Visibility.Collapsed;
                    cpuinfo.Visibility = Visibility.Collapsed;
                    syssummary.Visibility = Visibility.Collapsed;
                    soundinfo.Visibility = Visibility.Collapsed;
                    raminfo.Visibility = Visibility.Visible; Home.Visibility = Visibility.Collapsed;
                    inputdevicesinfo.Visibility = Visibility.Collapsed;
                    storagedevicesinfo.Visibility = Visibility.Collapsed; logicboardinfo.Visibility = Visibility.Collapsed; printinginfo.Visibility = Visibility.Collapsed; gpuinfo.Visibility = Visibility.Collapsed; networkinfo.Visibility = Visibility.Collapsed;
                    servicesinfo.Visibility = Visibility.Collapsed;
                }
                else if (nvSample.SelectedItem == SysHard)
                {
                    printjobsinfo.Visibility = Visibility.Collapsed; processesinfo.Visibility = Visibility.Collapsed;
                    driverinfo.Visibility = Visibility.Collapsed;
                    softwaree.Visibility = Visibility.Collapsed;
                    osinfo.Visibility = Visibility.Collapsed;
                    syssummary.Visibility = Visibility.Collapsed;
                    cpuinfo.Visibility = Visibility.Collapsed;
                    raminfo.Visibility = Visibility.Collapsed;
                    soundinfo.Visibility = Visibility.Collapsed; Home.Visibility = Visibility.Collapsed;
                    hardwaree.Visibility = Visibility.Visible;
                    inputdevicesinfo.Visibility = Visibility.Collapsed;
                    storagedevicesinfo.Visibility = Visibility.Collapsed; logicboardinfo.Visibility = Visibility.Collapsed; printinginfo.Visibility = Visibility.Collapsed; gpuinfo.Visibility = Visibility.Collapsed; networkinfo.Visibility = Visibility.Collapsed;
                    servicesinfo.Visibility = Visibility.Collapsed;
                }
                else if (nvSample.SelectedItem == SysSoft)
                {
                    printjobsinfo.Visibility = Visibility.Collapsed; processesinfo.Visibility = Visibility.Collapsed;
                    driverinfo.Visibility = Visibility.Collapsed;
                    osinfo.Visibility = Visibility.Collapsed;
                    syssummary.Visibility = Visibility.Collapsed;
                    cpuinfo.Visibility = Visibility.Collapsed;
                    raminfo.Visibility = Visibility.Collapsed;
                    soundinfo.Visibility = Visibility.Collapsed; Home.Visibility = Visibility.Collapsed;
                    softwaree.Visibility = Visibility.Visible;
                    hardwaree.Visibility = Visibility.Collapsed;
                    inputdevicesinfo.Visibility = Visibility.Collapsed;
                    storagedevicesinfo.Visibility = Visibility.Collapsed; logicboardinfo.Visibility = Visibility.Collapsed; printinginfo.Visibility = Visibility.Collapsed; gpuinfo.Visibility = Visibility.Collapsed; networkinfo.Visibility = Visibility.Collapsed;
                    servicesinfo.Visibility = Visibility.Collapsed;
                }
                else if (nvSample.SelectedItem == HardSound)
                {
                    printjobsinfo.Visibility = Visibility.Collapsed; processesinfo.Visibility = Visibility.Collapsed;
                    driverinfo.Visibility = Visibility.Collapsed;
                    softwaree.Visibility = Visibility.Collapsed;
                    osinfo.Visibility = Visibility.Collapsed;
                    syssummary.Visibility = Visibility.Collapsed;
                    cpuinfo.Visibility = Visibility.Collapsed;
                    raminfo.Visibility = Visibility.Collapsed;
                    soundinfo.Visibility = Visibility.Visible;
                    hardwaree.Visibility = Visibility.Collapsed; Home.Visibility = Visibility.Collapsed;
                    inputdevicesinfo.Visibility = Visibility.Collapsed;
                    storagedevicesinfo.Visibility = Visibility.Collapsed; logicboardinfo.Visibility = Visibility.Collapsed; printinginfo.Visibility = Visibility.Collapsed; gpuinfo.Visibility = Visibility.Collapsed; networkinfo.Visibility = Visibility.Collapsed;
                    servicesinfo.Visibility = Visibility.Collapsed;
                }
                else if (nvSample.SelectedItem == HardInputDevices)
                {
                    printjobsinfo.Visibility = Visibility.Collapsed; processesinfo.Visibility = Visibility.Collapsed;
                    driverinfo.Visibility = Visibility.Collapsed;
                    softwaree.Visibility = Visibility.Collapsed;
                    osinfo.Visibility = Visibility.Collapsed;
                    syssummary.Visibility = Visibility.Collapsed;
                    cpuinfo.Visibility = Visibility.Collapsed;
                    raminfo.Visibility = Visibility.Collapsed;
                    soundinfo.Visibility = Visibility.Collapsed;
                    hardwaree.Visibility = Visibility.Collapsed; Home.Visibility = Visibility.Collapsed;
                    inputdevicesinfo.Visibility = Visibility.Visible;
                    storagedevicesinfo.Visibility = Visibility.Collapsed; logicboardinfo.Visibility = Visibility.Collapsed; printinginfo.Visibility = Visibility.Collapsed; gpuinfo.Visibility = Visibility.Collapsed; networkinfo.Visibility = Visibility.Collapsed;
                    servicesinfo.Visibility = Visibility.Collapsed;
                }
                else if (nvSample.SelectedItem == HardStorageDevices)
                {
                    printjobsinfo.Visibility = Visibility.Collapsed; processesinfo.Visibility = Visibility.Collapsed;
                    driverinfo.Visibility = Visibility.Collapsed;
                    softwaree.Visibility = Visibility.Collapsed;
                    osinfo.Visibility = Visibility.Collapsed;
                    syssummary.Visibility = Visibility.Collapsed;
                    cpuinfo.Visibility = Visibility.Collapsed;
                    raminfo.Visibility = Visibility.Collapsed;
                    soundinfo.Visibility = Visibility.Collapsed;
                    hardwaree.Visibility = Visibility.Collapsed; Home.Visibility = Visibility.Collapsed;
                    inputdevicesinfo.Visibility = Visibility.Collapsed; logicboardinfo.Visibility = Visibility.Collapsed;
                    storagedevicesinfo.Visibility = Visibility.Visible; printinginfo.Visibility = Visibility.Collapsed; gpuinfo.Visibility = Visibility.Collapsed; networkinfo.Visibility = Visibility.Collapsed;
                    servicesinfo.Visibility = Visibility.Collapsed;
                }
                else if (nvSample.SelectedItem == HardPrinting)
                {
                    printjobsinfo.Visibility = Visibility.Collapsed; processesinfo.Visibility = Visibility.Collapsed;
                    driverinfo.Visibility = Visibility.Collapsed;
                    softwaree.Visibility = Visibility.Collapsed;
                    osinfo.Visibility = Visibility.Collapsed;
                    syssummary.Visibility = Visibility.Collapsed;
                    cpuinfo.Visibility = Visibility.Collapsed;
                    raminfo.Visibility = Visibility.Collapsed;
                    soundinfo.Visibility = Visibility.Collapsed;
                    hardwaree.Visibility = Visibility.Collapsed; Home.Visibility = Visibility.Collapsed;
                    inputdevicesinfo.Visibility = Visibility.Collapsed;
                    storagedevicesinfo.Visibility = Visibility.Collapsed; logicboardinfo.Visibility = Visibility.Collapsed;
                    printinginfo.Visibility = Visibility.Visible;
                    gpuinfo.Visibility = Visibility.Collapsed; networkinfo.Visibility = Visibility.Collapsed;
                    servicesinfo.Visibility = Visibility.Collapsed;
                }
                else if (nvSample.SelectedItem == HardGPU)
                {
                    printjobsinfo.Visibility = Visibility.Collapsed; processesinfo.Visibility = Visibility.Collapsed;
                    driverinfo.Visibility = Visibility.Collapsed;
                    softwaree.Visibility = Visibility.Collapsed;
                    osinfo.Visibility = Visibility.Collapsed;
                    syssummary.Visibility = Visibility.Collapsed;
                    cpuinfo.Visibility = Visibility.Collapsed;
                    raminfo.Visibility = Visibility.Collapsed;
                    soundinfo.Visibility = Visibility.Collapsed;
                    hardwaree.Visibility = Visibility.Collapsed; Home.Visibility = Visibility.Collapsed;
                    inputdevicesinfo.Visibility = Visibility.Collapsed;
                    storagedevicesinfo.Visibility = Visibility.Collapsed;
                    printinginfo.Visibility = Visibility.Collapsed;
                    gpuinfo.Visibility = Visibility.Visible;
                    networkinfo.Visibility = Visibility.Collapsed;
                    logicboardinfo.Visibility = Visibility.Collapsed;
                    servicesinfo.Visibility = Visibility.Collapsed;
                }
                else if (nvSample.SelectedItem == HardNetwork)
                {
                    printjobsinfo.Visibility = Visibility.Collapsed; processesinfo.Visibility = Visibility.Collapsed;
                    driverinfo.Visibility = Visibility.Collapsed;
                    softwaree.Visibility = Visibility.Collapsed;
                    osinfo.Visibility = Visibility.Collapsed;
                    syssummary.Visibility = Visibility.Collapsed;
                    cpuinfo.Visibility = Visibility.Collapsed;
                    raminfo.Visibility = Visibility.Collapsed;
                    soundinfo.Visibility = Visibility.Collapsed;
                    hardwaree.Visibility = Visibility.Collapsed; Home.Visibility = Visibility.Collapsed;
                    inputdevicesinfo.Visibility = Visibility.Collapsed;
                    storagedevicesinfo.Visibility = Visibility.Collapsed;
                    printinginfo.Visibility = Visibility.Collapsed;
                    gpuinfo.Visibility = Visibility.Collapsed;
                    networkinfo.Visibility = Visibility.Visible;
                    logicboardinfo.Visibility = Visibility.Collapsed;
                    servicesinfo.Visibility = Visibility.Collapsed;
                }
                else if (nvSample.SelectedItem == HardMotherBoard)
                {
                    printjobsinfo.Visibility = Visibility.Collapsed; processesinfo.Visibility = Visibility.Collapsed;
                    driverinfo.Visibility = Visibility.Collapsed;
                    softwaree.Visibility = Visibility.Collapsed;
                    osinfo.Visibility = Visibility.Collapsed;
                    syssummary.Visibility = Visibility.Collapsed;
                    cpuinfo.Visibility = Visibility.Collapsed;
                    raminfo.Visibility = Visibility.Collapsed;
                    soundinfo.Visibility = Visibility.Collapsed;
                    hardwaree.Visibility = Visibility.Collapsed; Home.Visibility = Visibility.Collapsed;
                    inputdevicesinfo.Visibility = Visibility.Collapsed;
                    storagedevicesinfo.Visibility = Visibility.Collapsed;
                    printinginfo.Visibility = Visibility.Collapsed;
                    gpuinfo.Visibility = Visibility.Collapsed;
                    networkinfo.Visibility = Visibility.Collapsed;
                    logicboardinfo.Visibility = Visibility.Visible;
                    servicesinfo.Visibility = Visibility.Collapsed;
                }
                else if (nvSample.SelectedItem == SoftDrivers)
                {
                    printjobsinfo.Visibility = Visibility.Collapsed; processesinfo.Visibility = Visibility.Collapsed;
                    driverinfo.Visibility = Visibility.Visible;
                    softwaree.Visibility = Visibility.Collapsed;
                    osinfo.Visibility = Visibility.Collapsed;
                    syssummary.Visibility = Visibility.Collapsed;
                    cpuinfo.Visibility = Visibility.Collapsed;
                    raminfo.Visibility = Visibility.Collapsed;
                    soundinfo.Visibility = Visibility.Collapsed;
                    hardwaree.Visibility = Visibility.Collapsed;
                    Home.Visibility = Visibility.Collapsed;
                    inputdevicesinfo.Visibility = Visibility.Collapsed;
                    storagedevicesinfo.Visibility = Visibility.Collapsed;
                    printinginfo.Visibility = Visibility.Collapsed;
                    gpuinfo.Visibility = Visibility.Collapsed;
                    networkinfo.Visibility = Visibility.Collapsed;
                    logicboardinfo.Visibility = Visibility.Collapsed;
                    servicesinfo.Visibility = Visibility.Collapsed;
                }
                else if (nvSample.SelectedItem == SoftServices)
                {
                    printjobsinfo.Visibility = Visibility.Collapsed; processesinfo.Visibility = Visibility.Collapsed;
                    driverinfo.Visibility = Visibility.Collapsed;
                    softwaree.Visibility = Visibility.Collapsed;
                    osinfo.Visibility = Visibility.Collapsed;
                    syssummary.Visibility = Visibility.Collapsed;
                    cpuinfo.Visibility = Visibility.Collapsed;
                    raminfo.Visibility = Visibility.Collapsed;
                    soundinfo.Visibility = Visibility.Collapsed;
                    hardwaree.Visibility = Visibility.Collapsed;
                    Home.Visibility = Visibility.Collapsed;
                    inputdevicesinfo.Visibility = Visibility.Collapsed;
                    storagedevicesinfo.Visibility = Visibility.Collapsed;
                    printinginfo.Visibility = Visibility.Collapsed;
                    gpuinfo.Visibility = Visibility.Collapsed;
                    networkinfo.Visibility = Visibility.Collapsed;
                    logicboardinfo.Visibility = Visibility.Collapsed;
                    servicesinfo.Visibility = Visibility.Visible;
                }
                else if (nvSample.SelectedItem == SoftProcesses)
                {
                    printjobsinfo.Visibility = Visibility.Collapsed;
                    processesinfo.Visibility = Visibility.Visible;
                    driverinfo.Visibility = Visibility.Collapsed;
                    softwaree.Visibility = Visibility.Collapsed;
                    osinfo.Visibility = Visibility.Collapsed;
                    syssummary.Visibility = Visibility.Collapsed;
                    cpuinfo.Visibility = Visibility.Collapsed;
                    raminfo.Visibility = Visibility.Collapsed;
                    soundinfo.Visibility = Visibility.Collapsed;
                    hardwaree.Visibility = Visibility.Collapsed;
                    Home.Visibility = Visibility.Collapsed;
                    inputdevicesinfo.Visibility = Visibility.Collapsed;
                    storagedevicesinfo.Visibility = Visibility.Collapsed;
                    printinginfo.Visibility = Visibility.Collapsed;
                    gpuinfo.Visibility = Visibility.Collapsed;
                    networkinfo.Visibility = Visibility.Collapsed;
                    logicboardinfo.Visibility = Visibility.Collapsed;
                    servicesinfo.Visibility = Visibility.Collapsed;
                }
                else if (nvSample.SelectedItem == SoftPrintJobs)
                {
                    printjobsinfo.Visibility = Visibility.Visible;
                    processesinfo.Visibility = Visibility.Collapsed;
                    driverinfo.Visibility = Visibility.Collapsed;
                    softwaree.Visibility = Visibility.Collapsed;
                    osinfo.Visibility = Visibility.Collapsed;
                    syssummary.Visibility = Visibility.Collapsed;
                    cpuinfo.Visibility = Visibility.Collapsed;
                    raminfo.Visibility = Visibility.Collapsed;
                    soundinfo.Visibility = Visibility.Collapsed;
                    hardwaree.Visibility = Visibility.Collapsed;
                    Home.Visibility = Visibility.Collapsed;
                    inputdevicesinfo.Visibility = Visibility.Collapsed;
                    storagedevicesinfo.Visibility = Visibility.Collapsed;
                    printinginfo.Visibility = Visibility.Collapsed;
                    gpuinfo.Visibility = Visibility.Collapsed;
                    networkinfo.Visibility = Visibility.Collapsed;
                    logicboardinfo.Visibility = Visibility.Collapsed;
                    servicesinfo.Visibility = Visibility.Collapsed;
                }
            }
        }
        Application application;
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Das.RequestedTheme = ElementTheme.Dark;
            selectedtheme = "Dark";
            SaveSettings();
        }
        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            Das.RequestedTheme = ElementTheme.Light;
            selectedtheme = "NOTDark";
            SaveSettings();
        }
        public const int ICON_SMALL = 0;
        public const int ICON_BIG = 1;
        public const int ICON_SMALL2 = 2;

        public const int WM_GETICON = 0x007F;
        public const int WM_SETICON = 0x0080;
        private bool TryGoBack()
        {
            if (!contentFrame.CanGoBack)
                return false;

            // Don't go back if the nav pane is overlayed.
            if (nvSample.IsPaneOpen &&
                (nvSample.DisplayMode == NavigationViewDisplayMode.Compact ||
                 nvSample.DisplayMode == NavigationViewDisplayMode.Minimal))
                return false;

            contentFrame.GoBack();
            return true;
        }
        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);
        private List<string> InfoItems = new List<string>()
        {
            "Computer Summary",
            "CPU",
            "GPU",
            "RAM",
            "OS Name",
            "Processor",
            "Memory",
            "PC Name",
            "System Name",
            "OS Architecture",
            "Graphics",
            "CPU Cores",
            "CPU Manufacturer",
            "GPU Status",
            "Adapter RAM",
            "GPU Device ID",
            "Display",
            "Virtual RAM",
            "Memory Page Size",
            "Input Devices",
            "Keyboard",
            "Mouse",
            "Pointing Device",
            "Keyboard Layout",
            "Storage Devices",
            "Disks",
            "Drives",
            "Disk Partitions",
            "Disk Size",
            "Removable Storage",
            "Pendrive",
            "USB",
            "HDD",
            "Hard Disk Drive",
            "SSD",
            "Solid State Drive",
            "Network",
            "Network Adapters",
            "IP Address",
            "IP Gateway",
            "Printing",
            "Printers",
            "Printer Server",
            "Sound",
            "Logic Board",
            "Motherboard",
            "Baseboard",
            "Hardware",
            "Software",
            "BIOS",
            "UEFI",
            "Firmware",
            "Drivers",
            "Services",
            "Processes",
            "Running Tasks",
            "Print Queue",
            "Print Jobs",
            "Operating System",
            "Windows",
            "System",
            "OS Developer",
            "Settings",
        };
        private void SelectionChosen(string selection)
        {
            if (selection == "Computer Summary")
            {
                nvSample.SelectedItem = SysSum;
            }
            else if (selection == "Hardware")
            {
                nvSample.SelectedItem = SysHard;
            }
            else if (selection == "Settings")
            {
                nvSample.SelectedItem = nvSample.SettingsItem;
            }
            else if (selection == "OS Architecture")
            {
                nvSample.SelectedItem = SysOS;
                cpybtn5.Focus(FocusState.Keyboard);
            }
            else if (selection == "CPU" || selection == "Processor" || selection == "CPU Cores" || selection == "CPU Manufacturer")
            {
                nvSample.SelectedItem = HardCPU;
            }
            else if (selection == "GPU" || selection == "Graphics" || selection == "Adapter RAM" || selection == "GPU Device ID" || selection == "GPU Status" || selection == "Display")
            {
                nvSample.SelectedItem = HardGPU;
            }
            else if (selection == "RAM" || selection == "Memory" || selection == "Virtual RAM" || selection == "Memory Page Size")
            {
                nvSample.SelectedItem = HardRAM;
            }
            else if (selection == "OS Name" || selection == "Operating System" || selection == "System" || selection == "Windows" || selection == "OS Developer" || selection == "PC Name" || selection == "System Name")
            {
                nvSample.SelectedItem = osinfoo;
            }
            else if (selection == "Input Devices")
            {
                nvSample.SelectedItem = HardInputDevices;
            }
            else if (selection == "Keyboard" || selection == "Keyboard Layout")
            {
                nvSample.SelectedItem = HardInputDevices;
                inputdevicestabs.SelectedItem = KEYBoardid;
            }
            else if (selection == "Mouse" || selection == "Pointing Device")
            {
                nvSample.SelectedItem = HardInputDevices;
                inputdevicestabs.SelectedItem = Mouseid;
            }
            else if (selection == "Disks" || selection == "Disk Partitions" || selection == "Disk Size")
            {
                nvSample.SelectedItem = HardStorageDevices;
                storagedevicestabs.SelectedItem = diskssd;
            }
            else if (selection == "Drives")
            {
                nvSample.SelectedItem = HardStorageDevices;
                storagedevicestabs.SelectedItem = drivessd;
            }
            else if (selection == "Hardware")
            {
                nvSample.SelectedItem = SysHard;
            }
            else if (selection == "Software")
            {
                nvSample.SelectedItem = SysSoft;
            }
            else if (selection == "Sound")
            {
                nvSample.SelectedItem = HardSound;
            }
            else if (selection == "Drivers")
            {
                nvSample.SelectedItem = SoftDrivers;
            }
            else if (selection == "Services")
            {
                nvSample.SelectedItem = SoftServices;
            }
            else if (selection == "Storage Devices" || selection == "USB" || selection == "Removable Storage" || selection == "HDD" || selection == "Hard Disk Drive" || selection == "SSD" || selection == "Solid State Drive" || selection == "Pendrive")
            {
                nvSample.SelectedItem = HardStorageDevices;
            }
            else if (selection == "Logic Board" || selection == "Motherboard" || selection == "Baseboard" || selection == "BIOS" || selection == "UEFI" || selection == "Firmware")
            {
                nvSample.SelectedItem = HardMotherBoard;
            }
            else if (selection == "Network" || selection == "Network Adapters" || selection == "IP Address" || selection == "IP Gateway")
            {
                nvSample.SelectedItem = HardNetwork;
            }
            else if (selection == "Printing" || selection == "Printers" || selection == "Printer Server")
            {
                nvSample.SelectedItem = HardPrinting;
            }
            else if (selection == "Print Queue" || selection == "Print Jobs")
            {
                nvSample.SelectedItem = SoftPrintJobs;
            }
            else if (selection == "Running Tasks" || selection == "Processes")
            {
                nvSample.SelectedItem = SoftProcesses;
            }
        }
        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            SelectionChosen(args.SelectedItem.ToString());
        }
        string selectedtheme;
        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            Das.RequestedTheme = ElementTheme.Default;
            selectedtheme = "WindowsDefault";
            SaveSettings();
        }
        private void SaveSettings()
        {
            try
            {
                if (DefaultLoad.SelectedItem.ToString() != null)
                {
                    settingtobesaved = "COMPInfo Windows Settings File Version 1.0.0 -- NOT TO BE MODIFIED" + Environment.NewLine + Environment.NewLine +
    "THEME:[USER SET SETTING]" + Environment.NewLine + Environment.NewLine +
    "Theme[]CompInfo_OC == " + selectedtheme + Environment.NewLine + Environment.NewLine +
    "DEFAULTLOADPAGE:[USER SET SETTING]" + Environment.NewLine + Environment.NewLine +
    "DefaultLoadPage[]CompInfo_OC == " + DefaultLoad.SelectedIndex.ToString() + Environment.NewLine + Environment.NewLine +
    "NavigationalStyle[]CompInfo_OC == " + MenuAlignment.SelectedIndex.ToString() + Environment.NewLine + Environment.NewLine +
    "THESE SETTINGS ARE AUTO-GENERATED BY COMPINFO AND SHOULD NOT BE MODIFIED AS MODIFIYING IT WILL LEAD TO UNEXPECTED BEHAVIOUR OF THE APP.";
                }
                string settingsdirectory = Directory.GetCurrentDirectory() + "\\settings.ocs.txt";
                File.WriteAllText(settingsdirectory, settingtobesaved);
                SetTitleBarColors();
            }
            catch (Exception ex)
            {
                errorlog += DateTime.Now + ":: " + ex.Message + Environment.NewLine;
                File.WriteAllText(@"C:\Users\bnara\Documents\log.txt", errorlog);
            }
        }
        string settingtobesaved;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.cpybtn1.Flyout is Flyout f)
            {
                f.Hide();
            }
        }
        DataPackage dataPackage = new DataPackage();
        private void cpybtn1_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(Osname.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void cpybtn2_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(Osver.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var suitableItems = new List<string>();
                var splitText = sender.Text.ToLower().Split(" ");
                foreach (var infoitem in InfoItems)
                {
                    var found = splitText.All((key) =>
                    {
                        return infoitem.ToLower().Contains(key);
                    });
                    if (found)
                    {
                        suitableItems.Add(infoitem);
                    }
                }
                if (suitableItems.Count == 0)
                {
                    suitableItems.Add("No results found");
                }
                sender.ItemsSource = suitableItems;
            }
        }

        private void cpybtn1_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void cpybtn2_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void cpybtn3_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(Osdev.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn4_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(pcname.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn5_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(Osarch.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn6_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(Buildtype.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn7_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(regist.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn8_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(sysdirec.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn9_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(osname.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn10_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(processor.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn11_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(ram.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn12_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(sysname.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn13_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(osarc.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn14_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(gpusummary.Text);
            Clipboard.SetContent(dataPackage);
        }
        private async void explrbtn1_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder storagefolder = await StorageFolder.GetFolderFromPathAsync(sysdirec.Text);
            await Launcher.LaunchFolderAsync(storagefolder);
        }

        private void DefaultLoad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveSettings();
        }

        private void cpybtn15_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(TOTALRam.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn16_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(AVAILRam.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn17_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(TOTALViRam.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn18_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(AVAILViRam.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn19_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(MemPageSize.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            nvSample.SelectedItem = HardRAM;
        }

        private void MenuAlignment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MenuAlignment.SelectedIndex == 0)
            {
                nvSample.PaneDisplayMode = NavigationViewPaneDisplayMode.Top;
            }
            else
            {
                nvSample.PaneDisplayMode = NavigationViewPaneDisplayMode.LeftCompact;
            }
            SaveSettings();
        }

        private void restartbtn1_Click(object sender, RoutedEventArgs e)
        {
            LoadRAMInfo();
        }

        private void cpybtn20_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(CPUName.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn21_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(CPUManufacturer.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn22_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(LogicProcessors.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn23_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(Cores.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void restartbtn2_Click(object sender, RoutedEventArgs e)
        {
            LoadCPUInfo();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            nvSample.SelectedItem = HardCPU;
        }

        private void nvSample_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            TryGoBack();
        }

        private void nvSample_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
        }

        private void contentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            //         nvSample.IsBackEnabled = contentFrame.CanGoBack;

            //  if (contentFrame.SourcePageType == typeof(())
            //    {
            // SettingsItem is not part of nvSample.MenuItems, and doesn't have a Tag.
            //    nvSample.SelectedItem = (NavigationViewItem)nvSample.SettingsItem;
            //   nvSample.Header = "Settings";
            // }
            //   else if (contentFrame.SourcePageType != null)
            //    {
            //      var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);
            //
            //    nvSample.SelectedItem = nvSample.MenuItems
            //       .OfType<muxc.NavigationViewItem>()
            //        .First(n => n.Tag.Equals(item.Tag));

            //      nvSample.Header =
            //        ((muxc.NavigationViewItem)nvSample.SelectedItem)?.Content?.ToString();
            //    }
        }

        private async void joindevpre_Click(object sender, RoutedEventArgs e)
        {
            ContentDialogResult result = await joinbeta.ShowAsync();
        }

        private void ConfirmAgeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            joinbeta.IsPrimaryButtonEnabled = true;
        }

        private void ConfirmAgeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            joinbeta.IsPrimaryButtonEnabled = false;
        }

        private async void abtbtn_Click(object sender, RoutedEventArgs e)
        {
            ContentDialogResult result = await aboutdialog.ShowAsync();
        }

        private void cpybtn24_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(soundname.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn25_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(soundmanufacturer.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void restartbtn3_Click(object sender, RoutedEventArgs e)
        {
            LoadSoundInfo();
        }

        private void cpybtn26_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(sounddevidentity);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn27_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(sounddescription.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            nvSample.SelectedItem = HardSound;
        }
        string sounddevidentity;
        private void ListSoundDevs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ObjectQuery SoundObject = new ObjectQuery("SELECT * FROM Win32_SoundDevice");
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(SoundObject);
            string SDname = "Unknown";
            string SDmanufacturer = "Unknown";
            string SDstatus = "Unknown";
            string SDid = "Unknown";
            string SDavailability = "Unknown";
            foreach (ManagementObject obj in managementObjectSearcher.Get())
            {
                if (ListSoundDevs.Items.Count != 0)
                {
                    if (ListSoundDevs.SelectedItem.ToString() == obj["Caption"].ToString())
                    {
                        SDname = obj["Name"].ToString();
                        SDmanufacturer = obj["Manufacturer"].ToString();
                        SDstatus = obj["Status"].ToString();
                        SDid = obj["DeviceID"].ToString();
                        SDavailability = obj["Description"].ToString();
                    }
                }
            }
            soundname.Text = SDname;
            SoundDeviceHeader.Text = SDname;
            soundmanufacturer.Text = SDmanufacturer;
            sounddevid.Text = SDid;
            sounddevidentity = SDid;
            sounddescription.Text = SDavailability;
            if (sounddevid.Text.Length > 39)
            {
                sounddevid.Text = string.Concat(sounddevid.Text.Substring(0, 39), "...");
            }
            else
            {
                sounddevid.Text = SDid;
            }
        }

        private void ListStorageDevs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                errorlog += DateTime.Now + ":: " + ex.Message + Environment.NewLine;
                File.WriteAllText(@"C:\Users\bnara\Documents\log.txt", errorlog);
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (Passwordtxt.Password.Length < 8)
            {
                PasswordVerify1.Content = "❌";
            }
            else
            {
                PasswordVerify1.Content = "✔️";
            }
            if (ConfirmPasswordtxt.Password != Passwordtxt.Password)
            {
                PasswordVerify2.Content = "❌";
            }
            else
            {
                PasswordVerify2.Content = "✔️";
            }
        }

        private void PasswordBox_PasswordChanged_1(object sender, RoutedEventArgs e)
        {
            if (ConfirmPasswordtxt.Password != Passwordtxt.Password)
            {
                PasswordVerify2.Content = "❌";
            }
            else
            {
                PasswordVerify2.Content = "✔️";
            }
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Name == "PasswordVerify1")
            {
                if (btn.Content == "❌")
                {
                    passwordwarn.Text = "Your password is less than 8 characters";
                    PasswordVerifyWarn.Target = Passwordtxt;
                    PasswordVerifyWarn.IsOpen = true;
                }
            }
            else if (btn.Name == "PasswordVerify2")
            {
                if (btn.Content == "❌")
                {
                    passwordwarn.Text = "Passwords do not match";
                    PasswordVerifyWarn.Target = ConfirmPasswordtxt;
                    PasswordVerifyWarn.IsOpen = true;
                }
            }
        }
        private static Regex email_validation()
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            return new Regex(pattern, RegexOptions.IgnoreCase);
        }
        static Regex validate_emailaddress = email_validation();
        private void Emailtxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (email_validation().IsMatch(Emailtxt.Text) == true)
            {
                PasswordVerifyWarn.Target = Emailtxt;
                passwordwarn.Text = "This e-mail is not valid";
                infobar1.IsOpen = false;
                PasswordVerifyWarn.IsOpen = false;
            }
            else
            {
                PasswordVerifyWarn.Target = Emailtxt;
                passwordwarn.Text = "This e-mail is not valid";
                infobar1.Message = "There are some errors that need to rechecked";
                infobar1.Severity = InfoBarSeverity.Error;
                infobar1.IsOpen = true;
                PasswordVerifyWarn.IsOpen = true;
            }
        }
        string rem;
        private void EmailVerify_Click(object sender, RoutedEventArgs e)
        {
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void ListKeyboardDevs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListKeyboardDevs.SelectedIndex != -1)
            {
                KeyboardDeviceHeader.Text = ListKeyboardDevs.SelectedItem.ToString();
                ObjectQuery diskquery2 = new ObjectQuery("SELECT * FROM CIM_Keyboard");
                ManagementObjectSearcher objectSearcher2 = new ManagementObjectSearcher(diskquery2);
                foreach (ManagementObject item in objectSearcher2.Get())
                {
                    if (ListKeyboardDevs.SelectedItem.ToString() == (item["Caption"]).ToString())
                    {
                        if (item["Name"].ToString != null)
                        {
                            keybaordname.Text = item["Name"].ToString();
                        }
                        else
                        {
                            keybaordname.Text = "Inaccessible Data";
                        }
                        if (item["Description"].ToString != null)
                        {
                            keyboarddescription.Text = item["Description"].ToString();
                        }
                        else
                        {
                            keyboarddescription.Text = "Inaccessible Data";
                        }
                        if (item["DeviceID"] != null)
                        {
                            keyboarddevid.Text = item["DeviceID"].ToString();
                        }
                        else
                        {
                            keyboarddevid.Text = "Inaccessible Data";
                        }
                        if (item["Layout"] != null)
                        {
                            keyboardlayout.Text = item["Layout"].ToString();
                        }
                        else
                        {
                            keyboardlayout.Text = "Inaccessible Data";
                        }
                        if (item["NumberOfFunctionKeys"] != null)
                        {
                            keyboardfnkeys.Text = item["NumberOfFunctionKeys"].ToString();
                        }
                        else
                        {
                            keyboardfnkeys.Text = "Inaccessible Data";
                        }
                    }
                }
            }
        }

        private void cpybtn28_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(keyboarddescription.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn29_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(keybaordname.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn30_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(keyboarddevid.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn31_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(keyboardlayout.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn32_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(keyboardfnkeys.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void inputdevicestabs_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (inputdevicestabs.SelectedItem == KEYBoardid)
            {
                KeyboardInfo.Visibility = Visibility.Visible;
                PDInfo.Visibility = Visibility.Collapsed;
            }
            else
            {
                KeyboardInfo.Visibility = Visibility.Collapsed;
                PDInfo.Visibility = Visibility.Visible;
            }
        }

        private void ListPointingDevs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListPointingDevs.SelectedIndex != -1)
            {
                PDeviceHeader.Text = ListPointingDevs.SelectedItem.ToString();
                ObjectQuery diskquery2 = new ObjectQuery("SELECT * FROM CIM_PointingDevice");
                ManagementObjectSearcher objectSearcher2 = new ManagementObjectSearcher(diskquery2);
                foreach (ManagementObject item in objectSearcher2.Get())
                {
                    if (ListPointingDevs.SelectedItem.ToString() == (item["Caption"]).ToString())
                    {
                        string PDdesc = item["PointingType"].ToString();
                        if (PDdesc == "1")
                        {
                            PDdesc = "Other";
                        }
                        else if (PDdesc == "2")
                        {
                            PDdesc = "Unknown";
                        }
                        else if (PDdesc == "3")
                        {
                            PDdesc = "Mouse";
                        }
                        else if (PDdesc == "4")
                        {
                            PDdesc = "Track Ball";
                        }
                        else if (PDdesc == "5")
                        {
                            PDdesc = "Track Point";
                        }
                        else if (PDdesc == "6")
                        {
                            PDdesc = "Glide Point";
                        }
                        else if (PDdesc == "7")
                        {
                            PDdesc = "Touch Pad";
                        }
                        else if (PDdesc == "9")
                        {
                            PDdesc = "Mouse Optical Sensor";
                        }
                        else
                        {
                            PDdesc = "Unspecified";
                        }
                        pdtype.Text = PDdesc;
                        pdname.Text = item["Name"].ToString();
                        pointingdevid.Text = item["DeviceID"].ToString();
                        pdstatus.Text = item["Status"].ToString();
                        pdbuttons.Text = item["NumberOfButtons"].ToString();
                    }
                }
            }
        }

        private void cpybtn33_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(pdtype.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn34_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(pdname.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn35_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(pointingdevid.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn36_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(pdstatus.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn37_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(pdbuttons.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void restartbtn5_Click(object sender, RoutedEventArgs e)
        {
            LoadPDInfo();
        }

        private void restartbtn4_Click(object sender, RoutedEventArgs e)
        {
            LoadKeyboardInfo();
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            nvSample.SelectedItem = HardInputDevices;
        }

        private void emailverifydialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
        Random r = new Random();
        int randNum;
        string verfiycode;
        private void EmailSender()
        {
            try
            {
                Progress.Visibility = Visibility.Visible;
                randNum = r.Next(1000000);
                verfiycode = randNum.ToString("D6");

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("mytechapplicationsfeedback@gmail.com");
                mail.To.Add(Emailtxt.Text);
                mail.Subject = "Verification Code - CompInfo";
                mail.Body = "<h4>" + "Hello! Please find the email-verification code for CompInfo below: " + "<br><br>" +
                    "Code: " + verfiycode + "<br>" +
                    "Enter this code in the Email Verification dialog. Don't share this code with anyone. If you did not request for this email verification, kindly ignore this e-mail " + "<br>"
                  + "© OceanCyan Tech" + "<br>";
                mail.IsBodyHtml = true;
                SmtpClient smpt = new SmtpClient("smtp.gmail.com", 587);
                NetworkCredential SMTPUserInfo = new NetworkCredential("mytechapplicationsfeedback@gmail.com", "7thoct2008");
                smpt.Credentials = SMTPUserInfo;
                smpt.EnableSsl = true;
                smpt.Send(mail);
                Progress.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                errorlog += DateTime.Now + ":: " + ex.Message + Environment.NewLine;
                File.WriteAllText(@"C:\Users\bnara\Documents\log.txt", errorlog);
            }
        }
        private async void signupbtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void VerifyCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (VerifyCode.Text == verfiycode)
            {
                emailverifydialog.IsPrimaryButtonEnabled = true;
                Teachingtip.IsOpen = false;
            }
            else
            {
                emailverifydialog.IsPrimaryButtonEnabled = false;
                teachtiptext1.Text = "The code is incorrect";
                Teachingtip.Target = VerifyCode;
                Teachingtip.IsOpen = true;
            }
        }

        private void backbtn_Click(object sender, RoutedEventArgs e)
        {
            SignUpPanel.Visibility = Visibility.Collapsed;
            Case_NotSigned.Visibility = Visibility.Visible;
        }
        string username;
        private async void signinbtn_Click(object sender, RoutedEventArgs e)
        {
        }
        private void rememberme_Checked(object sender, RoutedEventArgs e)
        {
        }
        private void rememberme_Unchecked(object sender, RoutedEventArgs e)
        {
        }

        private void backbtn2_Click(object sender, RoutedEventArgs e)
        {
            SignInPanel.Visibility = Visibility.Collapsed;
            Case_NotSigned.Visibility = Visibility.Visible;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Passwordtxtsignin.PasswordRevealMode = PasswordRevealMode.Visible;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Passwordtxtsignin.PasswordRevealMode = PasswordRevealMode.Hidden;
        }

        private void SignOutbtn_Click(object sender, RoutedEventArgs e)
        {
        }
        public int CountDown2_Seconds = 2;
        private void Timer_Tick1(object sender, object e)
        {
            try
            {
                CountDown2_Seconds = CountDown2_Seconds - 1;
                if (CountDown2_Seconds == 0)
                {
                    timer.Stop();
                    txtusername2.Text = "";
                    Passwordtxtsignin.Password = "";
                    string settingsdirectory = Directory.GetCurrentDirectory() + "\\compinfosignpaen.ocs.txt";
                    File.WriteAllText(settingsdirectory, "");
                    string settingsdirectory2 = Directory.GetCurrentDirectory() + "\\compinfosignsecuen.ocs.txt";
                    File.WriteAllText(settingsdirectory2, "");

                }
            }
            catch (Exception ex)
            {
                errorlog += DateTime.Now + ":: " + ex.Message + Environment.NewLine;
                File.WriteAllText(@"C:\Users\bnara\Documents\log.txt", errorlog);
            }
        }

        private void AcctDetailsbtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void AcctDetailsbtn_Click_1(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
         
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void accdet_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
            }
            catch (Exception ex)
            {
                errorlog += DateTime.Now + ":: " + ex.Message + Environment.NewLine;
                File.WriteAllText(@"C:\Users\bnara\Documents\log.txt", errorlog);
            }
        }

        private void storagedevicestabs_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (storagedevicestabs.SelectedItem == drivessd)
            {
                DisksInfo.Visibility = Visibility.Collapsed;
                DrivesInfo.Visibility = Visibility.Visible;
            }
            else if (storagedevicestabs.SelectedItem == diskssd)
            {
                DisksInfo.Visibility = Visibility.Visible;
                DrivesInfo.Visibility = Visibility.Collapsed;
            }
        }
        int diskno;
        private void ListDiskDevs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ListDiskDevs.SelectedIndex != -1)
                {
                    ObjectQuery diskquery = new ObjectQuery("SELECT * FROM Win32_DiskDrive");
                    ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher(diskquery);
                    foreach (ManagementObject item in objectSearcher.Get())
                    {
                        if (ListDiskDevs.SelectedItem.ToString() == item["Caption"].ToString())
                        {
                            DiskHeader.Text = item["Caption"].ToString();
                            diskmodel.Text = item["Model"].ToString();
                            diskdescription.Text = item["Description"].ToString();
                            diskdevid.Text = item["DeviceID"].ToString();
                            bypesse.Text = item["BytesPerSector"].ToString();
                            manufacturerdisk.Text = item["Manufacturer"].ToString();
                            string Size = item["Size"].ToString();
                            diskno = Convert.ToInt32(item["Index"]);
                            var Siz2 = Double.Parse(Size);
                            totaldisksize.Text = (Siz2 / 1e+9).ToString("#.##") + " GB";
                            mediatype.Text = item["MediaType"].ToString();
                            noofpart.Text = item["Partitions"].ToString();
                            totaldiskcyl.Text = item["TotalCylinders"].ToString();
                            disksectors.Text = item["TotalSectors"].ToString();
                            disktracks.Text = item["TotalTracks"].ToString();
                            scsibus.Text = item["SCSIBus"].ToString();
                            scsiport.Text = item["SCSIPort"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RecordofException(ex);
            }
        }
        private void RecordofException(Exception exceptionstring)
        {
            logtextrealtime += DateTime.Now + ":: " + exceptionstring.Message;
            WriteLog();
        }
        private void WriteLog()
        {
            //log_en.txt - Write any given exceptions or invalid parameters
            File.AppendAllText(logfile, logtextrealtime);
        }
        string logtextrealtime;
        string logfile = Environment.CurrentDirectory + "\\log_en.txt";
        private void cpybtn38_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(diskdescription.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn39_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(diskmodel.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn40_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(diskdevid.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn41_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(bypesse.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn42_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(manufacturerdisk.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn44_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(mediatype.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn45_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(noofpart.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn46_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(totaldiskcyl.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn44_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void cpybtn44_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void cpybtn47_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(disksectors.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn48_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(disktracks.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn49_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(scsibus.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn50_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(scsiport.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void restartbtn5_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void restartbtn6_Click(object sender, RoutedEventArgs e)
        {
            LoadDisks();
        }

        private void ListLDDevs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ListLDDevs.SelectedIndex != -1)
                {
                    ObjectQuery diskquery2 = new ObjectQuery("SELECT * FROM Win32_LogicalDisk");
                    ManagementObjectSearcher objectSearcher2 = new ManagementObjectSearcher(diskquery2);
                    foreach (ManagementObject item in objectSearcher2.Get())
                    {
                        if (ListLDDevs.SelectedItem.ToString() == (item["Caption"]).ToString())
                        {
                            //Get the drive letter
                            driveletter.Text = item["Caption"].ToString();
                            DriveHeader.Text = driveletter.Text;
                            //Get the type of file system of the drive
                            if (item["FileSystem"] != null)
                            {
                                filesystem.Text = item["FileSystem"].ToString();
                            }
                            else
                            {
                                availspace.Text = "Inaccessible Data";
                            }
                            //Get the total size of the partition drive
                            var totalsize = 1e+9;
                            if (item["Size"] != null)
                            {
                                string Size = item["Size"].ToString();
                                var Siz2 = Double.Parse(Size);
                                totalsize = Siz2 / 1e+9;
                                drivesize.Text = (Siz2 / 1e+9).ToString("#.##") + " GB";
                                drivesizeoccupy.Maximum = (Siz2 / 1e+9);
                                drivesizeoccupy.IsIndeterminate = false;
                            }
                            else
                            {
                                drivesize.Text = "Unknown";
                                drivesizeoccupy.IsIndeterminate = true;
                                usedspace.Text = "Unable to calculate. Invalid parameters...";
                            }
                            if (item["FreeSpace"] != null)
                            {
                                string Size2 = item["FreeSpace"].ToString();
                                var Siz3 = Double.Parse(Size2);
                                var totalusedspace = totalsize - (Siz3 / 1e+9);
                                drivesizeoccupy.Value = Siz3 / 1e+9;
                                availspace.Text = (Siz3 / 1e+9).ToString("#.##") + " GB";
                                usedspace.Text = (Siz3 / 1e+9).ToString("#.##") + " GB available";
                            }
                            else
                            {
                                availspace.Text = "Inaccessible Data";
                            }
                            if (item["VolumeName"] != null)
                            {
                                volumename.Text = item["VolumeName"].ToString();
                                if (volumename.Text == "")
                                {
                                    volumename.Text = "Inaccessible Data";
                                }
                            }
                            else
                            {
                                volumename.Text = "Inaccessible Data";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RecordofException(ex);
            }
        }

        private void cpybtn51_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(driveletter.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn52_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(drivesize.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn53_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(availspace.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn54_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(filesystem.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void restartbtn7_Click(object sender, RoutedEventArgs e)
        {
            LoadDisks();
        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            nvSample.SelectedItem = HardStorageDevices;
        }

        private void cpybtn43_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(totaldisksize.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn55_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ListPrintingDevs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListPrintingDevs.SelectedIndex != -1)
            {
                PrintingDeviceHeader.Text = ListPrintingDevs.SelectedItem.ToString();
                ObjectQuery diskquery2 = new ObjectQuery("SELECT * FROM Win32_Printer");
                ManagementObjectSearcher objectSearcher2 = new ManagementObjectSearcher(diskquery2);
                foreach (ManagementObject item in objectSearcher2.Get())
                {
                    if (ListPrintingDevs.SelectedItem.ToString() == (item["Caption"]).ToString())
                    {
                        if (item["Caption"].ToString != null)
                        {
                            printername.Text = item["Caption"].ToString();
                        }
                        else
                        {
                            printername.Text = "Inaccessible Data";
                        }
                        if (item["PortName"].ToString != null)
                        {
                            portname.Text = item["PortName"].ToString();
                            if (portname.Text.Length > 39)
                            {
                                portname.Text = string.Concat(portname.Text.Substring(0, 39), "...");
                            }
                        }
                        else
                        {
                            portname.Text = "Inaccessible Data";
                        }
                        if (item["ServerName"] != null)
                        {
                            servername.Text = item["ServerName"].ToString();
                        }
                        else
                        {
                            servername.Text = "Local Server";
                        }
                        if (item["DriverName"] != null)
                        {
                            driver.Text = item["DriverName"].ToString();
                        }
                        else
                        {
                            driver.Text = "Inaccessible Data";
                        }
                    }
                }
            }
        }

        private void cpybtn56_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(printername.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn57_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(portname.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn58_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(servername.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn59_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(driver.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void restartbtn8_Click(object sender, RoutedEventArgs e)
        {
            LoadPrintingInfo();
        }

        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            nvSample.SelectedItem = HardPrinting;
        }

        private void ListGPUDevs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListGPUDevs.SelectedIndex != -1)
            {
                ObjectQuery diskquery2 = new ObjectQuery("SELECT * FROM Win32_VideoController");
                ManagementObjectSearcher objectSearcher2 = new ManagementObjectSearcher(diskquery2);
                foreach (ManagementObject item in objectSearcher2.Get())
                {
                    if (ListGPUDevs.SelectedItem.ToString() == (item["Caption"]).ToString())
                    {
                        gpustatus.Text = item["Status"].ToString();
                        var AdapterRam = item["AdapterRam"].ToString();
                        adapterram.Text = (Double.Parse(AdapterRam) / 1e+9).ToString("#.#") + " GB";
                        gpudevid.Text = item["DeviceID"].ToString();
                        ismonochrome.Text = item["Monochrome"].ToString();
                        driverversion.Text = item["DriverVersion"].ToString();
                    }
                }
            }
        }

        private void cpybtn60_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(gpuname.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn61_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(gpustatus.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn62_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(adapterram.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn63_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(gpudevid.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void restartbtn9_Click(object sender, RoutedEventArgs e)
        {
            LoadDisplayInfo();
        }

        private void cpybtn64_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(ismonochrome.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void cpybtn65_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(driverversion.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void Button_Click_13(object sender, RoutedEventArgs e)
        {
            nvSample.SelectedItem = HardGPU;
        }

        private void restartbtn10_Click(object sender, RoutedEventArgs e)
        {
            LoadNetworkInfo();
        }

        private void Networkitemsname_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void networkadapterinfo_Click(object sender, RoutedEventArgs e)
        {
            if (Networkitemsname.SelectedIndex != -1)
            {
                Networkinfo.Title = Networkitemsname.SelectedItem.ToString();
                ObjectQuery diskquery2 = new ObjectQuery("SELECT * FROM Win32_NetworkAdapterConfiguration");
                ManagementObjectSearcher objectSearcher2 = new ManagementObjectSearcher(diskquery2);
                foreach (ManagementObject item in objectSearcher2.Get())
                {
                    if (Networkitemsname.SelectedItem.ToString() == (item["Caption"]).ToString())
                    {
                        if (item["Caption"] != null)
                        {
                            adaptername.Text = item["Caption"].ToString();
                        }
                        else
                        {
                            adaptername.Text = "Inaccessible Data";
                        }
                        if (item["IPAddress"] != null)
                        {
                            ipaddress.Text = "";
                            string[] arrIPAddress = (string[])(item["IPAddress"]);
                            string sIPAddress = arrIPAddress.FirstOrDefault(s => s.Contains('.'));
                            ipaddress.Text = sIPAddress;
                        }
                        else
                        {
                            ipaddress.Text = "Inaccessible Data";
                        }
                        if (item["DefaultIPGateway"] != null)
                        {
                            defip.Text = "";
                            string[] arrIPAddress = (string[])(item["DefaultIPGateway"]);
                            string sIPAddress = arrIPAddress.FirstOrDefault(s => s.Contains('.'));
                            defip.Text = sIPAddress;
                        }
                        else
                        {
                            defip.Text = "Inaccessible Data";
                        }
                        if (item["ServiceName"] != null)
                        {
                            servicenamenetwork.Text = item["ServiceName"].ToString();
                        }
                        else
                        {
                            servicenamenetwork.Text = "Inaccessible Data";
                        }
                    }
                }
                ObjectQuery diskquery3 = new ObjectQuery("SELECT * FROM Win32_NetworkAdapter");
                ManagementObjectSearcher objectSearcher3 = new ManagementObjectSearcher(diskquery3);
                foreach (ManagementObject item in objectSearcher3.Get())

                {
                    if (Networkitemsname.SelectedItem.ToString() == (item["Caption"]).ToString())
                    {
                        if (item["AdapterType"] != null)
                        {
                            adaptertype.Text = item["AdapterType"].ToString();
                        }
                        else
                        {
                            adaptertype.Text = "Value Null";
                        }
                        if (item["Manufacturer"] != null)
                        {
                            naman.Text = item["Manufacturer"].ToString();
                        }
                        else
                        {
                            naman.Text = "Unknown";
                        }
                    }
                }
                ContentDialogResult result = await Networkinfo.ShowAsync();
            }
        }

        private void Button_Click_14(object sender, RoutedEventArgs e)
        {
            nvSample.SelectedItem = HardNetwork;
        }

        private void restartbtn11_Click(object sender, RoutedEventArgs e)
        {
            LoadMotherBoardInfo();
        }

        private void Button_Click_15(object sender, RoutedEventArgs e)
        {
            nvSample.SelectedItem = HardMotherBoard;
        }

        private void Button_Click_16(object sender, RoutedEventArgs e)
        {
            nvSample.SelectedItem = SoftDrivers;
        }

        private async void driverinfobtn_Click(object sender, RoutedEventArgs e)
        {
            if (Driveritemname.SelectedIndex != -1)
            {
                ObjectQuery sysdrivobj = new ObjectQuery("SELECT * FROM Win32_SystemDriver");
                ManagementObjectSearcher sysdrivobjsearch = new ManagementObjectSearcher(sysdrivobj);
                foreach (ManagementObject obj in sysdrivobjsearch.Get())
                {
                    if (Driveritemname.SelectedItem.ToString() == obj["Caption"].ToString())
                    {
                        drivername.Text = obj["Name"].ToString();
                        DRIVERinfo.Title = Driveritemname.SelectedItem.ToString();
                        driverdescription.Text = obj["Description"].ToString();
                        drivererrorcontrol.Text = obj["ErrorControl"].ToString();
                        driverexitcode.Text = obj["ExitCode"].ToString();
                        driverpathname.Text = obj["PathName"].ToString();
                        driverservicetype.Text = obj["ServiceType"].ToString();
                        driverstartmode.Text = obj["StartMode"].ToString();
                        driverstate.Text = obj["State"].ToString();
                        driverstatus.Text = obj["Status"].ToString();
                        driveracceptstop.Text = obj["AcceptStop"].ToString();
                        if (driveracceptstop.Text == "False")
                        {
                            driveracceptstop.Text = "No";
                        }
                        else if (driveracceptstop.Text == "True")
                        {
                            driveracceptstop.Text = "Yes";
                        }
                    }
                }
            }
            ContentDialogResult result = await DRIVERinfo.ShowAsync();
        }

        private void restartbtn12_Click(object sender, RoutedEventArgs e)
        {
            LoadDriverInfo();
        }

        private async void serviceinfobtn_Click(object sender, RoutedEventArgs e)
        {
            if (Driveritemname.SelectedIndex != -1)
            {
                ObjectQuery sysdrivobj = new ObjectQuery("SELECT * FROM Win32_Service");
                ManagementObjectSearcher sysdrivobjsearch = new ManagementObjectSearcher(sysdrivobj);
                foreach (ManagementObject obj in sysdrivobjsearch.Get())
                {
                    if (serviceitemname.SelectedItem.ToString() == obj["Caption"].ToString())
                    {
                        servicename.Text = obj["Name"].ToString();
                        SERVICEInfo.Title = serviceitemname.SelectedItem.ToString();
                        servicedescription.Text = obj["Description"].ToString();
                        serviceerrorcontrol.Text = obj["ErrorControl"].ToString();
                        serviceexitcode.Text = obj["ExitCode"].ToString();
                        servicepathname.Text = obj["PathName"].ToString();
                        serviceservicetype.Text = obj["ServiceType"].ToString();
                        servicestartmode.Text = obj["StartMode"].ToString();
                        servicestate.Text = obj["State"].ToString();
                        servicestatus.Text = obj["Status"].ToString();
                        serviceacceptstop.Text = obj["AcceptStop"].ToString();
                        if (serviceacceptstop.Text == "False")
                        {
                            serviceacceptstop.Text = "No";
                        }
                        else if (serviceacceptstop.Text == "True")
                        {
                            serviceacceptstop.Text = "Yes";
                        }
                    }
                }
            }
            ContentDialogResult result = await SERVICEInfo.ShowAsync();
        }

        private void restartbtn13_Click(object sender, RoutedEventArgs e)
        {
            LoadServicesInfo();
        }

        private void Button_Click_17(object sender, RoutedEventArgs e)
        {
            nvSample.SelectedItem = SoftServices;
        }

        private async void launchservicesbtn_Click_1(object sender, RoutedEventArgs e)
        {
            Process CmdProcess = new Process();
            CmdProcess.StartInfo.FileName = "cmd.exe";
            CmdProcess.StartInfo.CreateNoWindow = true;
            CmdProcess.StartInfo.UseShellExecute = false;
            CmdProcess.StartInfo.RedirectStandardInput = true;
            CmdProcess.StartInfo.RedirectStandardOutput = true;
            CmdProcess.StartInfo.RedirectStandardError = true;
            CmdProcess.StartInfo.Arguments = "/c " + "services.msc";
            CmdProcess.Start();
            CmdProcess.StandardOutput.ReadToEnd();
            CmdProcess.WaitForExit();
            CmdProcess.Close();
        }

        private void restartbtn14_Click(object sender, RoutedEventArgs e)
        {
            LoadProcessesInfo();
        }

        private void processitemsname_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (processitemsname.SelectedIndex != -1)
            {
                InfoProcesses.Visibility = Visibility.Visible;
                ObjectQuery sysproceobj = new ObjectQuery("SELECT * FROM Win32_Process");
                ManagementObjectSearcher sysprocessesobjsearch = new ManagementObjectSearcher(sysproceobj);
                foreach (ManagementObject obj in sysprocessesobjsearch.Get())
                {
                    if (processitemsname.SelectedItem.ToString() == obj["Caption"].ToString())
                    {
                        processname.Text = obj["Name"].ToString();
                        processid.Text = obj["ProcessID"].ToString();
                        processversion.Text = obj["WindowsVersion"].ToString();
                        if (obj["ExecutablePath"] != null)
                        {
                            processpath.Text = obj["ExecutablePath"].ToString();
                        }
                        else
                        {
                            processpath.Text = "Not Available";
                        }
                        string input = obj["CreationDate"].ToString();
                        string year = input.Substring(0, 4);
                        string month = input.Substring(4, 2);
                        string day = input.Substring(6, 2);
                        string hour = input.Substring(8, 2);
                        string minutes = input.Substring(10, 2);
                        processcreationdate.Text = year + "-" + month + "-" + day + " " + hour + ":" + minutes;
                        if (obj["Status"] != null)
                        {
                            processstatus.Text = obj["Status"].ToString();
                        }
                        else
                        {
                            processstatus.Text = "Unknown";
                        }
                        processpriority.Text = obj["Priority"].ToString();
                    }
                }
            }
        }
        DispatcherTimer realtimeupdatertimer = new DispatcherTimer();
        private void launchtaskmanager_Click(object sender, RoutedEventArgs e)
        {
            Process CmdProcess = new Process();
            CmdProcess.StartInfo.FileName = "cmd.exe";
            CmdProcess.StartInfo.CreateNoWindow = true;
            CmdProcess.StartInfo.UseShellExecute = false;
            CmdProcess.StartInfo.RedirectStandardInput = true;
            CmdProcess.StartInfo.RedirectStandardOutput = true;
            CmdProcess.StartInfo.RedirectStandardError = true;
            CmdProcess.StartInfo.Arguments = "/c " + "Taskmgr.exe";
            CmdProcess.Start();
            CmdProcess.StandardOutput.ReadToEnd();
            CmdProcess.WaitForExit();
            CmdProcess.Close();
        }

        ContentDialog dialog = new ContentDialog();
        private async void endtaskbtn_Click(object sender, RoutedEventArgs e)
        {

        }
        ListView listView = new ListView();
        ListView listView2 = new ListView();
        private void SearchProcesses_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
        }

        private void SearchProcesses_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var suitableItems = new List<string>();
                var splitText = sender.Text.ToLower().Split(" ");
                foreach (string infoitem in listView.Items)
                {
                    var found = splitText.All((key) =>
                    {
                        return infoitem.ToLower().Contains(key);
                    });
                    if (found)
                    {
                        suitableItems.Add(infoitem);
                    }
                }
                if (suitableItems.Count == 0)
                {
                    suitableItems.Add("No results found!");
                }
                sender.ItemsSource = suitableItems;
            }
        }

        private void SearchProcesses_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            string selection = args.SelectedItem.ToString();
            processitemsname.SelectedItem = selection;
            processitemsname.ScrollIntoView(selection);
        }

        private void Button_Click_18(object sender, RoutedEventArgs e)
        {
            nvSample.SelectedItem = SoftProcesses;
        }

        private void Button_Click_19(object sender, RoutedEventArgs e)
        {
            try
            {
                Process process = Process.GetProcessById(Convert.ToInt32(processid.Text));
                process.Kill();
            }
            catch (Exception EX)
            {
                RecordofException(EX);
            }
            LoadProcessesInfo();
            flyt.Hide();
        }

        private void realtimeupdatetoggle_IsCheckedChanged(ToggleSplitButton sender, ToggleSplitButtonIsCheckedChangedEventArgs args)
        {
            if (realtimeupdatetoggle.IsChecked == true)
            {
                realtimeupdatertimer.Interval = new TimeSpan(0, 0, 1);
                realtimeupdatertimer.Start();
            }
        }

        private void launchprintersettings_Click(object sender, RoutedEventArgs e)
        {
            Process CmdProcess = new Process();
            CmdProcess.StartInfo.FileName = "cmd.exe";
            CmdProcess.StartInfo.CreateNoWindow = true;
            CmdProcess.StartInfo.UseShellExecute = false;
            CmdProcess.StartInfo.RedirectStandardInput = true;
            CmdProcess.StartInfo.RedirectStandardOutput = true;
            CmdProcess.StartInfo.RedirectStandardError = true;
            CmdProcess.StartInfo.Arguments = "/c " + "control printers"; //“/C”Indicates to exit immediately after executing the command  
            CmdProcess.Start();
            CmdProcess.StandardOutput.ReadToEnd();//get return value   
            CmdProcess.WaitForExit();//Wait for the program to finish executing and exit the process
            CmdProcess.Close();//END
        }

        private void restartbtn15_Click(object sender, RoutedEventArgs e)
        {
            LoadPrintJobsInfo();
        }

        private void printerjobitemname_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (printerjobitemname.SelectedIndex != -1)
            {
                InfoPrintJobs.Visibility = Visibility.Visible;
                ObjectQuery sysproceobj = new ObjectQuery("SELECT * FROM Win32_PrintJob");
                ManagementObjectSearcher sysprocessesobjsearch = new ManagementObjectSearcher(sysproceobj);
                foreach (ManagementObject obj in sysprocessesobjsearch.Get())
                {
                    if (printerjobitemname.SelectedItem.ToString() == obj["Caption"].ToString())
                    {
                        printerdevice.Text = obj["Name"].ToString();
                        documentname.Text = obj["Document"].ToString();
                        documentsize.Text = (Double.Parse(obj["Size"].ToString()) / 1e+6).ToString("#.##") + " MB";

                        owner.Text = obj["Owner"].ToString();
                        string input = obj["TimeSubmitted"].ToString();
                        string year = input.Substring(0, 4);
                        string month = input.Substring(4, 2);
                        string day = input.Substring(6, 2);
                        string hour = input.Substring(8, 2);
                        string minutes = input.Substring(10, 2);
                        timesubmitted.Text = year + "-" + month + "-" + day + " " + hour + ":" + minutes;
                        if (obj["StatusMask"] != null)
                        {
                            printstatus.Text = obj["StatusMask"].ToString();
                        }
                        else
                        {
                            printstatus.Text = "Unknown";
                        }
                        if (printstatus.Text == "1")
                        {
                            printstatus.Text = "Paused";
                        }
                        else if (printstatus.Text == "2")
                        {
                            printstatus.Text = "Error";
                        }
                        else if (printstatus.Text == "4")
                        {
                            printstatus.Text = "Deleting";
                        }
                        else if (printstatus.Text == "8")
                        {
                            printstatus.Text = "Spooling";
                        }
                        else if (printstatus.Text == "16")
                        {
                            printstatus.Text = "Printing";
                        }
                        else if (printstatus.Text == "32")
                        {
                            printstatus.Text = "Offline";
                        }
                        else if (printstatus.Text == "64")
                        {
                            printstatus.Text = "Paperout";
                        }
                        else if (printstatus.Text == "128")
                        {
                            printstatus.Text = "Printed";
                        }
                        else if (printstatus.Text == "256")
                        {
                            printstatus.Text = "Deleted";
                        }
                        else if (printstatus.Text == "512")
                        {
                            printstatus.Text = "Blocked_DevQ";
                        }
                        else if (printstatus.Text == "1024")
                        {
                            printstatus.Text = "User Intervention Required";
                        }
                        else if (printstatus.Text == "2048")
                        {
                            printstatus.Text = "Restarting";
                        }
                        pages.Text = obj["TotalPages"].ToString();
                    }
                }
            }
        }

        private void SearchPrintJobs_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var suitableItems = new List<string>();
                var splitText = sender.Text.ToLower().Split(" ");
                foreach (string infoitem in listView2.Items)
                {
                    var found = splitText.All((key) =>
                    {
                        return infoitem.ToLower().Contains(key);
                    });
                    if (found)
                    {
                        suitableItems.Add(infoitem);
                    }
                }
                if (suitableItems.Count == 0)
                {
                    if (printerjobitemname.Items.Count == 0)
                    {
                        suitableItems.Add("No print jobs available!");
                    }
                    else
                    {
                        suitableItems.Add("No results found!");
                    }
                }
                sender.ItemsSource = suitableItems;
            }
        }

        private void SearchPrintJobs_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {

        }

        private void SearchPrintJobs_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            string selection = args.SelectedItem.ToString();
            printerjobitemname.SelectedItem = selection;
            printerjobitemname.ScrollIntoView(selection);
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            nvSample.SelectedItem = SoftPrintJobs;
        }

        private void Button_Click_20(object sender, RoutedEventArgs e)
        {
            nvSample.SelectedItem = SoftPrintJobs;
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion == null)
            {
                NoResultstxt.Text = "";
                Magnifytxt.Text = "";
                SearchSugg.Visibility = Visibility.Visible;
                nvSample.IsPaneOpen = false;
                SearchSuggestions.Visibility = Visibility.Visible;
                nvSample.Header = "Search Results";
                settingspanel.Visibility = Visibility.Collapsed;
                osinfo.Visibility = Visibility.Collapsed;
                hardwaree.Visibility = Visibility.Collapsed;
                softwaree.Visibility = Visibility.Collapsed;
                driverinfo.Visibility = Visibility.Collapsed;
                syssummary.Visibility = Visibility.Collapsed;
                raminfo.Visibility = Visibility.Collapsed;
                cpuinfo.Visibility = Visibility.Collapsed;
                soundinfo.Visibility = Visibility.Collapsed;
                Home.Visibility = Visibility.Collapsed;
                inputdevicesinfo.Visibility = Visibility.Collapsed;
                storagedevicesinfo.Visibility = Visibility.Collapsed;
                printinginfo.Visibility = Visibility.Collapsed;
                gpuinfo.Visibility = Visibility.Collapsed;
                gpuinfo.Visibility = Visibility.Collapsed;
                networkinfo.Visibility = Visibility.Collapsed;
                logicboardinfo.Visibility = Visibility.Collapsed;
                servicesinfo.Visibility = Visibility.Collapsed;
                processesinfo.Visibility = Visibility.Collapsed;
                printjobsinfo.Visibility = Visibility.Collapsed;
                SearchSugg.ItemsSource = SearchNavig.ItemsSource;
                if (SearchSugg.Items.Contains("No results found"))
                {
                    SearchSugg.Visibility = Visibility.Collapsed;
                    Magnifytxt.Text = "\uE11A";
                    NoResultstxt.Text = "We couldn't find what you're looking for...";
                }
            }
        }

        private void SearchSugg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SearchSugg.SelectedIndex != -1)
                SelectionChosen(SearchSugg.SelectedItem.ToString());
        }

        private void joinbeta_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Process.Start(new ProcessStartInfo("https://groups.google.com/g/compinfo-beta") { UseShellExecute = true });
        }

        private void HyperlinkButton_Click_2(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/OceanCyanTech/CompInfo") { UseShellExecute = true });
        }

        private void HyperlinkButton_Click_3(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/OceanCyanTech/CompInfo/issues") { UseShellExecute = true });
        }

        private void HyperlinkButton_Click_4(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://forms.gle/eoiXdodq3yURTyXZ6") { UseShellExecute = true });
        }
    }
}
