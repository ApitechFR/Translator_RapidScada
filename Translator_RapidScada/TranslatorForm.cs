using OfficeOpenXml;
using System.Data;
using System.Xml;
using static System.Windows.Forms.DataFormats;
using GoogleTranslateFreeApi;
using System.Security.Principal;
using OfficeOpenXml.Style;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Drawing.Chart;
using System;
using System.Windows.Forms;
using System.Reflection.PortableExecutable;
using System.Reflection;
using OfficeOpenXml.Drawing.Controls;
using OfficeOpenXml.Drawing;
using System.Data.Common;
using System.IO.Packaging;
using OfficeOpenXml.Filter;
using Shell32;
using System.Security.Cryptography;
using System.IO;

namespace Translator_RapidScada
{
    public partial class TranslatorForm : Form
    {
        private string _folderPath = "";
        private string _excelPath = "";
        private List<string> _files = new List<string>(); // selected folder paths

        private string[] _pathsXml = { @"\ScadaWeb\plugins\Chart\lang", @"\ScadaWeb\plugins\Config\lang", @"\ScadaWeb\plugins\Registration\lang", @"\ScadaWeb\plugins\SchBasicComp\lang", @"\ScadaWeb\plugins\Scheme\lang", @"\ScadaWeb\plugins\Store\lang", @"\ScadaWeb\plugins\Table\lang", @"\ScadaWeb\plugins\WebPage\lang", @"\ScadaWeb\lang", @"\ScadaTableEditor\Lang", @"\ScadaServer\Lang", @"\ScadaSchemeEditor\Lang", @"\ScadaComm\Lang", @"\ScadaAgent\Lang", @"\ScadaAdmin\Lang" };
        private string[] _modulesNames = { "ScadaWeb", /*"ScadaTableEditor", */"ScadaServer", "ScadaSchemeEditor", "ScadaComm", "ScadaAgent", "ScadaAdmin" };

        private List<string> _listLanguages = new List<string>(); // languages list
        private Dictionary<string, List<string>> _dicoxfilename = new Dictionary<string, List<string>>(); // dictionnary <Dico, NomFichier>
        private Dictionary<string, Dictionary<string, List<string[]>>> _dicoTranslation = new Dictionary<string, Dictionary<string, List<string[]>>>(); // dictionnary <dico, Phrase, traslate, language>

        private DataTable _currentDt = new DataTable();
        private DataTable _oldDt = new DataTable();

        private string _errFolder = "Erreur: Impossible de choisir ce dossier.";
        private string _errFile = "Erreur: le chemin jusqu'� votre fichier n'existe pas.";
        private string _msgExcel = "Le tableur existe d�j�.";
        private string _msgEditFile = "Voulez vous modifier ce fichier ?";
        private string _msgGenerationExcel = "Le tableur a bien �t� g�n�r�.";
        private string _msgWorkInProgress = "Travail en cours...";
        private string _msgGenerationFolder = "Le dossier a bien �t� g�n�r�.";
        private string _errSelectionFile = "Aucun fichier s�lectionn�.";

        private string _tabDicoKey = "Clef du dictionnaire";
        private string _tabPhraseKey = "Clef de la phrase";
        private string _isNewTard = "Est une nouvelle traduction";



        public TranslatorForm()
        {
            InitializeComponent();

            if (!String.IsNullOrEmpty(Properties.Settings.Default.FolderPath))
                chosenPathLabel1.Text = Properties.Settings.Default.FolderPath;

            if (!String.IsNullOrEmpty(Properties.Settings.Default.XlsxPath))
            {
                labelCheminExcel.Text = "Selection : " + Properties.Settings.Default.XlsxPath;
                label5.Text = "Selection : " + Properties.Settings.Default.XlsxPath;
            }
        }

        // extraction of xml files

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _folderPath = folderBrowserDialog.SelectedPath;

                    Properties.Settings.Default.FolderPath = _folderPath;
                    Properties.Settings.Default.Save();

