using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateAnimationPng
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string savePath = "apng.png";
            ushort delay_num = 10;
            ushort delay_den = 8;

            #region 画像準備
            List<byte[]> Images = new List<byte[]>();
            Images.Add(File.ReadAllBytes(@"images\gray.png"));
            Images.Add(File.ReadAllBytes(@"images\yellow.png"));
            #endregion

            UInt32 width = 0, height = 0;
            byte depth = 0, colortype = 0, compress = 0, filter = 0, interlace = 0;

            #region ヘッダを読み込む
            foreach (byte[] chunk in ParseChunks(Images[0]))
            {
                string Type = Encoding.ASCII.GetString(chunk, 4, 4);

                if (Type == "IHDR")
                {
                    width = ToUInt32BE(chunk, 8 + 0);
                    height = ToUInt32BE(chunk, 8 + 4);
                    depth = chunk[8 + 8];
                    colortype = chunk[8 + 9];
                    compress = chunk[8 + 10];
                    filter = chunk[8 + 11];
                    interlace = chunk[8 + 12];
                }
            }
            #endregion

            if (width * height == 0)
            {
                // 幅と高さの積が0なら異常とする
                // 幅と高さが正常に読めてるならその他も正常なはずだからDepth, ColorType,Compress,Filter,Interlaceはチェックしない(手抜き)
                throw (new Exception());
            }

            using (MemoryStream ms = new MemoryStream())
            {
                ms.WriteByte(0x89);
                ms.Write(Encoding.ASCII.GetBytes("PNG\r\n\x1A\n"), 0, 7);

                #region IHDR
                ms.Write((UInt32)13);
                ms.Write("IHDR", Encoding.ASCII);
                ms.Write(width);
                ms.Write(height);
                ms.Write(new byte[] { depth, colortype, compress, filter, interlace }, 0, 5);
                WriteLastCRC(ms, -(4 + 13));
                #endregion

                #region acTL
                ms.Write((UInt32)8);
                ms.Write("acTL", Encoding.ASCII);
                ms.Write((uint)Images.Count);
                ms.Write((UInt32)0);
                WriteLastCRC(ms, -(4 + 8));
                #endregion

                UInt32 sequenceNumber = 0;

                #region fcTL
                ms.Write((UInt32)26);
                ms.Write("fcTL", Encoding.ASCII);
                ms.Write((UInt32)sequenceNumber++);
                ms.Write(width);
                ms.Write(height);
                ms.Write((UInt32)0);
                ms.Write((UInt32)0);
                ms.Write((UInt16)delay_num);
                ms.Write((UInt16)delay_den);
                ms.WriteByte(0);
                ms.WriteByte(0);
                WriteLastCRC(ms, -(4 + 26));
                #endregion

                #region IDAT(frame 0)
                foreach (byte[] chunk in ParseChunks(Images[0]))
                {
                    string Type = Encoding.ASCII.GetString(chunk, 4, 4);

                    if (Type == "IDAT")
                    {
                        ms.Write(chunk, 0, chunk.Length);
                    }
                }
                #endregion

                #region frames
                for (int i = 1; i < Images.Count; i++)
                {
                    #region fcTL
                    ms.Write((UInt32)26);
                    ms.Write("fcTL", Encoding.ASCII);
                    ms.Write((UInt32)sequenceNumber++);
                    ms.Write(width);
                    ms.Write(height);
                    ms.Write((UInt32)0);
                    ms.Write((UInt32)0);
                    ms.Write((UInt16)delay_num);
                    ms.Write((UInt16)delay_den);
                    ms.WriteByte(0);
                    ms.WriteByte(0);
                    WriteLastCRC(ms, -(4 + 26));
                    #endregion

                    #region IDAT(frame n)
                    foreach (byte[] chunk in ParseChunks(Images[i]))
                    {
                        string Type = Encoding.ASCII.GetString(chunk, 4, 4);

                        if (Type == "IDAT")
                        {
                            UInt32 len = ToUInt32BE(chunk, 0) + 4;

                            ms.Write(len);
                            ms.Write("fdAT", Encoding.ASCII);
                            ms.Write((UInt32)sequenceNumber++);

                            ms.Write(chunk, 8, chunk.Length - 12);

                            WriteLastCRC(ms, -(4 + len));
                        }
                    }
                    #endregion
                }
                #endregion

                #region IEND
                ms.Write((UInt32)0);
                ms.Write("IEND", Encoding.ASCII);
                WriteLastCRC(ms, -(4 + 0));
                #endregion

                #region 確認と保存
                {
                    long pos = ms.Position;
                    ms.Position = 0;
                    byte[] buff = new byte[ms.Length];
                    ms.Read(buff, 0, buff.Length);
                    ms.Position = pos;

                    // ParseChunksは各チャンクのCRCをチェックするので、最低限CRCが異常無いかだけは確認できる
                    ParseChunks(buff);

                    using (FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(buff, 0, buff.Length);
                    }
                }
                #endregion
            }
        }
        static byte[][] ParseChunks(byte[] ByteData)
        {
            if (ByteData[0] != 0x89 || Encoding.UTF8.GetString(ByteData, 1, 7) != "PNG\r\n\x1A\n")
            {
                throw (new Exception());
            }

            List<byte[]> Chunks = new List<byte[]>();

            {
                byte[] buff = new byte[8];
                Array.Copy(ByteData, 0, buff, 0, 8);
                Chunks.Add(buff);
            }

            for (int pos = 8; pos < ByteData.Length;)
            {
                int len = (int)ToUInt32BE(ByteData, pos);
                string type = Encoding.ASCII.GetString(ByteData, pos + 4, 4);
                UInt32 crc = ToUInt32BE(ByteData, pos + 4 + 4 + len);

                if (crc != CRC.Calc(ByteData, pos + 4, len + 4))
                {
                    throw (new Exception());
                }

                byte[] buff = new byte[4 + 4 + len + 4];
                Array.Copy(ByteData, pos, buff, 0, buff.Length);
                Chunks.Add(buff);

                pos += buff.Length;
            }

            return (Chunks.ToArray());
        }

        static byte[] Bitmap2Bytes(Bitmap Bitmap, ImageFormat Format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Bitmap.Save(ms, Format);

                ms.Position = 0;

                byte[] buff = new byte[ms.Length];
                ms.Read(buff, 0, buff.Length);

                return (buff);
            }
        }

        static UInt32 ToUInt32BE(byte[] value, int startIndex)
        {
            return (
             (UInt32)value[startIndex + 0] << 24 |
             (UInt32)value[startIndex + 1] << 16 |
             (UInt32)value[startIndex + 2] << 8 |
             (UInt32)value[startIndex + 3]);
        }

        static void WriteLastCRC(Stream stream, long startIndex)
        {
            if (startIndex < 0)
            {
                startIndex = stream.Position + startIndex;
            }

            stream.Position = startIndex;

            byte[] buff = new byte[stream.Length - stream.Position];
            stream.Read(buff, 0, buff.Length);

            stream.Write(CRC.Calc(buff, 0, buff.Length));
        }

        public class CRC
        {
            // [PNGで使うCRC32を計算する - Qiita #テーブルを導入する](http://qiita.com/mikecat_mixc/items/e5d236e3a3803ef7d3c5#%E3%83%86%E3%83%BC%E3%83%96%E3%83%AB%E3%82%92%E5%B0%8E%E5%85%A5%E3%81%99%E3%82%8B)

            static readonly UInt32[] Table;

            static CRC()
            {
                UInt32 magic = 0xEDB88320;

                Table = new UInt32[256];

                for (int i = 0; i < 256; i++)
                {
                    UInt32 value = (UInt32)i;

                    for (int j = 0; j < 8; j++)
                    {
                        bool b = (value & 1) != 0;

                        value >>= 1;

                        if (b)
                        {
                            value ^= magic;
                        }
                    }

                    Table[i] = value;
                }
            }

            public static UInt32 Calc(byte[] data, int start, int length)
            {
                UInt32 crc = 0xFFFFFFFF;

                for (int i = start; i < start + length; crc = Table[(crc ^ data[i++]) & 0xFF] ^ (crc >> 8)) ;

                return (~crc);
            }
        }
    }

    static class MyExt
    {
        public static void Write(this Stream stream, UInt32 value)
        {
            stream.Write(new byte[]
            {
    (byte)(value >> 24),
    (byte)(value >> 16),
    (byte)(value >> 8),
    (byte)(value),
            }, 0, 4);
        }

        public static void Write(this Stream stream, UInt16 value)
        {
            stream.Write(new byte[]
            {
    (byte)(value >> 8),
    (byte)(value),
            }, 0, 2);
        }

        public static void Write(this Stream stream, string value, Encoding encoding)
        {
            byte[] buff = encoding.GetBytes(value);
            stream.Write(buff, 0, buff.Length);
        }
    }
}
