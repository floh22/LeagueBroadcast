export default class Inhibitor {
    id: string;
    key: number;
    timer: string;
    
    constructor(id: string, key: number, timer: string) {
        this.id = id;
        this.key = key;
        this.timer = timer;
    }
}