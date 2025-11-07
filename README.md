![Platform](https://img.shields.io/badge/platform-Windows-blue.svg)
![Language](https://img.shields.io/badge/language-C%23-blueviolet.svg)
![Framework](https://img.shields.io/badge/.NET-8.0-blue.svg)
![Release](https://img.shields.io/github/v/release/BigPiloto/PopupTwitch.svg)
![Downloads](https://img.shields.io/github/downloads/BigPiloto/PopupTwitch/total.svg)
![Last commit](https://img.shields.io/github/last-commit/BigPiloto/PopupTwitch.svg)
![License](https://img.shields.io/github/license/BigPiloto/PopupTwitch.svg)

---

Ler em 🇧🇷 [Português Brasil](README/pt-BR.md)

# 🎬 Pop-up Twitch Messages

Windows desktop application built in **C# (.NET 8)** that displays **real-time pop-up alerts for Twitch chat activity**.  

It monitors the Twitch channel defined in the settings and shows configurable on-screen notifications whenever viewers send messages — so you’ll never miss a chat message again.  

You can customize appearance, duration, idle time, notification sound, and overall pop-up behavior through a modern and simple interface.

---

## 🖥️ Overview

**Pop-up Twitch** is a lightweight and fully local tool for streamers who want to know when there’s chat activity — **without showing message contents**, and without needing a browser extension or authentication.

Perfect for streamers who stay focused on gameplay and just need a quick **visual** or **sound cue** that someone sent a message. 

No browser, plugins, or external authentication required.

Key features:
- Show pop-up alerts above any window
- Ignore specific users (e.g., bots)
- Control duration, position, size, opacity, and corner radius
- Customize colors, fonts, and displayed text
- Real-time preview and sound test
- Lightweight and fast — runs locally

---

## 🌐 Official Links

🌍 Website: https://popuptwitch.meularsmart.com/en/

📘 Documentation: https://popuptwitch.meularsmart.com/en/documentacao/introducao/

---

## 📦 Download

Get the latest version from the [**Releases**](https://github.com/BigPiloto/PopupTwitch/releases) page.  
> File: `Pop-upTwitch-v2-Installer.exe`

After downloading, run the installer and follow the on-screen instructions.  

---

## ⚙️ Main Features

| Feature | Description |
|----------|-------------|
| 🎨 **Full customization** | Change colors, fonts, corner radius, and text displayed |
| 🔊 **Sound notification** | Choose between visual-only or visual + sound alerts |
| ⏱️ **Idle time** | Define the minimum interval between consecutive alerts |
| ⏱️ **Duration control** | Set how long the pop-up remains visible |
| 🖼️ **Live preview** | Instantly see all design changes |
| 🧭 **Position editor** | Drag and resize the popup on screen |
| 👁️ **Non-clickable mode** | Popup no longer blocks mouse or window focus |
| 🔧 **Modern interface** | Clean and user-friendly layout |

---

## 🧰 Repository Structure

PopupTwitch/  

├── source-code/...............→ Main source code (C# / .NET)  

├── popup-installer............→ Installer files  

├── README.md  

└── LICENSE  

---

## 🚀 Build Instructions (Manual installation)

1. Install .NET SDK 8.0  
2. Clone this repository and open the /source-code folder
3. From the project root, run:  
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained false -o "publish"
4. The final build will be in the /publish folder.
(this folder is not tracked in the repository — it’s for your local build only).

---

## 🧾 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## 📬 Support

🐞 Open an [Issue](https://github.com/BigPiloto/PopupTwitch/issues)

🌐 Visit the website: https://popuptwitch.meularsmart.com/en/

📘 Documentation: [Documentation Portal](https://popuptwitch.meularsmart.com/en/documentacao/introducao/)

Support the project: https://popuptwitch.meularsmart.com/en/product/apoie-o-projeto-%e2%98%95/

Author: BigPiloto