                    chosenPathLabel1.Text = "Selection : " + Properties.Settings.Default.FolderPath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(_errFolder + ex.Message);
                }
            }
        }

        // choose the folder where the spreadsheet will be saved

        private void button2_Click(object sender, EventArgs e)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();

            //fichier par d�faut
            string defaultFileName = "Traductions_RapidScada";
            saveFileDialog.FileName = defaultFileName;

            saveFileDialog.Filter = "Fichiers Excel (*.xlsx;*.xls)|*.xlsx;*.xls|Tous les fichiers (*.*)|*.*";

            DialogResult result = saveFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                try
                {
                    _excelPath = Path.GetFullPath(saveFileDialog.FileName);
                    labelCheminExcel.Text = "Selection : " + _excelPath;
                    Properties.Settings.Default.XlsxPath = _excelPath;
                    Properties.Settings.Default.Save();
                    label5.Text = "Selection : " + Properties.Settings.Default.XlsxPath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(_errFolder + ex.Message);
                }
            }
        }

        // Excel generation

        private void button3_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Properties.Settings.Default.FolderPath) && !String.IsNullOrEmpty(_excelPath))
            {
                this.Cursor = Cursors.WaitCursor;

                // si dossier contient des raccourcis

                foreach (string directory in Directory.GetFiles(Properties.Settings.Default.FolderPath, "*.lnk"))
                {
                    foreach (string path in _pathsXml)
                    {
                        if (path.Contains(Path.GetFileNameWithoutExtension(directory)))
                        {
                            Shell shell = new Shell();
                            Folder folder = shell.NameSpace(Path.GetDirectoryName(directory));
                            FolderItem folderItem = folder.ParseName(Path.GetFileName(directory));
                            if (folderItem != null)
                            {
                                ShellLinkObject link = (ShellLinkObject)folderItem.GetLink;
                                string targetPath = link.Path;
                                string[] pathLastFolder = path.Split('\\');
                                string pathCombine = Path.Combine(targetPath, pathLastFolder[pathLastFolder.Length - 1]);
                                if (Directory.Exists(pathCombine))
                                {
                                    foreach (string file in Directory.GetFiles(pathCombine, "*.xml"))
                                        _files.Add(file);
                                }
                            }
                        }
                    }
                }

                // sinon
                foreach (string directory in Directory.GetDirectories(Properties.Settings.Default.FolderPath))
                {
                    foreach (string path in _pathsXml)
                    {
                        if (path.Contains(Path.GetFileNameWithoutExtension(directory)))
                        {
                            string completePath = Properties.Settings.Default.FolderPath;
                            string pathCombine = completePath + path;
                            if (Directory.Exists(pathCombine))
                            {
                                foreach (string file in Directory.GetFiles(pathCombine, "*.xml"))
                                    _files.Add(file);
                            }
                        }
                    }
                }

                foreach (string file in _files)
                {
                    CreateRelationshipTable(file);
                }

                ExcelCreation();


                //r�initialisation des variables
                _folderPath = "";
                _excelPath = "";
                _files = new List<string>();
                _listLanguages = new List<string>();
                _dicoxfilename = new Dictionary<string, List<string>>();
                _dicoTranslation = new Dictionary<string, Dictionary<string, List<string[]>>>();
                _currentDt = new DataTable();
                chosenPathLabel1.Text = "Selection : " + Properties.Settings.Default.FolderPath;
                labelCheminExcel.Text = "Selection : " + Properties.Settings.Default.XlsxPath;

                this.Cursor = Cursors.Default;
            }
        }

        // creation of relationship table

        private void CreateRelationshipTable(string filePath)
        {
            XmlDocument xmlDoc = new XmlDocument();

            if (System.IO.File.Exists(filePath))
            {

                xmlDoc.Load(filePath);

                string[] splitPath = filePath.Split('\\');
                string[] splitsplitPath = splitPath[splitPath.Length - 1].Split('.');
                string name = splitsplitPath[0];
                if (name == "ScadaCommon") name = "Scada";

                XmlNodeList nodesDictionary = xmlDoc.SelectNodes("/*[local-name()='" + name + "Dictionaries']/*[local-name()='Dictionary']");

                // language recovery

                string language = splitsplitPath[1];

                if (!_listLanguages.Contains(language)) _listLanguages.Add(language);

                foreach (XmlNode node in nodesDictionary)
                {

                    // dictionnary key recovery
                    string[] xmlSplit = node.OuterXml.Split('"');
                    string key = xmlSplit[1];

                    XmlNodeList nodesPhrase = node.SelectNodes("Phrase");

                    foreach (XmlNode phrase in nodesPhrase)
                    {

                        string[] phraseSplit = phrase.OuterXml.Split('"');
                        string keyPhrase = phraseSplit[1];

                        List<string[]> listTemp = new List<string[]>();

                        //lien entre ma clef phrase et mon fichier 
                        if (!_dicoxfilename.ContainsKey(key))
                            _dicoxfilename.Add(key, new List<string>());
                        if (_dicoxfilename.ContainsKey(key) && !_dicoxfilename[key].Contains(filePath))
                            _dicoxfilename[key].Add(filePath);

                        // add to translaton dictionnary

                        if (!_dicoTranslation.ContainsKey(key))
                            _dicoTranslation.Add(key, new Dictionary<string, List<string[]>>());

                        string[] tabTemp = { phrase.InnerText, language };
                        listTemp.Add(tabTemp);

                        if (!_dicoTranslation[key].ContainsKey(keyPhrase))
                        {
                            _dicoTranslation[key].Add(keyPhrase, listTemp);
                        }
                        else if (_dicoTranslation[key].ContainsKey(keyPhrase) && !_dicoTranslation[key][keyPhrase].Contains(tabTemp))
                        {
                            _dicoTranslation[key][keyPhrase].Add(tabTemp);
                        }
                    }
                }
            }
            else MessageBox.Show(_errFile);

        }

        // spreadsheet creation

        private void ExcelCreation()
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            if (System.IO.File.Exists(_excelPath))
            {
                DialogResult result = MessageBox.Show(_msgExcel + "(" + _excelPath +")\n" +
                    _msgEditFile, "Confirmation", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    LoadOldFormSettings();

                    FileInfo path = new FileInfo(_excelPath);
                    path.Delete();

                    transformToExcel();
                }
            }
            else
            {
                transformToExcel();
            }

        }

        // filling of old datatable

        public void LoadOldFormSettings()
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            // ajout des donn�es de l'excel dans une datatable afin d'utiliser les donn�es
            using (ExcelPackage package = new ExcelPackage(new System.IO.FileInfo(_excelPath )))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                _oldDt.Columns.Add();

                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                {
                    if (worksheet.Cells[1, col].Value.ToString().Contains("fr"))
                    {
                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {

                            DataRow dr = _oldDt.NewRow();

                            dr[0] = worksheet.Cells[row, col].Value;

                            _oldDt.Rows.Add(dr);
                        }
                    }
                }
            }
        }

        // send xml to spreadsheet

        private void transformToExcel()
        {
            using (var package = new ExcelPackage(new FileInfo(_excelPath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Traduction 1");

                worksheet.Cells[1, 1].Value = "XML PATH";
                worksheet.Cells[1, 2].Value = _tabDicoKey;
                worksheet.Cells[1, 3].Value = _tabPhraseKey;
                worksheet.Cells[1, 4].Value = _isNewTard;


                int row = 2;

                for (int i = 0; i < _listLanguages.Count; i++)
                    worksheet.Cells[1, i + 5].Value = _listLanguages[i];

                foreach (KeyValuePair<string, Dictionary<string, List<string[]>>> dico in _dicoTranslation)
                {
                    worksheet.Cells[row, 2].Value = dico.Key;


                    foreach (KeyValuePair<string, List<string>> princ in _dicoxfilename)
                    {
                        if (princ.Key == worksheet.Cells[row, 2].Value)
                        {
                            string chaineTemp = "";
                            foreach (string value in princ.Value)
                            {
                                chaineTemp += $"{value},";
                            }
                            chaineTemp = chaineTemp.Substring(0, chaineTemp.Length - 1);
                            worksheet.Cells[row, 1].Value = chaineTemp;
                        }
                    }

                    foreach (KeyValuePair<string, List<string[]>> secondaire in dico.Value)
                    {
                        worksheet.Cells[row, 3].Value = secondaire.Key;

                        for (int i = 0; i < _listLanguages.Count; i++)
                        {
                            foreach (string[] tab in secondaire.Value)
                            {
                                if (tab[1] == _listLanguages[i])
                                {
                                    worksheet.Cells[row, i + 5].Value = tab[0];

                                    if (!(_listLanguages[i].Contains("en") || _listLanguages[i].Contains("ru")))
                                    {
                                        if (_oldDt.Rows.Count > 0)
                                        {
                                            for (int indexOld = 0; indexOld < _oldDt.Rows.Count; indexOld++)
                                            {
                                                if (indexOld + 2 == row && _oldDt.Rows[indexOld][0].ToString() == worksheet.Cells[row, i + 5].Value.ToString())
                                                    worksheet.Cells[row, 4].Value = 0;
                                                else if (indexOld + 2 == row && _oldDt.Rows[indexOld][0].ToString() != worksheet.Cells[row, i + 5].Value.ToString())
                                                    worksheet.Cells[row, 4].Value = 1;
                                            }
                                        }
                                        else
                                            worksheet.Cells[row, 4].Value = 1;

                                        worksheet.Column(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    }
                                }
                            }
                        }

                        row++;
                    }
                }

                // mise en page de l'excel

                for (int i = 5; i <= 7; i++)
                {
                    ExcelColumn column = worksheet.Column(i);
                    column.Width = 40;
                }

                ExcelColumn columnCheck = worksheet.Column(4);
                columnCheck.Width = 30;

                // autofilter
                ExcelRange columnRange = worksheet.Cells[1, 4, worksheet.Dimension.End.Row, 4];
                columnRange.AutoFilter = true;

                for (int i = 1; i <= 3; i++)
                {
                    ExcelColumn column = worksheet.Column(i);
                    column.Width = 20;
                    column.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    column.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    column.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    column.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    column.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    column.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }

                package.Save();
            }
            MessageBox.Show(_msgGenerationExcel);
        }

        // excel file selection

        //private void button4_Click(object sender, EventArgs e)
        //{
        //    using (OpenFileDialog openFileDialog = new OpenFileDialog())
        //    {
        //        openFileDialog.Filter = "Fichiers Excel (*.xls, *.xlsx)|*.xls;*.xlsx|Tous les fichiers (*.*)|*.*";

        //        if (openFileDialog.ShowDialog() == DialogResult.OK)
        //        {
        //            _excelPath = openFileDialog.FileName;

        //            Properties.Settings.Default.XlsxPath = _excelPath;
        //            Properties.Settings.Default.Save();

        //            label5.Text = "Selection : " + Properties.Settings.Default.XlsxPath;
        //        }
        //    }
        //}

        // excel file extraction

        private void button5_Click(object sender, EventArgs e)
        {


            // choix du dossier ou enregistrer le dossier de traductions

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _folderPath = folderBrowserDialog.SelectedPath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(_errFolder + ex.Message);
                }
            }

            if (Properties.Settings.Default.XlsxPath != "")
            {
                using (var progressDialog = new Form())
                {
                    progressDialog.Text = _msgWorkInProgress;
                    progressDialog.ControlBox = false;
                    progressDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                    progressDialog.StartPosition = FormStartPosition.CenterScreen;
                    progressDialog.AutoSize = true;
                    progressDialog.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    progressDialog.Show();


                    _listLanguages = new List<string>();
                    _dicoxfilename = new Dictionary<string, List<string>>();
                    _dicoTranslation = new Dictionary<string, Dictionary<string, List<string[]>>>();
                    _currentDt = new DataTable();

                    LoadFormSettings();
                    ExcelDataExtraction();

                    progressDialog.Close();
                }
                MessageBox.Show(_msgGenerationFolder);
            }


        }

        // filling of current datatable and dictionnaries

        public void LoadFormSettings()
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            // ajout des donn�es de l'excel dans une datatable afin d'utiliser les donn�es
            using (ExcelPackage package = new ExcelPackage(new System.IO.FileInfo(Properties.Settings.Default.XlsxPath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                for (int i = 1; i <= worksheet.Dimension.End.Column; i++)
                {
                    _currentDt.Columns.Add();
                }

                for (int row = 1; row <= worksheet.Dimension.End.Row; row++)
                {
                    DataRow dr = _currentDt.NewRow();

                    for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                    {
                        dr[col - 1] = worksheet.Cells[row, col].Value;
                    }
                    _currentDt.Rows.Add(dr);
                }
            }

            //remplissage liste langue 

            for (int i = 0; i < _currentDt.Columns.Count; i++)
            {
                if (i > 3)
                {
                    _listLanguages.Add(_currentDt.Rows[0][i].ToString());
                }
            }

            // remplissage du dicoTraduction et phrasexfilename

            string keyTemp = "";

            for (int i = 1; i < _currentDt.Rows.Count; i++)
            {
                if (!_dicoTranslation.ContainsKey(_currentDt.Rows[i][1].ToString()) && _currentDt.Rows[i][1].ToString() != "")
                {
                    _dicoTranslation.Add(_currentDt.Rows[i][1].ToString(), new Dictionary<string, List<string[]>>());
                    keyTemp = _currentDt.Rows[i][1].ToString();
                }


                for (int j = 4; j < _currentDt.Columns.Count; j++)
                {

                    List<string[]> listTemp = new List<string[]>();

                    if (!(_currentDt.Rows[0][j].ToString().Contains("en") || _currentDt.Rows[0][j].ToString().Contains("ru")))
                    {
                        string[] tabTemp = { _currentDt.Rows[i][j].ToString(), _currentDt.Rows[0][j].ToString() };

                        listTemp.Add(tabTemp);

                        if (!_dicoTranslation[keyTemp].ContainsKey(_currentDt.Rows[i][2].ToString()))
                        {
                            _dicoTranslation[keyTemp].Add(_currentDt.Rows[i][2].ToString(), listTemp);
                        }
                        else if (_dicoTranslation[keyTemp].ContainsKey(_currentDt.Rows[i][2].ToString()) && !_dicoTranslation[keyTemp][_currentDt.Rows[i][2].ToString()].Contains(tabTemp))
                        {
                            _dicoTranslation[keyTemp][_currentDt.Rows[i][2].ToString()].Add(tabTemp);
                        }
                    }
                }

                if (_currentDt.Rows[i][0].ToString() != "")
                {
                    if (!_dicoxfilename.ContainsKey(keyTemp))
                    {
                        List<string> listTemp = new List<string>();
                        listTemp = _currentDt.Rows[i][0].ToString().Split(',').ToList();

                        _dicoxfilename.Add(keyTemp, listTemp);
                    }
                }
            }
        }

        // spreadsheet data extraction

        public void ExcelDataExtraction()
        {
            //tableau regroupant tous les dossiers raccourcis du dossier selectionn�

            string[] shortCutDirectories = Directory.GetFiles(_folderPath, "*.lnk");

            // pour chaque module, on va v�rifier s'il appartient ou non � l'arborescence d�j� pr�sente dans le dossier choisi
            foreach (string moduleName in _modulesNames)
            {
                if (!File.Exists(_folderPath + @"\" + moduleName))
                {
                    if (!shortCutDirectories.Contains($@"{Path.Combine(_folderPath, moduleName)}.lnk"))
                        Directory.CreateDirectory(_folderPath + @"\" + moduleName);
                }
            }

            foreach (KeyValuePair<string, List<string>> dicoFile in _dicoxfilename)
            {
                foreach (KeyValuePair<string, Dictionary<string, List<string[]>>> dico in _dicoTranslation)
                {
                    bool createDictionaryInXml = true;

                    if (dicoFile.Key == dico.Key)
                    {
                        bool pathExists = false;
                        string tempDirectoryForPathExist = "";

                        // cr�ation des dossiers contenant le fichier xml d'arriv�
                        foreach (string path in dicoFile.Value)
                        {

                            string scada = path.Contains("SCADA") ? "SCADA" : "scada";
                            string[] splitWithSCADA = path.Split(new[] { $@"\{scada}\" }, StringSplitOptions.None);
                            string[] SplitWithAng = splitWithSCADA[1].Split(new[] { "ang\\" }, StringSplitOptions.None);
                            if (tempDirectoryForPathExist != SplitWithAng[0])
                            {
                                tempDirectoryForPathExist = SplitWithAng[0];
                                pathExists = false;
                            }
                            string subfolderPath = SplitWithAng[0] + "ang";
                            string completePath = Path.Combine(_folderPath, subfolderPath);

                            //d�terminer si ce dossier est un raccourci ou non 
                            string[] splitForDirectory = SplitWithAng[0].Split(@"\");
                            if (shortCutDirectories.Contains($@"{Path.Combine(_folderPath, splitForDirectory[0])}.lnk"))
                            {
                                completePath = GetLnkTarget($@"{Path.Combine(_folderPath, splitForDirectory[0])}.lnk");
                                string[] splitLang = subfolderPath.Split(@"\");
                                completePath = Path.Combine(completePath, splitLang[1]);
                            }

                            if (!Directory.Exists(completePath))
                            {
                                Directory.CreateDirectory(completePath);
                            }

                            if (!pathExists)
                            {
                                // cr�ation du fichier xml � ce chemin ou modification

                                foreach (KeyValuePair<string, List<string[]>> translation in dico.Value)
                                {
                                    string[] sTemp = SplitWithAng[1].Split('.');
                                    string newFileName = sTemp[0] + "." + translation.Value[0][1] + "." + sTemp[2];
                                    string completePathDoc = Path.Combine(completePath, newFileName);

                                    XmlDocument xmlDoc = new XmlDocument();

                                    if (!File.Exists(completePathDoc))
                                    {
                                        CreateXML(xmlDoc, sTemp[0]);
                                        if (createDictionaryInXml)
                                        {
                                            CreateDicoInXML(xmlDoc, dico.Key);
                                            createDictionaryInXml = false;
                                            CreatePhraseInXML(xmlDoc, translation.Key, translation.Value[0][0], dico.Key);
                                        }
                                    }
                                    else
                                    {
                                        xmlDoc.Load(completePathDoc);

                                        // si la phrase existe d�j� avec ou sans traduction, on doit ajouter ou modifier la traduction

                                        XmlNode targetNode = xmlDoc.SelectSingleNode($"//Phrase[@key='{translation.Key}']");

                                        bool createPharse = true;

                                        if (targetNode != null)
                                        {
                                            targetNode.InnerText = translation.Value[0][0];
                                            createDictionaryInXml = false;
                                            createPharse = false;
                                        }

                                        if (createDictionaryInXml)
                                        {
                                            CreateDicoInXML(xmlDoc, dico.Key);
                                            createDictionaryInXml = false;
                                        }

                                        if (createPharse)
                                            CreatePhraseInXML(xmlDoc, translation.Key, translation.Value[0][0], dico.Key);
                                    }
                                    xmlDoc.Save(completePathDoc);
                                }
                                pathExists = true;
                            }
                        }
                    }
                }
            }

        }

        // xml creation

        public void CreateDicoInXML(XmlDocument doc, string clefDico)
        {
            XmlElement dico = doc.CreateElement("Dictionary");
            doc.LastChild.AppendChild(dico);
            XmlAttribute keyDico = doc.CreateAttribute("key");
            keyDico.Value = clefDico;
            dico.Attributes.Append(keyDico);
        }

        public void CreatePhraseInXML(XmlDocument doc, string clefPhrase, string traduction, string clefDico)
        {
            XmlElement phrase = doc.CreateElement("Phrase");

            int count = 0;
            foreach (XmlNode node in doc.LastChild.ChildNodes)
            {
                for (int i = 0; i < node.Attributes.Count; i++)
                {
                    if (node.Attributes[i].InnerText == clefDico)
                    {
                        doc.LastChild.ChildNodes[count].AppendChild(phrase);
                        XmlAttribute keyPhrase = doc.CreateAttribute("key");
                        keyPhrase.Value = clefPhrase;
                        phrase.Attributes.Append(keyPhrase);
                        if (phrase.InnerText != traduction)
                            phrase.InnerText = traduction;
                    }
                }
                count++;
            }
        }

        public void CreateXML(XmlDocument doc, string nomFichier)
        {

            // Cr�ation de la d�claration XML
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.InsertBefore(xmlDeclaration, doc.DocumentElement);

            XmlElement balisePrincipale = doc.CreateElement(nomFichier + "Dictionaries");
            doc.AppendChild(balisePrincipale);
        }

        public static string GetLnkTarget(string lnkPath)
        {
            Shell shell = new Shell();
            Folder folder = shell.NameSpace(Path.GetDirectoryName(lnkPath));
            FolderItem folderItem = folder.ParseName(Path.GetFileName(lnkPath));
            if (folderItem != null)
            {
                ShellLinkObject link = (ShellLinkObject)folderItem.GetLink;
                string targetPath = link.Path;
                return targetPath;
            }
            return "";
        }
    }
}