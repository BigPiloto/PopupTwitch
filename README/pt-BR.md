![Plataforma](https://img.shields.io/badge/plataforma-Windows-blue.svg)
![Linguagem](https://img.shields.io/badge/linguagem-C%23-blueviolet.svg)
![Framework](https://img.shields.io/badge/.NET-8.0-blue.svg)
![Versão](https://img.shields.io/github/v/release/BigPiloto/PopupTwitch.svg)
![Downloads](https://img.shields.io/github/downloads/BigPiloto/PopupTwitch/total.svg)
![Último commit](https://img.shields.io/github/last-commit/BigPiloto/PopupTwitch.svg)
![Licença](https://img.shields.io/github/license/BigPiloto/PopupTwitch.svg)

---

Read in 🇺🇸 [English](../README.md)

# 🎬 Pop-up de Mensagens da Twitch

Aplicativo desktop para Windows desenvolvido em **C# (.NET 8)** que exibe **alertas pop-up em tempo real para atividades no chat da Twitch**.

Ele monitora o canal definido nas configurações e mostra notificações configuráveis sempre que os espectadores enviam mensagens — assim você nunca mais perderá a interação do chat.

Permite personalizar aparência, duração, tempo de inatividade, som de notificação e comportamento geral do pop-up em uma interface moderna e simples.

---

## 🖥️ Visão Geral

O **Pop-up Twitch** é uma ferramenta leve e totalmente local para streamers que desejam saber quando há atividade no chat — **sem exibir o conteúdo das mensagens e sem depender de extensões de navegador ou autenticação externa**.

Ideal para quem se mantém concentrado no jogo e quer apenas um lembrete visual ou sonoro quando alguém envia uma mensagem.

Não requer navegador, plugins nem autenticação externa.

Principais recursos:
- Exibe alertas sobre qualquer janela
- Permite ignorar usuários específicos (como bots)
- Controle total sobre duração, posição, tamanho, opacidade e raio das bordas
- Personalização completa de cores, fontes e texto exibido
- Pré-visualização em tempo real e teste de som
- Rápido, leve e totalmente offline

---

## 🌐 Links Oficiais

🌍 Site: https://popuptwitch.meularsmart.com/

📘 Documentação: https://popuptwitch.meularsmart.com/documentacao/introducao/

---

## 📦 Download

Baixe a versão mais recente na página de [**Releases**](https://github.com/BigPiloto/PopupTwitch/releases).  
> Arquivo: `Pop-upTwitch-v2-Installer.exe`

Após o download, execute o instalador e siga as instruções na tela.

---

## ⚙️ Funcionalidades Principais

| Recurso | Descrição |
|----------|------------|
| 🎨 **Personalização completa** | Altere cores, fontes, raio dos cantos e texto exibido |
| 🔊 **Som de notificação** | Escolha se o alerta será somente visual ou visual e sonoro  |
| ⏱️ **Tempo ocioso** | Defina o intervalo mínimo entre alertas consecutivos |
| ⏱️ **Controle de duração** | Defina por quanto tempo o pop-up permanece visível |
| 🖼️ **Prévia ao vivo** | Veja as alterações de design instantaneamente |
| 🧭 **Editor de posição** | Arraste e redimensione o pop-up na tela |
| 👁️ **Modo não clicável** | O pop-up não bloqueia mais o mouse ou foco da janela |
| 🔧 **Interface moderna** | Layout limpo e fácil de usar |

---

## 🧰 Estrutura do Repositório

PopupTwitch/  

├── source-code/...............→ Código-fonte principal (C# / .NET)  

├── publish/.........................→ Build pronta para distribuição  

├── popup-installer............→ Instalador

├── README.md  

└── LICENSE  

---

## 🚀 Instruções de Build (instalação manual)

1. Instale o .NET SDK 8.0  
2. No diretório raiz do projeto, execute:  
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained false -o "publish"
3. O build final estará na pasta /publish.

---

## 🧾 Licença
Este projeto está licenciado sob a MIT License. Consulte o arquivo [LICENSE](../LICENSE) para mais detalhes.

---

## 📬 Suporte

🐞 Abra uma [Issue](https://github.com/BigPiloto/PopupTwitch/issues)

🌐 Site oficial: https://popuptwitch.meularsmart.com/

📘 Documentação: [Documentação](https://popuptwitch.meularsmart.com/documentacao/introducao/)

☕ Apoie o projeto: https://popuptwitch.meularsmart.com/product/apoie-o-projeto-%e2%98%95/

Autor: **BigPiloto**


