// This code was taken from https://www.youtube.com/watch?v=-lGsuCEWkM0
namespace Main.Scripts.Utilities
{
    public class CircularBuffer<T>
    {
        private T[] _buffer;
        private readonly int _bufferSize;

        public CircularBuffer(int bufferSize)
        {
            this._bufferSize = bufferSize;
            _buffer = new T[bufferSize];
        }

        public void Add(T item, int index)
        {
            _buffer[index % _bufferSize] = item;
        }

        public T Get(int index)
        {
            return _buffer[index % _bufferSize];
        }

        public void Clear()
        {
            _buffer = new T[_bufferSize];
        }
    }
}