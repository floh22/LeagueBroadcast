import Queue from "~/util/Queue";
import { VisualElement } from "~/visual/VisualElement";

export default class RegionMask {
    ID: number;
    Masks: Phaser.Display.Masks.BitmapMask[];
    AnimationQueue: Queue<VisualElement>;
    CurrentAnimation: VisualElement | null;

    constructor(ID: number, Masks: Phaser.Display.Masks.BitmapMask[]) {
        this.ID = ID;
        this.Masks = Masks;
        this.AnimationQueue = new Queue();
        this.CurrentAnimation = null;
    }


    AddToAnimationQueue(ve: VisualElement): void {
        //Nothing in Queue, play directly
        if(this.CurrentAnimation === null || this.CurrentAnimation === undefined) {
            this.CurrentAnimation = ve;
            this.CurrentAnimation.AnimationComplete.sub(() => this.PlayNextAnimation());
            ve.Start();
            return;
        }

        //Add animation to queue
        this.AnimationQueue.enqueue(ve);
    }


    protected PlayNextAnimation(): void {
        if(this.AnimationQueue.length === 0) {
            this.CurrentAnimation = null;
            return;
        }

        this.CurrentAnimation = this.AnimationQueue.dequeue();
        this.CurrentAnimation.AnimationComplete.sub(() => this.PlayNextAnimation());
        this.CurrentAnimation.Start();
    }
}