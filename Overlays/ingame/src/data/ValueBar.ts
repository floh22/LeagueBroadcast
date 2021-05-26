export default class ValueBar {
    MinValue: number;
    MaxValue: number;
    CurrentValue: number;

    constructor(o: any) {
        this.MaxValue = o.MaxValue;
        this.MinValue = o.MinValue;
        this.CurrentValue = o.CurrentValue;
    }
}