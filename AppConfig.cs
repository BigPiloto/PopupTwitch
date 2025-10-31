using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PopupTwitch.Resources;

namespace PopupTwitch
{
    public static class AppConfig
    {
        private static readonly string ConfigPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        private class ConfigData
        {
            public string Canal { get; set; } = "";
            public List<string> Ignorados { get; set; } = new();
            public double PopupPosX { get; set; } = -1;
            public double PopupPosY { get; set; } = -1;
            public double PopupWidth { get; set; } = -1;
            public double PopupHeight { get; set; } = -1;
            public int PopupDuration { get; set; } = 3000;
            public int ChatIdleMs { get; set; } = 5000;
            public string PopupCorFundo { get; set; } = "#000000";
            public string PopupCorTexto { get; set; } = "#FFFFFF";
            public int PopupRaio { get; set; } = 30;
            public string PopupMensagem { get; set; } = Resources.Strings.Default_PopupMessage;
            public double PopupOpacity { get; set; } = 0.85;
            public bool SomAtivo { get; set; } = false;
            public string SomArquivo { get; set; } = "";
            public string Idioma { get; set; } = "pt-BR";
        }

        private static ConfigData Data = Load();

        private static ConfigData Load()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath);
                    var obj = JsonSerializer.Deserialize<ConfigData>(json);
                    if (obj != null) return obj;
                }
            }
            catch { }
            return new ConfigData();
        }

        private static void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(Data, options);
            File.WriteAllText(ConfigPath, json);
        }

        // --- Canal ---
        public static string GetCanal() => Data.Canal;
        public static void SetCanal(string canal)
        {
            Data.Canal = canal;
            Save();
        }

        // --- Usuários ignorados ---
        public static List<string> GetIgnorados() => new(Data.Ignorados);
        public static void AddIgnorado(string usuario)
        {
            usuario = usuario.Trim().ToLower();
            if (!Data.Ignorados.Contains(usuario))
            {
                Data.Ignorados.Add(usuario);
                Save();
            }
        }
        public static void RemoveIgnorado(string usuario)
        {
            Data.Ignorados.RemoveAll(u => u.Equals(usuario, StringComparison.OrdinalIgnoreCase));
            Save();
        }
        public static void ClearIgnorados()
        {
            Data.Ignorados.Clear();
            Save();
        }
        public static bool DeveIgnorar(string usuario) =>
            Data.Ignorados.Exists(u => u.Equals(usuario, StringComparison.OrdinalIgnoreCase));

        // --- Posição do pop-up ---
        public static (double X, double Y, double W, double H) GetPopupData()
        {
            double x = Data.PopupPosX;
            double y = Data.PopupPosY;
            double w = Data.PopupWidth;
            double h = Data.PopupHeight;
            return (x, y, w, h);
        }

        public static void SetPopupData(double x, double y, double w, double h)
        {
            Data.PopupPosX = x;
            Data.PopupPosY = y;
            Data.PopupWidth = w;
            Data.PopupHeight = h;
            Save();
        }

        public static int GetPopupDuration()
        {
            return Data.PopupDuration > 0 ? Data.PopupDuration : 3000; // padrão: 3 segundos
        }

        public static void SetPopupDuration(int durationMs)
        {
            if (durationMs < 500)
              throw new ArgumentOutOfRangeException(nameof(durationMs),
                 Strings.Error_PopupTooShort);
            Data.PopupDuration = durationMs;
            Save();
        }

        public static int GetChatIdle()
        {
            return Data.ChatIdleMs > 0 ? Data.ChatIdleMs / 1000 : 5;
        }

        public static void SetChatIdle(int seconds)
        {
            if (seconds < 1)
              throw new ArgumentOutOfRangeException(nameof(seconds),
                Strings.Error_IdleTooShort); // mínimo 1s
            Data.ChatIdleMs = seconds * 1000;
            Save();
        }

        public static (Color Fundo, Color Texto, int Raio, string Mensagem, double Opacidade) GetPopupStyle()
        {
            string fundo = Data.PopupCorFundo ?? "#000000";
            string texto = Data.PopupCorTexto ?? "#FFFFFF";
            int raio = Data.PopupRaio > 0 ? Data.PopupRaio : 30;
            string mensagem = string.IsNullOrEmpty(Data.PopupMensagem)
                ? Strings.Default_PopupMessage
                : Data.PopupMensagem;
            double opacidade = Data.PopupOpacity > 0 ? Data.PopupOpacity : 0.85;

            return (ColorTranslator.FromHtml(fundo), ColorTranslator.FromHtml(texto), raio, mensagem, opacidade);
        }

        public static void SetPopupStyle(Color fundo, Color texto, int raio, string mensagem, double opacidade)
        {
            Data.PopupCorFundo = ColorTranslator.ToHtml(fundo);
            Data.PopupCorTexto = ColorTranslator.ToHtml(texto);
            Data.PopupRaio = raio;
            Data.PopupMensagem = mensagem;
            Data.PopupOpacity = Math.Clamp(opacidade, 0.1, 1.0);
            Save();
        }

        public static (bool Ativo, string Caminho) GetSom()
        {
            bool ativo = Data.SomAtivo;
            string caminho = Data.SomArquivo;
            return (ativo, caminho);
        }

        public static void SetSom(bool ativo, string caminho)
        {
            Data.SomAtivo = ativo;
            Data.SomArquivo = caminho ?? "";
            Save();
        }

        public static void SetIdioma(string idioma)
        {
            Data.Idioma = idioma;
            Save();
        }

        public static string GetIdioma()
        {
            if (string.IsNullOrEmpty(Data.Idioma))
            {
                Data.Idioma = "pt-BR";
                Save();
            }
            return Data.Idioma;
        }
    }
}