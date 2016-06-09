#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Linq;
using System.Xml;
using DevExpress.ExpressApp.Model.Core;
namespace DevExpress.ExpressApp.Model {
	public class ModelXmlCombiner {
		private ModelApplicationBase model;
		private void CombineAttributes(XmlElement targetXmlNode, XmlElement sourceXmlNode) {
			foreach(XmlAttribute attribute in sourceXmlNode.Attributes) {
				if(targetXmlNode.Attributes[attribute.Name] != null) {
					targetXmlNode.Attributes[attribute.Name].Value = attribute.Value;
				}
				else {
					XmlNode importNode = targetXmlNode.OwnerDocument.ImportNode(attribute, true);
					targetXmlNode.Attributes.Append((XmlAttribute)importNode);
				}
			}
		}
		private string GetXmlString(XmlDocument document) { 
			using(StringWriter stringWriter = new StringWriter()) {
				using(XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter)) {
					xmlWriter.Formatting = Formatting.Indented;
					document.WriteTo(xmlWriter);
				}
				return stringWriter.ToString();
			}
		}
		private Boolean IsNameFitsToNode(ModelNode modelNode, string name) {
			Boolean result = false;
			if(modelNode.NodeInfo.GeneratedClass.Name == name || modelNode.NodeInfo.GeneratedClass.Name == "Model" + name) {
				result = true;
			}
			else {
				object[] attributes = modelNode.NodeInfo.BaseInterface.GetCustomAttributes(typeof(ModelPersistentNameAttribute), false);
				if(attributes.Length > 0) {
					ModelPersistentNameAttribute attribute = attributes.FirstOrDefault() as ModelPersistentNameAttribute;
					if(attribute.Name == name) {
						result = true;
					}
				}
			}
			return result;
		}
		private ModelNode GetNode(ModelNode modelNode, string name) {
			ModelNode result = null;
			foreach(ModelNode node in modelNode.GetNodes()) {
				if(IsNameFitsToNode(node, name)) {
					result = node;
				}
				else {
					result = GetNode(node, name);
				}
				if(result != null) {
					break;
				}
			}
			return result;
		}
		private String GetKeyValueName(String nodeName) {
			String result = null;
			ModelNode node = GetNode(model, nodeName);
			if(node != null) {
				result = node.KeyValueName;
			}
			return result;
		}
		private void CombineXmlSubElement(XmlElement targetxmlNode, XmlElement subXmlNode) {
			bool combineSucess = false;
			foreach(XmlNode subTargetXmlNode in targetxmlNode.ChildNodes) {
				if(subTargetXmlNode is XmlElement && subTargetXmlNode.Name == subXmlNode.Name) {
					String keyValueName = GetKeyValueName(subTargetXmlNode.Name);
					if(!String.IsNullOrWhiteSpace(keyValueName)) {
						XmlAttribute targetKeyValueAttribute = subTargetXmlNode.Attributes[keyValueName];
						XmlAttribute sourceKeyValueAttribute = subXmlNode.Attributes[keyValueName];
						if(
							(targetKeyValueAttribute == null) && (sourceKeyValueAttribute == null)
							||
							((targetKeyValueAttribute != null) && (sourceKeyValueAttribute != null) && (targetKeyValueAttribute.Value == sourceKeyValueAttribute.Value))
						) {
							CombineAttributes((XmlElement)subTargetXmlNode, subXmlNode);
							CombineNodes((XmlElement)subTargetXmlNode, subXmlNode);
							combineSucess = true;
							break;
						}
					}
				}
			}
			if(!combineSucess) {
				XmlNode importNode = targetxmlNode.OwnerDocument.ImportNode(subXmlNode, true);
				targetxmlNode.AppendChild(importNode);
			}
		}
		private void CombineNodes(XmlElement targetXmlNode, XmlElement sourceXmlNode) {
			foreach(XmlNode subXmlNode in sourceXmlNode.ChildNodes) {
				if(subXmlNode is XmlElement) {
					CombineXmlSubElement(targetXmlNode, (XmlElement)subXmlNode);
				}
			}
		}
		private string CombineXmlElements(XmlElement targetxmlNode, XmlElement sourceXmlNode) {
			CombineNodes(targetxmlNode, sourceXmlNode);
			return GetXmlString(targetxmlNode.OwnerDocument);
		}
		public ModelXmlCombiner() {
		}
		public String CombineXmlStrings(string targetXml, string sourceXml, IModelApplication modelApplication) {
			string result = string.Empty;
			model = modelApplication as ModelApplicationBase;
			if(model != null) {
				XmlDocument targetDocument = new XmlDocument();
				XmlDocument sourceDocument = new XmlDocument();
				try {
					targetDocument.LoadXml(targetXml);
					sourceDocument.LoadXml(sourceXml);
				}
				catch(XmlException e) {
					throw new ArgumentException("There are errors in the serialized data. See inner exception for details.", "xml", e);
				}
				result = CombineXmlElements(targetDocument.DocumentElement, sourceDocument.DocumentElement);
			}
			return result;
		}
	}
}
