using System;
using System.IO;
using System.Reflection;
using Itinero.Code.Samples;

namespace Itinero.Android.Samples
{   static class Setup
    {
        public static string Folder => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public static string DatabasePath => Path.Combine(Folder, "test.mbtiles");

        public static void Do()
        {
            if (!File.Exists(DatabasePath))
            {
                var stream = LoadEmbeddedResource("Itinero.Code.Samples.Data.test.mbtiles", typeof(MbTilesSample));
                Directory.CreateDirectory(Folder);
                var fileStream = File.Create(DatabasePath);
                ReadWriteStream(stream, fileStream);
            }
        }

        private static void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            var Length = 256;
            var buffer = new byte[Length];
            var bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
            readStream.Close();
            writeStream.Close();
        }

        private static Stream LoadEmbeddedResource(string embeddedResourcePath, Type type)
        {
            var assembly = type.GetTypeInfo().Assembly;
            return assembly.GetManifestResourceStream(embeddedResourcePath);
        }
    }
}