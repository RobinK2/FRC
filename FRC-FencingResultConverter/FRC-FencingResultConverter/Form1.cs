using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Xml;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace FRC_FencingResultConverter
{
    public partial class Form1 : Form
    {
        //List of xml file titles.
        private List<string> xmlTitles = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }
        
        private void buttonConvert_Click(object sender, EventArgs e)
        {
            buttonConvert.Enabled = false;
            try
            {
                convert();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("The format of the xml file is not supported by this program.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
                /*
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
                 * */
            
            buttonConvert.Enabled = true;
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "XML files|*.xml;";
                ofd.Multiselect = true;
                
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string[] filePath = ofd.FileNames;
                    string[] fileName = ofd.SafeFileNames;
                    for (int i = 0; i < fileName.Length; i++)
                    {
                        xmlTitles.Add(fileName[i].Replace(".xml", ""));
                        listBoxFileName.Items.Add(filePath[i]);
                    }

                    //Enable buttonConvert
                    buttonConvert.Enabled = true;
                }
                
            }
            catch (FileLoadException)
            {
                MessageBox.Show("The file could not be loaded.", "File Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (listBoxFileName.SelectedIndex >= 0 && listBoxFileName.SelectedIndex < listBoxFileName.Items.Count)
            {
                xmlTitles.RemoveAt(listBoxFileName.SelectedIndex);
                listBoxFileName.Items.RemoveAt(listBoxFileName.SelectedIndex);
            }

            if (listBoxFileName.Items.Count == 0)
                buttonConvert.Enabled = false;
        }

        /// <summary>
        /// Converts one or several xml files to pdf files.
        /// This method should be called when buttonConvert is clicked.
        /// </summary>
        private void convert()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PDF | *.pdf";
            sfd.OverwritePrompt = true;
            sfd.CheckPathExists = true;

            string filePath, fileName, directoryPath;
            FRCXmlInterpreter interpreter;
            FRCPdfPrinter printer;
            PdfDocument bundle = new PdfDocument();
            if (radioButtonOnlyFiles.Checked)
            {
                for (int i = 0; i < listBoxFileName.Items.Count; i++)
                {
                    filePath = listBoxFileName.Items[i].ToString();
                    directoryPath = Path.GetDirectoryName(filePath);
                    interpreter = new FRCXmlInterpreter(filePath);
                    interpreter.interpretXml();

                    printer = new FRCPdfPrinter(xmlTitles[i], interpreter);
                    PdfDocument document = printer.printResults();
                    fileName = directoryPath + "\\" + xmlTitles[i] + ".pdf";
                    try
                    {
                        //Save document
                        document.Save(fileName);
                        MessageBox.Show("Convertion succeeded!", "Done", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("PDF documents with same file name as chosen xml file must be closed.", "PDF Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }

                }
            }
            else if (sfd.ShowDialog() == DialogResult.OK)
            {
                directoryPath = Path.GetDirectoryName(sfd.FileName);

                for (int i = 0; i < listBoxFileName.Items.Count; i++)
                {
                    filePath = listBoxFileName.Items[i].ToString();
                    interpreter = new FRCXmlInterpreter(filePath);
                    interpreter.interpretXml();

                    printer = new FRCPdfPrinter(xmlTitles[i], interpreter);
                    PdfDocument document = printer.printResults();
                    fileName = directoryPath + "\\" + xmlTitles[i] + ".pdf";
                    try
                    {
                        //Save document
                        document.Save(fileName);
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("PDF documents with same file name as chosen xml file must be closed.", "PDF Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    document = PdfReader.Open(fileName, PdfDocumentOpenMode.Import);

                    //Concatenate documents.
                    for (int j = 0; j < document.PageCount; j++)
                        bundle.AddPage(document.Pages[j]);

                    if (radioButtonOnlyBundle.Checked)
                    {
                        try
                        {
                            File.Delete(fileName);
                        }
                        catch (IOException exc)
                        {
                            MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                try
                {
                    bundle.Save(sfd.FileName);
                    Process.Start(sfd.FileName);
                }
                catch (IOException)
                {
                    MessageBox.Show("PDF documents with same file name as chosen xml file must be closed.", "PDF Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

    }
}
