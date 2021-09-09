import { InhibitorDisplayConfig, ObjectiveTimerDisplayConfig } from "~/data/config/overlayConfig";
import FrontEndObjective from "~/data/frontEndObjective";
import IngameScene from "~/scenes/IngameScene";
import Vector2 from "~/util/Vector2";
import ObjectivePopUpVisual from "./ObjectivePopUpVisual";
import { VisualElement } from "./VisualElement";

export default class ObjectiveTimerVisual extends VisualElement {

    Type: string = '';
    BackgroundBox: Phaser.GameObjects.Image;
    Icon: Phaser.GameObjects.Image | null = null;
    Time: Phaser.GameObjects.Text | null = null;
    TimeIcon: Phaser.GameObjects.Image | null = null;
    Gold: Phaser.GameObjects.Text | null = null;
    GoldIcon: Phaser.GameObjects.Image | null = null;
    Mask: Phaser.Display.Masks.GeometryMask;
    MaskG: Phaser.GameObjects.Graphics;

    Config: ObjectiveTimerDisplayConfig;

    constructor(scene: IngameScene, type: string, cfg: ObjectiveTimerDisplayConfig) {
        super(scene, cfg.Position, `${type}Timer`);
        this.Type = type;

        this.Config = cfg;

        this.MaskG = scene.make.graphics({}, false);
        this.MaskG.fillStyle(0xffffff);
        this.MaskG.fillRect(cfg.MaskPosition.X, cfg.MaskPosition.Y, cfg.MaskSize.X, cfg.MaskSize.Y);
        this.Mask = this.MaskG.createGeometryMask();

        this.BackgroundBox = this.scene.add.image(this.position.X, this.position.Y, type === 'baron'? 'objectiveBg' : 'objectiveBgLeft');
        this.BackgroundBox.setScale(cfg.Scale);
        this.visualComponents.push(this.BackgroundBox);

        if (cfg.ObjectiveIcon) {
            this.Icon = scene.add.image(this.position.X + cfg.IconPosition.X, this.position.Y + cfg.IconPosition.Y, type + 'Icon');
            this.Icon.setScale(cfg.Scale);
            this.Icon.setDepth(2);
            this.Icon.setAlpha(0);
            this.visualComponents.push(this.Icon);
        }

        if (cfg.ShowGoldDiff) {
            this.GoldIcon = scene.add.image(this.position.X + cfg.GoldIconPosition.X, this.position.Y + cfg.GoldIconPosition.Y, 'objectiveGold');
            this.Gold = scene.add.text(this.position.X + cfg.GoldPosition.X, this.position.Y + cfg.GoldPosition.Y, '+-0', {
                fontFamily: cfg.GoldFont.Name,
                fontSize: cfg.GoldFont.Size,
                fontStyle: cfg.GoldFont.Style,
                color: cfg.GoldFont.Color
            });

            this.Gold.setAlign(cfg.Align);
            this.GoldIcon.setOrigin(0,0);

            this.visualComponents.push(this.Gold, this.GoldIcon);
        }

        if (cfg.ShowTimer) {
            this.TimeIcon = scene.add.image(this.position.X + cfg.TimeIconPosition.X, this.position.Y + cfg.TimeIconPosition.Y, 'objectiveCdr');
            this.Time = scene.add.text(this.position.X + cfg.TimePosition.X, this.position.Y + cfg.TimePosition.Y, '00:00', {
                fontFamily: cfg.TimeFont.Name,
                fontSize: cfg.TimeFont.Size,
                fontStyle: cfg.TimeFont.Style,
                color: cfg.TimeFont.Color
            });
            this.Time.setAlign(cfg.Align);
            this.TimeIcon.setOrigin(0,0);

            this.visualComponents.push(this.Time, this.TimeIcon);
        }

        this.ComponentsToMove().forEach(c => {
            c.setMask(this.Mask);
            c.setAlpha(0);
        });

        if (cfg.Align === "Left" || cfg.Align === "left") {
            this.Gold?.setOrigin(1,0);
            this.Time?.setOrigin(1,0);

        } else if (cfg.Align === "Right" || cfg.Align === "right") {
            this.Gold?.setOrigin(0,0);
            this.Time?.setOrigin(0,0);
        }

        this.Init();
    }
    UpdateValues(newValues: FrontEndObjective): void {
        if (newValues === undefined || newValues === null) {
            if (this.isActive) {
                this.Stop();
            }
            return;
        }
        if (!this.isActive) {
            this.Start();
        }

        if (this.Gold !== null)
            this.Gold.text = Math.trunc(newValues.GoldDifference) + '';
        if (this.Time !== null)
            this.Time.text = newValues.DurationRemaining;
    }

