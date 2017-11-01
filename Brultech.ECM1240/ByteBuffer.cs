using System;
using System.IO;

namespace Brultech.ECM1240
{
    public class ByteBuffer
    {
        internal byte[] _internalBuffer;
        internal int _bufferLength;
        internal int _size;
        internal int _head;
        internal int _tail;

        public ByteBuffer(int length)
        {
            _internalBuffer = new byte[length];
            _bufferLength = length;
        }

        public int Size => _size;
        public int Head => _head;

        public void Clear()
        {
            lock (this)
            {
                _size = 0;
                _head = 0;
                _tail = 0;
            }
        }

        public void Enqueue(byte[] buffer, int offset, int length)
        {
            lock (this)
            {
                if (_size + length > _bufferLength)
                    throw new InternalBufferOverflowException();

                if (_tail + length <= _bufferLength)
                {
                    Buffer.BlockCopy(buffer, offset, _internalBuffer, _tail, length);
                    _tail += length;
                }
                else
                {
                    var firstChunk = _bufferLength - _tail;
                    Buffer.BlockCopy(buffer, offset, _internalBuffer, _tail, firstChunk);
                    Buffer.BlockCopy(buffer, offset + firstChunk, _internalBuffer, 0, length - firstChunk);
                    _tail = length - firstChunk - 1;
                    if (_head < _tail) _head = _tail + 1;
                }

                _size += length;
            }
        }

        public void Dequeue(byte[] buffer, int count)
        {
            lock (this)
            {
                if (count > _size)
                    throw new ArgumentOutOfRangeException("too many bytes requested", nameof(count));

                if (count > buffer.Length)
                    throw new ArgumentOutOfRangeException("buffer too small", nameof(buffer));

                if (_head + count > _bufferLength)
                {
                    Buffer.BlockCopy(_internalBuffer, _head, buffer, 0, _bufferLength - _head);
                    Buffer.BlockCopy(_internalBuffer, 0, buffer, _bufferLength - _head, count - (_bufferLength - _head));
                    _head = count - (_bufferLength - _head);
                }
                else
                {
                    Buffer.BlockCopy(_internalBuffer, _head, buffer, 0, count);
                    _head += count;
                }
                _size -= count;
            }
        }

        public void Discard(int count)
        {
            lock (this)
            {
                if (count > _size)
                    throw new ArgumentOutOfRangeException("too many bytes requested", nameof(count));
                if (_head + count > _bufferLength)
                {
                    _head = count - (_bufferLength - _head);
                }
                else
                {
                    _head += count;
                }
                _size -= count;
            }
        }

        public byte this[int i]
        {
            get
            {
                lock (this)
                {
                    if (i < _size)
                    {
                        return _internalBuffer[(_head + i) % _bufferLength];
                    }
                    throw new IndexOutOfRangeException($"trying to access byte {i} while size is {_size}");
                }
            }
        }

        public byte[] Get(int count)
        {
            lock (this)
            {
                var ret = new byte[count];
                if (_head + count > _bufferLength)
                {
                    Buffer.BlockCopy(_internalBuffer, _head, ret, 0, _bufferLength - _head);
                    Buffer.BlockCopy(_internalBuffer, 0, ret, _bufferLength - _head, count - _bufferLength - _head);
                }
                else
                {
                    Buffer.BlockCopy(_internalBuffer, _head, ret, 0, count);
                }
                return ret;
            }
        }
    }
}
