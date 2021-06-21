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
}