namespace Microsoft.WPSync.UI.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class ListViewItemStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            Style style = new Style {
                TargetType = typeof(ListBoxItem)
            };
            Setter setter = new Setter {
                Property = Control.BackgroundProperty
            };
            ListBox box = ItemsControl.ItemsControlFromItemContainer(container) as ListBox;
            switch (box.ItemContainerGenerator.IndexFromContainer(container))
            {
                case 0:
                    setter.Value = Brushes.LightBlue;
                    break;

                case 1:
                    setter.Value = Brushes.Green;
                    break;

                default:
                    setter.Value = Brushes.SaddleBrown;
                    break;
            }
            style.Setters.Add(setter);
            return style;
        }
    }
}

