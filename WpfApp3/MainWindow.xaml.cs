using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp3
{
    
    public class FileSearch
    {
        public virtual string[] Search(string filePath, string searchTerm)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                return lines.Where(line => line.Contains(searchTerm)).ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error {ex.Message}");
            }
        }
    }

    
    public class FileSearchProxy : FileSearch
    {
        private FileSearch fileSearch;

        public override string[] Search(string filePath, string searchTerm)
        {
            
            if (fileSearch == null)
            {
                fileSearch = new FileSearch();
            }

           

            
            if (!Path.GetExtension(filePath).Equals(".txt", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Invalid file format.");
            }

            
            return fileSearch.Search(filePath, searchTerm);
        }
    }

    
    public partial class MainWindow : Window
    {
        private FileSearchProxy fileSearchProxy;

        public MainWindow()
        {
            InitializeComponent();
            fileSearchProxy = new FileSearchProxy();
        }

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFileText.Text = openFileDialog.FileName;
            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            string selectedFilePath = selectedFileText.Text;
            string searchTerm = searchTextBox.Text;

            if (string.IsNullOrEmpty(selectedFilePath) || !File.Exists(selectedFilePath))
            {
                MessageBox.Show("Please select a valid file.");
                return;
            }

            if (string.IsNullOrEmpty(searchTerm))
            {
                MessageBox.Show("Please enter a search term.");
                return;
            }

            try
            {
                
                string[] lines = fileSearchProxy.Search(selectedFilePath, searchTerm);

                resultStackPanel.Children.Clear();

                foreach (string line in lines)
                {
                    TextBlock resultTextBlock = new TextBlock();
                    resultTextBlock.Text = line;
                    resultStackPanel.Children.Add(resultTextBlock);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error {ex.Message}");
            }
        }
    }
}
