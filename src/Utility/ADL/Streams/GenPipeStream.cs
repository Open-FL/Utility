﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Utility.ADL.Streams
{
    public class GenPipeStream<T> : Stream, IDisposable
    {

        #region Private Variables

        /// <summary>
        ///     Queue of bytes provides the datastructure for transmitting from an
        ///     input stream to an output stream.
        ///     Possible more effecient ways to accomplish this.
        /// </summary>
        private readonly Queue<T> mBuffer = new Queue<T>();

        /// <summary>
        ///     Indicates that the input stream has been flushed and that
        ///     all remaining data should be written to the output stream.
        /// </summary>
        private bool mFlushed;

        /// <summary>
        ///     Setting this to true will cause Read() to block if it appears
        ///     that it will run out of data.
        /// </summary>
        private bool mBlockLastRead;

        /// <summary>
        ///     Number of bytes in a kilobyte
        /// </summary>
        private const long KB = 1024;

        /// <summary>
        ///     Number of bytes in a megabyte
        /// </summary>
        private const long MB = KB * 1024;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the maximum number of bytes to store in the buffer.
        /// </summary>
        public long MaxBufferLength { get; set; } = 200 * MB;

        /// <summary>
        ///     Gets or sets a value indicating whether to block last read method before the buffer is empty.
        ///     When true, Read() will block until it can fill the passed in buffer and count.
        ///     When false, Read() will not block, returning all the available buffer data.
        ///     Setting to true will remove the possibility of ending a stream reader prematurely.
        /// </summary>
        public bool BlockLastReadBuffer
        {
            get => mBlockLastRead;
            set
            {
                mBlockLastRead = value;

                // when turning off the block last read, signal Read() that it may now read the rest of the buffer.
                if (mBlockLastRead)
                {
                    return;
                }

                lock (mBuffer)
                {
                    Monitor.Pulse(mBuffer);
                }
            }
        }

        #region Overrides

        /// <summary>
        ///     When overridden in a derived class, gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <returns>
        ///     true if the stream supports reading; otherwise, false.
        /// </returns>
        public override bool CanRead => true;

        /// <summary>
        ///     When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <returns>
        ///     true if the stream supports seeking; otherwise, false.
        /// </returns>
        public override bool CanSeek => false;

        /// <summary>
        ///     When overridden in a derived class, gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <returns>
        ///     true if the stream supports writing; otherwise, false.
        /// </returns>
        public override bool CanWrite => true;

        /// <summary>
        ///     When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <returns>
        ///     A long value representing the length of the stream in bytes.
        /// </returns>
        public override long Length => mBuffer.Count;

        /// <summary>
        ///     When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <returns>
        ///     The current position within the stream.
        /// </returns>
        public override long Position
        {
            get => 0;
            set => throw new NotSupportedException();
        }

        #endregion

        #endregion

        #region Stream overide methods

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public new void Dispose()
        {
            // clear the internal buffer
            mBuffer.Clear();
        }

        /// <summary>
        ///     When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written
        ///     to the underlying device.
        /// </summary>
        public override void Flush()
        {
            mFlushed = true;
            lock (mBuffer)
            {
                Monitor.Pulse(mBuffer);
            }
        }

        /// <summary>
        ///     When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter. </param>
        /// <param name="origin">
        ///     A value of type System.IO.SeekOrigin indicating the reference point used to obtain the new
        ///     position.
        /// </param>
        /// <returns>
        ///     The new position within the current stream.
        /// </returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes. </param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position
        ///     within the stream by the number of bytes read.
        /// </summary>
        /// <returns>
        ///     The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many
        ///     bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <param name="offset">
        ///     The zero-based byte offset in buffer at which to begin storing the data read from the current
        ///     stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to be read from the current stream. </param>
        /// <param name="buffer">
        ///     An array of bytes. When this method returns, the buffer contains the specified byte array with the
        ///     values between offset and (offset + count - 1) replaced by the bytes read from the current source.
        /// </param>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (typeof(T) != typeof(byte))
            {
                throw new NotSupportedException();
            }

            return ReadGen(buffer as T[], offset, count);
        }

        /// <summary>
        ///     When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position
        ///     within the stream by the number of bytes read.
        /// </summary>
        /// <returns>
        ///     The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many
        ///     bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <param name="offset">
        ///     The zero-based byte offset in buffer at which to begin storing the data read from the current
        ///     stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to be read from the current stream. </param>
        /// <param name="buffer">
        ///     An array of bytes. When this method returns, the buffer contains the specified byte array with the
        ///     values between offset and (offset + count - 1) replaced by the bytes read from the current source.
        /// </param>
        public int ReadGen(T[] buffer, int offset, int count)
        {
            if (offset != 0)
            {
                throw new NotSupportedException("Offsets with value of non-zero are not supported");
            }

            if (buffer == null)
            {
                throw new ArgumentException("Buffer is null");
            }

            if (offset + count > buffer.Length)
            {
                throw new ArgumentException("The sum of offset and count is greater than the buffer length. ");
            }

            if (offset < 0 || count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "offset or count is negative.");
            }

            if (BlockLastReadBuffer && count >= MaxBufferLength)
            {
                throw new ArgumentException($"count({count}) > mMaxBufferLength({MaxBufferLength})");
            }

            if (count == 0)
            {
                return 0;
            }

            int readLength = 0;

            lock (mBuffer)
            {
                while (!ReadAvailable(count))
                {
                    Monitor.Wait(mBuffer);
                }

                // fill the read buffer
                for (; readLength < count && Length > 0; readLength++)
                {
                    buffer[offset + readLength] = mBuffer.Dequeue();
                }

                Monitor.Pulse(mBuffer);
            }

            return readLength;
        }

        /// <summary>
        ///     Returns true if there are
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private bool ReadAvailable(int count)
        {
            return (Length >= count || mFlushed) &&
                   (Length >= count + 1 || !BlockLastReadBuffer);
        }

        public void WriteGen(T[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentException("Buffer is null");
            }

            if (offset + count > buffer.Length)
            {
                throw new ArgumentException("The sum of offset and count is greater than the buffer length. ");
            }

            if (offset < 0 || count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "offset or count is negative.");
            }

            if (count == 0)
            {
                return;
            }

            lock (mBuffer)
            {
                // wait until the buffer isn't full
                while (Length >= MaxBufferLength)
                {
                    Monitor.Wait(mBuffer);
                }

                mFlushed = false; // if it were flushed before, it soon will not be.

                // queue up the buffer data
                for (int i = offset; i < offset + count; i++)
                {
                    mBuffer.Enqueue(buffer[i]);
                }

                Monitor.Pulse(mBuffer); // signal that write has occured
            }
        }

        /// <summary>
        ///     When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current
        ///     position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream. </param>
        /// <param name="count">The number of bytes to be written to the current stream. </param>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream. </param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (typeof(T) != typeof(byte))
            {
                throw new NotSupportedException();
            }

            WriteGen(buffer as T[], offset, count);
        }

        #endregion

    }
}