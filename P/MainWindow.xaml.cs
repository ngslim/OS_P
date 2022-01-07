using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace P
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            start();
        }

        private void refreshImageList()
        {
            imageList.Items.Clear();
            String searchFolder = Constant.SCREENSHOT_DIRECTORY;
            var filters = new String[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp", "svg" };
            var files = GetFilesFrom(searchFolder, filters, false);
            foreach (var file in files)
            {
                imageList.Items.Add(file);
            }
        }

        private async void start()
        {
            while(true)
            {
                refreshImageList();
                await Task.Delay(60000);
            }
        }

        private static String[] GetFilesFrom(String searchFolder, String[] filters, bool isRecursive)
        {
            List<String> filesFound = new List<String>();
            var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var filter in filters)
            {
                filesFound.AddRange(Directory.GetFiles(searchFolder, String.Format("*.{0}", filter), searchOption));
            }
            return filesFound.ToArray();
        }

        private void image_Click(object sender, MouseEventArgs e)
        {
            Image? image = sender as Image;
            Process openImage = new Process();
            openImage.StartInfo = new ProcessStartInfo(@image.Source.ToString())
            {
                UseShellExecute = true
            };
            openImage.Start();
        }
    }
}
