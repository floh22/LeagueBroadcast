import { ScoreDisplayConfig } from "~/data/config/overlayConfig";
import ScoreboardConfig from "~/data/scoreboardConfig";
import StateData from "~/data/stateData";
import PlaceholderConversion from "~/PlaceholderConversion";
import IngameScene from "~/scenes/IngameScene";
import TextUtils from "~/util/TextUtils";
import Utils from "~/util/Utils";
import variables from "~/variables";
import { VisualElement } from "./VisualElement";

export default class ScoreboardVisual extends VisualElement {

    BackgroundRect: Phaser.GameObjects.Rectangle | null = null;
    BackgroundImage: Phaser.GameObjects.Image | null = null;
    BackgroundVideo: Phaser.GameObjects.Video | null = null;
    ImgMask!: Phaser.Display.Masks.BitmapMask;
    GeoMask!: Phaser.Display.Masks.GeometryMask;
    MaskImage!: Phaser.GameObjects.Sprite;
    MaskG!: Phaser.GameObjects.Graphics;
    ScoreG!: Phaser.GameObjects.Graphics;

    GameTime: Phaser.GameObjects.Text;
    CenterIcon: Phaser.GameObjects.Sprite | null = null;

    BlueTag: Phaser.GameObjects.Text;
    BlueScoreTemplates: Phaser.Geom.Circle[] = [];
    BlueScoreText: Phaser.GameObjects.Text | null = null;
    BlueWins: number = -1;
    BlueKills: Phaser.GameObjects.Text;
    BlueTowers: Phaser.GameObjects.Text;
    BlueGold: Phaser.GameObjects.Text;
    BlueGoldImage: Phaser.GameObjects.Sprite | null = null;
    BlueTowerImage: Phaser.GameObjects.Sprite | null = null;
    BlueDragons: Phaser.GameObjects.Sprite[] = [];
    BlueIconBackground: Phaser.GameObjects.Sprite | null = null;
    BlueIconSprite: Phaser.GameObjects.Sprite | null = null;
    BlueIconName: string = '';


    RedTag: Phaser.GameObjects.Text;
    RedScoreTemplates: Phaser.Geom.Circle[] = [];
    RedScoreText: Phaser.GameObjects.Text | null = null;
    RedWins: number = -1;
    RedKills: Phaser.GameObjects.Text;
    RedTowers: Phaser.GameObjects.Text;
    RedGold: Phaser.GameObjects.Text;
    RedGoldImage: Phaser.GameObjects.Sprite | null = null;
    RedTowerImage: Phaser.GameObjects.Sprite | null = null;
    RedDragons: Phaser.GameObjects.Sprite[] = [];
    RedIconBackground: Phaser.GameObjects.Sprite | null = null;
    RedIconSprite: Phaser.GameObjects.Sprite | null = null;
    RedIconName: string = '';


    DrakeIconSize: number = 30;
    DrakeIconOffset: number = 0;
    ShowScores: boolean = false;

    TotalGameToWinsNeeded: Record<number, number> = {
        5: 3,
        3: 2,
        2: 2,
        1: 1
    };

