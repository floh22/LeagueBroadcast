import IngameScene from "~/scenes/IngameScene";
import Vector2 from "~/util/Vector2";
import { VisualElement } from "./VisualElement";

export default class InhibitorVisual extends VisualElement {

    bg!: Phaser.GameObjects.Rectangle;
    bgImage!: Phaser.GameObjects.Sprite;
    mask: Phaser.GameObjects.Graphics;

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

    constructor(scene: IngameScene) {
        super(scene, new Vector2(0, 0), 'inhibitorIndicator');
    }

    Load(): void {
        throw new Error("Method not implemented.");
    }

    UpdateValues(newValues: any): void {
        throw new Error("Method not implemented.");
    }

    UpdateConfig(newConfig: any): void {
        throw new Error("Method not implemented.");
    }

    Start(): void {
        throw new Error("Method not implemented.");
    }

    Stop(): void {
        throw new Error("Method not implemented.");
    }
}