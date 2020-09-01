using Microsoft.Win32;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace HuffmanTree.ViewModel
{
    public class CodingViewModel : ReactiveObject
    {
        #region privates
        string content;
        string encodedContent;
        double entropy;
        double averageLength;
        int numberOfInputBits;
        int numberOfOutputBits;
        bool isEncoded;
        Dictionary<char, List<bool>> encodedValues;
        BitArray encoded;
        HuffmanTree Tree;
        IObservable<bool> canExecuteOnEncoded;
        #endregion
        #region Property
        public string Content
        {
            get{ return content; }
            set{ this.RaiseAndSetIfChanged(ref content, value); }
        }
        public string EncodedContent
        {
            get { return encodedContent; }
            set{ if(value =="1") this.RaiseAndSetIfChanged(ref encodedContent, value);}
        }

        public BitArray Encoded
        {
            get { return encoded; }
            set { this.RaiseAndSetIfChanged(ref encoded, value); }
        }

        public double Entropy
        {
            get { return entropy;}
            private set{  this.RaiseAndSetIfChanged(ref entropy, value); }
        }

        public double AverageLength
        {
            get{ return averageLength; }
            private set{ this.RaiseAndSetIfChanged(ref averageLength, value);}
        }

        public int NumberOfInputBits
        {
            get{return numberOfInputBits; }
            private set{ this.RaiseAndSetIfChanged(ref numberOfInputBits, value);}
        }

        public int NumberOfOutputBits
        {
            get {return numberOfOutputBits; }
            private set { this.RaiseAndSetIfChanged(ref numberOfOutputBits, value); this.RaisePropertyChanged(nameof(Compression)); }
        }
        public double Compression
        {
            get { return NumberOfOutputBits > 0 ? (NumberOfOutputBits  / (double)numberOfInputBits) * 100 : 0; }
        }
        public int TreeHight
        {
            get;
            set;
        }

        public bool IsEncoded
        {
            get { return isEncoded; }
            set { this.RaiseAndSetIfChanged(ref isEncoded, value); }
        }

        public List<Node> Nodes { get; private set;  }

        public ObservableCollection<CharacterType> Characters { get; private set; }
        #endregion

        #region Commands
        public ReactiveCommand<Unit, Unit> EncodeCommand { get; }
        public ReactiveCommand<Unit,Unit> DecodeCommand { get; }
        public ReactiveCommand<Unit,bool> DrawCommand { get; }
        public ReactiveCommand<Unit,Unit> ExportToXmlCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportToJsonCommand { get; }
        public ReactiveCommand<Unit,Unit> ImportJson { get; }
        public ReactiveCommand<Unit,Unit> ImportXml { get; }
        #endregion
        public CodingViewModel()
        {
            Content = Properties.Resources.Startin_Text;
            Encoded = new BitArray(0);
            canExecuteOnEncoded = this.WhenAnyValue(x => x.IsEncoded, x => x == true);

            EncodeCommand = ReactiveCommand<Unit, Unit>.Create(() =>
             {
                 Tree = new HuffmanTree(content);
                 encodedValues = Tree.GetCodes();
                 Encoded = Tree.Encode(Content);
                 CreateStatsTable();
                 CalculateEntropy();
                 CalculateAverageWordLenth();
                 NumberOfInputBits = Content.Length * 8;
                 CalculateNumberOfOutputBits();
                 TreeHight = Tree.Height;
                 Nodes = Tree.Nodes;
                 IsEncoded = true;
                 DrawCommand.Execute().Subscribe();
                 SetEncodedValue();
             }, this.WhenAnyValue(c => c.Content, (c) => (!String.IsNullOrWhiteSpace(c) && (c.Any(x=>x!=c[0])))));

            DecodeCommand = ReactiveCommand.Create(() =>
                {
                    Content = Tree.Decode(Encoded);
                }, canExecuteOnEncoded.Merge(this.WhenAnyValue(c => c.Encoded.Length, (c) => c > 0)));

            DrawCommand = ReactiveCommand<Unit,bool>.Create(() =>
            {
                return true;
            }, this.WhenAnyValue(e=>e.IsEncoded, e=>e==true));

            ExportToXmlCommand = ReactiveCommand.Create(() =>
            {
                SaveFileDialog dialog = new SaveFileDialog() { Filter = "text|*.xml" };
                string filePath;
                if (dialog.ShowDialog() == true)
                {
                    filePath = dialog.FileName;
                    try
                    {
                        WriteToXmlFile(filePath, Tree);
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            },canExecuteOnEncoded);

            ExportToJsonCommand = ReactiveCommand.Create(() =>
            {
                SaveFileDialog dialog = new SaveFileDialog() { Filter = "text|*.json" };
                string filePath;
                if (dialog.ShowDialog() == true)
                {
                    filePath = dialog.FileName;
                    try
                    {
                        WriteToJsonFIle(filePath);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            },canExecuteOnEncoded);

            ImportJson = ReactiveCommand.CreateFromTask(async () =>
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "text|*.json";
                string content = string.Empty;
                if(dialog.ShowDialog() == true)
                {
                   content= File.ReadAllText(dialog.FileName);
                }
                if (string.IsNullOrEmpty(content))
                    return;
                ReadJson(content);
            });

            ImportXml = ReactiveCommand.CreateFromTask(async () =>
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "text|*.xml";
                string filePath = string.Empty;
                if (dialog.ShowDialog() == true)
                {
                    filePath = dialog.FileName;
                }
                if (filePath == null)
                    return;
                ReadXml(filePath);
            });
        }

        private void ReadXml(string filePath)
        {
            XmlSerializer serialize = new XmlSerializer(typeof(HuffmanTree));
            try
            {
                XDocument xdoc = XDocument.Load(filePath);
                HuffmanTree tree = serialize.Deserialize(xdoc.CreateReader()) as HuffmanTree;


            if (tree == null)
            {
                    throw new Exception();
            }
            this.Tree = tree;
            this.Content = tree.Content;
            this.RaisePropertyChanged(nameof(Content));
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MessageBox.Show("Nie udało się zaimportować obiektu", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReadJson(string content)
        {
            try
            {
                HuffmanTree tree = JsonConvert.DeserializeObject<HuffmanTree>(content);
                if (tree == null)
                {
                    throw new Exception();
                }
                this.Tree = tree;
                this.Content = tree.Content;
                this.RaisePropertyChanged(nameof(Content));
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MessageBox.Show("Nie udało się zaimportować obiektu","Błąd",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void CreateStatsTable()
        {
            Characters = new ObservableCollection<CharacterType>();
            foreach (var character in encodedValues.Keys)
            {
                Characters.Add(new CharacterType()
                {
                    Character = character,
                    Code = string.Join("", StringifyCode(encodedValues[character])),
                    Count = Tree.Frequencies[character],
                    Probability = (Tree.Frequencies[character] / (float)Tree.NumberOfChars)
                });
            }
            this.RaisePropertyChanged(nameof(Characters));
        }

        private string StringifyCode(List<bool> code)
        {
            string stringCode = "";
            foreach (bool bit in code)
            {
                stringCode += bit ? '1' : '0';
            }
            return stringCode;
        }

        private void CalculateEntropy()
        {
            double entropy = 0;
            foreach (CharacterType character in Characters)
            {
                entropy += character.Probability * Math.Log(1.0 / character.Probability, 2);
            }
            Entropy = entropy;
        }

        private void SetEncodedValue()
        {
            string encodedContent = "";
            foreach (bool bit in encoded)
            {

                encodedContent += bit ? '1' : '0';
            }
            EncodedContent = encodedContent;
        }

        private void CalculateAverageWordLenth()
        {
            double average = 0;
            foreach (CharacterType character in Characters)
            {
                average += character.Probability * encodedValues[character.Character].Count;
            }
            AverageLength = average;
        }

        private void CalculateNumberOfOutputBits()
        {
            int length = 0;
            foreach(var key in encodedValues.Keys)
            {
                length += encodedValues[key].Count * Tree.Frequencies[key];
            }
            NumberOfOutputBits = length;
        }

        private void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            
            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(filePath, append);
                serializer.Serialize(writer, objectToWrite);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        private void WriteToJsonFIle(string filePath)
        {
            var jsonObject = JsonConvert.SerializeObject(Tree);
            try
            {
                File.WriteAllText(filePath, jsonObject);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Nie udało się zapisać pliku");
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
