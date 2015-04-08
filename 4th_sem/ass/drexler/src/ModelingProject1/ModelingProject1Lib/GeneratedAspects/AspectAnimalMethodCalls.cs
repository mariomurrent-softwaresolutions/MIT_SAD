using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using PostSharp;
using PostSharp.Extensibility;
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;


[assembly: ClassDiagram.AspectAnimalMethodCalls(AttributeTargetTypes= "ClassDiagram.Animal")]

namespace ClassDiagram
{
	[Serializable]
	[AspectAnimalMethodCalls(AttributeExclude = true)]
	public class AspectAnimalMethodCalls : TypeLevelAspect
	{
		private int methodCounter = 0;

		[OnMethodEntryAdvice, MulticastPointcut(Targets = MulticastTargets.Method, MemberName="regex:^(?!.ctor|.cctor|Finalize).+")]
		public void OnEntry(MethodExecutionArgs args)
		{          
			if(!args.Method.IsConstructor)
			{
				++this.methodCounter;
			}

			this.LogToXML(@"C:\Temp", "AspectLog.xml");
		}

        /// <summary>
        /// Create a new xml file if not exist and log the values. If the file already exist
        /// update the values
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fileName"></param>
        private void LogToXML(string directory, string fileName)
        {
            try
            {
                // Log information to log file
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string filePath = Path.Combine(directory, fileName);

                if (!File.Exists(filePath))
                {
                    using (XmlWriter xWriter = XmlWriter.Create(filePath))
                    {
                        xWriter.WriteStartDocument();
                        xWriter.WriteStartElement("Objects");
                        xWriter.WriteStartElement("Animal");
                        xWriter.WriteElementString("MethodCallsCounter", this.methodCounter.ToString());
                        xWriter.WriteEndElement();
                        xWriter.WriteEndElement();
                        xWriter.WriteEndDocument();
                    }
                }
                else
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(@"C:\Temp\AspectLog.xml");
                    XmlNode root = doc.DocumentElement;
                    XmlNode myNode = root.SelectSingleNode("descendant::MethodCallsCounter");
                    if (myNode != null && myNode.HasChildNodes)
                    {
                        myNode.FirstChild.Value = this.methodCounter.ToString();
                    }
                    else
                    {
                        XmlNode animalNode = root.SelectSingleNode("descendant::Animal");
                        XmlElement element = doc.CreateElement("MethodCallsCounter");
                        element.InnerXml = this.methodCounter.ToString();
                        //element.AppendChild();
                        animalNode.AppendChild(element);
                    }

                    doc.Save(@"C:\Temp\AspectLog.xml");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
	}
}