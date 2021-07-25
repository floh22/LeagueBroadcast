export default class Utils {
    static ConvertGold(gold: number): string {
        let hundred = Math.round((gold % 1000) / 100);
        let thousand = Math.floor(gold / 1000);
        if (hundred === 10) {
            thousand++;
            hundred = 0;
        }
        return thousand + '.' + hundred + 'k';
    }
}