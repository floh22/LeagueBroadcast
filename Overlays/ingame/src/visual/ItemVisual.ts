import { ItemCompletedDisplayConfig, VisualElementAnimationConfig } from "~/data/config/overlayConfig";
import PlaceholderConversion from "~/PlaceholderConversion";
import IngameScene from "~/scenes/IngameScene";
import TextUtils from "~/util/TextUtils";
import Vector2 from "~/util/Vector2";
import VisualComponent from "./VisualComponent";
import { VisualElement } from "./VisualElement";

export default class ItemVisual extends VisualElement {
    icon!: Phaser.GameObjects.Image;
    infoTextBackground!: Phaser.GameObjects.Image;
    infoText!: Phaser.GameObjects.Text;

    itemData: any;
    name: string;
    playerID: number;
    id: string;

    static width: number = 78;
    static height: number = 74;

    constructor(scene: IngameScene, itemData: any, playerID: number) {
        super(scene, new Vector2(0, 0), `Player_${playerID}`);

        this.playerID = playerID;
        this.itemData = itemData;
        this.name = itemData.name;
        //this.id = itemData.itemID + '_' + playerID;
        this.id = Date.now() + '_' + playerID;

        this.Init();
    }


    UpdateValues(newValues: any): void {
        throw new Error("Method not implemented.");
    }

    UpdateConfig(newConfig: any): void {
        throw new Error("Method not implemented.");
    }

    static GetConfig(): ItemCompletedDisplayConfig {
        return IngameScene.Instance.overlayCfg!.ItemComplete;
    }

    Load(): void {
        this.scene.load.once(`filecomplete-image-${this.id}`, () => {
            var team = this.playerID > 4;
            var x = team ? 1881 : 39;
            var y = team ? 189 + ((this.playerID - 5) * 103) : 189 + (this.playerID * 103);
            this.position = new Vector2(x, y);
            this.scene.displayRegions[this.playerID].AddToAnimationQueue(this);
        });


        this._animationComplete.sub(() => {

            if (this.currentAnimation.some(a => (a !== null && a !== undefined) && a.progress !== 1)) {
                return;
            }

            this.RemoveFromVisualElementList();

            this.AnimationComplete.dispatch();
        });

        this.scene.load.image(this.id, PlaceholderConversion.ConvertItem(this.itemData).sprite);
        this.scene.load.start();
    }

    Start(): void {
        console.log(`Player ${this.playerID} item complete (${this.name})`);

        if(ItemVisual.GetConfig()?.ShowOnChampionIndicator) {
            const itemStart = this.scene.overlayCfg?.ItemComplete.ItemAnimationStates[0];

            this.icon = this.scene.add.image(this.position.X, this.position.Y, this.id);
            this.icon.setMask(this.scene.displayRegions[this.playerID].Masks[0]);
            this.icon.setDisplaySize(ItemVisual.width * itemStart!.Scale, ItemVisual.height * itemStart!.Scale);
            this.icon.setPosition(this.position.X + itemStart!.Position.X * ((itemStart?.Mirror && this.playerID > 4) ? - 1 : 1), this.position.Y + itemStart!.Position.Y);
            this.icon.setAlpha(itemStart!.Alpha);
    
            this.visualComponents.push(this.icon);
    
            this.PlayAnimationState(new VisualComponent(this.icon, new Vector2(ItemVisual.width, ItemVisual.height), true), this.scene.overlayCfg?.ItemComplete.ItemAnimationStates!, 0, 1);
        }

        if (ItemVisual.GetConfig()?.ShowItemName) {
            this.isActive = true;
            this.isShowing = true;
            const textStart = ItemVisual.GetConfig()?.InfoTextAnimationStates[0];
            const textBgStart = ItemVisual.GetConfig()?.InfoBackgroundAnimationStates[0];
            const align = this.playerID > 4 ? 'right' : 'left';

            this.infoTextBackground = this.scene.add.image(this.position.X + textBgStart!.Position.X * ((textStart?.Mirror && this.playerID > 4) ? - 1 : 1), this.position.Y + textBgStart!.Position.Y, 'itemTextBg');
            this.infoTextBackground.setOrigin(this.playerID > 4 ? 1 : 0, 0.5);
            this.infoTextBackground.setAlpha(textBgStart?.Alpha);


            this.infoText = this.scene.add.text(this.position.X + textStart!.Position.X * ((textBgStart?.Mirror && this.playerID > 4) ? - 1 : 1), this.position.Y + textStart!.Position.Y, this.itemData.name, {
                fontFamily: ItemVisual.GetConfig()?.InfoText.Name,
                fontStyle: ItemVisual.GetConfig()!.InfoText.Style,
                fontSize: ItemVisual.GetConfig()!.InfoText.Size,
                align: align

            });
            this.infoText.setOrigin(this.playerID > 4 ? 1 : 0, 0.5);
            this.infoText.setDepth(1);
            this.infoText.setAlpha(textStart?.Alpha);
            TextUtils.AutoSizeFont(this.infoText, this.infoTextBackground.width - textStart!.Position.X - 20, this.infoTextBackground.height, +ItemVisual.GetConfig()!.InfoText.Size.replace(/[^\d.-]/g, ''));

            this.infoText.setMask(this.scene.displayRegions[this.playerID].Masks[1]);
            this.infoTextBackground.setMask(this.scene.displayRegions[this.playerID].Masks[1]);

            this.PlayAnimationState(new VisualComponent(this.infoText, new Vector2(this.infoTextBackground.width, this.infoTextBackground.height), false), this.scene.overlayCfg?.ItemComplete.InfoTextAnimationStates!, 1, 1);
            this.PlayAnimationState(new VisualComponent(this.infoTextBackground, new Vector2(this.infoTextBackground.width, this.infoTextBackground.height), true), this.scene.overlayCfg?.ItemComplete.InfoBackgroundAnimationStates!, 2, 1);
        }
    }

    Stop(): void {
        //There is still an animation playing
        if (this.currentAnimation.some(a => (a !== null && a !== undefined) && a.progress < 1)) {
            return;
        }
        this.currentAnimation.forEach(anim => {
            if ((anim !== null && anim !== undefined) && anim.progress != 1) {
                anim.stop();
            }
        });

        this.isActive = false;
        this.isShowing = false;

        //Destroy this
        this.currentAnimation = [];
        if(this.icon !== null && this.icon !== undefined)
            this.icon.destroy();
        if (this.infoText !== null && this.infoText !== undefined)
            this.infoText.destroy();
        if (this.infoTextBackground !== null && this.infoTextBackground !== undefined)
            this.infoTextBackground.destroy();
        this.scene.textures.remove(this.id + '');
    }

}