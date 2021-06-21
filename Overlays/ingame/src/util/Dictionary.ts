export interface IDictionary<T> {
    add(key: string, value: T): void;
    remove(key: string): void;
    containsKey(key: string): boolean;
    keys(): string[];
    values(): T[];
}

export class Dictionary<T> implements IDictionary<T> {

    _keys: string[] = [];
    _values: T[] = [];

    constructor(init?: { key: string | number; value: T; }[]) {
        if (init) {
            for (var x = 0; x < init.length; x++) {
                var stringKey = stringKey = init[x].key + '';

                this[stringKey] = init[x].value;
                this._keys.push(stringKey);
                this._values.push(init[x].value);
            }
        }
    }

    add(key: string | number, value: T) {
        var stringKey = stringKey = key + '';
        
        this[stringKey] = value;
        this._keys.push(stringKey);
        this._values.push(value);
    }

    get(key: string) {
        var index = this._keys.indexOf(key, 0);
        return this._values[index];
    }

    remove(key: string) {
        var index = this._keys.indexOf(key, 0);
        this._keys.splice(index, 1);
        this._values.splice(index, 1);

        delete this[key];
    }

    keys(): string[] {
        return this._keys;
    }

    values(): T[] {
        return this._values;
    }

    containsKey(key: string) {
        if (typeof this[key] === "undefined") {
            return false;
        }

        return true;
    }

    toLookup(): IDictionary<T> {
        return this;
    }
}