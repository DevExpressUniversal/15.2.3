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
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
namespace DevExpress.Entity.ProjectModel {
	public class EdmxResource {
		public struct SchemaAttributeValues {
			string provider;
			string providerManifestToken;
			public SchemaAttributeValues(string provider, string providerManifestToken){
				this.provider = provider;
				this.providerManifestToken = providerManifestToken;
			}
			static SchemaAttributeValues() {
				Empty = new SchemaAttributeValues();
			}
			public string Provider {
				get {
					return provider;
				}
				set {
					provider = value;
				}
			}
			public string ProviderManifestToken {
				get {
					return providerManifestToken;
				}
				set {
					providerManifestToken = value;
				}
			}
			public static readonly SchemaAttributeValues Empty;
			public bool IsEmpty { get { return string.IsNullOrEmpty(Provider) || string.IsNullOrEmpty(ProviderManifestToken); } }
		}
		public const string CsdlExtension = ".csdl";
		public const string SsdlExtension = ".ssdl";
		public const string MslExtension = ".msl";
		public const string EntityContainerTagName = "EntityContainer";
		public const string MslContainerTagName = "EntityContainerMapping";
		public const string TagNameAttribute = "Name";
		public const string Msl_CsdlContainerAttributeName = "CdmEntityContainer";
		public const string Msl_SsdlContainerAttributeName = "StorageEntityContainer";
		public EdmxResource(string csdlContainerName, string ssdlContainerName) {
			CsdlContainerName = csdlContainerName;
			SsdlContainerName = ssdlContainerName;
		}
		public static EdmxResource GetEdmxResource(IDXTypeInfo typeInfo) {
			DXAssemblyInfo assembly = typeInfo.Assembly as DXAssemblyInfo;
			if(assembly == null)
				return null;
			return assembly.GetEdmxResource(typeInfo);
		}
		public static void GetContainerNamesFromMsl(Stream mslStream, out string csdlContainerName, out string ssdlContainerName) {
			csdlContainerName = null; 
			ssdlContainerName = null;
			if(mslStream == null)
				return;
			mslStream.Seek(0, SeekOrigin.Begin);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(mslStream);
			csdlContainerName = GetAttributeValue(xmlDocument, MslContainerTagName, Msl_CsdlContainerAttributeName);
			ssdlContainerName = GetAttributeValue(xmlDocument, MslContainerTagName, Msl_SsdlContainerAttributeName);
		}
		static string GetAttributeValue(XmlDocument xmlDocument, string tagName, string atrributeName) {
			if(xmlDocument == null || String.IsNullOrEmpty(tagName) || String.IsNullOrEmpty(atrributeName))
				return null;
			XmlNodeList containerNodes = xmlDocument.GetElementsByTagName(tagName);
			if(containerNodes == null || containerNodes.Count == 0)
				return null;
			XmlNode containerNode = containerNodes[0];
			if(containerNode == null)
				return null;
			XmlAttributeCollection atrs = containerNode.Attributes;
			if(atrs == null || atrs.Count == 0)
				return null;
			XmlNode nameAttr = atrs.GetNamedItem(atrributeName);
			if(nameAttr == null)
				return null;
			return nameAttr.Value;
		}		
		public static string GetEntityContainerName(Stream stream) {
			if(stream == null)
				return null;
			stream.Seek(0, SeekOrigin.Begin);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(stream);
			return GetAttributeValue(xmlDocument, EntityContainerTagName, TagNameAttribute);
		}
		List<Stream> csdlStreams;
		List<Stream> mslStreams;
		List<Stream> ssdlStreams;
		public bool HasCsdlContainer { set; get; }
		public bool HasSsdlContainer { set; get; }
		public bool HasMslContainer { set; get; }
		public List<Stream> SsdlStreams { get { return ssdlStreams; } }
		public void AddCsdlContainerStream(Stream stream) {
			if(HasCsdlContainer)
				return;
			HasCsdlContainer = true;
			AddStream(ref csdlStreams, stream);
		}
		public void AddSsdlContainerStream(Stream stream) {
			if(HasSsdlContainer)
				return;
			HasSsdlContainer = true;
			AddStream(ref ssdlStreams, stream);
		}
		public void AddMslContainerStream(Stream stream) {
			if(HasMslContainer)
				return;
			HasMslContainer = true;
			AddStream(ref mslStreams, stream);
		}
		public void AddCsdlStream(Stream stream) {
			AddStream(ref csdlStreams, stream);
		}
		public void AddSsdlStream(Stream stream) {
			AddStream(ref ssdlStreams, stream);
		}		
		public void AddMslStream(Stream stream) {
			AddStream(ref mslStreams, stream);
		}
		void AddStream(ref List<Stream> streams, Stream stream) {
			if(stream == null)
				return;
			if(streams == null)
				streams = new List<Stream>();
			streams.Add(stream);
		}		
		string DefaultFileName {
			get {
				if (CsdlContainerName == null && SsdlContainerName == null)
					return new Guid().ToString();
				if (CsdlContainerName == null)
					return SsdlContainerName;
				if(SsdlContainerName == null)
					return CsdlContainerName;
				return String.Format("{0}_{1}", CsdlContainerName, SsdlContainerName);
			}
		}
		public void WriteResources(string path) {
			WriteResources(path, SchemaAttributeValues.Empty);
		}
		public void WriteResources(string path, SchemaAttributeValues values) {
			WriteResourcesCore(path, DefaultFileName, values);
		}
		void WriteResourcesCore(string path, string fileName, SchemaAttributeValues values) {
			if(String.IsNullOrEmpty(path) || String.IsNullOrEmpty(fileName))
				return;
			path = Path.Combine(path, fileName);
			WriteResources(csdlStreams, path, CsdlExtension);
			WriteResources(ssdlStreams, path, SsdlExtension, values);
			WriteResources(mslStreams, path, MslExtension);
		}
		void WriteResources(List<Stream> streams, string path, string extension) { 
			WriteResources(streams, path, extension, SchemaAttributeValues.Empty);
		}
		void WriteResources(List<Stream> streams, string path, string extension, SchemaAttributeValues values) {
			if(String.IsNullOrEmpty(path) || streams == null || extension == null)
				return;
			for(int i = 0; i < streams.Count; i++)
				WriteResourceFile(streams[i], String.Format("{0}{1}{2}", path, i.ToString(), extension), values);
		}		
		void WriteResourceFile(Stream stream, string path, SchemaAttributeValues values) {
			if(stream == null || !stream.CanRead || String.IsNullOrEmpty(path) || File.Exists(path))
				return;
			using(StreamWriter sw = new StreamWriter(path)) {
				stream.Seek(0, SeekOrigin.Begin);
					StreamReader reader = new StreamReader(stream);
					string text = reader.ReadToEnd();
				if (!values.IsEmpty)
					text = SetProvider(text, values);
					if(String.IsNullOrEmpty(text))
						return;
					sw.Write(text);
			}
		}
		public string CsdlContainerName { set; get; }
		public string SsdlContainerName { set; get; }
		public string SetProvider(string text, SchemaAttributeValues values) {
			XmlDocument document = new XmlDocument();
			document.LoadXml(text);
			foreach (XmlNode item in document.ChildNodes) {
				XmlElement element = item as XmlElement;
				if (element != null && element.GetAttribute("Provider") != null && element.GetAttribute("ProviderManifestToken") != null) {
					element.GetAttributeNode("Provider").Value = values.Provider;
					element.GetAttributeNode("ProviderManifestToken").Value = values.ProviderManifestToken;
				}
			}
			return Format(document);
		}
		public string GetProviderName() {
			if(ssdlStreams.Count > 0) {
				Stream s = ssdlStreams[0];
				s.Seek(0, SeekOrigin.Begin);
				XElement el = XElement.Load(s);
				return el.Attribute("Provider").Value;
			}
			return "";
		}
		protected virtual string Format(XmlDocument document) {
			return document.InnerXml;
		}
		XmlElement FindRecursive(XmlElement element, Predicate<XmlElement> predicate) {
			if (element == null)
				return null;
			if (predicate != null && predicate(element))
				return element;
			foreach (XmlElement child in element.ChildNodes) {
				XmlElement result = FindRecursive(child, predicate);
				if (result != null)
					return result;
			}
			return null;
		}
		IEnumerable<XmlElement> FindElementsRecursive(XmlElement element, Predicate<XmlElement> predicate) {
			if (element == null)
				yield break;
			if (predicate != null && predicate(element))
				yield return element;
			foreach (XmlElement child in element.ChildNodes)
				foreach (XmlElement item in FindElementsRecursive(child, predicate))
					yield return item;
		}
	}
}
