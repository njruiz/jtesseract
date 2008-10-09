using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Xml;

namespace JTessaract
{
    public partial class Mainform : Form
    {
        private bool boxFileLoaded = false;
        private ArrayList boxes = null;
        private string fileName = null;
        private ArrayList frequentChars = null;
        private JProject project = null;
        private TesseractWrapper wrapper = null;

        public Mainform()
        {
            InitializeComponent();
            boxes = new ArrayList();
            frequentChars = new ArrayList();
            wrapper = new TesseractWrapper();
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "TIF (*.tif,*.tiff)|*.tif;*.tiff|BMP (*.bmp)|*.bmp|PNG (*.png)|*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
                LoadImage();
            }
        }

        private void LoadImage()
        {
            boxFileLoaded = false;
            canvas.MainImage = Bitmap.FromFile(fileName);
            preview.MainImage = canvas.MainImage;
            previewPerChar.MainImage = canvas.MainImage;

            fileName = fileName.Substring(0, fileName.LastIndexOf(".")) + ".box";
            if (File.Exists(fileName))
            {
                // if (MessageBox.Show("Is " + fileName + " the box file?", "JTesseract", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    LoadBoxFile(fileName);
                    boxFileLoaded = true;
                }
            }

            //fileName = fileName.Substring(0, fileName.LastIndexOf(".")) + ".txt";
            //if (!boxFileLoaded && File.Exists(fileName))
            //{
            //    if (MessageBox.Show("Is " + fileName + " the box file?", "JTesseract", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //    {
            //        LoadBoxFile(fileName);
            //        boxFileLoaded = true;
            //    }
            //}

            if (boxes.Count > 0)
            {
                canvas.Boxes = boxes;
                listView.Items.Clear();

                for (int i = 0; i < boxes.Count; i++)
                {
                    Box box = (Box)boxes[i];
                    ListViewItem item = new ListViewItem(box.Charactor);
                    item.SubItems.Add(box.X1.ToString());
                    item.SubItems.Add(box.Y1.ToString());
                    item.SubItems.Add(box.X2.ToString());
                    item.SubItems.Add(box.Y2.ToString());

                    listView.Items.Add(item);
                }
                
                if (listView.Items.Count > 0)
                {
                    listView.Items[0].Selected = true;
                }
            }

