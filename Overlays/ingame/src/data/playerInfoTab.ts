import ValueBar from "./ValueBar";

export default class PlayerInfoTab {
    PlayerName: string;
    IconPath: string;
    Values: ValueBar;
    ExtraInfo: string[];

    constructor(o: any) {
        this.PlayerName = o.PlayerName;
        this.IconPath = o.IconPath;
        this.Values = o.Values;
        this.ExtraInfo = o.ExtraInfo;
    }
}