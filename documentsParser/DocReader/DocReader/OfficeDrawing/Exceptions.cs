using System;
using System.Collections.Generic;
using System.Text;

namespace DocReader.OfficeDrawing
{
    public class InvalidRecordException : Exception
    {
        public InvalidRecordException()
            : base() { }

        public InvalidRecordException(string msg)
            : base(msg) { }

        public InvalidRecordException(string msg, Exception innerException)
            : base(msg, innerException) { }
    }

}
