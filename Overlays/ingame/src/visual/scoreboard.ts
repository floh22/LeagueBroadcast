import PlaceholderConversion from "~/PlaceholderConversion";
import IngameScene from "~/scenes/IngameScene";
import variables from "~/variables";
import ScoreboardConfig from "../data/scoreboardConfig";
import stateData from "../data/stateData";
import StateData from "../data/stateData";

export default class Scoreboard {

    background!: Phaser.GameObjects.Rectangle;
    mask: Phaser.Display.Masks.BitmapMask;
    maskImage: Phaser.GameObjects.Sprite;
    
    scene: IngameScene;
    isActive: boolean;
    isLoaded: boolean;
    scoreIsActive: boolean;
    gameTime: Phaser.GameObjects.Text;
    swordIcon: Phaser.GameObjects.Sprite;

    blueTag: Phaser.GameObjects.Text;
    blueScoreTemplates: Phaser.Geom.Circle[];
    blueWins: number = -1;
    blueKills: Phaser.GameObjects.Text;
    blueTowers: Phaser.GameObjects.Text;
    blueGold: Phaser.GameObjects.Text;
    blueGoldImage: Phaser.GameObjects.Sprite;
    blueTowerImage: Phaser.GameObjects.Sprite;
    blueDragons: Phaser.GameObjects.Sprite[];
    blueTextColor: string; 

    blueIconLoc: string = '';
    blueIconSprite!: Phaser.GameObjects.Sprite;


    redTag: Phaser.GameObjects.Text;
    redScoreTemplates: Phaser.Geom.Circle[];
    redWins: number = -1;
    redKills: Phaser.GameObjects.Text;
    redTowers: Phaser.GameObjects.Text;
    redGold: Phaser.GameObjects.Text;
    redGoldImage: Phaser.GameObjects.Sprite;
    redTowerImage: Phaser.GameObjects.Sprite;
    redDragons: Phaser.GameObjects.Sprite[];
    redTextColor: string;

    redIconLoc: string = '';
    redIconSprite!: Phaser.GameObjects.Sprite;


    drakeIconSize: number = 30;
    drakeIconOffset: number = 0;

    totalGameToWinsNeeded: Record<number, number> = {
        5 : 3,
        3 : 2,
        2 : 2,
        1 : 1
    };


