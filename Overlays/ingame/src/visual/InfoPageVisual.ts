import { InfoPageDisplayConfig, InfoTabElementVector2 } from "~/data/config/overlayConfig";
import InfoSidePage from "~/data/infoSidePage";
import PlayerInfoTab from "~/data/playerInfoTab";
import PlaceholderConversion from "~/PlaceholderConversion";
import IngameScene from "~/scenes/IngameScene";
import Utils from "~/util/Utils";
import Vector2 from "~/util/Vector2";
import variables from "~/variables";
import { VisualElement } from "./VisualElement";

export default class InfoPageVisual extends VisualElement {

    BackgroundRect: Phaser.GameObjects.Rectangle | null = null;
    BackgroundImage: Phaser.GameObjects.Image | null = null;
    BackgroundVideo: Phaser.GameObjects.Video | null = null;
    ImgMask!: Phaser.Display.Masks.BitmapMask;
    GeoMask!: Phaser.Display.Masks.GeometryMask;
    MaskImage!: Phaser.GameObjects.Sprite;
    MaskGeo!: Phaser.GameObjects.Graphics;

    Title: Phaser.GameObjects.Text | null = null;
    PlayerTabs: PlayerTabIndicator[];

    static CurrentInfoType: string = '';

    constructor(scene: IngameScene, cfg: InfoPageDisplayConfig) {
        super(scene, cfg.Position, "infoPage");

        this.CreateTextureListeners();

        //Mask
        if (cfg.Background.UseAlpha) {
            this.MaskImage = scene.make.sprite({ x: cfg.Position.X - cfg.Background.Size.X, y: cfg.Position.Y, key: 'infoPageMask', add: false });
            this.MaskImage.setOrigin(0, 0);
            this.MaskImage.setDisplaySize(cfg.Background.Size.X, cfg.Background.Size.Y);
            this.ImgMask = this.MaskImage.createBitmapMask();
        } else {
            this.MaskGeo = scene.make.graphics({add: false});
            this.MaskGeo.fillStyle(0xffffff);
            this.MaskGeo.fillRect(cfg.Position.X, cfg.Position.Y, cfg.Background.Size.X, cfg.Background.Size.Y);
            this.GeoMask = this.MaskGeo.createGeometryMask();
            this.MaskGeo.setPosition(cfg.Position.X - cfg.Background.Size.X, 0);
        }

        /*
        //Background
        if (cfg.Background.UseVideo) {
            this.scene.load.video('infoBgVideo', 'frontend/backgrounds/InfoPage.mp4');
        } else if (cfg.Background.UseImage) {
            this.scene.load.image('infoBg', 'frontend/backgrounds/InfoPage.png');
        } else {
            this.BackgroundRect = this.scene.add.rectangle(cfg.Position.X, cfg.Position.Y, cfg.Background.Size.X, cfg.Background.Size.Y, Phaser.Display.Color.RGBStringToColor(cfg.Background.FallbackColor).color);
            this.BackgroundRect.setOrigin(0, 0);
            this.BackgroundRect.depth = -1;
            this.BackgroundRect.setMask(cfg.Background.UseAlpha ? this.ImgMask : this.GeoMask);
            this.AddVisualComponent(this.BackgroundRect);
        }
        */


        //Title
        if (cfg.Title.Enabled) {
            this.Title = scene.add.text(cfg.Position.X + cfg.Title.Position.XP.X, cfg.Position.Y + cfg.Title.Position.XP.Y, 'Info Tab', {
                fontFamily: cfg.Title.Font.Name,
                fontSize: cfg.Title.Font.Size,
                color: cfg.Title.Font.Color,
                fontStyle: cfg.Title.Font.Style,
                align: cfg.Title.Font.Align
            });
            this.Title.setOrigin(0.5, 0);
            this.Title.setDepth(2);
            this.AddVisualComponent(this.Title);
            this.Title.setMask(InfoPageVisual.GetConfig().Background.UseAlpha ? this.ImgMask : this.GeoMask);
        }

        this.PlayerTabs = [];

        //Load Resources
        if (cfg.Background.UseImage || cfg.Background.UseVideo) {
            this.scene.load.start();
        }

        this.Init();
    }
    UpdateValues(newValues: InfoSidePage): void {
        if (newValues.Title === undefined || newValues.Title === null || newValues.Title == '') {
            if (this.isActive) {
                this.Stop();
            }
            return;
        }

        if (this.Title !== null && this.Title !== undefined)
            this.Title.text = newValues.Title;

        InfoPageVisual.CurrentInfoType = newValues.Players[0].ExtraInfo[1];
        this.UpdatePlayerTabs(newValues);

        this.Start();

    }

    UpdatePlayerTabs(cfg: InfoSidePage): void {
        if (this.PlayerTabs.length === 0) {
            //init player tabs
            console.log(cfg);
            var i = 0;
            cfg.Players.forEach(pt => {
                this.PlayerTabs.push(new PlayerTabIndicator(pt, InfoPageVisual.GetConfig(), this.scene, i++));
            });
            return;
        }

        //update player tabs
        var i = 0;
        cfg.Players.forEach(pt => {
            this.PlayerTabs[i++].UpdateValues(pt);
        });
    }

