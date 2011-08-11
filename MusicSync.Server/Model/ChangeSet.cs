using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using libMusicSync.iTunesExport.Parser;

namespace WifiSyncServer.Model
{
    /// <summary>
    /// A set of changes
    /// </summary>
    [Serializable]
    public class ChangeSet
    {

        public ChangeSet()
        {
            AddChanges= new List<string>();
            RemoveChanges = new List<Track>();
        }

        public List<string> AddChanges { get; set; }
        public List<Track> RemoveChanges { get; set; }

        [XmlIgnore]
        public bool IsEmpty
        {
            get
            {
                return ((AddChanges == null) || (AddChanges.Count == 0)) 
                    && ((RemoveChanges == null) || (RemoveChanges.Count == 0));
            }
        }
    }
}
