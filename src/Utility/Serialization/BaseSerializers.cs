using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Utility.Serialization.Serializers;

namespace Utility.Serialization
{
    public class BaseSerializers
    {

        public static readonly Dictionary<Type, ASerializer> SerializableTypes =
            new Dictionary<Type, ASerializer>
            {
                {
                    typeof(int), new DelegateSerializer(
                                                        bytes => ReadInt(bytes),
                                                        o => Write((int) o)
                                                       )
                },
                {
                    typeof(uint), new DelegateSerializer(
                                                         bytes => ReadUInt(bytes),
                                                         o => Write((uint) o)
                                                        )
                },
                {
                    typeof(long), new DelegateSerializer(
                                                         bytes => ReadLong(bytes),
                                                         o => Write((long) o)
                                                        )
                },
                {
                    typeof(ulong), new DelegateSerializer(
                                                          bytes => ReadULong(bytes),
                                                          o => Write((ulong) o)
                                                         )
                },
                {
                    typeof(short), new DelegateSerializer(
                                                          bytes => ReadShort(bytes),
                                                          o => Write((short) o)
                                                         )
                },
                {
                    typeof(ushort), new DelegateSerializer(
                                                           bytes => ReadUShort(bytes),
                                                           o => Write((ushort) o)
                                                          )
                },
                {
                    typeof(byte), new DelegateSerializer(
                                                         bytes => ReadByte(bytes),
                                                         o => Write((byte) o)
                                                        )
                },
                {
                    typeof(sbyte), new DelegateSerializer(
                                                          bytes => ReadSByte(bytes),
                                                          o => Write((sbyte) o)
                                                         )
                },
                {
                    typeof(bool), new DelegateSerializer(
                                                         bytes => ReadBool(bytes),
                                                         o => Write((bool) o)
                                                        )
                },
                {
                    typeof(float), new DelegateSerializer(
                                                          bytes => ReadFloat(bytes),
                                                          o => Write((float) o)
                                                         )
                },
                {
                    typeof(double), new DelegateSerializer(
                                                           bytes => ReadDouble(bytes),
                                                           o => Write((double) o)
                                                          )
                },
                { typeof(string), new DelegateSerializer(ReadString, o => Write((string) o)) }
            };

        /// <summary>
        ///     Reads an int from the Underlaying Stream
        /// </summary>
        /// <returns>Deserialized int</returns>
        public static int ReadInt(byte[] input)
        {
            return BitConverter.ToInt32(input, 0);
        }


        public static T[] ReadArray<T>(byte[] input)
        {
            int len = ReadInt(input);
            ASerializer s = SerializableTypes[typeof(T)];
            T[] ret = new T[len];
            MemoryStream ms = new MemoryStream(input);
            ms.Position = sizeof(int); //Move the Position to the begin of the array
            PrimitiveValueWrapper pvw = new PrimitiveValueWrapper(ms);
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = (T) s.Deserialize(pvw);
            }

            return ret;
        }

        /// <summary>
        ///     Reads an uint from the Underlaying Stream
        /// </summary>
        /// <returns>Deserialized uint</returns>
        public static uint ReadUInt(byte[] input)
        {
            return BitConverter.ToUInt32(input, 0);
        }

        /// <summary>
        ///     Reads a long from the Underlaying Stream
        /// </summary>
        /// <returns>Deserialized long</returns>
        public static long ReadLong(byte[] input)
        {
            return BitConverter.ToInt64(input, 0);
        }

        /// <summary>
        ///     Reads an ulong from the Underlaying Stream
        /// </summary>
        /// <returns>Deserialized ulong</returns>
        public static ulong ReadULong(byte[] input)
        {
            return BitConverter.ToUInt64(input, 0);
        }

        /// <summary>
        ///     Reads a short from the Underlaying Stream
        /// </summary>
        /// <returns>Deserialized short</returns>
        public static short ReadShort(byte[] input)
        {
            return BitConverter.ToInt16(input, 0);
        }

        /// <summary>
        ///     Reads an ushort from the Underlaying Stream
        /// </summary>
        /// <returns>Deserialized ushort</returns>
        public static ushort ReadUShort(byte[] input)
        {
            return BitConverter.ToUInt16(input, 0);
        }

        /// <summary>
        ///     Reads a bool from the Underlaying Stream
        /// </summary>
        /// <returns>Deserialized bool</returns>
        public static bool ReadBool(byte[] input)
        {
            return BitConverter.ToBoolean(input, 0);
        }

