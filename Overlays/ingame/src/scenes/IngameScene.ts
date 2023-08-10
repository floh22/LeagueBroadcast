import 'phaser';
import variables from '~/variables';
import StateData from '~/data/stateData';
import Phaser from 'phaser';
import GoldEntry from '~/data/goldEntry';
import WindowUtils from '~/convert/windowUtils';
import OverlayConfigEvent, { ObjectiveSpawnConfig, OverlayConfig, ScoreboardPopUpConfig } from '~/data/config/overlayConfig';
import { VisualElement } from '~/visual/VisualElement';
import RegionMask from '~/data/RegionMask';

import ItemVisual from '~/visual/ItemVisual';
import LevelUpVisual from '~/visual/LevelUpVisual';
import InhibitorVisual from '~/visual/InhibitorVisual';
import PowerPlayVisual from '~/visual/PowerPlayVisual';
import ScoreboardVisual from '~/visual/ScoreboardVisual';
import InfoPageVisual from '~/visual/InfoPageVisual';
import GraphVisual from '~/visual/GraphVisual';
import ObjectivePopUpVisual from '~/visual/ObjectivePopUpVisual';
import ObjectiveTimerVisual from '~/visual/ObjectiveTimerVisual';
import Utils from '~/util/Utils';

export default class IngameScene extends Phaser.Scene {
    [x: string]: any;
    ws!: WebSocket;
    displayRegions: RegionMask[];
    loadedFonts: string[];

    baronPowerPlay!: PowerPlayVisual;
    elderPowerPlay!: PowerPlayVisual;

    dragonTimer!: ObjectiveTimerVisual;
    baronTimer!: ObjectiveTimerVisual;

    inhib!: InhibitorVisual;
    score!: ScoreboardVisual;
    info!: InfoPageVisual;
    centerGraph!: GraphVisual;

    state!: StateData | null;
    overlayCfg!: OverlayConfig | null;

    graphics!: Phaser.GameObjects.Graphics;
    currentVisualElements: VisualElement[] = [];
    visualIDs: number = 0;

    static Instance: IngameScene;


    constructor() {
        super('ingameOverlay');
        IngameScene.Instance = this;
        this.displayRegions = [];
        this.loadedFonts = [];
    }

    preload() {
        variables.backendUrl = WindowUtils.GetQueryVariable('backend');
        this.graphics = this.add.graphics();
        this.load.script('chartjs', 'https://cdnjs.cloudflare.com/ajax/libs/Chart.js/3.4.1/chart.min.js');

        //Masks
        this.load.image('champCoverLeft', 'frontend/masks/ChampCoverLeft.png');
        this.load.image('champCoverRight', 'frontend/masks/ChampCoverRight.png');
        this.load.image('scoreboardMask', 'frontend/masks/ScoreboardMask.png');
        this.load.image('itemTextMaskLeft', 'frontend/masks/ItemTextLeft.png');
        this.load.image('itemTextMaskRight', 'frontend/masks/ItemTextRight.png');
        this.load.image('infoPageMask', 'frontend/masks/InfoPage.png');
        this.load.image('graphMask', 'frontend/Masks/Graph.png');
        this.load.image('popUpMask', 'frontend/Masks/PopUpMask.png')

        //Objective Timers
        this.load.image('baronIcon', 'frontend/backgrounds/BaronIcon.png');
        this.load.image('objectiveBg', 'frontend/backgrounds/ObjectiveBG.png');
        this.load.image('elderIcon', 'frontend/backgrounds/DragonIcon.png');
        this.load.image('objectiveBgLeft', 'frontend/backgrounds/ObjectiveBGLeft.png');
        this.load.image('objectiveGold', 'frontend/images/ObjectiveGold.png');
        this.load.image('objectiveCdr', 'frontend/images/ObjectiveCdr.png');

        this.load.image('dragonTimerBg', 'frontend/backgrounds/DragonTimer.png');
        this.load.image('baronTimerBg', 'frontend/backgrounds/BaronTimer.png');

        //Scoreboard
        this.load.image('scoreGold', 'frontend/images/ScoreboardGold.png');
        this.load.image('scoreTower', 'frontend/images/tower.png');
        this.load.image('scoreCenter', 'frontend/images/ScoreboardCenterIcon.png');
        this.load.image('scoreBlueIcon', 'frontend/backgrounds/ScoreTeamIconBGLeft.png');
        this.load.image('scoreRedIcon', 'frontend/backgrounds/ScoreTeamIconBGRight.png');


        //Info Tab
        this.load.image('infoPageSeparator', 'frontend/images/InfoTabSeparator.png');

        //Dragons
        this.load.image('dragon_Fire', 'frontend/images/dragons/scoreboard/fireLarge.png');
        this.load.image('dragon_Mountain', 'frontend/images/dragons/scoreboard/mountainLarge.png');
        this.load.image('dragon_Cloud', 'frontend/images/dragons/scoreboard/cloudLarge.png');
        this.load.image('dragon_Ocean', 'frontend/images/dragons/scoreboard/oceanLarge.png');
        this.load.image('dragon_Elder', 'frontend/images/dragons/scoreboard/elderLarge.png');
        this.load.image('dragon_Hextech', 'frontend/images/dragons/scoreboard/hextechLarge.png');
        this.load.image('dragon_Chemtech', 'frontend/images/dragons/scoreboard/chemtechLarge.png');

        this.load.image('timer_Fire', 'frontend/images/dragons/timers/fireTimer.png');
        this.load.image('timer_Mountain', 'frontend/images/dragons/timers/mountainTimer.png');
        this.load.image('timer_Cloud', 'frontend/images/dragons/timers/cloudTimer.png');
        this.load.image('timer_Ocean', 'frontend/images/dragons/timers/oceanTimer.png');
        this.load.image('timer_Elder', 'frontend/images/dragons/timers/elderTimer.png');
        this.load.image('timer_Hextech', 'frontend/images/dragons/timers/hextechTimer.png');
        this.load.image('timer_Chemtech', 'frontend/images/dragons/timers/chemtechTimer.png');
        this.load.image('timer_Baron', 'frontend/images/baronTimer.png');

        //Inhibitor
        this.load.svg('top', 'frontend/images/lanes/top.svg');
        this.load.svg('mid', 'frontend/images/lanes/mid.svg');
        this.load.svg('bot', 'frontend/images/lanes/bot.svg');

        //Item
        this.load.image('itemTextBg', 'frontend/backgrounds/ItemText.png');
    }

