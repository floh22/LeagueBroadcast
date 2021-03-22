# Spectate Overlay
> Front end stream overlay for League Broadcast Hub and lol spectate games

![License](https://img.shields.io/badge/license-MIT-green)

## Capabilities
Currently LBH and this frontend component are able to show the following:

- Player Level Up
- Player item completion
- Baron Power Play
- Elder Power Play

## Prerequisites

You'll need [Node.js](https://nodejs.org/en/), [npm](https://www.npmjs.com/), and [Parcel](https://parceljs.org/) installed.

It is highly recommended to use [Node Version Manager](https://github.com/nvm-sh/nvm) (nvm) to install Node.js and npm.

For Windows users there is [Node Version Manager for Windows](https://github.com/coreybutler/nvm-windows).

A installNode.bat is included to do all these steps for you.
Incase you already have node installed, or wish to install yourself, these are the required steps

Install Node.js and `npm` with `nvm`:

```bash
nvm install node

nvm use node
```

Replace 'node' with 'latest' for `nvm-windows`.

Then install Parcel:

```bash
npm install -g parcel-bundler
```

## Getting Started

Make sure both this repo and League Broadcast Hub are installed on your local machine.
This project should be included in your install of League Broadcast Hub.



Incase you did not use installNode.bat Go into your new project folder and install dependencies:

```bash
cd frontend/ingame/ # or 'my-folder-name'
npm install
```

Start development server if using this separately from LBH:

```
npm run start
```

Copy dist to your web server incase you wish to host this project elsewhere. LBH uses parcel by default to serve files.

## Dev Server Port

You can change the dev server's port number by modifying the `start` script in `package.json`. We use Parcel's `-p` option to specify the port number.

The script looks like this:

```
parcel src/index.html -p 10001
```

Change 10001 to whatever you want.

## License

[MIT License](https://github.com/ourcade/phaser3-typescript-parcel-template/blob/master/LICENSE)
