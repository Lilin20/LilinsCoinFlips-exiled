# 🎲 Lilin's CoinFlip Plugin

A customizable **coin flip effect system** for EXILED beta (LabAPI), letting you define good and bad outcomes via YAML config files — no recompilation required!

Exiled Version: 9.6.0-beta8

---

## 🚀 Features

- 🎯 Coin flip triggers with random outcomes
- 🟢 Good / 🔴 Bad effect lists
- 🧾 Easy YAML configuration
- 🔁 Reload effects at runtime (optional)
- 🧩 Add multiple actions per effect
- 🧠 Simple to extend with custom actions

---

## 📁 YAML Configuration

### 🔧 Files
You should place two files inside the plugin's config directory:

- `good.yaml`: For positive outcomes (coin lands on heads)
- `bad.yaml`: For negative outcomes (coin lands on tails)

### 🧾 Format Example

```yaml
- message: "You've received healing items!"
  chance: 20
  actions:
    - type: "SpawnItems"
      parameters:
        items:
          - "Medkit"
          - "Painkillers"
```

---

## 🔑 YAML Fields

| Field       | Type               | Description                                               |
|-------------|--------------------|-----------------------------------------------------------|
| `message`   | `string`           | Message shown when this effect is triggered               |
| `chance`    | `int`              | Weight for random selection (higher = more likely)        |
| `actions`   | `list<object>`     | List of actions to run sequentially                       |
| `type`      | `string`           | Action name (see below)                                   |
| `parameters`| `dictionary`       | Parameters for the specific action                        |

---

## ⚙️ Supported Actions

### 🔹 `SpawnItems`

Spawns one or more items at the player's location.

```yaml
- type: "SpawnItems"
  parameters:
    items:
      - "Medkit"
      - "Painkillers"
```

---

### 🔹 `SpawnCustomItems`

Spawns one or more custom items at the player's location.

```yaml
- type: "SpawnCustomItems"
  parameters:
    items:
      - 100
      - 200
```

---

### 🔹 `TeleportToRoom`

Teleports the player to a specific room.

```yaml
- type: "TeleportToRoom"
  parameters:
    room: "LczArmory"
```

---

### 🔹 `TeleportRandom`

Teleports the player to a random room from a list.

```yaml
- type: "TeleportRandom"
  parameters:
    room:
      - "LCZ_ClassDSpawn"
      - "HCZ_Testroom"
```

---

### 🔹 `RandomEffect`

Applies one random status effect from a list.

```yaml
- type: "RandomEffect"
  parameters:
    effects:
      - "Asphyxiated"
      - "Burned"
      - "Flashed"
      - "Bleeding"
```

✅ Supported values: Any valid EffectType

---

## 🔁 Reloading YAML Files

In order to reload your effects just type "lcfr" into the RA-Console.

---

## 💡 Tips

- Use `#` in YAML for comments
- Use [YAML validators](https://www.yamllint.com/) to check for format errors
- Combine actions for advanced effects (e.g., spawn + teleport + status)

---

## 📜 License

MIT — free for personal or commercial use.

---

## ❤️ Credits

Developed by **Lilin** using EXILED and [YamlDotNet](https://github.com/aaubry/YamlDotNet).
