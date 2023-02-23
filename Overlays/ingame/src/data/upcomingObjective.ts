export default class UpcomingObjective {
    public Element: string;
    public SpawnTimer: number;

    constructor(element: string, spawnTimer: number) {
        this.Element = element;
        this.SpawnTimer = spawnTimer; 
    }
}