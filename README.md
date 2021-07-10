<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]
[![Donate][donate-paypal]](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=ZSY2FXPUHXVUJ)


<!-- ABOUT THE PROJECT -->
## League Broadcast

A central hub for League of Legends stream overlays to augment and elevate streams. 

Currently includes support for champion select and ingame overlays, with post game support as a possible future feature.

League Broadcast uses Memory Reading to get information that the Riot API does not expose. Riot's policy in the past has been to allow passive memory reading, which is exactly what this program does, but this may change at any time.
Use League Broadcast at your own risk. Anti Cheat does not ban for programs used during spectate, but it does however run while in a live game. Having League Broadcast open
during a live (non-spectate) game may lead to account bans incase checks to stop it from running fail for some reason.

## Features

Ingame Features currently include:
1. Level Up Indicators with support for item names
2. Item Purchase Indicators
3. Baron Timer & Power Play Pop Up
4. Elder Timer & Power Play Pop Up
5. Dynamic Gold Graph
6. CS/EXP/Gold Player Tab
7. Custom Scoreboard
8. Inhibitor Timers
9. Auto init UI on Game Start

Includes a C# port of RCVolus ChampSelect tool!

Auto update included

<!-- GETTING STARTED -->
## Getting Started

To get a local copy up and running follow these steps.

### Prerequisites


* Windows 10 20H1 (May 2020 Update) Build 19041
* Active Internet connection

### Installation

1. Download [latest release](https://github.com/floh22/LeagueBroadcastHub/releases/latest)
2. Unzip release to desired install folder
3. Add http://localhost:9001/frontend?backend=localhost as a browser source in OBS in your ingame scene
4. Add http://localhost:9001/?backend=ws://localhost:9001/api as a browser source in OBS in your PickBan scene


<!-- USAGE EXAMPLES -->
## Usage
- Components of League Broadcast can be en-/disabled separately. Enable Pick/Ban or Ingame as you need.

- On first run League Broadcast will download the latest DataDragon cache. 
  -  Champ Select data included to support a port of [RCVolus Pick/Ban](https://github.com/RCVolus/lol-pick-ban-ui) 
  -  Champ Select delay support added! No more waiting between champ select and ingame. Champ select is delayed by a configurable amount to reduce the wait
  
- If League is not installed at the default location, add the folder which contains the "Riot Games" Folder or your "League of Legends" Install folder to **Config/Component.json -> LeagueInstall**. This is a comma separated list which determines where LB will look for League. This is needed for the LiveEvents and Replay API.

- Because League Broadcast uses Memory Reading to gather information, it will not properly work without the correct
memory offsets for each patch. Offsets will be updated in this repo, please feel free to reach out here if they have not.
You should not have to do anything after a patch as offsets will be updated automatically when available, __it can however take around 24 hours or longer!__. You may also set your own offset repo location in configuration if/when this repo stops including updated offsets.


### Switching from RCVolus PickBan Tools

If you have previously used RCVolus PickBan tool and wish to use the version included in LB, build your frontend scene. Where one would usually run
```bash
npm run start
```
to start PickBan, now run 
```bash
npm run build
```
in the same location and move the files in **build/** to **LeagueBroadcastHub/frontend/pickban/**

Make sure Champ Select is enabled

League Broadcast will now host PickBan automatically when started

### Overlay Configuration

`Config/Ingame.json` contains the configuration for the ingame overlay. Change this to suit your needs.

Add any Fonts you wish to use to the comma separated list `"GoogleFonts"`

Change the images and videos in `Frontend/ingame` to swap out resources.

<!-- ROADMAP -->
## Roadmap

Planned Features:
1. Hotkey Support
2. Support for languages other than english. Your language may or may not work depending on if it changes champion names
3. Http/Websocket support for controlling League Broadcast externally

See the [open issues](https://github.com/floh22/LeagueBroadcastHub/issues) for a list of proposed features (and known issues).



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE` for more information.

__This is a standalone project from Lars Eble. Riot Games does not endorse or sponsor this project.__  
_The project was certified as ToS-compliant in a previous state. The current version using memory reading has not yet been certified._ 

_Riot Games has expressed tolerance of passive memory reading in past statements, this however may change at any moment. Use this tool at your own discretion._

This project's port of [lol-pick-ban-ui](https://github.com/RCVolus/lol-pick-ban-ui) and its author are in no way affiliated or partnered with Riot Community Volunteers.


<!-- CONTACT -->
## Contact

Twitter - [@larseble](https://twitter.com/@larseble)

Support this project: [![Donate](https://img.shields.io/badge/Paypal-Donate-blueviolet?style=flat-square&logo=paypal)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=ZSY2FXPUHXVUJ)




<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/floh22/LeagueBroadcast.svg?style=for-the-badge
[contributors-url]: https://github.com/floh22/LeagueBroadcast/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/floh22/LeagueBroadcast.svg?style=for-the-badge
[forks-url]: https://github.com/floh22/LeagueBroadcast/network/members
[stars-shield]: https://img.shields.io/github/stars/floh22/LeagueBroadcast.svg?style=for-the-badge
[stars-url]: https://github.com/floh22/LeagueBroadcast/stargazers
[issues-shield]: https://img.shields.io/github/issues/floh22/LeagueBroadcast.svg?style=for-the-badge
[issues-url]: https://github.com/floh22/LeagueBroadcast/issues
[license-shield]: https://img.shields.io/github/license/floh22/LeagueBroadcast.svg?style=for-the-badge
[license-url]: https://github.com/floh22/LeagueBroadcast/blob/master/LICENSE
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/floh22
[donate-paypal]: https://img.shields.io/badge/Paypal-Donate-blueviolet?style=for-the-badge&logo=paypal
