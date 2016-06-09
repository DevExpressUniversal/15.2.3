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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Model {
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class ModelXmlWriter {
		private string SaveXmlDocument(XmlDocument document) {
			using(StringWriter stringWriter = new StringWriter()) {
				using(XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter)) {
					xmlWriter.Formatting = Formatting.Indented;
					document.WriteTo(xmlWriter);
				}
				return stringWriter.ToString();
			}
		}
		private XmlElement SerializeNode(ModelNode modelNode, int aspectIndex, XmlDocument document) {
			XmlElement xmlNode = document.CreateElement(modelNode.GetXmlName());
			SerializeAttributes(xmlNode, modelNode, aspectIndex, document);
			SerializeNodes(xmlNode, modelNode, aspectIndex, document);
			return xmlNode;
		}
		private void SerializeAttributes(XmlElement xmlNode, ModelNode modelNode, int aspectIndex, XmlDocument document) {
			IDictionary<string, string> values = modelNode.GetSerializedValues(aspectIndex);
			foreach(KeyValuePair<string, string> item in values) {
				XmlAttribute attribute = document.CreateAttribute(item.Key);
				attribute.Value = item.Value;
				xmlNode.Attributes.Append(attribute);
			}
		}
		private void SerializeNodes(XmlElement xmlNode, ModelNode modelNode, int aspectIndex, XmlDocument document) {
			ModelNode master = modelNode.Master;
			if(master != null) {
				master.EnsureNodes();
			}
			IList<ModelNode> nodes = modelNode.GetChildrenForSerialization();
			foreach(ModelNode subModelNode in nodes) {
				XmlElement subNode = SerializeNode(subModelNode, aspectIndex, document);
				if(IsNotEmptyNode(subNode, subModelNode.KeyValueName, subModelNode.IsSingleLayer && aspectIndex == 0 && !subModelNode.IsUnusableNode)) {
					xmlNode.AppendChild(subNode);
				}
			}
		}
		private bool IsNotEmptyNode(XmlElement xmlNode, string keyPropertyName, bool isOneLayer) {
			if(xmlNode.ChildNodes.Count > 0) {
				return true;
			}
			XmlAttributeCollection attributes = xmlNode.Attributes;
			if(attributes[keyPropertyName] != null) {
				return attributes.Count > 1 || isOneLayer;
			}
			else {
				return attributes[ModelValueNames.IsNewNode] != null ? attributes.Count > 1 : attributes.Count > 0;
			}
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string WriteToString(IModelNode modelNode, int aspectIndex) {
			Guard.ArgumentNotNull(modelNode, "modelNode");
			ModelNode node = (ModelNode)modelNode;
			XmlDocument document = new XmlDocument();
			XmlElement root = SerializeNode(node, aspectIndex, document);
			if(root.Attributes.Count > 0 || root.ChildNodes.Count > 0) {
				document.AppendChild(root);
				string xml = SaveXmlDocument(document);
				return xml;
			}
			return string.Empty;
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool WriteToFile(IModelNode modelNode, int aspectIndex, string fileName, Encoding encoding) {
			Guard.ArgumentNotNull(modelNode, "modelNode");
			Guard.ArgumentNotNullOrEmpty(fileName, "fileName");
			Guard.ArgumentNotNull(encoding, "encoding");
			string xml = WriteToString(modelNode, aspectIndex);
			try {
				xml = string.IsNullOrEmpty(xml) ? "<Application />" : xml;
				xml = string.Format(@"<?xml version=""1.0"" encoding=""{0}""?>{1}{2}", encoding.HeaderName, Environment.NewLine, xml);
				File.WriteAllText(fileName, xml, encoding);
			}
			catch(IOException) {
				return false;
			}
			return true;
		}
	}
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class ModelXmlReader {
		private const int MaxExpectedEncodingStringLengthInBytes = 512;
		private static readonly Encoding[] ExpectedEncodings = new Encoding[] { Encoding.UTF8, Encoding.ASCII, Encoding.Unicode, Encoding.UTF7, Encoding.UTF32, Encoding.BigEndianUnicode };
		private static readonly Encoding DefaultEncoding = Encoding.UTF8;
		private IDictionary<string, string> GetAttributes(XmlElement node) {
			IDictionary<string, string> result = new Dictionary<string, string>();
			foreach(XmlAttribute attribute in node.Attributes) {
				result[attribute.Name] = attribute.Value;
			}
			return result;
		}
		private void GetNodes(XmlElement xmlNode, ModelNode modelNode) {
			foreach(XmlNode subXmlNode in xmlNode.ChildNodes) {
				if(subXmlNode is XmlElement) {
					ModelNode subModelNode = modelNode.AddNodeFromXml(subXmlNode.Name, GetAttributes((XmlElement)subXmlNode));
					GetNodes((XmlElement)subXmlNode, subModelNode);
				}
			}
		}
		private void SetSchemaVersionInfos(XmlDocument document, IModelApplicationServices application) {
			if(application != null) {
				XmlNode schemaModulesNode = document.DocumentElement.SelectSingleNode("SchemaModules");
				if(schemaModulesNode != null) {
					Dictionary<string, Version> result = new Dictionary<string, Version>();
					foreach(XmlNode schemaModule in schemaModulesNode.ChildNodes) {
						result.Add(schemaModule.Attributes["Name"].InnerText, new Version(schemaModule.Attributes["Version"].InnerText));
					}
					application.SetSchemaModuleInfos(result);
				}
			}
		}
		private void Read(ModelNode node, string aspect, XmlDocument document) {
			ModelNode master = node.Master;
			int layerIndex = master != null ? master.RemoveLayerInternal(node) : -1;
			ModelApplicationBase application = node.Root as ModelApplicationBase;
			string currentAspect = null;
			if(application != null) {
				currentAspect = application.CurrentAspect;
				application.AddAspect(aspect);
				application.SetCurrentAspect(aspect);
				application.StartLoading();
				SetSchemaVersionInfos(document, application);
			}
			try {
				foreach(KeyValuePair<string, string> pair in GetAttributes(document.DocumentElement)) {
					node.SetSerializedValue(pair.Key, pair.Value);
				}
				GetNodes(document.DocumentElement, node);
			}
			finally {
				if(application != null) {
					application.StopLoading();
					application.SetCurrentAspect(currentAspect);
				}
				if(layerIndex >= 0) {
					master.InsertLayerAtInternal(node, layerIndex);
				}
			}
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ReadFromString(IModelNode modelNode, string aspect, string xml) {
			Guard.ArgumentNotNull(modelNode, "modelNode");
			Guard.ArgumentNotNull(aspect, "aspect");
			Guard.ArgumentNotNull(xml, "xml");
			ModelNode node = (ModelNode)modelNode;
			if(node.IsMaster)
				throw new ArgumentException(string.Format("The '{0}' node is master. This operation cannot be performed over a master node.", node.Path), "modelNode");
			XmlDocument document = new XmlDocument();
			try {
				document.LoadXml(xml);
			}
			catch(XmlException e) {
				throw new ArgumentException("There are errors in the serialized data. See inner exception for details.", "xml", e);
			}
			Read(node, aspect, document);
		}
		private Encoding GetEncodingFromHeader(string encodingString) {
			if(string.IsNullOrEmpty(encodingString)) return null;
			int start = encodingString.IndexOf(@"encoding=""");
			if(start >= 0) {
				start += 10;
				int end = encodingString.IndexOf(@"""", start);
				if(end > 0) {
					string encodingStr = encodingString.Substring(start, end - start);
					try {
						return Encoding.GetEncoding(encodingStr);
					}
					catch {
						return null;
					}
				}
			}
			return null;
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ReadFromStream(IModelNode modelNode, string aspect, Stream stream) {
			Guard.ArgumentNotNull(modelNode, "modelNode");
			Guard.ArgumentNotNull(aspect, "aspect");
			Guard.ArgumentNotNull(stream, "stream");
			if(stream.Length == 0) return;
			Encoding encoding = GetStreamEncoding(stream) ?? DefaultEncoding;
			string xml;
			using(StreamReader reader = new StreamReader(stream, encoding)) {
				xml = reader.ReadToEnd();
			}
			ReadFromString(modelNode, aspect, xml);
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Encoding GetStreamEncoding(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			if(stream.Length == 0) return null;
			byte[] bytes = new byte[Math.Min(stream.Length, MaxExpectedEncodingStringLengthInBytes)];
			long position = stream.Position;
			try {
				stream.Position = 0;
				stream.Read(bytes, 0, bytes.Length);
			}
			finally {
				stream.Position = position;
			}
			foreach(Encoding encoding in ExpectedEncodings) {
				string content = encoding.GetString(bytes);
				Encoding result = GetEncodingFromHeader(content);
				if(result == null) continue;
				if(result == encoding) return result;
				content = result.GetString(bytes);
				if(GetEncodingFromHeader(content) == null) {
					throw new ArgumentException(string.Format("Encoding '{0}' manually specified in stream does not match actual stream encoding '{1}'.", result.HeaderName, encoding.HeaderName), "stream");
				}
				return result;
			}
			return null;
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ReadFromFile(IModelNode modelNode, string aspect, string fileName) {
			Guard.ArgumentNotNull(modelNode, "modelNode");
			Guard.ArgumentNotNull(aspect, "aspect");
			Guard.ArgumentNotNull(fileName, "fileName");
			try {
				using(FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
					ReadFromStream(modelNode, aspect, stream);
				}
			}
			catch(Exception e) {
				throw new ModelDeserializationException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.AnErrorHasOccurredWhileLoadingTheModel, fileName), e);
			}
		}
		string GetResourceFullName(Assembly assembly, string resourceName) {
			return Array.Find(assembly.GetManifestResourceNames(), delegate(string manifestResourceName) {
				return manifestResourceName.Contains(resourceName);
			});
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ReadFromResource(IModelNode modelNode, string aspect, Assembly assembly, string resourceName) {
			Guard.ArgumentNotNull(modelNode, "modelNode");
			Guard.ArgumentNotNull(aspect, "aspect");
			Guard.ArgumentNotNull(assembly, "assembly");
			Guard.ArgumentNotNull(resourceName, "resourceName");
			string resourceFullName = GetResourceFullName(assembly, resourceName);
			if(string.IsNullOrEmpty(resourceFullName)) return;
			try {
				using(Stream stream = assembly.GetManifestResourceStream(resourceFullName)) {
					if(stream == null)
						throw new ArgumentException(string.Format("The resource '{0}' was not found. Most likely it is not embedded into the assembly {1}.", resourceName, assembly.GetName().Name), "resourceName");
					ReadFromStream(modelNode, aspect, stream);
				}
			}
			catch(Exception e) {
				throw new ModelDeserializationException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CannotReadDictionaryFromResource, assembly, resourceName), e);
			}
		}
	}
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class ModelXmlHelper {
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string SimplifyString(string value) {
			StringBuilder s = new StringBuilder(value);
			s.Replace("\r\n", "").Replace("\t", "").Replace(" />", "/>").Replace("  ", "").Replace(" >", ">");
			return s.ToString();
		}
	}
	[Serializable]
	public sealed class ModelDeserializationException : ModelException {
		private ModelDeserializationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
		internal ModelDeserializationException(string message, Exception innerException) : base(message, innerException) { }
	}
}
