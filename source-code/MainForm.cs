using System;
using System.Windows.Forms;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using Timer = System.Windows.Forms.Timer;
using System.Threading;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using PopupTwitch.Resources;

namespace PopupTwitch
{
    public partial class MainForm : Form
    {
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int x1, int y1, int x2, int y2, int w, int h
        );

        private TwitchClient? client;
        private bool conectado = false;
        private string canal = "";
        private DateTime ultimaMensagem = DateTime.Now;

        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_SHOWWINDOW = 0x0040;

        public MainForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = Strings.Get("Title_Main");
            this.Width = 300;
            this.Height = 200;

            var label = new Label
            {
                Text = Strings.Get("Lbl_Channel"),
                Top = 20,
                Left = 20,
                Width = 120
            };
            Controls.Add(label);

            var txtCanal = new TextBox
            {
                Name = "txtCanal",
                Top = 50,
                Left = 20,
                Width = 240,
                Text = AppConfig.GetCanal(),
                TextAlign = HorizontalAlignment.Center
            };
            Controls.Add(txtCanal);

            var btnToggle = new Button
            {
                Name = "btnToggle",
                Text = Strings.Get("Btn_Connect"),
                Top = 100,
                Left = 20
            };
            btnToggle.AutoSize = true;
            btnToggle.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnToggle.MinimumSize = new Size(240, 30);

            btnToggle.Click += async (s, e) =>
            {
                btnToggle.Enabled = false;

                if (!conectado)
                {
                    canal = txtCanal.Text.Trim();
                    if (string.IsNullOrEmpty(canal))
                    {
                        btnToggle.Enabled = true;
                        return;
                    }

                    btnToggle.Text = Strings.Get("Btn_Connecting");
                    await Task.Run(() => Conectar(canal));
                    btnToggle.Text = Strings.Get("Btn_Disconnect");
                }
                else
                {
                    btnToggle.Text = Strings.Get("Btn_Disconnecting");
                    await Task.Run(() => Desconectar());
                    btnToggle.Text = Strings.Get("Btn_Connect");
                }

                btnToggle.Enabled = true;
            };
            Controls.Add(btnToggle);

            this.FormClosing += (s, e) => AppConfig.SetCanal(txtCanal.Text);

            var btnConfig = new Button
            {
                Name = "btnConfig",
                Width = 24,
                Height = 24,
                Left = this.ClientSize.Width - 50,
                Top = 10,
                FlatStyle = FlatStyle.Flat,
                BackColor = SystemColors.Control,
                TabStop = false,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            btnConfig.FlatAppearance.BorderSize = 0;
            btnConfig.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnConfig.Text = "";

            try
            {
                string staticPath = Path.Combine(Application.StartupPath, "Assets", "gear_static.png");
                if (File.Exists(staticPath))
                {
                    using (var img = Image.FromFile(staticPath))
                    {
                        btnConfig.Image = new Bitmap(img, new Size(24, 24));
                    }
                }
            }
            catch { }

            btnConfig.ImageAlign = ContentAlignment.MiddleCenter;

            btnConfig.Click += (s, e) =>
            {
                var menu = new SettingsMenuForm();
                menu.ShowDialog();
            };

            Controls.Add(btnConfig);

            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
        }

        private void Conectar(string canal)
        {
            var creds = new ConnectionCredentials("justinfan12345", "oauth:fake_token");
            client = new TwitchClient();
            client.Initialize(creds, canal);

            client.OnConnected += (s, e) =>
            {
                conectado = true;
                ultimaMensagem = DateTime.Now;
                BeginInvoke(() => Text = Strings.Get("Title_Main"));
            };

            client.OnMessageReceived += (s, e) =>
            {
                if (AppConfig.DeveIgnorar(e.ChatMessage.Username))
                    return;

                var diff = (DateTime.Now - ultimaMensagem).TotalSeconds;
                int idleSec = AppConfig.GetChatIdle();
                if (diff >= idleSec)
                    MostrarPopup();
                ultimaMensagem = DateTime.Now;
            };

            client.Connect();
            conectado = true;
        }

        private void Desconectar()
        {
            if (client != null && client.IsConnected)
                client.Disconnect();
            conectado = false;
        }

        private void MostrarPopup()
        {
            SonsForm.ReproduzirSom();

            foreach (var screen in Screen.AllScreens)
            {
                Thread popupThread = new Thread(() =>
                {
                    var (fundo, texto, raio, mensagem, opacidade, fonte, tamanho) = AppConfig.GetPopupStyle();
                    var (xPct, yPct, wPct, hPct) = AppConfig.GetPopupData();

                    var popup = new Form
                    {
                        FormBorderStyle = FormBorderStyle.None,
                        BackColor = fundo,
                        Opacity = Math.Clamp(opacidade, 0.1, 1.0),
                        TopMost = true,
                        ShowInTaskbar = false,
                        StartPosition = FormStartPosition.Manual,
                    };

                    popup.Width = (wPct > 0 && hPct > 0)
                        ? (int)(screen.Bounds.Width * wPct / 100.0)
                        : 300;
                    popup.Height = (wPct > 0 && hPct > 0)
                        ? (int)(screen.Bounds.Height * hPct / 100.0)
                        : 100;

                    if (xPct >= 0 && yPct >= 0)
                    {
                        popup.Left = screen.Bounds.Left + (int)(screen.Bounds.Width * xPct / 100.0);
                        popup.Top = screen.Bounds.Top + (int)(screen.Bounds.Height * yPct / 100.0);
                    }
                    else
                    {
                        popup.Left = screen.Bounds.Left + (screen.Bounds.Width - popup.Width) / 2;
                        popup.Top = screen.Bounds.Top + (screen.Bounds.Height - popup.Height) / 2;

                    }

                    var lbl = new Label
                    {
                        Text = mensagem,
                        Dock = DockStyle.Fill,
                        ForeColor = texto,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font(fonte, tamanho, FontStyle.Bold)
                    };
                    popup.Controls.Add(lbl);

                    popup.Load += (_, __) =>
                    {
                        IntPtr region = CreateRoundRectRgn(0, 0, popup.Width, popup.Height, raio, raio);
                        popup.Region = Region.FromHrgn(region);

                        SetWindowPos(
                            popup.Handle, HWND_TOPMOST,
                            0, 0, 0, 0,
                            SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW
                        );

                        const int GWL_EXSTYLE = -20;
                        const int WS_EX_TRANSPARENT = 0x20;
                        const int WS_EX_TOOLWINDOW = 0x80;
                        int exStyle = GetWindowLong(popup.Handle, GWL_EXSTYLE);
                        SetWindowLong(popup.Handle, GWL_EXSTYLE, exStyle | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);

                        popup.Activate();
                    };

                    int duracao = AppConfig.GetPopupDuration();
                    var timer = new Timer { Interval = duracao };
                    timer.Tick += (s, e) => popup.Close();
                    timer.Start();

                    Application.Run(popup);
                });

                popupThread.SetApartmentState(ApartmentState.STA);
                popupThread.Start();
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Desconectar();
            base.OnFormClosing(e);
        }
    }
}
