import 'phaser';
import variables from '~/variables';
import ObjectiveIndicator from '~/visual/objectiveIndicator';
import StateData from '~/data/stateData';
import GraphPopUp from '~/visual/graphPopUp';
import Phaser from 'phaser';
import GoldEntry from '~/data/goldEntry';
import Scoreboard from '~/visual/scoreboard';
import InfoSidePageIndicator from '~/visual/infoSidePageIndicator';
import InhibitorIndicator from '~/visual/inhibitorIndicator';
import WindowUtils from '~/convert/windowUtils';
import OverlayConfigEvent, { OverlayConfig } from '~/data/config/overlayConfig';
import ScoreboardConfig from '~/data/scoreboardConfig';
import ItemVisual from '~/visual/ItemVisual';
import { VisualElement } from '~/visual/VisualElement';
import { Dictionary } from '~/util/Dictionary';
import LevelUpVisual from '~/visual/LevelUpVisual';
import RegionMask from '~/data/RegionMask';

export default class IngameScene extends Phaser.Scene
{
    ws!: WebSocket;
    players: RegionMask[];
    baronIndicator!: ObjectiveIndicator;
    elderIndicator!: ObjectiveIndicator;
    goldGraph!: GraphPopUp;
    scoreboard!: Scoreboard;
    sidePage!: InfoSidePageIndicator;
    inhib!: InhibitorIndicator;

    state!: StateData | null;
    overlayCfg!: OverlayConfig | null;

    graphics!: Phaser.GameObjects.Graphics;

    currentVisualElements: VisualElement[] = [];
    visualIDs: number = 0;

    static Instance: IngameScene;
    

    constructor ()
    {
        super('ingameOverlay');
        IngameScene.Instance = this;
        this.players = [];
    }

    preload ()
    {
        var config = {
            google: {
                families: ['Droid Sans', 'News Cycle', 'News Cycle:bold']
            }
        };

        variables.backendUrl = WindowUtils.GetQueryVariable('backend');

        //@ts-ignore
        this.graphics = this.add.graphics();

        this.load.rexWebFont(config);
        this.load.script('chartjs', 'https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.9.4/Chart.min.js');
        //this.load.script('chartjs', 'https://cdnjs.cloudflare.com/ajax/libs/Chart.js/3.0.0-rc/chart.min.js');

        //Masks
        this.load.image('champCoverLeft', 'frontend/images/ChampCoverLeft.png');
        this.load.image('champCoverRight', 'frontend/images/ChampCoverRight.png');

        //Objective Timers
        this.load.image('baronIcon', 'frontend/backgrounds/BaronIcon.png');
        this.load.image('objectiveBg', 'frontend/backgrounds/ObjectiveBG.png');
        this.load.image('objectiveMask', 'frontend/backgrounds/ObjectiveMask.png');
        this.load.image('dragonIcon', 'frontend/backgrounds/DragonIcon.png');
        this.load.image('objectiveBgLeft', 'frontend/backgrounds/ObjectiveBGLeft.png');

        //Scoreboard
        this.load.image('goldIcon', 'frontend/images/goldicon.png');
        this.load.image('cdr', 'frontend/images/cdr.png');
        this.load.image('scoreboardMask', 'frontend/backgrounds/ScoreboardMask.png');
        this.load.image('fullGoldIcon', 'frontend/images/fullGoldIcon.png');
        this.load.image('tower', 'frontend/images/tower.png');
        this.load.image('sword', 'frontend/images/sword.png');

        //Center Graph
        this.load.image('centerCover', 'frontend/backgrounds/CenterCover.png');


        //Dragons
        this.load.image('dragon_Fire', 'frontend/images/dragons/fireLarge.png');
        this.load.image('dragon_Earth', 'frontend/images/dragons/mountainLarge.png');
        this.load.image('dragon_Air', 'frontend/images/dragons/cloudLarge.png');
        this.load.image('dragon_Water', 'frontend/images/dragons/oceanLarge.png');
        this.load.image('dragon_Elder', 'frontend/images/dragons/elderLarge.png');

        //Inhibitor
        this.load.svg('top', 'frontend/images/top.svg');
        this.load.svg('mid', 'frontend/images/mid.svg');
        this.load.svg('bot', 'frontend/images/bot.svg');

        //Item
        this.load.image('itemTextBg', 'frontend/backgrounds/ItemText.png');
        this.load.image('itemTextMask', 'frontend/masks/ItemText.png');
    }

