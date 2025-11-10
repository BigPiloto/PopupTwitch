using System;
using System.Windows.Forms;
using PopupTwitch.Resources;
using System.Reflection;

namespace PopupTwitch
{
    public class SettingsMenuForm : Form
    {
        public SettingsMenuForm()
        {
            this.Text = Strings.Get("Title_Settings");
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 300;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var lblTitulo = new Label
            {
                Text = Strings.Get("Lbl_SelectOption"),
                Top = 20,
                Left = 20,
                Width = 200
            };
            Controls.Add(lblTitulo);

            int margemEsq = 20;
            int topo = 60;
            int espacamento = 40;

            void AddButton(string key, EventHandler click)
            {
                var btn = new Button
                {
                    Text = Strings.Get(key),
                    Left = margemEsq,
                    Top = topo,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    MinimumSize = new System.Drawing.Size(240, 30)
                };
                btn.Click += click;
                Controls.Add(btn);
                topo += espacamento;
            }

            AddButton("Btn_IgnoredUsers", (s, e) => new IgnoredUsersForm().ShowDialog());
            AddButton("Btn_PopupPosition", (s, e) => 
            {
                using var pos = new PopupPositionForm(this);
                pos.ShowDialog(this);
            });
            AddButton("Btn_PopupDuration", (s, e) => new PopupDurationForm().ShowDialog());
            AddButton("Btn_ChatIdle", (s, e) => new ChatIdleForm().ShowDialog());
            AddButton("Btn_PopupStyle", (s, e) => new PopupStyleForm().ShowDialog());
            AddButton("Btn_SoundSettings", (s, e) => new SonsForm().ShowDialog());
            AddButton("Btn_Language", (s, e) => new LanguageForm().ShowDialog());

            // --- Checkbox: iniciar com o Windows ---
            var chkIniciarComWindows = new CheckBox
            {
                Text = Strings.Get("Chk_StartupWithWindows"),
                Left = margemEsq,
                Top = topo + 8,
                AutoSize = true,
                Checked = AppConfig.GetIniciarComWindows()
            };
            chkIniciarComWindows.CheckedChanged += (s, e) =>
            {
                AppConfig.SetIniciarComWindows(chkIniciarComWindows.Checked);
            };
            Controls.Add(chkIniciarComWindows);
            topo += espacamento + 10;

            // --- Checkbox: minimizar para bandeja ---
            var chkMostrarNaBandeja = new CheckBox
            {
                Text = Strings.Get("Chk_MinimizeToTray"),
                Left = margemEsq,
                Top = topo,
                AutoSize = true,
                Checked = AppConfig.GetMostrarNaBandeja()
            };
            chkMostrarNaBandeja.CheckedChanged += (s, e) =>
            {
                AppConfig.SetMostrarNaBandeja(chkMostrarNaBandeja.Checked);
            };
            Controls.Add(chkMostrarNaBandeja);
            topo += espacamento + 10;

            AddButton("Btn_Feedback", (s, e) =>
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://popuptwitch.meularsmart.com/#Contato",
                    UseShellExecute = true
                });
            });
            AddButton("Btn_Issue", (s, e) =>
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/BigPiloto/PopupTwitch/issues",
                    UseShellExecute = true
                });
            });
            AddButton("Btn_Close", (s, e) => Close());

            int margemInferior = 60;
            this.Height = topo + margemInferior;

            // Obtém versão "limpa" (sem hash)
            string version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "N/A";

            // Label de versão
            var lblVersao = new Label
            {
                Text = $"Versão {version}",
                AutoSize = true,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Segoe UI", 8, System.Drawing.FontStyle.Regular),
                ForeColor = System.Drawing.Color.Gray,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            Controls.Add(lblVersao);

            // Ajusta a posição depois que o formulário estiver carregado
            this.Load += (s, e) =>
            {
                lblVersao.Left = (this.ClientSize.Width - lblVersao.Width) / 2;
                lblVersao.Top = this.ClientSize.Height - lblVersao.Height - 8;
            };
        }
    }
}