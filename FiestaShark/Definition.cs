using System;

namespace MapleShark
{
    public sealed class Definition
    {
        public bool Outbound = false;
		public ushort Opcode = 0;
        public string Name = "";
        public bool Ignore = false;

        public static string GetOpcodeAsString(ushort Opcode)
        {
            byte val1 = Convert.ToByte(Opcode >> 10);
            byte val2 = Convert.ToByte(Opcode & 1023);
            return string.Format("{0:D3} | {1:D3}", val1, val2);
        }

        public static byte[] GetOpcodeAsBytes(ushort Opcode)
        {
            byte val1 = Convert.ToByte(Opcode >> 10);
            byte val2 = Convert.ToByte(Opcode & 1023);
            return new byte[] { val1, val2 };
        }
    }
}