    create() {
        var div = document.getElementById('gameContainer');

        const blue_1 = this.make.sprite({ x: 39, y: 152 + 37, key: 'champCoverLeft', add: false }).createBitmapMask();
        const playerNotificationMaskSprite = this.make.sprite({ x: 78, y: 152 + 37, key: 'itemTextMaskLeft', add: false });
        playerNotificationMaskSprite.setOrigin(0, 0.5);
        var playerNotificationWidth = playerNotificationMaskSprite.width / 2;
        const blue_1_notif = playerNotificationMaskSprite.createBitmapMask();

        const blue_2 = this.make.sprite({ x: 39, y: 255 + 37, key: 'champCoverLeft', add: false }).createBitmapMask();
        const blue_2_notif = this.make.sprite({ x: 78 + playerNotificationWidth, y: 255 + 37, key: 'itemTextMaskLeft', add: false }).createBitmapMask();

        const blue_3 = this.make.sprite({ x: 39, y: 358 + 37, key: 'champCoverLeft', add: false }).createBitmapMask();
        const blue_3_notif = this.make.sprite({ x: 78 + playerNotificationWidth, y: 358 + 37, key: 'itemTextMaskLeft', add: false }).createBitmapMask();

        const blue_4 = this.make.sprite({ x: 39, y: 461 + 37, key: 'champCoverLeft', add: false }).createBitmapMask();
        const blue_4_notif = this.make.sprite({ x: 78 + playerNotificationWidth, y: 461 + 37, key: 'itemTextMaskLeft', add: false }).createBitmapMask();

        const blue_5 = this.make.sprite({ x: 39, y: 564 + 37, key: 'champCoverLeft', add: false }).createBitmapMask();
        const blue_5_notif = this.make.sprite({ x: 78 + playerNotificationWidth, y: 564 + 37, key: 'itemTextMaskLeft', add: false }).createBitmapMask();


        const red_1 = this.make.sprite({ x: 1920 - 39, y: 152 + 37, key: 'champCoverRight', add: false }).createBitmapMask();
        const red_1_notif = this.make.sprite({ x: 1920 - 78 - playerNotificationWidth, y: 152 + 37, key: 'itemTextMaskRight', add: false }).createBitmapMask();

        const red_2 = this.make.sprite({ x: 1920 - 39, y: 255 + 37, key: 'champCoverRight', add: false }).createBitmapMask();
        const red_2_notif = this.make.sprite({ x: 1920 - 78 - playerNotificationWidth, y: 255 + 37, key: 'itemTextMaskRight', add: false }).createBitmapMask();

        const red_3 = this.make.sprite({ x: 1920 - 39, y: 358 + 37, key: 'champCoverRight', add: false }).createBitmapMask();
        const red_3_notif = this.make.sprite({ x: 1920 - 78 - playerNotificationWidth, y: 358 + 37, key: 'itemTextMaskRight', add: false }).createBitmapMask();

        const red_4 = this.make.sprite({ x: 1920 - 39, y: 461 + 37, key: 'champCoverRight', add: false }).createBitmapMask();
        const red_4_notif = this.make.sprite({ x: 1920 - 78 - playerNotificationWidth, y: 461 + 37, key: 'itemTextMaskRight', add: false }).createBitmapMask();

        const red_5 = this.make.sprite({ x: 1920 - 39, y: 564 + 37, key: 'champCoverRight', add: false }).createBitmapMask();
        const red_5_notif = this.make.sprite({ x: 1920 - 78 - playerNotificationWidth, y: 564 + 37, key: 'itemTextMaskRight', add: false }).createBitmapMask();


        this.displayRegions = [
            new RegionMask(0, [blue_1, blue_1_notif]),
            new RegionMask(1, [blue_2, blue_2_notif]),
            new RegionMask(2, [blue_3, blue_3_notif]),
            new RegionMask(3, [blue_4, blue_4_notif]),
            new RegionMask(4, [blue_5, blue_5_notif]),
            new RegionMask(5, [red_1, red_1_notif]),
            new RegionMask(6, [red_2, red_2_notif]),
            new RegionMask(7, [red_3, red_3_notif]),
            new RegionMask(8, [red_4, red_4_notif]),
            new RegionMask(9, [red_5, red_5_notif]),
            new RegionMask(10, [])
        ];

        this.overlayCfg = null;

        const connect = () => {
            this.ws = new WebSocket(`${variables.useSSL ? 'wss' : 'ws'}://${variables.backendUrl}:${variables.backendPort}/${variables.backendWsLoc}`);
            this.ws.onopen = () => {
                console.log('[LB] Connection established!');
                setTimeout(() => {
                    console.log('[LB] Requesting overlay config');
                    this.ws.send('\{"requestType": "OverlayConfig","OverlayType": "Ingame"\}')
                }, 1000);
            };

            this.ws.onclose = () => {
                setTimeout(connect, 500);
                this.score?.Stop();
                this.inhib?.Stop();
                this.baronPowerPlay?.Stop();
                this.elderPowerPlay?.Stop();
                this.dragonTimer?.Stop();
                this.baronTimer?.Stop();
                this.info?.Stop();
                this.centerGraph?.Stop();
                this.centerGraph?.Stop();
                console.log('[LB] Attempt reconnect in 500ms');
            };
            this.ws.onerror = () => {
                console.log('[LB] Connection error!');
            };

            this.ws.onmessage = msg => {
                const data = JSON.parse(msg.data);
                if (data.eventType) {
                    switch (data.eventType) {
                        case 'GameHeartbeat':
                            OnNewState(data.stateData);
                            break;
                        case 'OverlayConfig':
                            if (data.type !== 1)
                                break;
                            this.UpdateConfigWhenReady(data);
                            break;
                        case 'PlayerLevelUp':
                            if (this.info.isActive && data.playerId < 5)
                                break;
                            new LevelUpVisual(this, data.playerId, data.level);
                            break;
                        case 'ObjectiveKilled':
                            this.OnObjectiveKill(data.ObjectiveName, data.TeamName);
                            break;
                        case 'BuffDespawn':
                            console.log('Legacy objective despawn: ' + JSON.stringify(data));
                            break;
                        case 'ObjectiveSpawn':
                            this.OnObjectiveSpawn(data.ObjectiveName);
                            break;
                        case 'ItemCompleted':
                            if (this.info.isActive && data.playerId < 5)
                                break;
                            new ItemVisual(this, data.itemData, data.playerId);
                            break;
                        case 'GameEnd':
                            console.log('Game Ended');
                            this.state = null;
                            this.baronPowerPlay?.Stop();
                            this.elderPowerPlay?.Stop();
                            this.dragonTimer?.Stop();
                            this.centerGraph?.Stop();
                            this.inhib?.Stop();
                            this.score?.Stop();
                            this.info?.Stop();
                            break;
                        case 'GameStart':
                            console.log('Game Start');
                            break;
                        case 'GamePause':
                            break;
                        case 'GameUnpause':
                            break;
                        case 'ForceRefresh':
                            window.location.reload();
                        //Ignore pick/ban events
                        case 'newState':
                        case 'newAction':
                        case 'heartbeat':
                        case 'champSelectStart':
                        case 'champSelectEnd':
                            break;
                        default:
                            console.log('[LB] Unknown event type: ' + JSON.stringify(data));
                            break;
                    }
                } else {
                    console.log('[LB] Unexpected packet format: ' + JSON.stringify(data));
                }
            };
        };

        const OnNewState = (data: any): void => {

            //Dont show anything if overlay has not been configured yet
            if (this.overlayCfg === null)
                return;


            var newState = new StateData(data);

            if (this.state === undefined)
                this.state = newState;

            this.inhib.UpdateValues(newState.inhibitors);
            this.score.UpdateValues(newState);
            this.baronPowerPlay.UpdateValues(newState.baron);
            this.elderPowerPlay.UpdateValues(newState.dragon);
            this.dragonTimer.UpdateValues(newState.nextDragon, newState.dragon);
            this.baronTimer.UpdateValues(newState.nextBaron, newState.baron);
            this.info.UpdateValues(newState.infoPage);
            this.centerGraph.UpdateValues(newState.goldGraph);
            this.CheckSoulPoint(newState);

            this.state = newState;
        }

        connect();
    }