    constructor(scene: IngameScene) {
        this.scene = scene;
        this.isActive = false;
        this.isLoaded = false;
        this.scoreIsActive = false;

        //Mask
        this.maskImage = scene.make.sprite({ x: 960, y: -100, key: 'scoreboardMask', add: false });
        this.mask = this.maskImage.createBitmapMask();

        //Background
        //TODO: Make color configurable, maybe make it per side configurable
        this.background = scene.add.rectangle(960, 0, 1000, 300, 0x13183f);
        this.background.mask = this.mask;

        this.gameTime = scene.add.text(940, 48, '00:00', {
            fontFamily: 'News Cycle',
            fontSize: '20px',
            color: 'rgb(255,255,255)',
        });
        this.gameTime.setAlign('center');

        this.swordIcon = scene.make.sprite({ x: 960, y: 26, key: 'sword', add: true });
        this.swordIcon.setScale(1.5);

        //#region BlueScores
        this.blueTextColor = blueRGB;

        this.blueTag = scene.add.text(570, 0, 'WWW', {
            fontFamily: 'News Cycle',
            fontSize: '45px',
            fontStyle: 'bold',
            align: 'left',
            color: this.blueTextColor,
        });
        this.blueTag.setOrigin(0,0);

        this.blueKills = scene.add.text(930, 3, '0', {
            fontFamily: 'News Cycle',
            fontSize: '60px',
            fontStyle: 'bold',
            align: 'right',
            color: this.blueTextColor,
        });
        this.blueKills.setOrigin(1,0);

        this.blueTowers = scene.add.text(705, 20, '0', {
            fontFamily: 'News Cycle',
            fontSize: '25px',
            fontStyle: 'bold',
            align: 'right',
            color: this.blueTextColor,
        });
        this.blueTowers.setOrigin(1,0);

        this.blueGold = scene.add.text(810, 20, '2.5k', {
            fontFamily: 'News Cycle',
            fontSize: '25px',
            fontStyle: 'bold',
            align: 'right',
            color: this.blueTextColor,
        });
        this.blueGold.setOrigin(1,0);

        this.blueGoldImage = scene.make.sprite({ x: 835, y: 32, key: 'fullGoldIcon', add: true });
        this.blueGoldImage.setScale(1.0);

        this.blueTowerImage = scene.make.sprite({ x: 720, y: 33, key: 'tower', add: true });
        this.blueTowerImage.setScale(1.0);

        this.blueDragons = [];

        this.blueScoreTemplates = [];
        for(var i = 0; i < 5; i++) {
            this.blueScoreTemplates.push(new Phaser.Geom.Circle(585 + i * 25, 60, 10));
        }
        //#endregion

        //#region RedScores
        this.redTextColor = redRGB;

        this.redTag = scene.add.text(1355, 0, 'WWW', {
            fontFamily: 'News Cycle',
            fontSize: '45px',
            fontStyle: 'bold',
            align: 'right',
            color: this.redTextColor,
        });
        this.redTag.setOrigin(1,0);

        this.redKills = scene.add.text(1000, 3, '0', {
            fontFamily: 'News Cycle',
            fontSize: '60px',
            fontStyle: 'bold',
            align: 'left',
            color: this.redTextColor,
        });
        this.redKills.setOrigin(0,0);

        this.redGold = scene.add.text(1115, 20, '2.5k', {
            fontFamily: 'News Cycle',
            fontSize: '25px',
            fontStyle: 'bold',
            align: 'left',
            color: this.redTextColor,
        });
        this.redGold.setOrigin(0,0);

        this.redTowers = scene.add.text(1225, 20, '0', {
            fontFamily: 'News Cycle',
            fontSize: '25px',
            fontStyle: 'bold',
            align: 'left',
            color: this.redTextColor,
        });
        this.redTowers.setOrigin(0,0);

        this.redGoldImage = scene.make.sprite({ x: 1095, y: 32, key: 'fullGoldIcon', add: true });
        this.redGoldImage.setScale(1.0);

        this.redTowerImage = scene.make.sprite({ x: 1210, y: 33, key: 'tower', add: true });
        this.redTowerImage.setScale(1.0);

        this.redDragons = [];

        this.redScoreTemplates = [];
        for(var i = 0; i <= 5; i++) {
            this.redScoreTemplates.push(new Phaser.Geom.Circle(1340 - i * 25, 60, 10));
        }
        //#endregion

        //Hide on start
        
        this.gameTime.alpha = 0;
        this.swordIcon.alpha = 0;

        this.blueTag.alpha = 0;
        this.blueGold.alpha = 0;
        this.blueKills.alpha = 0;
        this.blueTowers.alpha = 0;
        this.blueGoldImage.alpha = 0;
        this.blueTowerImage.alpha = 0;

        this.redTag.alpha = 0;
        this.redGold.alpha = 0;
        this.redKills.alpha = 0;
        this.redTowers.alpha = 0;
        this.redGoldImage.alpha = 0;
        this.redTowerImage.alpha = 0;

        this.scene.load.on(`filecomplete-image-blue_icon`, () => {
            this.blueIconSprite = this.scene.make.sprite({ x: 523, y: 37.5, key: 'blue_icon', add: true });
            this.blueIconSprite.displayWidth = 75;
            this.blueIconSprite.displayHeight = 75;

            if(!this.isLoaded) {
                let ctx = this;
                this.blueIconSprite.alpha = 0;
                this.scene.tweens.add({
                    targets: ctx.blueIconSprite,
                    props: {
                        alpha: { from: 0, to: 1, duration: 1000, ease: 'Cubic.easeOut' }
                    },
                    paused: false,
                    yoyo: false,
                    delay: 550
                });
            }
        });

        this.scene.load.on(`filecomplete-image-red_icon`, () => {
            this.redIconSprite = this.scene.make.sprite({ x: 1398, y: 37.5, key: 'red_icon', add: true });
            this.redIconSprite.displayWidth = 75;
            this.redIconSprite.displayHeight = 75;

            if(!this.isLoaded) {
                let ctx = this;
                this.redIconSprite.alpha = 0;
                this.scene.tweens.add({
                    targets: ctx.redIconSprite,
                    props: {
                        alpha: { from: 0, to: 1, duration: 1000, ease: 'Cubic.easeOut' }
                    },
                    paused: false,
                    yoyo: false,
                    delay: 550
                });
            }
        });

        this.showContent();
    }

