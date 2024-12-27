using System.Collections;

namespace DNDMapper.Infrastructure
{
    public class BiDictionary<T1, T2> : IEnumerable<(T1, T2)>
    {
        private readonly Dictionary<T1, T2> forward = new();
        private readonly Dictionary<T2, T1> reverse = new();

        public void Add(T1 key1, T2 key2)
        {
            if (forward.ContainsKey(key1) || reverse.ContainsKey(key2))
                throw new ArgumentException("Either key1 or key2 already exists.");

            forward[key1] = key2;
            reverse[key2] = key1;
        }
        public bool TryGetByFirst(T1 key1, out T2 value) => forward.TryGetValue(key1, out value);

        public bool TryGetBySecond(T2 key2, out T1 value) => reverse.TryGetValue(key2, out value);
        public void RemoveByFirst(T1 key1)
        {
            if (forward.TryGetValue(key1, out var key2))
            {
                forward.Remove(key1);
                reverse.Remove(key2);
            }
        }
        public void RemoveBySecond(T2 key2)
        {
            if (reverse.TryGetValue(key2, out var key1))
            {
                reverse.Remove(key2);
                forward.Remove(key1);
            }
        }

        public IEnumerator<(T1, T2)> GetEnumerator()
        {
            foreach (var pair in forward)
            {
                yield return (pair.Key, pair.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}