    UpdateConfig(newConfig: ObjectiveTimerDisplayConfig): void {

        //Update Background
        this.BackgroundBox.setPosition(this.position.X, this.position.Y);

        this.MaskG.clear();
        this.MaskG.fillStyle(0xffffff);
        this.MaskG.fillRect(newConfig.MaskPosition.X, newConfig.MaskPosition.Y, newConfig.MaskSize.X, newConfig.MaskSize.Y);


        //Update Icon
        if(newConfig.ObjectiveIcon) {
            if(this.Config?.ObjectiveIcon) {
                this.Icon?.setPosition(this.position.X + newConfig.IconPosition.X, this.position.Y + newConfig.IconPosition.Y);
            } else {
                this.Icon = this.scene.add.image(this.position.X + newConfig.IconPosition.X, this.position.Y + newConfig.IconPosition.Y, this.Type + 'Icon');
                this.Icon.setScale(newConfig.Scale);
                this.visualComponents.push(this.Icon);
            }
                
        } else if(this.Config?.ObjectiveIcon) {
            this.RemoveVisualComponent(this.Icon);
            this.Icon?.destroy();
        }

        //Update Gold
        if (newConfig.ShowGoldDiff) {
            if (this.Config?.ShowGoldDiff) {
                this.Gold?.setPosition(this.position.X + newConfig.GoldPosition.X, this.position.Y + newConfig.GoldPosition.Y);
                this.UpdateTextStyle(this.Gold!, newConfig.GoldFont);
                this.GoldIcon?.setPosition(this.position.X + newConfig.GoldIconPosition.X, this.position.Y + newConfig.GoldIconPosition.Y);
            } else {
                this.GoldIcon = this.scene.add.image(this.position.X + newConfig.GoldIconPosition.X, this.position.Y + newConfig.GoldIconPosition.Y, 'objectiveGold');
                this.Gold = this.scene.add.text(this.position.X + newConfig.GoldPosition.X, this.position.Y + newConfig.GoldPosition.Y, '+-0', {
                    fontFamily: newConfig.GoldFont.Name,
                    fontSize: newConfig.GoldFont.Size,
                    fontStyle: newConfig.GoldFont.Style,
                    color: newConfig.GoldFont.Color
                });

                this.Gold.setAlign(newConfig.Align);
                this.GoldIcon.setOrigin(0, 0);

                this.visualComponents.push(this.Gold, this.GoldIcon);
            }
        } else if (this.Config?.ShowGoldDiff) {
            this.RemoveVisualComponent(this.Gold);
            this.RemoveVisualComponent(this.GoldIcon);
            this.GoldIcon?.destroy();
            this.Gold?.destroy();
        }

        //Update Time
        if (newConfig.ShowTimer) {
            if (this.Config?.ShowTimer) {
                this.Time?.setPosition(this.position.X + newConfig.TimePosition.X, this.position.Y + newConfig.TimePosition.Y);
                this.UpdateTextStyle(this.Time!, newConfig.TimeFont);
                this.TimeIcon?.setPosition(this.position.X + newConfig.TimeIconPosition.X, this.position.Y + newConfig.TimeIconPosition.Y);
            } else {
                this.TimeIcon = this.scene.add.image(this.position.X + newConfig.TimeIconPosition.X, this.position.Y + newConfig.TimeIconPosition.Y, 'objectiveCdr');
                this.Time = this.scene.add.text(this.position.X + newConfig.TimePosition.X, this.position.Y + newConfig.TimePosition.Y, '00:00', {
                    fontFamily: newConfig.TimeFont.Name,
                    fontSize: newConfig.TimeFont.Size,
                    fontStyle: newConfig.TimeFont.Style,
                    color: newConfig.TimeFont.Color
                });
                this.Time.setAlign(newConfig.Align);
                this.TimeIcon.setOrigin(0, 0);

                this.visualComponents.push(this.Time, this.TimeIcon);
            }
        } else if(this.Config?.ShowTimer) {
            this.RemoveVisualComponent(this.Time);
            this.RemoveVisualComponent(this.TimeIcon);
            this.Time?.destroy();
            this.TimeIcon?.destroy();
        }

        this.Config = newConfig;
    }
    Load(): void {
        this.InitPositions(this.Config!);
        //this.Start();
    }

