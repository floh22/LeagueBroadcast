import PlaceholderConversion from "~/PlaceholderConversion";
import IngameScene from "~/scenes/IngameScene";
import variables from "~/variables";
import InfoSidePage from "./infoSidePage";
import PlayerInfoTab from "./playerInfoTab";

export default class InfoSidePageIndicator {

    scene: IngameScene;
    isActive: boolean;
    isLoaded: boolean;
    bg: Phaser.GameObjects.Rectangle;
    mask: Phaser.GameObjects.Graphics;
    title: Phaser.GameObjects.Text;
    playerTabs: PlayerTabIndicator[];

    constructor(scene: IngameScene) {
        this.scene = scene;
        this.isActive = false;
        this.isLoaded = false;
        this.bg = this.scene.add.rectangle(0, 135, 300, 600, 0x13183f);
        this.bg.setOrigin(0, 0);

        this.mask = scene.make.graphics({});
        this.mask.fillStyle(0xffffff);
        this.mask.fillRect(-300, 135, 300, 735);
        var tempMask = this.mask.createGeometryMask();

        this.title = scene.add.text(90, 150, 'Info Tab', {
            fontFamily: 'News Cycle',
            fontSize: '20px',
            color: 'rgb(255,255,255)',
        });
        this.bg.setMask(tempMask);
        this.title.setMask(tempMask);

        this.playerTabs = [];

    }

    updateContent = (config: InfoSidePage) => {
        if (config.Title === undefined || config.Title === null || config.Title == '') {
            if (this.isActive) {
                this.hideContent();
            }
            return;
        }

        this.title.text = config.Title;
        this.updatePlayerTabs(config);

        if (!this.isActive) {
            this.bg.setFillStyle(Phaser.Display.Color.RGBStringToColor(this.scene.state!.uiColor).color);
            this.showContent();
        }
    }

    updatePlayerTabs = (config: InfoSidePage) => {
        if (this.playerTabs.length === 0) {
            //init player tabs
            var i = 0;
            config.Players.forEach(pt => {
                this.playerTabs.push(new PlayerTabIndicator(pt, this.scene, i++));
            });
            return;
        }

        //update player tabs
        var i = 0;
        config.Players.forEach(pt => {
            this.playerTabs[i++].updateValues(pt);
        });
    }

    showContent = () => {
        if (this.isActive)
            return
        this.isActive = true;
        var ctx = this;

        ctx.scene.tweens.add({
            targets: ctx.mask,
            props: {
                x: { from: 0, to: 300, duration: 1000, ease: 'Cubic.easeInOut' }
            },
            paused: false,
            yoyo: false,
            onComplete: function () {
                ctx.playerTabs.forEach(pt => pt.show());
                ctx.isLoaded = true;
            }
        });
    }

    hideContent = () => {
        if (!this.isActive)
            return;
        console.log('hiding side bar');
        this.isActive = false;
        var ctx = this;
        ctx.playerTabs.forEach(pt => pt.hide());
        ctx.scene.tweens.add({
            targets: ctx.mask,
            props: {
                x: { from: 300, to: 0, duration: 1000, ease: 'Cubic.easeInOut' }
            },
            paused: false,
            yoyo: false,
            delay: 500,
            onComplete: function () {
                ctx.isLoaded = false;
            }
        });
    }
}

export class PlayerTabIndicator {
    scene: IngameScene;
    topSeparator: Phaser.GameObjects.Rectangle;
    playerName: string;
    image!: Phaser.GameObjects.Sprite;
    name: Phaser.GameObjects.Text;
    progressBarWidth: number;
    progressBarTotal: Phaser.GameObjects.Rectangle;
    progresssBarCompleted: Phaser.GameObjects.Rectangle;
    minVal: Phaser.GameObjects.Text;
    maxVal: Phaser.GameObjects.Text;
    curVal!: Phaser.GameObjects.Text;
    mainVal: Phaser.GameObjects.Text;

    row: number;

