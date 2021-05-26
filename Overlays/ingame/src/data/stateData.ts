import FrontEndObjective from "./frontEndObjective";
import GoldEntry from "./goldEntry";
import InfoSidePage from "./infoSidePage";
import Inhibitor from "./inhibitor";
import Objective from "./objective";
import ScoreboardConfig from "./scoreboardConfig";

export default class StateData {
    dragon: FrontEndObjective;
    baron: FrontEndObjective;

    blueDragons: string[];
    redDragons: string[];

    gameTime: number;
    gamePaused: boolean;

    blueGold: number;
    redGold: number;

    goldGraph: GoldEntry[];
    inhibitors: Inhibitor[];

    scoreboard: ScoreboardConfig;
    infoPage: InfoSidePage;

    constructor(message: any){
        this.dragon = message.dragon;
        this.baron = message.baron;
        this.blueDragons = message.blueDragons;
        this.redDragons = message.redDragons;
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
        this.inhibitors = [];
        if(message.inhibitors != undefined && message.inhibitors != null) {
            for(const Inhib of Object.entries(message.inhibitors)) {
                this.inhibitors.push(Inhib as unknown as Inhibitor);
            }
        }
        this.scoreboard = new ScoreboardConfig(message.scoreboard);
        this.infoPage = new InfoSidePage(message.infoPage);
    }
}