    Start(): void {
        if (this.isActive || this.isShowing) {
            return;
        }

        this.isShowing = true;
        var ctx = this;

        if(!this.Config.Animate) {
            ctx.scene.tweens.add({
                targets: ctx.GetActiveVisualComponents(),
                props: {
                    alpha: { value: 1, duration: 500, ease: 'Cubic.easeInOut' }
                },
                paused: false,
                yoyo: false,
                onComplete: function() { ctx.isShowing = false; ctx.isActive = true;}
            });
            return;
        }
        
        this.ComponentsToMove().forEach(c => c.alpha = 1);

        if(this.Config?.ObjectiveIcon) {
            this.scene.tweens.add({
                targets: ctx.Icon,
                alpha: 1,
                paused: false,
                yoyo: false,
                duration: 250,
                onComplete: function () {
                    ctx.scene.tweens.add({
                        targets: ctx.ComponentsToMove(),
                        props: {
                            x: { value: '-=' + 200 * (ctx.Config?.Align === "Right" || ctx.Config?.Align === "right" ? -1 : 1), duration: 1000, ease: 'Cubic.easeOut' }
                        },
                        paused: false,
                        yoyo: false
                    });
                    ctx.isShowing = false;
                    ctx.isActive = true;
                }
            });
        } else {
            ctx.scene.tweens.add({
                targets: ctx.ComponentsToMove(),
                props: {
                    x: { value: '-=' + 200 * (ctx.Config?.Align === "Right" || ctx.Config?.Align === "right" ? -1 : 1), duration: 1000, ease: 'Cubic.easeOut' }
                },
                paused: false,
                yoyo: false,
                onComplete: function() { ctx.isShowing = false; ctx.isActive = true;}
            });
        }
    }


    Stop(): void {
        if (!this.isActive || this.isHiding) {
            return;
        }
        var ctx = this;
        this.isHiding = true;
        this.isActive = false;

        var ctx = this;

        if(!this.Config.Animate) {
            ctx.scene.tweens.add({
                targets: ctx.GetActiveVisualComponents(),
                props: {
                    alpha: { value: 0, duration: 250, ease: 'Cubic.easeInOut' }
                },
                paused: false,
                yoyo: false,
                onComplete: function() { ctx.isHiding = false;}
            });
            return;
        }

        if(this.Config?.ObjectiveIcon) {
            this.scene.tweens.add({
                targets: ctx.Icon,
                alpha: 0,
                paused: false,
                yoyo: false,
                duration: 200,
                delay: 300,
                onComplete: function () { }
            });
        }

        this.scene.tweens.add({
            targets: ctx.ComponentsToMove(),
            props: {
                x: { value: '+=' + 200 * (ctx.Config?.Align === "Right" || ctx.Config?.Align === "right" ? -1 : 1), duration: 300, ease: 'Cubic.easeOut' }
            },
            paused: false,
            yoyo: false,
            onComplete: function () { ctx.isHiding = false; }
        });
    }


    ComponentsToMove(): any[] {
        var components: any[] = [];

        if (this.Config?.ObjectiveIcon)
            components.push(this.BackgroundBox);

        if (this.Config?.ShowTimer) {
            components.push(this.Time!, this.TimeIcon!);
        }

        if (this.Config?.ShowGoldDiff) {
            components.push(this.GoldIcon!, this.Gold!);
        }

        return components;
    }

    InitPositions(cfg : ObjectiveTimerDisplayConfig): void {

        if(!cfg.Animate) {
            return;
        }
        this.BackgroundBox.x += 200 * (cfg.Align === "Right" || cfg.Align === "right" ? -1 : 1);
        if (cfg.ShowGoldDiff) {
            this.GoldIcon!.x += 200 * (cfg.Align === "Right" || cfg.Align === "right" ? -1 : 1);
            this.Gold!.x += 200 * (cfg.Align === "Right" || cfg.Align === "right" ? -1 : 1);
        }
        if (cfg.ShowTimer) {
            this.TimeIcon!.x += 200 * (cfg.Align === "Right" || cfg.Align === "right" ? -1 : 1);
            this.Time!.x += 200 * (cfg.Align === "Right" || cfg.Align === "right" ? -1 : 1);
        }
    }

}