using System;
using System.Windows.Forms;
using PopupTwitch.Resources;

namespace PopupTwitch
{
    public class SettingsMenuForm : Form
    {
        public SettingsMenuForm()
        {
            this.Text = Strings.Get("Title_Settings");
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 300;
            this.Height = 520;
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
            AddButton("Btn_Feedback", (s, e) =>
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/BigPiloto/PopupTwitch/issues",
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
        }
    }
}