    GetBlueColor = (state: StateData = this.state!): string => {
        let color = GetRGBAString(Phaser.Display.Color.IntegerToColor(variables.fallbackBlue), 1);
        if (state !== undefined && state.blueColor !== undefined && state.blueColor !== '') {
            color = state.blueColor;
        }

        return color;
    }

    GetRedColor = (state: StateData = this.state!): string => {
        let color = GetRGBAString(Phaser.Display.Color.IntegerToColor(variables.fallbackRed), 1);
        if (state !== undefined && state.redColor !== undefined && state.redColor !== '') {
            color = state.redColor;
        }

        return color;
    }

    UpdateConfig = (message: OverlayConfigEvent): void => {
        console.log('[LB] Configuring Visual Elements');

        if (this.overlayCfg === null) {
            //Init Message, create Visual Elements
            this.overlayCfg = message.config;
            this.inhib = new InhibitorVisual(this);
            this.baronPowerPlay = new PowerPlayVisual(this, 'baron', message.config.BaronPowerPlay);
            this.elderPowerPlay = new PowerPlayVisual(this, 'elder', message.config.ElderPowerPlay);
            this.dragonTimer = new ObjectiveTimerVisual(this, 'Ocean', message.config.DragonTimer, `dragonTimer`);
            this.baronTimer = new ObjectiveTimerVisual(this, 'Baron', message.config.BaronTimer, `baronTimer`);
            this.score = new ScoreboardVisual(this, message.config.Score);
            this.info = new InfoPageVisual(this, message.config.InfoPage);
            this.centerGraph = new GraphVisual(this, message.config.GoldGraph, [new GoldEntry(0, 100), new GoldEntry(40, -100)]);
        } else {
            //Update Message, update elements if needed
            this.inhib.UpdateConfig(message.config.Inhib);
            this.baronPowerPlay.UpdateConfig(message.config.BaronPowerPlay);
            this.elderPowerPlay.UpdateConfig(message.config.ElderPowerPlay);
            this.dragonTimer.UpdateConfig(message.config.DragonTimer);
            this.score.UpdateConfig(message.config.Score);
            this.info.UpdateConfig(message.config.InfoPage);
            this.centerGraph.UpdateConfig(message.config.GoldGraph);
        }

        console.log('[LB] Saving new display config');
        this.overlayCfg = message.config;
        //console.log(message.config);

    }

