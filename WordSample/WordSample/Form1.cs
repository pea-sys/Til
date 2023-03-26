using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Word = Microsoft.Office.Interop.Word;

namespace WordSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            // wordファイル
            string wordFile = @"C:\Users\user\source\repos\WordSample\sample\Program Files\sample2.docx";
            List<string> lst = new List<string>();
            

            // Application (Word) を宣言する
            Word.Application word = null;


            try
            { 
                // Application (Word)を作成する
                word = new Word.Application();
                WordTextExporter wte = new WordTextExporter(wordFile, word.Documents);
                foreach(string s in wte.Export())
                {
                    Console.WriteLine(s);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {

                if (word != null)
                {
                    word.Quit();
                }
            }
        }
        private void ExportWord(string file, ComWrapper<Word.Documents> docs)
        {
            var exporter = new WordTextExporter(file, docs.ComObject);
            var contents = exporter.Export();
            if (contents.Count != 0)
            {
            }
        }
    }

}
