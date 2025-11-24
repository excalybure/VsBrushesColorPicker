using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace VsBrushesColorPicker
{
    [Guid("3cf30e56-4d35-4b52-9d25-5d8c7309b9b0")]
    public class ColorListToolWindow : ToolWindowPane
    {
        public ColorListToolWindow() : base(null)
        {
            Caption = "VS Brushes Color List";
            Content = new ColorListControl();
        }
    }
}
