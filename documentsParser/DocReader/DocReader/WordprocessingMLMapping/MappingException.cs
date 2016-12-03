using System;
using System.Collections.Generic;
using System.Text;

namespace DocReader.WordprocessingMLMapping
{
    public class MappingException : Exception
    {
        public MappingException(string message)
            : base(message)
        { }
    }
}
