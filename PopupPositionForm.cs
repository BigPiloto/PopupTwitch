using System;
using System.Drawing;
using System.Windows.Forms;
using PopupTwitch.Resources;

namespace PopupTwitch
{
    public class PopupPositionForm : Form
    {
        private readonly Screen targetScreen;
        private Panel popup;
        private Point offset;
        private bool dragging = false;
        private bool resizing = false;
        private ResizeDirection resizeDir = ResizeDirection.None;
        private const int resizeMargin = 8;

        private enum ResizeDirection { None, Left, Right, Top, Bottom, TopLeft, TopRight, BottomLeft, BottomRight }

        // construtor default: usa o monitor do mouse
        public PopupPositionForm() : this(Screen.FromPoint(Cursor.Position)) { }

        // construtor recebendo dono
        public PopupPositionForm(Form owner) : this(Screen.FromControl(owner)) { }

        // construtor base recebendo Screen
        private PopupPositionForm(Screen screen)
        {
            targetScreen = screen;

            Text = Strings.Get("Title_PopupPosition");
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;

            // ocupa exatamente o monitor alvo
            var b = targetScreen.Bounds;
            Location = b.Location;
            Size = b.Size;

            TopMost = true;
            Opacity = 0.85;
            BackColor = Color.Black;
            KeyPreview = true;

            var topBar = new Panel { Height = 50, Dock = DockStyle.Top, BackColor = Color.FromArgb(235, 235, 235) };
            Controls.Add(topBar);

            var lblInfo = new Label
            {
                Text = Strings.Get("Lbl_InfoPosition"),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black,
                Left = 15,
                Top = 15
            };
            topBar.Controls.Add(lblInfo);

            popup = new Panel
            {
                BackColor = Color.LightBlue,
                Width = 300,
                Height = 150,
                BorderStyle = BorderStyle.FixedSingle,
            };
            popup.MouseDown += Popup_MouseDown;
            popup.MouseMove += Popup_MouseMove;
            popup.MouseUp += Popup_MouseUp;
            Controls.Add(popup);

            var bottomBar = new Panel { Height = 60, Dock = DockStyle.Bottom, BackColor = Color.FromArgb(235, 235, 235) };
            Controls.Add(bottomBar);

            var btnSalvar = new Button
            {
                Text = Strings.Get("Btn_Save"),
                Width = 150, Height = 36, Left = 15, Top = 12,
                BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.Standard
            };
            btnSalvar.Click += BtnSalvar_Click;
            bottomBar.Controls.Add(btnSalvar);

            var btnCancelar = new Button
            {
                Text = Strings.Get("Btn_Cancel"),
                Width = 150, Height = 36, Left = 180, Top = 12,
                BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.Standard
            };
            btnCancelar.Click += (s, e) => Close();
            bottomBar.Controls.Add(btnCancelar);

            var btnPadrao = new Button
            {
                Text = Strings.Get("Btn_ResetDefault"),
                Width = 150, Height = 36, Left = 345, Top = 12,
                BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.Standard
            };
            btnPadrao.Click += (s, e) =>
            {
                var sb = targetScreen.Bounds;
                popup.Width = 300;
                popup.Height = 150;
                // centraliza respeitando Left/Top do monitor alvo
                popup.Left = (sb.Width - popup.Width) / 2;
                popup.Top  = (sb.Height - popup.Height) / 2;

                AppConfig.SetPopupData(-1, -1, -1, -1);

                MessageBox.Show(Strings.Get("Msg_ResetSuccess"),
                    Strings.Get("Title_ResetDefault"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            bottomBar.Controls.Add(btnPadrao);

            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) Close(); };

            Load += (s, e) =>
            {
                var sb = targetScreen.Bounds;

                // posição padrão: centro do monitor alvo
                popup.Left = (sb.Width - popup.Width) / 2;
                popup.Top  = (sb.Height - popup.Height) / 2;

                var (x, y, w, h) = AppConfig.GetPopupData();
                if (x >= 0 && y >= 0)
                {
                    popup.Left = (int)Math.Round(sb.Width  * x / 100.0);
                    popup.Top  = (int)Math.Round(sb.Height * y / 100.0);
                }
                if (w > 0 && h > 0)
                {
                    popup.Width  = (int)Math.Round(sb.Width  * w / 100.0);
                    popup.Height = (int)Math.Round(sb.Height * h / 100.0);
                }

                // garante que fique dentro do cliente (0..ClientSize)
                ClampInsideClient();
            };
        }

        private void Popup_MouseDown(object sender, MouseEventArgs e)
        {
            resizeDir = GetResizeDirection(e.Location);
            if (resizeDir != ResizeDirection.None) resizing = true;
            else { dragging = true; offset = e.Location; }
        }

        private void Popup_MouseMove(object sender, MouseEventArgs e)
        {
            if (resizing) { ResizePopup(e); return; }

            if (dragging)
            {
                popup.Left = Math.Max(0, Math.Min(ClientSize.Width  - popup.Width,  popup.Left + e.X - offset.X));
                popup.Top  = Math.Max(0, Math.Min(ClientSize.Height - popup.Height, popup.Top  + e.Y - offset.Y));
                return;
            }

            var dir = GetResizeDirection(e.Location);
            popup.Cursor = dir switch
            {
                ResizeDirection.Top or ResizeDirection.Bottom => Cursors.SizeNS,
                ResizeDirection.Left or ResizeDirection.Right => Cursors.SizeWE,
                ResizeDirection.TopLeft or ResizeDirection.BottomRight => Cursors.SizeNWSE,
                ResizeDirection.TopRight or ResizeDirection.BottomLeft => Cursors.SizeNESW,
                _ => Cursors.SizeAll
            };
        }

        private void Popup_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
            resizing = false;
        }

