namespace Microsoft.WPSync.UI.ViewModels
{
    using Microsoft.WPSync.Shared;
    using Microsoft.WPSync.Sync.Engine;
    using Microsoft.WPSync.Sync.Rules;
    using Microsoft.WPSync.UI.Properties;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Runtime.CompilerServices;

    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public sealed class ErrorsViewModel : DialogViewModel
    {
        public ErrorsViewModel(IEnumerable<IOperationResult> syncErrors)
        {
            this.OperationErrors = syncErrors;
        }

        public override void Commit()
        {
        }

        private static string ErrorFormatForOperation(IOperationResult result)
        {
            SourceResult result2 = result as SourceResult;
            if (result2 != null)
            {
                switch (result2.Operation)
                {
                    case SourceOperationType.LoadingCache:
                        return null;

                    case SourceOperationType.LoadingCacheFile:
                        return Resources.ErrLoadingCache;

                    case SourceOperationType.Verifying:
                        return null;

                    case SourceOperationType.VerifyingFile:
                        return Resources.ErrVerifyingFile;

                    case SourceOperationType.FileSystemChange:
                        return Resources.ErrFileSystemChange;
                }
                return Resources.UnknownOperation;
            }
            SyncResult result3 = result as SyncResult;
            if (result3 != null)
            {
                switch (result3.Operation)
                {
                    case SyncOperationType.TransferTo:
                        return Resources.ErrCopyToPhone;

                    case SyncOperationType.TransferFrom:
                        return Resources.ErrCopyToPc;

                    case SyncOperationType.DeferredTransferTo:
                        return Resources.ErrTranscode;

                    case SyncOperationType.Update:
                        return Resources.ErrUpdatePhone;

                    case SyncOperationType.Delete:
                        return Resources.ErrDeleteFromPhone;
                }
            }
            return Resources.UnknownOperation;
        }

        public override void Init()
        {
            this.ItemErrorStrings = new List<string>();
            string str3 = null;
            foreach (OperationResult result in this.OperationErrors)
            {
                string format = ErrorFormatForOperation(result);
                if (format == null)
                {
                    format = ResultMapping.LocalizedResultString((ResultCode) result.ResultCode);
                }
                else
                {
                    str3 = ResultMapping.LocalizedResultString((ResultCode) result.ResultCode);
                }
                string item = string.Format(CultureInfo.CurrentUICulture, format, new object[] { result.Details, str3 });
                this.ItemErrorStrings.Add(item);
            }
        }

        public ICollection<string> ItemErrorStrings { get; private set; }

        private IEnumerable<IOperationResult> OperationErrors { get; set; }
    }
}

