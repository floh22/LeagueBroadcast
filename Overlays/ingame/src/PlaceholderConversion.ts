import variables from "./variables";

export default class PlaceholderConversion {

    public static MakeUrlAbsolute(url: string): string {
        if(!url || !url.startsWith('/cache')) {
            return url;
        }

        return `${variables.useSSL? 'https' : 'http'}://${variables.backendUrl}:${variables.backendPort}${url}`;
    }

    public static ConvertItem(itemData: any): any {
        itemData.sprite = this.MakeUrlAbsolute(itemData.sprite);
        return itemData;
    }
}