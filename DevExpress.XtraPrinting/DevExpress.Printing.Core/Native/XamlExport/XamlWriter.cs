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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using DevExpress.Utils;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraPrinting.XamlExport {
	public class XamlWriter : IDisposable {
		const string defaultFormatString = "{0}";
		const string argbColorFormatString = "#{0:X8}";
		XmlWriter writer;
		bool disposed;
		public XamlWriter(Stream output) {
			Guard.ArgumentNotNull(output, "output");
			var settings = new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true };
			writer = XmlWriter.Create(output, settings);
		}
		public void WriteStartElement(string tag) {
			writer.WriteStartElement(tag);
		}
		public void WriteStartElement(string tag, string ns) {
			writer.WriteStartElement(tag, ns);
		}
		public void WriteStartElement(string prefix, string tag, string ns) {
			writer.WriteStartElement(prefix, tag, ns);
		}
		public void WriteEndElement() {
			writer.WriteEndElement();
		}
		public void WriteValue(string value) {
			Guard.ArgumentNotNull(value, "value");
			writer.WriteValue(value);
		}
		public void WriteAttribute(string prefix, string attribute, RectangleF value) {
			WriteAttribute(prefix, attribute, value.X, value.Y, value.Width, value.Height);
		}
		public void WriteAttribute(string prefix, string attribute, params float[] values) {
			string stringValue = JoinItemsToString(values);
			WriteAttribute(prefix, attribute, stringValue);
		}
		public void WriteAttribute(string prefix, string attribute, Color value) {
			string formattedValue = string.Format(CultureInfo.InvariantCulture, argbColorFormatString, value.ToArgb());
			WriteAttribute(prefix, attribute, formattedValue);
		}
		public void WriteAttribute(string prefix, string attribute, string value) {
			writer.WriteAttributeString(prefix, attribute, null, value);
		}
		public void WriteAttribute(string attribute, RectangleF value) {
			WriteAttribute(attribute, value.X, value.Y, value.Width, value.Height);
		}
		public void WriteAttribute(string attribute, params float[] values) {
			Guard.ArgumentNotNull(values, "values");
			string stringValue = JoinItemsToString(values);
			WriteAttribute(attribute, stringValue);
		}
		public void WriteAttribute(string attribute, string value) {
			Guard.ArgumentNotNull(value, "value");
			WriteAttributeCore(attribute, defaultFormatString, value);
		}
		public void WriteAttribute(string attribute, float value) {
			WriteAttributeCore(attribute, defaultFormatString, value);
		}
		public void WriteAttribute(string attribute, Color value) {
			WriteAttributeCore(attribute, argbColorFormatString, value.ToArgb());
		}
		void WriteAttributeCore(string attribute, string format, object value) {
			string formattedValue = string.Format(CultureInfo.InvariantCulture, format, value);
			if(attribute == XamlAttribute.Text && formattedValue.Length > 0 && formattedValue[0] == '{') {
				formattedValue = "{}" + formattedValue;
			}
			string validFormattedValue = RemoveInvalidXmlCharacters(formattedValue);
			writer.WriteAttributeString(attribute, validFormattedValue);
		}
		public void WriteNamespace(string prefix, string value) {
			const string xmlNamespacePrefix = "xmlns";
			writer.WriteAttributeString(xmlNamespacePrefix, prefix, null, value);
		}
		public void WriteSetter(string attribute, string value) {
			Guard.ArgumentNotNull(value, "value");
			WriteSetterCore(attribute, defaultFormatString, value);
		}
		public void WriteSetter(string attribute, float value) {
			WriteSetterCore(attribute, defaultFormatString, value);
		}
		public void WriteSetter(string attribute, Color value) {
			WriteSetterCore(attribute, argbColorFormatString, value.ToArgb());
		}
		public void WriteSetter(string attribute, Thickness value) {
			WriteSetter(attribute, (float)value.Left, (float)value.Top, (float)value.Right, (float)value.Bottom);
		}
		public void WriteSetter(string attribute, params float[] values) {
			Guard.ArgumentNotNull(values, "values");
			string stringValue = JoinItemsToString(values);
			WriteSetterCore(attribute, defaultFormatString, stringValue);
		}
		public void WriteRaw(string data) {
			writer.WriteRaw(data);
		}
		void WriteSetterCore(string attribute, string format, object value) {
			WriteStartElement(XamlTag.Setter);
			WriteAttribute(XamlAttribute.Property, attribute);
			WriteAttributeCore(XamlAttribute.Value, format, value);
			WriteEndElement(); 
		}
		public void Flush() {
			writer.Flush();
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		void Dispose(bool disposing) {
			if(!disposed) {
				if(disposing) {
					writer.Close();
					writer = null;
				}
				disposed = true;
			}
		}
		static string JoinItemsToString<T>(IEnumerable<T> items) where T : IConvertible {
			var stringItems = items.Select(value => value.ToString(CultureInfo.InvariantCulture));
			return string.Join(",", stringItems);
		}
		static string RemoveInvalidXmlCharacters(string text) {
			if(string.IsNullOrEmpty(text)) return string.Empty;
			return string.Concat(text.Where(ch => XmlConvert.IsXmlChar(ch)));
		}
	}
}
