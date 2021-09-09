import { InhibitorDisplayConfig } from "~/data/config/overlayConfig";
import { InhibitorInfo } from "~/data/inhibitor";
import IngameScene from "~/scenes/IngameScene";
import Vector2 from "~/util/Vector2";
import variables from "~/variables";
import { VisualElement } from "./VisualElement";

export default class InhibitorVisual extends VisualElement {

    bg!: Phaser.GameObjects.Rectangle | null;
    bgImage!: Phaser.GameObjects.Sprite | null;
    bgVideo!: Phaser.GameObjects.Video | null;
    mask: Phaser.Display.Masks.GeometryMask;
    maskG: Phaser.GameObjects.Graphics;

    blueTopIndicator: Phaser.GameObjects.Sprite;
    blueTopTime: Phaser.GameObjects.Text;
    blueMidIndicator: Phaser.GameObjects.Sprite;
    blueMidTime: Phaser.GameObjects.Text;
    blueBotIndicator: Phaser.GameObjects.Sprite;
    blueBotTime: Phaser.GameObjects.Text;

    redTopIndicator: Phaser.GameObjects.Sprite;
    redTopTime: Phaser.GameObjects.Text;
    redMidIndicator: Phaser.GameObjects.Sprite;
    redMidTime: Phaser.GameObjects.Text;
    redBotIndicator: Phaser.GameObjects.Sprite;
    redBotTime: Phaser.GameObjects.Text;

    Config = this.scene.overlayCfg!.Inhib;

