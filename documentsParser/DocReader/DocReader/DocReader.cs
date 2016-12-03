using DocReader.DocFileFormat;
using DocReader.OpenXmlLib;
using DocReader.OpenXmlLib.WordprocessingML;
using DocReader.StructuredStorage.Reader;
using DocReader.WordprocessingMLMapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DocReader
{
    public static class OldDocReader
    {
        /// <summary>
        /// return MainDocumentPart from doc
        /// </summary>
        /// <param name="docData"></param>
        /// <returns></returns>
        public static XmlDocument GetXMLContent(byte[] docData)
        {
            using (Stream stream = new MemoryStream(docData))
            {
                using (StructuredStorageReader reader = new StructuredStorageReader(stream))
                {
                    //parse the input document
                    WordDocument doc = new WordDocument(reader);

                    //prepare the output document
                    OpenXmlPackage.DocumentType outType = Converter.DetectOutputType(doc);
                    //string conformOutputFile = Converter.GetConformFilename(ChoosenOutputFile, outType);
                    WordprocessingDocument docx = WordprocessingDocument.Create(outType);

                    //start time
                    //DateTime start = DateTime.Now;
                    //TraceLogger.Info("Converting file {0} into {1}", InputFile, conformOutputFile);

                    //convert the document
                    return Converter.Convert(doc, docx);

                   //DateTime end = DateTime.Now;
                    //TimeSpan diff = end.Subtract(start);
                    //TraceLogger.Info("Conversion of file {0} finished in {1} seconds", InputFile, diff.TotalSeconds.ToString(CultureInfo.InvariantCulture));
                }
            }
        }
    }
}
