import Objective from "./objective";

export default class FrontEndObjective {
    Objective: Objective;
    DurationRemaining: string;
    GoldDifference: number;

    constructor(o: Objective, duration: string, goldDiff: number) {
        this.Objective = o;
        this.DurationRemaining = duration;
        this.GoldDifference = goldDiff;
    }
}