using System;
using System.Collections.Generic;
using System.Linq;

namespace ResourceRun.Utilities
{
    public class WeightedRandomBag<T>
    {
        private readonly List<Entry> _entries = new List<Entry>();
        private readonly Random _random = new Random();
        private double _accumulatedWeight;

        public void AddEntry(T item, double weight)
        {
            _accumulatedWeight += weight;
            _entries.Add(new Entry { Item = item, AccumulatedWeight = _accumulatedWeight });
        }

        public T GetRandom()
        {
            var r = _random.NextDouble() * _accumulatedWeight;

            foreach (var entry in _entries.Where(entry => entry.AccumulatedWeight >= r)) return entry.Item;

            return default;
        }

        private struct Entry
        {
            public double AccumulatedWeight;
            public T Item;
        }
    }
}