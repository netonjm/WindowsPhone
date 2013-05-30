namespace Microsoft.WPSync.UI.ViewModels
{
    using System;
    using System.Collections;

    public sealed class LabelSorter : IComparer
    {
        private static IComparer stringComparer = new CaseInsensitiveComparer(CultureInfo.CurrentCulture);

        int IComparer.Compare(object x, object y)
        {
            SelectableOptionViewModel model = (SelectableOptionViewModel) x;
            SelectableOptionViewModel model2 = (SelectableOptionViewModel) y;
            if ((model != null) && (model2 != null))
            {
                return stringComparer.Compare(model.SortString, model2.SortString);
            }
            return 0;
        }
    }
}

