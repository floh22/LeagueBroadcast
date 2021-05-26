export default class FrontEndTeam {
    Color: string;
    Name: string;
    Icon: string;
    Score: number;

    Kills: number;
    Towers: number;
    Gold: number;
    Dragons: string[];

    constructor(o:any) {
        this.Color = o.Color;
        this.Name = o.Name;
        this.Icon = o.Icon;
        this.Score = o.Score;

        this.Kills = o.Kills;
        this.Towers = o.Towers;
        this.Gold = o.Gold;
        this.Dragons = o.Dragons;
    }
}