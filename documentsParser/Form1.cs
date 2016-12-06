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
     
        List<XmlDocument> XDOClist = new List<XmlDocument>();
        
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
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            myStream = openFileDialog1.OpenFile();
                            
                            myDoc = convertStreamToByte(myStream);

                        }
                        replace_button.Enabled = true;
                        logBox.Text = "Открыт документ #" + XDOClist.Count.ToString() + " \n";
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
            converDOCtoDOCX(myDoc);
            logBox.Text += "Конвертация в xdoc\n";
        }
        public void converDOCtoDOCX(byte[] fileBytes) {
            XmlDocument xdoc = new XmlDocument();
            xdoc = OldDocReader.GetXMLContent(fileBytes);
            XDOClist.Add(xdoc);
        }       
    }
}
