# 🛠️ Prefab Icon Renderer for Unity

A lightweight Unity Editor tool to render clean prefab icons into PNG sprites

---

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

## 📂 Installation

### Option 1: UnityPackage  
1. Download the latest `.unitypackage` from [this link](#)  
2. In Unity: `Assets → Import Package → Custom Package...`  
3. Import the file  
4. Done ✅

### Option 2: Manual Copy  
1. Download `PrefabIconRenderer.cs`  
2. Place it under: `Assets/Editor/PrefabIconRenderer.cs`  
3. Open Unity → `Tools → Render Prefab Icon`

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

Made with ❤️ by **Lokbit**

Licensed under the [MIT License](LICENSE)

Feel free to contribute, fork or use it in your own games/projects!
