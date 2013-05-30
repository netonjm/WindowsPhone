namespace Microsoft.WPSync.UI.ViewModels
{
    using Microsoft.WPSync.Logging;
    using Microsoft.WPSync.Shared;
    using Microsoft.WPSync.Sync.Engine;
    using Microsoft.WPSync.Sync.Rules;
    using Microsoft.WPSync.UI;
    using Microsoft.WPSync.UI.Models;
    using Microsoft.WPSync.UI.Properties;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;

    public class SyncProgressViewModel : PropChangeNotifier, ISyncProgressViewModel
    {
        private string _caption;
        private string _header;
        private string _subHeader;
        private readonly IMainController controller;
        private ISyncPartnership currentPartnership;
        private float currentProgress;
        private bool isProgressLocked;
        private float progressLeft;
        private static SpinLock spinLock = new SpinLock();
        private System.Windows.Visibility visibility = System.Windows.Visibility.Hidden;

        public SyncProgressViewModel(IMainController controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }
            this.controller = controller;
            controller.PropertyChanged += new PropertyChangedEventHandler(this.OnControllerPropertyChanged);
        }

        public bool AddProgress(float percent)
        {
            this.CurrentProgress += percent;
            this.ProgressLeft -= percent;
            return true;
        }

        public void ClearProgress()
        {
            this.Header = string.Empty;
            this.SubHeader = string.Empty;
            this.Caption = string.Empty;
            this.SetProgress(0f, 100f);
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        private void OnContentLoading(object sender, ContentLoadingEventArgs args)
        {
            string subHeader = string.Empty;
            if (args.RemainingCount >= 0)
            {
                subHeader = string.Format(CultureInfo.CurrentCulture, Resources.SyncProgressRemainingItems, new object[] { args.RemainingCount });
            }
            this.UpdateProgressBar(args.Header, subHeader, args.Caption, args.PercentComplete, 100f, false);
        }

        private void OnControllerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string str;
            if (((str = e.PropertyName) != null) && (str == "CurrentSyncPartnership"))
            {
                this.OnPartnershipChanged();
            }
        }

        private void OnPartnershipChanged()
        {
            if (this.currentPartnership != null)
            {
                if (this.currentPartnership.Engine != null)
                {
                    this.currentPartnership.Engine.SyncProgressEvent -= new SyncEngine.SyncProgressDelegate(this.OnSyncProgress);
                    this.currentPartnership.Engine.SendItemProgressEvent -= new SendItemProgressDelegate(this.AddProgress);
                }
                this.currentPartnership.StateChanged -= new EventHandler<PartnershipStateChangeEventArgs>(this.OnPartnershipStateChanged);
                this.currentPartnership.ContentLoadingProgress -= new EventHandler<ContentLoadingEventArgs>(this.OnContentLoading);
            }
            this.currentPartnership = this.controller.CurrentSyncPartnership;
            if (this.currentPartnership != null)
            {
                this.currentPartnership.Engine.SyncProgressEvent += new SyncEngine.SyncProgressDelegate(this.OnSyncProgress);
                this.currentPartnership.Engine.SendItemProgressEvent += new SendItemProgressDelegate(this.AddProgress);
                this.currentPartnership.StateChanged += new EventHandler<PartnershipStateChangeEventArgs>(this.OnPartnershipStateChanged);
                this.currentPartnership.ContentLoadingProgress += new EventHandler<ContentLoadingEventArgs>(this.OnContentLoading);
            }
            this.ResetModelState();
        }

        private void OnPartnershipStateChanged(object sender, PartnershipStateChangeEventArgs e)
        {
            string progressHeaderForSync;
            ISyncPartnership partnership = sender as ISyncPartnership;
            if (partnership != this.controller.CurrentSyncPartnership)
            {
                return;
            }
            switch (e.NewState)
            {
                case PartnershipState.PreparingSync:
                    progressHeaderForSync = string.Empty;
                    switch (partnership.SyncStartType)
                    {
                        case SyncStartType.AutoSync:
                        case SyncStartType.ManualSync:
                            progressHeaderForSync = Resources.ProgressHeaderForSync;
                            break;

                        case SyncStartType.Delete:
                            progressHeaderForSync = Resources.ProgressHeaderForDelete;
                            break;

                        case SyncStartType.CopyToPC:
                            progressHeaderForSync = Resources.ProgressHeaderForCopyToPC;
                            break;

                        case SyncStartType.SendRingtones:
                            progressHeaderForSync = Resources.ProgressHeaderForRingtones;
                            break;
                    }
                    break;

                case PartnershipState.CancelingSync:
                    this.UpdateProgressBar(Resources.CancellingSyncText, string.Empty, string.Empty, 100f, 100f, true);
                    goto Label_00CB;

                case PartnershipState.Idle:
                    this.IsProgressLocked = false;
                    goto Label_00CB;

                default:
                    goto Label_00CB;
            }
            this.UpdateProgressBar(progressHeaderForSync, string.Empty, string.Empty, 0f, 100f, false);
        Label_00CB:
            this.ResetModelState();
        }

        private void OnSyncProgress(ISyncable item, SyncEngine.ProgressType type, int filesTransferred, int totalItems)
        {
            using (new OperationLogger())
            {
                string displayName = null;
                string format = null;
                if (item != null)
                {
                    displayName = item.Properties.ObjectForKey("Name") as string;
                    if (string.IsNullOrEmpty(displayName))
                    {
                        ISyncableMedia media = item as ISyncableMedia;
                        if (media != null)
                        {
                            displayName = media.DisplayName;
                        }
                        if (string.IsNullOrEmpty(displayName) && (item.Location != null))
                        {
                            displayName = item.Location.LocalPath;
                        }
                    }
                }
                switch (type)
                {
                    case SyncEngine.ProgressType.Add:
                    case SyncEngine.ProgressType.Update:
                        format = Resources.TransferProgressCopyText;
                        break;

                    case SyncEngine.ProgressType.Delete:
                        format = Resources.TransferProgressDeleteText;
                        break;

                    case SyncEngine.ProgressType.WaitingForDeferred:
                        format = Resources.TranferProgressTranscoding;
                        break;

                    case SyncEngine.ProgressType.Retrying:
                        format = Resources.TransferProgressRetryText;
                        break;

                    default:
                        throw new ArgumentException("invalid value", "type");
                }
                string caption = string.Format(CultureInfo.CurrentCulture, format, new object[] { string.IsNullOrEmpty(displayName) ? Resources.UnknownFileNameText : displayName });
                string subHeader = string.Format(CultureInfo.CurrentCulture, Resources.SyncProgressRemainingItems, new object[] { totalItems - filesTransferred });
                this.UpdateProgressBar(null, subHeader, caption, (float) (filesTransferred * 100), (float) (totalItems * 100), false);
            }
        }

        private void ResetModelState()
        {
            ISyncPartnership currentSyncPartnership = this.controller.CurrentSyncPartnership;
            if ((currentSyncPartnership != null) && ((((currentSyncPartnership.CurrentState == PartnershipState.Syncing) || (currentSyncPartnership.CurrentState == PartnershipState.CancelingSync)) || ((currentSyncPartnership.CurrentState == PartnershipState.PreparingSync) || (currentSyncPartnership.CurrentState == PartnershipState.LoadingSources))) || (currentSyncPartnership.CurrentState == PartnershipState.VerifyingSources)))
            {
                this.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.Visibility = System.Windows.Visibility.Hidden;
                this.ClearProgress();
            }
            CommandManager.InvalidateRequerySuggested();
        }

        public void SetProgress(float current, float maximum)
        {
            bool flag;
            bool flag2;
            this.SetProgress(current, maximum, out flag, out flag2);
            if (flag)
            {
                this.OnPropertyChanged("CurrentProgress");
            }
            if (flag2)
            {
                this.OnPropertyChanged("ProgressLeft");
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId="2#"), SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId="3#")]
        public void SetProgress(float current, float maximum, out bool currentChanged, out bool leftChanged)
        {
            if (current == maximum)
            {
                if (this.progressLeft == 0f)
                {
                    leftChanged = false;
                }
                else
                {
                    this.progressLeft = 0f;
                    leftChanged = true;
                }
                if (this.currentProgress == 1f)
                {
                    currentChanged = false;
                }
                else
                {
                    this.currentProgress = 1f;
                    currentChanged = true;
                }
            }
            else
            {
                if (this.progressLeft == 1f)
                {
                    leftChanged = false;
                }
                else
                {
                    this.progressLeft = 1f;
                    leftChanged = true;
                }
                float num = current / maximum;
                float num2 = num / (1f - num);
                if (this.currentProgress == num2)
                {
                    currentChanged = false;
                }
                else
                {
                    this.currentProgress = num2;
                    currentChanged = true;
                }
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void UpdateProgressBar(string header, string subHeader, string caption, float current, float maximum, bool lockProgress = false)
        {
            bool lockTaken = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            bool currentChanged = false;
            bool leftChanged = false;
            try
            {
                spinLock.Enter(ref lockTaken);
                if (this.isProgressLocked)
                {
                    return;
                }
                this.isProgressLocked = lockProgress;
                if (header != null)
                {
                    this._header = header;
                    flag2 = true;
                }
                if (subHeader != null)
                {
                    this._subHeader = subHeader;
                    flag3 = true;
                }
                if ((caption != null) || (this._caption != null))
                {
                    this._caption = caption;
                    flag4 = true;
                }
                this.SetProgress(current, maximum, out currentChanged, out leftChanged);
            }
            finally
            {
                if (lockTaken)
                {
                    spinLock.Exit();
                }
            }
            if (flag2)
            {
                this.OnPropertyChanged("Header");
            }
            if (flag3)
            {
                this.OnPropertyChanged("SubHeader");
            }
            if (flag4)
            {
                this.OnPropertyChanged("Caption");
            }
            if (currentChanged)
            {
                this.OnPropertyChanged("CurrentProgress");
            }
            if (leftChanged)
            {
                this.OnPropertyChanged("ProgressLeft");
            }
        }

        public string Caption
        {
            get
            {
                return this._caption;
            }
            set
            {
                this._caption = value;
                this.OnPropertyChanged("Caption");
            }
        }

        public float CurrentProgress
        {
            get
            {
                return this.currentProgress;
            }
            set
            {
                this.currentProgress = value;
                this.OnPropertyChanged("CurrentProgress");
            }
        }

        public string Header
        {
            get
            {
                return this._header;
            }
            set
            {
                if (this._header != value)
                {
                    this._header = value;
                    this.OnPropertyChanged("Header");
                }
            }
        }

        public bool IsProgressLocked
        {
            get
            {
                return this.isProgressLocked;
            }
            set
            {
                this.isProgressLocked = value;
            }
        }

        public float ProgressLeft
        {
            get
            {
                return this.progressLeft;
            }
            set
            {
                this.progressLeft = value;
                this.OnPropertyChanged("ProgressLeft");
            }
        }

        public string SubHeader
        {
            get
            {
                return this._subHeader;
            }
            set
            {
                this._subHeader = value;
                this.OnPropertyChanged("SubHeader");
            }
        }

        public System.Windows.Visibility Visibility
        {
            get
            {
                return this.visibility;
            }
            set
            {
                this.visibility = value;
                this.OnPropertyChanged("Visibility");
            }
        }
    }
}

