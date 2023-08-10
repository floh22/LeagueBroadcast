import { ObjectiveTimerDisplayConfig } from "~/data/config/overlayConfig";
import FrontEndObjective from "~/data/frontEndObjective";
import UpcomingObjective from "~/data/upcomingObjective";
import IngameScene from "~/scenes/IngameScene";
import Vector2 from "~/util/Vector2";
import ObjectivePopUpVisual from "./ObjectivePopUpVisual";
import { VisualElement } from "./VisualElement";

export default class ObjectiveTimerVisual extends VisualElement {

    Type: string = '';
    BackgroundBox: Phaser.GameObjects.Image;
    Icon: Phaser.GameObjects.Image;
    Time: Phaser.GameObjects.Text;
    currentTime: number = -1;
    isTransitioning: boolean = false;

    Config: ObjectiveTimerDisplayConfig;

    constructor(scene: IngameScene, type: string, cfg: ObjectiveTimerDisplayConfig, name: string) {
        super(scene, cfg.Position, name);
        this.Type = type;

        this.Config = cfg;

        this.BackgroundBox = this.scene.add.image(this.position.X, this.position.Y, `${name}Bg`);
        this.BackgroundBox.setScale(cfg.Scale);
        this.visualComponents.push(this.BackgroundBox);

        this.Icon = scene.add.image(this.position.X + cfg.IconPosition.X, this.position.Y + cfg.IconPosition.Y, 'timer_' + type);
        this.Icon.setScale(cfg.Scale);
        this.Icon.setDepth(2);
        this.Icon.setAlpha(0);
        this.visualComponents.push(this.Icon);

        this.Time = scene.add.text(this.position.X + cfg.TimePosition.X, this.position.Y + cfg.TimePosition.Y, '00:00', {
            fontFamily: cfg.TimeFont.Name,
            fontSize: cfg.TimeFont.Size,
            fontStyle: cfg.TimeFont.Style,
            color: cfg.TimeFont.Color
        });
        this.Time.setAlign(cfg.Align);

        this.visualComponents.push(this.Time);


        this.ComponentsToMove().forEach(c => {
            c.setAlpha(0);
        });

        if (cfg.Align === "Left" || cfg.Align === "left") {
            this.Time?.setOrigin(1, 0);

        } else if (cfg.Align === "Right" || cfg.Align === "right") {
            this.Time?.setOrigin(0, 0);
        }

        this.UpdateConfig(cfg);

        this.Init();
    }
    UpdateValues(newValues: UpcomingObjective): void {
        if (newValues === undefined || newValues === null || (newValues.SpawnTimer === 0 && !this.Config.KeepDisplayedWhenAlive)) {
            if (this.isActive) {
                this.Stop();
            }
            return;
        }

        //Transition to alive
        if(this.Config.KeepDisplayedWhenAlive && newValues.SpawnTimer !== 0 && this.currentTime <= 0 && !this.isTransitioning) {
            this.isTransitioning = true;

            let ctx = this;

            if (this.Config.IconAliveLerpDurationSetToZeroToDisable === 0) {
                ctx.Time.setAlpha(1);
                ctx.Icon.setScale(this.Config.IconAliveScale);
                ctx.Icon.setPosition(ctx.position.X + ctx.Config.IconAlivePosition.X, ctx.position.Y + ctx.Config.IconAlivePosition.Y);
            }
            else {
                ctx.scene.tweens.add({
                    targets: ctx.Time,
                    props: {
                        alpha: { value: 1, duration: ctx.Config.IconAliveLerpDurationSetToZeroToDisable / 5, ease: 'Cubic.easeInOut' },
                    },
                    paused: false,
                    yoyo: false,
                    delay: ctx.Config.IconAliveLerpDurationSetToZeroToDisable / 5 * 4,
                });


                ctx.scene.tweens.add({
                    targets: ctx.Icon,
                    props: {
                        x: { value: ctx.position.X + ctx.Config.IconPosition.X, duration: ctx.Config.IconAliveLerpDurationSetToZeroToDisable, ease: 'Cubic.easeInOut' },
                        scale: { value: ctx.Config.Scale, duration: ctx.Config.IconAliveLerpDurationSetToZeroToDisable, ease: 'Cubic.easeInOut' },
                    },
                    paused: false,
                    yoyo: false,
                    onComplete: function () {
                        ctx.isTransitioning = false;
                    }
                });
            }
        }


        //Transition to dead
        if (this.Config.HideTimeIfAlive && newValues.SpawnTimer === 0 && this.currentTime !== 0 && !this.isTransitioning) {
            this.isTransitioning = true;


            let ctx = this;

            if (this.Config.IconAliveLerpDurationSetToZeroToDisable === 0) {
                ctx.Time.setAlpha(0);
                ctx.Icon.setScale(this.Config.IconAliveScale);
                ctx.Icon.setPosition(ctx.position.X + ctx.Config.IconAlivePosition.X, ctx.position.Y + ctx.Config.IconAlivePosition.Y);
            } else {
                ctx.scene.tweens.add({
                    targets: ctx.Time,
                    props: {
                        alpha: { value: 0, duration: ctx.Config.IconAliveLerpDurationSetToZeroToDisable / 5, ease: 'Cubic.easeInOut' },
                    },
                    paused: false,
                    yoyo: false,
                });


                ctx.scene.tweens.add({
                    targets: ctx.Icon,
                    props: {
                        x: { value: ctx.position.X + ctx.Config.IconAlivePosition.X, duration: ctx.Config.IconAliveLerpDurationSetToZeroToDisable, ease: 'Cubic.easeInOut' },
                        scale: { value: ctx.Config.IconAliveScale, duration: ctx.Config.IconAliveLerpDurationSetToZeroToDisable, ease: 'Cubic.easeInOut' },
                    },
                    paused: false,
                    yoyo: false,
                    onComplete: function () {
                        ctx.isTransitioning = false;
                    }
                });
            }
        }

        this.currentTime = newValues.SpawnTimer;


        //Activate if not active during death
        if (!this.isActive) {
            this.Start();
        }

        

        //Update Icon
        if (this.Icon) {
            if (this.Type !== newValues.Element && newValues.Element !== "" && newValues.Element !== undefined && newValues.Element !== null) {
                this.Type = newValues.Element;
                this.Icon.setTexture('timer_' + this.Type);
            }
        }

        //Update Time
        if (this.Time) {
            var timeInSec = Math.round(newValues.SpawnTimer);
            this.Time.text = (Math.floor(timeInSec / 60) >= 10 ? Math.floor(timeInSec / 60) : '0' + Math.floor(timeInSec / 60)) + ':' + (timeInSec % 60 >= 10 ? timeInSec % 60 : '0' + timeInSec % 60);
            if(this.currentTime === 0 && this.Config.HideTimeIfAlive){
                this.Time.setAlpha(0);
            }
        }

    }

