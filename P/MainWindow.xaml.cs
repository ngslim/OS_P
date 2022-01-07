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
using System.Threading;

namespace P
{
    public partial class MainWindow : Window
    {   
        public MainWindow()
        {
            InitializeComponent();
            phaseList.ItemsSource = Phases.GetPhases();
            Phase test = new Phase();
            Start();
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

        private async void Start()
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            updatePhase();
        }

        public void updatePhase()
        {
            var file = @$"{Constant.APP_DIRECTORY}{Constant.CONFIG_FILE}";
            DateTime modification = File.GetLastWriteTime(file);
            DateTime now = DateTime.Now;

            System.TimeSpan timeSpan = now - modification;
            if(timeSpan.Minutes < Constant.TIME_BETWEEN_SAVE)
            {
                MessageBox.Show($"Bạn vừa chỉnh sửa gần đây, thử lại sau {Constant.TIME_BETWEEN_SAVE} phút", "Thông báo");
                return;
            }
            
            string lines = "";
            foreach (var phase in phaseList.Items)
            {
                if (phase is Phase)
                {
                    Phase _phase = (Phase)phase;

                    if(_phase.From == "" || _phase.To == "")
                    {
                        continue;
                    }

                    if (!Phase.checkTimeFormat(_phase.From))
                    {
                        MessageBox.Show($"{_phase.From} không phải thời gian hợp lệ", "Lỗi");
                        _phase.From = "00:00";
                        phaseList.ItemsSource = null;
                        phaseList.ItemsSource = Phases.GetPhases();
                        return;
                    }
                    if (!Phase.checkTimeFormat(_phase.To))
                    {
                        MessageBox.Show($"{_phase.To} không phải thời gian hợp lệ", "Lỗi");
                        _phase.To = "00:00";
                        phaseList.ItemsSource = null;
                        phaseList.ItemsSource = Phases.GetPhases();
                        return;
                    }
                    String phaseString = $"F{_phase.From.ToString()} T{_phase.To.ToString()}";
                    if (_phase.Duration != 0)
                    {
                        phaseString += $" D{_phase.Duration.ToString()}";
                    }
                    if (_phase.InterruptTime != 0)
                    {
                        phaseString += $" I{_phase.InterruptTime.ToString()}";
                    }
                    if (_phase.Sum != 0)
                    {
                        phaseString += $" S{_phase.Sum.ToString()}";
                    }
                    lines += phaseString + "\n";
                }
            }

            File.WriteAllText(file, lines);
            phaseList.ItemsSource = null;
            phaseList.ItemsSource = Phases.GetPhases();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            phaseList.ItemsSource = null;
            phaseList.ItemsSource = Phases.GetPhases();
        }
    }
}
