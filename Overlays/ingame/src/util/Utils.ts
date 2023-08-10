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

    //Load Font by name and url
    static LoadFont(name: string, url: string): Promise<boolean> {
        return new Promise((resolve, reject) => {
            let font = new FontFace(name, `url(${url})`);
            font.load().then(() => {
                document.fonts.add(font);
                resolve(true);
            }).catch((err) => {
                console.log(err);
                reject(false);
            });
        });
    }
}