    UpdateConfig(newConfig: ObjectiveTimerDisplayConfig): void {

        this.position = newConfig.Position;

        //Update Background
        this.BackgroundBox.setPosition(this.position.X, this.position.Y);


        //Update Icon
        if (this.currentTime === 0 && this.Config.HideTimeIfAlive) {
            this.Icon.setPosition(this.position.X + newConfig.IconAlivePosition.X, this.position.Y + newConfig.IconAlivePosition.Y);
            this.Icon.setScale(newConfig.IconAliveScale);
        } else {
            this.Icon.setPosition(this.position.X + newConfig.IconPosition.X, this.position.Y + newConfig.IconPosition.Y);
            this.Icon.setScale(newConfig.Scale);
        }

        //Update Time
        if(this.currentTime === 0 && this.Config.HideTimeIfAlive){
            this.Time?.setAlpha(0);
        }

        this.Time?.setPosition(this.position.X + newConfig.TimePosition.X, this.position.Y + newConfig.TimePosition.Y);
        this.UpdateTextStyle(this.Time!, newConfig.TimeFont);

        if(!this.Config.KeepDisplayedWhenAlive && this.currentTime === 0){
            this.Stop();
        }

        this.Config = newConfig;
    }
    Load(): void {
    }

    Start(): void {
        if (this.isActive || this.isShowing) {
            return;
        }

        this.isShowing = true;
        var ctx = this;


        this.Icon.setScale(this.Config.Scale);
        this.Icon.setPosition(this.position.X + this.Config.IconPosition.X, this.position.Y + this.Config.IconPosition.Y);

        ctx.scene.tweens.add({
            targets: ctx.GetActiveVisualComponents(),
            props: {
                alpha: { value: 1, duration: 500, ease: 'Cubic.easeInOut' }
            },
            paused: false,
            yoyo: false,
            onComplete: function () { ctx.isShowing = false; ctx.isActive = true; }
        });
    }


    Stop(): void {
        if (!this.isActive || this.isHiding) {
            return;
        }
        var ctx = this;
        this.isHiding = true;
        this.isActive = false;

        var ctx = this;

        ctx.scene.tweens.add({
            targets: ctx.GetActiveVisualComponents(),
            props: {
                alpha: { value: 0, duration: 250, ease: 'Cubic.easeInOut' }
            },
            paused: false,
            yoyo: false,
            onComplete: function () { ctx.isHiding = false; }
        });
    }


    ComponentsToMove(): any[] {
        return [this.BackgroundBox, this.Time];
    }

}