    constructor(scene: IngameScene) {
        super(scene, scene.overlayCfg!.Inhib.Position, 'inhibitorIndicator');

        this.maskG = scene.make.graphics({});
        this.maskG.fillStyle(0xffffff);
        this.maskG.fillRect(this.Config!.Position.X, this.Config!.Position.Y, this.Config!.Size.X, this.Config!.Size.Y);
        this.mask = this.maskG.createGeometryMask();

        var color = Phaser.Display.Color.IntegerToColor(variables.fallbackBlue);
        if (this.scene.state?.blueColor !== undefined && this.scene.state.blueColor !== '') {
            if (this.Config.UseTeamColors) {
                color = Phaser.Display.Color.RGBStringToColor(this.scene.state.blueColor);
            } else {
                color = Phaser.Display.Color.RGBStringToColor(this.Config!.BlueTeam.Color);
            }
        }

        const TopIndex = this.Config!.LaneOrder.findIndex(l => l === "Top" || l === "top");
        const MidIndex = this.Config!.LaneOrder.findIndex(l => l === "Mid" || l === "mid");
        const BotIndex = this.Config!.LaneOrder.findIndex(l => l === "Bot" || l === "bot");

        let FontConfig = {
            fontFamily: this.Config!.Font.Name,
            fontSize: this.Config!.Font.Size,
            align: this.Config!.Font.Align,
            color: this.Config!.Font.Color,
            fontStyle: this.Config!.Font.Style
        };

        this.blueTopIndicator = scene.make.sprite({ x: this.position.X + TopIndex * this.Config!.BlueTeam.LaneOffset.X + this.Config!.BlueTeam.Position.X + this.Config!.BlueTeam.IconOffset.X, y: this.position.Y + TopIndex * this.Config!.BlueTeam.LaneOffset.Y + this.Config!.BlueTeam.Position.Y + this.Config!.BlueTeam.IconOffset.Y, key: 'top', add: true });
        this.blueTopIndicator.setSize(this.Config!.IconSize, this.Config!.IconSize);
        this.blueTopIndicator.setOrigin(0.5, 0.5);
        this.blueTopIndicator.setDepth(1);

        this.blueMidIndicator = scene.make.sprite({ x: this.position.X + MidIndex * this.Config!.BlueTeam.LaneOffset.X + this.Config!.BlueTeam.Position.X + this.Config!.BlueTeam.IconOffset.X, y: this.position.Y + MidIndex * this.Config!.BlueTeam.LaneOffset.Y + this.Config!.BlueTeam.Position.Y + this.Config!.BlueTeam.IconOffset.Y, key: 'mid', add: true });
        this.blueMidIndicator.setSize(this.Config!.IconSize, this.Config!.IconSize);
        this.blueMidIndicator.setOrigin(0.5, 0.5);
        this.blueMidIndicator.setDepth(1);

        this.blueBotIndicator = scene.make.sprite({ x: this.position.X + BotIndex * this.Config!.BlueTeam.LaneOffset.X + this.Config!.BlueTeam.Position.X + this.Config!.BlueTeam.IconOffset.X, y: this.position.Y + BotIndex * this.Config!.BlueTeam.LaneOffset.Y + this.Config!.BlueTeam.Position.Y + this.Config!.BlueTeam.IconOffset.Y, key: 'bot', add: true });
        this.blueBotIndicator.setSize(this.Config!.IconSize, this.Config!.IconSize);
        this.blueBotIndicator.setOrigin(0.5, 0.5);
        this.blueBotIndicator.setDepth(1);

        this.blueTopIndicator.tint = color.color;
        this.blueMidIndicator.tint = color.color;
        this.blueBotIndicator.tint = color.color;

        this.blueTopTime = scene.add.text(this.position.X + (TopIndex * this.Config!.BlueTeam.LaneOffset.X) + this.Config!.BlueTeam.Position.X, this.position.Y + (TopIndex * this.Config!.BlueTeam.LaneOffset.Y) + this.Config!.BlueTeam.Position.Y, '-  ', FontConfig);
        this.blueTopTime.setOrigin(1, 0);
        this.blueTopTime.setDepth(1);

        this.blueMidTime = scene.add.text(this.position.X + (MidIndex * this.Config!.BlueTeam.LaneOffset.X) + this.Config!.BlueTeam.Position.X, this.position.Y + (MidIndex * this.Config!.BlueTeam.LaneOffset.Y) + this.Config!.BlueTeam.Position.Y, '-  ', FontConfig);
        this.blueMidTime.setOrigin(1, 0);
        this.blueMidTime.setDepth(1);

        this.blueBotTime = scene.add.text(this.position.X + (BotIndex * this.Config!.BlueTeam.LaneOffset.X) + this.Config!.BlueTeam.Position.X, this.position.Y + (BotIndex * this.Config!.BlueTeam.LaneOffset.Y) + this.Config!.BlueTeam.Position.Y, '-  ', FontConfig);
        this.blueBotTime.setOrigin(1, 0);
        this.blueBotTime.setDepth(1);


        color = Phaser.Display.Color.IntegerToColor(variables.fallbackRed);
        if (this.scene.state?.redColor !== undefined && this.scene.state.redColor !== '') {
            if (this.Config.UseTeamColors) {
                color = Phaser.Display.Color.RGBStringToColor(this.scene.state.redColor);
            } else {
                color = Phaser.Display.Color.RGBStringToColor(this.Config!.RedTeam.Color);
            }
        }

        this.redTopIndicator = scene.make.sprite({ x: this.position.X + TopIndex * this.Config!.RedTeam.LaneOffset.X + this.Config!.RedTeam.Position.X + this.Config!.RedTeam.IconOffset.X, y: this.position.Y + TopIndex * this.Config!.RedTeam.LaneOffset.Y + this.Config!.RedTeam.Position.Y + this.Config!.RedTeam.IconOffset.Y, key: 'top', add: true });
        this.redTopIndicator.setSize(this.Config!.IconSize, this.Config!.IconSize);
        this.redTopIndicator.setOrigin(0.5, 0.5);
        this.redTopIndicator.setDepth(1);

        this.redMidIndicator = scene.make.sprite({ x: this.position.X + MidIndex * this.Config!.RedTeam.LaneOffset.X + this.Config!.RedTeam.Position.X + this.Config!.RedTeam.IconOffset.X, y: this.position.Y + MidIndex * this.Config!.RedTeam.LaneOffset.Y + this.Config!.RedTeam.Position.Y + this.Config!.RedTeam.IconOffset.Y, key: 'mid', add: true });
        this.redMidIndicator.setSize(this.Config!.IconSize, this.Config!.IconSize);
        this.redMidIndicator.setOrigin(0.5, 0.5);
        this.redMidIndicator.setDepth(1);

        this.redBotIndicator = scene.make.sprite({ x: this.position.X + BotIndex * this.Config!.RedTeam.LaneOffset.X + this.Config!.RedTeam.Position.X + this.Config!.RedTeam.IconOffset.X, y: this.position.Y + BotIndex * this.Config!.RedTeam.LaneOffset.Y + this.Config!.RedTeam.Position.Y + this.Config!.RedTeam.IconOffset.Y, key: 'bot', add: true });
        this.redBotIndicator.setSize(this.Config!.IconSize, this.Config!.IconSize);
        this.redBotIndicator.setOrigin(0.5, 0.5);
        this.redBotIndicator.setDepth(1);

        this.redTopTime = scene.add.text(this.position.X + (TopIndex * this.Config!.RedTeam.LaneOffset.X) + this.Config!.RedTeam.Position.X, this.position.Y + (TopIndex * this.Config!.RedTeam.LaneOffset.Y) + this.Config!.RedTeam.Position.Y, '-  ', FontConfig);
        this.redTopTime.setOrigin(1, 0);
        this.redTopTime.setDepth(1);

        this.redMidTime = scene.add.text(this.position.X + (MidIndex * this.Config!.RedTeam.LaneOffset.X) + this.Config!.RedTeam.Position.X, this.position.Y + (MidIndex * this.Config!.RedTeam.LaneOffset.Y) + this.Config!.RedTeam.Position.Y, '-  ', FontConfig);
        this.redMidTime.setOrigin(1, 0);
        this.redMidTime.setDepth(1);

        this.redBotTime = scene.add.text(this.position.X + (BotIndex * this.Config!.RedTeam.LaneOffset.X) + this.Config!.RedTeam.Position.X, this.position.Y + (BotIndex * this.Config!.RedTeam.LaneOffset.Y) + this.Config!.RedTeam.Position.Y, '-  ', FontConfig);
        this.redBotTime.setOrigin(1, 0);
        this.redBotTime.setDepth(1);

        this.redTopIndicator.tint = color.color;
        this.redMidIndicator.tint = color.color;
        this.redBotIndicator.tint = color.color;

        //Background Image support
        this.scene.load.on(`filecomplete-image-inhibitorBg`, () => {
            this.bgImage = this.scene.make.sprite({ x: this.Config!.Position.X, y: this.Config!.Position.Y, key: 'inhibitorBg', add: true });
            this.bgImage.setOrigin(0,0);
            this.bgImage.setMask(this.mask);
            this.AddVisualComponent(this.bgImage);
            if (!this.isActive && !this.isShowing) {
                this.bgImage.alpha = 0;
            }
        });

        //Background Video support
        this.scene.load.on(`filecomplete-video-inhibitorBgVideo`, () => {
            if (this.bgVideo !== undefined && this.bgVideo !== null) {
                this.RemoveVisualComponent(this.bgVideo);
                this.bgVideo.destroy();
            }
            // @ts-ignore
            this.bgVideo = this.scene.add.video(this.Config!.Position.X, this.Config!.Position.Y, 'inhibitorBgVideo', false, true);
            this.bgVideo.setOrigin(0,0);
            this.bgVideo.setMask(this.mask);
            this.bgVideo.setLoop(true);
            this.bgVideo.play();
            this.AddVisualComponent(this.bgVideo);
            if (!this.isActive && !this.isShowing) {
                this.bgVideo.alpha = 0;
            }
        });

        if (this.Config?.UseBackgroundVideo) {
            this.scene.load.video('inhibitorBgVideo', 'frontend/backgrounds/Inhibitor.mp4');
        } else if (this.Config?.UseBackgroundImage) {
            this.scene.load.image('inhibitorBg', 'frontend/backgrounds/Inhibitor.png');
        } else {
            this.bg = this.scene.add.rectangle(this.Config!.Position.X, this.Config!.Position.Y, this.Config!.Size.X, this.Config!.Size.Y, Phaser.Display.Color.RGBStringToColor(this.Config!.Color).color32);
            this.bg.setOrigin(0, 0);
            this.bg.depth = -1;
            this.AddVisualComponent(this.bg);
        }
        this.visualComponents.push(this.redBotIndicator, this.redMidIndicator, this.redTopIndicator, this.redTopTime, this.redMidTime, this.redBotTime, this.blueBotIndicator, this.blueBotTime, this.blueMidIndicator, this.blueMidTime, this.blueTopIndicator, this.blueTopTime);


        this.GetActiveVisualComponents().forEach(c => {
            if (c === null || c === undefined)
                return;
            c.mask = this.mask;
            c.alpha = 0;
            c.y += this.Config.Size.Y;
        });


        if(this.Config.UseBackgroundImage || this.Config.UseBackgroundVideo) {
            this.scene.load.start();
        }
    }

