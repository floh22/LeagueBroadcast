import FrontEndTeam from "./frontEndTeam";

export default class ScoreboardConfig {
    BlueTeam: FrontEndTeam;
    RedTeam: FrontEndTeam;
    GameTime: number;
    SeriesGameCount: number;

    constructor(o: any) {
        this.BlueTeam = new FrontEndTeam(o.BlueTeam);
        this.RedTeam = new FrontEndTeam(o.RedTeam);
        this.GameTime = o.GameTime;
        this.SeriesGameCount = o.SeriesGameCount;
    }
}