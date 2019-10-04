using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace LevelEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string iniFile = "LevelEditor.ini";
        private const string fileDialogLF = "Level File (*.lf)|*.lf";
        private const string fileDialogAll = "All Files (*.*)|*";
        public static RoutedCommand NewCommand = new RoutedCommand();
        public static RoutedCommand OpenCommand = new RoutedCommand();
        public static RoutedCommand SaveCommand = new RoutedCommand();
        public static RoutedCommand HelpCommand = new RoutedCommand();

        public LevelMap LevelMap = new LevelMap();
        public string CurrentFileName { get; set; }
        public bool isSaved;
        private MenuItem[] recentFilesList = new MenuItem[10];

        public MainWindow()
        {
            InitializeComponent();
            InitWindow();
            NewMap();
        }

        private void InitWindow()
        {
            NewCommand.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            OpenCommand.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            SaveCommand.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            HelpCommand.InputGestures.Add(new KeyGesture(Key.F1));
            DataContext = this;
            CurrentFileName = "";
            isSaved = true;
            recentFilesList[0] = RecentFile1Button;
            recentFilesList[1] = RecentFile2Button;
            recentFilesList[2] = RecentFile3Button;
            recentFilesList[3] = RecentFile4Button;
            recentFilesList[4] = RecentFile5Button;
            recentFilesList[5] = RecentFile6Button;
            recentFilesList[6] = RecentFile7Button;
            recentFilesList[7] = RecentFile8Button;
            recentFilesList[8] = RecentFile9Button;
            recentFilesList[9] = RecentFile10Button;
            ReadRecentFileFromIni();
        }



        private void NewMap()
        {
            CreateNewMap createNewMap = new CreateNewMap();
            if (createNewMap.ShowDialog() == true)
            {
                LevelMap.Init(this, createNewMap.LevelHeight, createNewMap.LevelWidth);
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isSaved)
            {
                if (MessageBox.Show("Create new file without save old one?", "Without Save?", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            }
            NewMap();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isSaved)
            {
                if (MessageBox.Show("Open new file without save old one?" , "Without Save?", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = fileDialogLF + "|" + fileDialogAll;
            if (openFileDialog.ShowDialog() == true)
            {
                Load(openFileDialog.FileName);
            }
        }

        private void Load(string fileName)
        {
            CurrentFileName = fileName;
            SaveButton.GetBindingExpression(MenuItem.IsEnabledProperty).UpdateTarget(); //WPF fix
            AddFileToRecentFiles(fileName);
            LevelMap.Load(fileName, this);
            RepaintCanvas();
            ListToolsListBox.Items.Refresh();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!SaveButton.IsEnabled)
            {
                SaveAsButton_Click(sender, e);
                return;
            }
            LevelMap.Save(CurrentFileName);
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = fileDialogLF;
            if (saveFileDialog.ShowDialog() == true)
            {
                LevelMap.Save(saveFileDialog.FileName);
            }
        }

        public void DrawTileOnCanvas(int x, int y)
        {
            Canvas.Children.Add(new Rectangle()
            {
                Width = LevelMap.TileWidth,
                Height = LevelMap.TileHeight,
                Margin = new Thickness(x * LevelMap.TileWidth, y * LevelMap.TileHeight, 0, 0),
                Stroke = Brushes.Black,
                Fill = LevelMap.Level[x, y].Color
            });
        }

        public void RepaintCanvas()
        {
            Canvas.Children.Clear();
            Canvas.Height = LevelMap.Height * LevelMap.TileHeight;
            Canvas.Width = LevelMap.Width * LevelMap.TileWidth;
            for (int x = 0; x < LevelMap.Level.GetLength(0); x++)
            {
                for (int y = 0; y < LevelMap.Level.GetLength(1); y++)
                {
                    DrawTileOnCanvas(x, y);
                }
            }
        }

        private void Canvas_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas_OnMouseMove(sender, e);
        }

        private void Canvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RepaintCanvas();
        }

        private void Canvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (ListToolsListBox.SelectedIndex == -1) return;
                isSaved = false;
                PlayerCheck();
                LevelMap.Draw(e.GetPosition(Canvas), (Tool)ListToolsListBox.SelectedItem);
            }
        }

        private void PlayerCheck()
        {
            if (ListToolsListBox.SelectedIndex == -1) return;
            if (((Tool)ListToolsListBox.SelectedItem).Name == "Player")
            {
                for (int x = 0; x < LevelMap.Level.GetLength(0); x++)
                {
                    for (int y = 0; y < LevelMap.Level.GetLength(1); y++)
                    {
                        if (LevelMap.Level[x, y].Name == "Player")
                        {
                            LevelMap.Level[x, y] = (Tool)ListToolsListBox.Items[0];
                        }
                    }
                }
                RepaintCanvas();
            }
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://canvas.sae.edu/courses/4016/files/209302/download?download_frd=1");
        }

        private void AddToolButton_Click(object sender, RoutedEventArgs e)
        {
            CreateNewTool createNewTool = new CreateNewTool();
            if (createNewTool.ShowDialog() == true)
            {
                LevelMap.tools.Add(new Tool(createNewTool.ToolName, createNewTool.ToolColor, createNewTool.ToolToolTip));
            }
            ListToolsListBox.Items.Refresh();
        }

        private void RemoveToolButton_Click(object sender, RoutedEventArgs e)
        {
            if (ListToolsListBox.SelectedIndex == -1) return;
            if (ListToolsListBox.SelectedIndex <= 4)
            {
                MessageBox.Show("You can't delete default tools");
                return;
            }
            LevelMap.tools.RemoveAt(ListToolsListBox.SelectedIndex);
            ListToolsListBox.Items.Refresh();
        }

        private void RecentFile_Click(object sender, RoutedEventArgs e)
        {
            Load(((MenuItem)sender).Header.ToString());
        }

        private void AddFileToRecentFiles(string newFile)
        {
            if (string.IsNullOrWhiteSpace(newFile)) return;

            // reorder exist
            if ((string)RecentFile1Button.Header == newFile) return;
            for (int i = 0; i < recentFilesList.Length; i++)
            {
                if ((string)recentFilesList[i].Header == newFile)
                {
                    for (int j = i; j > 0; j--)
                    {
                        recentFilesList[j].Header = (string)recentFilesList[j - 1].Header;
                    }
                    RecentFile1Button.Header = newFile;
                    return;
                }
            }

            //list not full
            foreach (MenuItem recentFile in recentFilesList)
            {
                if (recentFile.IsEnabled) continue;
                recentFile.Header = newFile;
                recentFile.IsEnabled = true;
                recentFile.Visibility = Visibility.Visible;
                return;
            }

            // list full
            for (int i = recentFilesList.Length - 1; i > 0; i--)
            {
                recentFilesList[i].Header = recentFilesList[i - 1].Header;
            }
            RecentFile1Button.Header = newFile;
        }

        private void Window_OnClosing(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Close application without save file?", "Without Save?", MessageBoxButton.YesNo) == MessageBoxResult.No) e.Cancel = true;
            WriteRecentFileToIni();
        }

        private void WriteRecentFileToIni()
        {
            using (StreamWriter writer =
                new StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + iniFile))
            {
                foreach (MenuItem recentFile in recentFilesList)
                {
                    if (!recentFile.IsEnabled) continue;
                    writer.WriteLine(recentFile.Header.ToString());
                }
            }
        }

        private void ReadRecentFileFromIni()
        {
            if (!File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + iniFile)) return;
            using (StreamReader reader = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + iniFile))
            {
                do
                {
                    AddFileToRecentFiles(reader.ReadLine());
                } while (!reader.EndOfStream);
            }
        }
    }
}
