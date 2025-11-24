using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace VsBrushesColorPicker
{
    [Command(PackageIds.ColorPickerCommand)]
    internal sealed class VwsBrushesColorPickerCommand : BaseCommand<VwsBrushesColorPickerCommand>
    {
        protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
        {   
            return ColorPickerToolWindow.ShowAsync();
        }
    }
}
