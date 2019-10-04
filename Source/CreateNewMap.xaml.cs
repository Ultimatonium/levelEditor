using System.Windows;
using System.Windows.Input;

namespace LevelEditor
{
    /// <summary>
    /// Interaction logic for CreateNewMap.xaml
    /// </summary>
    public partial class CreateNewMap : Window
    {
        public static RoutedCommand EnterCommand = new RoutedCommand();

        public int LevelHeight { get; set; }
        public int LevelWidth { get; set; }

        public CreateNewMap()
        {
            EnterCommand.InputGestures.Add(new KeyGesture(Key.Enter));
            InitializeComponent();
            LevelHeight = LevelWidth = 16;
            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
