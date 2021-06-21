import Vector2 from "~/util/Vector2";

export default class VisualComponent {
    Component: any;
    Size: Vector2;
    AnimateScale: boolean;

    constructor(Component: any, Size: Vector2, AnimateScale: boolean) {
        this.Component = Component;
        this.Size = Size;
        this.AnimateScale = AnimateScale;
    }
}