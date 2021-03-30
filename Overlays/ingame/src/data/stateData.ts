import FrontEndObjective from "./frontEndObjective";
import GoldEntry from "./goldEntry";
import Objective from "./objective";

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
    }
}