    UpdateConfigWhenReady(message: OverlayConfigEvent): void {
        let remotePromises: Promise<boolean>[] = [];

        //local fonts all in ip:port/Cache/Fonts
        let remoteSite = `${variables.useSSL ? 'https' : 'http'}://${variables.backendUrl}:${variables.backendPort}/cache/fonts`;

        console.log('[LB] Checking remote fonts at ' + remoteSite);

        //query remoteSite
        var xhr = new XMLHttpRequest();
        xhr.open('GET', remoteSite, true);
        xhr.send();

        xhr.onreadystatechange = () => {
            if (xhr.readyState === 4) {
                //Load new fonts
                let newGoogleFonts = message.config.GoogleFonts.filter(f => !this.loadedFonts.includes(f));
                if (newGoogleFonts.length > 0) {
                    this.load.rexWebFont({
                        google: {
                            families: newGoogleFonts
                        }
                    });

                    console.log(`[LB] Loading ${newGoogleFonts.length} new google fonts`);
                    this.loadedFonts.push(...newGoogleFonts);
                    this.load.start();

                    //Wait for loader to finish loading fonts
                    this.load.once('complete', () => {
                        console.log('[LB] Google Fonts loaded');
                        if (remotePromises.length !== 0) {
                            // Wait for remote fonts to load before updating config
                            return;
                        }
                        this.UpdateConfig(message);
                    });
                };


                if (xhr.status === 200) {
                    let responseText = xhr.responseText;

                    //Create html element to parse responseText
                    let parser = new DOMParser();
                    let doc = parser.parseFromString(responseText, 'text/html');

                    //Get all font files

                    let remoteFonts = Array.from(doc.getElementsByTagName('a')).map(a => {
                        let href = a.getAttribute('href');
                        if (href !== null) {
                            return `${remoteSite}/${href}`;
                        } else {
                            return '';
                        }
                    }).filter(a => a.endsWith('.ttf') || a.endsWith('.otf'));
                    let newRemoteFonts = remoteFonts.filter(f => !this.loadedFonts.includes(f));
                    if (newRemoteFonts.length > 0) {
                        console.log(`[LB] Loading ${newRemoteFonts.length} new local fonts`);
                        for (let font of newRemoteFonts) {
                            let fontName = font.split('/').pop()!.split('.')[0].replace(/%20/g, ' ');

                            //Add font to loaded fonts
                            this.loadedFonts.push(font);

                            //Add font to load queue
                            remotePromises.push(Utils.LoadFont(fontName, font));
                        }


                    }

                    //Wait for all fonts to load
                    Promise.all(remotePromises).then(() => {
                        console.log('[LB] All local fonts loaded');

                        remotePromises = [];

                        //Update config
                        this.UpdateConfig(message);
                    });
                }

                if (newGoogleFonts.length !== 0 || remotePromises.length !== 0) {
                    return;
                }

                //No new fonts, update config
                console.log('[LB] No new fonts, updating config');
                this.UpdateConfig(message);
            };

        };
    }


