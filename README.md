# 🛠️ Prefab Icon Renderer for Unity

A lightweight Unity Editor tool to render clean prefab icons into PNG sprites

---
- **Showcase**  https://github.com/user-attachments/assets/9db85170-cc91-46dd-9bdf-0df15d85834d


## ✨ Features

- **Live preview** of your icon with all adjustments  
- **Transparent or solid color background**
- **Add optional frame and background sprites**
- **Color tint** for frame & background  
- **Object rotation** and **zoom control**
- **Visual auto-centering** based on renderer bounds
- **Custom file naming** and **PNG resolution**
- **One-click render & save**
- Output is **imported as Unity sprite**, ready to use in UI  
- All inside a **scrollable, user-friendly editor window**

---

📂 Installation
Just clone or copy the tool into your Unity project – no setup required.

✅ Quick Install
Open your Unity project

Copy the folder Assets/Editor/PrefabIconRenderer.cs from this repository into your project

In Unity, go to:
Tools → Render Prefab Icon

Start rendering! 

---

## 🎮 How to Use

1. Open via `Tools → Render Prefab Icon`  
2. Select any prefab from your project  
3. Customize background, frame, zoom, rotation, etc.  
4. Click **“Render & Save Icon”**  
5. The result appears in `Assets/GeneratedSprites/`

---

## 🛠 Output Settings

- Format: `.png`  
- Resolution: 128–1024 px  
- Unity import settings:  
  - Texture Type: **Sprite (2D and UI)**  
  - Sprite Mode: **Single**  
  - Alpha is Transparency: ✅  
  - Mipmaps: ❌  
  - Filter: **Bilinear**  
  - Compression: **Uncompressed**

---


## 💡 Roadmap / Ideas

- [ ] Batch icon rendering  
- [ ] Preset save/load  
- [ ] Custom alpha masks  
- [ ] Drag & drop prefab support  

---

## ❤️ Credits & License

Made with by **Lokbit**

Licensed under the [MIT License](LICENSE)

Feel free to contribute, fork or use it in your own games/projects!
