using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;
using Microsoft.VSFolders.FastTree;
using Microsoft.VSFolders.Models;
using Microsoft.VSFolders.Settings;
using Microsoft.VSFolders.ViewModels;

namespace Microsoft.VSFolders.Converters
{
    public class MultiConverter : List<IValueConverter>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.Aggregate(value, (r, c) => c.Convert(r, targetType, parameter, culture));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.Aggregate(value, (r, c) => c.ConvertBack(r, targetType, parameter, culture));
        }
    }

    public class FileDataToEffectivelyHiddenIndicationConverter : IValueConverter
    {
        public static bool IsEffectivelyHidden(FileData data)
        {
            if (data.IsHidden)
            {
                return true;
            }

            if (data.Parent != null)
            {
                if (IsEffectivelyHidden(data.Parent))
                {
                    return true;
                }
            }

            var settingsPage = VSFoldersPackage.GetDialogPage<GeneralSettingsPage>();
            var settingsPageTarget = settingsPage.OriginalTarget ?? settingsPage.DialogTarget ?? new GeneralSettings();
            var hidePatterns = settingsPageTarget.AlwaysHidePatterns ?? new List<WrappedString>();
            var hideMatchResult = hidePatterns.Where(x => !string.IsNullOrEmpty(x.Name))
                .Select(x => new Regex(Regex.Escape(x.Name.ToLowerInvariant()).Replace("\\*", ".*").Replace("\\?", ".*") + "$"))
                .Any(x => x.IsMatch(data.FullPath.ToLowerInvariant()));

            return hideMatchResult;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = value as TreeNode<FileData>;

            if (data == null)
            {
                return null;
            }

            return IsEffectivelyHidden(data.Value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