    constructor(scene: IngameScene, cfg: ScoreDisplayConfig) {
        super(scene, cfg.Position, 'score');

        this.CreateTextureListeners();

        //Mask
        if (cfg.Background.UseAlpha) {
            this.MaskImage = scene.make.sprite({ x: cfg.Position.X, y: cfg.Position.Y, key: 'scoreboardMask', add: false });
            this.MaskImage.setOrigin(0.5, 0);
            this.MaskImage.setDisplaySize(cfg.Size.X, cfg.Size.Y);
            this.ImgMask = this.MaskImage.createBitmapMask();
        } else {
            this.MaskG = scene.make.graphics({});
            this.MaskG.fillStyle(0xffffff);
            this.MaskG.fillRect(cfg.Position.X - (cfg.Size.X / 2), cfg.Position.Y, cfg.Size.X, cfg.Size.Y);
            this.GeoMask = this.MaskG.createGeometryMask();
        }

        this.ScoreG = this.scene.make.graphics({}, true);
        this.AddVisualComponent(this.ScoreG);

        //Background
        if (cfg.Background.UseVideo) {
            this.scene.load.video('scoreBgVideo', 'frontend/backgrounds/Score.mp4');
        } else if (cfg.Background.UseImage) {
            this.scene.load.image('scoreBg', 'frontend/backgrounds/Score.png');
        } else {
            this.BackgroundRect = this.scene.add.rectangle(cfg.Position.X, cfg.Position.Y + (cfg.Size.Y / 2), cfg.Size.X, cfg.Size.Y, Phaser.Display.Color.RGBStringToColor(cfg.Background.FallbackColor).color32);
            this.BackgroundRect.depth = -1;
            this.BackgroundRect.setMask(ScoreboardVisual.GetConfig()?.Background.UseAlpha ? this.ImgMask : this.GeoMask);
            this.AddVisualComponent(this.BackgroundRect);
        }

        //Game Time
        this.GameTime = scene.add.text(this.position.X + cfg.TimePosition.X, this.position.Y + cfg.TimePosition.Y, '00:00', {
            fontFamily: cfg.TimeFont.Name,
            fontSize: cfg.TimeFont.Size,
            color: cfg.TimeFont.Color,
            fontStyle: cfg.TimeFont.Style,
            align: cfg.TimeFont.Align
        });
        this.GameTime.setOrigin(0.5,0.5);
        this.GameTime.setAlign(cfg.TimeFont.Align);
        this.AddVisualComponent(this.GameTime);

        //Center Icon
        if (cfg.Misc.ShowCenterIcon) {
            this.CenterIcon = scene.make.sprite({ x: this.position.X + cfg.Misc.CenterIconPosition.X, y: this.position.Y + cfg.Misc.CenterIconPosition.Y, key: 'scoreCenter', add: true });
            this.CenterIcon.setOrigin(0.5,0.5);
            this.CenterIcon.setDisplaySize(cfg.Misc.CenterIconSize.X, cfg.Misc.CenterIconSize.Y);
            this.AddVisualComponent(this.CenterIcon);
        }

        //Blue Team

        //Kills
        this.BlueKills = scene.add.text(this.position.X + cfg.BlueTeam.Kills.Position.X, this.position.Y + cfg.BlueTeam.Kills.Position.Y, '0', {
            fontFamily: cfg.BlueTeam.Kills.Font.Name,
            fontSize: cfg.BlueTeam.Kills.Font.Size,
            color: cfg.BlueTeam.Kills.Font.Color,
            fontStyle: cfg.BlueTeam.Kills.Font.Style,
            align: cfg.BlueTeam.Kills.Font.Align
        });
        this.AddVisualComponent(this.BlueKills);

        if (cfg.BlueTeam.Kills.Font.Align === "right" || cfg.BlueTeam.Kills.Font.Align === "Right")
            this.BlueKills.setOrigin(1, 0);
        if (cfg.BlueTeam.Kills.Font.Align === "left" || cfg.BlueTeam.Kills.Font.Align === "Left")
            this.BlueKills.setOrigin(0, 0);

        //Towers
        this.BlueTowers = scene.add.text(this.position.X + cfg.BlueTeam.Towers.Position.X, this.position.Y + cfg.BlueTeam.Towers.Position.Y, '0', {
            fontFamily: cfg.BlueTeam.Towers.Font.Name,
            fontSize: cfg.BlueTeam.Towers.Font.Size,
            fontStyle: cfg.BlueTeam.Towers.Font.Style,
            align: cfg.BlueTeam.Towers.Font.Align,
            color: cfg.BlueTeam.Towers.Font.Color,
        });
        this.AddVisualComponent(this.BlueTowers);

        if (cfg.BlueTeam.Towers.Font.Align === "right" || cfg.BlueTeam.Towers.Font.Align === "Right")
            this.BlueTowers.setOrigin(1, 0);
        if (cfg.BlueTeam.Towers.Font.Align === "left" || cfg.BlueTeam.Towers.Font.Align === "Left")
            this.BlueTowers.setOrigin(0, 0);

        this.BlueTowerImage = scene.make.sprite({ x: this.position.X + cfg.BlueTeam.Towers.Position.X + cfg.BlueTeam.Towers.Icon.Offset.X, y: this.position.Y + cfg.BlueTeam.Towers.Position.Y + cfg.BlueTeam.Towers.Icon.Offset.Y, key: 'scoreTower', add: true });
        this.BlueTowerImage.setDisplaySize(cfg.BlueTeam.Towers.Icon.Size.X, cfg.BlueTeam.Towers.Icon.Size.Y);
        this.BlueTowerImage.setOrigin(0.5,0.5);
        this.AddVisualComponent(this.BlueTowerImage);

        //Gold
        this.BlueGold = scene.add.text(this.position.X + cfg.BlueTeam.Gold.Position.X, this.position.Y + cfg.BlueTeam.Gold.Position.Y, '2.5k', {
            fontFamily: cfg.BlueTeam.Gold.Font.Name,
            fontSize: cfg.BlueTeam.Gold.Font.Size,
            fontStyle: cfg.BlueTeam.Gold.Font.Style,
            align: cfg.BlueTeam.Gold.Font.Align,
            color: cfg.BlueTeam.Gold.Font.Color,
        });
        this.AddVisualComponent(this.BlueGold);
        if (cfg.BlueTeam.Gold.Font.Align === "right" || cfg.BlueTeam.Gold.Font.Align === "Right")
            this.BlueGold.setOrigin(1, 0);
        if (cfg.BlueTeam.Gold.Font.Align === "left" || cfg.BlueTeam.Gold.Font.Align === "Left")
            this.BlueGold.setOrigin(0, 0);

        this.BlueGoldImage = scene.make.sprite({ x: this.position.X + cfg.BlueTeam.Gold.Position.X + cfg.BlueTeam.Gold.Icon.Offset.X, y: this.position.Y + cfg.BlueTeam.Gold.Position.Y + cfg.BlueTeam.Gold.Icon.Offset.Y, key: 'scoreGold', add: true });
        this.BlueGoldImage.setDisplaySize(cfg.BlueTeam.Gold.Icon.Size.X, cfg.BlueTeam.Gold.Icon.Size.Y);
        this.BlueGoldImage.setOrigin(0.5,0.5);
        this.AddVisualComponent(this.BlueGoldImage);

        this.BlueDragons = [];

        this.BlueScoreTemplates = [];
        for (var i = 0; i < 5; i++) {
            this.BlueScoreTemplates.push(new Phaser.Geom.Circle(this.position.X + cfg.BlueTeam.Score.Position.X + i * cfg.BlueTeam.Score.CircleOffset.X, this.position.Y + cfg.BlueTeam.Score.Position.Y + i * cfg.BlueTeam.Score.CircleOffset.Y, cfg.BlueTeam.Score.CircleRadius));
        }

        //Tag
        this.BlueTag = scene.add.text(this.position.X + cfg.BlueTeam.Name.Position.X, this.position.Y + cfg.BlueTeam.Name.Position.Y, 'Blue', {
            fontFamily: cfg.BlueTeam.Name.Font.Name,
            fontSize: cfg.BlueTeam.Name.Font.Size,
            fontStyle: cfg.BlueTeam.Name.Font.Style,
            align: cfg.BlueTeam.Name.Font.Align,
            color: cfg.BlueTeam.Name.Font.Color,
        });
        this.AddVisualComponent(this.BlueTag);

        if (cfg.BlueTeam.Name.Font.Align === "right" || cfg.BlueTeam.Name.Font.Align === "Right")
            this.BlueTag.setOrigin(1, 0);
        if (cfg.BlueTeam.Name.Font.Align === "left" || cfg.BlueTeam.Name.Font.Align === "Left")
            this.BlueTag.setOrigin(0, 0);

        //Icon
        if (cfg.BlueTeam.Icon.UseBackground) {
            this.BlueIconBackground = scene.make.sprite({
                x: cfg.Position.X + cfg.BlueTeam.Icon.Position.X + cfg.BlueTeam.Icon.BackgroundOffset.X,
                y: cfg.Position.Y + cfg.BlueTeam.Icon.Position.Y + cfg.BlueTeam.Icon.BackgroundOffset.Y,
                key: 'scoreBlueIcon',
                add: true
            });
            this.AddVisualComponent(this.BlueIconBackground);
        }

        //Red Team

        //Kills
        this.RedKills = scene.add.text(this.position.X + cfg.RedTeam.Kills.Position.X, this.position.Y + cfg.RedTeam.Kills.Position.Y, '0', {
            fontFamily: cfg.RedTeam.Kills.Font.Name,
            fontSize: cfg.RedTeam.Kills.Font.Size,
            fontStyle: cfg.RedTeam.Kills.Font.Style,
            align: cfg.RedTeam.Kills.Font.Align,
            color: cfg.RedTeam.Kills.Font.Color,
        });
        this.AddVisualComponent(this.RedKills);

        if (cfg.RedTeam.Kills.Font.Align === "right" || cfg.RedTeam.Kills.Font.Align === "Right")
            this.RedKills.setOrigin(1, 0);
        if (cfg.RedTeam.Kills.Font.Align === "left" || cfg.RedTeam.Kills.Font.Align === "Left")
            this.RedKills.setOrigin(0, 0);

        //Towers
        this.RedTowers = scene.add.text(this.position.X + cfg.RedTeam.Towers.Position.X, this.position.Y + cfg.RedTeam.Towers.Position.Y, '0', {
            fontFamily: cfg.RedTeam.Towers.Font.Name,
            fontSize: cfg.RedTeam.Towers.Font.Size,
            fontStyle: cfg.RedTeam.Towers.Font.Style,
            align: cfg.RedTeam.Towers.Font.Align,
            color: cfg.RedTeam.Towers.Font.Color,
        });
        this.AddVisualComponent(this.RedTowers);

        if (cfg.RedTeam.Towers.Font.Align === "right" || cfg.RedTeam.Towers.Font.Align === "Right")
            this.RedTowers.setOrigin(1, 0);
        if (cfg.RedTeam.Towers.Font.Align === "left" || cfg.RedTeam.Towers.Font.Align === "Left")
            this.RedTowers.setOrigin(0, 0);

        this.RedTowerImage = scene.make.sprite({ x: this.position.X + cfg.RedTeam.Towers.Position.X + cfg.RedTeam.Towers.Icon.Offset.X, y: this.position.Y + cfg.RedTeam.Towers.Position.Y + cfg.RedTeam.Towers.Icon.Offset.Y, key: 'scoreTower', add: true });
        this.RedTowerImage.setDisplaySize(cfg.RedTeam.Towers.Icon.Size.X, cfg.RedTeam.Towers.Icon.Size.Y);
        this.RedTowerImage.setOrigin(0.5,0.5);
        this.AddVisualComponent(this.RedTowerImage);

        //Gold
        this.RedGold = scene.add.text(this.position.X + cfg.RedTeam.Gold.Position.X, this.position.Y + cfg.RedTeam.Gold.Position.Y, '2.5k', {
            fontFamily: cfg.RedTeam.Gold.Font.Name,
            fontSize: cfg.RedTeam.Gold.Font.Size,
            fontStyle: cfg.RedTeam.Gold.Font.Style,
            align: cfg.RedTeam.Gold.Font.Align,
            color: cfg.RedTeam.Gold.Font.Color,
        });
        this.AddVisualComponent(this.RedGold);

        if (cfg.RedTeam.Gold.Font.Align === "right" || cfg.RedTeam.Gold.Font.Align === "Right")
            this.RedGold.setOrigin(1, 0);
        if (cfg.RedTeam.Gold.Font.Align === "left" || cfg.RedTeam.Gold.Font.Align === "Left")
            this.RedGold.setOrigin(0, 0);

        this.RedGoldImage = scene.make.sprite({ x: this.position.X + cfg.RedTeam.Gold.Position.X + cfg.RedTeam.Gold.Icon.Offset.X, y: this.position.Y + cfg.RedTeam.Gold.Position.Y + cfg.RedTeam.Gold.Icon.Offset.Y, key: 'scoreGold', add: true });
        this.RedGoldImage.setDisplaySize(cfg.RedTeam.Gold.Icon.Size.X, cfg.RedTeam.Gold.Icon.Size.Y);
        this.RedGoldImage.setOrigin(0.5,0.5);
        this.AddVisualComponent(this.RedGoldImage);

        this.RedDragons = [];

        this.RedScoreTemplates = [];

        for (var i = 0; i <= 5; i++) {
            this.RedScoreTemplates.push(new Phaser.Geom.Circle(this.position.X + cfg.RedTeam.Score.Position.X + i * cfg.RedTeam.Score.CircleOffset.X, this.position.Y + cfg.RedTeam.Score.Position.Y + i * cfg.RedTeam.Score.CircleOffset.Y, cfg.RedTeam.Score.CircleRadius));
        }

        //Tag
        this.RedTag = scene.add.text(this.position.X + cfg.RedTeam.Name.Position.X, this.position.Y + cfg.RedTeam.Name.Position.Y, 'Red', {
            fontFamily: cfg.RedTeam.Name.Font.Name,
            fontSize: cfg.RedTeam.Name.Font.Size,
            fontStyle: cfg.RedTeam.Name.Font.Style,
            align: cfg.RedTeam.Name.Font.Align,
            color: cfg.RedTeam.Name.Font.Color,
        });
        this.AddVisualComponent(this.RedTag);
        
        if (cfg.RedTeam.Name.Font.Align === "right" || cfg.RedTeam.Name.Font.Align === "Right")
            this.RedTag.setOrigin(1, 0);
        if (cfg.RedTeam.Name.Font.Align === "left" || cfg.RedTeam.Name.Font.Align === "Left")
            this.RedTag.setOrigin(0, 0);

        //Icon
        if (cfg.RedTeam.Icon.UseBackground) {
            this.RedIconBackground = scene.make.sprite({
                x: cfg.Position.X + cfg.RedTeam.Icon.Position.X + cfg.RedTeam.Icon.BackgroundOffset.X,
                y: cfg.Position.Y + cfg.RedTeam.Icon.Position.Y + cfg.RedTeam.Icon.BackgroundOffset.Y,
                key: 'scoreRedIcon',
                add: true
            });
            this.AddVisualComponent(this.RedIconBackground);
        }

        //Load Resources
        if (cfg.Background.UseImage || cfg.Background.UseVideo) {
            this.scene.load.start();
        }

        //Hide Elements on start
        this.GetActiveVisualComponents().forEach(c => {
            c.alpha = 0;
            c.y -= cfg.Size.Y;
        });

        this.Init();
    }

