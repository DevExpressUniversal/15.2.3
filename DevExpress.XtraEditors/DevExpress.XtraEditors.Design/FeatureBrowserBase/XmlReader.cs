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
using System.Xml;
namespace DevExpress.XtraEditors.FeatureBrowser { 
	public abstract class XmlFeaturesReaderBase {
		Type sourceType;
		public XmlFeaturesReaderBase(Type sourceType) {
			this.sourceType = sourceType;
		}
		protected Type SourceType { get { return sourceType;} }
		protected abstract string ItemTagName { get; }
		protected virtual string RootTagName { get { return ItemTagName + 's'; } }
		protected virtual void LoadFromXmlFiles(string[] xmlFiles) {
			foreach(string xmlFile in xmlFiles) {
				LoadFromXmlFile(xmlFile);
			}
		}
		protected virtual void LoadFromXmlFile(string xmlFile) {
			XmlDocument doc = GetXmlDocument(xmlFile);
			if(doc != null) {
				LoadFromXmlDocument(doc);
			}
		}
		protected  internal virtual void LoadFromXmlDocument(XmlDocument doc) {
			if(!IsSourceTypeSupported(doc)) return;
			XmlNodeList nodes = doc.GetElementsByTagName(ItemTagName);
			foreach(XmlNode node in nodes) {
				AddXmlNode(node);
			}
		}
		protected void AddXmlNode(XmlNode node) {
			if(IsXmlNodeSupported(node))
				AddXmlNodeCore(node);
		}
		protected virtual void AddXmlNodeCore(XmlNode node) {
		}
		protected virtual bool IsXmlNodeSupported(XmlNode node) {
			return true; 
		}
		protected XmlDocument GetXmlDocument(string xmlResourceFullName) {
			if(xmlResourceFullName == "") return null;
			XmlDocument doc = null;
			if(System.IO.File.Exists(xmlResourceFullName)) {
				doc = new XmlDocument();
				doc.Load(xmlResourceFullName);
			} 
			else {
				System.IO.Stream stream = null;
				try {
					stream = GetStream(xmlResourceFullName);
					if(stream != null) {
						doc = new XmlDocument();
						doc.Load(stream);
					}
				}
				finally {
					if(stream != null)
						stream.Close();
				}
			}
			return doc;
		}
		System.IO.Stream GetStream(string xmlResourceFullName) {
			System.IO.Stream stream = null;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach(Assembly assembly in assemblies) {
				if(assembly.FullName.IndexOf("DevExpress.") > -1) {
					if(assembly.IsDynamic)
						continue;
					stream = assembly.GetManifestResourceStream(xmlResourceFullName);
					if(stream != null)
						return stream;
				}
			}
			return stream;
		}
		protected bool IsSourceTypeSupported(XmlDocument doc) {
			string xmlSourceType = GetSourceType(doc, RootTagName);
			if(xmlSourceType == string.Empty) return false;
			return IsSourceTypeSupported(xmlSourceType);
		}
		bool IsSourceTypeSupported(string xmlSourceType) {
			if(SourceType == null) return false;
			if(xmlSourceType != "") {
				Type baseType = SourceType;
				while(baseType != null && baseType.Name != xmlSourceType)
					baseType = baseType.BaseType;
				if(baseType == null)
					return false;
			}
			return true;
		}
		string GetSourceType(XmlDocument doc, string rootTagName) {
			XmlNodeList nodes = doc.GetElementsByTagName(rootTagName);
			if(nodes.Count > 0) {
				XmlNode node = nodes[0];
				string typeName = GetNodeAttributeValue(node, "SourceType");
				if(typeName != "") 
					return typeName;
			}
			return string.Empty;
		}
		protected string GetNodeAttributeValue(XmlNode node, string name) {
			if(node.Attributes == null) return string.Empty;
			name = name.ToUpper();
			for(int i = 0; i < node.Attributes.Count; i ++) {
				if(node.Attributes[i].Name.ToUpper() == name)
					return node.Attributes[i].Value;
			}
			return string.Empty;
		}
		protected string[] GetArrayValues(XmlNode node, string nodeName, string attributeName) {
			ArrayList list = new ArrayList();
			GetArrayValues(node, nodeName, attributeName, list);
			return (string[])list.ToArray(typeof(string));
		}
		protected void GetArrayValues(XmlNode node, string nodeName, string attributeName, ArrayList list) {
			XmlNode childNode = null;
			for(int i = 0; i < node.ChildNodes.Count; i ++)
				if(node.ChildNodes[i].Name == nodeName) {
					childNode = node.ChildNodes[i];
					break;
				}
			if(childNode != null)
				GetArrayValues(childNode, attributeName, list);
		}
		protected string[] GetArrayValues(XmlNode node, string attributeName) {
			ArrayList list = new ArrayList();
			GetArrayValues(node, attributeName, list);
			return (string[]) list.ToArray(typeof(string));
		}
		protected void GetArrayValues(XmlNode node, string attributeName, ArrayList list) {
			for(int i = 0; i < node.ChildNodes.Count; i ++) {
				string value = GetNodeAttributeValue(node.ChildNodes[i], attributeName);
				if(value != string.Empty)
					list.Add(value);
			}
		}
		protected XmlNode FindChildNode(XmlNode node, string nodeName) {
			nodeName = nodeName.ToUpper();
			for(int i = 0; i < node.ChildNodes.Count; i ++)
				if(node.ChildNodes[i].Name.ToUpper() == nodeName) {
					return node.ChildNodes[i];
				}
			return null;
		}
		protected string GetNodeDescription(XmlNode node) {
			XmlNode descNode = FindChildNode(node, "Description");
			return descNode != null && descNode.InnerText != null ? descNode.InnerText.Trim() : string.Empty;
		}
		protected string GetNodeInnerText(XmlNode node) {
			return node.InnerText != null ? node.InnerText.Trim() : string.Empty;
		}
	}
}
