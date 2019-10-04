using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace LevelEditor
{
    /// <summary>
    /// Interaction logic for CreateNewMap.xaml
    /// </summary>
    public partial class CreateNewTool : Window
    {
        public static RoutedCommand EnterCommand = new RoutedCommand();

        public string ToolName { get; set; }
        public SolidColorBrush ToolColor { get; set; }
        public string ToolToolTip { get; set; }

        public CreateNewTool()
        {
            EnterCommand.InputGestures.Add(new KeyGesture(Key.Enter));
            InitializeComponent();
            ToolColor = new SolidColorBrush(GetRandomColor());
            DataContext = this;
        }

        private Color GetRandomColor()
        {
            Color color = new Color();
            Byte[] tmpByte = new Byte[1];
            new Random().NextBytes(tmpByte);
            color.A = GetRandomByte();
            color.R = GetRandomByte();
            color.G = GetRandomByte();
            color.B = GetRandomByte();
            return color;
        }

        private Byte GetRandomByte()
        {
            Thread.Sleep(20);
            Byte[] tmpByte = new Byte[1];
            new Random().NextBytes(tmpByte);
            return tmpByte[0];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