    UpdateValues(state: StateData): void {
        let scoreConfig = state.scoreboard;

        if (scoreConfig.GameTime === undefined || scoreConfig.GameTime === null || scoreConfig.GameTime == -1) {
            if (this.isActive) {
                this.Stop();
            }
            return;
        }

        var timeInSec = Math.round(scoreConfig.GameTime);
        this.GameTime.text = (Math.floor(timeInSec / 60) >= 10 ? Math.floor(timeInSec / 60) : '0' + Math.floor(timeInSec / 60)) + ':' + (timeInSec % 60 >= 10 ? timeInSec % 60 : '0' + timeInSec % 60);

        //Update blue team values
        this.BlueGold.text = Utils.ConvertGold(scoreConfig.BlueTeam.Gold);
        this.BlueKills.text = scoreConfig.BlueTeam.Kills + '';
        this.BlueTowers.text = scoreConfig.BlueTeam.Towers + '';

        if (scoreConfig.BlueTeam.Name !== undefined){
            this.BlueTag.text = scoreConfig.BlueTeam.Name;
            if(ScoreboardVisual.GetConfig()?.RedTeam.Name.AdaptiveFontSize)
                TextUtils.AutoSizeFont(this.RedTag!, ScoreboardVisual.GetConfig()!.RedTeam.Name.MaxSize.X, ScoreboardVisual.GetConfig()!.RedTeam.Name.MaxSize.Y, +ScoreboardVisual.GetConfig()!.RedTeam.Name.Font.Size.replace(/[^\d.-]/g, ''));
        }
        else
            this.BlueTag.text = '';

        if (scoreConfig.BlueTeam.Icon !== undefined && scoreConfig.BlueTeam.Icon !== this.BlueIconName) {
            this.LoadIcon(scoreConfig.BlueTeam.Icon, false);
            this.BlueIconName = scoreConfig.BlueTeam.Icon;
        }
        if (scoreConfig.BlueTeam.Icon === undefined && this.BlueIconName !== '') {
            this.BlueIconSprite!.destroy();
            this.BlueIconSprite = null;
            this.BlueIconName = '';
        }

        //Update red team values
        this.RedGold.text = Utils.ConvertGold(scoreConfig.RedTeam.Gold);
        this.RedKills.text = scoreConfig.RedTeam.Kills + '';
        this.RedTowers.text = scoreConfig.RedTeam.Towers + '';

        if (scoreConfig.RedTeam.Name !== undefined) {
            this.RedTag!.text = scoreConfig.RedTeam.Name;
            if(ScoreboardVisual.GetConfig()?.RedTeam.Name.AdaptiveFontSize)
                TextUtils.AutoSizeFont(this.RedTag!, ScoreboardVisual.GetConfig()!.RedTeam.Name.MaxSize.X, ScoreboardVisual.GetConfig()!.RedTeam.Name.MaxSize.Y, +ScoreboardVisual.GetConfig()!.RedTeam.Name.Font.Size.replace(/[^\d.-]/g, ''));
        }
        else
            this.RedTag.text = '';

        if (scoreConfig.RedTeam.Icon !== undefined && scoreConfig.RedTeam.Icon !== this.RedIconName) {
            this.LoadIcon(scoreConfig.RedTeam.Icon, true);
            this.RedIconName = scoreConfig.RedTeam.Icon;
        }
        if (scoreConfig.RedTeam.Icon === undefined && this.RedIconName !== '') {
            this.RedIconSprite!.destroy();
            this.RedIconName = '';
            this.RedIconSprite = null;
        }

        if (this.isActive && !this.isShowing) {
            this.UpdateDragons(scoreConfig.BlueTeam.Dragons, false);
            this.UpdateDragons(scoreConfig.RedTeam.Dragons, true);
        }

        //Update scores for both teams

        if (this.ShowScores && (scoreConfig.BlueTeam.Score === undefined || scoreConfig.RedTeam.Score === undefined)) {
            this.ScoreG.clear();
            this.ShowScores = false;
        }
        if (scoreConfig.BlueTeam.Score !== undefined || scoreConfig.RedTeam.Score !== undefined) {
            if(this.ShowScores === false) {
                this.ShowScores = true;
                this.UpdateScores(state, true);
            } else {
                this.UpdateScores(state, false);
            }
        }

        if (!this.isActive) {
            this.Start();
        }
    }
    UpdateConfig(newConfig: ScoreDisplayConfig): void {

        //Position
        this.position = newConfig.Position;

        //Background
        if (!newConfig.Background.UseAlpha) {
            this.MaskG.clear();
            this.MaskG.fillStyle(0xffffff);
            this.MaskG.fillRect(newConfig.Position.X - (newConfig.Size.X / 2), newConfig.Position.Y, newConfig.Size.X, newConfig.Size.Y);
        }

        //Background Image
        if (newConfig.Background.UseImage) {
            if (this.BackgroundVideo !== undefined && this.BackgroundVideo !== null) {
                this.RemoveVisualComponent(this.BackgroundVideo);
                this.BackgroundVideo.destroy();
                this.BackgroundVideo = null;
                this.scene.cache.video.remove('scoreBgVideo');
            }
            if (this.BackgroundRect !== undefined && this.BackgroundRect !== null) {
                this.RemoveVisualComponent(this.BackgroundRect);
                this.BackgroundRect.destroy();
                this.BackgroundRect = null;
            }
            //Load Texture only if it does not already exist
            if (this.BackgroundImage === null || this.BackgroundImage === undefined) {
                this.scene.load.image('scoreBg', 'frontend/backgrounds/Score.png');
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
                this.BackgroundImage.destroy();
                this.BackgroundImage = null;
                this.scene.textures.remove('scoreBg');
            }
            //Load Video only if it does not already exist
            if(this.BackgroundVideo === null || this.BackgroundVideo === undefined) {
                this.scene.load.video('scoreBgVideo', 'frontend/backgrounds/Score.mp4');
            }
        }
        //Background Color
        else {
            if (this.BackgroundImage !== undefined && this.BackgroundImage !== null) {
                this.RemoveVisualComponent(this.BackgroundImage);
                this.BackgroundImage.destroy();
                this.BackgroundImage = null;
                this.scene.textures.remove('scoreBg');
            }
            if (this.BackgroundVideo !== undefined && this.BackgroundVideo !== null) {
                this.RemoveVisualComponent(this.BackgroundVideo);
                this.BackgroundVideo.destroy();
                this.BackgroundVideo = null;
                this.scene.cache.video.remove('scoreBgVideo');
            }
            if (this.BackgroundRect === null || this.BackgroundRect === undefined) {
                this.BackgroundRect = this.scene.add.rectangle(newConfig.Position.X, newConfig.Position.Y + (newConfig.Size.Y / 2), newConfig.Size.X, newConfig.Size.Y, Phaser.Display.Color.RGBStringToColor(newConfig.Background.FallbackColor).color, 1);
                this.BackgroundRect.setMask(ScoreboardVisual.GetConfig()?.Background.UseAlpha ? this.ImgMask : this.GeoMask);
                this.AddVisualComponent(this.BackgroundRect);
                if (!this.isActive) {
                    this.BackgroundRect.alpha = 0;
                }
            }
            this.BackgroundRect.setPosition(newConfig.Position.X, newConfig.Position.Y + (newConfig.Size.Y / 2));
            this.BackgroundRect.setSize(newConfig.Size.X, newConfig.Size.Y);
            this.BackgroundRect.setFillStyle(Phaser.Display.Color.RGBStringToColor(newConfig.Background.FallbackColor).color, 1);
            this.BackgroundRect.setDepth(-1);
        }

        //Game Time
        this.UpdateTextStyle(this.GameTime, newConfig.TimeFont);
        this.GameTime.setPosition(this.position.X + newConfig.TimePosition.X, this.position.Y + newConfig.TimePosition.Y);

        //Center Icon
        if (ScoreboardVisual.GetConfig()?.Misc.ShowCenterIcon && newConfig.Misc.ShowCenterIcon) {
            //Update Center Icon
            this.CenterIcon?.setPosition(this.position.X + newConfig.Misc.CenterIconPosition.X, this.position.Y + newConfig.Misc.CenterIconPosition.Y);
            this.CenterIcon?.setDisplaySize(newConfig.Misc.CenterIconSize.X, newConfig.Misc.CenterIconSize.Y);
        } else if (ScoreboardVisual.GetConfig()?.Misc.ShowCenterIcon && !newConfig.Misc.ShowCenterIcon) {
            //Destroy Center Icon
            this.RemoveVisualComponent(this.CenterIcon);
            this.CenterIcon?.destroy();
            this.CenterIcon = null;

        } else if (!ScoreboardVisual.GetConfig()?.Misc.ShowCenterIcon && newConfig.Misc.ShowCenterIcon) {
            //Create Center Icon
            this.CenterIcon = this.scene.make.sprite({ x: this.position.X + newConfig.Misc.CenterIconPosition.X, y: this.position.Y + newConfig.Misc.CenterIconPosition.Y, key: 'scoreCenter', add: true });
            this.CenterIcon.setDisplaySize(newConfig.Misc.CenterIconSize.X, newConfig.Misc.CenterIconSize.Y);
            this.AddVisualComponent(this.CenterIcon);
        }

        //Blue Fonts
        this.UpdateTextStyle(this.BlueKills, newConfig.BlueTeam.Kills.Font);
        this.UpdateTextStyle(this.BlueGold, newConfig.BlueTeam.Gold.Font);
        this.UpdateTextStyle(this.BlueTowers, newConfig.BlueTeam.Towers.Font);
        this.UpdateTextStyle(this.BlueTag, newConfig.BlueTeam.Name.Font);
        if (!ScoreboardVisual.GetConfig()?.BlueTeam.Score.UseCircleIcons && this.BlueScoreText !== null)
            this.UpdateTextStyle(this.BlueScoreText, newConfig.BlueTeam.Score.NumberFont);

        //Red Fonts
        this.UpdateTextStyle(this.RedKills, newConfig.RedTeam.Kills.Font);
        this.UpdateTextStyle(this.RedGold, newConfig.RedTeam.Gold.Font);
        this.UpdateTextStyle(this.RedTowers, newConfig.RedTeam.Towers.Font);
        this.UpdateTextStyle(this.RedTag, newConfig.RedTeam.Name.Font);
        if (!ScoreboardVisual.GetConfig()?.RedTeam.Score.UseCircleIcons && this.RedScoreText !== null)
            this.UpdateTextStyle(this.RedScoreText, newConfig.RedTeam.Score.NumberFont);

        //Blue Positions
        this.BlueKills.setPosition(this.position.X + newConfig.BlueTeam.Kills.Position.X, this.position.Y + newConfig.BlueTeam.Kills.Position.Y);
        this.BlueTowers.setPosition(this.position.X + newConfig.BlueTeam.Towers.Position.X, this.position.Y + newConfig.BlueTeam.Towers.Position.Y);
        this.BlueGold.setPosition(this.position.X + newConfig.BlueTeam.Gold.Position.X, this.position.Y + newConfig.BlueTeam.Gold.Position.Y);
        if (!newConfig.BlueTeam.Score.UseCircleIcons && this.BlueScoreText !== null)
            this.BlueScoreText?.setPosition(this.position.X + newConfig.BlueTeam.Score.Position.X, this.position.Y + newConfig.BlueTeam.Score.Position.Y);
        this.BlueTag.setPosition(this.position.X + newConfig.BlueTeam.Name.Position.X, this.position.Y + newConfig.BlueTeam.Name.Position.Y);

        this.BlueGoldImage?.setPosition(this.position.X + newConfig.BlueTeam.Gold.Position.X + newConfig.BlueTeam.Gold.Icon.Offset.X, this.position.Y + newConfig.BlueTeam.Gold.Position.Y + newConfig.BlueTeam.Gold.Icon.Offset.Y);
        this.BlueTowerImage?.setPosition(this.position.X + newConfig.BlueTeam.Towers.Position.X + newConfig.BlueTeam.Towers.Icon.Offset.X, this.position.Y + newConfig.BlueTeam.Towers.Position.Y + newConfig.BlueTeam.Towers.Icon.Offset.Y);

        this.BlueIconSprite?.setPosition(this.position.X + newConfig.BlueTeam.Icon.Position.X, this.position.Y + newConfig.BlueTeam.Icon.Position.Y);


        //Red Positions
        this.RedKills.setPosition(this.position.X + newConfig.RedTeam.Kills.Position.X, this.position.Y + newConfig.RedTeam.Kills.Position.Y);
        this.RedTowers.setPosition(this.position.X + newConfig.RedTeam.Towers.Position.X, this.position.Y + newConfig.RedTeam.Towers.Position.Y);
        this.RedGold.setPosition(this.position.X + newConfig.RedTeam.Gold.Position.X, this.position.Y + newConfig.RedTeam.Gold.Position.Y);
        if (!newConfig.RedTeam.Score.UseCircleIcons && this.RedScoreText !== null)
            this.RedScoreText?.setPosition(this.position.X + newConfig.RedTeam.Score.Position.X, this.position.Y + newConfig.RedTeam.Score.Position.Y);
        this.RedTag.setPosition(this.position.X + newConfig.RedTeam.Name.Position.X, this.position.Y + newConfig.RedTeam.Name.Position.Y);

        this.RedGoldImage?.setPosition(this.position.X + newConfig.RedTeam.Gold.Position.X + newConfig.RedTeam.Gold.Icon.Offset.X, this.position.Y + newConfig.RedTeam.Gold.Position.Y + newConfig.RedTeam.Gold.Icon.Offset.Y);
        this.RedTowerImage?.setPosition(this.position.X + newConfig.RedTeam.Towers.Position.X + newConfig.RedTeam.Towers.Icon.Offset.X, this.position.Y + newConfig.RedTeam.Towers.Position.Y + newConfig.RedTeam.Towers.Icon.Offset.Y);

        this.RedIconSprite?.setPosition(this.position.X + newConfig.RedTeam.Icon.Position.X, this.position.Y + newConfig.RedTeam.Icon.Position.Y);

        //Blue Sizes
        this.BlueTowerImage?.setDisplaySize(newConfig.BlueTeam.Towers.Icon.Size.X, newConfig.BlueTeam.Towers.Icon.Size.Y);
        this.BlueGoldImage?.setDisplaySize(newConfig.BlueTeam.Gold.Icon.Size.X, newConfig.BlueTeam.Gold.Icon.Size.Y);
        this.BlueIconSprite?.setDisplaySize(newConfig.BlueTeam.Icon.Size.X, newConfig.BlueTeam.Icon.Size.Y);

        //Red Sizes
        this.RedTowerImage?.setDisplaySize(newConfig.RedTeam.Towers.Icon.Size.X, newConfig.RedTeam.Towers.Icon.Size.Y);
        this.RedGoldImage?.setDisplaySize(newConfig.RedTeam.Gold.Icon.Size.X, newConfig.RedTeam.Gold.Icon.Size.Y);
        this.RedIconSprite?.setDisplaySize(newConfig.RedTeam.Icon.Size.X, newConfig.RedTeam.Icon.Size.Y);

        //Blue Icon Background
        if(newConfig.BlueTeam.Icon.UseBackground && ScoreboardVisual.GetConfig().BlueTeam.Icon.UseBackground) {
            this.BlueIconBackground?.setPosition(newConfig.Position.X + newConfig.BlueTeam.Icon.Position.X + newConfig.BlueTeam.Icon.BackgroundOffset.X, newConfig.Position.Y + newConfig.BlueTeam.Icon.Position.Y + newConfig.BlueTeam.Icon.BackgroundOffset.Y)
        }
        else if(newConfig.BlueTeam.Icon.UseBackground && !ScoreboardVisual.GetConfig().BlueTeam.Icon.UseBackground) {
            this.BlueIconBackground = this.scene.make.sprite({
                x: newConfig.Position.X + newConfig.BlueTeam.Icon.Position.X + newConfig.BlueTeam.Icon.BackgroundOffset.X,
                y: newConfig.Position.Y + newConfig.BlueTeam.Icon.Position.Y + newConfig.BlueTeam.Icon.BackgroundOffset.Y,
                key: 'scoreBlueIcon',
                add: true
            });
            this.AddVisualComponent(this.BlueIconBackground);
        }
        else if(!newConfig.BlueTeam.Icon.UseBackground && ScoreboardVisual.GetConfig().BlueTeam.Icon.UseBackground) {
            this.RemoveVisualComponent(this.BlueIconBackground)
        }

        //Red Icon Background
        if(newConfig.RedTeam.Icon.UseBackground && ScoreboardVisual.GetConfig().RedTeam.Icon.UseBackground) {
            this.RedIconBackground?.setPosition(newConfig.Position.X + newConfig.RedTeam.Icon.Position.X + newConfig.RedTeam.Icon.BackgroundOffset.X, newConfig.Position.Y + newConfig.RedTeam.Icon.Position.Y + newConfig.RedTeam.Icon.BackgroundOffset.Y)
        }
        else if(newConfig.RedTeam.Icon.UseBackground && !ScoreboardVisual.GetConfig().RedTeam.Icon.UseBackground) {
            this.RedIconBackground = this.scene.make.sprite({
                x: newConfig.Position.X + newConfig.RedTeam.Icon.Position.X + newConfig.RedTeam.Icon.BackgroundOffset.X,
                y: newConfig.Position.Y + newConfig.RedTeam.Icon.Position.Y + newConfig.RedTeam.Icon.BackgroundOffset.Y,
                key: 'scoreRedIcon',
                add: true
            });
            this.AddVisualComponent(this.RedIconBackground);
        }
        else if(!newConfig.RedTeam.Icon.UseBackground && ScoreboardVisual.GetConfig().RedTeam.Icon.UseBackground) {
            this.RemoveVisualComponent(this.RedIconBackground)
        }

        if (this.scene.state !== null && this.scene.state !== undefined) {


            //Reset Score Templates
            this.BlueScoreTemplates = [];
            this.RedScoreTemplates = [];

            for (var i = 0; i < 5; i++) {
                this.BlueScoreTemplates.push(new Phaser.Geom.Circle(this.position.X + newConfig.BlueTeam.Score.Position.X + i * newConfig.BlueTeam.Score.CircleOffset.X, this.position.Y + newConfig.BlueTeam.Score.Position.Y + i * newConfig.BlueTeam.Score.CircleOffset.Y, newConfig.BlueTeam.Score.CircleRadius));
            }

            for (var i = 0; i <= 5; i++) {
                this.RedScoreTemplates.push(new Phaser.Geom.Circle(this.position.X + newConfig.RedTeam.Score.Position.X + i * newConfig.RedTeam.Score.CircleOffset.X, this.position.Y + newConfig.RedTeam.Score.Position.Y + i * newConfig.RedTeam.Score.CircleOffset.Y, newConfig.RedTeam.Score.CircleRadius));
            }

            //Redraw Scores
            this.UpdateScores(this.scene.state, true, newConfig);

            //Drake
            this.UpdateDragons(this.scene.state.scoreboard.BlueTeam.Dragons, false, newConfig, true);
            this.UpdateDragons(this.scene.state.scoreboard.RedTeam.Dragons, true, newConfig, true);
        }

        if (newConfig.Background.UseImage || newConfig.Background.UseVideo) {
            console.log('loading assets');
            this.scene.load.start();
        }

        if(!this.isActive) {
            this.GetActiveVisualComponents().forEach(c => {
                //Hide now visible elements
                c.alpha = 0;
                if(c.y > 0)
                    c.y -= newConfig.Size.Y;
            });
        }
    }

