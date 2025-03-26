using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace kuro.Core
{
    public struct PooledFixedArray<T>
    {
        private static T[] s_emptyArray = System.Array.Empty<T>();
        private T[] _array;

        public PooledFixedArray(int size)
        {
            _array = FixedArrayPool<T>.Shared(size).Rent();
        }

        public T[] Array
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _array ?? s_emptyArray;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan() => Array.AsSpan();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Release()
        {
            if (_array != null)
            {
                FixedArrayPool<T>.Shared(_array.Length).Return(_array);
                _array = null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Resize(int newSize)
        {
            if (_array != null)
            {
                if (_array.Length == newSize)
                    return;
                FixedArrayPool<T>.Shared(_array.Length).Return(_array);
            }

            _array = FixedArrayPool<T>.Shared(newSize).Rent();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PooledFixedArray<T> Rent(int size)
        {
            var r = new PooledFixedArray<T>();
            r._array = FixedArrayPool<T>.Shared(size).Rent();
            return r;
        }
    }

    public struct FixedArrayPooledObject<T> : IDisposable
    {
        private readonly T[] _toReturn;
        private readonly FixedArrayPool<T> _pool;
        private readonly bool _autoClear;

        internal FixedArrayPooledObject(T[] value, FixedArrayPool<T> pool, bool autoClear)
        {
            this._toReturn = value;
            this._pool = pool;
            this._autoClear = autoClear;
        }

        void IDisposable.Dispose() => this._pool.Return(_toReturn, _autoClear);
    }

    public abstract class FixedArrayPool
    {

    }

    public class FixedArrayPool<T> : FixedArrayPool
    {
        static Dictionary<int, FixedArrayPool<T>> s_poolDict = new Dictionary<int, FixedArrayPool<T>>(8);

        public static FixedArrayPool<T> Shared(int size)
        {
            lock (s_poolDict)
            {
                if (!s_poolDict.TryGetValue(size, out var r))
                {
                    r = new FixedArrayPool<T>(size);
                    s_poolDict.Add(size, r);
                }

                return r;
            }
        }

        private Queue<T[]> _objects;
        private int _arraySize;
        private int _maxArraysPerBucket = 50;

        public FixedArrayPool(int arraySize, int maxArraysPerBucket = 50)
        {
            _objects = new Queue<T[]>();
            _arraySize = arraySize;
            _maxArraysPerBucket = 50;
        }

        public T[] Rent()
        {
            lock (_objects)
            {
                if (_objects.Count > 0)
                {
                    T[] item = _objects.Dequeue();
                    if (item != null)
                    {
                        return item;
                    }
                }
            }

            T[] ret = new T[_arraySize];
            return ret;
        }

        public FixedArrayPooledObject<T> Rent(out T[] r, bool autoClear = true)
        {
            r = Rent();
            return new(r, this, autoClear);
        }

        public bool Return(T[] item, bool clear = true)
        {
            if (item == null)
                return false;
            if (item.Length != _arraySize)
                return false;
            if (clear)
                System.Array.Clear(item, 0, _arraySize);

            lock (_objects)
            {
                if (_objects.Count >= _maxArraysPerBucket)
                    return true;

                _objects.Enqueue(item);
                return true;
            }
        }
    }
}