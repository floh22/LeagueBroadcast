import { InhibitorDisplayConfig, PowerPlayDisplayConfig as PowerPlayTimerDisplayConfig } from "~/data/config/overlayConfig";
import FrontEndObjective from "~/data/frontEndObjective";
import IngameScene from "~/scenes/IngameScene";
import Vector2 from "~/util/Vector2";
import ObjectivePopUpVisual from "./ObjectivePopUpVisual";
import { VisualElement } from "./VisualElement";

export default class PowerPlayVisual extends VisualElement {

    Type: string = '';
    BackgroundBox: Phaser.GameObjects.Image;
    Time: Phaser.GameObjects.Text | null = null;
    Gold: Phaser.GameObjects.Text | null = null;

    Config: PowerPlayTimerDisplayConfig;

    constructor(scene: IngameScene, type: string, cfg: PowerPlayTimerDisplayConfig) {
        super(scene, cfg.Position, `${type}PowerPlay`);
        this.Type = type;

        this.Config = cfg;

        this.BackgroundBox = this.scene.add.image(this.position.X, this.position.Y, type === 'baron' ? 'powerPlayRight' : 'powerPlayLeft');
        this.BackgroundBox.setScale(cfg.Scale);
        this.BackgroundBox.setDepth(3);
        this.visualComponents.push(this.BackgroundBox);

        if (cfg.ShowGoldDiff) {
            this.Gold = scene.add.text(this.position.X + cfg.GoldPosition.X, this.position.Y + cfg.GoldPosition.Y, '+-0', {
                fontFamily: cfg.GoldFont.Name,
                fontSize: cfg.GoldFont.Size,
                fontStyle: cfg.GoldFont.Style,
                color: cfg.GoldFont.Color
            });

            this.Gold.setAlign(cfg.Align);

            this.Gold.setDepth(4);

            this.visualComponents.push(this.Gold);
        }

        if (cfg.ShowTimer) {
            this.Time = scene.add.text(this.position.X + cfg.TimePosition.X, this.position.Y + cfg.TimePosition.Y, '00:00', {
                fontFamily: cfg.TimeFont.Name,
                fontSize: cfg.TimeFont.Size,
                fontStyle: cfg.TimeFont.Style,
                color: cfg.TimeFont.Color
            });
            this.Time.setAlign(cfg.Align);

            this.Time.setDepth(4);

            this.visualComponents.push(this.Time);
        }

        if (cfg.Align === "Left" || cfg.Align === "left") {
            this.Gold?.setOrigin(1, 0);
            this.Time?.setOrigin(1, 0);

        } else if (cfg.Align === "Right" || cfg.Align === "right") {
            this.Gold?.setOrigin(0, 0);
            this.Time?.setOrigin(0, 0);
        }


        this.visualComponents.forEach(element => {
            element.setAlpha(0);
        });
    }
    UpdateValues(newValues: FrontEndObjective): void {
        /*
        if(!this.isActive) {
            this.Start();
        }

        if(this.Gold !== null)
            this.Gold.text = 1000 + "";

        if(this.Time !== null)
        this.Time.text = "01:25";
    */

        if (newValues === undefined || newValues === null) {
            if (this.isActive) {
                this.Stop();
            }
            return;
        }
        if (!this.isActive) {
            console.log("activating");
            this.Start();
        }

        if (this.Gold !== null)
            this.Gold.text = Math.trunc(newValues.GoldDifference) + '';
        if (this.Time !== null)
            this.Time.text = newValues.DurationRemaining;

    }

    UpdateConfig(newConfig: PowerPlayTimerDisplayConfig): void {

        //Update Background
        this.BackgroundBox.setPosition(this.position.X, this.position.Y);


        //Update Gold
        if (newConfig.ShowGoldDiff) {
            if (this.Config?.ShowGoldDiff) {
                this.Gold?.setPosition(this.position.X + newConfig.GoldPosition.X, this.position.Y + newConfig.GoldPosition.Y);
                this.UpdateTextStyle(this.Gold!, newConfig.GoldFont);
            
            } else {
                this.Gold = this.scene.add.text(this.position.X + newConfig.GoldPosition.X, this.position.Y + newConfig.GoldPosition.Y, '+-0', {
                    fontFamily: newConfig.GoldFont.Name,
                    fontSize: newConfig.GoldFont.Size,
                    fontStyle: newConfig.GoldFont.Style,
                    color: newConfig.GoldFont.Color
                });

                this.Gold.setAlign(newConfig.Align);

                this.Gold.setDepth(4);

                this.visualComponents.push(this.Gold);
            }
        } else if (this.Config?.ShowGoldDiff) {
            this.RemoveVisualComponent(this.Gold);
            this.Gold?.destroy();
        }

        //Update Time
        if (newConfig.ShowTimer) {
            if (this.Config?.ShowTimer) {
                this.Time?.setPosition(this.position.X + newConfig.TimePosition.X, this.position.Y + newConfig.TimePosition.Y);
                this.UpdateTextStyle(this.Time!, newConfig.TimeFont);
            } else {
                this.Time = this.scene.add.text(this.position.X + newConfig.TimePosition.X, this.position.Y + newConfig.TimePosition.Y, '00:00', {
                    fontFamily: newConfig.TimeFont.Name,
                    fontSize: newConfig.TimeFont.Size,
                    fontStyle: newConfig.TimeFont.Style,
                    color: newConfig.TimeFont.Color
                });
                this.Time.setAlign(newConfig.Align);

                this.Time.setDepth(4);

                this.visualComponents.push(this.Time);
            }
        } else if (this.Config?.ShowTimer) {
            this.RemoveVisualComponent(this.Time);
            this.Time?.destroy();
        }

        this.Config = newConfig;

        if(!this.isActive) {
            this.visualComponents.forEach(element => {
                element.setAlpha(0);
            });
        }
    }
    Load(): void {
    }

    Start(): void {
        if (this.isActive || this.isShowing) {
            return;
        }

        if (this.Type === 'elder') {
            this.scene.dragonTimer.Stop();
        }

        this.isShowing = true;
        var ctx = this;

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
            onComplete: function () {
                ctx.isHiding = false;
                if (ctx.Type === 'elder' && !ctx.scene.dragonTimer.Config.HideTimeIfAlive) {
                    ctx.scene.dragonTimer.Start();
                }
            }
        });
    }

}