    Load(): void {
        //Init in constructor
    }

    UpdateValues(newValues: InhibitorInfo): void {
        if (newValues === undefined || newValues === null || (newValues.Inhibitors.every(i => i.timeLeft <= 0) && this.Config!.HideWhenNoneDestroyed)) {
            if (this.isActive) {
                this.Stop();
            }
            return;
        }

        this.blueBotTime.text = this.ToTimeString(newValues.Inhibitors[0].timeLeft);
        this.blueMidTime.text = this.ToTimeString(newValues.Inhibitors[1].timeLeft);
        this.blueTopTime.text = this.ToTimeString(newValues.Inhibitors[2].timeLeft);
        this.redBotTime.text = this.ToTimeString(newValues.Inhibitors[5].timeLeft);
        this.redMidTime.text = this.ToTimeString(newValues.Inhibitors[4].timeLeft);
        this.redTopTime.text = this.ToTimeString(newValues.Inhibitors[3].timeLeft);

        if (!this.isActive) {
            this.Start();
        }
    }

    UpdateConfig(newConfig: InhibitorDisplayConfig): void {
        console.log('[LB] Updating Inhibitor Visual Element');

        //Get new indicator order
        const TopIndex = newConfig.LaneOrder.findIndex(l => l === "Top" || l === "top");
        const MidIndex = newConfig.LaneOrder.findIndex(l => l === "Mid" || l === "mid");
        const BotIndex = newConfig.LaneOrder.findIndex(l => l === "Bot" || l === "bot");

        //Update position
        this.position = newConfig.Position;
        
        this.maskG.clear();
        this.maskG.fillStyle(0xffffff);
        this.maskG.fillRect(newConfig.Position.X, newConfig.Position.Y, newConfig.Size.X, newConfig.Size.Y);

        //Update Text Style

        this.UpdateTextStyle(this.blueTopTime, newConfig.Font);
        this.UpdateTextStyle(this.blueMidTime, newConfig.Font);
        this.UpdateTextStyle(this.blueBotTime, newConfig.Font);

        this.UpdateTextStyle(this.redTopTime, newConfig.Font);
        this.UpdateTextStyle(this.redMidTime, newConfig.Font);
        this.UpdateTextStyle(this.redBotTime, newConfig.Font);

        //Update Text Position
        var TextPos: Vector2 = new Vector2(this.position.X + newConfig.BlueTeam.Position.X, this.position.Y + newConfig.BlueTeam.Position.Y);
        this.blueTopTime.setPosition(TopIndex * newConfig.BlueTeam.LaneOffset.X + TextPos.X, TopIndex * newConfig.BlueTeam.LaneOffset.Y + TextPos.Y);
        this.blueMidTime.setPosition(MidIndex * newConfig.BlueTeam.LaneOffset.X + TextPos.X, MidIndex * newConfig.BlueTeam.LaneOffset.Y + TextPos.Y);
        this.blueBotTime.setPosition(BotIndex * newConfig.BlueTeam.LaneOffset.X + TextPos.X, BotIndex * newConfig.BlueTeam.LaneOffset.Y + TextPos.Y);

        TextPos = new Vector2(this.position.X + newConfig.RedTeam.Position.X, this.position.Y + newConfig.RedTeam.Position.Y);
        this.redTopTime.setPosition(TopIndex * newConfig.RedTeam.LaneOffset.X + TextPos.X, TopIndex * newConfig.RedTeam.LaneOffset.Y + TextPos.Y);
        this.redMidTime.setPosition(MidIndex * newConfig.RedTeam.LaneOffset.X + TextPos.X, MidIndex * newConfig.RedTeam.LaneOffset.Y + TextPos.Y);
        this.redBotTime.setPosition(BotIndex * newConfig.RedTeam.LaneOffset.X + TextPos.X, BotIndex * newConfig.RedTeam.LaneOffset.Y + TextPos.Y);


        //Update Icon Positions
        var IconPos: Vector2 = new Vector2(this.position.X + newConfig.BlueTeam.Position.X + newConfig.BlueTeam.IconOffset.X, this.position.Y + newConfig.BlueTeam.Position.Y + newConfig.BlueTeam.IconOffset.Y);

        this.blueTopIndicator.setPosition(TopIndex * newConfig.BlueTeam.LaneOffset.X + IconPos.X, TopIndex * newConfig.BlueTeam.LaneOffset.Y + IconPos.Y);
        this.blueMidIndicator.setPosition(MidIndex * newConfig.BlueTeam.LaneOffset.X + IconPos.X, MidIndex * newConfig.BlueTeam.LaneOffset.Y + IconPos.Y);
        this.blueBotIndicator.setPosition(BotIndex * newConfig.BlueTeam.LaneOffset.X + IconPos.X, BotIndex * newConfig.BlueTeam.LaneOffset.Y + IconPos.Y);

        IconPos = new Vector2(this.position.X + newConfig.RedTeam.Position.X + newConfig.RedTeam.IconOffset.X, this.position.Y + newConfig.RedTeam.Position.Y + newConfig.RedTeam.IconOffset.Y);

        this.redTopIndicator.setPosition(TopIndex * newConfig.RedTeam.LaneOffset.X + IconPos.X, TopIndex * newConfig.RedTeam.LaneOffset.Y + IconPos.Y);
        this.redMidIndicator.setPosition(MidIndex * newConfig.RedTeam.LaneOffset.X + IconPos.X, MidIndex * newConfig.RedTeam.LaneOffset.Y + IconPos.Y);
        this.redBotIndicator.setPosition(BotIndex * newConfig.RedTeam.LaneOffset.X + IconPos.X, BotIndex * newConfig.RedTeam.LaneOffset.Y + IconPos.Y);

        //Update Icon Size
        this.blueTopIndicator.setSize(newConfig.IconSize, newConfig.IconSize);
        this.blueMidIndicator.setSize(newConfig.IconSize, newConfig.IconSize);
        this.blueBotIndicator.setSize(newConfig.IconSize, newConfig.IconSize);

        this.redTopIndicator.setSize(newConfig.IconSize, newConfig.IconSize);
        this.redMidIndicator.setSize(newConfig.IconSize, newConfig.IconSize);
        this.redBotIndicator.setSize(newConfig.IconSize, newConfig.IconSize);

        //Update Icon Colors
        var color = Phaser.Display.Color.IntegerToColor(variables.fallbackBlue);
        if (this.scene.state?.blueColor !== undefined && this.scene.state.blueColor !== '') {
            if (newConfig.UseTeamColors) {
                color = Phaser.Display.Color.RGBStringToColor(this.scene.state.blueColor);
            } else {
                color = Phaser.Display.Color.RGBStringToColor(newConfig.BlueTeam.Color);
            }
        }

        this.blueTopIndicator.tint = color.color;
        this.blueMidIndicator.tint = color.color;
        this.blueBotIndicator.tint = color.color;

        color = Phaser.Display.Color.IntegerToColor(variables.fallbackRed);
        if (this.scene.state?.redColor !== undefined && this.scene.state.redColor !== '') {
            if (newConfig.UseTeamColors) {
                color = Phaser.Display.Color.RGBStringToColor(this.scene.state.redColor);
            } else {
                color = Phaser.Display.Color.RGBStringToColor(newConfig.RedTeam.Color);
            }
        }

        this.redTopIndicator.tint = color.color;
        this.redMidIndicator.tint = color.color;
        this.redBotIndicator.tint = color.color;

        //Update Background


        //Background Image
        if (newConfig.UseBackgroundImage) {
            if (this.bgVideo !== undefined && this.bgVideo !== null) {
                this.RemoveVisualComponent(this.bgVideo);
                this.bgVideo.destroy();
            }
            if (this.bg !== undefined && this.bg !== null) {
                this.RemoveVisualComponent(this.bgImage);
                this.bg.destroy();
                this.bg = null;
            }
            //Reset old Texture
            if (this.scene.textures.exists('inhibitorBg')) {
                this.RemoveVisualComponent(this.bgImage);
                this.bgImage?.destroy();
                this.bgImage = null;
                this.scene.textures.remove('inhibitorBg');
            }
            this.scene.load.image('inhibitorBg', 'frontend/backgrounds/Inhibitor.png');
        }
        //Background Video
        else if (newConfig.UseBackgroundVideo) {
            if (this.bg !== undefined && this.bg !== null) {
                this.RemoveVisualComponent(this.bg);
                this.bg.destroy();
                this.bg = null;
            }
            if (this.bgImage !== undefined && this.bgImage !== null) {
                this.RemoveVisualComponent(this.bgImage);
                this.bgImage.destroy();
            }
            //Reset old Video
            if(this.scene.cache.video.has('inhibitorBgVideo')) {
                this.RemoveVisualComponent(this.bgVideo);
                this.bgVideo?.destroy(),
                this.bgVideo = null;
                this.scene.cache.video.remove('inhibitorBgVideo');
            }
            this.scene.load.video('inhibitorBgVideo', 'frontend/backgrounds/Inhibitor.mp4');
        }
        //Background Color
        else {
            if (this.bgImage !== undefined && this.bgImage !== null) {
                this.RemoveVisualComponent(this.bgImage);
                this.bgImage.destroy();
            }
            if (this.bgVideo !== undefined && this.bgVideo !== null) {
                this.RemoveVisualComponent(this.bgVideo);
                this.bgVideo.destroy();
            }
            if (this.bg === null || this.bg === undefined) {
                this.bg = this.scene.add.rectangle(newConfig.Position.X, newConfig.Position.Y, newConfig.Size.X, newConfig.Size.Y, Phaser.Display.Color.RGBStringToColor(newConfig.Color).color, 1);
                this.bg.setMask(this.mask);
                this.AddVisualComponent(this.bg);
                if (!this.isActive) {
                    this.bg.alpha = 0;
                }
            }
            this.bg.setOrigin(0,0);
            this.bg.setPosition(newConfig.Position.X, newConfig.Position.Y);
            this.bg.setSize(newConfig.Size.X, newConfig.Size.Y);
            this.bg.setFillStyle(Phaser.Display.Color.RGBStringToColor(newConfig.Color).color, 1);
        }

        if (!this.isActive) {
            this.GetActiveVisualComponents().forEach(c => {
                c.alpha = 0;
            });
        }

        if (newConfig.UseBackgroundImage || newConfig.UseBackgroundVideo) {
            console.log('loading assets');
            this.scene.load.start();
        }
    }