    create ()
    {
        var div = document.getElementById('gameContainer');

        const blue_1 = this.make.sprite({x: 39 , y: 152 + 37, key: 'champCoverLeft', add: false}).createBitmapMask();
        const playerNotificationMaskSprite = this.make.sprite({x: 78, y: 152 + 37, key: 'itemTextMask', add: false});
        playerNotificationMaskSprite.setOrigin(0,0.5);
        var playerNotificationWidth = playerNotificationMaskSprite.width / 2;
        const blue_1_notif = playerNotificationMaskSprite.createBitmapMask();

        const blue_2 = this.make.sprite({x: 39 , y: 255 + 37, key: 'champCoverLeft', add: false}).createBitmapMask();
        const blue_2_notif = this.make.sprite({x: 78 + playerNotificationWidth, y: 255 + 37, key: 'itemTextMask', add: false}).createBitmapMask();

        const blue_3 = this.make.sprite({x: 39 , y: 358 + 37, key: 'champCoverLeft', add: false}).createBitmapMask();
        const blue_3_notif = this.make.sprite({x: 78 + playerNotificationWidth, y: 358 + 37, key: 'itemTextMask', add: false}).createBitmapMask();

        const blue_4 = this.make.sprite({x: 39 , y: 461 + 37, key: 'champCoverLeft', add: false}).createBitmapMask();
        const blue_4_notif = this.make.sprite({x: 78 + playerNotificationWidth, y: 461 + 37, key: 'itemTextMask', add: false}).createBitmapMask();

        const blue_5 = this.make.sprite({x: 39 , y: 564 + 37, key: 'champCoverLeft', add: false}).createBitmapMask();
        const blue_5_notif = this.make.sprite({x: 78 + playerNotificationWidth, y: 564 + 37, key: 'itemTextMask', add: false}).createBitmapMask();


        const red_1 = this.make.sprite({x: 1920 - 39, y: 152 + 37, key:'champCoverRight', add: false}).createBitmapMask();
        const red_1_notif = this.make.sprite({x: 1920 - 78 - playerNotificationWidth, y: 152 + 37, key: 'itemTextMask', add: false}).createBitmapMask();

        const red_2 = this.make.sprite({x: 1920 - 39, y: 255 + 37, key:'champCoverRight', add: false}).createBitmapMask();
        const red_2_notif = this.make.sprite({x: 1920 - 78 - playerNotificationWidth, y: 255 + 37, key: 'itemTextMask', add: false}).createBitmapMask();

        const red_3 = this.make.sprite({x: 1920 - 39, y: 358 + 37, key:'champCoverRight', add: false}).createBitmapMask();
        const red_3_notif = this.make.sprite({x: 1920 - 78 - playerNotificationWidth, y: 358 + 37, key: 'itemTextMask', add: false}).createBitmapMask();

        const red_4 = this.make.sprite({x: 1920 - 39, y: 461 + 37, key:'champCoverRight', add: false}).createBitmapMask();
        const red_4_notif = this.make.sprite({x: 1920 - 78 - playerNotificationWidth, y: 461 + 37, key: 'itemTextMask', add: false}).createBitmapMask();

        const red_5 = this.make.sprite({x: 1920 - 39, y: 564 + 37, key:'champCoverRight', add: false}).createBitmapMask();
        const red_5_notif = this.make.sprite({x: 1920 - 78 - playerNotificationWidth, y: 564 + 37, key: 'itemTextMask', add: false}).createBitmapMask();


        this.players = [
            new RegionMask(0, [blue_1, blue_1_notif]), 
            new RegionMask(1, [blue_2, blue_2_notif]), 
            new RegionMask(2, [blue_3, blue_3_notif]), 
            new RegionMask(3, [blue_4, blue_4_notif]), 
            new RegionMask(4, [blue_5, blue_5_notif]), 
            new RegionMask(5, [red_1, red_1_notif]), 
            new RegionMask(6, [red_2, red_2_notif]), 
            new RegionMask(7, [red_3, red_3_notif]), 
            new RegionMask(8, [red_4, red_4_notif]), 
            new RegionMask(9, [red_5, red_5_notif])];

        this.baronIndicator = new ObjectiveIndicator('baron', 1800, 55, this, 'baronIcon', 'objectiveBg', '00:00', 0, true );
        this.elderIndicator = new ObjectiveIndicator('elder', 120, 55, this, 'dragonIcon', 'objectiveBgLeft', '00:00', 0, false);
        //Color calc breaks with no data so init with dummy data
        this.goldGraph = new GraphPopUp(this, [new GoldEntry(0,100), new GoldEntry(1,-100)]);

        //this.goldGraph.Enable();

        this.scoreboard = new Scoreboard(this);
        this.sidePage = new InfoSidePageIndicator(this);

        //this.sidePage.showContent();

        this.inhib = new InhibitorIndicator(this);

        //this.inhib.show();

        const connect = () => {
            this.ws = new WebSocket(`${variables.useSSL? 'wss' : 'ws'}://${variables.backendUrl}:${variables.backendPort}/${variables.backendWsLoc}`);
            this.ws.onopen = () => {
                console.log('[LB] Connection established!');
                setTimeout(() => {
                    console.log('[LB] Requesting overlay config');
                    this.ws.send('\{"requestType": "OverlayConfig","OverlayType": "Ingame"\}')
                }, 1000);
            };
    
            this.ws.onclose = () => {
                setTimeout(connect, 500);
                //this.scoreboard.hideContent();
                //this.sidePage.hideContent();
                //this.goldGraph.Disable();
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
                            this.UpdateConfig(data);
                            break;
                        case 'PlayerLevelUp':
                            if(this.sidePage.isActive && data.playerId < 5)
                                break;
                            new LevelUpVisual(this, data.playerId, data.level);
                            break;
                        case 'ObjectiveKilled':
                            console.log('Legacy objective kill: ' + JSON.stringify(data));
                            showObjective(data.objective);
                            break;
                        case 'BuffDespawn':
                            console.log('Legacy objective despawn: ' + JSON.stringify(data)); 
                            hideObjective(data.objective);
                            break;
                        case 'ItemCompleted':
                            if(this.sidePage.isActive && data.playerId < 5)
                                break;
                            new ItemVisual(this, data.itemData, data.playerId);
                            break;
                        case 'GameEnd':
                            console.log('Game Ended');
                            this.state = null;
                            this.baronIndicator.hideContent();
                            this.elderIndicator.hideContent();
                            this.sidePage.hideContent();
                            this.goldGraph.Disable();
                            this.inhib.hide();
                            break;
                        case 'GameStart':
                            console.log('Game Start');
                            break;
                        case 'GamePause':
                            break;
                        case 'GameUnpause':
                            break;
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

        const showObjective = (objective: string) => {
            console.log(`${objective} taken`);
            switch (objective) {
                case 'baron':
                    this.baronIndicator.updateContent(this.state?.baron!);
                    this.baronIndicator.showContent();
                    break;
                case 'elder':
                    this.elderIndicator.updateContent(this.state?.dragon!);
                    this.elderIndicator.showContent();
                    break;
                default:
                    break;
            }
        };

        const hideObjective = (objective: string) => {
            switch (objective) {
                case 'baron':
                    this.baronIndicator.hideContent();
                    break;
                case 'elder':
                    this.elderIndicator.hideContent();
                    break;
                default:
                    break;
            }
        }

        const OnNewState = (data: any): void => {
            var newState = new StateData(data);

            if(this.state === undefined)
                this.state = newState;

            //console.log(newState);
            this.scoreboard.updateContent(newState);

            this.state = newState;

            this.baronIndicator.updateContent(newState.baron);
            this.elderIndicator.updateContent(newState.dragon);

            this.goldGraph.Update(newState.goldGraph);
            this.sidePage.updateContent(newState.infoPage);
            this.inhib.updateContent(newState.inhibitors);
        }

        connect();
    }

    GetBlueColor = (state: StateData = this.state!): string => {
        let color = GetRGBAString(Phaser.Display.Color.IntegerToColor(variables.blueColor), 1);
        if(state !== undefined && state.blueColor !== undefined && state.blueColor !== '') {
            color = state.blueColor;
        }

        return color;
    }
    
    GetRedColor = (state: StateData = this.state!): string => {
        let color = GetRGBAString(Phaser.Display.Color.IntegerToColor(variables.redColor), 1);
        if(state !== undefined && state.redColor !== undefined && state.redColor !== '') {
            color = state.redColor;
        }

        return color;
    }

    UpdateConfig = (message: OverlayConfigEvent): void => {
        console.log('Configuring overlay');
        this.overlayCfg = message.config;
        console.log(message.config);
    }
    
}

var GetRGBAString = function (color, alpha) {
    if (alpha === undefined) {
        alpha = color.alphaGL;
    }
    return 'rgba(' + color.red + ',' + color.green + ',' + color.blue + ',' + alpha + ')';
}