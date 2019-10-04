using System.Windows.Media;

namespace LevelEditor
{
    public class Tool
    {
        public string Name { get; set; }
        public SolidColorBrush Color { get; set; }
        public string ToolTip { get; set; }

        public Tool(string name, SolidColorBrush color, string toolTip)
        {
            this.Name = name;
            this.Color = color;
            this.ToolTip = toolTip;
        }

        public static Tool[] DefaultTools()
        {
            return new Tool[]
            {
                new Tool("Floor", new SolidColorBrush(Colors.Gray), ""),
                new Tool("Wall", new SolidColorBrush(Colors.Black), "" ),
                new Tool("Player", new SolidColorBrush(Colors.Green), "just once available" ),
                new Tool("Enemy", new SolidColorBrush(Colors.Red), "" ),
                new Tool("Loot", new SolidColorBrush(Colors.Gold), "")
            };
        }
    }
}
