export default class Vector4 {
    public static add(a: Vector4, b: Vector4): Vector4 {
        return new Vector4(a.w + b.w, a.x + b.x, b.y + b.y, a.z + b.z);
    }

    public static mul(a: Vector4, b: Vector4): Vector4 {
        return new Vector4(a.w * b.w, a.x * b.x, b.y * b.y, a.z * b.z);
    }

    public w: number;
    public x: number;
    public y: number;
    public z: number;

    constructor(w: number = 0, x: number = 0, y: number = 0, z: number = 0) {
        this.w = w;
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public get magnitude(): number {
        return Math.sqrt(this.sqrMagnitude);
    }

    public get sqrMagnitude(): number {
        return this.w * this.w * this.x * this.x + this.y * this.y + this.z * this.z;
    }

    public normalize() {
        const len = this.magnitude;
        if (len > 0) {
            this.w /= len;
            this.x /= len;
            this.y /= len;
            this.z /= len;
        }
    }

    public inverse() {
        this.w = -this.w;
        this.x = -this.x;
        this.y = -this.y;
        this.z = -this.z;
    }

    public get normalized(): Vector4 {
        const v = new Vector4(this.w, this.x, this.y, this.z);
        const len = this.magnitude;
        if (len > 0) {
            v.w /= len;
            v.x /= len;
            v.y /= len;
            v.z /= len;
        }
        return v;
    }

    public get inversed(): Vector4 {
        return new Vector4(-this.w, -this.x, -this.y, -this.z);
    }
}