namespace Cawotte.Utils
{
    public class Serialized2DArray<T>
    {
        private T[] array;

        private int width;
        private int height;

        #region Properties
        public int Height { get => height; }
        public int Width { get => width; }
        public int Lenght { get => width * height; }

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
            array = new T[width * height];
        }
        
    }
}
