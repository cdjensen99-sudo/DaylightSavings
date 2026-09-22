# DaylightSavings

**Pause fires and lights when they are not needed.** Remaining fuel stays put. They relight when someone walks up, or when daylight is gone.

**Current version:** 1.0.0 — Valheim 1.0 complete  
**Requires:** Valheim **1.0** (Unity 6) and [BepInEx Pack for Valheim](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/) **5.4.2350+**  
**Install:** **Client-side.** Dedicated server is not required. **Every player who joins should install it.**

**Links**
- **[Team Extreme Discord](https://discord.gg/cCNG8xKXMn)** — setup help, bug reports, and updates
- **[GitHub — DaylightSavings](https://github.com/cdjensen99-sudo/DaylightSavings)** — source and issues

---

## Fires

Campfires, iron fire pits, hearths, and bonfires pause when no player is within **10 m**. Walk back and they relight with the same fuel. They still burn in the daytime while you cook.

## Lights

Sconces, standing torches, colored torches, wisp torches, braziers, jack-o-turnip, and lava lanterns:

- Empty building at night: **off**.
- Windowed hall in full daylight: **off**, even if you are standing in it.
- Sealed pit / basement with no openings: **on** only while someone is within 10 m, day or night.

## Always burn

Vanilla pieces have no on/off control. **Alt+E** on a fire or light pins it to **Always burn** (vanilla fuel use). Alt+E again returns it to Managed. Hover text shows which mode it is in.

## Compatibility

Stay-lit mods that force `Fireplace.IsBurning` true conflict — this mod runs last and still pauses. Smelters, kilns, and ovens are not touched.

## Config

| Key | Default | Meaning |
|-----|---------|---------|
| `ProximityMeters` | `10` | Player range |
| `PauseLightsInUsableDaylight` | `true` | Windowed / open daylight pauses lights |
| `SealedCoverThreshold` | `0.8` | Sealed pit vs windowed hall |
| `ExtraFirePrefabs` / `ExtraLightPrefabs` / `ExcludePrefabs` | | Prefab name lists |

Full keys live in `BepInEx/config/Hardwire99.DaylightSavings.cfg`.
