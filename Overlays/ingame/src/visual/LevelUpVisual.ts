import { LevelUpDisplayConfig } from "~/data/config/overlayConfig";
import IngameScene from "~/scenes/IngameScene";
import { Dictionary } from "~/util/Dictionary";
import Vector2 from "~/util/Vector2";
import variables from "~/variables";
import ItemVisual from "./ItemVisual";
import VisualComponent from "./VisualComponent";
import { VisualElement } from "./VisualElement";

export default class LevelUpVisual extends VisualElement {

    Team: boolean;
    Level: number;
    PlayerID: number;
    Background!: Phaser.GameObjects.Rectangle;
    BackgroundColor: Phaser.Display.Color;

    Text!: Phaser.GameObjects.Text;
    
    constructor(scene: IngameScene, playerID: number, level: number) {
        super(scene, new Vector2(0,0), `Player_${playerID}`);
        
        if(LevelUpVisual.GetConfig() === null || LevelUpVisual.GetConfig() === undefined) {
            console.log('[LB] Tried showing LevelUp without overlay config');
        }

        this.Team = playerID > 4;
        this.Level = level;
        this.PlayerID = playerID;
        this.position = new Vector2(this.Team? 1881 : 39, this.Team ? 189 + ((playerID - 5) * 103) : 189 + (playerID * 103));

        this.BackgroundColor = Phaser.Display.Color.IntegerToColor(this.Team ? variables.fallbackRed : variables.fallbackBlue);

        if (LevelUpVisual.GetConfig()?.UseTeamColors ) {
            if(IngameScene.Instance.state?.blueColor !== undefined && IngameScene.Instance.state?.blueColor !== '') {
                this.BackgroundColor = Phaser.Display.Color.RGBStringToColor(this.Team ?  IngameScene.Instance.state?.redColor! : IngameScene.Instance.state?.blueColor!);
            } else{
                console.log('[LB] Could not load Team colors');
            }
        } else {
            this.BackgroundColor = Phaser.Display.Color.RGBStringToColor(this.Team ? LevelUpVisual.GetConfig()!.ChaosColor : LevelUpVisual.GetConfig()!.OrderColor);
        }

        this._animationComplete.sub(() => {

            if(this.currentAnimation.some(a => (a !== null && a !== undefined) && a.progress < 1)) {
                return;
            }

            this.RemoveFromVisualElementList();

            this.AnimationComplete.dispatch();
        });

        this.Init();
    }

    static GetConfig(): LevelUpDisplayConfig {
        return IngameScene.Instance.overlayCfg!.LevelUp;
    }

    Load(): void {
        const backgroundStart = LevelUpVisual.GetConfig()?.BackgroundAnimationStates[0];
        const textStart = LevelUpVisual.GetConfig()?.NumberAnimationStates[0];
        const align = this.Team? 'right' : 'left';
        const dirMult = ((textStart?.Mirror && this.PlayerID > 4)? - 1 : 1);


        this.Background = this.scene.add.rectangle(this.position.X + backgroundStart!.Position.X * dirMult, this.position.Y + backgroundStart!.Position.Y, ItemVisual.width, ItemVisual.height, this.BackgroundColor.color, backgroundStart!.Alpha);

        this.Text = this.scene.add.text(this.position.X + textStart!.Position.X * dirMult, this.position.Y + textStart!.Position.Y , '' + this.Level, {
            fontFamily: LevelUpVisual.GetConfig()?.LevelFont.Name,
            fontSize: LevelUpVisual.GetConfig()?.LevelFont.Size,
            fontStyle: LevelUpVisual.GetConfig()!.LevelFont.Style,
            align: align
        });
        this.Text.setOrigin(0.5, 0);
        this.Text.alpha = textStart!.Alpha;
        this.Text.scale = textStart!.Scale;

        this.Background.setMask(this.scene.displayRegions[this.PlayerID].Masks[0]);
        this.Text.setMask(this.scene.displayRegions[this.PlayerID].Masks[0]);

        this.visualComponents.push([this.Background, this.Text]);

        this.scene.displayRegions[this.PlayerID].AddToAnimationQueue(this);
    }

    UpdateValues(newValues: any): void {
        throw new Error("Method not implemented.");
    }

    UpdateConfig(newConfig: any): void {
        throw new Error("Method not implemented.");
    }

    Start(): void {
        console.log(`Player ${this.PlayerID} level up (Lvl. ${this.Level})`);
        this.isShowing = true;
        this.isActive = true;
        this.PlayAnimationState(new VisualComponent(this.Background, new Vector2(this.Background.width, this.Background.height), true), this.scene.overlayCfg?.LevelUp.BackgroundAnimationStates!, 0, 1);
        this.PlayAnimationState(new VisualComponent(this.Text, new Vector2(this.Text.width, this.Text.height), false), this.scene.overlayCfg?.LevelUp.NumberAnimationStates!, 1, 1);
    }

    Stop(): void {
        //There is still an animation playing
        if(this.currentAnimation.some(a => (a !== null && a !== undefined) && a.progress < 1)) {
            return;
        }
        this.currentAnimation.forEach(anim =>  {
            if((anim !== null && anim !== undefined) && anim.progress != 1) {
                anim.stop();
            }
        });
        this.isShowing = false;
        this.isActive = false;

        //Destroy this
        this.currentAnimation = [];
        this.Background.destroy();
        this.Text.destroy();
    }
}