import Objective from "./objective";

export default class FrontEndObjective {
    Objective: Objective;
    DurationRemaining: string;
    GoldDifference: number;
    SpawnTimer: number

    constructor(o: Objective, duration: string, goldDiff: number, spawnTimer: number) {
        this.Objective = o;
        this.DurationRemaining = duration;
        this.GoldDifference = Math.round(goldDiff);
        this.SpawnTimer = spawnTimer;
    }
}