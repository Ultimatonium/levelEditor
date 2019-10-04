using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace LevelEditor
{
    public class LevelMap
    {
        public List<Tool> tools;

        public Tool[,] Level { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        private MainWindow mainWindow;

        public const int TileWidth = 32;
        public const int TileHeight = 32;

        public void Init(MainWindow mainWindow, int height, int width)
        {
            this.mainWindow = mainWindow;
            Height = height;
            Width = width;
            tools = new List<Tool>(Tool.DefaultTools());
            mainWindow.ListToolsListBox.ItemsSource = tools;
            mainWindow.ListToolsListBox.SelectedIndex = 0;
            InitLevel();
            mainWindow.RepaintCanvas();
        }

        private void InitLevel()
        {
            Level = new Tool[Width, Height];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Level[i, j] = tools[0];
                }
            }
        }

        public void Draw(Point point, Tool tool)
        {
            int x = (int)(point.X / TileWidth);
            int y = (int)(point.Y / TileHeight);
            Level[x, y] = tool;
            mainWindow.DrawTileOnCanvas(x, y);
        }

        public void Load(string fileName, MainWindow mainWindow)
        {
            if (!File.Exists(fileName))
            {
                MessageBox.Show($"File \"{fileName}\" does not exist");
                return;
            }
            LevelMap newLevelMap = LoadFromFile(fileName, mainWindow);
            Width = newLevelMap.Width;
            Height = newLevelMap.Height;
            Level = newLevelMap.Level;
            tools = newLevelMap.tools;
            mainWindow.isSaved = true;
        }

        public static LevelMap LoadFromFile(string fileName, MainWindow mainWindow)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                LevelMap newLevelMap = new LevelMap();
                string[] elements = reader.ReadLine().Split(';');
                newLevelMap.Init(mainWindow, int.Parse(elements[1]), int.Parse(elements[0]));
                newLevelMap.tools.Clear();
                int iTools = int.Parse(reader.ReadLine());
                for (int i = 0; i < iTools; i++)
                {
                    newLevelMap.tools.Add(deserializeTool(reader.ReadLine()));
                }
                for (int i = 0; i < newLevelMap.Width; i++)
                {
                    for (int j = 0; j < newLevelMap.Height; j++)
                    {
                        newLevelMap.Level[i, j] = deserializeTool(reader.ReadLine());
                    }
                }
                return newLevelMap;
            }
        }

        public void Save(string fileName)
        {
            SaveToFile(fileName, this);
            mainWindow.isSaved = true;
        }

        public static void SaveToFile(string fileName, LevelMap currentLevelMap)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine($"{currentLevelMap.Width};{currentLevelMap.Height}");
                writer.WriteLine(currentLevelMap.tools.Count);
                foreach (Tool tool in currentLevelMap.tools)
                {
                    writer.WriteLine(serializeTool(tool));
                }
                for (int x = 0; x < currentLevelMap.Level.GetLength(0); x++)
                {
                    for (int y = 0; y < currentLevelMap.Level.GetLength(1); y++)
                    {
                        writer.WriteLine(serializeTool(currentLevelMap.Level[x, y]));
                    }
                }
            }
        }

        private static string serializeTool(Tool tool)
        {
            return (
                $"{tool.Name};{tool.Color.Color.A};{tool.Color.Color.R};{tool.Color.Color.G};{tool.Color.Color.B};{tool.ToolTip}"
            );
        }

        private static Tool deserializeTool(string toolString)
        {
            string[] tmpString = toolString.Split(';');
            Color tmpColor = new Color();
            tmpColor.A = byte.Parse(tmpString[1]);
            tmpColor.R = byte.Parse(tmpString[2]);
            tmpColor.G = byte.Parse(tmpString[3]);
            tmpColor.B = byte.Parse(tmpString[4]);
            return new Tool(tmpString[0], new SolidColorBrush(tmpColor), tmpString[5]);
        }
    }
}