        private ResizeDirection GetResizeDirection(Point m)
        {
            bool left = m.X < resizeMargin;
            bool right = m.X > popup.Width - resizeMargin;
            bool top = m.Y < resizeMargin;
            bool bottom = m.Y > popup.Height - resizeMargin;

            if (top && left) return ResizeDirection.TopLeft;
            if (top && right) return ResizeDirection.TopRight;
            if (bottom && left) return ResizeDirection.BottomLeft;
            if (bottom && right) return ResizeDirection.BottomRight;
            if (top) return ResizeDirection.Top;
            if (bottom) return ResizeDirection.Bottom;
            if (left) return ResizeDirection.Left;
            if (right) return ResizeDirection.Right;
            return ResizeDirection.None;
        }

        private void ResizePopup(MouseEventArgs e)
        {
            switch (resizeDir)
            {
                case ResizeDirection.Left:        popup.Left += e.X; popup.Width -= e.X; break;
                case ResizeDirection.Right:       popup.Width = e.X;                     break;
                case ResizeDirection.Top:         popup.Top  += e.Y; popup.Height -= e.Y;break;
                case ResizeDirection.Bottom:      popup.Height = e.Y;                    break;
                case ResizeDirection.TopLeft:     popup.Left += e.X; popup.Width -= e.X; popup.Top += e.Y; popup.Height -= e.Y; break;
                case ResizeDirection.TopRight:    popup.Width = e.X;  popup.Top += e.Y;  popup.Height -= e.Y; break;
                case ResizeDirection.BottomLeft:  popup.Left += e.X; popup.Width -= e.X; popup.Height = e.Y; break;
                case ResizeDirection.BottomRight: popup.Width = e.X;  popup.Height = e.Y; break;
            }

            if (popup.Width < 100) popup.Width = 100;
            if (popup.Height < 60) popup.Height = 60;
            ClampInsideClient();
        }

        private void ClampInsideClient()
        {
            popup.Left = Math.Max(0, Math.Min(ClientSize.Width  - popup.Width,  popup.Left));
            popup.Top  = Math.Max(0, Math.Min(ClientSize.Height - popup.Height, popup.Top));
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            var sb = targetScreen.Bounds;

            // percentuais relativos ao monitor alvo
            double percentX = (double)popup.Left  / sb.Width * 100.0;
            double percentY = (double)popup.Top   / sb.Height * 100.0;
            double percentW = (double)popup.Width / sb.Width * 100.0;
            double percentH = (double)popup.Height/ sb.Height* 100.0;

            AppConfig.SetPopupData(percentX, percentY, percentW, percentH);

            MessageBox.Show(Strings.Get("Msg_SavedPosition"),
                Strings.Get("Title_Config"),
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
    }
}
