using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PopupTwitch.Resources;

namespace PopupTwitch
{
    public class IgnoredUsersForm : Form
    {
        private List<string> ignorados = new();

        public IgnoredUsersForm()
        {
            this.Text = Strings.Get("Title_IgnoredUsers");
            this.Width = 300;
            this.Height = 400;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            int margem = 20;
            
            var lbl = new Label {
                Text = Strings.Get("Lbl_IgnoredUsers"),
                Top = 20,
                Left = margem,
                Width = 200
            };
            Controls.Add(lbl);

            var txtUser = new TextBox {
                Name = "txtUser",
                Top = 50,
                Left = margem,
                Width = 195
            };
            Controls.Add(txtUser);

            var btnAdd = new Button {
                Text = "+",
                Top = 49,
                Left = txtUser.Left + txtUser.Width + 15,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new System.Drawing.Size(35, 30)
            };
            Controls.Add(btnAdd);

            var list = new ListBox {
                Name = "listUsers",
                Top = 90,
                Left = margem, Width = btnAdd.Left + btnAdd.Width - margem,
                Height = 150
            };
            Controls.Add(list);

            int linhaInferior = 255;
            int espaco = 10;

            var btnRemove = new Button {
                Text = "-",
                Top = linhaInferior,
                Left = margem,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new System.Drawing.Size(50, 30)
            };
            Controls.Add(btnRemove);

            var btnClear = new Button {
                Text = Strings.Get("Btn_ClearAll"),
                Top = linhaInferior, Left = btnRemove.Left + btnRemove.Width + espaco,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new System.Drawing.Size(110, 30)
            };
            Controls.Add(btnClear);

            var btnSave = new Button {
                Text = Strings.Get("Btn_Save"),
                Top = linhaInferior, Left = btnClear.Left + btnClear.Width + espaco,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new System.Drawing.Size(65, 30)
            };
            Controls.Add(btnSave);

            var btnCancelar = new Button {
                Text = Strings.Get("Btn_Cancel"),
                Top = linhaInferior + 40, Left = margem,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new System.Drawing.Size(245, 30)
            };
            Controls.Add(btnCancelar);

            btnAdd.Click += (s, e) =>
            {
                var user = txtUser.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(user) && !ignorados.Contains(user))
                {
                    ignorados.Add(user);
                    list.Items.Add(user);
                    txtUser.Clear();
                }
            };

            btnRemove.Click += (s, e) =>
            {
                if (list.SelectedItem is string user)
                {
                    var confirm = MessageBox.Show(
                        string.Format(Strings.Get("Msg_RemoveConfirm"), user),
                        Strings.Get("Title_RemoveConfirm"),
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (confirm == DialogResult.Yes)
                    {
                        ignorados.Remove(user);
                        list.Items.Remove(user);
                    }
                }
                else
                {
                    MessageBox.Show(Strings.Get("Msg_NoUserSelected"),
                        Strings.Get("Title_Warning"),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            btnClear.Click += (s, e) =>
            {
                if (ignorados.Count == 0)
                {
                    MessageBox.Show(Strings.Get("Msg_ListEmpty"),
                        Strings.Get("Title_Info"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var confirm = MessageBox.Show(
                    Strings.Get("Msg_ClearAllConfirm"),
                    Strings.Get("Title_Confirm"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    ignorados.Clear();
                    list.Items.Clear();
                    MessageBox.Show(Strings.Get("Msg_ListCleared"),
                        Strings.Get("Title_Success"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            btnSave.Click += (s, e) =>
            {
                AppConfig.ClearIgnorados();
                foreach (var u in ignorados)
                    AppConfig.AddIgnorado(u);

                MessageBox.Show(Strings.Get("Msg_ListSaved"),
                    Strings.Get("Title_Config"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            };

            btnCancelar.Click += (s, e) =>
            {
                Close();
            };

            Carregar(list);
        }

        private void Carregar(ListBox list)
        {
            ignorados = AppConfig.GetIgnorados();
            list.Items.AddRange(ignorados.ToArray());
        }
    }
}
