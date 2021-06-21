export default class Vector2 {
    public static add(a: Vector2, b: Vector2): Vector2 {
        return new Vector2(a.X + b.X, b.Y + b.Y);
    }

    public static mul(a: Vector2, b: Vector2): Vector2 {
        return new Vector2(a.X * b.X, b.Y * b.Y);
    }

    public static dot(a: Vector2, b: Vector2): Vector2 {
        return new Vector2(a.X * b.X + b.Y * b.Y);
    }

    public X: number;
    public Y: number;

    constructor(x: number = 0, y: number = 0) {
        this.X = x;
        this.Y = y;
    }

    public get magnitude(): number {
        return Math.sqrt(this.sqrMagnitude);
    }

    public get sqrMagnitude(): number {
        return this.X * this.X + this.Y * this.Y;
    }

    public normalize() {
        const len = this.magnitude;
        if (len > 0) {
            this.X /= len;
            this.Y /= len;
        }
    }

    public inverse() {
        this.X = -this.X;
        this.Y = -this.Y;
    }

    public get normalized(): Vector2 {
        const v = new Vector2(this.X, this.Y);
        const len = this.magnitude;
        if (len > 0) {
            v.X /= len;
            v.Y /= len;
        }
        return v;
    }

    public get inversed(): Vector2 {
        return new Vector2(-this.X, -this.Y);
    }
}