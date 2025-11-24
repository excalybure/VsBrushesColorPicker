using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VsBrushesColorPicker
{
    public partial class ColorListControl : UserControl
    {
        private List<ColorEntry> _all;

        public ColorListControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            LoadColors();
        }

        private void LoadColors()
        {
            _all = new List<ColorEntry>();
            var vsBrushesType = Type.GetType("Microsoft.VisualStudio.PlatformUI.VsBrushes, Microsoft.VisualStudio.Shell.15.0", throwOnError: false);
            if (vsBrushesType == null)
            {
                vsBrushesType = AppDomain.CurrentDomain.GetAssemblies()
                    .Select(a => a.GetType("Microsoft.VisualStudio.PlatformUI.VsBrushes", throwOnError: false))
                    .FirstOrDefault(t => t != null);
            }

            if (vsBrushesType == null)
            {
                ColorList.ItemsSource = Array.Empty<ColorEntry>();
                return;
            }

            foreach (var field in vsBrushesType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.FieldType == typeof(SolidColorBrush) || typeof(Brush).IsAssignableFrom(field.FieldType))
                {
                    if (field.GetValue(null) is Brush brush)
                    {
                        _all.Add(new ColorEntry
                        {
                            Name = field.Name,
                            Brush = brush,
                            InClass = "VsBrushes",
                            Type = field.FieldType.Name
                        });
                    }
                }
            }

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (_all == null)
            {
                return;
            }

            var filterText = FilterBox.Text.Trim();
            var filtered = string.IsNullOrEmpty(filterText)
                ? _all
                : _all.Where(c => c.Name.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0);

            var list = filtered.OrderBy(c => c.Name).ToList();
            ColorList.ItemsSource = list;
            if (list.Count > 0)
            {
                ColorList.SelectedIndex = 0;
            }
            else
            {
                UpdatePreview(null);
            }
        }

        private void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ColorList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePreview(ColorList.SelectedItem as ColorEntry);
        }

        private void UpdatePreview(ColorEntry entry)
        {
            if (entry == null)
            {
                Preview.Background = Brushes.Transparent;
                NameText.Text = string.Empty;
                ClassText.Text = string.Empty;
                TypeText.Text = string.Empty;
                return;
            }

            Preview.Background = entry.Brush;
            NameText.Text = "Name: " + entry.Name;
            ClassText.Text = "In class: " + entry.InClass;
            TypeText.Text = "Type: " + entry.Type;
        }
    }
}