    Start(): void {
        if (this.isActive || this.isShowing)
            return;

        var ctx = this;
        this.isShowing = true;
        this.visualComponents.forEach(c => c.alpha = 1);

        ctx.scene.tweens.add({
            targets: this.GetActiveVisualComponents(),
            props: {
                y: { value: '-=' + ctx.Config.Size.Y, duration: 500, ease: 'Cubic.easeOut' }
            },
            paused: false,
            yoyo: false,
            duration: 500,
            onComplete: function () { ctx.isActive = true; ctx.isShowing = false;}
        });
    }

    Stop(): void {
        if (!this.isActive || this.isHiding)
            return;

        var ctx = this;
        this.isHiding = true;

        ctx.scene.tweens.add({
            targets: this.GetActiveVisualComponents(),
            props: {
                y: { value: '+=' + ctx.Config.Size.Y, duration: 500, ease: 'Cubic.easeOut' }
            },
            paused: false,
            yoyo: false,
            duration: 500,
            onComplete: function () { ctx.visualComponents.forEach(c => c.alpha = 0); ctx.isActive = false; ctx.isHiding = false; }
        });
    }


    ToTimeString(time: number): string {
        if (time <= 0.5) {
            return '-  ';
        }

        var timeInSec = Math.round(time);
        return (Math.floor(timeInSec / 60) >= 10 ? Math.floor(timeInSec / 60) : '0' + Math.floor(timeInSec / 60)) + ':' + (timeInSec % 60 >= 10 ? timeInSec % 60 : '0' + timeInSec % 60);
    }

    StringPadLeft(string: string, pad, length: number): string {
        return (new Array(length + 1).join(pad) + string).slice(-length);
    }
}