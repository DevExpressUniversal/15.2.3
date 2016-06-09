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
using DevExpress.Utils.Serializing.Helpers;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
namespace DevExpress.Utils.Serializing {
	public class XmlXtraSerializer : XtraSerializer {
		#region inner classes
		public class XmlXtraPropertyInfo : XtraPropertyInfo {
			public XmlXtraPropertyInfo(string name, Type propertyType, object val, bool isKey) : base(name, propertyType, val, isKey) { }
			public override object ValueToObject(Type type) {
				if(object.Equals(typeof(object), type)) {
					if(PropertyType != null)
						type = PropertyType;
				}
				if(Value is string) {
					object res = XmlXtraSerializer.XmlStringToObject(Value.ToString(), type);
					if(res is string)
						return ObjectConverterImplementation.StringToObject(res.ToString(), type);
					return res;
				}
				return Value;
			}
			public static TimeSpan StringToTimeSpan(string str) {
				return TimeSpan.Parse(str);
			}
			public static DateTime StringToDateTime(string str) {
				DateTime result;
				if(DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
					return result;
				else
#if !SILVERLIGHT && !DXPORTABLE
#pragma warning disable 618
					return XmlConvert.ToDateTime(str);
#pragma warning restore 618
#else
					return XmlConvert.ToDateTime(str, XmlDateTimeSerializationMode.RoundtripKind);
#endif
			}
		}
		#endregion
		#region static
		static readonly Dictionary<Type, MethodInfo> XmlConvertToStringMethods = new Dictionary<Type, MethodInfo>();
		static readonly Dictionary<Type, MethodInfo> XmlConvertFromStringMethods = new Dictionary<Type, MethodInfo>();
		static XmlXtraSerializer() {
			MethodInfo[] methods = typeof(XmlConvert).GetMethods(BindingFlags.Static | BindingFlags.Public);
			PopulateToStringMethods(methods);
			PopulateFromStringMethods(methods);
		}
		static void PopulateToStringMethods(MethodInfo[] methods) {
			int count = methods.Length;
			for(int i = 0; i < count; i++) {
				MethodInfo mi = methods[i];
				if(mi.Name != "ToString" || mi.ReturnType != typeof(string))
					continue;
				ParameterInfo[] parameters = mi.GetParameters();
				if(parameters.Length != 1)
					continue;
				XmlConvertToStringMethods.Add(parameters[0].ParameterType, mi);
			}
		}
		static void PopulateFromStringMethods(MethodInfo[] methods) {
			int count = methods.Length;
			for(int i = 0; i < count; i++) {
				MethodInfo mi = methods[i];
				string methodName = mi.Name;
				int index = methodName.IndexOf("To");
				if(index == 0) {
					string typeName = methodName.Substring(index + 2); 
					if(mi.ReturnType.Name == typeName) {
						ParameterInfo[] parameters = mi.GetParameters();
						if(parameters.Length == 1) {
							if(parameters[0].ParameterType == typeof(string))
								XmlConvertFromStringMethods.Add(mi.ReturnType, mi);
						}
					}
				}
			}
			MethodInfo result;
			MethodInfo timeSpanConverter = typeof(XmlXtraPropertyInfo).GetMethod("StringToTimeSpan");
			MethodInfo dateTimeConverter = typeof(XmlXtraPropertyInfo).GetMethod("StringToDateTime");
			System.Diagnostics.Debug.Assert(timeSpanConverter != null);
			System.Diagnostics.Debug.Assert(dateTimeConverter != null);
			if(XmlConvertFromStringMethods.TryGetValue(typeof(TimeSpan), out result))
				XmlConvertFromStringMethods[typeof(TimeSpan)] = timeSpanConverter;
			else
				XmlConvertFromStringMethods.Add(typeof(TimeSpan), timeSpanConverter);
			if(XmlConvertFromStringMethods.TryGetValue(typeof(DateTime), out result))
				XmlConvertFromStringMethods[typeof(DateTime)] = dateTimeConverter;
			else
				XmlConvertFromStringMethods.Add(typeof(DateTime), dateTimeConverter);
		}
		static MethodInfo FindXmlToStringMethod(Type type) {
			MethodInfo mi;
			XmlConvertToStringMethods.TryGetValue(type, out mi);
			return mi;
		}
		static MethodInfo FindXmlFromStringMethod(Type type) {
			MethodInfo mi;
			XmlConvertFromStringMethods.TryGetValue(type, out mi);
			return mi;
		}
		#endregion
		protected virtual string SerializerName { get { return "XtraSerializer"; } }
		protected virtual string Version { get { return "1.0"; } }
		public override bool CanUseStream { get { return true; } }
		protected override bool Serialize(Stream stream, IXtraPropertyCollection props, string appName) {
			return SerializeCore(stream, props, appName);
		}
		protected override bool Serialize(string path, IXtraPropertyCollection props, string appName) {
			bool res = false;
			FileStream stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
			res = SerializeCore(stream, props, appName);
			stream.Dispose();
			return res;
		}
		protected virtual bool SerializeCore(Stream stream, IXtraPropertyCollection props, string appName) {
			XmlWriter tw = CreateXmlTextWriter(stream);
			WriteStartDocument(tw);
			tw.WriteStartElement(SerializerName);
			WriteVersionAttribute(tw);
			WriteApplicationAttribute(appName, tw);
			SerializeLevel(tw, props);
			tw.WriteEndElement();
			tw.Flush();
			return true;
		}
		protected virtual XmlWriterSettings CreateXmlWriterSettings() {
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.CheckCharacters = false;
			settings.Encoding = System.Text.Encoding.UTF8;
			return settings;
		}
		protected XmlWriter CreateXmlTextWriter(Stream stream) {
			return XmlWriter.Create(stream, CreateXmlWriterSettings());
		}
		protected virtual void WriteStartDocument(XmlWriter tw) {
		}
		protected virtual void WriteApplicationAttribute(string appName, XmlWriter tw) {
			tw.WriteAttributeString("application", appName);
		}
		protected virtual void WriteVersionAttribute(XmlWriter tw) {
			tw.WriteAttributeString("version", Version);
		}
		protected void SerializeLevel(XmlWriter tw, IXtraPropertyCollection props) {
			if(props == null) return;
			SerializeLevelCore(tw, props);
		}
		protected virtual void SerializeLevelCore(XmlWriter tw, IXtraPropertyCollection props) {
			foreach(XtraPropertyInfo pInfo in props)
				SerializeProperty(tw, pInfo);
		}
		void SerializeProperty(XmlWriter writer, XtraPropertyInfo pInfo) {
			writer.WriteStartElement("property");
			Dictionary<string, string> attributes = GetAttributes(pInfo);
			foreach(KeyValuePair<string, string> pair in attributes) {
				writer.WriteAttributeString(pair.Key, pair.Value);
			}
			try {
				object val = pInfo.Value;
				if(val != null) {
					Type type = val.GetType();
					if(!type.IsPrimitive())
						val = ObjectConverterImpl.ObjectToString(val);
				}
				if(!pInfo.IsKey && pInfo.Value != null) {
					writer.WriteString(XmlObjectToString(val));
				}
				if(pInfo.IsKey) {
					SerializeLevel(writer, pInfo.ChildProperties);
				}
			}
			finally { writer.WriteEndElement(); }
		}
		protected virtual Dictionary<string, string> GetAttributes(XtraPropertyInfo pInfo) {
			Dictionary<string, string> attributes = new Dictionary<string, string>();
			attributes.Add("name", pInfo.Name);
			if(pInfo.Value == null)
				attributes.Add("isnull", "true");
			else {
				if(!pInfo.IsKey && object.Equals(pInfo.PropertyType, typeof(object))) {
					attributes.Add("type", pInfo.Value.GetType().FullName);
				}
			}
			if(pInfo.IsKey) {
				attributes.Add("iskey", "true");
				if(pInfo.Value != null)
					attributes.Add("value", XmlObjectToString(pInfo.Value));
			}
			return attributes;
		}
		public static object XmlStringToObject(string str, Type type) {
			if(str == NullValueString) return null;
			if(type.Equals(typeof(string))) return str;
			MethodInfo mi = FindXmlFromStringMethod(type);
			try {
				if(mi != null)
					return mi.Invoke(null, new object[] { str });
				else if(typeof(Enum).IsAssignableFrom(type))
					return Enum.Parse(type, str);
			} catch {
			}
			return str;
		}
		public static string XmlObjectToString(object val) {
			if(val is string) return val.ToString();
			if(val == null) return NullValueString;
			MethodInfo mi = FindXmlToStringMethod(val.GetType());
			try {
				if(mi != null) {
					object res = mi.Invoke(null, new object[] { val });
					if(res != null) return res.ToString();
				} else if(val is Enum)
					return val.ToString();
			} catch {
			}
			return val.ToString();
		}
		protected override IXtraPropertyCollection Deserialize(Stream stream, string appName, IList objects) {
			return DeserializeCore(stream, appName, objects);
		}
		protected override IXtraPropertyCollection Deserialize(string path, string appName, IList objects) {
			IXtraPropertyCollection res = null;
			FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
			try {
				res = DeserializeCore(stream, appName, objects);
			}
			finally {
				stream.Dispose();
			}
			return res;
		}
		protected virtual XmlReader CreateReader(Stream stream) {
#if DXPORTABLE
			XmlReaderSettings settings = new XmlReaderSettings();
			return XmlReader.Create(stream, settings);
#else
			return new XmlTextReader(stream);
#endif
		}
		protected virtual IXtraPropertyCollection DeserializeCore(Stream stream, string appName, IList objects) {
			XmlReader tr = CreateReader(stream);
			IXtraPropertyCollection list = new XtraPropertyCollection();
			while(tr.Read()) {
				if(tr.IsStartElement()) {
					if(tr.Name == "XtraSerializer") {
						string app = tr.GetAttribute("application");
						if(app == appName) {
							Read(tr, list, -1);
							break;
						}
					}
				}
			}
			if(list.Count == 0)
				return null;
			return list;
		}
		void Read(XmlReader reader, IXtraPropertyCollection list, int depth) {
			string name = "", val;
			bool isKey = false, isNullValue;
			ILineInfoProvider lineInfo = LineInfoProvider.Create(reader);
			while(!reader.EOF) {
				if(depth != -1 && reader.Depth <= depth)
					return;
				if(reader.IsStartElement()) {
					if(reader.Name == "property") {
						if(reader.AttributeCount < 1)
							continue;
						Dictionary<string, string> attributes = new Dictionary<string, string>();
						while(reader.MoveToNextAttribute()) {
							attributes.Add(reader.Name, reader.Value);
						}
						reader.MoveToElement();
						attributes.TryGetValue("name", out name);
						attributes.TryGetValue("value", out val);
						string _isnull, _iskey, _type;
						attributes.TryGetValue("isnull", out _isnull);
						attributes.TryGetValue("iskey", out _iskey);
						attributes.TryGetValue("type", out _type);
						isNullValue = (_isnull == "true");
						isKey = (_iskey == "true");
						Type propType = GetType(_type);
						XtraPropertyInfo prop = CreateXtraPropertyInfo(name, propType, isKey, attributes);
						prop.IsNull = isNullValue;
						int curDepth = reader.Depth;
						int lineN = lineInfo.LineNumber;
						int lineP = lineInfo.LinePosition;
						if(!isKey)
							val = reader.ReadString();
						if(!isNullValue) {
							prop.Value = val;
						}
						if(isKey && !reader.IsEmptyElement) {
							reader.Read();
							lineN = lineInfo.LineNumber;
							lineP = lineInfo.LinePosition;
							Read(reader, prop.ChildProperties, curDepth);
						}
						list.Add(prop);
						if (lineN == lineInfo.LineNumber && lineP == lineInfo.LinePosition)
							reader.Read();
						continue;
					}
				}
#if DXPORTABLE
				try {
					reader.Read();
				}
				catch (XmlException e) {
					if (!String.IsNullOrEmpty(e.Message) && e.Message.Contains("0x00"))
						return;
					throw;
				}
#else
				reader.Read();
#endif
			}
		}
		interface ILineInfoProvider {
			int LineNumber { get; }
			int LinePosition { get; }
		}
		class LineInfoProvider : ILineInfoProvider {
			readonly IXmlLineInfo lineInfo;
			LineInfoProvider(IXmlLineInfo lineInfo) {
				this.lineInfo = lineInfo;
			}
			public static ILineInfoProvider Create(XmlReader reader) {
				IXmlLineInfo lineInfo = reader as IXmlLineInfo;
				if (lineInfo != null)
					return new LineInfoProvider(lineInfo);
				else
					return new FakeLineInfoProvider();
			}
			public int LineNumber { get { return lineInfo.LineNumber; } }
			public int LinePosition { get { return lineInfo.LinePosition; } }
		}
		class FakeLineInfoProvider : ILineInfoProvider {
			public int LineNumber { get { return 1; } }
			public int LinePosition { get { return 1; } }
		}
		Type GetType(string typeName) {
			if(!String.IsNullOrEmpty(typeName)) {
				Type type = Type.GetType(typeName, false);
				if(type == null && HasCustomObjectConverter)
					type = CustomObjectConverter.GetType(typeName);
				return type;
			}
			return (Type)null;
		}
		protected virtual XtraPropertyInfo CreateXtraPropertyInfo(string name, Type propType, bool isKey, Dictionary<string, string> attributes) {
			return new XmlXtraPropertyInfo(name, propType, null, isKey);
		}
	}
}
