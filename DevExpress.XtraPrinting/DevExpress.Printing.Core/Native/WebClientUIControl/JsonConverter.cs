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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;
namespace DevExpress.XtraPrinting.Native.WebClientUIControl {
	public static class JsonConverter {
		class NodeElement {
			public string Name { get; private set; }
			public List<object> Nodes { get; private set; }
			public NodeElement(string name) {
				Name = name;
				Nodes = new List<object>();
			}
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public static string XmlToJson(Stream stream, bool includeRootTag = true) {
			if(!includeRootTag) {
				return InnerXmlNodesToJson(stream);
			}
			var document = XDocument.Load(stream);
			var builder = new StringBuilder();
			builder.Append('{');
			XmlToJsonNode(builder, document.Root, true);
			builder.Append('}');
			return builder.ToString();
		}
		public static string XmlToJson(XElement element) {
			var builder = new StringBuilder();
			XmlToJsonNode(builder, element, false);
			return builder.ToString();
		}
		static string InnerXmlNodesToJson(Stream stream) {
			var document = XDocument.Load(stream);
			var builder = new StringBuilder();
			builder.Append('{');
			var isFirst = true;
			foreach(XElement element in document.Root.Elements()) {
				if(!isFirst)
					builder.Append(',');
				isFirst = false;
				XmlToJsonNode(builder, element, true);
			}
			builder.Append('}');
			return builder.ToString();
		}
		public static void XmlToJsonNode(StringBuilder builder, XElement node, bool showNodeName) {
			if(showNodeName) {
				JsonGenerator.AppendQuotedValue(builder, node.Name.ToString(), isKey: true);
			}
			builder.Append('{');
			var childNodeNames = new List<NodeElement>();
			foreach(XAttribute attr in node.Attributes()) {
				StoreChildNode(childNodeNames, "@" + attr.Name, attr.Value);
			}
			foreach(XElement cnode in node.Elements()) {
				if(cnode.NodeType == XmlNodeType.Text) {
					StoreChildNode(childNodeNames, "value", cnode.Value);
				} else {
					StoreChildNode(childNodeNames, cnode.Name.ToString(), cnode);
				}
			}
			foreach(var pair in childNodeNames) {
				string childName = pair.Name;
				List<object> children = pair.Nodes;
				if(children.Count == 1) {
					OutputNode(childName, children[0], builder, true);
				} else {
					JsonGenerator.AppendQuotedValue(builder, childName, isKey: true);
					builder.Append('[');
					foreach(object child in children)
						OutputNode(childName, child, builder, false);
					builder.Remove(builder.Length - 1, 1);
					builder.Append("],");
				}
			}
			builder.Remove(builder.Length - 1, 1);
			builder.Append('}');
		}
		static void StoreChildNode(List<NodeElement> childNodeNames, string nodeName, object nodeValue) {
			var nodeXElement = nodeValue as XElement;
			if(nodeXElement != null) {
				if(!nodeXElement.HasAttributes) {
					XElement[] children = nodeXElement.Elements().ToArray();
					if(children.Length == 0) {
						nodeValue = null;
					} else if(children.Length == 1 && children[0].NodeType == XmlNodeType.Text) {
						nodeValue = children[0].Value;
					}
				}
			}
			NodeElement nodeElement = childNodeNames.Find(x => x.Name == nodeName);
			if(nodeElement == null) {
				nodeElement = new NodeElement(nodeName);
				childNodeNames.Add(nodeElement);
			}
			nodeElement.Nodes.Add(nodeValue);
		}
		static void OutputNode(string childName, object children, StringBuilder builder, bool showNodeName) {
			if(children == null) {
				if(showNodeName) {
					JsonGenerator.AppendQuotedValue(builder, childName, isKey: true);
				}
				builder.Append("null");
			} else {
				var childrenString = children as string;
				if(childrenString != null) {
					if(showNodeName) {
						JsonGenerator.AppendQuotedValue(builder, childName, isKey: true);
					}
					childrenString = childrenString.Trim();
					JsonGenerator.AppendQuotedValue(builder, childrenString);
				} else
					XmlToJsonNode(builder, (XElement)children, showNodeName);
			}
			builder.Append(',');
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public static byte[] JsonToXml(string json, bool firstChildAsRoot = true) {
			var xmlSettings = new XmlWriterSettings {
				OmitXmlDeclaration = false,
				Indent = true,
				Encoding = JsonGenerator.Encoding
			};
			using(var stream = new MemoryStream())
			using(var xmlWriter = XmlWriter.Create(stream, xmlSettings)) {
				XDocument document = JsonGenerator.JsonToCleanXmlDeclaration(json);
				if(firstChildAsRoot) {
					var firstChildNode = document.Root.FirstNode;
					document.Root.Remove();
					document.Add(firstChildNode);
				}
				ModifyRecursively(document.Elements());
				document.Save(xmlWriter);
				xmlWriter.Flush();
				return stream.ToArray();
			}
		}
		static void ModifyRecursively(IEnumerable<XElement> elements) {
			var elementsToRemove = new List<XElement>();
			foreach(XElement element in elements) {
				if(TryProcessJsonAttribute(element)) {
					elementsToRemove.Add(element);
					continue;
				}
				element.RemoveAttributes();
				ModifyRecursively(element.Elements());
			}
			elementsToRemove.ForEach(x => x.Remove());
		}
		static bool TryProcessJsonAttribute(XElement element) {
			string attrName;
			if(!JsonGenerator.TryProcessJsonAttribute(element, out attrName)) {
				return false;
			}
			element.Parent.SetAttributeValue(attrName, element.Value);
			return true;
		}
	}
}
