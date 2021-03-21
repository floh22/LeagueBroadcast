import FrontEndObjective from "./frontEndObjective";
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

    constructor(message: any){
        this.dragon = message.dragon;
        this.baron = message.baron;
        this.blueDragons = message.blueDragons;
        this.redDragons = message.redDragons;
        this.gameTime = message.gameTime;
        this.gamePaused = message.gamePaused;
        this.blueGold = message.blueGold;
        this.redGold = message.redGold;
    }
}