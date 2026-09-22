# Changelog

## 1.0.0 — Valheim 1.0

- Requires **Valheim 1.0** (Unity 6) and [BepInEx Pack for Valheim](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/) **5.4.2350+**.
- First release: pauses fueled `Fireplace` pieces when they are not needed and keeps remaining fuel.
- **Fires** (campfire, iron fire pit, hearth, bonfire) pause when no player is within **10 m** (configurable). They still burn in daytime while you cook.
- **Lights** (sconces, standing torches, colored/wisp torches, braziers, jack-o-turnip, lava lantern) pause unless someone is in range **and** the space has no usable daylight.
- Windowed halls in full sun stay off even with a player standing there. Sealed pits only light while someone is nearby, day or night.
- **Alt+E** pins a piece to Always burn (vanilla fuel use) or back to Managed. Hover text shows the mode and pause reason.
- Fuel catch-up after an unloaded zone is skipped so walking back in does not dump a huge fuel bill.
- **Client-side.** Dedicated server is not required. Every player who joins should install it. Stay-lit mods conflict; DaylightSavings runs last on `Fireplace.IsBurning`.
