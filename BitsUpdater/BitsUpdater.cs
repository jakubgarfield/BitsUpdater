using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BitsUpdater.Behavior;
using SharpBits.Base;
using System.Net;
using System.Xml.Serialization;
using System.Reflection;
using BitsUpdater.Extensions;
using System.IO;

namespace BitsUpdater
{
    public sealed class BitsUpdater : IDisposable
    {
        private static readonly XmlSerializer _manifestSerializer = new XmlSerializer(typeof(XmlUpdateManifest));

        private const String JobDescription = "BitsUpdater application update download";
        private const String UpdateDirectory = "Versions\\{0}";

        private readonly UpdateStatus _status = UpdateStatus.Load();
        private readonly BitsManager _manager = new BitsManager();
        private readonly WebClient _webClient = new WebClient();
        private readonly String _manifestUrl;

        private XmlUpdateManifest _manifest;

        public event EventHandler<UpdateCheckedEventArgs> UpdateChecked;
        public event EventHandler<UpdateDownloadedEventArgs> UpdateDownloaded;
        public event EventHandler<UpdateErrorEventArgs> UpdateDownloadError;
        public event EventHandler<UpdateProgressEventArgs> UpdateDownloadProgressChanged;

        public BitsUpdater(string manifestUrl)
        {
            _manifestUrl = manifestUrl;
        }

        /// <summary>
        /// Checks for new updates.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when manifestUrl is null.</exception>
        /// <exception cref="WebException">Thrown when manifestUrl is not valid url or client can't connect to specified url.</exception>
        /// <returns>True if new updates are available. False if there are no new updates or downloaded UpdateManifest couldn't be deserialized.</returns>
        public bool CheckUpdate()
        {
            _manifest = _manifestSerializer.Deserialize(_webClient.OpenRead(_manifestUrl)) as XmlUpdateManifest;
            return UpdatesAvailable();
        }

        /// <summary>
        /// Checks for new updates asynchronnously. After update check is finished, event UpdateChecked is raised.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when manifestUrl is null.</exception>
        /// <exception cref="WebException">Thrown when manifestUrl is not valid url or client can't connect to specified url.</exception>
        public void CheckUpdateAsync()
        {
            _webClient.OpenReadCompleted += (sender, args) =>
            {
                _manifest = _manifestSerializer.Deserialize(args.Result) as XmlUpdateManifest;
                OnUpdateChecked(new UpdateCheckedEventArgs(UpdatesAvailable()));
            };
            _webClient.OpenReadAsync(new Uri(_manifestUrl));
        }

        /// <summary>
        /// Starts downloading updates using BITS. If new updates are downloaded event UpdateDownloaded is raised. If hadn't checked for updates, it uses synchronous method CheckUpdates.
        /// </summary>
        /// <returns>Returns true if new updates are available and starts downloading.</returns>
        public bool Download()
        {
            if ((_manifest == null) ? CheckUpdate() : UpdatesAvailable())
            {
                _status.NextVersion = new Version(_manifest.Version);
                _status.Save();
                StartDownload();
                return true;
            }
            return false;
        }

        public void ResumePreviousDownload()
        {
            if (_status.BitsJobId != Guid.Empty)
            {
                if (!IsUpdateDownloaded())
                {
                    if (_manager.EnumJobs().ContainsKey(_status.BitsJobId))
                    {
                        var job = _manager.Jobs[_status.BitsJobId];
                        RegisterEvents(job);
                        job.Resume();
                    }
                    else
                    {
                        _status.NextVersion = new Version();
                        _status.BitsJobId = Guid.Empty;
                        _status.Save();
                    }
                }
            }
        }

        public bool Update(string publicToken)
        {
            return Update(null, publicToken);
        }

        public bool Update(IUpdateBehavior behavior, string publicToken)
        {
            if (_status.NextVersion > Assembly.GetExecutingAssembly().GetName().Version && File.Exists(GetUpdateSaveLocation()))
            {
                UpdatePackage.Extract(string.Format(UpdateDirectory, _status.NextVersion), _status.NextVersion, publicToken);

                if (behavior != null)
                {
                    behavior.Execute();
                }

                return true;
            }

            return false;
        }

        public void Dispose()
        {
            _manager.Dispose();
        }

        private void StartDownload()
        {
            if (!IsUpdateDownloaded())
            {
                var job = _manager.CreateJob(JobDescription, JobType.Download);
                job.AddFile(_manifest.Url, GetUpdateSaveLocation());
                RegisterEvents(job);
                _status.BitsJobId = job.JobId;
                _status.Save();
                job.Resume();
            }
        }

        private bool IsUpdateDownloaded()
        {
            if (File.Exists(GetUpdateSaveLocation()))
            {
                OnUpdateDownloaded(new UpdateDownloadedEventArgs());
                return true;
            }
            return false;
        }

        private void RegisterEvents(BitsJob job)
        {
            job.OnJobError += (s, e) =>
                {
                    _status.NextVersion = new Version();
                    _status.BitsJobId = Guid.Empty;
                    _status.Save();
                    OnUpdateDownloadError(new UpdateErrorEventArgs(e.Error));
                    job.Cancel();
                    job.Dispose();
                };

            job.OnJobTransferred += (s, e) =>
                {
                    job.Complete();
                    job.Dispose();
                    OnUpdateDownloaded(new UpdateDownloadedEventArgs());
                };

            job.OnJobModified += (s, e) =>
                {
                    if (job != null && job.State == JobState.Transferring)
                    {
                        if (job.Progress != null)
                        {
                            OnUpdateDownloadProgressChanged(new UpdateProgressEventArgs(job.Progress.BytesTransferred, job.Progress.BytesTotal));
                        }
                    }
                };
        }

        private String GetUpdateSaveLocation()
        {
            return Path.Combine(Assembly.GetExecutingAssembly().GetDirectory(), String.Format("{0}{1}{2}", string.Format(UpdatePackage.AssemblyName, _status.NextVersion), UpdatePackage.AssemblySuffix, UpdatePackage.PackageSuffix));
        }

        private bool UpdatesAvailable()
        {
            if (_manifest != null)
            {
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                var manifestVersion = new Version(_manifest.Version);
                return (manifestVersion > currentVersion && _status.NextVersion < manifestVersion);
            }

            return false;
        }

        private void OnUpdateChecked(UpdateCheckedEventArgs e)
        {
            if (UpdateChecked != null)
            {
                UpdateChecked(this, e);
            }
        }

        private void OnUpdateDownloaded(UpdateDownloadedEventArgs e)
        {
            if (UpdateDownloaded != null)
            {
                UpdateDownloaded(this, e);
            }
        }

        private void OnUpdateDownloadError(UpdateErrorEventArgs e)
        {
            if (UpdateDownloadError != null)
            {
                UpdateDownloadError(this, e);
            }
        }

        private void OnUpdateDownloadProgressChanged(UpdateProgressEventArgs e)
        {
            if (UpdateDownloadProgressChanged != null)
            {
                UpdateDownloadProgressChanged(this, e);
            }
        }
    }
}
