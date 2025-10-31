using System;
using System.Drawing;
using System.Windows.Forms;
using PopupTwitch.Resources;
using WinFormsTimer = System.Windows.Forms.Timer;

namespace PopupTwitch
{
    public class PopupStyleForm : Form
    {
        private Panel previewPanel;
        private Label previewLabel;
        private Button btnCorFundo, btnCorTexto, btnSalvar, btnCancelar;
        private TrackBar trackRaio;
        private TextBox txtTexto;
        private WinFormsTimer textoTimer;

        private Color corFundo;
        private Color corTexto;
        private int raioCantos;
        private string textoPopup;

        private TrackBar trackOpacidade;
        private double opacidade;

        public PopupStyleForm()
        {
            this.Text = Strings.Get("Title_PopupStyle");
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(700, 400);
            this.MaximizeBox = false;

            // Carregar valores atuais do AppConfig
            (corFundo, corTexto, raioCantos, textoPopup, opacidade) = AppConfig.GetPopupStyle();

            // --- PREVIEW ---
            previewPanel = new Panel
            {
                Width = 320,
                Height = 180,
                Left = 25,
                Top = 100,
            };
            previewPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.Clear(this.BackColor);

                int a = (int)Math.Round(opacidade * 255);

                using var brush = new SolidBrush(Color.FromArgb(a, corFundo));
                using var path = RoundedRect(new Rectangle(0, 0, previewPanel.Width - 1, previewPanel.Height - 1), raioCantos);
                e.Graphics.FillPath(brush, path);
                
                using var pen = new Pen(Color.FromArgb(Math.Min(120, a), 0, 0, 0), 1);
                e.Graphics.DrawPath(pen, path);

                var textSize = e.Graphics.MeasureString(previewLabel.Text, previewLabel.Font);
                var textX = (previewPanel.Width - textSize.Width) / 2;
                var textY = (previewPanel.Height - textSize.Height) / 2;

                using var textBrush = new SolidBrush(Color.FromArgb(a, corTexto));
                e.Graphics.DrawString(previewLabel.Text, previewLabel.Font, textBrush, textX, textY);
            };

            previewPanel.BackColor = Color.Transparent;
            Controls.Add(previewPanel);

            previewLabel = new Label
            {
                Text = textoPopup,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            Controls.Add(previewPanel);

            // --- CONFIGURAÇÕES ---
            int x = 380, y = 40;
            Controls.Add(new Label { Text = Strings.Get("Lbl_BackgroundColor"), Left = x, Top = y, AutoSize = true });
            btnCorFundo = new Button {
                Text = Strings.Get("Btn_Change"),
                Left = x + 120,
                Top = y - 5,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new System.Drawing.Size(80, 30)
            };
            btnCorFundo.Click += (s, e) =>
            {
                using var dlg = new ColorDialog { Color = corFundo };
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    corFundo = dlg.Color;
                    previewPanel.Invalidate();
                }
            };
            Controls.Add(btnCorFundo);

            y += 40;
            Controls.Add(new Label { Text = Strings.Get("Lbl_TextColor"), Left = x, Top = y, AutoSize = true });
            btnCorTexto = new Button {
                Text = Strings.Get("Btn_Change"),
                Left = x + 120,
                Top = y - 5,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new System.Drawing.Size(80, 30)
            };
            btnCorTexto.Click += (s, e) =>
            {
                using var dlg = new ColorDialog { Color = corTexto };
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    corTexto = dlg.Color;
                    previewLabel.ForeColor = corTexto;
                }
            };
            Controls.Add(btnCorTexto);

            y += 50;
            Controls.Add(new Label { Text = Strings.Get("Lbl_CornerRadius"), Left = x, Top = y, AutoSize = true });
            trackRaio = new TrackBar { Left = x + 120, Top = y - 5, Width = 180, Minimum = 1, Maximum = 50, TickFrequency = 5, Value = Math.Max(1, raioCantos) };
            trackRaio.ValueChanged += (s, e) => { raioCantos = Math.Max(1, trackRaio.Value); previewPanel.Invalidate(); };
            Controls.Add(trackRaio);

            y += 60;
            Controls.Add(new Label { Text = Strings.Get("Lbl_Opacity"), Left = x, Top = y, AutoSize = true });
            trackOpacidade = new TrackBar
            {
                Left = x + 120,
                Top = y - 5,
                Width = 180,
                Minimum = 10,
                Maximum = 100,
                TickFrequency = 10,
                Value = (int)(opacidade * 100)
            };
            trackOpacidade.ValueChanged += (s, e) =>
            {
                opacidade = trackOpacidade.Value / 100.0;
                previewPanel.Invalidate();
            };
            Controls.Add(trackOpacidade);

            y += 50;
            Controls.Add(new Label { Text = Strings.Get("Lbl_PopupText"), Left = x, Top = y, AutoSize = true });
            textoTimer = new WinFormsTimer();
            textoTimer.Interval = 1000;
            textoTimer.Tick += (s, e) =>
            {
                textoTimer.Stop();
                previewLabel.Text = txtTexto.Text;
                previewPanel.Invalidate();
            };

            txtTexto = new TextBox
            {
                Left = x,
                Top = y + 30,
                Width = 300,
                Text = textoPopup
            };

            txtTexto.TextChanged += (s, e) => 
            {
                textoTimer.Stop();
                textoTimer.Start();
            };
            Controls.Add(txtTexto);

            // --- BOTÕES ---
            int botoesTop = txtTexto.Top + txtTexto.Height + 60;
            
            var btnPadrao = new Button
            {
                Text = Strings.Get("Btn_ResetDefault"),
                Left = x,
                Top = botoesTop - 40,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new System.Drawing.Size(300, 30)
            };
            btnPadrao.Click += BtnPadrao_Click;
            Controls.Add(btnPadrao);

            btnSalvar = new Button {
                Text = Strings.Get("Btn_Save"),
                Left = x,
                Top = botoesTop,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new System.Drawing.Size((txtTexto.Width / 2) - 10, 30)
            };
            btnSalvar.Click += BtnSalvar_Click;
            Controls.Add(btnSalvar);

            btnCancelar = new Button {
                Text = Strings.Get("Btn_Cancel"),
                Left = x + (txtTexto.Width / 2) + 10,
                Top = botoesTop,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new System.Drawing.Size((txtTexto.Width / 2) - 10, 30)
            };
            btnCancelar.Click += (s, e) => this.Close();
            Controls.Add(btnCancelar);

            this.ClientSize = new Size(700, botoesTop + 80);

        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            AppConfig.SetPopupStyle(corFundo, corTexto, raioCantos, txtTexto.Text, opacidade);
            MessageBox.Show(Strings.Get("Msg_StyleSaved"), Strings.Get("Title_Config"),
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        // Função utilitária para bordas arredondadas
        private System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int d = radius * 2;
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void BtnPadrao_Click(object sender, EventArgs e)
        {
            // Valores padrão
            corFundo = Color.Black;
            corTexto = Color.White;
            raioCantos = 30;
            textoPopup = Strings.Get("Default_PopupMessage");
            opacidade = 0.85;

            // Atualiza controles
            trackRaio.Value = raioCantos;
            trackOpacidade.Value = (int)(opacidade * 100);
            txtTexto.Text = textoPopup;
            previewLabel.Text = textoPopup;
            previewLabel.ForeColor = corTexto;

            previewPanel.Invalidate();

            // Salva direto
            AppConfig.SetPopupStyle(corFundo, corTexto, raioCantos, textoPopup, opacidade);
            MessageBox.Show(Strings.Get("Msg_StyleReset"), Strings.Get("Title_ResetDefault"),
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