        /// <summary>
        ///     Reads a float from the Underlaying Stream
        /// </summary>
        /// <returns>Deserialized float</returns>
        public static float ReadFloat(byte[] input)
        {
            return BitConverter.ToSingle(input, 0);
        }

        /// <summary>
        ///     Reads a double from the Underlaying Stream
        /// </summary>
        /// <returns>Deserialized double</returns>
        public static double ReadDouble(byte[] input)
        {
            return BitConverter.ToDouble(input, 0);
        }

        /// <summary>
        ///     Reads a string from the Underlaying Stream
        /// </summary>
        /// <returns>Deserialized string</returns>
        public static string ReadString(byte[] input)
        {
            return Encoding.ASCII.GetString(input);
        }

        public static byte ReadByte(byte[] input)
        {
            return input[0];
        }

        public static sbyte ReadSByte(byte[] input)
        {
            return (sbyte) input[0];
        }


        /// <summary>
        ///     Writes an int to the Stream
        /// </summary>
        /// <param name="value">Value to Write</param>
        /// <returns>Bytes Written</returns>
        public static byte[] Write(int value)
        {
            return BitConverter.GetBytes(value);
        }


        public static byte[] WriteArray<T>(T[] input)
        {
            int len = input.Length;
            ASerializer s = SerializableTypes[typeof(T)];
            MemoryStream ms = new MemoryStream();
            PrimitiveValueWrapper pvw = new PrimitiveValueWrapper(ms);
            pvw.Write(len);
            for (int i = 0; i < input.Length; i++)
            {
                s.Serialize(pvw, input[i]);
            }

            pvw.CompleteWrite();
            byte[] ret = ms.ToArray();
            ms.Close();
            return ret;
        }

        /// <summary>
        ///     Writes an uint to the Stream
        /// </summary>
        /// <param name="value">Value to Write</param>
        /// <returns>Bytes Written</returns>
        public static byte[] Write(uint value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        ///     Writes a short to the Stream
        /// </summary>
        /// <param name="value">Value to Write</param>
        /// <returns>Bytes Written</returns>
        public static byte[] Write(short value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        ///     Writes an ushort to the Stream
        /// </summary>
        /// <param name="value">Value to Write</param>
        /// <returns>Bytes Written</returns>
        public static byte[] Write(ushort value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        ///     Writes an long to the Stream
        /// </summary>
        /// <param name="value">Value to Write</param>
        /// <returns>Bytes Written</returns>
        public static byte[] Write(long value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        ///     Writes an ulong to the Stream
        /// </summary>
        /// <param name="value">Value to Write</param>
        /// <returns>Bytes Written</returns>
        public static byte[] Write(ulong value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        ///     Writes a sbyte to the Stream
        /// </summary>
        /// <param name="value">Value to Write</param>
        /// <returns>Bytes Written</returns>
        public static byte[] Write(sbyte value)
        {
            return new[] { (byte) value };
        }

        /// <summary>
        ///     Writes a byte to the Stream
        /// </summary>
        /// <param name="value">Value to Write</param>
        /// <returns>Bytes Written</returns>
        public static byte[] Write(byte value)
        {
            return new[] { value };
        }

        /// <summary>
        ///     Writes a bool to the Stream
        /// </summary>
        /// <param name="value">Value to Write</param>
        /// <returns>Bytes Written</returns>
        public static byte[] Write(bool value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        ///     Writes a float to the Stream
        /// </summary>
        /// <param name="value">Value to Write</param>
        /// <returns>Bytes Written</returns>
        public static byte[] Write(float value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        ///     Writes a double to the Stream
        /// </summary>
        /// <param name="value">Value to Write</param>
        /// <returns>Bytes Written</returns>
        public static byte[] Write(double value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        ///     Writes a string to the Stream
        /// </summary>
        /// <param name="value">Value to Write</param>
        /// <returns>Bytes Written</returns>
        public static byte[] Write(string value)
        {
            return Encoding.ASCII.GetBytes(value);
        }

        public static byte[] WriteSimpleStruct<T>(T obj) where T : struct
        {
            return StructConverter.StructToBytes(obj);
        }

        public static T ReadSimpleStruct<T>(byte[] input) where T : struct
        {
            T ret = default;
            StructConverter.BytesToStruct(input, ref ret);
            return ret;
        }

    }
}