            canvas.Invalidate();
            preview.Invalidate();
            previewPerChar.Invalidate();
        }

        private void LoadBoxFile(string fileName)
        {
            boxes.Clear();
            StreamReader stream = new StreamReader(fileName, Encoding.UTF8);
            string currentLine = null;

            currentLine = stream.ReadLine();

            while (currentLine != null)
            {
                try
                {
                    string[] pieces = currentLine.Split(' ');
                    if (pieces.Length >= 5)
                    {
                        Box box = new Box();

                        box.Charactor = pieces[0];
                        box.X1 = Int16.Parse(pieces[1]);
                        box.Y1 = Int16.Parse(pieces[2]);
                        box.X2 = Int16.Parse(pieces[3]);
                        box.Y2 = Int16.Parse(pieces[4]);

                        boxes.Add(box);
                    }
                    else
                    {
                        Logger.Warning("Box file has a incomplete line: " + currentLine);
                    }
                }
                catch 
                {
                    Logger.Warning("Box file reading triggered an exception on line: " + currentLine);
                }

                currentLine = stream.ReadLine();
            }

            stream.Close();
           
        }

        private void listView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                Box box = ((Box)boxes[e.ItemIndex]);
                canvas.CurrentBox = e.ItemIndex;
                canvas.Invalidate();
                preview.CurrentBox = box;
                previewPerChar.CurrentBox = box;
                textBoxChars.Text = box.Charactor;
                preview.Invalidate();
                previewPerChar.Invalidate();
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView.SelectedIndices.Count > 0)
            {
                int lastSelect = 0;
                int[] selectedIndices = new int[listView.SelectedItems.Count];
                for (int i = 0; i < listView.SelectedItems.Count; i++)
                {
                    selectedIndices[i] = listView.SelectedItems[i].Index;
                }

                Array.Sort<int>(selectedIndices);

                lastSelect = selectedIndices[0];

                for (int i = selectedIndices.Length - 1; i >= 0; i--)
                {
                    boxes.RemoveAt(selectedIndices[i]);
                    listView.Items.RemoveAt(selectedIndices[i]);
                }

                if (listView.Items.Count > lastSelect)
                {
                    listView.Items[lastSelect].Selected = true;
                }
                else
                {
                    if ((listView.Items.Count == lastSelect) && (listView.Items.Count > 0))
                    {
                        listView.Items[lastSelect - 1].Selected = true;
                    }
                }

                canvas.Invalidate();
                preview.Invalidate();
                previewPerChar.Invalidate();
            }
        }

        private void splitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Investigate tesseract more to implement this function
            if (listView.SelectedIndices.Count > 0)
            {
                int index = listView.SelectedIndices[0];
                Box boxOriginal = (Box)boxes[index];
                Box boxNew = new Box();
                boxNew.Charactor = boxOriginal.Charactor;
                boxNew.X2 = boxOriginal.X2;
                boxNew.Y2 = boxOriginal.Y2;
                boxOriginal.X2 = (boxOriginal.X1 + boxOriginal.X2) / 2;
                boxNew.X1 = boxOriginal.X2;
                boxNew.Y1 = boxOriginal.Y1;

                ListViewItem item = new ListViewItem(boxNew.Charactor);
                item.SubItems.Add(boxNew.X1.ToString());
                item.SubItems.Add(boxNew.Y1.ToString());
                item.SubItems.Add(boxNew.X2.ToString());
                item.SubItems.Add(boxNew.Y2.ToString());
                
                if (boxes.Count <= index + 1)
                {
                    boxes.Add(boxNew);
                    listView.Items.Add(item);
                }
                else
                {
                    boxes.Insert(index + 1, boxNew);
                    listView.Items.Insert(index + 1, item);
                }

                canvas.Invalidate();

                // TODO: Handle Preview
                preview.Invalidate();
                previewPerChar.Invalidate();
            }
        }

        private void addAfterToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void addBeforeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void buttonSetCharactors_Click(object sender, EventArgs e)
        {
            if (listView.SelectedIndices.Count > 0)
            {
                int index = listView.SelectedIndices[0];
                ((Box)boxes[index]).Charactor = textBoxChars.Text;
                listView.Items[index].Text = textBoxChars.Text;
                canvas.Invalidate();
            }
        }

        private void aToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void Mainform_Load(object sender, EventArgs e)
        {
            LoadConfiguration(GetAssemblyFolder());

            foreach (string charactor in frequentChars)
            {
                ToolStripCharMapMenuItem item = new ToolStripCharMapMenuItem(charactor);
                item.Text = charactor;
                item.CharacterMapClickEvent += new CharacterMapClickEventDelegate(menu_CharacterMapClickEvent);
                contextMenuStripCharactorMap.Items.Add(item);
            }
        }

        void menu_CharacterMapClickEvent(string charactor)
        {
            textBoxChars.AppendText(charactor);
        }

        private void buttonCharactorMap_Click(object sender, EventArgs e)
        {
            contextMenuStripCharactorMap.Show(buttonCharactorMap, 0, 0);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileName = fileName.Substring(0, fileName.LastIndexOf(".")) + ".box";
            StreamWriter stream = new StreamWriter(fileName, false, Encoding.UTF8);

            for (int i = 0; i < boxes.Count; i++)
            {
                Box box = (Box)boxes[i];
                string currentLine = String.Format("{0} {1} {2} {3} {4}", box.Charactor, box.X1.ToString(), box.Y1.ToString(), box.X2.ToString(), box.Y2.ToString()); ;
                stream.WriteLine(currentLine);
            } 
            
            stream.Close();
        }

        private string GetAssemblyFolder()
        {
            return Path.GetDirectoryName(GetType().Assembly.CodeBase.Replace("file:///", String.Empty));
        }

        private bool LoadConfiguration(string path)
        {
            string configurationFile = path + @"\config.xml";
            bool operationSuccess = false;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configurationFile);

                XmlElement rootElement = doc.DocumentElement;

                XmlNode frequentCharElement = rootElement.SelectSingleNode("frequent-char");

                XmlNodeList currentNodeList = frequentCharElement.SelectNodes("char");
                foreach (XmlNode node in currentNodeList)
                {
                    frequentChars.Add(node.InnerText);
                }

                operationSuccess = true;
            }
            catch
            {
                operationSuccess = false;
            }

            return operationSuccess;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxChars_TextChanged(object sender, EventArgs e)
        {
            string str = textBoxChars.Text;
            if (str.Length > 0)
            {
                textBoxUnicode1.Text = String.Format("{0:x4}", (int)str[0]).ToUpper();
                if (str.Length > 1)
                {
                    textBoxUnicode2.Text = String.Format("{0:x4}", (int)str[1]).ToUpper();
                    if (str.Length > 2)
                    {
                        textBoxUnicode3.Text = String.Format("{0:x4}", (int)str[2]).ToUpper();
                        if (str.Length > 3)
                        {
                            textBoxUnicode4.Text = String.Format("{0:x4}", (int)str[3]).ToUpper();
                            if (str.Length > 4)
                            {
                                textBoxUnicode5.Text = String.Format("{0:x4}", (int)str[4]).ToUpper();
                                if (str.Length > 5)
                                {
                                    textBoxUnicode6.Text = String.Format("{0:x4}", (int)str[5]).ToUpper();
                                }
                                else
                                {
                                    textBoxUnicode6.Text = "";
                                }
                            }
                            else
                            {
                                textBoxUnicode5.Text = "";
                                textBoxUnicode6.Text = "";
                            }
                        }
                        else
                        {
                            textBoxUnicode4.Text = "";
                            textBoxUnicode5.Text = "";
                            textBoxUnicode6.Text = "";
                        }
                    }
                    else
                    {
                        textBoxUnicode3.Text = "";
                        textBoxUnicode4.Text = "";
                        textBoxUnicode5.Text = "";
                        textBoxUnicode6.Text = "";
                    }
                }
                else
                {
                    textBoxUnicode2.Text = "";
                    textBoxUnicode3.Text = "";
                    textBoxUnicode4.Text = "";
                    textBoxUnicode5.Text = "";
                    textBoxUnicode6.Text = "";
                }
            }
            else
            {
                textBoxUnicode1.Text = "";
                textBoxUnicode2.Text = "";
                textBoxUnicode3.Text = "";
                textBoxUnicode4.Text = "";
                textBoxUnicode5.Text = "";
                textBoxUnicode6.Text = "";
            }
        }

        private void mergeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 2)
            {
                // consecutive boxes assumed
                int firstIndex = listView.SelectedItems[0].Index;
                int secondIndex = listView.SelectedItems[1].Index;

                Box box1 = (Box)boxes[firstIndex];
                Box box2 = (Box)boxes[secondIndex];

                box1.X2 = box2.X2;
                box1.Y2 = box2.Y2;

                ListViewItem item = listView.Items[firstIndex];
                item.SubItems[3].Text = box1.X2.ToString();
                item.SubItems[4].Text = box1.Y2.ToString();

                boxes.RemoveAt(secondIndex);
                listView.Items.RemoveAt(secondIndex);

                if (listView.Items.Count > firstIndex)
                {
                    listView.Items[firstIndex].Selected = true;
                }

                canvas.Invalidate();
                preview.Invalidate();
                previewPerChar.Invalidate();
            }
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (listView.SelectedItems.Count == 1)
            {
                addAfterToolStripMenuItem.Enabled = true;
                addBeforeToolStripMenuItem.Enabled = true;
                splitToolStripMenuItem.Enabled = true;
                mergeToolStripMenuItem.Enabled = false;
                deleteToolStripMenuItem.Enabled = true;
            }
            else if (listView.SelectedItems.Count == 2)
            {
                if (1 != Math.Abs(listView.SelectedItems[0].Index - listView.SelectedItems[1].Index))
                {
                    mergeToolStripMenuItem.Enabled = false;
                }
                else
                {
                    mergeToolStripMenuItem.Enabled = true;
                }

                addAfterToolStripMenuItem.Enabled = false;
                addBeforeToolStripMenuItem.Enabled = false;
                splitToolStripMenuItem.Enabled = false;
                deleteToolStripMenuItem.Enabled = true;
            }
            else if (listView.SelectedItems.Count > 2)
            {
                addAfterToolStripMenuItem.Enabled = false;
                addBeforeToolStripMenuItem.Enabled = false;
                splitToolStripMenuItem.Enabled = false;
                mergeToolStripMenuItem.Enabled = false;
                deleteToolStripMenuItem.Enabled = true;
            }
        }

        private void buttonLoadImages_Click(object sender, EventArgs e)
        {
            ImportImages();
            UpdateTesseractLog();
        }

        public bool UpdateListViewSourceImages()
        {
            if (project != null)
            {
                int imagesCount = project.SourceImages.Count;

                //listViewSourceImages.Clear();
                listViewSourceImages.Items.Clear();

                for (int i = 0; i < imagesCount; i++)
                {
                    ListViewItem item = listViewSourceImages.Items.Add(new ListViewItem(new string[] {(i + 1).ToString(), project.SourceImages[i].ToString(), "yes"}));
                    //item.SubItems.Add(project.SourceImages[i].ToString());
                    //tem.SubItems.Add("yes");
                }
            }

            return true;
        }

        public bool ImportImages()
        {
            if (project == null)
            {
                NewProject();
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "TIFF (*.tif, *.tiff)|*.tif;*.tiff";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                int count = openFileDialog.FileNames.Length;
                for (int i = 0; i < count; i++)
                {
                    string fileName = openFileDialog.FileNames[i];
                    project.ImportSourceImage(fileName);
                }
            }

            CreateBoxesWithoutOverwriting();

            UpdateListViewSourceImages();

            return true;
        }

        public void CreateBoxesWithoutOverwriting()
        {
            int count = project.SourceImages.Count;
            
            for (int i = 0; i < count; i++)
            {
                string fileName = (string)project.SourceImages[i];
                string fileNameSegment = fileName.Substring(0, fileName.LastIndexOf('.'));

                if (!File.Exists(project.ProjectFolder + fileNameSegment + ".box"))
                {
                    wrapper.CreateBoxFile(project.ProjectFolder, fileName);
                }
            }
        }               

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProject();   
        }

        public bool NewProject()
        {
            if (project != null)
            {
                if (MessageBox.Show("Do you want to save the current project?", "JTesseract", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    project.SaveProject();
                }
            }

            project = new JProject();
            string projectFilePath = null;
            bool existingProject = true;
            string projectFolderPath = null;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JTesseract Project Files (*.tes)|*.tes";
            while (existingProject)
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    projectFilePath = saveFileDialog.FileName;


                    projectFolderPath = projectFilePath.Substring(0, projectFilePath.LastIndexOf('.'));

                    if (Directory.Exists(projectFolderPath))
                    {
                        if (MessageBox.Show("Do you want to overwrite the existing folder: " + projectFolderPath, "JTesseract", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            existingProject = false;
                        }
                    }
                    else
                    {
                        existingProject = false;
                    }
                }
                else
                {
                    return false;
                }
            }

            project.NewProject(projectFilePath);

            return true;
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (project != null)
            {
                if (MessageBox.Show("Do you want to save the current project?", "JTesseract", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    project.SaveProject();
                }
            }

            project = new JProject();
            string projectFilePath = null;
            bool existingProject = false;
            string projectFolderPath = null;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JTesseract Project Files (*.tes)|*.tes";

            while (!existingProject)
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    projectFilePath = openFileDialog.FileName;
                }

                projectFolderPath = projectFilePath.Substring(0, projectFilePath.LastIndexOf('.'));

                if (!Directory.Exists(projectFolderPath))
                {
                    MessageBox.Show("Seems like a currupted project: " + projectFolderPath + ". Please try another!", "JTesseract", MessageBoxButtons.OK);
                }
                else
                {
                    existingProject = true;
                }
            }
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (project != null)
            {
                project.SaveProject();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (project != null)
            {
                if (MessageBox.Show("Do you want to save the current project?", "JTesseract", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    project.SaveProject();
                }
            }
        }

        private void buttonAddSourceImages_Click(object sender, EventArgs e)
        {
            ImportImages();
        }

        private void listViewSourceImages_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            fileName = project.ProjectFolder + e.Item.SubItems[1].Text;           
            LoadImage();
            LoadBoxFile();
            LoadTrainingFile();
        }

        private void LoadBoxFile()
        {
            string boxFile = fileName.Substring(0, fileName.LastIndexOf('.'));
            boxFile += ".box";

            if (File.Exists(boxFile))
            {
                textBoxBoxFile.Enabled = true;
                buttonUpdateBoxFile.Enabled = true;
                textBoxBoxFile.Text = File.ReadAllText(boxFile);
            }
            else
            {
                buttonUpdateBoxFile.Enabled = false;
                textBoxBoxFile.Enabled = false;
            }
        }

        private void LoadTrainingFile()
        {
            string trainingFile = fileName.Substring(0, fileName.LastIndexOf('.'));
            trainingFile += ".tr";

            if (File.Exists(trainingFile))
            {
                buttonUpdateTrainingFile.Enabled = true;
                textBoxTrainingFile.Enabled = true;
                textBoxTrainingFile.Text = File.ReadAllText(trainingFile);
            }
            else
            {
                buttonUpdateTrainingFile.Enabled = false;
                textBoxTrainingFile.Enabled = false;
            }
        }

        private void buttonGenerateTrainingNClustering_Click(object sender, EventArgs e)
        {
            int count = project.SourceImages.Count;

            for (int i = 0; i < count; i++)
            {
                string fileName = (string)project.SourceImages[i];
                wrapper.CreateTrainingFile(project.ProjectFolder, fileName);
                UpdateTesseractLog();
            }

            wrapper.PerformClustering(project.ProjectFolder, project.SourceImages);
            UpdateTesseractLog();
        }

        private void tabPageTrainFile_Click(object sender, EventArgs e)
        {

        }

        private void UpdateTesseractLog()
        {
            tesseractLog.Text = wrapper.GetTesseractLog(project.ProjectFolder);
        }

        private void tabControlImages_TabIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void tabControlImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if (tabControlImages.SelectedIndex
        }

        private void listViewSourceImages_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonUpdateBoxFile_Click(object sender, EventArgs e)
        {
            string boxFile = fileName.Substring(0, fileName.LastIndexOf('.'));
            boxFile += ".box";

            if (File.Exists(boxFile))
            {
                File.WriteAllText(boxFile, textBoxBoxFile.Text);
            }
        }

        private void buttonUpdateTrainingFile_Click(object sender, EventArgs e)
        {
            string trainingFile = fileName.Substring(0, fileName.LastIndexOf('.'));
            trainingFile += ".tr";

            if (File.Exists(trainingFile))
            {
                File.WriteAllText(trainingFile, textBoxTrainingFile.Text);
            }
        }

        private void buttonGenerateDictionary_Click(object sender, EventArgs e)
        {
            wrapper.GenerateUniCharSet(project.ProjectFolder, project.SourceImages);
            UpdateTesseractLog();
            wrapper.CreateDictionaries(project.ProjectFolder);
            UpdateTesseractLog();
            wrapper.CreateDangAmbigs(project.ProjectFolder);
            UpdateTesseractLog();
        }

        private void buttonCharSet_Click(object sender, EventArgs e)
        {
            wrapper.GenerateUniCharSet(project.ProjectFolder, project.SourceImages);
            UpdateTesseractLog();
        }

        private void buttonDictionary_Click(object sender, EventArgs e)
        {
            wrapper.CreateDictionaries(project.ProjectFolder);
            UpdateTesseractLog();
        }

        private void buttonAmbiguity_Click(object sender, EventArgs e)
        {
            wrapper.CreateDangAmbigs(project.ProjectFolder);
            UpdateTesseractLog();
        }

        private void buttonSaveLanguagePack_Click(object sender, EventArgs e)
        {
            wrapper.CreateLanguagePack(project.ProjectFolder, "ras");
            UpdateTesseractLog();
        }
    }
}