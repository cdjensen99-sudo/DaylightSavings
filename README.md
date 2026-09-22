# DaylightSavings

Valheim BepInEx mod that **pauses fires and lights when they are not needed**. Remaining fuel stays in the piece. They auto-relight when the condition flips.

**Current version:** 1.0.0 — Valheim 1.0 complete  
**Requires:** Valheim **1.0** (Unity 6) and [BepInEx Pack for Valheim](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/) **5.4.2350+**  
**Install:** **Client-side.** Dedicated server is not required. **Every player who joins should install it.**

**Links**
- **[Team Extreme Discord](https://discord.gg/cCNG8xKXMn)** — setup help, bug reports, and updates
- **[GitHub — DaylightSavings](https://github.com/cdjensen99-sudo/DaylightSavings)** — source and issues

---

## What it does

**Fires** (campfire, iron fire pit, bonfire, hearth) pause when no player is within **10 m** (configurable). They still burn in daytime if you are standing there to cook.

**Lights** (sconces, standing torches, green/blue/wisp torches, braziers, jack-o-turnip, lava lantern) use two gates:

1. Someone within **10 m**, or they pause — including at night in an empty hall.
2. If the *space* has usable daylight (windowed above-ground room, open sun), they pause even with a player in the room.

A sealed pit under another room has no usable daylight. Those lights burn only while someone is in range, day or night.

Vanilla indoor ambient barely changes in a basement, so this does **not** use sneak light-level. It uses cover (windows lower it) plus a sun ray into the room, not a shadow test on the wall sconce itself.

## Always burn

Vanilla fires and lights have no on/off switch. **Alt+E** (Alt+Use) on a piece pins it to **Always burn** — it stays lit and consumes fuel whenever it has fuel, ignoring DaylightSavings. Alt+E again returns it to **Managed**. Hover text shows the mode.

This is not a fire toggle. Empty pieces still go out.

## Compatibility

- **Always stay lit** mods (NoSmokeStayLit keep-lit, and similar) conflict. DaylightSavings runs last on `Fireplace.IsBurning` and wins the pause.
- Covers every vanilla piece that uses `Fireplace`. Modded fires/lights using that component are classified as fires if they burn wood, otherwise as lights. Override with config lists.
- Does **not** touch smelters, kilns, ovens, or cooking stations.

## Config (`BepInEx/config/Hardwire99.DaylightSavings.cfg`)

| Key | Default | Meaning |
|-----|---------|---------|
| `Enabled` | `true` | Master switch |
| `ProximityMeters` | `10` | Range for fires and for lights without usable daylight |
| `PauseFiresWhenNoPlayerNearby` | `true` | Fires pause when nobody is in range |
| `PauseLightsWhenNoPlayerNearby` | `true` | Lights pause when nobody is in range |
| `PauseLightsInUsableDaylight` | `true` | Lights pause in windowed/open daylight |
| `SealedCoverThreshold` | `0.8` | Cover fraction treated as a sealed pit (vanilla shelter) |
| `ExtraFirePrefabs` | _(empty)_ | Extra prefab names treated as fires |
| `ExtraLightPrefabs` | _(empty)_ | Extra prefab names treated as lights |
| `ExcludePrefabs` | _(empty)_ | Prefab names never paused |

## Install

Place `DaylightSavings.dll` in `BepInEx/plugins/` (Gale folder `Hardwire99-DaylightSavings`).

Fireplace fuel is simulated by the **nearby player's client**, not a dedicated server. Mixed sessions desync flames and only pause fuel on pieces owned by someone who has the mod.

## Build

```powershell
.\build.ps1 -Deploy                  # Build + copy into Gale New Release
.\build.ps1 -Package                 # Build + Thunderstore zip in artifacts\
```

Or: `dotnet build DaylightSavings.sln -c Release`.

Compile refs: Valheim 1.0 / Unity 6 from `D:\SteamLibrary\steamapps\common\Valheim\valheim_Data\Managed`, BepInExPack **5.4.2350**. Target framework `net472`.