    UpdateConfig(newConfig: InfoPageDisplayConfig): void {
        //Position
        this.position = newConfig.Position;

        //Background
        if (!newConfig.Background.UseAlpha) {
            this.MaskGeo.clear();
            this.MaskGeo.fillStyle(0xffffff);
            this.MaskGeo.fillRect(newConfig.Position.X, newConfig.Position.Y, newConfig.Background.Size.X, newConfig.Background.Size.Y);
        }

        //Background Image
        if (newConfig.Background.UseImage) {
            if (this.BackgroundVideo !== undefined && this.BackgroundVideo !== null) {
                this.RemoveVisualComponent(this.BackgroundVideo);
                this.BackgroundVideo?.destroy();
                this.BackgroundVideo = null;
                this.scene.cache.video.remove('infoBgVideo');

            }
            if (this.BackgroundRect !== undefined && this.BackgroundRect !== null) {
                this.RemoveVisualComponent(this.BackgroundRect);
                this.BackgroundRect.destroy();
                this.BackgroundRect = null;
            }
            //Load Texture only if it does not already exist
            if (this.BackgroundImage === null || this.BackgroundImage === undefined) {
                this.scene.load.image('infoBg', 'frontend/backgrounds/InfoPage.png');
            }
        }
        //Background Video
        else if (newConfig.Background.UseVideo) {
            if (this.BackgroundRect !== undefined && this.BackgroundRect !== null) {
                this.RemoveVisualComponent(this.BackgroundRect);
                this.BackgroundRect.destroy();
                this.BackgroundRect = null;
            }
            if (this.BackgroundImage !== undefined && this.BackgroundImage !== null) {
                this.RemoveVisualComponent(this.BackgroundImage);
                this.BackgroundImage?.destroy();
                this.BackgroundImage = null;
                this.scene.textures.remove('infoBg');
            }
            //Load Video only if it does not already exist
            if (this.BackgroundVideo === null || this.BackgroundVideo === undefined) {
                this.scene.load.video('infoBgVideo', 'frontend/backgrounds/InfoPage.mp4');
            }
        }
        //Background Color
        else {
            if (this.BackgroundImage !== undefined && this.BackgroundImage !== null) {
                this.RemoveVisualComponent(this.BackgroundImage);
                this.BackgroundImage?.destroy();
                this.BackgroundImage = null;
                this.scene.textures.remove('infoBg');
            }
            if (this.BackgroundVideo !== undefined && this.BackgroundVideo !== null) {
                this.RemoveVisualComponent(this.BackgroundVideo);
                this.BackgroundVideo?.destroy();
                this.BackgroundVideo = null;
                this.scene.cache.video.remove('infoBgVideo');
            }
            if (this.BackgroundRect === null || this.BackgroundRect === undefined) {
                this.BackgroundRect = this.scene.add.rectangle(newConfig.Position.X, newConfig.Position.Y, newConfig.Background.Size.X, newConfig.Background.Size.Y, Phaser.Display.Color.RGBStringToColor(newConfig.Background.FallbackColor).color32);
                this.BackgroundRect.setOrigin(0, 0);
                this.BackgroundRect.setDepth(1);
                this.BackgroundRect.setMask(InfoPageVisual.GetConfig().Background.UseAlpha ? this.ImgMask : this.GeoMask);
                this.AddVisualComponent(this.BackgroundRect);
            }
            this.BackgroundRect.setPosition(newConfig.Position.X, newConfig.Position.Y);
            this.BackgroundRect.setDisplaySize(newConfig.Background.Size.X, newConfig.Background.Size.Y);
            this.BackgroundRect.setFillStyle(Phaser.Display.Color.RGBStringToColor(newConfig.Background.FallbackColor).color, 1);
        }


        //Title
        if (newConfig.Title.Enabled) {
            if (InfoPageVisual.GetConfig().Title.Enabled) {
                this.Title?.setPosition(newConfig.Position.X + InfoPageVisual.GetCurrentElementVector2(newConfig.Title.Position).X, newConfig.Position.Y + InfoPageVisual.GetCurrentElementVector2(newConfig.Title.Position).Y);
                this.UpdateTextStyle(this.Title!, newConfig.Title.Font);
            } else {
                this.Title = this.scene.add.text(newConfig.Position.X + InfoPageVisual.GetCurrentElementVector2(newConfig.Title.Position).X, newConfig.Position.Y + InfoPageVisual.GetCurrentElementVector2(newConfig.Title.Position).Y, 'Info Tab', {
                    fontFamily: newConfig.Title.Font.Name,
                    fontSize: newConfig.Title.Font.Size,
                    color: newConfig.Title.Font.Color,
                    fontStyle: newConfig.Title.Font.Style,
                    align: newConfig.Title.Font.Align
                });
                this.Title.setOrigin(0.5, 0);
                this.Title.setDepth(2);
                this.Title.setMask(InfoPageVisual.GetConfig().Background.UseAlpha ? this.ImgMask : this.GeoMask);
                this.AddVisualComponent(this.Title);
            }
        }
        else if (!(this.Title === null || this.Title === undefined)) {
            this.RemoveVisualComponent(this.Title);
            this.Title.destroy();
            this.Title = null;
        }

        //Player Tabs
        if (this.PlayerTabs.length !== 0) {
            this.PlayerTabs.forEach(tab => { tab.UpdateConfig(newConfig) });
        }

        this.scene.load.start();
    }
    Load(): void {
        //Load in constructor
    }
    Start(): void {
        if (this.isActive || this.isShowing)
            return;

        var ctx = this;
        this.isShowing = true;

        this.UpdateConfig(InfoPageVisual.GetConfig());

        this.currentAnimation[0] = ctx.scene.tweens.add({
            targets: [ctx.MaskGeo, ctx.MaskImage],
            props: {
                x: { from: InfoPageVisual.GetConfig().Position.X - InfoPageVisual.GetConfig().Background.Size.X, to: 0, duration: 1000, ease: 'Cubic.easeInOut' }
            },
            paused: false,
            yoyo: false,
            onComplete: function () {
                ctx.PlayerTabs.forEach(pt => pt.Start());
                setTimeout(() => {
                    ctx.isActive = true;
                    ctx.isShowing = false;
                    //Force Images to show incase some race condition prevented it earlier
                    ctx.PlayerTabs.forEach(pt => {
                        if(pt.Image !== null && pt.Image !== undefined) {
                            pt.Image.setAlpha(1);
                        }
                    })
                }, 500);
            }
        });
    }
    Stop(): void {
        if (!this.isActive || this.isHiding)
            return;

        this.isActive = false;
        this.isHiding = true;
        var ctx = this;
        ctx.PlayerTabs.forEach(pt => pt.Stop());
        this.currentAnimation[0] = ctx.scene.tweens.add({
            targets: [ctx.MaskGeo, ctx.MaskImage],
            props: {
                x: { from: 0, to: ctx.position.X - InfoPageVisual.GetConfig().Background.Size.X, duration: 1000, ease: 'Cubic.easeInOut' }
            },
            paused: false,
            yoyo: false,
            delay: 500,
            onComplete: function () {
                ctx.isHiding = false;
            }
        });
    }

