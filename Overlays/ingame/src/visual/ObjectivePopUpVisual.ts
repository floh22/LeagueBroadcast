import { ScoreboardPopUpConfig } from "~/data/config/overlayConfig";
import IngameScene from "~/scenes/IngameScene";
import Vector2 from "~/util/Vector2";
import { VisualElement } from "./VisualElement";

export default class ObjectivePopUpVisual extends VisualElement {

    BackgroundImage: Phaser.GameObjects.Image | null = null;
    BackgroundVideo: Phaser.GameObjects.Video | null = null;
    ImgMask!: Phaser.Display.Masks.BitmapMask;
    GeoMask!: Phaser.Display.Masks.GeometryMask;
    MaskImage: Phaser.GameObjects.Sprite | null = null;
    MaskGeo: Phaser.GameObjects.Graphics | null = null;
    
    Type: string;
    Height: number = 0;
    Config: ScoreboardPopUpConfig;

    constructor(scene: IngameScene, cfg: ScoreboardPopUpConfig, type: string) {
        super(scene, new Vector2(960,1080), 'scoreboard');
        this.Config = cfg;
        this.Type = type;

        this.CreateTextureListeners();


        let resourceLocation = `frontend/images/scoreboardPopUps/`;
        if(type.toLowerCase().includes('baron')) {
            resourceLocation += 'Baron/';
        } else if(type.toLowerCase().includes('herald')){
            resourceLocation += 'Herald/';
        } else {
            resourceLocation += `Dragon/${type.replace('Kill', '').replace('Spawn', '').replace('Soul', '')}/`;
        }
        resourceLocation += type;

        //Background
        if (cfg.UseVideo) {
            this.scene.load.video(`${type}PopUpVideo`, `${resourceLocation}.mp4`);
        } else if (cfg.UseImage) {
            this.scene.load.image(`${type}PopUp`, `${resourceLocation}.png`);
        } else {
            //Do nothing since only a color seems rather useless...
        }

        this.scene.load.start();

        this.Init();
    }
    UpdateValues(newValues: any): void {
        //not needed
    }
    UpdateConfig(newConfig: any): void {
        //not needed
    }
    Load(): void {
        //load when mask created
    }
    Start(): void {
        if (this.isActive || this.isShowing)
            return;
        this.isShowing = true;
        var ctx = this;

        if(this.Config.UseVideo && this.BackgroundVideo !== null) {
            this.BackgroundVideo.play();

            if(!this.Config.ForceDisplayDurationForVideo) {
                setTimeout(() => {
                    this.Stop();
                }, this.BackgroundVideo.getDuration() * 1000 - this.Config.AnimationDuration);
            }
        }

        this.currentAnimation[0] = this.scene.tweens.add({
            targets: [ctx.BackgroundVideo, ctx.BackgroundImage],
            props: {
                y: { from: 1080, to: 1080 - this.Height, duration: ctx.Config.AnimationDuration, ease: 'Cubic.easeInOut' }
            },
            paused: false,
            yoyo: false,
            duration: ctx.Config.AnimationDuration,
            onComplete: function() {
                ctx.isActive = true;
                ctx.isShowing = false;
                if(ctx.Config.UseImage || ctx.Config.ForceDisplayDurationForVideo)
                    ctx.Stop();
            }
        });
    }
    Stop(): void {
        if (!this.isActive || this.isHiding)
            return;
        this.isActive = false;
        this.isHiding = true;
        var ctx = this;

        let delay = this.Config.UseImage? ctx.Config.DisplayDuration : 0;

        this.currentAnimation[0] = this.scene.tweens.add({
            targets: [ctx.BackgroundVideo, ctx.BackgroundImage],
            props: {
                y: { from: 1080 - this.Height, to: 1080, duration: ctx.Config.AnimationDuration, ease: 'Cubic.easeInOut' }
            },
            paused: false,
            yoyo: false,
            duration: ctx.Config.AnimationDuration,
            delay: delay,
            onComplete: function() {
                ctx.isHiding = false;
                ctx._animationComplete.dispatch();
            }
        });
    }