    updateContent = (state: StateData) => {
        let scoreConfig = state.scoreboard;

        if (scoreConfig.GameTime === undefined || scoreConfig.GameTime === null || scoreConfig.GameTime == -1) {
            if (this.isActive) {
                this.hideContent();
            }
            return;
        }

        let updateScores = false;
        var timeInSec = Math.round(scoreConfig.GameTime);
        this.gameTime.text = (Math.floor(timeInSec / 60) >= 10 ? Math.floor(timeInSec / 60) : '0' + Math.floor(timeInSec / 60)) + ':' + (timeInSec % 60 >= 10 ? timeInSec % 60 : '0' + timeInSec % 60);

        //Update blue team values
        if (this.blueTextColor !== state.blueColor && state.blueColor !== undefined) {
            console.log('Updating blue color');
            this.blueGold.setColor(state.blueColor);
            this.blueKills.setColor(state.blueColor);
            this.blueTowers.setColor(state.blueColor);
            this.blueTag.setColor(state.blueColor);
            this.blueTextColor = state.blueColor;
            updateScores = true;
        }
        var hundred = Math.round((scoreConfig.BlueTeam.Gold % 1000) / 100);
        var thousand = Math.floor(scoreConfig.BlueTeam.Gold / 1000);
        if(hundred === 10) {
            thousand++;
            hundred = 0;
        }
        this.blueGold.text = thousand + '.' + hundred + 'k';
        this.blueKills.text = scoreConfig.BlueTeam.Kills + '';
        this.blueTowers.text = scoreConfig.BlueTeam.Towers + '';

        if (scoreConfig.BlueTeam.Name !== undefined)
            this.blueTag.text = scoreConfig.BlueTeam.Name;
        else
            this.blueTag.text = '';

        if (scoreConfig.BlueTeam.Icon !== undefined && scoreConfig.BlueTeam.Icon !== this.blueIconLoc) {
            this.loadIcon(scoreConfig.BlueTeam.Icon, false);
            this.blueIconLoc = scoreConfig.BlueTeam.Icon;
        }
        if(scoreConfig.BlueTeam.Icon === undefined && this.blueIconLoc !== '') {
            this.blueIconLoc = '';
            this.blueIconSprite.destroy();
        }

        this.updateDragons(scoreConfig.BlueTeam.Dragons, false);

        if(this.scoreIsActive && ( scoreConfig.BlueTeam.Score === undefined || scoreConfig.RedTeam.Score === undefined)) {
            this.scene.graphics.clear();
        }


        //Update red team values
        if (this.redTextColor !== state.redColor && state.redColor !== undefined) {
            console.log('Updating red color');
            this.redGold.setColor(state.redColor);
            this.redKills.setColor(state.redColor);
            this.redTowers.setColor(state.redColor);
            this.redTag.setColor(state.redColor);
            this.redTextColor = state.redColor;
            updateScores = true;
        }
        hundred = Math.round((scoreConfig.RedTeam.Gold % 1000) / 100);
        thousand = Math.floor(scoreConfig.RedTeam.Gold / 1000);
        if(hundred === 10) {
            thousand++;
            hundred = 0;
        }
        this.redGold.text = Math.floor(scoreConfig.RedTeam.Gold / 1000) + '.' + Math.floor((scoreConfig.RedTeam.Gold % 1000) / 100) + 'k';
        this.redKills.text = scoreConfig.RedTeam.Kills + '';
        this.redTowers.text = scoreConfig.RedTeam.Towers + '';

        if (scoreConfig.RedTeam.Name !== undefined)
            this.redTag.text = scoreConfig.RedTeam.Name;
        else
            this.redTag.text = '';

        if (scoreConfig.RedTeam.Icon !== undefined && scoreConfig.RedTeam.Icon !== this.redIconLoc) {
            this.loadIcon(scoreConfig.RedTeam.Icon, true);
            this.redIconLoc = scoreConfig.RedTeam.Icon;
        }
        if(scoreConfig.RedTeam.Icon === undefined && this.redIconLoc !== '') {
            this.redIconLoc = '';
            this.redIconSprite.destroy();
        }

        this.updateDragons(scoreConfig.RedTeam.Dragons, true);

        //Update scores for both teams

        if (scoreConfig.BlueTeam.Score !== undefined || scoreConfig.RedTeam.Score !== undefined) {
            this.updateScores(state, updateScores);
        }

        if(state.uiColor !== this.scene.state?.uiColor) {
            this.background.setFillStyle(Phaser.Display.Color.RGBStringToColor(state.uiColor).color);
        }

        if (!this.isActive) {
            this.showContent();
        }
    }

