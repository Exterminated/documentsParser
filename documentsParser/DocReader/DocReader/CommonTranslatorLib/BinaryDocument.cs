using System;
using System.Collections.Generic;
using System.Text;

namespace DocReader.CommonTranslatorLib
{
    public abstract class BinaryDocument : IVisitable
    {
        #region IVisitable Members

        public abstract void Convert<T>(T mapping);
        
        #endregion
    }
}
