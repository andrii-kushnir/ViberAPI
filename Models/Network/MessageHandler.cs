using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Models.Network
{
    public class MessageHandler
    {
        public event EventHandler<MessageEventArgs> MessageReceived;

        private const Int32 DefaultBufferSize = 8192;
        private const Int32 MessageLengthSize = sizeof(Int32);

        private Byte[] _data = new Byte[DefaultBufferSize];
        private Int32 _dataCount;

        public Byte[] PackData(Byte[] buffer, Int32 offset, Int32 count)
        {
            var data = new Byte[count + MessageLengthSize];
            var messageLenBytes = BitConverter.GetBytes(count);

            Buffer.BlockCopy(messageLenBytes, 0, data, 0, messageLenBytes.Length);
            Buffer.BlockCopy(buffer, offset, data, messageLenBytes.Length, count);

            return data;
        }

        public void ProcessRawData(Byte[] buffer, Int32 offset, Int32 count, SocketAsyncEventArgs e)
        {
            while (_dataCount + count > _data.Length)
                Array.Resize(ref _data, _data.Length * 2);

            Buffer.BlockCopy(buffer, offset, _data, _dataCount, count);
            _dataCount += count;

            var dataOffset = 0;
            while (true)
            {
                if (dataOffset + MessageLengthSize > _dataCount)
                {
                    ShiftDataToStart(dataOffset);
                    break;
                }

                var messageLen = BitConverter.ToInt32(_data, dataOffset);
                int messageOffset = dataOffset + MessageLengthSize;

                if (messageOffset + messageLen > _dataCount)
                {
                    ShiftDataToStart(dataOffset);
                    break;
                }

                ShiftDataToStart(messageOffset);
                if (_dataCount >= messageLen)
                {
                    var dt = new byte[messageLen];
                    Buffer.BlockCopy(_data, dataOffset, dt, 0, dt.Length);
                    ShiftDataToStart(messageLen);

                    ThreadPool.QueueUserWorkItem(OnProcessMessageReceived, dt);
                }
                else
                {
                    Array.Resize(ref _data, messageLen);
                    OnProcessMessageReceived(_data);
                }
            }
        }

        protected virtual void OnProcessMessageReceived(object data)
        {
            MessageReceived.Raise(this, new MessageEventArgs((byte[])data));
        }

        private void ShiftDataToStart(Int32 offset)
        {
            if (offset == 0)
                return;

            if (offset >= _dataCount)
            {
                _dataCount = 0;
                return;
            }

            Buffer.BlockCopy(_data, offset, _data, 0, _dataCount - offset);
            Array.Resize(ref _data, _dataCount - offset);
            _dataCount -= offset;
        }
    }
}
