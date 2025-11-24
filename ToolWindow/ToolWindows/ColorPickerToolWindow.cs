using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;

namespace VsBrushesColorPicker
{
    public class ColorPickerToolWindow : BaseToolWindow<ColorPickerToolWindow>
    {
        public override string GetTitle(int toolWindowId) => "VsBrushes Color Picker";
        public override Type PaneType => typeof(Pane);

        public override async Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            return new ColorPickerToolWindowControl();
        }

        [Guid("03030460-e1a2-49ab-a4c5-b7b9cfc2a4df")]
        public class Pane : ToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.ToolWindow;
            }
        }
    }
}