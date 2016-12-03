using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using DocReader;
using Microsoft.SharePoint;
using System.Text.RegularExpressions;

namespace EAE.LUK.ServiceNews.Classes
{
    /// <summary>Парсер данных из файла
    /// </summary>
    public static class Parser
    {
        /// <summary>Получение новостей
        /// </summary>
        /// <param name="fileDate">Исходный файл </param>
        /// <param name="format">Формат исходного файла </param>
        /// <returns>Список новостей</returns>
        public static string ParseNews(byte[] fileDate, Constants.FileFormats format, SPWeb web, ref List<News> result)
        {
            string[] separatedNews;

            string textFull = GetTextFromFileData(fileDate, format);

            Regex regex = new Regex("\r\n[ ]{2,}\r\n", RegexOptions.None);
            textFull = regex.Replace(textFull, "\r\n \r\n");

            string s = '\u000D'.ToString() + '\u000A'.ToString() + '\u000D'.ToString() + '\u000A'.ToString();   //separators between different news, CR-LF style
            string s1 = '\u000D'.ToString() + '\u000A'.ToString() + " " + '\u000D'.ToString() + '\u000A'.ToString();
            string[] newsSeparators = new string[] { s , s1};
            separatedNews = textFull.Trim().Split(newsSeparators, StringSplitOptions.RemoveEmptyEntries);

            DateTime globalNewsTime = new DateTime();                                       //extracts global news time from 2nd line of text

            if (separatedNews.Length == 0)
            {
                throw new Exception("Пустой документ");
            }

            string strGlobalNewsTime = "";

            if (separatedNews[0].Contains("ЛЕНТА НОВОСТЕЙ"))
            {
                strGlobalNewsTime = separatedNews[0].Substring(separatedNews[0].IndexOf(',') + 2);
                if (!ParseDateTime(strGlobalNewsTime, out globalNewsTime))
                {
                    throw new Exception("Неверный формат даты в шапке новостей. ");
                }
            }
            else
            {
                globalNewsTime = DateTime.Now;
            }

            int numNews = 0;
            string errorsParse = string.Empty;
            MainHelper mainHelper = new MainHelper(web);
            foreach (string n in separatedNews)
            {
                string text = n.Trim();
                if (IsValidNews(text))
                {
                    numNews++;
                    News nws = new News();
                    nws.Active = true;
					nws.Mistakes = string.Empty;

                    nws.Title = text.Substring(0, text.IndexOf('\n'));  //place is optional
                    if (string.IsNullOrEmpty(nws.Title))
                    {
                        errorsParse += string.Format("Неверный формат заголовка, заголовок не найден, новость в документе № {0}</br>", numNews.ToString());
                        continue;
                    }
                    text = text.Substring(text.IndexOf('\n') + 1);

                    nws.Content = text;

                    errorsParse += ParseNewsHeader.Parse(text, globalNewsTime, numNews, ref nws, mainHelper);
                    if (nws.Source == null)
                    {
                        nws.Source = string.Empty;
                    }
                    result.Add(nws);
                }
                else
                {
                    if (numNews != 0) errorsParse += string.Format("Обнаружен блок, не являющийся новостью, после новости № {0}</br>", numNews.ToString());
                }
            }
            return errorsParse;
        }

        private static string GetSource(string source, char[] separators)
        {
            foreach (char sep in separators)
            {
                if (source.EndsWith(sep.ToString()))
                {
                    source = source.Replace(sep.ToString(), "");
                }
            }
            return source;
        }