    constructor(tabInfo: PlayerInfoTab, scene: IngameScene, row: number) {
        this.scene = scene;
        this.row = row;
        var baseOffset = 185;
        var tabHeight = 55;
        this.progressBarWidth = 210;
        this.topSeparator = scene.add.rectangle(0, baseOffset + row * tabHeight, 300, 2, 0xffffff);
        this.topSeparator.setOrigin(0, 0);
        this.topSeparator.setAlpha(0.6);

        var id = `${row}_champIcon`;
        this.scene.load.on(`filecomplete-image-${id}`, () => {
            this.image = this.scene.make.sprite({ x: 4, y: baseOffset + 8 + row * tabHeight, key: id, add: true });
            this.image.setOrigin(0, 0);
            this.image.displayWidth = 42;
            this.image.displayHeight = 42;
            if (!this.scene.sidePage.isLoaded) {
                this.image.alpha = 0;
            }
        });

        this.name = scene.add.text(54, baseOffset + 6 + row * tabHeight, tabInfo.PlayerName, {
            fontFamily: 'News Cycle',
            fontSize: '15px',
            color: 'rgb(255,255,255)',
        });

        var color = Phaser.Display.Color.IntegerToColor(tabInfo.ExtraInfo[2] === "ORDER" ? variables.blueColor : variables.redColor);
        if (this.scene.state?.redColor !== undefined && this.scene.state.redColor !== '') {
            color = Phaser.Display.Color.RGBStringToColor(tabInfo.ExtraInfo[2] === "ORDER" ? this.scene.state?.blueColor : this.scene.state?.redColor);
        }

        this.progressBarTotal = this.scene.add.rectangle(52, baseOffset + 40 + row * tabHeight, this.progressBarWidth, 10, 0x000000);
        this.progressBarTotal.setOrigin(0, 0);
        this.progresssBarCompleted = this.scene.add.rectangle(52, baseOffset + 40 + row * tabHeight, this.progressBarWidth * ((tabInfo.Values.CurrentValue - tabInfo.Values.MinValue) / (tabInfo.Values.MaxValue - tabInfo.Values.MinValue)), 10, 0xffffff);
        this.progresssBarCompleted.setOrigin(0, 0);
        this.progresssBarCompleted.setFillStyle(color.color);

        this.minVal = scene.add.text(54, baseOffset + 22 + row * tabHeight, tabInfo.Values.MinValue + '', {
            fontFamily: 'News Cycle',
            fontSize: '15px',
            color: 'rgb(255,255,255)',
        });

        this.maxVal = scene.add.text(this.progressBarWidth + 50, baseOffset + 22 + row * tabHeight, tabInfo.Values.MaxValue + '', {
            fontFamily: 'News Cycle',
            fontSize: '15px',
            align: 'right',
            color: 'rgb(255,255,255)',
        });
        this.maxVal.setOrigin(1, 0);

        this.mainVal = scene.add.text(280, baseOffset + 15 + row * tabHeight, tabInfo.ExtraInfo[0], {
            fontFamily: 'News Cycle',
            fontSize: '25px',
            align: 'right',
            color: 'rgb(255,255,255)',
        });
        this.mainVal.setOrigin(0.5, 0);

        this.playerName = tabInfo.PlayerName;
        this.loadIcon(tabInfo.IconPath, id, row);

        this.mainVal.alpha = 0;
        this.minVal.alpha = 0;
        this.maxVal.alpha = 0;
        this.name.alpha = 0;
        this.progresssBarCompleted.alpha = 0;
        this.progressBarTotal.alpha = 0;
        this.topSeparator.alpha = 0;
    }

    loadIcon = (iconLoc: string, id: string, row: number) => {
        iconLoc = PlaceholderConversion.MakeUrlAbsolute(iconLoc.replace('Cache', '/cache').replace('\\', '/').replace('\'', '').replace('Wukong', 'MonkeyKing').replace(' ', ''));

        if (this.image !== undefined && this.image !== null) {
            this.image.destroy();
        }

        if (this.scene.textures.exists(id)) {
            this.scene.textures.remove(id);
        }

        this.scene.load.image(id, iconLoc);
        this.scene.load.start();

    }

    convertGold = (gold: number): string => {
        let hundred = Math.round((gold % 1000) / 100);
        let thousand = Math.floor(gold / 1000);
        if (hundred === 10) {
            thousand++;
            hundred = 0;
        }
        return Math.floor(gold / 1000) + '.' + Math.floor((gold % 1000) / 100) + 'k';
    }

