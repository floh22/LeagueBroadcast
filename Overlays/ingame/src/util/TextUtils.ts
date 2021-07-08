export default class TextUtils {
    static AutoSizeFont(text: Phaser.GameObjects.Text, w: number, h: number, fontSize: number): void {
        if (w > 0 && h > 0) {
            var metrics = text.context.measureText(text.text);
            while (metrics.width > w && fontSize > 4) {
                
                if (text.width > 1.5 * w) {
                    fontSize = Math.ceil(fontSize * (w / metrics.width));
                } else {
                    fontSize--;
                }
                
                //fontSize--;
                text.setFontSize(fontSize);
                metrics = text.context.measureText(text.text);
            }
        }
    };


    static LoadFont(font: any): void {
        if(font && typeof font === "string") {
            //Load single font
            this.LoadFontInternal([font]);
        }
        if(font && Array.isArray(font) && typeof font[0] === "string") {
            //Load multiple fonts
            this.LoadFontInternal(font)
        }
    }

    private static LoadFontInternal(fonts: string[]): void {

    }
}