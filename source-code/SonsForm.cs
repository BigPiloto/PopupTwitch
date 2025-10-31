using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using PopupTwitch.Resources;

namespace PopupTwitch
{
    public class SonsForm : Form
    {
        private CheckBox chkSom;
        private Button btnUpload, btnRestaurar, btnSalvar;
        private readonly string pastaSons;
        private readonly string somPadrao;
        private readonly string somPersonalizado;
        private string somAtual;

        public SonsForm()
        {
            Text = Strings.Get("Title_SoundSettings");
            Size = new System.Drawing.Size(400, 210);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;

            pastaSons = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "sons");
            Directory.CreateDirectory(pastaSons);

            somPadrao = Path.Combine(pastaSons, "default.mp3");
            somPersonalizado = Path.Combine(pastaSons, "custom.mp3");

            chkSom = new CheckBox
            {
                Text = Strings.Get("Chk_EnableSound"),
                Left = 30,
                Top = 30,
                AutoSize = true
            };
            Controls.Add(chkSom);

            btnUpload = new Button
            {
                Text = Strings.Get("Btn_SelectSound"),
                Left = 30,
                Top = 70,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new System.Drawing.Size(150, 30)
            };
            btnUpload.Click += BtnUpload_Click;
            Controls.Add(btnUpload);

            btnRestaurar = new Button
            {
                Text = Strings.Get("Btn_RestoreSound"),
                Left = 200,
                Top = 70,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new System.Drawing.Size(150, 30)
            };
            btnRestaurar.Click += BtnRestaurar_Click;
            Controls.Add(btnRestaurar);

            btnSalvar = new Button
            {
                Text = Strings.Get("Btn_Save"),
                Left = 30,
                Top = 110,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new System.Drawing.Size(150, 30)
            };
            btnSalvar.Click += BtnSalvar_Click;
            Controls.Add(btnSalvar);

            var btnCancelar = new Button
            {
                Text = Strings.Get("Btn_Cancel"),
                Left = 200,
                Top = 110,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new System.Drawing.Size(150, 30)
            };
            btnCancelar.Click += (s, e) => Close();
            Controls.Add(btnCancelar);

            CarregarConfiguracoes();
        }

        private void CarregarConfiguracoes()
        {
            var (ativo, caminho) = AppConfig.GetSom();
            chkSom.Checked = ativo;
            somAtual = string.IsNullOrEmpty(caminho) ? somPadrao : caminho;
            if (!File.Exists(somAtual))
                somAtual = somPadrao;
        }

        private void SalvarConfiguracoes()
        {
            AppConfig.SetSom(chkSom.Checked, somAtual);
        }

        private void BtnUpload_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "Arquivos de som (*.mp3)|*.mp3",
                Title = Strings.Get("Btn_SelectSound")
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.Copy(ofd.FileName, somPersonalizado, true);
                    somAtual = somPersonalizado;
                    MessageBox.Show(Strings.Get("Msg_SoundUploaded"),
                        Strings.Get("Title_Success"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(Strings.Get("Msg_SoundCopyError"), ex.Message),
                        Strings.Get("Title_Error"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnRestaurar_Click(object sender, EventArgs e)
        {
            somAtual = somPadrao;
            if (File.Exists(somPersonalizado))
                File.Delete(somPersonalizado);

            SalvarConfiguracoes();
            MessageBox.Show(Strings.Get("Msg_SoundRestored"),
                Strings.Get("Title_Info"),
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            SalvarConfiguracoes();
            MessageBox.Show(Strings.Get("Msg_SoundSaved"),
                Strings.Get("Title_Success"),
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        private static int _tocando = 0;

        public static void ReproduzirSom()
        {
            _ = Task.Run(() =>
            {
                try
                {
                    var (ativo, caminho) = AppConfig.GetSom();
                    if (!ativo) return;

                    string final = File.Exists(caminho)
                        ? caminho
                        : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sons", "default.mp3");

                    if (Interlocked.Exchange(ref _tocando, 1) == 1) return;

                    using (var audio = new AudioFileReader(final))
                    using (var output = new WaveOutEvent())
                    {
                        output.Init(audio);
                        output.Play();
                        while (output.PlaybackState == PlaybackState.Playing)
                            Thread.Sleep(25);
                    }
                }
                catch
                {

                }
                finally
                {
                  Interlocked.Exchange(ref _tocando, 0);
                }
            });
        }
    }
}