    static GetConfig(): ScoreDisplayConfig {
        return IngameScene.Instance.overlayCfg!.Score;
    }
    Load(): void {
        //Load in constructor since there is no reason to queue creation
    }
    Start(): void {
        if (this.isActive || this.isShowing)
            return;

        this.isShowing = true;
        let ctx = this;

        switch (ScoreboardVisual.GetConfig().Misc.Animation) {
            case 'none':
            case 'None':
                this.GetActiveVisualComponents().forEach(c => {
                    if(c.y < 0) {
                        c.y += ScoreboardVisual.GetConfig()!.Size.Y;
                    }
                    c.alpha = 1;
                });
                this.isShowing = false;
                this.isActive = true;
                break;
            case 'simple':
            case 'Simple':
                
                [this.BackgroundImage, this.BackgroundRect, this.BackgroundVideo, this.GameTime, this.CenterIcon, this.BlueKills, this.RedKills, this.BlueGoldImage, this.RedGoldImage, this.BlueGold, this.RedGold, this.BlueTowerImage, this.RedTowerImage, this.BlueTowers, this.RedTowers, this.BlueTag, this.RedTag, this.ScoreG, this.BlueScoreText, this.RedScoreText, this.BlueIconBackground, this.RedIconBackground].forEach(c => {
                    if(c === null || c === undefined)
                        return;
                    c.alpha = 1;
                });
                
                this.currentAnimation[0] = this.scene.tweens.add({
                    targets: [this.BackgroundImage, this.BackgroundRect, this.BackgroundVideo, this.GameTime, this.CenterIcon, this.BlueKills, this.RedKills, this.BlueGoldImage, this.RedGoldImage, this.BlueGold, this.RedGold, this.BlueTowerImage, this.RedTowerImage, this.BlueTowers, this.RedTowers, this.BlueTag, this.RedTag, this.ScoreG, this.BlueScoreText, this.RedScoreText, this.BlueIconBackground, this.RedIconBackground],
                    props: {
                        y: { value: '+=' + ScoreboardVisual.GetConfig().Size.Y, duration: 550, ease: 'Circ.easeOut' }
                    },
                    paused: false,
                    yoyo: false,
                    duration: 550,
                    onComplete: function() {
                        ctx.isShowing = false;
                        ctx.isActive = true;
                        ctx.currentAnimation = [];
                    }
                });
                this.currentAnimation[1] = this.scene.tweens.add({
                    targets: [this.BlueIconSprite, this.RedIconSprite].concat(this.BlueDragons).concat(this.RedDragons),
                    props: {
                        alpha: { from: 0, to: 1, duration: 250, ease: 'Circ.easeInOut' }
                    },
                    paused: false,
                    yoyo: false,
                    delay: 550,
                    duration: 250
                });
                break;
            case 'fancy':
            case 'Fancy':
                //TODO
                ScoreboardVisual.GetConfig().Misc.Animation = "Simple";
                this.Start();
                break;
            default:
                break;
        }
    }
    Stop(): void {
        if(!this.isActive || this.isHiding) {
            return;
        }
        this.isHiding = true;
        this.isActive = false;
        let ctx = this;

        switch (ScoreboardVisual.GetConfig()?.Misc.Animation) {
            case 'none':
            case 'None':
                this.GetActiveVisualComponents().forEach(c => {
                    c.y -= ScoreboardVisual.GetConfig()!.Size.Y;
                    c.alpha = 0;
                });
                this.isHiding = false;
                break;
            case 'simple':
            case 'Simple':
                this.currentAnimation[0] = this.scene.tweens.add({
                    targets: [this.BackgroundImage, this.BackgroundRect, this.BackgroundVideo, this.GameTime, this.CenterIcon, this.BlueKills, this.RedKills, this.BlueGoldImage, this.RedGoldImage, this.BlueGold, this.RedGold, this.BlueTowerImage, this.RedTowerImage, this.BlueTowers, this.RedTowers, this.BlueTag, this.RedTag, this.ScoreG, this.BlueScoreText, this.RedScoreText, this.BlueIconBackground, this.RedIconBackground],
                    props: {
                        y: { value: '-=' + ScoreboardVisual.GetConfig().Size.Y, duration: 550, ease: 'Circ.easeIn' }
                    },
                    paused: false,
                    yoyo: false,
                    duration: 550,
                    delay: 100,
                    onComplete: function () {
                        ctx.isHiding = false;
                        ctx.currentAnimation = [];
                    }
                });

                this.currentAnimation[1] = this.scene.tweens.add({
                    targets: [this.BlueIconSprite, this.RedIconSprite].concat(this.BlueDragons).concat(this.RedDragons),
                    props: {
                        alpha: { from: 1, to: 0, duration: 250, ease: 'Circ.easeInOut' }
                    },
                    paused: false,
                    yoyo: false,
                    duration: 100
                });
                break;
            case 'fancy':
            case 'Fancy':
                //TODO
                ScoreboardVisual.GetConfig().Misc.Animation = "Simple";
                this.Stop();
                break;
            default:
                break;
        }
    }

