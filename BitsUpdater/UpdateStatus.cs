using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace BitsUpdater
{
    internal sealed class UpdateStatus
    {
        private static readonly XmlSerializer _xmlUpdateStatusSerializer = new XmlSerializer(typeof(XmlUpdateStatus));
        private XmlUpdateStatus _updateStatus = new XmlUpdateStatus();
        private const string UpdateStatusFileName = "UpdateStatus.xml";
        private Guid _jobId;
        private Version _nextVersion;

        public Version NextVersion
        {
            get
            {
                return _nextVersion;
            }
            set
            {
                _nextVersion = value;
                _updateStatus.NextVersion = _nextVersion.ToString();
            }
        }

        public Guid BitsJobId
        {
            get
            {
                return _jobId;
            }
            set
            {
                _jobId = value;
                _updateStatus.BitsJobId = _jobId.ToString();
            }
        }

        private UpdateStatus()
        {
        }

        public static UpdateStatus Load()
        {
            var updateStatus = new UpdateStatus();

            try
            {
                using (var xmlFile = new FileStream(UpdateStatus.UpdateStatusFileName, FileMode.Open, FileAccess.Read))
                {
                    updateStatus._updateStatus = (XmlUpdateStatus)_xmlUpdateStatusSerializer.Deserialize(xmlFile);
                }
            }
            catch (FileNotFoundException)
            {
                updateStatus.CreateDefault();
            }

            try
            {
                updateStatus._jobId = new Guid(updateStatus._updateStatus.BitsJobId);
                updateStatus._nextVersion = new Version(updateStatus._updateStatus.NextVersion);
            }
            catch (FormatException)
            {
                updateStatus.CreateDefault();
            }

            return updateStatus;
        }

        public void Save()
        {
            using (var file = new FileStream(UpdateStatusFileName, FileMode.OpenOrCreate))
            {
                _xmlUpdateStatusSerializer.Serialize(file, _updateStatus);
            }
        }

        private void CreateDefault()
        {
            BitsJobId = Guid.Empty;
            NextVersion = new Version();
            Save();
        }
    }
}
