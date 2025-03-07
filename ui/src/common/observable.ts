type Subscriber<T> = (value: T) => void;

interface Observable<T> {
  getValue: () => T | null;
  setValue: (newValue: T) => void;
  subscribe: (callback: Subscriber<T>) => void;
  unsubscribe: (callback: Subscriber<T>) => void;
  notify: () => void;
}

export function createObservable<T>(): Observable<T> {
  let value: T | null = null; // The internal value
  const subscribers: Subscriber<T>[] = []; // Array to hold subscriber callbacks

  const notify = () => {
    subscribers.forEach((callback) => callback(value as T));
  };

  return {
    // Getter for the value
    getValue: () => value,

    // Setter for the value
    setValue: (newValue: T) => {
      if (value !== newValue) {
        value = newValue;
        notify(); // Notify all subscribers of the change
      }
    },
    notify,

    // Subscribe to changes
    subscribe: (callback: Subscriber<T>) => {
      // ensure the same callback is not added multiple times
      if (!subscribers.includes(callback)) {
        subscribers.push(callback);
      }
    },

    // Unsubscribe from changes
    unsubscribe: (callback: Subscriber<T>) => {
      const index = subscribers.indexOf(callback);
      if (index !== -1) {
        subscribers.splice(index, 1);
      }
    },
  };
}
