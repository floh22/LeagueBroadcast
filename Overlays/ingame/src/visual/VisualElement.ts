import { SignalDispatcher } from "strongly-typed-events";
import { VisualElementAnimationConfig } from "~/data/config/overlayConfig";
import IngameScene from "~/scenes/IngameScene";
import Vector2 from "~/util/Vector2";
import VisualComponent from "./VisualComponent";

export abstract class VisualElement {

    key: string;
    visualID: number = 0;
    scene: IngameScene;
    isActive = false;
    isHiding: boolean = false;
    isShowing: boolean = false;
    visualUpdateQueued: boolean = false;
    position: Vector2 = new Vector2(0,0);
    currentAnimation: Phaser.Tweens.Tween[] = [];

    visualComponents: any[] = [];

    AnimationStart: SignalDispatcher = new SignalDispatcher();
    AnimationComplete: SignalDispatcher = new SignalDispatcher();
    protected _animationComplete: SignalDispatcher = new SignalDispatcher();
    
    constructor(scene: IngameScene, position: Vector2, key: string) {
        this.scene = scene;
        this.position = position;
        this.key = key;
        
    }

    protected Init(): void {

        //Assign and increment unique visual ids, add to list
        this.visualID = this.scene.visualIDs++;
        this.scene.visualIDs = this.visualID;
        this.scene.currentVisualElements[this.visualID] = this;

        this.Load();
        return;
    }

    abstract UpdateValues(newValues: any): void;
    
    abstract UpdateConfig(newConfig: any): void;

    abstract Load(): void;

    abstract Start(): void;

    abstract Stop(): void;

    Show(): void {

    };

    Hide(): void {

    };

    PlayAnimationState(visualComponent: VisualComponent, states: VisualElementAnimationConfig[], animationNum: number, stateNum: number): void {
        const isFirst = stateNum === 1;
        const isOver = states.length === stateNum;

        if(isFirst) {
            this.AnimationStart.dispatch();
        }
        
        //Stop Animation when end of animation list is reached
        if (isOver) {
            this.Stop();
            this._animationComplete.dispatch();
            return;
        }

        const animation = states[stateNum];
        const ctx = this;

        //Stop any previous animation incase any were playing for some reason
        //to make sure elements transition smoothly and end up where they should
        if ((this.currentAnimation[animationNum] !== undefined && this.currentAnimation[animationNum] !== null) && this.currentAnimation[animationNum].progress != 1) {
            this.currentAnimation[animationNum].stop;
        }

        //Mirror Animation
        var dirX = animation!.Position.X;
        if (animation.Mirror && this.position.X > 960) {
            dirX *= -1;
        }

        //Do not scale if tagged to not scale
        var props = {};
        if (visualComponent.AnimateScale) {
            props = {
                x: { value: ctx.position.X + dirX, duration: animation!.Duration, ease: animation!.Ease },
                y: { value: ctx.position.Y + animation!.Position.Y, duration: animation!.Duration, ease: animation!.Ease },
                displayWidth: { value: visualComponent.Size.X * animation!.Scale, duration: animation!.Duration, ease: animation!.Ease },
                displayHeight: { value: visualComponent.Size.Y * animation!.Scale, duration: animation!.Duration, ease: animation!.Ease },
                alpha: { value: animation!.Alpha, duration: animation!.Duration, ease: animation!.Ease }
            };
        } else {
            props = {
                x: { value: ctx.position.X + dirX, duration: animation!.Duration, ease: animation!.Ease },
                y: { value: ctx.position.Y + animation!.Position.Y, duration: animation!.Duration, ease: animation!.Ease },
                alpha: { value: animation!.Alpha, duration: animation!.Duration, ease: animation!.Ease }
            }
        }

        //Create animation
        this.currentAnimation[animationNum] = this.scene.tweens.add({
            targets: visualComponent.Component,
            props: props,
            paused: false,
            yoyo: false,
            delay: animation!.Delay,
            duration: animation!.Duration,
            onComplete: function () {
                ctx.PlayAnimationState(visualComponent, states, animationNum, stateNum + 1);
            }
          });
    }

    RemoveFromVisualElementList(): void {
        this.scene.currentVisualElements.forEach((item, index) => {
            if(item.visualID === this.visualID) {
                this.scene.currentVisualElements.splice(index, 1);
            }
        })
    }


    RemoveVisualComponent(component: any): void {
        this.visualComponents = this.visualComponents.filter(c => c !== component);
    }

    AddVisualComponent(component: any): void {
        const componentInList = this.visualComponents.find(c => c === component);
        if(componentInList === undefined) {
            this.visualComponents.push(component);
        }
    }

    GetActiveVisualComponents(): any[] {
        return this.visualComponents.filter(c => c !== null && c !== undefined);
    }


    UpdateTextStyle(textElement: Phaser.GameObjects.Text, style: {Name: string, Size: string, Align: string, Color: string, Style: string}): void {

        textElement.setFontFamily(style.Name);
        //textElement.setFontSize(+style.Size.replace('/[-]{0,1}[\d]*[.]{0,1}[\d]+/g', ''));
        textElement.setFontStyle(style.Style);
        //@ts-ignore
        textElement.setFontSize(style.Size)
        textElement.setColor(style.Color);
        textElement.setAlign(style.Align);
    }

}