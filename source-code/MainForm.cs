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
using System.Net.Http;
using System.Text.Json;
using System.Reflection;

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

        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private ToolStripMenuItem trayConnectItem;

        public MainForm()
        {
            // Ícone da bandeja
            trayMenu = new ContextMenuStrip();

            // abrir janela principal
            trayMenu.Items.Add(Strings.Get("Btn_Open"), null, (s, e) =>
            {
                Show();
                WindowState = FormWindowState.Normal;
            });

            // botão conectar/desconectar dinâmico
            trayConnectItem = new ToolStripMenuItem();
            trayMenu.Items.Add(trayConnectItem);

            trayConnectItem.Click += (s, e) =>
            {
                if (conectado)
                {
                    Desconectar();
                    MessageBox.Show(Strings.Get("Msg_Disconnected"),
                        "Pop-up Twitch", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string canalSalvo = AppConfig.GetCanal()?.Trim();
                    if (!string.IsNullOrWhiteSpace(canalSalvo))
                    {
                        Conectar(canalSalvo);
                        MessageBox.Show(Strings.Get("Msg_Connected"),
                            "Pop-up Twitch", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(Strings.Get("Msg_NoChannel"),
                            "Pop-up Twitch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                AtualizarEstadoInterface();
            };

            // submenu Configurações
            var configMenu = new ToolStripMenuItem(Strings.Get("Title_Settings"));
            configMenu.DropDownItems.Add(Strings.Get("Btn_IgnoredUsers"), null, (s, e) => new IgnoredUsersForm().ShowDialog());
            configMenu.DropDownItems.Add(Strings.Get("Btn_PopupPosition"), null, (s, e) =>
            {
                using var pos = new PopupPositionForm(this);
                pos.ShowDialog(this);
            });
            configMenu.DropDownItems.Add(Strings.Get("Btn_PopupDuration"), null, (s, e) => new PopupDurationForm().ShowDialog());
            configMenu.DropDownItems.Add(Strings.Get("Btn_ChatIdle"), null, (s, e) => new ChatIdleForm().ShowDialog());
            configMenu.DropDownItems.Add(Strings.Get("Btn_PopupStyle"), null, (s, e) => new PopupStyleForm().ShowDialog());
            configMenu.DropDownItems.Add(Strings.Get("Btn_SoundSettings"), null, (s, e) => new SonsForm().ShowDialog());
            configMenu.DropDownItems.Add(Strings.Get("Btn_Language"), null, (s, e) => new LanguageForm().ShowDialog());
            trayMenu.Items.Add(configMenu);

            // links externos
            trayMenu.Items.Add("-");
            trayMenu.Items.Add(Strings.Get("Btn_Feedback"), null, (s, e) =>
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://popuptwitch.meularsmart.com/#Contato",
                    UseShellExecute = true
                });
            });
            trayMenu.Items.Add(Strings.Get("Btn_Issue"), null, (s, e) =>
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/BigPiloto/PopupTwitch/issues",
                    UseShellExecute = true
                });
            });

            // sair
            trayMenu.Items.Add("-");
            trayMenu.Items.Add(Strings.Get("Btn_Exit"), null, (s, e) => Application.Exit());


            trayIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                ContextMenuStrip = trayMenu,
                Text = "Pop-up Twitch",
                Visible = true
            };

            // Clique duplo no ícone reabre o app
            trayIcon.DoubleClick += (s, e) => { Show(); WindowState = FormWindowState.Normal; };

            InitializeComponent();
            this.Resize += (s, e) =>
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    Hide();
                }
            };
            CheckForUpdate();
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

            var chkAutoConectar = new CheckBox
            {
                Name = "chkAutoConectar",
                Text = Strings.Get("Chk_AutoConnect"),
                Top = txtCanal.Bottom + 10,
                Left = 20,
                AutoSize = true,
                Checked = AppConfig.GetAutoConectar()
            };
            chkAutoConectar.CheckedChanged += (s, e) =>
            {
                AppConfig.SetAutoConectar(chkAutoConectar.Checked);
            };
            Controls.Add(chkAutoConectar);

            var btnToggle = new Button
            {
                Name = "btnToggle",
                Text = Strings.Get("Btn_Connect"),
                Top = chkAutoConectar.Bottom + 10,
                Left = 20
            };
            AtualizarEstadoInterface();
            btnToggle.AutoSize = true;
            btnToggle.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnToggle.MinimumSize = new Size(240, 30);

            btnToggle.Click += async (s, e) =>
            {
                btnToggle.Enabled = false;

                if (!conectado)
                {
                    canal = txtCanal.Text.Trim();
                    if (string.IsNullOrWhiteSpace(canal))
                    {
                        btnToggle.Enabled = true;
                        return;
                    }

                    await Task.Run(() => Conectar(canal));

                }
                else
                {
                    await Task.Run(() => Desconectar());
                }

                AtualizarEstadoInterface();
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
                if (IsHandleCreated)
                {
                    BeginInvoke((Action)(() =>
                    {
                        Text = Strings.Get("Title_Main");
                        AtualizarEstadoInterface();
                    }));
                }
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

            if (IsHandleCreated)
                BeginInvoke((Action)AtualizarEstadoInterface);
        }

        private void Desconectar()
        {
            if (client != null && client.IsConnected)
                client.Disconnect();
            conectado = false;

            if (IsHandleCreated)
                BeginInvoke((Action)AtualizarEstadoInterface);
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

        private async void CheckForUpdate()
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Pop-upTwitch");

                // Obtém a última release pública do GitHub
                var json = await client.GetStringAsync("https://api.github.com/repos/BigPiloto/PopupTwitch/releases/latest");
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                string latestTag = root.GetProperty("tag_name").GetString() ?? "";
                string latestUrl = root.GetProperty("html_url").GetString() ?? "";
                string assetUrl = "";

                if (root.TryGetProperty("assets", out var assets) && assets.GetArrayLength() > 0)
                    assetUrl = assets[0].GetProperty("browser_download_url").GetString() ?? "";

                string currentVersion = "v" + Assembly.GetExecutingAssembly().GetName().Version?.ToString(3);

                if (!string.Equals(latestTag, currentVersion, StringComparison.OrdinalIgnoreCase))
                {
                    var message =
                        $"{Strings.Get("Msg_NewVersionAvailable")}: {latestTag}\n" +
                        $"{Strings.Get("Msg_CurrentVersion")}: {currentVersion}\n\n" +
                        Strings.Get("Msg_DownloadQuestion");

                    var result = MessageBox.Show(message, Strings.Get("Msg_UpdateAvailable"),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            string tempPath = Path.Combine(Path.GetTempPath(), "Pop-upTwitch-Installer.exe");

                            using var http = new HttpClient();
                            var bytes = await http.GetByteArrayAsync(assetUrl);
                            await File.WriteAllBytesAsync(tempPath, bytes);

                            MessageBox.Show(Strings.Get("Msg_InstallerStarting"),
                                Strings.Get("Msg_UpdateTitle"),
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = tempPath,
                                UseShellExecute = true
                            });

                            Application.Exit();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(Strings.Get("Msg_UpdateFailed") + "\n\n" + ex.Message,
                                Strings.Get("Msg_ErrorTitle"),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch
            {
                // ignora erros silenciosamente
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Se o fechamento for pelo botão X, apenas ocultar
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                return;
            }

            // Fechamento real (sair pelo menu "Exit")
            Desconectar();
            trayIcon.Visible = false;
            base.OnFormClosing(e);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            bool autoConectar = AppConfig.GetAutoConectar();
            string canalSalvo = AppConfig.GetCanal()?.Trim();

            if (autoConectar && !string.IsNullOrWhiteSpace(canalSalvo))
            {
                try
                {
                    var btnToggle = this.Controls["btnToggle"] as Button;
                    if (btnToggle != null && !conectado)
                    {
                        btnToggle.PerformClick(); // simula clique no botão "Conectar"
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"{Strings.Get("Msg_AutoConnectFail")}\n{ex.Message}",
                        Strings.Get("Msg_ErrorTitle"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
        }

        private void AtualizarEstadoInterface()
        {
            // Atualiza botão da janela principal
            var btnToggle = this.Controls["btnToggle"] as Button;
            if (btnToggle != null)
            {
                btnToggle.Text = conectado
                    ? Strings.Get("Btn_Disconnect")
                    : Strings.Get("Btn_Connect");
            }

            // Atualiza texto do menu da bandeja
            if (trayConnectItem != null)
            {
                trayConnectItem.Text = conectado
                    ? Strings.Get("Btn_Disconnect")
                    : Strings.Get("Btn_Connect");
            }
        }

    }
}
