using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MapleShark {
    public sealed class FiestaPacket: ListViewItem {
        // Fields
        private byte[] mBuffer;
        private int mCursor;
		private ushort mOpcode;
        private bool mOutbound;
        private DateTime mTimestamp;
        public Definition definition;
        public byte Header { get; private set; }
        public byte Type { get; private set; }

        // Methods
		internal FiestaPacket(DateTime pTimestamp, bool pOutbound, ushort pOpcode, string pName, byte[] pBuffer, DateTime pStreamCreated)
			: this(pTimestamp, pOutbound, Convert.ToByte(pOpcode >> 10), Convert.ToByte(pOpcode & 1023), pName, pBuffer, pStreamCreated) {

		}
        internal FiestaPacket(DateTime pTimestamp, bool pOutbound, byte pHeader, byte pType, string pName, byte[] pBuffer, DateTime pStreamCreated)
            : base(new string[] {
              ((pTimestamp - pStreamCreated).Ticks / 10000).ToString(),
                pOutbound ? "Outbound" : "Inbound",
                pBuffer.Length.ToString(),
                string.Format("{0:D3} | {1:D3}", pHeader, pType),
                pName }) {
            mTimestamp = pTimestamp;
            mOutbound = pOutbound;
			mOpcode = Convert.ToUInt16((pHeader << 10) + (pType & 1023));
            Header = pHeader;
            Type = pType;
            mBuffer = pBuffer;

            definition = Config.Instance.Definitions.Find(d => d.Outbound == mOutbound && d.Opcode == mOpcode);
        }

        public byte[] Dump() {
            byte[] dst = new byte[this.mBuffer.Length + 12];
            ushort length = (ushort)this.mBuffer.Length;
            if( this.mOutbound ) {
                length = (ushort)(length | 0x8000);
            }
            long ticks = this.mTimestamp.Ticks;
            dst[0] = (byte)ticks;
            dst[1] = (byte)(ticks >> 8);
            dst[2] = (byte)(ticks >> 0x10);
            dst[3] = (byte)(ticks >> 0x18);
            dst[4] = (byte)(ticks >> 0x20);
            dst[5] = (byte)(ticks >> 0x28);
            dst[6] = (byte)(ticks >> 0x30);
            dst[7] = (byte)(ticks >> 0x38);
            dst[8] = (byte)length;
            dst[9] = (byte)(length >> 8);
            dst[10] = (byte)this.mOpcode;
            dst[11] = (byte)(this.mOpcode >> 8);
            Buffer.BlockCopy(this.mBuffer, 0, dst, 12, this.mBuffer.Length);
            return dst;
        }

        public bool ReadByte(out byte pValue) {
            pValue = 0;
            if( (this.mCursor + 1) > this.mBuffer.Length ) {
                return false;
            }
            pValue = this.mBuffer[this.mCursor++];
            return true;
        }

        public bool ReadBytes(byte[] pBytes) {
            return this.ReadBytes(pBytes, 0, pBytes.Length);
        }

        public bool ReadBytes(byte[] pBytes, int pStart, int pLength) {
            if( (this.mCursor + pLength) > this.mBuffer.Length ) {
                return false;
            }
            Buffer.BlockCopy(this.mBuffer, this.mCursor, pBytes, pStart, pLength);
            this.mCursor += pLength;
            return true;
        }

        public bool ReadDouble(out double pValue) {
            pValue = 0.0;
            if( (this.mCursor + 8) > this.mBuffer.Length ) {
                return false;
            }
            pValue = BitConverter.ToDouble(this.mBuffer, this.mCursor);
            this.mCursor += 8;
            return true;
        }

        public bool ReadFloat(out float pValue) {
            pValue = 0f;
            if( (this.mCursor + 4) > this.mBuffer.Length ) {
                return false;
            }
            pValue = BitConverter.ToSingle(this.mBuffer, this.mCursor);
            this.mCursor += 4;
            return true;
        }

        public bool ReadInt(out int pValue) {
            pValue = 0;
            if( (this.mCursor + 4) > this.mBuffer.Length ) {
                return false;
            }
            pValue = ((this.mBuffer[this.mCursor++] | (this.mBuffer[this.mCursor++] << 8)) | (this.mBuffer[this.mCursor++] << 0x10)) | (this.mBuffer[this.mCursor++] << 0x18);
            return true;
        }

        public bool ReadLong(out long pValue) {
            pValue = 0L;
            if( (this.mCursor + 8) > this.mBuffer.Length ) {
                return false;
            }
            pValue = ((((((this.mBuffer[this.mCursor++] | (this.mBuffer[this.mCursor++] << 8)) | (this.mBuffer[this.mCursor++] << 0x10)) | (this.mBuffer[this.mCursor++] << 0x18)) | this.mBuffer[this.mCursor++]) | (this.mBuffer[this.mCursor++] << 8)) | (this.mBuffer[this.mCursor++] << 0x10)) | (this.mBuffer[this.mCursor++] << 0x18);
            return true;
        }

        public bool ReadPaddedString(out string pValue, int pLength) {
            pValue = "";
            if( (this.mCursor + pLength) > this.mBuffer.Length ) {
                return false;
            }
            int count = 0;
            while( (count < pLength) && (this.mBuffer[this.mCursor + count] != 0) ) {
                count++;
            }
            if( count > 0 ) {
                pValue = Encoding.ASCII.GetString(this.mBuffer, this.mCursor, count);
            }
            this.mCursor += pLength;
            return true;
        }

        public bool ReadSByte(out sbyte pValue) {
            pValue = 0;
            if( (this.mCursor + 1) > this.mBuffer.Length ) {
                return false;
            }
            pValue = (sbyte)this.mBuffer[this.mCursor++];
            return true;
        }

        public bool ReadShort(out short pValue) {
            pValue = 0;
            if( (this.mCursor + 2) > this.mBuffer.Length ) {
                return false;
            }
            pValue = (short)(this.mBuffer[this.mCursor++] | (this.mBuffer[this.mCursor++] << 8));
            return true;
        }

        public bool ReadUInt(out uint pValue) {
            pValue = 0;
            if( (this.mCursor + 4) > this.mBuffer.Length ) {
                return false;
            }
            pValue = (uint)(((this.mBuffer[this.mCursor++] | (this.mBuffer[this.mCursor++] << 8)) | (this.mBuffer[this.mCursor++] << 0x10)) | (this.mBuffer[this.mCursor++] << 0x18));
            return true;
        }

        public bool ReadULong(out ulong pValue) {
            pValue = 0L;
            if( (this.mCursor + 8) > this.mBuffer.Length ) {
                return false;
            }
            pValue = (ulong)(((((((this.mBuffer[this.mCursor++] | (this.mBuffer[this.mCursor++] << 8)) | (this.mBuffer[this.mCursor++] << 0x10)) | (this.mBuffer[this.mCursor++] << 0x18)) | this.mBuffer[this.mCursor++]) | (this.mBuffer[this.mCursor++] << 8)) | (this.mBuffer[this.mCursor++] << 0x10)) | (this.mBuffer[this.mCursor++] << 0x18));
            return true;
        }

        public bool ReadUShort(out ushort pValue) {
            pValue = 0;
            if( (this.mCursor + 2) > this.mBuffer.Length ) {
                return false;
            }
            pValue = (ushort)(this.mBuffer[this.mCursor++] | (this.mBuffer[this.mCursor++] << 8));
            return true;
        }

        public void Rewind() {
            this.mCursor = 0;
        }

        // Properties
        public int Cursor {
            get {
                return this.mCursor;
            }
        }

        public byte[] InnerBuffer {
            get {
                return this.mBuffer;
            }
        }

        public int Length {
            get {
                return this.mBuffer.Length;
            }
        }

        public string DefinitionName {
            set {
                base.SubItems[4].Text = value;
            }
        }

        public ushort Opcode {
            get {
                return this.mOpcode;
            }
        }

        public bool Outbound {
            get {
                return this.mOutbound;
            }
        }

        public int Remaining {
            get {
                return (this.mBuffer.Length - this.mCursor);
            }
        }

        public DateTime Timestamp {
            get {
                return this.mTimestamp;
            }
        }

        public void ReverseCursor(int pLength) {
            this.mCursor -= pLength;
        }
    }
}
