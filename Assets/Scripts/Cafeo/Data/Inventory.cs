using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Cafeo.Data
{
    public class Inventory : MonoBehaviour, IEnumerable<KeyValuePair<WorldItem, int>>
    {
        private Dictionary<WorldItem, int> data;
        public UnityEvent onDirty;

        protected virtual void Awake()
        {
            data = new Dictionary<WorldItem, int>();
            onDirty = new UnityEvent();
        }

        public int this[WorldItem item]
        {
            get => data.ContainsKey(item) ? data[item] : 0;
            set
            {
                bool hasValue = data.TryGetValue(item, out var oldValue);
                data[item] = value;
                if (value == 0)
                    data.Remove(item);
                if (hasValue && oldValue != value)
                    onDirty.Invoke();
            }
        }

        public void Add(WorldItem item)
        {
            this[item]++;
        }

        public IEnumerator<KeyValuePair<WorldItem, int>> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}