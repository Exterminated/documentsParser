using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using Microsoft.Office.Interop.Word;

namespace documentsParser

{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        byte[] myDoc;
        Stream myStream = null;

        List<string> replasment = new List<string>() { "строчка", "Строчка", "СТРОЧКА" };

        public void openDOC(string path) {
            // Open a doc file.
            Microsoft.Office.Interop.Word.Application application = new Microsoft.Office.Interop.Word.Application();
            Document document = application.Documents.Open(path);

            replaceInDOC(document, replasment);

            // Close word.
            application.Quit();
        }
        private void open_button_Click(object sender, EventArgs e)
        {
            // Displays an OpenFileDialog so the user can select a Cursor.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            
            openFileDialog1.Filter = "Word (*.doc)|*.doc";

            openFileDialog1.Title = "Выберите документ";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (openFileDialog1.OpenFile() != null) {
                        openDOC(openFileDialog1.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                    replace_button.Enabled = false;
                }
            }

            //if (openFileDialog1.ShowDialog() == DialogResult.OK)
            //{
            //    try
            //    {
            //        if ((myStream = openFileDialog1.OpenFile()) != null)
            //        {
            //            using (myStream)
            //            {
            //                myStream = openFileDialog1.OpenFile();
            //                logBox.Text = "Открыт документ\n";
            //                myDoc = convertStreamToByte(myStream);                             
            //            }
            //            replace_button.Enabled = true;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            //        replace_button.Enabled = false;
            //    }
            //}
        }
        private byte[] convertStreamToByte(Stream input) {
            byte[] output = new byte[input.Length];
            input.Read(output, 0, (int)input.Length);
            logBox.Text = "Преобразованно в байты\n";
            return output;
        }
        
        private void replace_button_Click(object sender, EventArgs e)
        {
            //logBox.Text += "Текст заменен\n";
            //replaceInDOC(myDoc, replasment);
        }
        public void replaceInDOC(Document document, List<string> stringsToReplase) {
            // https://msdn.microsoft.com/en-us/library/6b9478cs.aspx

            object start = 0;
            object end = 12;

            Hyperlinks myLinks = document.Hyperlinks;
            
            object linkAddr = "google.ru";
            //string test_bookmark = "testsbookmark";
            //object linkSubAddr = test_bookmark;

            Range rng = document.Range(ref start, ref end);
            rng.Text = "New Text";

            Hyperlink myLink = myLinks.Add(rng, ref linkAddr);

            rng.Select();
        }
    }
}
