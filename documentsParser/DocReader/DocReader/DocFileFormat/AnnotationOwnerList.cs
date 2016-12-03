using System;
using System.Collections.Generic;
using System.Text;
using DocReader.StructuredStorage.Reader;
using DocReader.Tools;

namespace DocReader.DocFileFormat
{
    public class AnnotationOwnerList : List<string>
    {
        public AnnotationOwnerList(FileInformationBlock fib, VirtualStream tableStream) : base()
        {
            tableStream.Seek(fib.fcGrpXstAtnOwners, System.IO.SeekOrigin.Begin);

            while (tableStream.Position < (fib.fcGrpXstAtnOwners + fib.lcbGrpXstAtnOwners))
            {
                this.Add(Utils.ReadXst(tableStream));
            }
        }
    }
}
