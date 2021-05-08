import { Time } from "phaser";

export default class Objective {
    Cooldown: number;
    IsAlive: boolean;
    TimesTakenInMatch: number;
    LastTakenBy: number;
    Type: string;

    constructor(Cooldown: number, IsAlive: boolean, TimesTakenInMatch: number, LastTakenBy: number, Type: string) {
        this.Cooldown = Cooldown;
        this.IsAlive = IsAlive;
        this.TimesTakenInMatch = TimesTakenInMatch;
        this.LastTakenBy = LastTakenBy;
        this.Type = Type;
    }
}