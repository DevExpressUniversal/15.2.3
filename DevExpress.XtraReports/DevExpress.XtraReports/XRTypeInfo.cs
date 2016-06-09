#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.IO;
using DevExpress.XtraReports.Native;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.Generic;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Serialization 
{
	[Browsable(false)]
	public class XRTypeInfo {
		#region inner classes
		abstract class Formatter {
			public abstract void Serialize(XRTypeInfo info, Stream stream);
			public abstract XRTypeInfo Deserialize(string streamString);
		}
		class XmlFormatter : Formatter {
			public override void Serialize(XRTypeInfo info, Stream stream) {
				throw new NotImplementedException();
			}
			public override XRTypeInfo Deserialize(string streamString) {
				XRTypeInfo info = new XRTypeInfo();
				string asmQualifiedName = GetReportType(streamString);
				string[] items = asmQualifiedName.Split(new char[] { ',' }, 2, StringSplitOptions.None);
				info.TypeName = GetItem(items, 0);
				info.AssemblyFullName = GetItem(items, 1);
				return info;
			}
			static string GetItem(string[] items, int index) { 
				return index < items.Length ? items[index].Trim() : string.Empty;
			}
			static string GetReportType(string streamString) {
				TextReader input = new StringReader(streamString);
				XmlTextReader reader = new XmlTextReader(input);
				while(!reader.EOF) {
					reader.Read();
					if(reader.Name == XtraReportsXmlSerializer.Name)
						return reader.GetAttribute("ControlType");
				}
				return string.Empty;
			}
		}
		class CodeFormatter : Formatter {
			const string commentPrefix = "/// ";
			public override void Serialize(XRTypeInfo info, Stream stream) {
				StringWriter stringWriter = new StringWriter();
				XRTypeInfoSerializer.Serialize(info, stringWriter);
				stringWriter.Close();
				StreamWriter streamWriter = new StreamWriter(stream);
				string[] items = System.Text.RegularExpressions.Regex.Split(stringWriter.ToString(), stringWriter.NewLine);
				for(int i = 0; i< items.Length; i++)
					streamWriter.WriteLine(commentPrefix + items[i]);
				streamWriter.Flush();
			}
			public override XRTypeInfo Deserialize(string streamString) {
				StringBuilder stringBuilder = new StringBuilder();
				foreach(string rawLine in streamString.AllSubstrings()) {
					string line = rawLine.Trim();
					if(line.Length > 0) {
						if(line.StartsWith(commentPrefix)) {
							stringBuilder.Append(line.Substring(commentPrefix.Length));
						} else
							break;
					}
				}
				return XRTypeInfoSerializer.Deserialize(new StringReader(stringBuilder.ToString()));
			}
		}
		class SoapFormatter : Formatter {
			#region static
			static Hashtable ReadProperties(string streamString) {
				Hashtable ht = new Hashtable();
				string[] lines = streamString.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				bool inComment = false;
				foreach(string line in lines) {
					if(line.StartsWith(beginCommentTag))
						inComment = true;
					if(line == endCommentTag)
						inComment = false;
					if(inComment) {
						string[] parts = line.Split('=');
						if(parts.Length == 2)
							ht.Add(parts[0], parts[1]);
					}
				}
				return ht;
			}
			static void WriteProperty(StreamWriter sw, string propName, string propVal) {
				sw.WriteLine(propName + "=" + propVal);
			}
			#endregion
			const string beginCommentTag = "<!--",
				endCommentTag = "-->",
				propAsmName = "AssemblyName",
				propAsmLocation = "AssemblyLocation",
				propTypeName = "TypeName",
				propLocalization = "Localization";
			public override void Serialize(XRTypeInfo info, Stream stream) {
				StreamWriter sw = new StreamWriter(stream); 
				sw.WriteLine(beginCommentTag);
				WriteProperty(sw, propAsmName, NativeMethods.HexEscape(info.AssemblyFullName));
				WriteProperty(sw, propAsmLocation, info.AssemblyLocation);
				WriteProperty(sw, propTypeName, info.TypeName);
				WriteProperty(sw, propLocalization, info.Localization);
				sw.WriteLine(endCommentTag);
				sw.Flush();
			}
			public override XRTypeInfo Deserialize(string streamString) {
				XRTypeInfo info = new XRTypeInfo();
				Hashtable ht = ReadProperties(streamString);
				if(ht.Count > 0) {
					info.AssemblyFullName =  NativeMethods.HexUnescape( (string)ht[propAsmName] );
					info.AssemblyLocation = (string)ht[propAsmLocation];
					info.TypeName = (string)ht[propTypeName];
					info.Localization = (string)ht[propLocalization];
				}
				return info;
			}
		}
		#endregion
		internal static void Serialize(Type type, string version, string[] refPaths, Dictionary<string, string> resources, Stream stream) {
			new CodeFormatter().Serialize(new XRTypeInfo(type, version, refPaths, resources), stream);
		}
		internal static XRTypeInfo Deserialize(string streamString) {
			return CreateFormatter(streamString).Deserialize(streamString);
		}
		static Formatter CreateFormatter(string streamString) {
			if(FileFormatDetector.CreateSoapDetector().FormatExists(streamString))
				return new SoapFormatter();
			if(FileFormatDetector.CreateXmlDetector().FormatExists(streamString))
				return new XmlFormatter();
			return new CodeFormatter();
		}
		const string noneString = "None";
		public string AssemblyFullName = string.Empty;
		public string AssemblyLocation = string.Empty;
		public string TypeName = noneString;
		public string Localization = string.Empty;
		public string Version = string.Empty;
		public Reference[] References = new Reference[0];
		public Resource[] Resources = new Resource[0];
		internal bool IsValid {
			get { return !TypeName.Equals(noneString); }
		}
		public XRTypeInfo() {
		}
		public XRTypeInfo(Type type) {
			TypeName = type.FullName;
			AssemblyFullName = type.Assembly.FullName;
			AssemblyLocation = type.Assembly.Location;
			Localization = DevExpress.XtraReports.Localization.ReportLocalizer.Active.Language;
		}
		public XRTypeInfo(Type type, string version, string[] refPaths, Dictionary<string, string> resourses) : this(type) {
			Version = version;
			SetReferences(refPaths);
			SetResourses(resourses);
		}
		void SetResourses(Dictionary<string, string> resourses) {
			List<Resource> resList = new List<Resource>(resourses.Count);
			foreach(KeyValuePair<string, string> keyValuePair in resourses) { 
				resList.Add(new Resource() { Name = keyValuePair.Key, Content = keyValuePair.Value });
			}
			Resources = resList.ToArray();
		}
		internal string[] GetReferences() {
			string[] references = new string[References.Length];
			for(int i = 0; i < References.Length; i++)
				references[i] = References[i].Path;
			return references;
		}
		void SetReferences(string[] refPaths) {
			References = new Reference[refPaths.Length];
			for(int i = 0; i < refPaths.Length; i++)
				References[i] = new Reference(refPaths[i]);
		}
		internal bool TypeEquals(Type type, bool ignoreAssemblyVersionInfo) {
			return !TypeName.Equals(type.FullName) ? false :
				ignoreAssemblyVersionInfo ? String.Compare(XRSerializer.GetShortAssemblyName(AssemblyFullName), XRSerializer.GetShortAssemblyName(type.Assembly.FullName), true) == 0 :
				AssemblyFullName.Equals(type.Assembly.FullName);
		}
	}	
	public class Reference {
		[XmlAttribute("Path")]
		public string Path = "";
		public Reference() {
		}
		public Reference(string path) {
			this.Path = path;
		}
	}
	public class Resource { 
		[XmlAttribute("Name")]
		public string Name = "";
		public string Content = "";
		public Resource() {
		}
		public Resource(string name, string content) {
			this.Name = name;
			this.Content = content;
		}
	}
	public sealed class XRTypeInfoSerializer {
		XRTypeInfoSerializer() { 
		} 
		public static void Serialize(XRTypeInfo typeInfo, TextWriter textWriter) {
			XmlTextWriter writer = new XmlTextWriter(textWriter);
			writer.Formatting = Formatting.Indented;
			writer.Indentation = 2;
			writer.WriteStartElement("XRTypeInfo");
			WriteElement(writer, "AssemblyFullName", typeInfo.AssemblyFullName);
			WriteElement(writer, "AssemblyLocation", typeInfo.AssemblyLocation);
			WriteElement(writer, "TypeName", typeInfo.TypeName);
			WriteElement(writer, "Localization", typeInfo.Localization);
			WriteElement(writer, "Version", typeInfo.Version);
			if(typeInfo.References.Length > 0) {
				writer.WriteStartElement("References");
				foreach(Reference reference in typeInfo.References)
					WriteReference(writer, reference);
				writer.WriteEndElement();
			}
			if(typeInfo.Resources.Length > 0) {
				writer.WriteStartElement("Resources");
				foreach(Resource resource in typeInfo.Resources)
					WriteResource(writer, resource);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.Flush();
		}
		static void WriteResource(XmlTextWriter writer, Resource resource) {
			writer.WriteStartElement("Resource");
			writer.WriteStartAttribute(null, "Name", null);
			writer.WriteString(resource.Name);
			writer.WriteEndAttribute();
			for(int i = 0; i < resource.Content.Length; i += 500) {
				writer.WriteString(Environment.NewLine);
				string part = resource.Content.Substring(i, resource.Content.Length > i + 500 ? 500 : resource.Content.Length - i);
				writer.WriteString(part);
			}
			writer.WriteEndElement();
		}
		static void WriteReference(XmlTextWriter writer, Reference reference) {
			writer.WriteStartElement("Reference");
			writer.WriteStartAttribute(null, "Path", null);
			writer.WriteString(reference.Path);
			writer.WriteEndAttribute();
			writer.WriteEndElement();
		}
		static void WriteElement(XmlTextWriter writer, string name, string value) {
			writer.WriteStartElement(name);
			writer.WriteString(value);
			writer.WriteEndElement();
		}
		public static XRTypeInfo Deserialize(TextReader textReader) {
			XRTypeInfo typeInfo = new XRTypeInfo();
			XmlTextReader reader = new XmlTextReader(textReader);
			List<Reference> references = new List<Reference>();
			List<Resource> resources = new List<Resource>();
			try {
				while(reader.Read()) {
					if(reader.NodeType == XmlNodeType.Element) {
						string name = reader.Name;
						if(name == "Reference") {
							string path = reader.GetAttribute("Path");
							if(path != null && path.Length > 0)
								references.Add(new Reference(path));
						} else if(name == "Resource") {
							string resName = reader.GetAttribute("Name");
							string resContent = reader.ReadString();
							if(resName != null && resName.Length > 0)
								resources.Add(new Resource(resName, resContent));
						} else if(name == "AssemblyFullName") {
							reader.Read();
							typeInfo.AssemblyFullName = reader.Value;
						} else if(name == "AssemblyLocation") {
							reader.Read();
							typeInfo.AssemblyLocation = reader.Value;
						} else if(name == "TypeName") {
							reader.Read();
							typeInfo.TypeName = reader.Value;
						} else if(name == "Localization") {
							reader.Read();
							typeInfo.Localization = reader.Value;
						} else if(name == "Version") {
							reader.Read();
							typeInfo.Version = reader.Value;
						}
					}
				}
			} catch(XmlException ex) {
				DevExpress.XtraPrinting.Tracer.TraceError(NativeSR.TraceSource, ex);
				reader.Close();
			}
			typeInfo.References = references.ToArray();
			typeInfo.Resources = resources.ToArray();
			return typeInfo;
		}
	}
}
