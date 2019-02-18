

namespace Cawotte.Utils
{
    using System;
    using UnityEngine;

    [Serializable]
    public class Serialized2DArray<T>
    {
        [SerializeField] private T[] array;

        private int width;
        private int height;

        #region Properties
        public int Height { get => height; }
        public int Width { get => width; }
        public int Length { get => width * height; }
        public T[] Array { get => array; }

        public T this[int index]
        {
            get => array[index];
            set => array[index] = value;
        }

        public T this[int x, int y]
        {
            get => array[y * width + x];
            set => array[y * width + x] = value;
        }
        #endregion

        public Serialized2DArray(int width, int height)
        {
            this.width = width;
            this.height = height;
            array = new T[(width + 1) * (height + 1)];
        }
        
    }
}
