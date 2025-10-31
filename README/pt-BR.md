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
Ele monitora as mensagens do canal da Twitch definido nas configurações e exibe notificações configuráveis ​​na tela sempre que os espectadores enviam mensagens, assim você nunca mais irá perder uma mensagem de seus viewers.
Permite configurar aparência, duração, tempo ocioso, som de notificação e demais comportamento dos pop-ups através de uma interface moderna e simples.

---

## 🖥️ Visão Geral

O **Pop-up Twitch** é uma ferramenta leve e totalmente local, criada para streamers que desejam ser avisados quando há novas mensagens no chat, **sem exibir o conteúdo das mensagens nem eventos do canal**, sem depender de extensões de navegador ou serviços externos.
Ideal para quem fica totalmente focado no jogo e quer apenas um lembrete visual ou sonoro de que há atividade no chat.
Não requer navegador, plugins nem autenticação externa.

Principais recursos:
- Exibe pop-ups personalizados sobre qualquer janela
- Possibilidade de ignorar usuários (perfeito para não receber notificação quando seu bot falar)
- Controle de duração, posição, tamanho e estilo (cores, fontes, cantos arredondados)  
- Visualização em tempo real e ajuste de ociosidade
- Interface de configuração intuitiva e limpa
- Notificação sonora (padrão: desativado)

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

## 🚀 Instruções de Build

1. Instale o .NET SDK 8.0  
2. No diretório raiz do projeto, execute:  
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained false -o "publish"
3. O build final estará na pasta /publish.

---

## 🧾 Licença
Este projeto está licenciado sob a MIT License. Consulte o arquivo LICENSE para mais detalhes.

---

## 📬 Suporte

Abra uma [Issue](https://github.com/BigPiloto/PopupTwitch/issues)
Autor: **BigPiloto**


