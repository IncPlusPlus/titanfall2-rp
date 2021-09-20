# Titanfall 2 Discord Rich Presence

This project aims to allow for Discord's Rich Presence feature to work with Titanfall 2. This is accomplished in a very hacky way. Specifically, known memory addresses are directly read from by this program. Yes, you read that right. I had to mess around in Cheat Engine to find the right memory addresses to read various values from. It's incredibly hacky but at least the code is relatively elegant if I say so myself.

## Important note
#### READ THE [DISCLAIMER BELOW](#disclaimer) BEFORE USING THIS SOFTWARE!!!

## Features
Here's what this application is capable of
- Rich presence features
  - Single player
    - Show campaign mission name
    - Show map previews (there are multiple which are rotated)
  - Multi-player
    - Show game mode
    - Show map preview and name
    - Show current faction
    - Show titan currently in use (planned in [#70](https://github.com/IncPlusPlus/titanfall2-rp/pull/70))
    - Show score* (see [#45](https://github.com/IncPlusPlus/titanfall2-rp/issues/45) for supported game modes)
- Application-specific features
  - Error logging
  - Customization/settings (planned)
  - Ability to start minimized (planned)
  - Ability to minimize to system tray (planned)
  - Automatic updates

## Screenshots

Coming soon...


## Installation
At the moment, there is no install process. Go to [the releases page](https://github.com/IncPlusPlus/titanfall2-rp/releases) or directly to the [latest release](https://github.com/IncPlusPlus/titanfall2-rp/releases/latest) and then download the `.exe` file from the release assets. If that's too much for you, you probably shouldn't try this out.

You're still here? Fine. If you really couldn't follow the instructions above, just click [here](https://github.com/IncPlusPlus/titanfall2-rp/releases/latest/download/titanfall2-rp.exe). Just promise me one thing. Never _**ever**_ file a bug report.

### Issues with Microsoft Defender SmartScreen
Upon attempting to run `titanfall2-rp.exe`, you may be prevented from doing so by a Microsoft Defender SmartScreen window. It should resemble the following:

![image](https://user-images.githubusercontent.com/6992149/133367975-0bc82639-360d-44d0-b916-068c04a06a17.png)

Click "More info" and then click "Run anyway". You will only be prompted like this once. It _may_ happen again after updating.

# OS Support
Currently this is officially supported only on Windows. However, I've been informed that it works on Linux with the use of a Discord IPC bridge. This may have been broken with the introduction of autmatic-updates but I think I mitigated that issue. Please file a bug if that's not the case. I'll provide more information about linux installation steps in the future.

## Usage
1. Open Discord
2. Run this program
3. Open Titanfall 2
4. Have fun!

## Supported Game Modes
While this project _is_ stable, it isn't **_nearly_** complete yet. Multiplayer game data isn't easy to track down in memory. For this reason, this project has a limited set of game modes that it supports. You can still play the unsupported game modes. It won't cause any problems. However, only the supported game modes will show your score and other info on Discord. For the list of supported games, see [#45](https://github.com/IncPlusPlus/titanfall2-rp/issues/45).

## Known Issues
See [the bugs area](https://github.com/IncPlusPlus/titanfall2-rp/issues?q=is%3Aopen+is%3Aissue+label%3Abug) for all the known issues at this time.

## DISCLAIMER

Titanfall 2 appears to have no client-side anti-cheat, nor any game integrity scanning. I personally use this tool and have had no issues with it so far.

### **_HOWEVER_**

I am not responsible for any action taken against you, automatically or manually, by Respawn Entertainment, Electronic Arts, or any anti-cheat that this tool might set off. While I am reasonably sure that Titanfall 2 has no, if not minimal VAC protection, I have not made certain of this. YOU TAKE FULL RESPONSIBILITY OF ANY REPERCUSSIONS THAT YOU MAY INCUR BY USING THIS TOOL.
