using System.Collections.Generic;

namespace Tools
{
    internal readonly struct HeapItem<T>
    {
        public HeapItem(T element, int value)
        {
            Element = element;
            Value = value;
        }

        public T Element { get; }

        public int Value { get; }
    }

    internal class Heap<T>
    {
        private readonly List<HeapItem<T>> _list = new() { default };

        public int Count => _list.Count - 1;

        public void Push(T element, int value)
        {
            _list.Add(new HeapItem<T>(element, value));
            SiftUp(_list.Count - 1);
        }

        public T Pop()
        {
            T result = _list[1].Element;
            _list[1] = _list[^1];
            _list.RemoveAt(_list.Count - 1);
            SiftDown(1);

            return result;
        }

        private static int Parent(int i) => i / 2;
        private static int Left(int i) => i * 2;
        private static int Right(int i) => i * 2 + 1;

        private void SiftUp(int index)
        {
            int current = index;

            while (current > 1)
            {
                int parent = Parent(current);

                if (_list[current].Value > _list[parent].Value)
                    return;

                (_list[parent], _list[current]) = (_list[current], _list[parent]);
                current = parent;
            }
        }

        private void SiftDown(int index)
        {
            int current = index;

            while (current < _list.Count)
            {
                int smallest = GetSmallest(current);

                if (smallest == current)
                    return;

                (_list[current], _list[smallest]) = (_list[smallest], _list[current]);
                current = smallest;
            }

            int GetSmallest(int root)
            {
                int left = Left(root);
                int right = Right(root);

                if (GetSmaller(root, left) == root && GetSmaller(root, right) == root)
                    return root;

                return GetSmaller(left, right);
            }

            int GetSmaller(int i, int j)
            {
                int iValue = i < _list.Count ? _list[i].Value : int.MaxValue;
                int jValue = j < _list.Count ? _list[j].Value : int.MaxValue;

                return iValue <= jValue ? i : j;
            }
        }
    }
}