    updateDragons = (dragons: string[], side: boolean) => {
        var dragonIcons = side ? this.redDragons : this.blueDragons;
        if (dragons.length !== dragonIcons.length) {
            dragonIcons.forEach(oldIcon => {
                oldIcon.destroy();
            });
            dragonIcons = [];
            var i = 0;
            dragons.forEach(drakeName => {
                var posX = side ? 1090 + i++ * (this.drakeIconOffset + this.drakeIconSize) : 840 - i++ * (this.drakeIconOffset + this.drakeIconSize);
                var toAdd = this.scene.make.sprite({ x: posX, y: 60, key: 'dragon_' + drakeName, add: true });
                toAdd.displayHeight = this.drakeIconSize;
                toAdd.displayWidth = this.drakeIconSize;
                dragonIcons.push(toAdd);
            });
            if (side) {
                this.redDragons = dragonIcons;
            } else {
                this.blueDragons = dragonIcons;
            }
        }
    }

    updateScores = (state: stateData, forceUpdate: boolean) => {
        let conf = state.scoreboard;
        if (forceUpdate || conf.RedTeam.Score !== this.redWins || conf.BlueTeam.Score !== this.blueWins) {
            let redWins = conf.RedTeam.Score;
            let blueWins = conf.BlueTeam.Score;

            this.scene.graphics.clear();
            this.scene.graphics.setDepth(1);

            //Draw red score icons
            let color = Phaser.Display.Color.IntegerToColor(variables.redColor);
            if (state.redColor !== undefined && state.redColor !== '') {
                color = Phaser.Display.Color.RGBStringToColor(state.redColor);
            }

            this.scene.graphics.fillStyle(color.color, 1);
            this.scene.graphics.lineStyle(3, color.color, 1);

            let numIcons = this.totalGameToWinsNeeded[conf.SeriesGameCount];
            this.redScoreTemplates.forEach(template => {
                if (numIcons-- > 0) {
                    if (redWins-- > 0) {
                        this.scene.graphics.fillCircleShape(template);
                    } else {
                        this.scene.graphics.strokeCircleShape(template);
                    }
                }
            });

            //Draw blue score icons
            color = Phaser.Display.Color.IntegerToColor(variables.blueColor);
            if (state.blueColor !== undefined && state.blueColor !== '') {
                color = Phaser.Display.Color.RGBStringToColor(state.blueColor);
            }

            this.scene.graphics.fillStyle(color.color, 1);
            this.scene.graphics.lineStyle(3, color.color, 1);
            numIcons = this.totalGameToWinsNeeded[conf.SeriesGameCount];
            this.blueScoreTemplates.forEach(template => {
                if (numIcons-- > 0) {
                    if (blueWins-- > 0) {
                        this.scene.graphics.fillCircleShape(template);
                    } else {
                        this.scene.graphics.strokeCircleShape(template);
                    }
                }
            });

            //update data
            this.blueWins = conf.RedTeam.Score;
            this.redWins = conf.BlueTeam.Score;
        }
    }

    loadIcon = (iconLoc: string, team: boolean) => {
        var id = (team ? 'red' : 'blue') + '_icon';
        iconLoc = PlaceholderConversion.MakeUrlAbsolute(iconLoc.replace('Cache', '/cache').replace('\\', '/'));

        if (team) {
            if (this.redIconSprite !== undefined) {
                this.redIconSprite.destroy();
            }
        } else {
            if (this.blueIconSprite !== undefined) {
                this.blueIconSprite.destroy();
            }
        }
        if (this.scene.textures.exists(id)) {
            this.scene.textures.remove(id);
        }

        this.scene.load.image(id, iconLoc);
        this.scene.load.start();
    }

