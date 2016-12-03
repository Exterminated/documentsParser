using System;
using documentsParser;
using System.Windows.Forms;
using System.IO;

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
        private void open_button_Click(object sender, EventArgs e)
        {
            // Displays an OpenFileDialog so the user can select a Cursor.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Word old (*.doc)|*.doc|Word new (*.docx)|*.docx";
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
                            myDoc = convertToByte(myStream);
                            logBox.Text = "Преобразованно в байты\n";
                            for (int i = 0; i < myDoc.Length; i++) logBox.Text += myDoc[i].ToString() + " ";
                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
        private byte[] convertToByte(Stream input) {
            byte[] output = new byte[input.Length];
            input.Read(output, 0, (int)input.Length);
            return output;

        }      
    }
}
