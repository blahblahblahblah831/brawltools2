﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrawlLib.SSBBTypes;

namespace BrawlLib.SSBB
{
    public unsafe class LabelBuilder
    {
        private List<LabelItem> _labels = new List<LabelItem>();

        public int Count { get { return _labels.Count; } }

        public void Clear() { _labels.Clear(); }

        public void Add(int tag, string str) { _labels.Add(new LabelItem() { Tag = tag, String = str }); }

        public int GetSize()
        {
            int len = 12;
            foreach (LabelItem label in _labels)
                len += label.DataLen + 4;
            return len.Align(0x20);
        }

        public void Write(VoidPtr address)
        {
            RSEQ_LABLHeader* header = (RSEQ_LABLHeader*)address;
            int count = _labels.Count;
            VoidPtr dataAddr = address + 12 + (count * 4);
            bint* list = (bint*)(address + 8);
            LabelItem label;
            int size;
            byte* pad;

            for (int i = 0; i < count; )
            {
                label = _labels[i++];
                list[i] = (int)dataAddr - (int)list;
                ((RSEQ_LABLEntry*)dataAddr)->Set(label.Tag, label.String);
                dataAddr += label.DataLen;
            }

            pad = (byte*)dataAddr;
            for (size = dataAddr - address; (size & 0x1F) != 0; size++)
                *pad++ = 0;

            header->Set(size, count);
        }
    }

    public struct LabelItem
    {
        public int Tag;
        public string String;

        public int DataLen { get { return (String.Length + 9).Align(4); } }
    }
}