        /// <summary>Получение Обзоров прессы
        /// </summary>
        /// <param name="fileDate">Исходный файл </param>
        /// <param name="format">Формат исходного файла </param>
        /// <returns>Список обзоров прессы</returns>
        public static string ParsePressReviews(byte[] fileDate, Constants.FileFormats format, ref List<PressReview> result)
        {
            string[] blocks;
            if (IsStyledSource(fileDate, format))
            {
                blocks = GetTextFromStyledData(fileDate, format);
            }
            else
            {
                string textFull = GetTextFromFileData(fileDate, format);

                string s = '\u000D'.ToString() + '\u000A'.ToString() + '\u000D'.ToString() + '\u000A'.ToString();   //separators between different news, CR-LF style
                string[] newsSeparators = new string[] { s };                                                       //looks tricky
                blocks = textFull.Split(newsSeparators, StringSplitOptions.RemoveEmptyEntries);
            }

            int indexShift = 0, globalTitleIndexShift = 0;		//globalTitleIndexShift сдвигает счетчик блоков на один, если существует глобальный заголовок, содержащий "ТЭК СЕГОДНЯ"
            if (blocks.Length >= 2 && blocks[1].ToUpper().Contains("СОДЕРЖАНИЕ"))
            {
                indexShift = 2;
            }
			if (blocks[0].ToUpper().Contains("ТЭК СЕГОДНЯ"))
			{
				globalTitleIndexShift = 1;
			}

            int numPrews = 0;
            string errorsParse = string.Empty;
            for (int i = globalTitleIndexShift + indexShift; i < blocks.Length; i++)
            {
                string contentBlock = blocks[i].Trim();
                if (contentBlock.Length == 0 || contentBlock.IndexOf('\n') == -1)
                {
                    errorsParse += string.Format("Обнаружен блок, не являющийся обзором прессы, после документа № {0}</br>", numPrews.ToString());
                    continue;  
                }

                numPrews++;
                PressReview review = new PressReview();
                review.Active = true;

                string caption = contentBlock.Substring(0, contentBlock.IndexOf('\n')).Trim();

                string content = string.Empty;
                try
                {
                    content = contentBlock.Substring(contentBlock.IndexOf('\n') + 1);
                }
                catch (Exception ex)
                {
                    errorsParse += string.Format("Не найдено тело обзора, обзор в документе № {0} - {1}</br>", numPrews.ToString(), ex.Message);
                    continue;
                }
                review.Content = content;

                try
                {
					if (caption.IndexOf('(') != -1)
					{
						review.Source = caption.Substring(0, caption.IndexOf('(') - 1);
						caption = caption.Substring(caption.IndexOf('(') + 1);
						review.Place = caption.Substring(0, caption.IndexOf(')'));
						caption = caption.Substring(caption.IndexOf(',') + 2);
					}
					else
					{
						review.Source = caption.Substring(0, caption.IndexOf(',') - 1);
						caption = caption.Substring(caption.IndexOf(',') + 2);
					}
                }
                catch (Exception ex)
                {
                    errorsParse += string.Format("Неверный формат в теле заголовка обзора в полях - источник/географическое место/номер издания. Обзор в документе № {0} - {1}</br>", review.ToString(), ex.Message);
                    continue;
                }

                string[] separators = new string[] { "  ", "   " };
                string[] attributes = caption.Split(separators, 3, StringSplitOptions.RemoveEmptyEntries);
                DateTime pressReviewDate = new DateTime();
                if (attributes.Length == 3)
                {
					string[] first;
					if (attributes[0].Contains(','))
					{
						first = attributes[0].Split(',');
						review.SourceNumber = first[0];
						ParseNewsHeader.GetDate(first[1], out pressReviewDate);
					}
					else ParseNewsHeader.GetDate(attributes[0], out pressReviewDate);
					review.Writer = attributes[1].Substring(1);
                    review.Title = attributes[2].Trim();
                }
                else if (attributes.Length == 2)
                {
					string[] first;
					if (attributes[0].Contains(','))
					{
						first = attributes[0].Split(',');
						review.SourceNumber = first[0];
						ParseNewsHeader.GetDate(first[1], out pressReviewDate);
					}
					else ParseNewsHeader.GetDate(attributes[0], out pressReviewDate);
					review.Title = attributes[1].Trim();
                }

                if (string.IsNullOrEmpty(review.Source))
                {
                    errorsParse += string.Format("Отсутствует источник, обзор в документе № {0}</br>", review.ToString());
                    continue;
                }

                if (string.IsNullOrEmpty(review.Title))
                {
                    errorsParse += string.Format("Неверный формат заголовка, заголовок не найден, обзор в документе № {0}</br>", numPrews.ToString());
                    continue;
                }

                if (1 == pressReviewDate.Year)
                {
                    errorsParse += string.Format("Неверный формат даты, обзор в документе № {0}<br/>Тема новости: {1}</br>", numPrews.ToString(), review.Title);
                    continue;
                }
                review.PressReviewDate = pressReviewDate;

                result.Add(review);
            }
            return errorsParse;
        }

        private static bool ParseDateTime(string sDate, out DateTime pDate)
        {
            CultureInfo provider = new CultureInfo("ru-ru");
            return DateTime.TryParse(sDate, provider, DateTimeStyles.None, out pDate);
        }

        private static bool IsValidNews(string text)
        {
            //checking if string has specific format
            bool result = true;
            //possibly need regexp here
            try
            {
                int cursor = text.IndexOf('\n');
                text = text.Substring(cursor + 1);

                cursor = text.IndexOf('.');
                text = text.Substring(cursor + 2);

                cursor = text.IndexOf(" - ");
            }
            catch (ArgumentOutOfRangeException)
            {
                result = false;
            }
            return result;
        }

        private static string GetTextFromXmlDocument(XmlDocument xdoc)
        {
            const string wordNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            StringBuilder textBuilder = new StringBuilder();
            NameTable nt = new NameTable();
            XmlNamespaceManager nsManager = new XmlNamespaceManager(nt);
            nsManager.AddNamespace("w", wordNamespace);

            XmlNodeList parNodes = xdoc.SelectNodes("//w:p", nsManager);

            foreach (XmlNode parNode in parNodes)
            {
                XmlNodeList textNodes = parNode.SelectNodes(".//w:t", nsManager);

                foreach (System.Xml.XmlNode textNode in textNodes)
                {
                    textBuilder.Append(textNode.InnerText);
                }
                textBuilder.Append(Environment.NewLine);
            }
            return textBuilder.ToString();
        }

