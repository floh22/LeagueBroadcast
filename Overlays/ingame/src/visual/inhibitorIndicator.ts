import { InhibitorDisplayConfig } from "~/data/config/overlayConfig";
import IngameScene from "~/scenes/IngameScene";
import variables from "~/variables";
import { InhibitorInfo } from "../data/inhibitor";

export default class InhibitorIndicator {
    scene: IngameScene;
    isActive: boolean;
    isLoaded: boolean;
    bg: Phaser.GameObjects.Rectangle;
    bgImage!: Phaser.GameObjects.Sprite;
    mask: Phaser.GameObjects.Graphics;

    blueOffset: number = 825;

    blueTopIndicator: Phaser.GameObjects.Sprite;
    blueTopTime: Phaser.GameObjects.Text;
    blueMidIndicator: Phaser.GameObjects.Sprite;
    blueMidTime: Phaser.GameObjects.Text;
    blueBotIndicator: Phaser.GameObjects.Sprite;
    blueBotTime: Phaser.GameObjects.Text;

    redOffset: number = 880;

    redTopIndicator: Phaser.GameObjects.Sprite;
    redTopTime: Phaser.GameObjects.Text;
    redMidIndicator: Phaser.GameObjects.Sprite;
    redMidTime: Phaser.GameObjects.Text;
    redBotIndicator: Phaser.GameObjects.Sprite;
    redBotTime: Phaser.GameObjects.Text;

    constructor(scene: IngameScene) {
        this.scene = scene;
        this.isActive = false;
        this.isLoaded = false;
        this.bg = this.scene.add.rectangle(0,845,306,118, 0x13183f);
        this.bg.setOrigin(0,0);

        this.mask = scene.make.graphics({});
        this.mask.fillStyle(0xffffff);
        this.mask.fillRect(0, 845, 306, 118);
        var tempMask = this.mask.createGeometryMask();

        var color = Phaser.Display.Color.IntegerToColor(variables.blueColor);
        if (this.scene.state?.blueColor !== undefined && this.scene.state.blueColor !== '') {
            color = Phaser.Display.Color.RGBStringToColor(this.scene.state?.blueColor);
        }

        this.blueTopIndicator = scene.make.sprite({ x: 40, y: this.blueOffset, key: 'top', add: true });
        this.blueTopIndicator.setSize(40,40);
        this.blueTopIndicator.setOrigin(0,0);

        this.blueMidIndicator = scene.make.sprite({ x: 140, y: this.blueOffset, key: 'mid', add: true });
        this.blueMidIndicator.setSize(40,40);
        this.blueMidIndicator.setOrigin(0,0);

        this.blueBotIndicator = scene.make.sprite({ x: 240, y: this.blueOffset, key: 'bot', add: true });
        this.blueBotIndicator.setSize(40,40);
        this.blueBotIndicator.setOrigin(0,0);

        this.blueTopIndicator.tint = color.color;
        this.blueMidIndicator.tint = color.color;
        this.blueBotIndicator.tint = color.color;

        this.blueTopTime = scene.add.text(60, this.blueOffset + 43, '03:00', {
            fontFamily: 'News Cycle',
            fontSize: '18px',
            align: 'right',
            color: 'rgb(255,255,255)',
        });
        this.blueTopTime.setOrigin(1,0);

        this.blueMidTime = scene.add.text(160, this.blueOffset + 43, '-  ', {
            fontFamily: 'News Cycle',
            fontSize: '18px',
            align: 'right',
            color: 'rgb(255,255,255)',
        });
        this.blueMidTime.setOrigin(1,0);

        this.blueBotTime = scene.add.text(260, this.blueOffset + 43, '-  ', {
            fontFamily: 'News Cycle',
            fontSize: '18px',
            align: 'right',
            color: 'rgb(255,255,255)',
        });
        this.blueBotTime.setOrigin(1,0);


        color = Phaser.Display.Color.IntegerToColor(variables.redColor);
        if (this.scene.state?.redColor !== undefined && this.scene.state.redColor !== '') {
            color = Phaser.Display.Color.RGBStringToColor(this.scene.state.redColor);
        }

        this.redTopIndicator = scene.make.sprite({ x: 40, y: this.redOffset, key: 'top', add: true });
        this.redTopIndicator.setSize(40,40);
        this.redTopIndicator.setOrigin(0,0);

        this.redMidIndicator = scene.make.sprite({ x: 140, y: this.redOffset, key: 'mid', add: true });
        this.redMidIndicator.setSize(40,40);
        this.redMidIndicator.setOrigin(0,0);

        this.redBotIndicator = scene.make.sprite({ x: 240, y: this.redOffset, key: 'bot', add: true });
        this.redBotIndicator.setSize(40,40);
        this.redBotIndicator.setOrigin(0,0);

        this.redTopTime = scene.add.text(60, this.redOffset + 43, '03:00', {
            fontFamily: 'News Cycle',
            fontSize: '18px',
            align: 'right',
            color: 'rgb(255,255,255)',
        });
        this.redTopTime.setOrigin(1,0);

        this.redMidTime = scene.add.text(160, this.redOffset + 43, '-  ', {
            fontFamily: 'News Cycle',
            fontSize: '18px',
            align: 'right',
            color: 'rgb(255,255,255)',
        });
        this.redMidTime.setOrigin(1,0);

        this.redBotTime = scene.add.text(260, this.redOffset + 43, '-  ', {
            fontFamily: 'News Cycle',
            fontSize: '18px',
            align: 'right',
            color: 'rgb(255,255,255)',
        });
        this.redBotTime.setOrigin(1,0);

        this.redTopIndicator.tint = color.color;
        this.redMidIndicator.tint = color.color;
        this.redBotIndicator.tint = color.color;

        this.getVisibleComponents().forEach(c => {
            if (c === null || c === undefined)
                return;
            c.y += this.bg.height;
            c.mask = tempMask;
        });

        this.scene.load.on(`filecomplete-image-inhibitorBg`, () => {
            this.bgImage.setTexture('inhibitorBg');
        });
 
    }