    CreateTextureListeners(): void {
        //Background Image support
        this.scene.load.on(`filecomplete-image-scoreBg`, () => {
            this.BackgroundImage = this.scene.make.sprite({ x: ScoreboardVisual.GetConfig()!.Position.X, y: ScoreboardVisual.GetConfig()!.Position.Y, key: 'scoreBg', add: true });
            this.BackgroundImage.setOrigin(0.5, 0);
            this.BackgroundImage.setDepth(-1);
            this.BackgroundImage.setMask(ScoreboardVisual.GetConfig()?.Background.UseAlpha ? this.ImgMask : this.GeoMask);
            this.AddVisualComponent(this.BackgroundImage);
            if (!this.isActive && !this.isShowing) {
                this.BackgroundImage.alpha = 0;
                this.BackgroundImage.y -= this.BackgroundImage.displayHeight;
            }
        });

        //Background Video support
        this.scene.load.on(`filecomplete-video-scoreBgVideo`, () => {
            if (this.BackgroundVideo !== undefined && this.BackgroundVideo !== null) {
                this.RemoveVisualComponent(this.BackgroundVideo);
                this.BackgroundVideo.destroy();
            }
            // @ts-ignore
            this.BackgroundVideo = this.scene.add.video(ScoreboardVisual.GetConfig()!.Position.X, ScoreboardVisual.GetConfig()!.Position.Y, 'scoreBgVideo', false, true);
            this.BackgroundVideo.setOrigin(0.5, 0);
            this.BackgroundVideo.setMask(ScoreboardVisual.GetConfig()?.Background.UseAlpha ? this.ImgMask : this.GeoMask);
            this.BackgroundVideo.setLoop(true);
            this.BackgroundVideo.setDepth(-1);
            this.BackgroundVideo.play();
            this.AddVisualComponent(this.BackgroundVideo);
            if (!this.isActive && !this.isShowing) {
                this.BackgroundVideo.alpha = 0;
                this.BackgroundVideo.y -= this.BackgroundVideo.displayHeight;
            }
        });

        //Team Icons

        this.scene.load.on(`filecomplete-image-blue_icon`, () => {
            this.BlueIconSprite = this.scene.make.sprite({ x: this.position.X + ScoreboardVisual.GetConfig()!.BlueTeam.Icon.Position.X, y: this.position.Y + ScoreboardVisual.GetConfig()!.BlueTeam.Icon.Position.Y, key: 'blue_icon', add: true });
            this.BlueIconSprite.displayWidth = ScoreboardVisual.GetConfig()!.BlueTeam.Icon.Size.X;
            this.BlueIconSprite.displayHeight = ScoreboardVisual.GetConfig()!.BlueTeam.Icon.Size.Y;
            this.BlueIconSprite.alpha = 0;

            if(this.isShowing && !this.isActive) {

                let delay = 550 - this.currentAnimation[0].totalElapsed;

                this.scene.tweens.add({
                    targets: this.BlueIconSprite,
                    props: {
                        alpha: { from: 0, to: 1, duration: 1000, ease: 'Circ.easeOut' }
                    },
                    paused: false,
                    yoyo: false,
                    delay: delay
                });
            }

            if(this.isActive) {
                this.BlueIconSprite.alpha = 1;
            }
               
        });

        this.scene.load.on(`filecomplete-image-red_icon`, () => {
            this.RedIconSprite = this.scene.make.sprite({ x: this.position.X + ScoreboardVisual.GetConfig()!.RedTeam.Icon.Position.X, y: this.position.Y + ScoreboardVisual.GetConfig()!.RedTeam.Icon.Position.Y, key: 'red_icon', add: true });
            this.RedIconSprite.displayWidth = ScoreboardVisual.GetConfig()!.RedTeam.Icon.Size.X;
            this.RedIconSprite.displayHeight = ScoreboardVisual.GetConfig()!.RedTeam.Icon.Size.Y;
            this.RedIconSprite.alpha = 0;

            if(this.isShowing && !this.isActive) {

                let delay = 550 - this.currentAnimation[0].totalElapsed;

                this.scene.tweens.add({
                    targets: this.RedIconSprite,
                    props: {
                        alpha: { from: 0, to: 1, duration: 1000, ease: 'Circ.easeOut' }
                    },
                    paused: false,
                    yoyo: false,
                    delay: delay
                });
            }

            if(this.isActive) {
                this.RedIconSprite.alpha = 1;
            }
        });
    }

