using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IISURLRewriteUtility.Entities;
using System.Xml;

namespace IISURLRewriteUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                FileStream redirectFile = File.Open("redirects.csv", FileMode.Open, FileAccess.Read);

                List<Redirect> redirects = new List<Redirect>();

                using (StreamReader reader = new StreamReader(redirectFile))
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.IndentChars = "\t";

                    using (XmlWriter writer = XmlWriter.Create("rewriteRules.config", settings))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("rules");

                        writer.WriteStartElement("clear");
                        writer.WriteEndElement(); // end clear

                        while (reader.Peek() >= 0)
                        {
                            var redirectToAdd = new Redirect(reader.ReadLine());

                            var exists = false;

                            foreach (var redirect in redirects)
                            {
                                // Cannot redirect the same url to different target URLs
                                if (redirectToAdd.From == redirect.From || redirectToAdd.Path == redirect.Path)
                                {
                                    exists = true;
                                    break;
                                }
                            }

                            if (!exists)
                            {
                                // Add rule element
                                writer.WriteStartElement("rule");
                                writer.WriteAttributeString("name", redirectToAdd.From);
                                writer.WriteAttributeString("stopProcessing", "true");

                                // Add match element
                                writer.WriteStartElement("match");
                                writer.WriteAttributeString("Url", "(.*)");
                                writer.WriteEndElement(); // end match

                                // Add conditions element
                                writer.WriteStartElement("conditions");
                                writer.WriteAttributeString("logicalGrouping", "MatchAny");

                                // Add "add" element
                                writer.WriteStartElement("add");
                                writer.WriteAttributeString("input", "{URL}");
                                writer.WriteAttributeString("pattern", redirectToAdd.Path);
                                writer.WriteEndElement(); // end add

                                writer.WriteEndElement(); // end conditions

                                // Add action element
                                writer.WriteStartElement("action");
                                writer.WriteAttributeString("type", "Redirect");
                                writer.WriteAttributeString("url", redirectToAdd.To);
                                writer.WriteAttributeString("redirectType", "Permanent");
                                writer.WriteEndElement(); // end action

                                writer.WriteEndElement(); // end rule

                                redirects.Add(redirectToAdd);
                            }
                        }

                        writer.WriteEndElement(); // end rules
                        writer.WriteEndDocument();
                    }
                }
            }
            catch
            {
                Console.Write("File Does Not Exist");
            }
        }
    }
}
