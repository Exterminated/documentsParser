using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using DocReader;

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

        private void open_button_Click(object sender, EventArgs e)
        {
            // Displays an OpenFileDialog so the user can select a Cursor.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            //openFileDialog1.Filter = "Word 1997-2003 (*.doc)|*.doc|Word (*.docx)|*.docx";
            openFileDialog1.Filter = "Word (*.doc)|*.doc";

            openFileDialog1.Title = "Выберите документ";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            myStream = openFileDialog1.OpenFile();
                            logBox.Text = "Открыт документ\n";
                            myDoc = convertStreamToByte(myStream);                             
                        }
                        replace_button.Enabled = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                    replace_button.Enabled = false;
                }
            }
        }
        private byte[] convertStreamToByte(Stream input) {
            byte[] output = new byte[input.Length];
            input.Read(output, 0, (int)input.Length);
            logBox.Text = "Преобразованно в байты\n";
            return output;
        }
        
        private void replace_button_Click(object sender, EventArgs e)
        {
            logBox.Text += "Текст заменен\n";
            replaceInDOCX(myDoc, replasment);
        }
        public void replaceInDOCX(byte[] fileBytes, List<string> stringsToReplase) {
            List<byte> bytes_stringToReplase=new List<byte>();
            
            XmlDocument xdoc = new XmlDocument();
            xdoc= OldDocReader.GetXMLContent(fileBytes);
            //foreach (string s in stringsToReplase) bytes_stringToReplase.Add(Convert.ToByte(s));
            //for (int i = 0; i < fileBytes.Length; i++) {
            //    if (bytes_stringToReplase.Contains(fileBytes[i])) {
            //        //fileBytes[i] = bytes_stringToReplase.Find(x=>x.Equals(fileBytes[i]));
            //        fileBytes[i] = Convert.ToByte("ТЕСТИРОВАНИЕ");
        //  }
        //}
        }
    }
}
