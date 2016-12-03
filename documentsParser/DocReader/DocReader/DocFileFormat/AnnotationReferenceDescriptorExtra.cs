using System;
using System.Collections.Generic;
using System.Text;
using DocReader.StructuredStorage.Reader;

namespace DocReader.DocFileFormat
{
    public class AnnotationReferenceDescriptorExtra : ByteStructure
    {
        public DateAndTime Date;

        public Int32 CommentDepth;

        public Int32 ParentOffset;

        public AnnotationReferenceDescriptorExtra(VirtualStreamReader reader, int length)
            : base(reader, length)
        {
            this.Date = new DateAndTime(_reader.ReadBytes(4));
            _reader.ReadBytes(2);
            this.CommentDepth = _reader.ReadInt32();
            this.ParentOffset = _reader.ReadInt32();
            if (length > 16)
            {
                Int32 flag = _reader.ReadInt32();
            }
        }
    }
}