    UpdateDragons(dragons: string[], side: boolean, cfg: ScoreDisplayConfig | null = null, force: boolean = false): void {
        let conf = cfg === null ? ScoreboardVisual.GetConfig()! : cfg;
        var dragonIcons = side ? this.RedDragons : this.BlueDragons;

        if (dragons === undefined || dragons === null) {
            dragonIcons.forEach(oldIcon => {
                this.RemoveVisualComponent(oldIcon);
                oldIcon.destroy();
            });
        }

        if (force || dragons.length !== dragonIcons.length) {
            dragonIcons.forEach(oldIcon => {
                this.RemoveVisualComponent(oldIcon);
                oldIcon.destroy();
            });
            dragonIcons = [];
            var i = 0;
            dragons.forEach(drakeName => {
                var toAdd = this.scene.make.sprite({ 
                    x: this.position.X + (side ? conf.RedTeam.Drakes.Position.X + i++ * conf.RedTeam.Drakes.Offset.X : conf.BlueTeam.Drakes.Position.X + i++ * conf.BlueTeam.Drakes.Offset.X),
                    y: this.position.Y + (side ? conf.RedTeam.Drakes.Position.Y + i++ * conf.RedTeam.Drakes.Offset.Y : conf.BlueTeam.Drakes.Position.Y + i++ * conf.BlueTeam.Drakes.Offset.Y),
                    key: 'dragon_' + drakeName,
                    add: true 
                });
                toAdd.displayHeight = conf.Misc.DrakeIconSize.X;
                toAdd.displayWidth = conf.Misc.DrakeIconSize.Y;
                dragonIcons.push(toAdd);
                this.AddVisualComponent(toAdd);
            });
            if (side) {
                this.RedDragons = dragonIcons;
            } else {
                this.BlueDragons = dragonIcons;
            }
        }
    }

