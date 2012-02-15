using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace ShineTableParser
{
    public sealed class ZepheusFile
    {
        public Dictionary<string, ZepheusTable> Tables { get; private set; }
        public void Load(string filename)
        {
            using (var stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    byte tables = br.ReadByte();
                    for (byte i = 0; i < tables; i++)
                    {

                    }
                }
            }
        }
    }

    public sealed class ZepheusTable : DataTable
    {

    }
}
