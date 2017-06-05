using RSSNews.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace RSSNews.Services
{
    public class SourcesXMLHandler
    {
        string XMLPath;
        XDocument xSources;

        public SourcesXMLHandler(string XMLPath)
        {
            XmlReader xRead;
            this.XMLPath = XMLPath;

            if (!File.Exists(XMLPath))
            {
                CreateXML(XMLPath);
            }

            try
            {
                xRead = XmlReader.Create(XMLPath);
                try
                {
                    XElement xEle = XElement.Load(xRead);
                    xRead.Close();
                }
                catch (Exception)
                {
                    xRead.Close();
                    CreateXML(XMLPath);
                }
            }
            catch (Exception)
            {
                CreateXML(XMLPath);
            }

            xSources = XDocument.Load(XMLPath);
        }

        public static void CreateXMLIfNotExists(string XMLPath)
        {
            XmlReader xRead;

            if (!File.Exists(XMLPath))
            {
                CreateXML(XMLPath);
            }

            try
            {
                xRead = XmlReader.Create(XMLPath);
                try
                {
                    XElement xEle = XElement.Load(xRead);
                    xRead.Close();
                }
                catch (Exception)
                {
                    xRead.Close();
                    CreateXML(XMLPath);
                }
            }
            catch (Exception)
            {
                CreateXML(XMLPath);
            }
        }

        public static void CreateXML(string XMLPath)
        {
            XNamespace cntNM = "urn:lst-src:src";          

            XDocument xDoc = new XDocument(
                        new XDeclaration("1.0", "UTF-16", null),
                        new XElement("Sources"));

            StringWriter sw = new StringWriter();
            XmlWriter xWrite = XmlWriter.Create(sw);
            xDoc.Save(xWrite);
            xWrite.Close();

            xDoc.Element("Sources").Add(new XElement("Source",
                                new XElement("Address", "http://www.komputerswiat.pl/rss-feeds/komputer-swiat-feed.aspx"),
                                new XElement("Category", "Technologia")));
            xDoc.Element("Sources").Add(new XElement("Source",
                                new XElement("Address", "http://www.tvn24.pl/najnowsze.xml"),
                                new XElement("Category", "Informacje")));
            // Save to Disk
            xDoc.Save(XMLPath);
        }

        public void Save()
        {
            xSources.Save(XMLPath);
        }

        public List<SourceXML> GetEntries()
        {
            List<SourceXML> entries = new List<SourceXML>();

            foreach (var item in xSources.Descendants("Source"))
            {
                entries.Add(new SourceXML
                {
                    Address = item.Element("Address").Value,
                    Category = item.Element("Category").Value
                });
            }
            //return entries.Count > 0 ? entries : null;
            return entries;
        }

    }
}