    CreateTextureListeners(): void {
        //Background Image support
        this.scene.load.once(`filecomplete-image-${this.Type}PopUp`, () => {
            this.BackgroundImage = this.scene.make.sprite({ x: 960, y: 0, key: `${this.Type}PopUp`, add: true });
            this.BackgroundImage.setOrigin(0.5,0);
            this.BackgroundImage.setDepth(3);
            this.BackgroundImage.setPosition(960, 1080 - this.BackgroundImage.displayHeight);
            this.AddVisualComponent(this.BackgroundImage);
            this.LoadMask();
        });

        //Background Video support
        this.scene.load.once(`filecomplete-video-${this.Type}PopUpVideo`, () => {
            if (this.BackgroundVideo !== undefined && this.BackgroundVideo !== null) {
                this.RemoveVisualComponent(this.BackgroundVideo);
                this.BackgroundVideo.destroy();
            }
            // @ts-ignore
            this.BackgroundVideo = this.scene.add.video(960, 0, `${this.Type}PopUpVideo`, false, true);
            this.BackgroundVideo.setOrigin(0.5,0);
            this.BackgroundVideo.setDepth(3);
            this.AddVisualComponent(this.BackgroundVideo);
            this.LoadMask();
        });
    }


    LoadMask(): void {
        //Mask
        if(this.MaskImage !== null && this.MaskImage !== undefined) {
            this.MaskImage.destroy();
            this.MaskImage = null;
        }
        if(this.MaskGeo !== null && this.MaskGeo !== undefined) {
            this.MaskGeo.destroy();
            this.MaskGeo = null;
        }

        if(this.BackgroundVideo !== null && this.BackgroundVideo !== undefined) {
            this.Height = this.BackgroundVideo.displayHeight;
        } else if(this.BackgroundImage !== null && this.BackgroundImage !== undefined) {
            this.Height = this.BackgroundImage.displayHeight;
        } 

        if (this.Config.UseAlpha) {
            this.MaskImage = this.scene.make.sprite({ x: 960, y: 0, key: 'popUpMask', add: false });
            this.MaskImage.setOrigin(0.5,1);
            this.MaskImage.setPosition(960, 1080);
            this.ImgMask = this.MaskImage.createBitmapMask();
        } else {
            this.MaskGeo = this.scene.make.graphics({ add: false });
            this.MaskGeo.fillStyle(0xffffff);
            this.MaskGeo.fillRect(0, 0, 1920, this.Height);
            this.MaskGeo.setPosition(0, 1080 - this.Height);
            this.GeoMask = this.MaskGeo.createGeometryMask();
        }

        this.GetActiveVisualComponents().forEach(vc => {
            vc.setMask(this.Config.UseAlpha ? this.ImgMask : this.GeoMask);
            vc.setPosition(960,1080);
        });
        this._animationComplete.sub(() => {

            if (this.currentAnimation.some(a => (a !== null && a !== undefined) && a.progress !== 1)) {
                return;
            }

            //Reset old Texture
            if (this.scene.textures.exists(`${this.Type}PopUp`)) {
                this.RemoveVisualComponent(this.BackgroundImage);
                this.BackgroundImage?.destroy();
                this.BackgroundImage = null;
                this.scene.textures.remove(`${this.Type}PopUp`);
            }

            //Reset old Video
            if(this.scene.cache.video.has(`${this.Type}PopUpVideo`)) {
                this.RemoveVisualComponent(this.BackgroundVideo);
                this.BackgroundVideo?.destroy(),
                this.BackgroundVideo = null;
                this.scene.cache.video.remove(`${this.Type}PopUpVideo`);
            }

            this.RemoveFromVisualElementList();

            this.AnimationComplete.dispatch();
        });

        this.scene.displayRegions[10].AddToAnimationQueue(this);
    }

}