using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Labels
{
    internal class Storage
    {
        public void SaveElementLocations(IEnumerable<NamedDesktopPoint> iconPositions)
        {
            var xDoc = new XDocument(
                new XElement("Desktop",
                    new XElement("Elements",
                        iconPositions.Select(p => new XElement("Element",
                            new XAttribute("x", p.X),
                            new XAttribute("y", p.Y),
                            new XText(p.Name))))));

            using (var storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                if (storage.FileExists("Desktop"))
                { storage.DeleteFile("Desktop"); }

                using (var stream = storage.CreateFile("Desktop"))
                {
                    using (var writer = XmlWriter.Create(stream))
                    {
                        xDoc.WriteTo(writer);
                    }
                }
            }
        }

        public IEnumerable<NamedDesktopPoint> GetElementsLocations()
        {
            using (var storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                if (storage.FileExists("Desktop") == false)
                { return new NamedDesktopPoint[0]; }

                using (var stream = storage.OpenFile("Desktop", FileMode.Open))
                {
                    using (var reader = XmlReader.Create(stream))
                    {
                        var xDoc = XDocument.Load(reader);

                        return xDoc.Root.Element("Elements").Elements("Element")
                            .Select(el => new NamedDesktopPoint(el.Value, int.Parse(el.Attribute("x").Value), int.Parse(el.Attribute("y").Value)))
                            .ToArray();
                    }
                }
            }
        }


    }
}
