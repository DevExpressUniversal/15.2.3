#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.Utils;
using System.Globalization;
using System.ComponentModel;
using DevExpress.DashboardCommon.Layout;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon.Native {
	public static class DataItemXmlHelper {
		const string xmlUniqueName = "UniqueName";
		const string xmlName = "Name";
		public static string GetUniqueNameAttribute(this XElement element) {
			string name = element.GetAttributeValue(xmlUniqueName);
			if(String.IsNullOrEmpty(name)) {
				name = element.GetAttributeValue(xmlName);
				if(String.IsNullOrEmpty(name))
					throw new XmlException();
			}
			return name;
		}
		public static void SetUniqueNameAttribute(this XElement element, string uniqueName) {
			element.Add(new XAttribute(xmlUniqueName, uniqueName));
		}
		public static XElement SaveDataItemToXml(DataItem dataItem, DataItemXmlSerializationContext context) {
			XmlSerializer<DataItem> dataItemSerializer = XmlRepository.DataItemRepository.GetSerializer(dataItem);
			if (dataItemSerializer != null)
				return dataItemSerializer.SaveToXml(dataItem, context);
			return null;
		}
		public static DataItem LoadDataItemFromXml(XElement element, DataItemXmlSerializationContext context) {
			XmlSerializer<DataItem> dataItemSerializer = XmlRepository.DataItemRepository.GetSerializer(element.Name.LocalName);
			if (dataItemSerializer != null)
				return dataItemSerializer.LoadFromXml(element, context);
			return null;
		}
	}
	public static class XmlHelper {
		internal const string xmlColor = "Color";
		const string xmlType = "Type";
		const string xmlValue = "Value";
		public static string GetAttributeValue(this XElement element, string attributeName) {
			Guard.ArgumentNotNull(element, "element");
			Guard.ArgumentIsNotNullOrEmpty(attributeName, "attributeName");
			XAttribute attrbute = element.Attribute(attributeName);
			return attrbute == null ? null : attrbute.Value;
		}
		public static XElement Element(this XElement element, IList<string> expectedNames) {
			foreach(string name in expectedNames) {
				XElement elem = element.Element(name);
				if(elem != null)
					return elem;
			}
			return null;
		}
		public static string ToString(object value) {
			if (value == null)
				return null;
			Type type = value.GetType();
			if (type == typeof(DateTime))
				return ((DateTime)value).ToString("MM/dd/yyyy HH:mm:ss.fff", DateTimeFormatInfo.InvariantInfo);
			if (type == typeof(TimeSpan))
				return XmlConvert.ToString((TimeSpan)value);
			return TypeDescriptor.GetConverter(type).ConvertToInvariantString(value);
		}
		public static T FromString<T>(string str) {
			return (T)XmlHelperBase.FromString(typeof(T), str);
		}
		public static object FromString(Type type, string str)  {
			return XmlHelperBase.FromString(type, str);
		}
		public static TEnum EnumFromString<TEnum>(string str) {
			return (TEnum)Enum.Parse(typeof(TEnum), str);
		}
		public static void Save<T>(XElement element, string name, T value) {
			element.Add(new XAttribute(name, value));
		}
		public static void Save<T>(XElement element, string name, T value, T defaultValue) {
			if (!object.Equals(value, defaultValue))
				element.Add(new XAttribute(name, value));
		}
		public static void SaveObject(XElement element, string name, object obj) {
			if(!ReferenceEquals(obj, null)) {
				XElement objElement = new XElement(name);
				SaveObjectToXml(obj, objElement);
				element.Add(objElement);
			}
		}
		public static void Load<T>(XElement element, string name, Action<T> setter) {
			string value = XmlHelper.GetAttributeValue(element, name);
			if (!string.IsNullOrEmpty(value))
				setter(XmlHelper.FromString<T>(value));
		}
		public static void LoadEnum<T>(XElement element, string name, Action<T> setter) {
			string value = XmlHelper.GetAttributeValue(element, name);
			if (!string.IsNullOrEmpty(value))
				setter(XmlHelper.EnumFromString<T>(value));
		}
		public static void LoadObject(XElement element, string name, Action<object> setter) {
			XElement objElement = element.Element(name);
			if(!ReferenceEquals(objElement, null))
				setter(LoadObjectFromXml(objElement));
		}
		public static void LoadColor(XElement element, string name, Action<Color> setter) {
			string colorAttribute = XmlHelper.GetAttributeValue(element, name);
			if(!string.IsNullOrEmpty(colorAttribute))
				setter(ColorFromString(colorAttribute));
		}
		public static Color ColorFromString(string str) {
			int argb = 0;
			int.TryParse(str, out argb);
			return Color.FromArgb(argb);
		}
		internal  static XElement SaveToXml(this Color color) {
			return new XElement(xmlColor, color.ToArgb());
		}
		internal static double LoadDoubleFromXml(XElement element, string attributeName) {
			string attribute = XmlHelper.GetAttributeValue(element, attributeName);
			if (String.IsNullOrEmpty(attribute))
				throw new XmlException("LoadDoubleFromXml");
			return XmlHelper.FromString<double>(attribute);
		}
		public static void CheckStream(Stream stream) {
			if(stream == null)
				throw new ArgumentNullException(DashboardLocalizer.GetString(DashboardStringId.MessageInvalidStream));
		}
		public static void SaveXmlToStream(Stream stream, XElement rootElement) {
			XmlWriterSettings settings = new XmlWriterSettings() {
				Encoding = Encoding.UTF8,
				Indent = true
			};
			XmlWriter writer = XmlWriter.Create(stream, settings);
			try {
				new XDocument(rootElement).WriteTo(writer);
			}
			finally {
				writer.Flush();
			}
		}
		public static XElement LoadXmlFromStream(Stream stream, string rootElementName) {
			XmlReader reader = XmlReader.Create(stream);
			XDocument document = XDocument.Load(reader);
			if(document == null)
				throw new XmlException("LoadXmlFromStream");
			XElement rootElement = document.Root;
			if(rootElement == null || rootElement.Name != rootElementName )
				throw new XmlException("LoadXmlFromStream");
			return rootElement;
		}
		static void SaveObjectToXml(object obj, XElement element) {
			element.Add(new XAttribute(xmlType, obj.GetType().FullName));
			element.Add(new XAttribute(xmlValue, ToString(obj)));
		}
		static object LoadObjectFromXml(XElement element) {
			string attr = element.GetAttributeValue(xmlType);
			if(!string.IsNullOrEmpty(attr)) {
				Type type = Type.GetType(attr, true);
				attr = element.GetAttributeValue(xmlValue);
				if(attr != null)
					return FromString(type, attr);
			}
			throw new XmlException("LoadObjectFromXml");
		}
	}
}
