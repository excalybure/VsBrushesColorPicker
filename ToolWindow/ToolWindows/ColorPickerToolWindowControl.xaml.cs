using Community.VisualStudio.Toolkit;
using System;
using System.Windows;
using System.Windows.Controls;

namespace VsBrushesColorPicker
{
    public partial class ColorPickerToolWindowControl : UserControl
    {
        public ColorPickerToolWindowControl(Version vsVersion)
        {
            InitializeComponent();

            lblHeadline.Content = $"Visual Studio v{vsVersion}";
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            VS.MessageBox.Show("ColorPickerToolWindow", "Button clicked");
        }
    }
}