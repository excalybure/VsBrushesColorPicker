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
        private List<string> _searchHistory;
        private const int MaxHistoryItems = 10;

        public ColorPickerToolWindowControl()
        {
            InitializeComponent();
            _searchHistory = new List<string>();
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
                        var color = (brush as SolidColorBrush)?.Color ?? Colors.Transparent;
                        _all.Add(new ColorEntry
                        {
                            Name = prop.Name,
                            Brush = brush,
                            Color = color,
                            InClass = "VsBrushes",
                            Type = "SolidColorBrush"
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

        private void FilterBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                AddToSearchHistory(FilterBox.Text);
            }
            ApplyFilter();
        }

        private void FilterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void AddToSearchHistory(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return;
            }

            // Remove if already exists to avoid duplicates
            _searchHistory.Remove(searchText);
            
            // Add to the beginning of the list
            _searchHistory.Insert(0, searchText);
            
            // Keep only the most recent items
            if (_searchHistory.Count > MaxHistoryItems)
            {
                _searchHistory.RemoveAt(_searchHistory.Count - 1);
            }
            
            // Update ComboBox items
            FilterBox.ItemsSource = null;
            FilterBox.ItemsSource = _searchHistory;
        }

        private void ColorList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePreview(ColorList.SelectedItem as ColorEntry);
        }

        private void ColorList_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.C && 
                (System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Control) == System.Windows.Input.ModifierKeys.Control)
            {
                var selectedEntry = ColorList.SelectedItem as ColorEntry;
                if (selectedEntry != null)
                {
                    try
                    {
                        Clipboard.SetText(selectedEntry.Name);
                        e.Handled = true;
                    }
                    catch (Exception ex)
                    {
                        // Clipboard operations can fail, ignore errors
                        System.Diagnostics.Debug.WriteLine($"Failed to copy to clipboard: {ex.Message}");
                    }
                }
            }
        }

        private void UpdatePreview(ColorEntry entry)
        {
            if (entry == null)
            {
                Preview.Background = Brushes.Transparent;
                NameText.Text = string.Empty;
                ArgbText.Text = string.Empty;
                ClassText.Text = string.Empty;
                TypeText.Text = string.Empty;
                return;
            }

            Preview.Background = entry.Brush;
            NameText.Text = "Name: " + entry.Name;
            ArgbText.Text = "ARGB: " + entry.ArgbHex;
            ClassText.Text = "In class: " + entry.InClass;
            TypeText.Text = "Type: " + entry.Type;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadColors();
        }
    }
}
