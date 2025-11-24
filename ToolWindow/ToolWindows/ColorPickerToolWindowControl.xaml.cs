using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VsBrushesColorPicker
{
    public partial class ColorPickerToolWindowControl : UserControl
    {
        private List<ColorEntry> _all;

        public ColorPickerToolWindowControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            VSColorTheme.ThemeChanged += OnThemeChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            LoadColors();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= OnUnloaded;
            VSColorTheme.ThemeChanged -= OnThemeChanged;
        }

        private void OnThemeChanged(ThemeChangedEventArgs e)
        {
            LoadColors();
        }

        private void LoadColors()
        {
            _all = new List<ColorEntry>();

            var brushType = typeof(VsBrushes);
            foreach (var prop in brushType.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                try
                {
                    var key = prop.GetValue(null);
                    if (key == null)
                    {
                        continue;
                    }
                    var resource = TryFindResource(key);
                    if (resource is Brush brush)
                    {
                        var color = (brush as SolidColorBrush)?.Color;
                        _all.Add(new ColorEntry
                        {
                            Name = prop.Name,
                            Brush = brush,
                            InClass = "VsBrushes",
                            Type = prop.ReflectedType.FullName
                        });
                    }
                }
                catch { }
                {
                    // Ignore exceptions for properties that cannot be accessed}
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

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadColors();
        }
    }
}
