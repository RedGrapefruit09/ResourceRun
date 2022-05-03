using System;
using System.Collections.Generic;

public class WeightedRandomBag<T>  {
    private struct Entry {
        public double AccumulatedWeight;
        public T Item;
    }

    private readonly List<Entry> _entries = new List<Entry>();
    private double _accumulatedWeight;
    private readonly Random _random = new Random();

    public void AddEntry(T item, double weight) {
        _accumulatedWeight += weight;
        _entries.Add(new Entry { Item = item, AccumulatedWeight = _accumulatedWeight });
    }

    public T GetRandom() {
        double r = _random.NextDouble() * _accumulatedWeight;

        foreach (Entry entry in _entries) {
            if (entry.AccumulatedWeight >= r) {
                return entry.Item;
            }
        }
        
        return default;
    }
}