    updateContent = (info: InhibitorInfo) => {
        if (info === undefined || info === null || info.Inhibitors.every(i => i.timeLeft <= 0)) {
            if (this.isActive) {
                this.isActive = false;
                this.hide();
            }
            return;
        }

        this.blueTopTime.text = this.toTimeString(info.Inhibitors[0].timeLeft);
        this.blueMidTime.text = this.toTimeString(info.Inhibitors[1].timeLeft);
        this.blueBotTime.text = this.toTimeString(info.Inhibitors[2].timeLeft);
        this.redTopTime.text = this.toTimeString(info.Inhibitors[3].timeLeft);
        this.redMidTime.text = this.toTimeString(info.Inhibitors[4].timeLeft);
        this.redBotTime.text = this.toTimeString(info.Inhibitors[5].timeLeft);

        if(!this.isActive) {
            this.isActive = true;
            this.bg.setFillStyle(Phaser.Display.Color.RGBStringToColor(this.scene.state!.uiColor).color);
            this.show();
        }
    }

    configureContent = (cfg: InhibitorDisplayConfig) => {
        //Update Icon Colors
        var color = Phaser.Display.Color.IntegerToColor(variables.blueColor);
        if (this.scene.state?.blueColor !== undefined && this.scene.state.blueColor !== '') {
            color = Phaser.Display.Color.RGBStringToColor(this.scene.state?.blueColor);
        }

        this.blueTopIndicator.tint = color.color;
        this.blueMidIndicator.tint = color.color;
        this.blueBotIndicator.tint = color.color;

        color = Phaser.Display.Color.IntegerToColor(variables.redColor);
        if (this.scene.state?.redColor !== undefined && this.scene.state.redColor !== '') {
            color = Phaser.Display.Color.RGBStringToColor(this.scene.state.redColor);
        }

        this.redTopIndicator.tint = color.color;
        this.redMidIndicator.tint = color.color;
        this.redBotIndicator.tint = color.color;

        //Update Mask
        this.mask.clear();
        this.mask.fillStyle(0xffffff);
        this.mask.fillRect(cfg.Location.X, cfg.Location.Y, cfg.Size.X, cfg.Size.Y);
        var tempMask = this.mask.createGeometryMask();

        //Update Background
        this.bg.setPosition(cfg.Location.X, cfg.Location.Y);
        this.bg.setSize(cfg.Size.X, cfg.Size.Y);
        if(cfg.UseImage) {
            this.bg.setAlpha(0);
            this.bgImage = this.scene.make.sprite({x: cfg.Location.X , y: cfg.Location.Y, key: 'inhibitorBg', add: true});
            this.scene.load.image('inhibitorBg', 'frontend/backgrounds/Inhibitor.png');
        } else {
            if(this.bgImage !== undefined || this.bgImage !== null) {
                this.bgImage.destroy();
            }
            this.bg.setFillStyle(Phaser.Display.Color.RGBStringToColor(cfg.Color).color, 1);
        }
    }


    toTimeString = (time: number): string => {
        if(time <= 0) {
            return '-  ';
        }

        var timeInSec = Math.round(time);
        return (Math.floor(timeInSec / 60) >= 10 ? Math.floor(timeInSec / 60) : '0' + Math.floor(timeInSec / 60)) + ':' + (timeInSec % 60 >= 10 ? timeInSec % 60 : '0' + timeInSec % 60);
    }

    str_pad_left = (string,pad,length):string => {
        return (new Array(length+1).join(pad)+string).slice(-length);
    }

    getVisibleComponents = (): any[] => {
        return [this.bg, this.bgImage, this.blueTopIndicator, this.blueTopTime, this.blueMidIndicator, this.blueMidTime, this.blueBotIndicator, this.blueBotTime, this.redTopIndicator, this.redTopTime, this.redMidIndicator, this.redMidTime, this.redBotIndicator, this.redBotTime];
    }

    show = () => {
        var ctx = this;

        ctx.scene.tweens.add({
            targets: ctx.getVisibleComponents(),
            props: {
                y: { value: '-=' + ctx.bg.height, duration: 500, ease: 'Cubic.easeOut' }
            },
            paused: false,
            yoyo: false,
            duration: 500
        });
    }

    hide = () => {
        var ctx = this;
        ctx.scene.tweens.add({
            targets: ctx.getVisibleComponents(),
            props: {
                y: { value: '+=' + ctx.bg.height, duration: 500, ease: 'Cubic.easeIn' }
            },
            paused: false,
            yoyo: false,
            duration: 500
        });
    }
}