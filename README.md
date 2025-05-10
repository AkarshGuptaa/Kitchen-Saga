# 🍳 Kitchen Saga  
**Voice-Controlled Cooking Chaos for Everyone!**

---

## 🔹 About the Game

**Kitchen Saga** is a single-player, fully voice-controlled cooking simulator designed with accessibility in mind — specifically for players who cannot use traditional input devices such as a keyboard, mouse, or controller.

Inspired by *Overcooked*, you play as a solo chef navigating a busy kitchen by shouting commands like `"bun"`, `"chicken"`, `"gas"`, `"cheese"`, `"board"` and more to teleport, interact, cook, and serve recipes. Every order is a surprise, and the goal is to complete as many recipes as possible before time runs out.

Kitchen Saga provides a **fun, fast-paced, and empowering experience for all — no hands needed.**

---

## 🗂 File Overview

- `GameInput.cs`  
  Core script handling player input via both keyboard and voice recognition using Unity's `KeywordRecognizer`.  
  - Maps voice commands to in-game teleportation and interaction actions.  
  - Handles recipe interaction, object pickup/placement, and teleportation using simple speech.  
  - Stores customizable keybindings and supports voice remapping.

---

## 🎮 How to Play (Voice Commands)

Speak one of the following commands to control your character:

| **Command** | **Action (Teleports to / Interacts with)**        |
|-------------|----------------------------------------------------|
| `chicken`   | Moves to the chicken station                       |
| `cheese`    | Moves to the cheese counter                        |
| `bun`       | Moves to the bun counter                           |
| `tomato`    | Moves to the tomato station                        |
| `lettuce`   | Moves to the lettuce station                       |
| `gas`       | Moves to the stove                                 |
| `board`     | Moves to the chopping board                        |
| `plate`     | Moves to the serving plate                         |
| `send`      | Moves to the serve/delivery area                   |
| `bin`       | Moves to the trash bin                             |
| `counter`   | Moves to empty counter space                       |
| `empty`     | Same as `"counter"`                                |
| `pick`      | Pick up an object *(interact)*                     |
| `place`     | Place an object *(interact)*                       |
| `cook`      | Start cooking *(interact)*                         |
| `chop`      | Start chopping *(alternate interact)*              |
| `start`     | Start or interact with objects                     |

> ⚠️ Make sure your microphone is active and permissions are granted.

---

## 🧠 Features

- 🎙️ Fully **voice-controlled** gameplay (no keyboard or mouse needed)
- 🔊 Dynamic **keyword recognition** using Unity's `KeywordRecognizer`
- 📍 Smooth **teleportation** to key kitchen stations
- 🛠️ Customizable keybindings and command mapping
- ♿ Designed for **players with limited mobility**
- 👁️ Visual and debug feedback in Unity Console for voice events

---

## ⚙️ Technology Used

- **Unity Engine** (C#)
- `UnityEngine.Windows.Speech` for voice recognition
- Custom voice command → key simulation system
- Rigidbody-based teleportation and motion control

---

## 📦 Setup Instructions

1. Open the project in **Unity**.
2. Attach `GameInput.cs` to a GameObject in your scene.
3. Tag your **player GameObject** as `"Player"`.
4. Make sure your **microphone is working** and permitted by your OS.
5. Hit **Play** and start speaking commands!

> 🎯 Optional: Modify teleport coordinates or add/remove commands in `GameInput.cs` for custom kitchen layouts.

---

## 🙌 Credits

- **Game by:** Akarsh  
- **Voice Input Logic:** Powered by Unity’s built-in speech recognition  
- 💖 Special thanks to anyone supporting accessible game design.

---

## 📧 Contact

For support, feedback, or collaboration, reach out at:  
**[Your GitHub | Email | Portfolio link here]**

---

*Let your voice be the controller. Welcome to the Kitchen Saga.*

---
