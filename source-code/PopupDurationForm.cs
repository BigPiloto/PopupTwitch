using System;
using System.Drawing;
using System.Windows.Forms;
using PopupTwitch.Resources;

namespace PopupTwitch
{
    public class PopupDurationForm : Form
    {
        public PopupDurationForm()
        {
            this.Text = Strings.Get("Title_PopupDuration");
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 250;
            this.Height = 200;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var lbl = new Label
            {
                Text = Strings.Get("Lbl_PopupDuration"),
                Left = 15,
                Top = 20,
                Width = 200
            };
            Controls.Add(lbl);

            var txtTempo = new TextBox
            {
                Left = 15,
                Top = 45,
                Width = 200,
                TextAlign = HorizontalAlignment.Center,
                Text = AppConfig.GetPopupDuration().ToString()
            };
            Controls.Add(txtTempo);

            var btnSalvar = new Button
            {
                Text = Strings.Get("Btn_Save"),
                Height = 30,
                Left = 15,
                Top = 100,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new System.Drawing.Size(90, 30)
            };
            btnSalvar.Click += (s, e) =>
            {
                if (int.TryParse(txtTempo.Text, out int valor))
                {
                  try
                  {
                      AppConfig.SetPopupDuration(valor);
                      MessageBox.Show(Strings.Get("Msg_SavedDuration"),
                          Strings.Get("Title_Config"),
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
                      this.Close();
                  }
                  catch (ArgumentOutOfRangeException ex)
                  {
                      MessageBox.Show(ex.Message.Split('(')[0].Trim(),
                          Strings.Get("Title_InvalidValue"),
                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
                  }
                }
                else
                {
                    MessageBox.Show(Strings.Get("Msg_InvalidValue"),
                        Strings.Get("Title_Error"),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
            Controls.Add(btnSalvar);

            var btnCancelar = new Button
            {
                Text = Strings.Get("Btn_Cancel"),
                Height = 30,
                Left = 125,
                Top = 100,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new System.Drawing.Size(90, 30)
            };
            btnCancelar.Click += (s, e) => this.Close();
            Controls.Add(btnCancelar);
        }
    }
}
