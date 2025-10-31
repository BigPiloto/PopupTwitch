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

**Pop-up Twitch** is a lightweight and fully local tool designed for streamers who want to be notified when new chat messages arrive — **without displaying the message content or Twitch events**, and without relying on browser extensions or external services.  
Ideal for those who get deeply focused on the game and just need a visual or sound reminder that there’s chat activity.  
No browser, plugins, or external authentication required.

Key features:
- Displays custom pop-up alerts over any window  
- Option to ignore specific users (perfect for avoiding notifications from bots)  
- Full control over duration, position, size, and style (colors, fonts, rounded corners)  
- Real-time preview and idle-time adjustment  
- Clean, intuitive configuration interface  
- Optional sound notification (disabled by default)  

---

## 📦 Download

Get the latest version from the [**Releases**](https://github.com/BigPiloto/PopupTwitch/releases) page.  
> File: `Pop-upTwitch-v2-Installer.exe`

After downloading, run the installer and follow the on-screen instructions.  
Requires **.NET Desktop Runtime 8.0** (available at [dotnet.microsoft.com](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)).

---

## ⚙️ Main Features

| Feature | Description |
|----------|-------------|
| 🎨 **Full customization** | Change colors, fonts, corner radius, and text displayed |
| 🔊 **Sound notification** | Choose between visual-only or visual + sound alerts |
| ⏱️ **Idle time** | Define the minimum interval between consecutive alerts |
| ⏱️ **Duration control** | Set how long the pop-up remains visible |
| 🖼️ **Live preview** | Instantly see all design changes |
| 🔧 **Modern interface** | Clean and user-friendly layout |

---

## 🧰 Repository Structure

PopupTwitch/  

├── source-code/...............→ Main source code (C# / .NET)  

├── publish/.........................→ Build ready for distribution  

├── popup-installer............→ Installer files  

├── README.md  

└── LICENSE  

---

## 🚀 Build Instructions

1. Install .NET SDK 8.0  
2. From the project root, run:  
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained false -o "publish"
3. The final build will be in the /publish folder.

---

## 🧾 License

This project is licensed under the MIT License. See the LICENSE file for details.

---

## 📬 Support

Open an [Issue](https://github.com/BigPiloto/PopupTwitch/issues)
Author: BigPiloto
