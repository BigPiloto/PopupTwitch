using System;
using System.Windows.Forms;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using Timer = System.Windows.Forms.Timer;
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
        private static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int w, int h);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

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
            Icon appIcon;
            try
            {
                string iconPath = Path.Combine(Application.StartupPath, "Assets", "favicon.ico");
                if (File.Exists(iconPath))
                    appIcon = new Icon(iconPath);
                else
                    appIcon = SystemIcons.Application;
            }
            catch
            {
                // Fallback para o ícone padrão do sistema
                appIcon = SystemIcons.Application;
            }

            trayMenu = new ContextMenuStrip();

            trayIcon = new NotifyIcon
            {
                Icon = appIcon,
                ContextMenuStrip = trayMenu,
                Text = "Pop-up Twitch",
                Visible = true
            };

            trayMenu.Items.Add(Strings.Get("Btn_Open"), null, (s, e) =>
            {
                Show();
                CentralizarNaTela();
            });

            // botão conectar/desconectar dinâmico
            trayConnectItem = new ToolStripMenuItem
            {
                Text = Strings.Get("Btn_Connect")
            };
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
                    // lê o campo da janela principal
                    var txt = this.Controls["txtCanal"] as TextBox;
                    string canalAtual = txt?.Text.Trim() ?? string.Empty;

                    if (!string.IsNullOrWhiteSpace(canalAtual))
                    {
                        // opcional: já salvar ao conectar
                        AppConfig.SetCanal(canalAtual);

                        Conectar(canalAtual);
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
            configMenu.DropDownItems.Add(Strings.Get("Btn_IgnoredUsers"), null, (s, e) =>
            {
                var f = new IgnoredUsersForm();
                CentralizarNaTela(f);
                f.ShowDialog();
            });
            configMenu.DropDownItems.Add(Strings.Get("Btn_PopupPosition"), null, (s, e) =>
            {
                using var pos = new PopupPositionForm(this);
                pos.ShowDialog(this);
            });
            configMenu.DropDownItems.Add(Strings.Get("Btn_PopupDuration"), null, (s, e) =>
            {
                var f = new PopupDurationForm();
                CentralizarNaTela(f);
                f.ShowDialog();
            });
            configMenu.DropDownItems.Add(Strings.Get("Btn_ChatIdle"), null, (s, e) =>
            {
                var f = new ChatIdleForm();
                CentralizarNaTela(f);
                f.ShowDialog();
            });
            configMenu.DropDownItems.Add(Strings.Get("Btn_PopupStyle"), null, (s, e) =>
            {
                var f = new PopupStyleForm();
                CentralizarNaTela(f);
                f.ShowDialog();
            });
            configMenu.DropDownItems.Add(Strings.Get("Btn_SoundSettings"), null, (s, e) =>
            {
                var f = new SonsForm();
                CentralizarNaTela(f);
                f.ShowDialog();
            });
            configMenu.DropDownItems.Add(Strings.Get("Btn_Language"), null, (s, e) =>
            {
                var f = new LanguageForm();
                CentralizarNaTela(f);
                f.ShowDialog();
            });
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
            trayIcon.DoubleClick += (s, e) => { Show(); WindowState = FormWindowState.Normal; };

            InitializeComponent();

            this.Resize += (s, e) =>
            {
                if (WindowState == FormWindowState.Minimized)
                    Hide();
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
                Left = 20,
                AutoSize = true,
                MinimumSize = new Size(240, 30)
            };
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

            // botão engrenagem
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

            this.FormClosing += (s, e) => AppConfig.SetCanal(txtCanal.Text);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true;

            CentralizarNaTela();
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
                    BeginInvoke((Action)AtualizarEstadoInterface);
            };

            client.OnMessageReceived += (s, e) =>
            {
                string msg = e.ChatMessage.Message.Trim();
                string autor = e.ChatMessage.Username.ToLower();
                string canalConfig = AppConfig.GetCanal().ToLower();

                // 1) Carrega a palavra definida pelo usuário
                string testKeyword = AppConfig.GetTestKeyword();

                // 2) Se a mensagem for exatamente a palavra de teste e foi enviada pelo dono do canal → dispara popup
                if (string.Equals(msg, testKeyword, StringComparison.OrdinalIgnoreCase) && autor == canalConfig)
                {
                    MostrarPopup();
                    ultimaMensagem = DateTime.Now;
                    return;
                }

                // 3) Verifica ignorados
                if (AppConfig.DeveIgnorar(e.ChatMessage.Username))
                    return;

                // 4) Lógica normal do idle
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

            var (fundo, texto, raio, mensagem, opacidade, fonte, tamanho) = AppConfig.GetPopupStyle();
            var (xPct, yPct, wPct, hPct) = AppConfig.GetPopupData();
            int duracao = AppConfig.GetPopupDuration();

            if (InvokeRequired)
            {
                BeginInvoke((Action)MostrarPopup);
                return;
            }

            foreach (var screen in Screen.AllScreens)
            {
                var popup = new Form
                {
                    FormBorderStyle = FormBorderStyle.None,
                    BackColor = fundo,
                    Opacity = Math.Clamp(opacidade, 0.1, 1.0),
                    TopMost = true,
                    ShowInTaskbar = false,
                    StartPosition = FormStartPosition.Manual
                };

                popup.Width = wPct > 0 ? (int)(screen.Bounds.Width * wPct / 100.0) : 300;
                popup.Height = hPct > 0 ? (int)(screen.Bounds.Height * hPct / 100.0) : 100;

                popup.Left = xPct >= 0 ? screen.Bounds.Left + (int)(screen.Bounds.Width * xPct / 100.0)
                                    : screen.Bounds.Left + (screen.Bounds.Width - popup.Width) / 2;
                popup.Top = yPct >= 0 ? screen.Bounds.Top + (int)(screen.Bounds.Height * yPct / 100.0)
                                    : screen.Bounds.Top + (screen.Bounds.Height - popup.Height) / 2;

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

                    const int GWL_EXSTYLE = -20;
                    const int WS_EX_TRANSPARENT = 0x20;
                    const int WS_EX_TOOLWINDOW = 0x80;

                    int exStyle = GetWindowLong(popup.Handle, GWL_EXSTYLE);
                    SetWindowLong(popup.Handle, GWL_EXSTYLE, exStyle | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);

                    SetWindowPos(
                        popup.Handle,
                        HWND_TOPMOST,
                        popup.Left,
                        popup.Top,
                        popup.Width,
                        popup.Height,
                        SWP_SHOWWINDOW
                    );

                    //popup.BringToFront();
                    //popup.Activate();
                };

                var timer = new Timer { Interval = duracao };
                timer.Tick += (s, e) =>
                {
                    timer.Stop();
                    timer.Dispose();
                    popup.Close();
                    popup.Dispose();
                };
                popup.Shown += (_, __) => timer.Start();

                popup.Show();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // Só minimizar se a opção estiver ativa
                if (AppConfig.GetMostrarNaBandeja())
                {
                    e.Cancel = true;
                    Hide();
                    return;
                }
                else
                {
                    // Pergunta antes de sair
                    var result = MessageBox.Show(
                        Strings.Get("Msg_ConfirmExit"),
                        "Pop-up Twitch",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (result == DialogResult.No)
                    {
                         e.Cancel = true;
                         return;
                    }
                }
            }           

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
                        btnToggle.PerformClick();
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
            var btnToggle = this.Controls["btnToggle"] as Button;
            if (btnToggle != null)
            {
                btnToggle.Text = conectado
                    ? Strings.Get("Btn_Disconnect")
                    : Strings.Get("Btn_Connect");
            }

            if (trayConnectItem != null)
            {
                trayConnectItem.Text = conectado
                    ? Strings.Get("Btn_Disconnect")
                    : Strings.Get("Btn_Connect");
            }
        }

        private async void CheckForUpdate()
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Pop-upTwitch");

                var json = await client.GetStringAsync("https://api.github.com/repos/BigPiloto/PopupTwitch/releases/latest");
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                string latestTag = root.GetProperty("tag_name").GetString() ?? "";
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
            catch { }
        }

        private void CentralizarNaTela(Form form = null)
        {
            form ??= this;

            var screen = Screen.FromPoint(Cursor.Position);
            int x = screen.Bounds.Left + (screen.Bounds.Width - form.Width) / 2;
            int y = screen.Bounds.Top + (screen.Bounds.Height - form.Height) / 2;

            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(x, y);
        }
    }
}
