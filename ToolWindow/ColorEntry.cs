using System.Windows.Media;

namespace VsBrushesColorPicker
{
    public class ColorEntry
    {
        public string Name { get; set; }
        public Brush Brush { get; set; }
        public Color Color { get; set; }
        public string Type { get; set; }
        public string InClass { get; set; }
        public string ArgbHex => $"#{Color.A:X2}{Color.R:X2}{Color.G:X2}{Color.B:X2}";
    }
}