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



<!-- PROJECT LOGO -->
<br />
<p align="center">
  <h3 align="center">League Broadcast Hub</h3>

  <p align="center">
    A Collection of League of Legends Spectate Overlay tools to enhance streams and tournament productions
    <br />
    <a href="https://github.com/floh22/LeagueBroadcastHub"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/floh22/LeagueBroadcastHub">View Demo</a>
    ·
    <a href="https://github.com/floh22/LeagueBroadcastHub/issues">Issues</a>
    ·
    <a href="https://github.com/floh22/LeagueBroadcastHub/issues">Request Feature</a>
  </p>
</p>

<!-- ABOUT THE PROJECT -->
## About The Project

Currently no proper broadcast solution for League of Legends streams exists which can recreate the features of Riot Broadcasts.

This project aims to change that and provide one such singular broadcast solution.

## Features

Features currently include:
1. Level Up Indicators
2. Item Purchase Indicators
3. Baron Power Play
4. Elder Power Play
5. Dynamic Gold Graph

Includes a C# port of RCVolus ChampSelect tool!

<!-- GETTING STARTED -->
## Getting Started

To get a local copy up and running follow these steps.

### Prerequisites


* (OPTIONAL) [LeagueOCR](https://github.com/floh22/LeagueOCR)
* Windows 10 20H1 (May 2020 Update) Build 19041

### Installation

1. Download latest release
2. Unzip release to desired install folder
3. Add http://localhost:9001/frontend as a browser source in OBS in your ingame scene
4. Add http://localhost:9001/ as a browser source in OBS in your PickBan scene


<!-- USAGE EXAMPLES -->
## Usage
- Components of LBH can be en-/disabled separately. Enable Pick/Ban or Ingame as you need.

- On first run League Broadcast Hub will download the latest DataDragon cache. 
  -  Champ Select data included to support a port of [RCVolus Pick/Ban](https://github.com/RCVolus/lol-pick-ban-ui) 
  -  Champ Select delay support added! No more waiting between champ select and ingame. Champ select is delayed by a configurable amount to reduce the way

- If you wish to use OCR Data to augment the League of Legends API data, enable OCR in settings and start [LeagueOCR](https://github.com/floh22/LeagueOCR) before spectating.
  - Currently settings are otherwise not functional, but proposed future features.

- Change Player Layout in the Game Information Tab. This will show once a game has started 

- Enable components you wish to use in Broadcast Triggers Tab.
  - Automatic Events will be shown automatically when selected.
  - Event Control Events will only show when activated.

### Switching from RCVolus PickBan Tools

If you have previously used RCVolus PickBan tool and wish to use the version included in LBH, build your frontend scene. Where one would usually run
```bash
npm run start
```
to start PickBan, now run 
```bash
npm run build
```
in the same location and move the files in **build/** to **LeagueBroadcastHub/frontend/pickban/**

Enable Champ Select in Settings

LeagueBroadcastHub will now host PickBan automatically when started

<!-- ROADMAP -->
## Roadmap

Planned Features:
1. Team Information (Name/Logo)
2. Team Standings
3. Inhibitor Indicators
4. ~~Gold Graphs~~  _finished_
5. CS/min Graphs
6. Tournament/Game Series support
7. ~~Pick/Ban support~~ _finished_
8. Spectate API support
9. Web Control
10. Manual Overrides

See the [open issues](https://github.com/floh22/LeagueBroadcastHub/issues) for a list of proposed features (and known issues).



<!-- CONTRIBUTING -->
## Contributing

Any contributions you make are **greatly appreciated**. I am by no means an expert at .NET development and this project is somewhat of a mess in parts.

I am particularly having issues with publishing this project to a single executable, so if you have any experience with that, I would love the help.

<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE` for more information.

__This is a standalone project from Lars Eble. Riot Games does not endorse or sponsor this project.__  
_The project has not been certified as ToS-compliant yet though it has been submitted. _ 

This project's port of [lol-pick-ban-ui](https://github.com/RCVolus/lol-pick-ban-ui) and its author are in no way affiliated or partnered with Riot Community Volunteers.


<!-- CONTACT -->
## Contact

Lars Eble - [@larseble](https://twitter.com/@larseble)

Project Link: [https://github.com/floh22/LeagueBroadcastHub](https://github.com/floh22/LeagueBroadcastHub)






<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/floh22/LeagueBroadcastHub.svg?style=for-the-badge
[contributors-url]: https://github.com/floh22/LeagueBroadcastHub/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/floh22/LeagueBroadcastHub.svg?style=for-the-badge
[forks-url]: https://github.com/floh22/LeagueBroadcastHub/network/members
[stars-shield]: https://img.shields.io/github/stars/floh22/LeagueBroadcastHub.svg?style=for-the-badge
[stars-url]: https://github.com/floh22/LeagueBroadcastHub/stargazers
[issues-shield]: https://img.shields.io/github/issues/floh22/LeagueBroadcastHub.svg?style=for-the-badge
[issues-url]: https://github.com/floh22/LeagueBroadcastHub/issues
[license-shield]: https://img.shields.io/github/license/floh22/LeagueBroadcastHub.svg?style=for-the-badge
[license-url]: https://github.com/floh22/LeagueBroadcastHub/blob/master/LICENSE
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/floh22
