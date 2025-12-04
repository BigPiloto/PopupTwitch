using System;
using System.Windows.Forms;
using PopupTwitch.Resources;

namespace PopupTwitch
{
    public class TestKeywordForm : Form
    {
        private TextBox txtKeyword;

        public TestKeywordForm()
        {
            this.Text = Strings.Get("Title_TestKeyword");
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 300;
            this.Height = 160;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var lbl = new Label
            {
                Text = Strings.Get("Lbl_TestKeyword"),
                Left = 20,
                Top = 20,
                AutoSize = true
            };
            Controls.Add(lbl);

            txtKeyword = new TextBox
            {
                Left = 20,
                Top = 45,
                Width = 240,
                Text = AppConfig.GetTestKeyword()
            };
            Controls.Add(txtKeyword);

            var btnSalvar = new Button
            {
                Text = Strings.Get("Btn_Save"),
                Left = 20,
                Top = 80,
                Width = 240
            };
            btnSalvar.Click += (s, e) =>
            {
                var kw = txtKeyword.Text.Trim();

                if (string.IsNullOrWhiteSpace(kw))
                {
                    MessageBox.Show(Strings.Get("Msg_KeywordEmpty"));
                    return;
                }

                AppConfig.SetTestKeyword(kw);
                MessageBox.Show(Strings.Get("Msg_Test_Saved"));
                this.Close();
            };
            Controls.Add(btnSalvar);
        }
    }
}