    CheckSoulPoint(state: StateData): void {
        if (!this.overlayCfg?.ObjectiveKill.SoulPointScoreboardPopUp.Enabled)
            return;
        if (state.scoreboard.BlueTeam.Dragons.length === 3 && this.state?.scoreboard.BlueTeam.Dragons.length === 2) {
            console.log('Blue Soul Point');
            new ObjectivePopUpVisual(this, this.overlayCfg!.ObjectiveKill.SoulPointScoreboardPopUp, `${state.scoreboard.BlueTeam.Dragons[2]}Soul`);
        }

        if (state.scoreboard.RedTeam.Dragons.length === 3 && this.state?.scoreboard.RedTeam.Dragons.length === 2) {
            console.log('Red Soul Point');
            new ObjectivePopUpVisual(this, this.overlayCfg!.ObjectiveKill.SoulPointScoreboardPopUp, `${state.scoreboard.RedTeam.Dragons[2]}Soul`);
        }
    }


    OnObjectiveSpawn(objectiveName: string): void {
        let cfg: ScoreboardPopUpConfig | null = null;
        switch (objectiveName) {
            case 'Baron':
                cfg = this.overlayCfg!.ObjectiveSpawn.BaronSpawnScoreboardPopUp;
                break;
            case 'Herald':
                cfg = this.overlayCfg!.ObjectiveSpawn.HeraldSpawnScoreboardPopUp;
                break;
            default:
                cfg = this.overlayCfg!.ObjectiveSpawn.DrakeSpawnScoreboardPopUp;
                break;
        }

        console.log(`${objectiveName} spawned`);
        new ObjectivePopUpVisual(this, cfg, `${objectiveName.toLowerCase()}Spawn`);
    }

    OnObjectiveKill(objectiveName: string, teamName: string): void {
        let cfg: ScoreboardPopUpConfig | null = null;
        switch (objectiveName) {
            case 'Herald':
                cfg = this.overlayCfg!.ObjectiveKill.HeraldKillScoreboardPopUp;
                break;
            case 'Baron':
                cfg = this.overlayCfg!.ObjectiveKill.BaronKillScoreboardPopUp;
                break;
            case 'Elder':
                cfg = this.overlayCfg!.ObjectiveKill.ElderKillScoreboardPopUp;
                break;
            default:
                cfg = this.overlayCfg!.ObjectiveKill.DragonKillScoreboardPopUp;
                break;
        }

        console.log(`${objectiveName} killed`);
        new ObjectivePopUpVisual(this, cfg, `${objectiveName.toLowerCase()}Kill`);
    }

}

var GetRGBAString = function (color, alpha) {
    if (alpha === undefined) {
        alpha = color.alphaGL;
    }
    return 'rgba(' + color.red + ',' + color.green + ',' + color.blue + ',' + alpha + ')';
}