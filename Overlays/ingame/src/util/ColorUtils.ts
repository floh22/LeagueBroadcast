export default class ColorUtils {
    static GetRGBAString(color, alpha):string {
        if (alpha === undefined) {
            alpha = color.alphaGL;
        }
        return 'rgba(' + color.red + ',' + color.green + ',' + color.blue + ',' + alpha + ')';
    }
}