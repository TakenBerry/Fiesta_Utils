using ScriptNET.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MapleShark
{
    public sealed class ScriptAPI
    {
        private StructureForm mStructure = null;

        [Bindable(false)]
        internal ScriptAPI(StructureForm pStructure) { mStructure = pStructure; }

        public void Write(string pPath, string pLine) { using (StreamWriter writer = File.AppendText(pPath)) writer.WriteLine(pLine); }

        public long AddByte(string pName) { return mStructure.APIAddByte(pName); }
        public long AddSByte(string pName) { return mStructure.APIAddSByte(pName); }
        public long AddUShort(string pName) { return mStructure.APIAddUShort(pName); }
        public long AddShort(string pName) { return mStructure.APIAddShort(pName); }
        public long AddUInt(string pName) { return mStructure.APIAddUInt(pName); }
        public long AddInt(string pName) { return mStructure.APIAddInt(pName); }
        public ulong AddULong(string pName) { return mStructure.APIAddULong(pName); }
        public long AddLong(string pName) { return mStructure.APIAddLong(pName); }

        public double AddFloat(string pName) { return mStructure.APIAddFloat(pName); }
        public double AddDouble(string pName) { return mStructure.APIAddDouble(pName); }

        public string AddString(string pName) { return mStructure.APIAddString(pName); }
        public string AddPaddedString(string pName, int pLength) { return mStructure.APIAddPaddedString(pName, pLength); }
        public void AddField(string pName, int pLength) { mStructure.APIAddField(pName, pLength); }

        public string AddDate(string pName) { return mStructure.APIAddDate(pName); }

        public void AddComment(string pComment) { mStructure.APIAddComment(pComment); }
        public void StartNode(string pName) { mStructure.APIStartNode(pName); }
        public void EndNode(bool pExpand) { mStructure.APIEndNode(pExpand); }
        public long Remaining() { return mStructure.APIRemaining(); }

        public void ShowTypes(bool pShow) { mStructure.APIShowTypes(pShow); }
        public void ShowData(bool pShow) { mStructure.APIShowData(pShow); }
        public void SetTypeFormat() { mStructure.APISetTypeFormat("[{0}] "); }
        public void SetTypeFormat(string pFormat) { mStructure.APISetTypeFormat(pFormat); }
        public void SetNameFormat() { mStructure.APISetNameFormat("{1}"); }
        public void SetNameFormat(string pFormat) { mStructure.APISetNameFormat(pFormat); }
        public void SetDataFormat() { mStructure.APISetDataFormat(" - {2}"); }
        public void SetDataFormat(string pFormat) { mStructure.APISetDataFormat(pFormat); }
        public void SetDateFormat() { mStructure.APISetDateFormat("{0}/{1:00}/{2:00} {3:00}:{4:00}"); }
        public void SetDateFormat(string pFormat) { mStructure.APISetDateFormat(pFormat); }

        // These functions return value, but won't increase Cursor postition
        public long ReadByte() { return mStructure.APIReadByte(); }
        public long ReadSByte() { return mStructure.APIReadSByte(); }
        public long ReadUShort() { return mStructure.APIReadUShort(); }
        public long ReadShort() { return mStructure.APIReadShort(); }
        public long ReadUInt() { return mStructure.APIReadUInt(); }
        public long ReadInt() { return mStructure.APIReadInt(); }
        public ulong ReadULong() { return mStructure.APIReadULong(); }
        public long ReadLong() { return mStructure.APIReadLong(); }

        public double ReadFloat() { return mStructure.APIReadFloat(); }
        public double ReadDouble() { return mStructure.APIReadDouble(); }
        public double Sqrt(double _in) { return Math.Sqrt(_in); }

        public string ReadPaddedString(int pLength) { return mStructure.APIReadPaddedString(pLength); }

        // These functions add value to the list
        public void AddVariable(string pName, ulong pValue) { mStructure.APIAddVariable(pName, pValue); }
        public void AddVariable(string pName, long pValue, int pLength) { mStructure.APIAddVariable(pName, pValue, pLength); }
        public void AddVariable(string pName, double pValue, int pLength) { mStructure.APIAddVariable(pName, pValue, pLength); }
        public void AddVariable(string pName, string pValue, int pLength) { mStructure.APIAddVariable(pName, pValue, pLength); }
    }
}