    static GetConfig(): InfoPageDisplayConfig {
        return IngameScene.Instance.overlayCfg!.InfoPage;
    }

    CreateTextureListeners(): void {
        //Background Image support
        this.scene.load.on(`filecomplete-image-infoBg`, () => {
            this.BackgroundImage = this.scene.make.sprite({ x: InfoPageVisual.GetConfig()!.Position.X, y: InfoPageVisual.GetConfig()!.Position.Y, key: 'infoBg', add: true });
            this.BackgroundImage.setOrigin(0, 0);
            this.BackgroundImage.setDepth(1);
            this.BackgroundImage.setMask(InfoPageVisual.GetConfig()?.Background.UseAlpha ? this.ImgMask : this.GeoMask);
            this.AddVisualComponent(this.BackgroundImage);
            if (!this.isActive && !this.isShowing) {
                this.BackgroundImage.alpha = 0;
            }
        });

        //Background Video support
        this.scene.load.on(`filecomplete-video-infoBgVideo`, () => {
            if (this.BackgroundVideo !== undefined && this.BackgroundVideo !== null) {
                this.RemoveVisualComponent(this.BackgroundVideo);
                this.BackgroundVideo.destroy();
            }
            // @ts-ignore
            this.BackgroundVideo = this.scene.add.video(InfoPageVisual.GetConfig()!.Position.X, InfoPageVisual.GetConfig()!.Position.Y, 'infoBgVideo', false, true);
            this.BackgroundVideo.setOrigin(0, 0);
            this.BackgroundVideo.setMask(InfoPageVisual.GetConfig().Background.UseAlpha ? this.ImgMask : this.GeoMask);
            this.BackgroundVideo.setLoop(true);
            this.BackgroundVideo.setDepth(1);
            this.BackgroundVideo.play();
            this.AddVisualComponent(this.BackgroundVideo);
            if (!this.isActive && !this.isShowing) {
                this.BackgroundVideo.alpha = 0;
            }
        });
    }


    static GetCurrentElementVector2(pos: InfoTabElementVector2, infoType: string = 'current'): Vector2 {
        if (infoType === 'current') {
            infoType = InfoPageVisual.CurrentInfoType;
        }
        switch (infoType) {
            case 'gold':
                return pos.Gold;
            case 'cspm':
                return pos.CSPM;
            case 'exp':
                return pos.XP;
            default:
                return new Vector2(0, 0);
        }
    }

}

export class PlayerTabIndicator {
    Scene: IngameScene;
    TopSeparator: Phaser.GameObjects.Sprite | null = null;
    PlayerName: string;
    Image: Phaser.GameObjects.Sprite | null = null;
    Name: Phaser.GameObjects.Text | null = null;
    ProgressBarTotal: Phaser.GameObjects.Rectangle | null = null;
    ProgresssBarCompleted: Phaser.GameObjects.Rectangle | null = null;
    MinVal: Phaser.GameObjects.Text | null = null;
    MaxVal: Phaser.GameObjects.Text | null = null;
    CurVal: Phaser.GameObjects.Text | null = null;
    MainVal: Phaser.GameObjects.Text | null = null;

    Row: number;
    BaseOffset: number;
    Id: string;
    CurrentInfo: PlayerInfoTab;

    Config = IngameScene.Instance.overlayCfg?.InfoPage;
    VisualComponents: any[] = [];

