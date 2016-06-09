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
using System.Globalization;
using System.IO;
using System.Xml;
using System.Drawing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
using System.Windows.Controls;
using System.Xml.Linq;
#endif
namespace DevExpress.Utils.Serializing {
	#region IXmlContext
	public interface IXmlContext {
		string ElementName { get; }
		ICollection Attributes { get; }
		ICollection Elements { get; }
		bool XmlDocumentHeader { get; }
	}
	#endregion
	#region IXmlContextItem
	public interface IXmlContextItem {
		string Name { get; }
		object Value { get; }
		object DefaultValue { get; }
		string ValueToString();
	}
	#endregion
	#region Context Attributes
	public abstract class XmlContextItem : IXmlContextItem {
		string name = String.Empty;
		object val = null;
		object defaultValue;
		protected XmlContextItem(string name, object val, object defaultValue) {
			this.name = name == null ? string.Empty : name;
			this.val = val;
			this.defaultValue = defaultValue;
		}
		public string Name { get { return name; } }
		public object Value { get { return val; } }
		public object DefaultValue { get { return defaultValue; } }
		public abstract string ValueToString();
		public void SetValue(object val) {
			this.val = val;
		}
	}
	public class ObjectContextAttribute : XmlContextItem {
		public ObjectContextAttribute(string name, object val, object defaultValue)
			: base(name, val, defaultValue) {
		}
		public override string ValueToString() {
			return ObjectConverter.ObjectToString(Value);
		}
	}
	public class StringContextAttribute : XmlContextItem {
		public StringContextAttribute(string name, string val, string defaultValue)
			: base(name, val, defaultValue) {
		}
		public override string ValueToString() {
			string str = Convert.ToString(Value);
			return str;
		}
	}
	public class BooleanContextAttribute : XmlContextItem {
		public BooleanContextAttribute(string name, bool val, bool defaultValue)
			: base(name, val, defaultValue) {
		}
		public override string ValueToString() {
			string str = Convert.ToString(Value);
			return str;
		}
	}
	public class IntegerContextAttribute : XmlContextItem {
		public IntegerContextAttribute(string name, int val, int defaultValue)
			: base(name, val, defaultValue) {
		}
		public override string ValueToString() {
			int i = Convert.ToInt32(Value);
			return i.ToString(CultureInfo.InvariantCulture);
		}
	}
	public class GuidContextAttribute : XmlContextItem {
		public GuidContextAttribute(string name, Guid val, Guid defaultValue)
			: base(name, val, defaultValue) {
		}
		public override string ValueToString() {
			string str = Convert.ToString(Value);
			return str;
		}
	}
	public class DateTimeContextAttribute : XmlContextItem {
		public DateTimeContextAttribute(string name, DateTime val, DateTime defaultValue)
			: base(name, val, defaultValue) {
		}
		public override string ValueToString() {
			DateTime dateTime = Convert.ToDateTime(Value);
			return dateTime.ToString(CultureInfo.InvariantCulture.DateTimeFormat);
		}
	}
	public class TimeSpanContextAttribute : XmlContextItem {
		public TimeSpanContextAttribute(string name, TimeSpan val, TimeSpan defaultValue)
			: base(name, val, defaultValue) {
		}
		public override string ValueToString() {
			TimeSpan timeSpan = (TimeSpan)Value;
			return timeSpan.ToString();
		}
	}
	public class ColorContextAttribute : XmlContextItem {
		public ColorContextAttribute(string name, Color val, Color defaultValue)
			: base(name, val, defaultValue) {
		}
		public override string ValueToString() {
			int colorInt = DXColor.ToArgb((Color)Value);
			return Convert.ToString(colorInt);
		}
	}
	public class ImageContextAttribute : XmlContextItem {
		public ImageContextAttribute(string name, Image val, Image defaultValue)
			: base(name, val, defaultValue) {
		}
		public override string ValueToString() {
			return ObjectConverter.ObjectToString(Value as Image);
		}
	}
	#endregion
	#region XmlContextItemCollection
	public class XmlContextItemCollection : DXNamedItemCollection<IXmlContextItem> {
		protected override IXmlContextItem GetItem(int index) {
			if (index < 0 || index >= Count)
				return null;
			return base.GetItem(index);
		}
		protected internal override int AddIfNotAlreadyAdded(IXmlContextItem obj) {
			if (obj == null)
				return -1;
			return base.AddIfNotAlreadyAdded(obj);
		}
		protected internal override bool RemoveIfAlreadyAdded(IXmlContextItem obj) {
			if (obj == null)
				return false;
			return base.RemoveIfAlreadyAdded(obj);
		}
		protected override string GetItemName(IXmlContextItem item) {
			return item.Name;
		}
	}
	#endregion
	#region XmlContext
	public class XmlContext : IXmlContext {
		string elementName = String.Empty;
		XmlContextItemCollection attributes;
		XmlContextItemCollection elements;
		bool xmlDocumentHeader;
		public XmlContext(string elementName) {
			this.attributes = new XmlContextItemCollection();
			this.elements = new XmlContextItemCollection();
			ElementName = elementName;
		}
		public string ElementName {
			get { return elementName; }
			set {
				if (value == null) value = string.Empty;
				elementName = value;
			}
		}
		#region IXmlContext implementation
		ICollection IXmlContext.Attributes { get { return Attributes; } }
		ICollection IXmlContext.Elements { get { return Elements; } }
		#endregion
		public XmlContextItemCollection Attributes { get { return attributes; } }
		public XmlContextItemCollection Elements { get { return elements; } }
		public bool XmlDocumentHeader { get { return xmlDocumentHeader; } set { xmlDocumentHeader = value; } }
	}
	#endregion
	#region XmlContextWriter
	public class XmlContextWriter {
		public const string XmlWhiteSpace = "  ";
		IXmlContext context;
		public XmlContextWriter(IXmlContext context) {
			this.context = context;
		}
		public virtual void Save(XmlWriter writer) {
			if (context.XmlDocumentHeader)
				WriteStartDocument(writer);
			WriteStartElement(writer, context.ElementName);
			WriteAttributes(writer, context.Attributes);
			WriteElements(writer, context.Elements);
			WriteEndElement(writer);
			if (context.XmlDocumentHeader)
				WriteEndDocument(writer);
			writer.Flush();
		}
		protected virtual void WriteStartDocument(XmlWriter writer) {
			writer.WriteStartDocument();
		}
		protected virtual void WriteEndDocument(XmlWriter writer) {
			writer.WriteEndDocument();
		}
		protected virtual void WriteStartElement(XmlWriter writer, string name) {
			writer.WriteStartElement(name);
		}
		protected virtual void WriteEndElement(XmlWriter writer) {
			writer.WriteEndElement();
		}
		protected virtual void WriteAttributes(XmlWriter writer, ICollection items) {
			foreach (IXmlContextItem item in items)
				if (!Object.Equals(item.Value, item.DefaultValue))
					writer.WriteAttributeString(item.Name, item.ValueToString());
		}
		protected virtual void WriteElements(XmlWriter writer, ICollection items) {
			if (items.Count == 0) return;
			WriteNewLine(writer);
			foreach (IXmlContextItem item in items) {
				WriteElement(writer, item);
			}
		}
		protected virtual void WriteElement(XmlWriter writer, IXmlContextItem item) {
			string xml = item.ValueToString();
			if (!String.IsNullOrEmpty(xml)) {
				writer.WriteRaw(xml);
				WriteNewLine(writer);
			}
		}
		protected virtual void WriteIndent(XmlWriter writer, int count) {
		}
		protected virtual void WriteNewLine(XmlWriter writer) {
			writer.WriteString(Environment.NewLine);
		}
	}
	#endregion
	#region DXXmlNodeCollection
	public class DXXmlNodeCollection : DXCollection<System.Xml.XmlNode> {
		public DXXmlNodeCollection() {
			this.UniquenessProviderType = DXCollectionUniquenessProviderType.None;
		}
		protected override XmlNode GetItem(int index) {
			if (index < 0 || index >= this.InnerList.Count)
				return null;
			return base.GetItem(index);
		}
	}
	#endregion
	#region XmlDocumentHelper
	public static class XmlDocumentHelper {
		public static DXXmlNodeCollection GetChildren(XmlNode root) {
			return ConvertXmlNodeList(root.ChildNodes);
		}
		public static XmlNode GetDocumentElement(XmlDocument doc) {
			return doc.DocumentElement;
		}
		public static DXXmlNodeCollection ConvertXmlNodeList(XmlNodeList list) {
			DXXmlNodeCollection result = new DXXmlNodeCollection();
			foreach (XmlNode node in list)
				result.Add(node);
			return result;
		}
		public static XmlDocument LoadFromStream(Stream stream) {
#if !SL
			XmlDocument doc = new XmlDocument();
			doc.Load(stream);
			return doc;
#else
			return new XmlDocument(XDocument.Load(stream));
#endif
		}
		public static XmlDocument LoadFromXml(String xml) {
#if !SL
			XmlDocument doc = new XmlDocument(); ;
			doc.LoadXml(xml);
			return doc;
#else
			return new XmlDocument(XDocument.Parse(xml));
#endif
		}
	}
	#endregion
	#region ObjectXmlLoader
	public abstract class ObjectXmlLoader {
		XmlNode root;
		protected ObjectXmlLoader(XmlNode root) {
			this.root = root;
		}
		public abstract object ObjectFromXml();
		protected internal object ReadAttributeValueCore(string name) {
			if (root == null)
				return null;
			XmlAttribute attr = root.Attributes[name];
			return attr != null ? attr.Value : null;
		}
		protected internal DXXmlNodeCollection GetChildNodes(string name) {
			if (root == null)
				return new DXXmlNodeCollection();
			return GetElementsByTagName(root, name);
		}
		protected internal DXXmlNodeCollection GetElementsByTagName(XmlNode root, string tagName) {
			DXXmlNodeCollection result = new DXXmlNodeCollection();
			if (root.Name == tagName)
				result.Add(root);
			FindRecursive(root, tagName, result);
			return result;
		}
		protected internal virtual void FindRecursive(XmlNode root, string tagName, DXXmlNodeCollection searchResult) {
			DXXmlNodeCollection children = XmlDocumentHelper.GetChildren(root);
			int count = children.Count;
			for (int i = 0; i < count; i++) {
				XmlNode node = children[i];
				if (node.Name == tagName)
					searchResult.Add(node);
				FindRecursive(node, tagName, searchResult);
			}
		}
		#region ReadAttributeValue
		static Type intType = typeof(Int32);
		static Type decimalType = typeof(Decimal);
		static Type dateTimeType = typeof(DateTime);
		static Type stringType = typeof(String);
		static Type booleanType = typeof(Boolean);
		static Type objectType = typeof(Object);
		public object ReadAttributeValue(string name, Type t) {
			if (Object.ReferenceEquals(t, intType))
				return ReadAttributeAsInt(name, 0);
			else if (Object.ReferenceEquals(t, decimalType))
				return ReadAttributeAsDecimal(name, 0);
			else if (Object.ReferenceEquals(t, dateTimeType))
				return ReadAttributeAsDateTime(name, DateTime.MinValue);
			else if (Object.ReferenceEquals(t, stringType))
				return ReadAttributeAsString(name, String.Empty);
			else if (Object.ReferenceEquals(t, booleanType))
				return ReadAttributeAsBoolean(name, false);
			else if (Object.ReferenceEquals(t, objectType))
				return ReadAttributeAsObject(name, t, null);
			else
				return ReadAttributeAsObject(name, t, null);
		}
		#endregion
		public object ReadAttributeAsObject(string name, Type t, object defaultValue) {
			object val = ReadAttributeValueCore(name);
			if (val is string) {
				int asInt = int.MinValue;
				bool isParsed = int.TryParse((string)val, out asInt);
				return isParsed ? asInt : ObjectConverter.StringToObject((string)val, t);
			}
			return defaultValue;
		}
		public bool ReadAttributeAsBoolean(string name, bool defaultValue) {
			object val = ReadAttributeValueCore(name);
			return val != null ? Convert.ToBoolean(val) : defaultValue;
		}
		public string ReadAttributeAsString(string name, string defaultValue) {
			object val = ReadAttributeValueCore(name);
			return val != null ? Convert.ToString(val) : defaultValue;
		}
		public int ReadAttributeAsInt(string name, int defaultValue) {
			object val = ReadAttributeValueCore(name);
			return val != null ? Convert.ToInt32(val) : defaultValue;
		}
		public DateTime ReadAttributeAsDateTime(string name, DateTime defaultValue) {
			object val = ReadAttributeValueCore(name);
			return (val != null) ? DateTime.Parse(Convert.ToString(val), CultureInfo.InvariantCulture) : defaultValue;
		}
		public TimeSpan ReadAttributeAsTimeSpan(string name, TimeSpan defaultValue) {
			object val = ReadAttributeValueCore(name);
			return val is string ? TimeSpan.Parse((string)val) : defaultValue;
		}
		public Image ReadAttributeAsImage(string name, Image defaultValue) {
			object val = ReadAttributeValueCore(name);
			return val != null ? ObjectConverter.StringToObject(Convert.ToString(val), typeof(Image)) as Image : defaultValue;
		}
		public Color ReadAttributeAsColor(string name, Color defaultValue) {
			object val = ReadAttributeValueCore(name);
			return val is string ? (Color)ObjectConverter.StringToObject((string)val, typeof(Color)) : defaultValue;
		}
		public Guid ReadAttributeAsGuid(string name, Guid defaultValue) {
			object val = ReadAttributeValueCore(name);
			return val is string ? new Guid(Convert.ToString(val)) : defaultValue;
		}
		public Decimal ReadAttributeAsDecimal(string name, Decimal defaultValue) {
			object val = ReadAttributeValueCore(name);
			return val is string ? Decimal.Parse((string)val) : defaultValue;
		}
	}
	#endregion
	#region ObjectCollectionXmlLoader
	public abstract class ObjectCollectionXmlLoader : ObjectXmlLoader {
		protected ObjectCollectionXmlLoader(XmlNode root)
			: base(root) {
		}
		protected abstract ICollection Collection { get; }
		protected abstract string XmlCollectionName { get; }
		protected abstract object LoadObject(XmlNode root);
		protected abstract void AddObjectToCollection(object obj);
		protected abstract void ClearCollectionObjects();
		public override object ObjectFromXml() {
			ClearCollectionObjects();
			DXXmlNodeCollection nodes = GetChildNodes(XmlCollectionName);
			if (nodes.Count > 0) {
				foreach (XmlNode item in nodes[0].ChildNodes) {
					object obj = LoadObject(item);
					if (obj != null)
						AddObjectToCollection(obj);
				}
			}
			return Collection;
		}
	}
#endregion
	#region XmlPersistenceHelper
	public abstract class XmlPersistenceHelper {
		protected XmlPersistenceHelper() {
		}
		protected abstract IXmlContext GetXmlContext();
		public abstract ObjectXmlLoader CreateObjectXmlLoader(XmlNode root);
		protected virtual XmlContextWriter CreateXmlContextWriter(IXmlContext context) {
			return new XmlContextWriter(context);
		}
		public virtual void WriteXml(Stream stream) {
			IXmlContext context = GetXmlContext();
			if (IsValidContext(context))
				ContextToStream(context, stream);
		}
		public virtual string ToXml() {
			IXmlContext context = GetXmlContext();
			return IsValidContext(context) ? ContextToString(context) : string.Empty;
		}
		public static XmlNode GetRootElement(string xml) {
			if (String.IsNullOrEmpty(xml))
				return null;
#if SL
			XmlDocument doc = new XmlDocument(XDocument.Parse(xml));
#endif
#if !SL
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);
#endif
			return XmlDocumentHelper.GetDocumentElement(doc);
		}
		public virtual object FromXml(string xml) {
			return FromXmlNode(GetRootElement(xml));
		}
		public virtual object FromXmlNode(XmlNode root) {
			return ParseXmlDocument(root);
		}
		protected internal virtual bool IsValidContext(IXmlContext context) {
			if (context == null) return false;
			if (String.IsNullOrEmpty(context.ElementName))
				return false;
			return context.Attributes.Count > 0 || context.Elements.Count > 0;
		}
		protected internal virtual string ContextToString(IXmlContext context) {
			StringWriter sw = new StringWriter();
			if (sw != null) {
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.OmitXmlDeclaration = true;
				XmlWriter writer = XmlWriter.Create(sw, settings);
				SaveXml(context, writer);
			}
			return sw.ToString();
		}
		protected internal virtual void ContextToStream(IXmlContext context, Stream stream) {
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.OmitXmlDeclaration = true;
			XmlWriter writer = XmlWriter.Create(stream, settings);
			try {
				SaveXml(context, writer);
			} finally {
			}
		}
		protected internal virtual void SaveXml(IXmlContext context, XmlWriter writer) {
			CreateXmlContextWriter(context).Save(writer);
		}
		public virtual object ParseXmlDocument(XmlNode root) {
			if (root == null)
				return null;
			ObjectXmlLoader loader = CreateObjectXmlLoader(root);
			return (loader != null) ? loader.ObjectFromXml() : null;
		}
	}
	#endregion
	#region CollectionXmlPersistenceHelper
	public abstract class CollectionXmlPersistenceHelper : XmlPersistenceHelper {
		ICollection collection;
		protected CollectionXmlPersistenceHelper(ICollection collection) {
			this.collection = collection;
		}
		protected ICollection Collection { get { return collection; } }
		protected abstract string XmlCollectionName { get; }
		protected abstract IXmlContextItem CreateXmlContextItem(object obj);
		protected abstract ObjectCollectionXmlLoader CreateObjectCollectionXmlLoader(XmlNode root);
		protected override IXmlContext GetXmlContext() {
			IXmlContext context = CreateXmlContext();
			InitXmlContext(context);
			return context;
		}
		protected virtual IXmlContext CreateXmlContext() {
			return new XmlContext(XmlCollectionName);
		}
		protected virtual void InitXmlContext(IXmlContext context) {
			foreach (object item in Collection) {
				AddItemToContext(item, context);
			}
		}
		protected virtual void AddItemToContext(object item, IXmlContext context) {
			((XmlContext)context).Elements.Add(CreateXmlContextItem(item));
		}
		public override ObjectXmlLoader CreateObjectXmlLoader(XmlNode root) {
			return CreateObjectCollectionXmlLoader(root);
		}
	}
	#endregion
}
