export interface IQueue<T> {
    enqueue(value: T): void;
    dequeue(): T;
    values(): T[];
    peek(): T;
    get length(): number;
}

export default class Queue<T> implements IQueue<T> {
    items: T[];
    headIndex: number = 0;
    tailIndex: number = 0;


    constructor() {
      this.items = [];
      this.headIndex = 0;
      this.tailIndex = 0;
    }

    values(): T[] {
        return this.items;
    }
  
    enqueue(item : T) {
      this.items[this.tailIndex] = item;
      this.tailIndex++;
    }
  
    dequeue(): T {
      const item = this.items[this.headIndex];
      delete this.items[this.headIndex];
      this.headIndex++;
      return item;
    }
  
    peek(): T {
      return this.items[this.headIndex];
    }
  
    get length(): number {
      return this.tailIndex - this.headIndex;
    }
  }