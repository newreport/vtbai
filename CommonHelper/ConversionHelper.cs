using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper
{
    public static class ConversionHelper
    {

        #region 字节数组转换
        /// <summary>
        /// 字节型转换成十六进制字符串
        /// </summary>
        /// <param name="InBytes"></param>
        /// <returns></returns>
        public static string ByteToHexS(byte[] InBytes)
        {
            string StringOut = "";
            foreach (byte InByte in InBytes)
            {
                StringOut = StringOut + String.Format("{0:X2} ", InByte);
            }
            return StringOut;
        }

        /// <summary>
        /// 把十六进制字符串转换成字节型(方法1)
        /// </summary>
        /// <param name="InString"></param>
        /// <returns></returns>
        public static byte[] HexSToByte(string InString)
        {
            string[] ByteStrings;
            ByteStrings = InString.Split(" ".ToCharArray());
            byte[] ByteOut;
            ByteOut = new byte[ByteStrings.Length];
            for (int i = 0; i <= ByteStrings.Length - 1; i++)
            {
                //ByteOut[i] = System.Text.Encoding.ASCII.GetBytes(ByteStrings[i]);
                ByteOut[i] = Byte.Parse(ByteStrings[i], System.Globalization.NumberStyles.HexNumber);
                //ByteOut[i] =Convert.ToByte("0x" + ByteStrings[i]);
            }
            return ByteOut;
        }

        public static int GetInt(byte[] buffer)
        {
            buffer = buffer.Reverse().ToArray();
            int sum = buffer[0];
            for (int i = 1; i < buffer.Length; i++)
            {
                if (buffer[i] > 0) sum += buffer[i] << (i * 8); //sum += buffer[i] * (int)Math.Pow(16, i * 2);
            }
            return sum;
        }



        public static byte[] GetBytes(int value) => new byte[] { (byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value };
        public static byte[] GetBytes(long value) =>
            new byte[] {
                (byte)(value >> 56), (byte)(value >> 48), (byte)(value >> 40), (byte)(value >> 32),
                (byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value
            };
        public static byte[] GetBytes(short value) => new byte[] { (byte)(value >> 8), (byte)value };
        #endregion

        #region 解压压缩
        //https://www.prowaretech.com/articles/current/dot-net/compression-brotli#!
        //https://www.infoworld.com/article/3660629/how-to-compress-and-decompress-strings-in-c-sharp.html
        //https://www.dotnetperls.com/compress

        // 使用System.IO.Compression进行Deflate压缩
        public static byte[] ZlibCompress(byte[] data)
        {
            using (var uncompressed = new MemoryStream(data))
            {  // 这里举例用的是内存中的数据；需要对文本进行压缩的话，使用 FileStream 即可
                using (var compressed = new MemoryStream())
                {
                    using (var deflateStream = new DeflateStream(compressed, CompressionMode.Compress)) // 注意：这里第一个参数填写的是压缩后的数据应该被输出到的地方
                    {
                        uncompressed.CopyTo(deflateStream); // 用 CopyTo 将需要压缩的数据一次性输入；也可以使用Write进行部分输入
                        deflateStream.Close();  // 在Close中，会先后执行 Finish 和 Flush 操作。
                        byte[] result = compressed.ToArray();
                        return result;
                    }
                }
            };
        }

        // 使用System.IO.Compression进行Deflate解压
        public static byte[] ZlibDecompress(byte[] data)
        {
            using (var compressed = new MemoryStream(data))
            {
                using (var decompressed = new MemoryStream())
                {
                    DeflateStream deflateStream = new DeflateStream(compressed, CompressionMode.Decompress); // 注意： 这里第一个参数同样是填写压缩的数据，但是这次是作为输入的数据
                    deflateStream.CopyTo(decompressed);
                    byte[] result = decompressed.ToArray();
                    return result;
                }
            }
        }
        public static byte[] BrotliDecompress(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream(bytes))
            {
                using (var outputStream = new MemoryStream())
                {
                    using (var decompressStream = new BrotliStream(memoryStream, CompressionMode.Decompress))
                    {
                        decompressStream.CopyTo(outputStream);
                    }
                    return outputStream.ToArray();
                }
            }
        }
        public static byte[] BrotliCompress(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var brotliStream = new BrotliStream(memoryStream, CompressionLevel.Optimal))
                {
                    brotliStream.Write(bytes, 0, bytes.Length);
                }
                return memoryStream.ToArray();
            }
        }

        public static byte[] GZipCompress(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
                {
                    gzipStream.Write(bytes, 0, bytes.Length);
                }
                return memoryStream.ToArray();
            }
        }
        public static byte[] GZipDecompress(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream(bytes))
            {

                using (var outputStream = new MemoryStream())
                {
                    using (var decompressStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                    {
                        decompressStream.CopyTo(outputStream);
                    }
                    return outputStream.ToArray();
                }
            }
        }
        #endregion


    }
}