    UpdateScores(state: StateData, forceUpdate: boolean, cfg: ScoreDisplayConfig | null = null): void {
        let conf = state.scoreboard;
        cfg = cfg === null ? ScoreboardVisual.GetConfig()! : cfg;
        if (forceUpdate || conf.RedTeam.Score !== this.RedWins || conf.BlueTeam.Score !== this.BlueWins) {
            console.log('[LB] Updating Score display');
            let redWins = conf.RedTeam.Score;
            let blueWins = conf.BlueTeam.Score;

            this.ScoreG.clear();
            this.ScoreG.setDepth(1);


            if (cfg.BlueTeam.Score.UseCircleIcons) {
                //Draw blue score icons
                let color = Phaser.Display.Color.IntegerToColor(variables.fallbackBlue);
                if (cfg.BlueTeam.Score.UseTeamColor) {
                    if (state.blueColor !== undefined && state.blueColor !== '') {
                        color = Phaser.Display.Color.RGBStringToColor(state.blueColor);
                        this.ScoreG.fillStyle(color.color, 1);
                        this.ScoreG.lineStyle(3, color.color, 1);
                    }
                } else {
                    this.ScoreG.fillStyle(Phaser.Display.Color.RGBStringToColor(cfg.BlueTeam.Score.FillColor).color, 1);
                    this.ScoreG.lineStyle(3, Phaser.Display.Color.RGBStringToColor(cfg.BlueTeam.Score.StrokeColor).color, 1);
                }

                let numIcons = this.TotalGameToWinsNeeded[conf.SeriesGameCount];
                this.BlueScoreTemplates.forEach(template => {
                    if (numIcons-- > 0) {
                        if (blueWins-- > 0) {
                            this.ScoreG.fillCircleShape(template);
                        } else {
                            this.ScoreG.strokeCircleShape(template);
                        }
                    }
                });
            } else {
                this.BlueScoreText!.text = conf.BlueTeam.Score + '';
            }

            if (cfg.RedTeam.Score.UseCircleIcons) {
                //Draw red score icons
                let color = Phaser.Display.Color.IntegerToColor(variables.fallbackRed);
                if (cfg.RedTeam.Score.UseTeamColor) {
                    if (state.redColor !== undefined && state.redColor !== '') {
                        color = Phaser.Display.Color.RGBStringToColor(state.redColor);
                        this.ScoreG.fillStyle(color.color, 1);
                        this.ScoreG.lineStyle(3, color.color, 1);
                    }
                } else {
                    this.ScoreG.fillStyle(Phaser.Display.Color.RGBStringToColor(cfg.RedTeam.Score.FillColor).color, 1);
                    this.ScoreG.lineStyle(3, Phaser.Display.Color.RGBStringToColor(cfg.RedTeam.Score.StrokeColor).color, 1);
                }

                let numIcons = this.TotalGameToWinsNeeded[conf.SeriesGameCount];
                this.RedScoreTemplates.forEach(template => {
                    if (numIcons-- > 0) {
                        if (redWins-- > 0) {
                            this.ScoreG.fillCircleShape(template);
                        } else {
                            this.ScoreG.strokeCircleShape(template);
                        }
                    }
                });
            } else {
                this.RedScoreText!.text = conf.RedTeam.Score + '';
            }

            //update data
            this.BlueWins = conf.BlueTeam.Score;
            this.RedWins = conf.RedTeam.Score;
        }
    }

    LoadIcon(iconLoc: string, team: boolean): void {
        var id = (team ? 'red' : 'blue') + '_icon';
        iconLoc = PlaceholderConversion.MakeUrlAbsolute(iconLoc.replace('Cache', '/cache').replace('\\', '/'));

        if (team) {
            if (this.RedIconSprite !== undefined && this.RedIconSprite !== null) {
                this.RemoveVisualComponent(this.RedIconSprite);
                this.RedIconSprite.destroy();
                this.RedIconSprite = null;
            }
        } else {
            if (this.BlueIconSprite !== undefined && this.BlueIconSprite !== null) {
                this.RemoveVisualComponent(this.BlueIconSprite);
                this.BlueIconSprite.destroy();
                this.BlueIconSprite = null;
            }
        }
        if (this.scene.textures.exists(id)) {
            this.scene.textures.remove(id);
        }

        this.scene.load.image(id, iconLoc);
        this.scene.load.start();
    }
}