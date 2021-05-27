import { Vector } from "matter";

export default class Inhibitor {
    id: string;
    key: number;
    timeLeft: number;
    
    constructor(id: string, key: number, timeLeft: number) {
        this.id = id;
        this.key = key;
        this.timeLeft = timeLeft;
    }
}

export class InhibitorInfo {
    Inhibitors: Inhibitor[];
    Location: Vector;

    constructor(Inhibitors: Inhibitor[], Location: Vector) {
        this.Inhibitors = Inhibitors;
        this.Location = Location;
    }
}