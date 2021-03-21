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
  <h3 align="center">LeagueOCR</h3>

  <p align="center">
    OCR Analysis on League of Legends spectator games to augment the official Riot Games API
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

<!-- GETTING STARTED -->
## Getting Started

To get a local copy up and running follow these steps.

### Prerequisites


* (OPTIONAL) [LeagueOCR](https://github.com/floh22/LeagueOCR)
* Windows 10 20H1 (May 2020 Update) Build 19041

### Installation

1. Download latest release
2. Unzip release to desired install folder
3. Incase you do not have NodeJS installed on your system, run installNode.bat or install NodeJS manually
4. Run setup.bat or follow frontend install instructions [here](https://github.com/floh22/LeagueBroadcastHub/blob/master/Overlays/ingame/)



<!-- USAGE EXAMPLES -->
## Usage
- On first run League Broadcast Hub will download the latest DataDragon cache. 
  - It is currently far more than needed, but this project will at some point hopefully also be able to support [RCVolus Pick/Ban](https://github.com/RCVolus/lol-pick-ban-ui) frontend as well.

- If you wish to use OCR Data to augment the League of Legends API data, enable OCR in settings and start [LeagueOCR](https://github.com/floh22/LeagueOCR) before spectating.
  - Currently settings are otherwise not functional, but proposed future features.

- Change Player Layout in the Game Information Tab. This will show once a game has started 
  - swap between this page and another incase it does not update

- Enable components you wish to use in Broadcast Triggers Tab.
  - Automatic Events will be shown automatically when selected.
  - Event Control Events will only show when activated.

<!-- ROADMAP -->
## Roadmap

Planned Features:
1. Team Information (Name/Logo)
2. Team Standings
3. Inhibitor Indicators
4. Gold Graphs
5. CS/min Graphs
6. Tournament/Game Series support
7. Pick/Ban support
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



<!-- CONTACT -->
## Contact

Lars Eble - [@larseble](https://twitter.com/@larseble)

Project Link: [https://github.com/floh22/LeagueBroadcastHub](https://github.com/floh22/LeagueBroadcastHub)






<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/floh22/LeagueOCR.svg?style=for-the-badge
[contributors-url]: https://github.com/floh22/LeagueOCR/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/floh22/LeagueOCR.svg?style=for-the-badge
[forks-url]: https://github.com/floh22/LeagueOCR/network/members
[stars-shield]: https://img.shields.io/github/stars/floh22/LeagueOCR.svg?style=for-the-badge
[stars-url]: https://github.com/floh22/LeagueOCR/stargazers
[issues-shield]: https://img.shields.io/github/issues/floh22/LeagueOCR.svg?style=for-the-badge
[issues-url]: https://github.com/floh22/LeagueOCR/issues
[license-shield]: https://img.shields.io/github/license/floh22/LeagueOCR.svg?style=for-the-badge
[license-url]: https://github.com/floh22/LeagueOCR/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/floh22
