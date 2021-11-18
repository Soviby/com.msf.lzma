using System;
using System.IO;

namespace SevenZip.Compression.LZMA
{
    public static class LZMAUtils
    {
        public enum LZMADictionarySize
        {
            Dict256KiB = 262144, // 2^18 = 262144 = 256 KiB
            Dict512KiB = 524288, // 2^19
            Dict1MiB = 1048576, // 2^20
            Dict2MiB = 2097152, // 2^21
            Dict4MiB = 4194304  // 2^22
        }

        public static void CompressLZMA(Stream inputStream, Stream outputStream,
            LZMADictionarySize dictSize = LZMADictionarySize.Dict256KiB)
        {
            var coder = new SevenZip.Compression.LZMA.Encoder();
            var dictSize32 = (Int32)dictSize;

            coder.SetCoderProperties(
                new SevenZip.CoderPropID[] { SevenZip.CoderPropID.DictionarySize },
                new object[] { dictSize32 });

            // Write encoder properties to output stream
            coder.WriteCoderProperties(outputStream);


            // Write size of input stream to output stream.

            outputStream.Write(BitConverter.GetBytes(inputStream.Length), 0, 8);
            // Encode
            coder.Code(inputStream, outputStream, inputStream.Length, -1, null);
        }

        public static void CompressFileToLZMAFile(string inFile, string outFile,
                    LZMADictionarySize dictSize = LZMADictionarySize.Dict256KiB)
        {
            using (var input = new FileStream(inFile, FileMode.Open))
            using (var output = new FileStream(outFile, FileMode.Create))
            {
                CompressLZMA(input, output, dictSize);
            }
        }

        public static void TestCompressFileToLZMAFile(string inFile, string outFile,
                    LZMADictionarySize dictSize = LZMADictionarySize.Dict256KiB)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var bytes = File.ReadAllBytes(inFile);
            byte[] outBytes = new byte[bytes.Length];
            long outputLength = 0;
            for (int i = 0; i < 1; ++i)
            {
                using (var input = new MemoryStream(bytes))
                using (var output = new MemoryStream(outBytes))
                {
                    CompressLZMA(input, output, dictSize);
                    UnityEngine.Debug.Log($"Compress, times: {i + 1}, cost: {sw.ElapsedMilliseconds}");
                    outputLength = output.Position;
                }
            }
            using (var output = new FileStream(outFile, FileMode.Create))
            {
                output.Write(outBytes, 0, (int)outputLength);
            }
        }

    }

}
