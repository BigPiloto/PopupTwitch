using System;
using System.Windows.Forms;
using PopupTwitch.Resources;

namespace PopupTwitch
{
    public class LanguageForm : Form
    {
        private ComboBox comboIdioma;

        public LanguageForm()
        {
            Text = Strings.Get("Title_Language");
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Width = 300;
            Height = 200;

            var lbl = new Label
            {
                Text = Strings.Get("Lbl_SelectLanguage"),
                Left = 20,
                Top = 20,
                AutoSize = true
            };
            Controls.Add(lbl);

            comboIdioma = new ComboBox
            {
                Left = 20,
                Top = 50,
                Width = 240,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            comboIdioma.Items.Add(Strings.Get("Opt_Portuguese"));
            comboIdioma.Items.Add(Strings.Get("Opt_English"));

            string atual = AppConfig.GetIdioma();
            comboIdioma.SelectedIndex = atual == "en-US" ? 1 : 0;

            Controls.Add(comboIdioma);

            var btnSalvar = new Button
            {
                Text = Strings.Get("Btn_Save"),
                Left = 20,
                Top = 100,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new System.Drawing.Size(100, 30)
            };
            btnSalvar.Click += BtnSalvar_Click;
            Controls.Add(btnSalvar);

            var btnCancelar = new Button
            {
                Text = Strings.Get("Btn_Cancel"),
                Left = 160,
                Top = 100,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new System.Drawing.Size(100, 30)
            };
            btnCancelar.Click += (s, e) => Close();
            Controls.Add(btnCancelar);
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            string selecionado = comboIdioma.SelectedIndex == 1 ? "en-US" : "pt-BR";
            AppConfig.SetIdioma(selecionado);

            MessageBox.Show(Strings.Get("Msg_LanguageChanged"),
                Strings.Get("Title_Info"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            Close();
        }
    }
}
