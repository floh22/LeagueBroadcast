import PlayerInfoTab from "./playerInfoTab";

export default class InfoSidePage {
    Title!: string;
    Order!: PlayerOrder;
    Players!: PlayerInfoTab[];

    constructor(o: any) {
        if(o === undefined)
            return;
        this.Title = o.Title;
        this.Order = PlayerOrder[o.Order as keyof typeof PlayerOrder];
        this.Players = o.Players;
    }
}

enum PlayerOrder {
    MaxToMin,
    MinToMax,
    BlueFirst,
    RedFirst
}