        private static string[] GetTextFromStyledXmlDocument(XmlDocument xdoc)
        {
            List<string> result = new List<string>();
            const string wordNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            StringBuilder textBuilder = new StringBuilder();
            NameTable nt = new NameTable();
            XmlNamespaceManager nsManager = new XmlNamespaceManager(nt);
            nsManager.AddNamespace("w", wordNamespace);

            XmlNodeList parNodes = xdoc.SelectNodes("//w:p", nsManager);

            //bool containBody = false;

            foreach (XmlNode parNode in parNodes)
            {
                XmlNodeList styleNodes = parNode.SelectNodes(".//w:pStyle/@w:val", nsManager);
                XmlNodeList textNodes = parNode.SelectNodes(".//w:t", nsManager);

                if (styleNodes.Count > 0)
                {
                    if (styleNodes[0].Value == "3" || parNode.OuterXml.Contains("Heading3"))
                    {
                        //if(containBody)
                        //{
                        result.Add(textBuilder.ToString());
                        textBuilder.Clear();
                        //}
                        //containBody = false;
                    }
                }
                //else
                //{
                //containBody = true;
                //}

                foreach (XmlNode textNode in textNodes)
                {
                    textBuilder.Append(textNode.InnerText);
                }
                textBuilder.Append(Environment.NewLine);
            }
            result.Add(textBuilder.ToString());
            return result.ToArray();
        }

        private static string GetTextFromDocx(byte[] fileDate)
        {

            //transforms docx into plain text

            using (Stream stream = new MemoryStream(fileDate))
            {
                try
                {
                    using (WordprocessingDocument wDocument = WordprocessingDocument.Open(stream, false))
                    {
                        XmlDocument xdoc = new XmlDocument();
                        xdoc.Load(wDocument.MainDocumentPart.GetStream());

                        return GetTextFromXmlDocument(xdoc);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Неверная структура docx документа - " + ex.Message);
                }

            }
        }

        private static string[] GetTextFromStyledDocx(byte[] fileDate)
        {
            using (Stream stream = new MemoryStream(fileDate))
            {
                try
                {
                    using (WordprocessingDocument wDocument = WordprocessingDocument.Open(stream, false))
                    {
                        XmlDocument xdoc = new XmlDocument();
                        xdoc.Load(wDocument.MainDocumentPart.GetStream());

                        return GetTextFromStyledXmlDocument(xdoc);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Неверная структура docx документа - " + ex.Message);
                }
            }
        }

        private static string GetContentFromTxt(byte[] fileDate)
        {
            Stream stream = new MemoryStream(fileDate);
            var startPosition = stream.Position;
            try
            {
                var streamReader = new StreamReader(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true), detectEncodingFromByteOrderMarks: true);
                return streamReader.ReadToEnd();
            }
            catch
            {
                stream.Position = startPosition;
                var streamReader = new StreamReader(stream, Encoding.GetEncoding(1251));
                return streamReader.ReadToEnd();
            }
        }

        private static string GetTextFromFileData(byte[] fileDate, Constants.FileFormats format)
        {
            string textFull = string.Empty;
            switch (format)
            {
                case Constants.FileFormats.doc:
                    {
                        textFull = GetTextFromXmlDocument(OldDocReader.GetXMLContent(fileDate));
                        break;
                    }
                case Constants.FileFormats.docx:
                    {
                        textFull = GetTextFromDocx(fileDate);
                        break;
                    }
                case Constants.FileFormats.txt:
                    {
                        textFull = GetContentFromTxt(fileDate);
                        break;
                    }
            }
            return textFull;
        }

        private static string[] GetTextFromStyledData(byte[] fileDate, Constants.FileFormats format)
        {
            switch (format)
            {
                case Constants.FileFormats.doc:
                    return GetTextFromStyledXmlDocument(OldDocReader.GetXMLContent(fileDate));

                case Constants.FileFormats.docx:
                    return GetTextFromStyledDocx(fileDate);

                default:
                    return null;
            }
        }

        private static bool IsStyledSource(byte[] fileDate, Constants.FileFormats format)
        {
            switch (format)
            {
                case Constants.FileFormats.doc:
                    {
                        XmlDocument xdoc = OldDocReader.GetXMLContent(fileDate);
                        const string wordNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
                        NameTable nt = new NameTable();
                        XmlNamespaceManager nsManager = new XmlNamespaceManager(nt);
                        nsManager.AddNamespace("w", wordNamespace);

                        XmlNodeList parNodes = xdoc.SelectNodes("//w:p", nsManager);

                        foreach (XmlNode parNode in parNodes)
                        {
                            if (parNode.OuterXml.Contains("Heading3"))
                            {
                                return true;
                            }
                        }
                        break;
                    }
                case Constants.FileFormats.docx:
                    {
                        using (Stream stream = new MemoryStream(fileDate))
                        {
                            try
                            {
                                using (WordprocessingDocument wDocument = WordprocessingDocument.Open(stream, false))
                                {
                                    Body body = wDocument.MainDocumentPart.Document.Body;
                                    return body.Descendants<ParagraphStyleId>().Any(psId => psId.Val == "3");
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Неверная структура docx документа - " + ex.Message);
                            }
                        }
                    }
                case Constants.FileFormats.txt:
                    return false;
            }
            return false;
        }
    }
}