    updateValues = (tabInfo: PlayerInfoTab) => {
        var newWidth = this.progressBarWidth * ((tabInfo.Values.CurrentValue - tabInfo.Values.MinValue) / (tabInfo.Values.MaxValue - tabInfo.Values.MinValue));
        if (newWidth > this.progressBarWidth)
            newWidth = this.progressBarWidth;
        if (newWidth < 0)
            newWidth = 0;

        if (this.playerName !== tabInfo.PlayerName) {
            this.loadIcon(tabInfo.IconPath, `${this.row}_champIcon`, this.row)
            this.playerName = tabInfo.PlayerName;
            this.progresssBarCompleted.width = newWidth;

            var color = Phaser.Display.Color.IntegerToColor(tabInfo.ExtraInfo[2] === "ORDER" ? variables.blueColor : variables.redColor);
            if (this.scene.state?.redColor !== undefined && this.scene.state.redColor !== '') {
                color = Phaser.Display.Color.RGBStringToColor(tabInfo.ExtraInfo[2] === "ORDER" ? this.scene.state?.blueColor : this.scene.state?.redColor);
            }

            this.progresssBarCompleted.setFillStyle(color.color);
        } else {
            var ctx = this;
            ctx.scene.tweens.add({
                targets: ctx.progresssBarCompleted,
                props: {
                    width: { value: newWidth, duration: 500, ease: 'Cubic.easeInOut' }
                },
                paused: false,
                yoyo: false
            });

        }

        let min = "";
        let max = "";
        let cur = "";

        switch (tabInfo.ExtraInfo[1]) {
            case "gold":
                //min = Math.trunc(tabInfo.Values.MinValue) + '';
                min = '';
                max = Math.trunc(tabInfo.Values.CurrentValue) + '';
                let val = Math.trunc(tabInfo.Values.CurrentValue);
                cur = this.convertGold(val);
                if (cur.includes('NaN')) {
                    console.log(`${val} -> ${cur}`);
                }
                this.setBarWidth(180);
                this.mainVal.x = 266;
                break;
            case 'cspm':
                min = tabInfo.Values.MinValue.toFixed(1);
                max = Math.trunc((tabInfo.ExtraInfo[0] as unknown as number)) + '';
                cur = (tabInfo.Values.CurrentValue).toFixed(1);
                this.mainVal.x = 280;
                break;
            default:
                min = tabInfo.Values.MinValue + '';
                max = tabInfo.Values.MaxValue + ''
                cur = tabInfo.ExtraInfo[0];
                this.setBarWidth();
                this.mainVal.x = 280;
                break;
        }

        this.name.text = tabInfo.PlayerName;
        this.minVal.text = min;
        this.maxVal.text = max;
        this.mainVal.text = cur;

    }

    setBarWidth = (width: number = 210) => {
        if (width === this.progressBarWidth)
            return;
        this.progressBarWidth = width;

        this.progressBarTotal.width = this.progressBarWidth;
        this.progresssBarCompleted.width = 0;
        this.maxVal.x = this.progressBarWidth + 50;
    }

    show = () => {

        var ctx = this;
        ctx.scene.tweens.add({
            targets: [ctx.topSeparator, ctx.name, ctx.image, ctx.progressBarTotal, ctx.progresssBarCompleted, ctx.minVal, ctx.mainVal, ctx.maxVal],
            props: {
                alpha: { from: 0, to: 1, duration: 500, ease: 'Cubic.easeInOut' }
            },
            paused: false,
            yoyo: false,
            duration: 500
        });
    }

    hide = () => {
        var ctx = this;
        ctx.scene.tweens.add({
            targets: [ctx.topSeparator, ctx.name, ctx.image, ctx.progressBarTotal, ctx.progresssBarCompleted, ctx.minVal, ctx.mainVal, ctx.maxVal],
            props: {
                alpha: { from: 1, to: 0, duration: 500, ease: 'Cubic.easeInOut' }
            },
            paused: false,
            yoyo: false,
            duration: 500
        });
    }
}