    showContent = () => {
        if (this.isActive) {
            return;
        }

        this.isActive = true;
        var ctx = this;

        if (this.blueIconSprite !== undefined)
            this.blueIconSprite.alpha = 0;

        if (this.redIconSprite !== undefined)
            this.redIconSprite.alpha = 0;

        this.redDragons.forEach(d => d.alpha = 0);
        this.blueDragons.forEach(d => d.alpha = 0);

        this.scene.graphics.alpha = 0;

        this.scene.tweens.add({
            targets: this.maskImage,
            props: {
                y: { from: -100, to: 0, duration: 550, ease: 'Circ.easeOut' }
            },
            paused: false,
            yoyo: false,
            duration: 550
        });

        this.scene.tweens.add({
            targets: [ctx.gameTime, ctx.swordIcon, ctx.blueTowers, ctx.blueGold, ctx.blueKills, ctx.redGold, ctx.redKills, ctx.redTowers, ctx.redGoldImage, ctx.redTowerImage, ctx.blueGoldImage, ctx.blueTowerImage, ctx.blueTag, ctx.redTag, ctx.redIconSprite, ctx.blueIconSprite],
            props: {
                alpha: { from: 0, to: 1, duration: 1000, ease: 'Cubic.easeOut' }
            },
            paused: false,
            yoyo: false,
            delay: 550
        });
        this.scene.tweens.add({
            targets: ctx.blueDragons,
            props: {
                alpha: { from: 0, to: 1, duration: 1000, ease: 'Cubic.easeOut' }
            },
            paused: false,
            yoyo: false,
            delay: 1500
        });
        this.scene.tweens.add({
            targets: ctx.redDragons,
            props: {
                alpha: { from: 0, to: 1, duration: 1000, ease: 'Cubic.easeOut' }
            },
            paused: false,
            yoyo: false,
            delay: 1500
        });
        
        this.scene.tweens.add({
            targets: ctx.scene.graphics,
            props: {
                alpha: { from: 0, to: 1, duration: 1000, ease: 'Cubic.easeOut' }
            },
            paused: false,
            yoyo: false,
            delay: 1500,
            onComplete: function() {
                ctx.isLoaded = true;
            }
        });

    }

    hideContent = () => {
        if (!this.isActive) {
            return;
        }

        this.isActive = false;
        var ctx = this;

        this.scene.tweens.add({
            targets: [ctx.gameTime, ctx.swordIcon, ctx.blueTowers, ctx.blueGold, ctx.blueKills, ctx.redGold, ctx.redKills, ctx.redTowers, ctx.redGoldImage, ctx.redTowerImage, ctx.blueGoldImage, ctx.blueTowerImage, ctx.blueTag, ctx.redTag, ctx.redIconSprite, ctx.blueIconSprite],
            props: {
                alpha: { from: 1, to: 0, duration: 50, ease: 'Cubic.easeOut' }
            },
            paused: false,
            yoyo: false,
            duration: 50,
            onComplete: function () {
                ctx.scene.tweens.add({
                    targets: ctx.maskImage,
                    props: {
                        y: { from: 0, to: -100, duration: 550, ease: 'Circ.easeOut' }
                    },
                    paused: false,
                    yoyo: false,
                    onComplete: function() {
                        ctx.isLoaded = false;
                    }
                });
            }
        });

        this.scene.tweens.add({
            targets: ctx.blueDragons,
            props: {
                alpha: { from: 1, to: 0, duration: 50, ease: 'Cubic.easeOut' }
            },
            paused: false,
            yoyo: false
        });
        this.scene.tweens.add({
            targets: ctx.redDragons,
            props: {
                alpha: { from: 1, to: 0, duration: 50, ease: 'Cubic.easeOut' }
            },
            paused: false,
            yoyo: false
        });
        this.scene.tweens.add({
            targets: ctx.scene.graphics,
            props: {
                alpha: { from: 1, to: 0, duration: 50, ease: 'Cubic.easeOut' }
            },
            paused: false,
            yoyo: false
        });
    }

}


var GetRGBAString = function (color, alpha) {
    if (alpha === undefined) {
        alpha = color.alphaGL;
    }
    return 'rgba(' + color.red + ',' + color.green + ',' + color.blue + ',' + alpha + ')';
}

const blueRGB = GetRGBAString(Phaser.Display.Color.IntegerToColor(variables.blueColor), 1);
const redRGB = GetRGBAString(Phaser.Display.Color.IntegerToColor(variables.redColor), 1);