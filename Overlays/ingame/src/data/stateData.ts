import FrontEndObjective from "./frontEndObjective";
import GoldEntry from "./goldEntry";
import InfoSidePage from "./infoSidePage";
import Inhibitor, { InhibitorInfo } from "./inhibitor";
import Objective from "./objective";
import ScoreboardConfig from "./scoreboardConfig";

export default class StateData {
    dragon: FrontEndObjective;
    baron: FrontEndObjective;

    gameTime: number;
    gamePaused: boolean;

    blueGold: number;
    redGold: number;

    goldGraph: GoldEntry[];
    inhibitors: InhibitorInfo;

    scoreboard: ScoreboardConfig;
    infoPage: InfoSidePage;

    blueColor: string;
    redColor: string;

    constructor(message: any){
        this.dragon = message.dragon;
        this.baron = message.baron;
        this.gameTime = message.gameTime;
        this.gamePaused = message.gamePaused;
        this.blueGold = message.blueGold;
        this.redGold = message.redGold;
        this.goldGraph = [];
        if(message.goldGraph != undefined && message.goldGraph != null) {
            for(const [key,value] of Object.entries(message.goldGraph)) {
                this.goldGraph.push(new GoldEntry(+key, value as number ))
            }
        }
        this.inhibitors = message.inhibitors;
        this.scoreboard = new ScoreboardConfig(message.scoreboard);
        this.infoPage = new InfoSidePage(message.infoPage);

        this.blueColor = message.blueColor;
        this.redColor = message.redColor;
    }
}