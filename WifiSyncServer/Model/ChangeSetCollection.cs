using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WifiSyncServer.Model
{
    public class ChangeSetCollection : List<ChangeSet>
    {
        public static ChangeSetCollection DeserializeOrCreate(string path)
        {
            ChangeSetCollection pendingChanges;
            if (File.Exists(path))
                pendingChanges = ChangeSetCollection.Deserialize(path);
            else
                pendingChanges = new ChangeSetCollection();

            return pendingChanges;
        }

        public static ChangeSetCollection Deserialize(string path)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(ChangeSetCollection));
            using (Stream s = File.OpenRead(path))
            {
                return deserializer.Deserialize(s) as ChangeSetCollection;
            }
        }

        public static void Serialize(ChangeSetCollection obj, string path)
        {
            string dirName = System.IO.Path.GetDirectoryName(path);

            if ((dirName != null) && !Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);
            else if (File.Exists(path))
                File.Delete(path);

            XmlSerializer serializer = new XmlSerializer(typeof(ChangeSetCollection));
            using (Stream s = File.OpenWrite(path))
            {
                serializer.Serialize(s, obj);
            }
        }
    }
}