    constructor(tabInfo: PlayerInfoTab, cfg: InfoPageDisplayConfig, scene: IngameScene, row: number) {
        this.Scene = scene;
        this.Row = row;
        this.BaseOffset = cfg.Position.Y + cfg.TitleHeight;
        this.CurrentInfo = tabInfo;
        this.Id = `${this.Row}_champIcon`;
        this.CreateTextureListeners();

        var ProgressColor = Phaser.Display.Color.IntegerToColor(tabInfo.ExtraInfo[2] === "ORDER" ? variables.fallbackBlue : variables.fallbackRed);
        if (cfg.TabConfig.ProgressBar.UseTeamColors && this.Scene.state?.blueColor !== undefined && this.Scene.state.blueColor !== '') {
            ProgressColor = Phaser.Display.Color.RGBStringToColor(tabInfo.ExtraInfo[2] === "ORDER" ? this.Scene.state?.blueColor : this.Scene.state?.redColor);
        } else {
            ProgressColor = Phaser.Display.Color.RGBStringToColor(tabInfo.ExtraInfo[2] === "ORDER" ? cfg.TabConfig.ProgressBar.OrderColor : cfg.TabConfig.ProgressBar.OrderColor);
        }

        var TextColor = Phaser.Display.Color.IntegerToColor(0xffffff);
        if (cfg.TabConfig.UseTeamColorsText && this.Scene.state?.blueColor !== undefined && this.Scene.state.blueColor !== '') {
            TextColor = Phaser.Display.Color.RGBStringToColor(tabInfo.ExtraInfo[2] === "ORDER" ? this.Scene.state?.blueColor : this.Scene.state?.redColor);
        }


        if (cfg.TabConfig.Separator.Enabled) {
            this.TopSeparator = scene.make.sprite({ x: cfg.Position.X + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.Separator.Position, InfoPageVisual.CurrentInfoType).X, y: this.BaseOffset + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.Separator.Position, InfoPageVisual.CurrentInfoType).Y + row * cfg.TabConfig.TabSize.Y, key: 'infoPageSeparator', add: false });
            this.TopSeparator.setOrigin(0, 0);
            this.TopSeparator.setDepth(2);
            this.VisualComponents.push(this.TopSeparator);
        }

        if (cfg.TabConfig.PlayerName.Enabled) {
            this.Name = scene.add.text(cfg.Position.X + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.PlayerName.Position, InfoPageVisual.CurrentInfoType).X, this.BaseOffset + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.PlayerName.Position, InfoPageVisual.CurrentInfoType).Y + row * cfg.TabConfig.TabSize.Y, tabInfo.PlayerName, {
                fontFamily: cfg.TabConfig.PlayerName.Font.Name,
                fontSize: cfg.TabConfig.PlayerName.Font.Size,
                color: cfg.TabConfig.UseTeamColorsText ? this.ColorToRGBString(TextColor) : cfg.TabConfig.PlayerName.Font.Color,
                fontStyle: cfg.TabConfig.PlayerName.Font.Style,
                align: cfg.TabConfig.PlayerName.Font.Align
            });
            this.Name.setDepth(2);
            this.VisualComponents.push(this.Name);
        }

        if (cfg.TabConfig.ProgressBar.Enabled) {
            this.ProgressBarTotal = this.Scene.add.rectangle(cfg.Position.X + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.ProgressBar.Position).X, this.BaseOffset + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.ProgressBar.Position, InfoPageVisual.CurrentInfoType).Y + row * cfg.TabConfig.TabSize.Y, InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.ProgressBar.Size, InfoPageVisual.CurrentInfoType).X, InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.ProgressBar.Size).Y, Phaser.Display.Color.RGBStringToColor(cfg.TabConfig.ProgressBar.DefaultColor).color);
            this.ProgressBarTotal.setOrigin(0, 0);
            this.ProgressBarTotal.setDepth(2);
            this.ProgresssBarCompleted = this.Scene.add.rectangle(cfg.Position.X + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.ProgressBar.Position).X, this.BaseOffset + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.ProgressBar.Position, InfoPageVisual.CurrentInfoType).Y + row * cfg.TabConfig.TabSize.Y, InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.ProgressBar.Size, InfoPageVisual.CurrentInfoType).X * ((tabInfo.Values.CurrentValue - tabInfo.Values.MinValue) / (tabInfo.Values.MaxValue - tabInfo.Values.MinValue)), InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.ProgressBar.Size, InfoPageVisual.CurrentInfoType).Y, ProgressColor.color);
            this.ProgresssBarCompleted.setOrigin(0, 0);
            this.ProgresssBarCompleted.setDepth(2);
            this.ProgresssBarCompleted.setFillStyle(ProgressColor.color);
            this.VisualComponents.push(this.ProgressBarTotal, this.ProgresssBarCompleted);
        }


        if (cfg.TabConfig.MinValue.Enabled) {
            this.MinVal = scene.add.text(cfg.Position.X + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.MinValue.Position, InfoPageVisual.CurrentInfoType).X, this.BaseOffset + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.MinValue.Position, InfoPageVisual.CurrentInfoType).Y + row * cfg.TabConfig.TabSize.Y, tabInfo.Values.MinValue + '', {
                fontFamily: cfg.TabConfig.MinValue.Font.Name,
                fontSize: cfg.TabConfig.MinValue.Font.Size,
                color: cfg.TabConfig.UseTeamColorsText ? this.ColorToRGBString(TextColor) : cfg.TabConfig.MinValue.Font.Color,
                fontStyle: cfg.TabConfig.MinValue.Font.Style,
                align: cfg.TabConfig.MinValue.Font.Align
            });
            if (cfg.TabConfig.MinValue.Font.Align === "right" || cfg.TabConfig.MinValue.Font.Align === "Right")
                this.MinVal.setOrigin(1, 0);
            if (cfg.TabConfig.MinValue.Font.Align === "left" || cfg.TabConfig.MinValue.Font.Align === "Left")
                this.MinVal.setOrigin(0, 0);
            this.MinVal.setDepth(2);
            this.VisualComponents.push(this.MinVal);
        }

        if (cfg.TabConfig.MaxValue.Enabled) {
            this.MaxVal = scene.add.text(cfg.Position.X + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.MaxValue.Position, InfoPageVisual.CurrentInfoType).X, this.BaseOffset + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.MaxValue.Position, InfoPageVisual.CurrentInfoType).Y + row * cfg.TabConfig.TabSize.Y, tabInfo.Values.MaxValue + '', {
                fontFamily: cfg.TabConfig.MaxValue.Font.Name,
                fontSize: cfg.TabConfig.MaxValue.Font.Size,
                color: cfg.TabConfig.UseTeamColorsText ? this.ColorToRGBString(TextColor) : cfg.TabConfig.MaxValue.Font.Color,
                fontStyle: cfg.TabConfig.MaxValue.Font.Style,
                align: cfg.TabConfig.MaxValue.Font.Align
            });
            if (cfg.TabConfig.MaxValue.Font.Align === "right" || cfg.TabConfig.MaxValue.Font.Align === "Right")
                this.MaxVal.setOrigin(1, 0);
            if (cfg.TabConfig.MaxValue.Font.Align === "left" || cfg.TabConfig.MaxValue.Font.Align === "Left")
                this.MaxVal.setOrigin(0, 0);
            this.MaxVal.setDepth(2);
            this.VisualComponents.push(this.MaxVal);
        }

        if (cfg.TabConfig.CurrentValue.Enabled) {
            this.MainVal = scene.add.text(cfg.Position.X + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.CurrentValue.Position, InfoPageVisual.CurrentInfoType).X, this.BaseOffset + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.CurrentValue.Position, InfoPageVisual.CurrentInfoType).Y + row * cfg.TabConfig.TabSize.Y, tabInfo.ExtraInfo[0], {
                fontFamily: cfg.TabConfig.CurrentValue.Font.Name,
                fontSize: cfg.TabConfig.CurrentValue.Font.Size,
                color: cfg.TabConfig.UseTeamColorsText ? this.ColorToRGBString(TextColor) : cfg.TabConfig.CurrentValue.Font.Color,
                fontStyle: cfg.TabConfig.CurrentValue.Font.Style,
                align: cfg.TabConfig.CurrentValue.Font.Align
            });
            this.MainVal.setOrigin(0.5, 0);
            this.MainVal.setDepth(2);
            this.VisualComponents.push(this.MainVal);
        }

        this.PlayerName = tabInfo.PlayerName;

        this.GetActiveVisualComponents().forEach(vc => {
            vc.alpha = 0;
        });

        this.LoadIcon(tabInfo.IconPath, row);


    }

    LoadIcon(iconLoc: string, row: number): void {
        iconLoc = PlaceholderConversion.MakeUrlAbsolute(iconLoc.replace('Cache', '/cache').replace('\\', '/'));

        if (this.Image !== undefined && this.Image !== null) {
            this.Image.destroy();
        }

        if (this.Scene.textures.exists(this.Id)) {
            this.Scene.textures.remove(this.Id);
        }

        if (!InfoPageVisual.GetConfig()?.TabConfig.ChampIcon.Enabled) {
            return;
        }

        this.Scene.load.image(this.Id, iconLoc);
        this.Scene.load.start();

    }

    UpdateValues(tabInfo: PlayerInfoTab): void {
        this.CurrentInfo = tabInfo;
        let width = InfoPageVisual.GetCurrentElementVector2(InfoPageVisual.GetConfig()!.TabConfig.ProgressBar.Size, InfoPageVisual.CurrentInfoType).X;
        var newWidth = width * ((tabInfo.Values.CurrentValue - tabInfo.Values.MinValue) / (tabInfo.Values.MaxValue - tabInfo.Values.MinValue));
        if (newWidth > width)
            newWidth = width;
        if (newWidth < 0)
            newWidth = 0;

        if (this.PlayerName !== tabInfo.PlayerName) {
            this.LoadIcon(tabInfo.IconPath, this.Row)
            this.PlayerName = tabInfo.PlayerName;

            var ProgressColor = Phaser.Display.Color.IntegerToColor(tabInfo.ExtraInfo[2] === "ORDER" ? variables.fallbackBlue : variables.fallbackRed);
            if (InfoPageVisual.GetConfig()!.TabConfig.ProgressBar.UseTeamColors && this.Scene.state?.blueColor !== undefined && this.Scene.state.blueColor !== '') {
                ProgressColor = Phaser.Display.Color.RGBStringToColor(tabInfo.ExtraInfo[2] === "ORDER" ? this.Scene.state?.blueColor : this.Scene.state?.redColor);
            } else {
                ProgressColor = Phaser.Display.Color.RGBStringToColor(tabInfo.ExtraInfo[2] === "ORDER" ? InfoPageVisual.GetConfig()!.TabConfig.ProgressBar.OrderColor : InfoPageVisual.GetConfig()!.TabConfig.ProgressBar.OrderColor);
            }

            if (InfoPageVisual.GetConfig()!.TabConfig.ProgressBar.Enabled) {
                this.ProgresssBarCompleted!.width = newWidth;
                this.ProgresssBarCompleted!.setFillStyle(ProgressColor.color);
            }
        } else {
            var ctx = this;
            ctx.Scene.tweens.add({
                targets: ctx.ProgresssBarCompleted,
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
                //Dont show min as its always going to be 0
                //min = Math.trunc(tabInfo.Values.MinValue) + '';
                min = '';
                max = Math.trunc(tabInfo.Values.CurrentValue) + '';
                let val = Math.trunc(tabInfo.Values.CurrentValue);
                cur = Utils.ConvertGold(val);
                if (cur.includes('NaN')) {
                    console.log(`${val} -> ${cur}`);
                }
                break;
            case 'cspm':
                min = tabInfo.Values.MinValue.toFixed(1);
                max = Math.trunc((tabInfo.ExtraInfo[0] as unknown as number)) + '';
                cur = (tabInfo.Values.CurrentValue).toFixed(1);
                break;
            default:
                min = tabInfo.Values.MinValue + '';
                max = tabInfo.Values.MaxValue + ''
                cur = tabInfo.ExtraInfo[0];
                break;
        }

        if (InfoPageVisual.GetConfig()!.TabConfig.PlayerName.Enabled)
            this.Name!.text = tabInfo.PlayerName;
        if (InfoPageVisual.GetConfig()!.TabConfig.MinValue.Enabled)
            this.MinVal!.text = min;
        if (InfoPageVisual.GetConfig()!.TabConfig.MaxValue.Enabled)
            this.MaxVal!.text = max;
        if (InfoPageVisual.GetConfig()!.TabConfig.CurrentValue.Enabled)
            this.MainVal!.text = cur;

    }


    UpdateConfig(cfg: InfoPageDisplayConfig): void {

        var ProgressColor = Phaser.Display.Color.IntegerToColor(this.CurrentInfo.ExtraInfo[2] === "ORDER" ? variables.fallbackBlue : variables.fallbackRed);
        if (cfg.TabConfig.ProgressBar.UseTeamColors && this.Scene.state?.blueColor !== undefined && this.Scene.state.blueColor !== '') {
            ProgressColor = Phaser.Display.Color.RGBStringToColor(this.CurrentInfo.ExtraInfo[2] === "ORDER" ? this.Scene.state?.blueColor : this.Scene.state?.redColor);
        } else {
            ProgressColor = Phaser.Display.Color.RGBStringToColor(this.CurrentInfo.ExtraInfo[2] === "ORDER" ? cfg.TabConfig.ProgressBar.OrderColor : cfg.TabConfig.ProgressBar.OrderColor);
        }

        var TextColor = Phaser.Display.Color.IntegerToColor(0xffffff);
        if (cfg.TabConfig.UseTeamColorsText && this.Scene.state?.blueColor !== undefined && this.Scene.state.blueColor !== '') {
            TextColor = Phaser.Display.Color.RGBStringToColor(this.CurrentInfo.ExtraInfo[2] === "ORDER" ? this.Scene.state?.blueColor : this.Scene.state?.redColor);
        }



        if (cfg.TabConfig.Separator.Enabled) {
            if (this.TopSeparator !== null && this.TopSeparator !== undefined) {
                this.TopSeparator.setPosition(cfg.Position.X + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.Separator.Position, InfoPageVisual.CurrentInfoType).X, this.BaseOffset + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.Separator.Position, InfoPageVisual.CurrentInfoType).Y + this.Row * cfg.TabConfig.TabSize.Y);
            } else {
                this.TopSeparator = this.Scene.make.sprite({ x: cfg.Position.X + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.Separator.Position, InfoPageVisual.CurrentInfoType).X, y: this.BaseOffset + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.Separator.Position, InfoPageVisual.CurrentInfoType).Y + this.Row * cfg.TabConfig.TabSize.Y, key: 'infoPageSeparator', add: true });
                this.TopSeparator.setOrigin(0, 0);
                this.TopSeparator.setDepth(2);
                this.VisualComponents.push(this.TopSeparator);
            }
        }
        if (!cfg.TabConfig.Separator.Enabled && InfoPageVisual.GetConfig()?.TabConfig.Separator.Enabled) {
            this.RemoveVisualComponent(this.TopSeparator);
            this.TopSeparator?.destroy();
            this.TopSeparator = null;
        }

        if (cfg.TabConfig.PlayerName.Enabled) {
            if (this.Name !== null && this.Name !== undefined) {
                this.Name.setPosition(cfg.Position.X + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.PlayerName.Position, InfoPageVisual.CurrentInfoType).X, this.BaseOffset + 6 + this.Row * cfg.TabConfig.TabSize.Y);
                this.UpdateTextStyle(this.Name, cfg.TabConfig.PlayerName.Font);
            } else {
                this.Name = this.Scene.add.text(cfg.Position.X + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.PlayerName.Position, InfoPageVisual.CurrentInfoType).X, this.BaseOffset + 6 + this.Row * cfg.TabConfig.TabSize.Y, this.CurrentInfo.PlayerName, {
                    fontFamily: cfg.TabConfig.PlayerName.Font.Name,
                    fontSize: cfg.TabConfig.PlayerName.Font.Size,
                    color: cfg.TabConfig.UseTeamColorsText ? this.ColorToRGBString(TextColor) : cfg.TabConfig.PlayerName.Font.Color,
                    fontStyle: cfg.TabConfig.PlayerName.Font.Style,
                    align: cfg.TabConfig.PlayerName.Font.Align
                });
                this.Name.setDepth(2);
                this.VisualComponents.push(this.Name);
            }
        }
        if (!cfg.TabConfig.PlayerName.Enabled && InfoPageVisual.GetConfig()?.TabConfig.PlayerName.Enabled) {
            this.RemoveVisualComponent(this.Name);
            this.Name?.destroy();
            this.Name = null;
        }

        if (cfg.TabConfig.ProgressBar.Enabled) {
            if (this.ProgressBarTotal !== undefined && this.ProgressBarTotal !== null) {
                this.ProgressBarTotal.setPosition(cfg.Position.X + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.ProgressBar.Position, InfoPageVisual.CurrentInfoType).X, this.BaseOffset + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.ProgressBar.Position, InfoPageVisual.CurrentInfoType).Y + this.Row * cfg.TabConfig.TabSize.Y);
                this.ProgressBarTotal.setDisplaySize(InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.ProgressBar.Size, InfoPageVisual.CurrentInfoType).X, InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.ProgressBar.Size).Y);
                this.ProgresssBarCompleted?.setPosition(cfg.Position.X + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.ProgressBar.Position, InfoPageVisual.CurrentInfoType).X, this.BaseOffset + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.ProgressBar.Position, InfoPageVisual.CurrentInfoType).Y + this.Row * cfg.TabConfig.TabSize.Y);
            } else {
                this.ProgressBarTotal = this.Scene.add.rectangle(cfg.Position.X + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.ProgressBar.Position, InfoPageVisual.CurrentInfoType).X, this.BaseOffset + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.ProgressBar.Position, InfoPageVisual.CurrentInfoType).Y + this.Row * cfg.TabConfig.TabSize.Y, InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.ProgressBar.Size, InfoPageVisual.CurrentInfoType).X, InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.ProgressBar.Size, InfoPageVisual.CurrentInfoType).Y, Phaser.Display.Color.RGBStringToColor(cfg.TabConfig.ProgressBar.DefaultColor).color);
                this.ProgressBarTotal.setOrigin(0, 0);
                this.ProgressBarTotal.setDepth(2);
                this.ProgresssBarCompleted = this.Scene.add.rectangle(cfg.Position.X + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.ProgressBar.Position, InfoPageVisual.CurrentInfoType).X, this.BaseOffset + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.ProgressBar.Position, InfoPageVisual.CurrentInfoType).Y + this.Row * cfg.TabConfig.TabSize.Y, InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.ProgressBar.Size, InfoPageVisual.CurrentInfoType).X * ((this.CurrentInfo.Values.CurrentValue - this.CurrentInfo.Values.MinValue) / (this.CurrentInfo.Values.MaxValue - this.CurrentInfo.Values.MinValue)), InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.ProgressBar.Size, InfoPageVisual.CurrentInfoType).Y, ProgressColor.color);
                this.ProgresssBarCompleted.setDepth(2);
                this.ProgresssBarCompleted.setOrigin(0, 0);

                this.VisualComponents.push(this.ProgressBarTotal, this.ProgresssBarCompleted);
            }
            this.ProgresssBarCompleted!.setFillStyle(ProgressColor.color);
        }
        if (!cfg.TabConfig.ProgressBar.Enabled && InfoPageVisual.GetConfig()?.TabConfig.ProgressBar.Enabled) {
            this.RemoveVisualComponent(this.ProgressBarTotal);
            this.RemoveVisualComponent(this.ProgresssBarCompleted);
            this.ProgressBarTotal?.destroy();
            this.ProgresssBarCompleted?.destroy();
            this.ProgressBarTotal = null;
            this.ProgresssBarCompleted = null;
        }


        if (cfg.TabConfig.MinValue.Enabled) {
            if (this.MinVal !== undefined && this.MinVal !== null) {
                this.MinVal.setPosition(cfg.Position.X + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.MinValue.Position, InfoPageVisual.CurrentInfoType).X, this.BaseOffset + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.MinValue.Position, InfoPageVisual.CurrentInfoType).Y + this.Row * cfg.TabConfig.TabSize.Y);
                this.UpdateTextStyle(this.MinVal, cfg.TabConfig.MinValue.Font);
            } else {
                this.MinVal = this.Scene.add.text(cfg.Position.X + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.MinValue.Position, InfoPageVisual.CurrentInfoType).X, this.BaseOffset + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.MinValue.Position, InfoPageVisual.CurrentInfoType).Y + this.Row * cfg.TabConfig.TabSize.Y, this.CurrentInfo.Values.MinValue + '', {
                    fontFamily: cfg.TabConfig.MinValue.Font.Name,
                    fontSize: cfg.TabConfig.MinValue.Font.Size,
                    color: cfg.TabConfig.UseTeamColorsText ? this.ColorToRGBString(TextColor) : cfg.TabConfig.MinValue.Font.Color,
                    fontStyle: cfg.TabConfig.MinValue.Font.Style,
                    align: cfg.TabConfig.MinValue.Font.Align
                });
                if (cfg.TabConfig.MinValue.Font.Align === "right" || cfg.TabConfig.MinValue.Font.Align === "Right")
                    this.MinVal.setOrigin(1, 0);
                if (cfg.TabConfig.MinValue.Font.Align === "left" || cfg.TabConfig.MinValue.Font.Align === "Left")
                    this.MinVal.setOrigin(0, 0);
                this.MinVal.setDepth(2);
                this.VisualComponents.push(this.MinVal);
            }
        }
        if (!cfg.TabConfig.MinValue.Enabled && InfoPageVisual.GetConfig()?.TabConfig.MinValue.Enabled) {
            this.RemoveVisualComponent(this.MinVal);
            this.MinVal?.destroy();
            this.MinVal = null;
        }

        if (cfg.TabConfig.MaxValue.Enabled) {
            if (this.MaxVal !== undefined && this.MaxVal !== null) {
                this.MaxVal.setPosition(cfg.Position.X + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.MaxValue.Position, InfoPageVisual.CurrentInfoType).X, this.BaseOffset + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.MaxValue.Position, InfoPageVisual.CurrentInfoType).Y + this.Row * cfg.TabConfig.TabSize.Y);
                this.UpdateTextStyle(this.MaxVal, cfg.TabConfig.MaxValue.Font);
            } else {
                this.MaxVal = this.Scene.add.text(cfg.Position.X + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.MaxValue.Position, InfoPageVisual.CurrentInfoType).X, this.BaseOffset + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.MaxValue.Position, InfoPageVisual.CurrentInfoType).Y + this.Row * cfg.TabConfig.TabSize.Y, this.CurrentInfo.Values.MaxValue + '', {
                    fontFamily: cfg.TabConfig.MaxValue.Font.Name,
                    fontSize: cfg.TabConfig.MaxValue.Font.Size,
                    color: cfg.TabConfig.UseTeamColorsText ? this.ColorToRGBString(TextColor) : cfg.TabConfig.MaxValue.Font.Color,
                    fontStyle: cfg.TabConfig.MaxValue.Font.Style,
                    align: cfg.TabConfig.MaxValue.Font.Align
                });
                if (cfg.TabConfig.MaxValue.Font.Align === "right" || cfg.TabConfig.MaxValue.Font.Align === "Right")
                    this.MaxVal.setOrigin(1, 0);
                if (cfg.TabConfig.MaxValue.Font.Align === "left" || cfg.TabConfig.MaxValue.Font.Align === "Left")
                    this.MaxVal.setOrigin(0, 0);
                this.MaxVal.setDepth(2);
                this.VisualComponents.push(this.MaxVal);
            }
        }
        if (!cfg.TabConfig.MaxValue.Enabled && InfoPageVisual.GetConfig()?.TabConfig.MaxValue.Enabled) {
            this.RemoveVisualComponent(this.MaxVal);
            this.MaxVal?.destroy();
            this.MaxVal = null;
        }

        if (cfg.TabConfig.CurrentValue.Enabled) {
            if (this.MainVal !== undefined && this.MainVal !== null) {
                this.MainVal.setPosition(cfg.Position.X + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.CurrentValue.Position, InfoPageVisual.CurrentInfoType).X, this.BaseOffset + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.CurrentValue.Position, InfoPageVisual.CurrentInfoType).Y + this.Row * cfg.TabConfig.TabSize.Y);
                this.UpdateTextStyle(this.MainVal, cfg.TabConfig.CurrentValue.Font);
            } else {
                this.MainVal = this.Scene.add.text(cfg.Position.X + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.CurrentValue.Position, InfoPageVisual.CurrentInfoType).X, this.BaseOffset + InfoPageVisual.GetCurrentElementVector2(cfg.TabConfig.CurrentValue.Position, InfoPageVisual.CurrentInfoType).Y + this.Row * cfg.TabConfig.TabSize.Y, this.CurrentInfo.ExtraInfo[0], {
                    fontFamily: cfg.TabConfig.CurrentValue.Font.Name,
                    fontSize: cfg.TabConfig.CurrentValue.Font.Size,
                    color: cfg.TabConfig.UseTeamColorsText ? this.ColorToRGBString(TextColor) : cfg.TabConfig.CurrentValue.Font.Color,
                    fontStyle: cfg.TabConfig.CurrentValue.Font.Style,
                    align: cfg.TabConfig.CurrentValue.Font.Align
                });
                this.MainVal.setDepth(2);
                this.MainVal.setOrigin(0.5, 0);
                this.VisualComponents.push(this.MainVal);
            }
        }
        if (!cfg.TabConfig.CurrentValue.Enabled && InfoPageVisual.GetConfig()?.TabConfig.CurrentValue.Enabled) {
            this.RemoveVisualComponent(this.MainVal);
            this.MainVal?.destroy();
            this.MainVal = null;
        }
        if (this.Scene.info.isActive || this.Scene.info.isHiding) {
            this.GetActiveVisualComponents().forEach(vc => {
                vc.alpha = 0;
            });
        }
    }

    SetBarWidth(width: number = 210): void {
        if (width === InfoPageVisual.GetCurrentElementVector2(InfoPageVisual.GetConfig()!.TabConfig.ProgressBar.Size, InfoPageVisual.CurrentInfoType).X || !InfoPageVisual.GetConfig()!.TabConfig.ProgressBar.Enabled || this.ProgresssBarCompleted === null)
            return;

        this.ProgressBarTotal!.width = width;
        this.ProgresssBarCompleted!.width = 0;
    }

    Start(): void {

        var ctx = this;
        ctx.Scene.tweens.add({
            targets: [ctx.TopSeparator, ctx.Name, ctx.Image, ctx.ProgressBarTotal, ctx.ProgresssBarCompleted, ctx.MinVal, ctx.MainVal, ctx.MaxVal],
            props: {
                alpha: { from: 0, to: 1, duration: 500, ease: 'Cubic.easeInOut' }
            },
            paused: false,
            yoyo: false,
            duration: 500
        });
    }

    Stop(): void {
        var ctx = this;
        ctx.Scene.tweens.add({
            targets: [ctx.TopSeparator, ctx.Name, ctx.Image, ctx.ProgressBarTotal, ctx.ProgresssBarCompleted, ctx.MinVal, ctx.MainVal, ctx.MaxVal],
            props: {
                alpha: { from: 1, to: 0, duration: 500, ease: 'Cubic.easeInOut' }
            },
            paused: false,
            yoyo: false,
            duration: 500
        });
    }

    CreateTextureListeners(): void {
        this.Scene.load.on(`filecomplete-image-${this.Id}`, () => {
            this.Image = this.Scene.make.sprite({ x: InfoPageVisual.GetConfig()!.Position.X + InfoPageVisual.GetCurrentElementVector2(InfoPageVisual.GetConfig()!.TabConfig.ChampIcon.Position).X, y: InfoPageVisual.GetConfig()!.Position.Y + InfoPageVisual.GetConfig()!.TitleHeight + this.Row * InfoPageVisual.GetConfig()!.TabConfig.TabSize.Y + InfoPageVisual.GetCurrentElementVector2(InfoPageVisual.GetConfig()!.TabConfig.ChampIcon.Position).Y, key: this.Id, add: true });
            this.Image.setOrigin(0, 0);
            this.Image.setDepth(2);
            this.Image.displayWidth = InfoPageVisual.GetConfig()!.TabConfig.ChampIcon.Size.X;
            this.Image.displayHeight = InfoPageVisual.GetConfig()!.TabConfig.ChampIcon.Size.Y;
            this.Image.alpha = 0;
            if (this.Scene.info.isActive) {
                this.Image.alpha = 1;
            }

            
            if(this.Scene.info.isShowing) {
                setTimeout(() => {
                    let ctx = this;
                    ctx.Scene.tweens.add({
                        targets: ctx.Image,
                        props: {
                            alpha: { from: 0, to: 1, duration: 500, ease: 'Cubic.easeInOut' }
                        },
                        paused: false,
                        yoyo: false,
                        duration: 500
                    });
                }, this.Scene.info.currentAnimation[0].duration - this.Scene.info.currentAnimation[0].elapsed);
            }
            
        });
    }

    ColorToRGBString(Color: Phaser.Display.Color): string {
        return `rgb(${Color.red},${Color.green},${Color.blue})`
    }

    //I hate duplicating code but making each player tab a separate VE seems overkill... I think... I'm probably wrong

    GetActiveVisualComponents(): any[] {
        return this.VisualComponents.filter(c => c !== null && c !== undefined);
    }

    RemoveVisualComponent(component: any): void {
        this.VisualComponents = this.VisualComponents.filter(c => c !== component);
    }

    AddVisualComponent(component: any): void {
        const componentInList = this.VisualComponents.find(c => c === component);
        if (componentInList === undefined) {
            this.VisualComponents.push(component);
        }
    }

    UpdateTextStyle(textElement: Phaser.GameObjects.Text, style: { Name: string, Size: string, Align: string, Color: string, Style: string }): void {

        textElement.setFontFamily(style.Name);
        textElement.setFontStyle(style.Style);
        //@ts-ignore
        textElement.setFontSize(style.Size)
        textElement.setColor(style.Color);
        textElement.setAlign(style.Align);

    }
}