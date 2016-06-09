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
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;
namespace DevExpress.XtraPrinting.Native.WebClientUIControl {
	public static class JsonGenerator {
		public static Encoding Encoding { get; private set; }
		static JsonGenerator() {
			Encoding = Encoding.UTF8;
		}
		public static bool TryProcessJsonAttribute(XElement element, out string attributeName) {
			attributeName = null;
			XAttribute typeAttribute = element.Attribute("type");
			XAttribute aAttribute = element.Attribute(XName.Get("a", "http://www.w3.org/2000/xmlns/"));
			if(typeAttribute == null || aAttribute == null) {
				return false;
			}
			if(typeAttribute.Value == "object" || aAttribute.Value != "item") {
				return false;
			}
			XAttribute itemAttribute = element.Attribute("item");
			if(itemAttribute == null) {
				return false;
			}
			string item = itemAttribute.Value;
			if(item.Length == 0 || item[0] != '@') {
				return false;
			}
			attributeName = item.Substring(1);
			return true;
		}
		public static XDocument JsonToCleanXmlDeclaration(string json) {
			var jsonData = Encoding.GetBytes(json);
			using(XmlReader jsonReader = JsonReaderWriterFactory.CreateJsonReader(jsonData, 0, jsonData.Length, Encoding, XmlDictionaryReaderQuotas.Max, null)) {
				var document = XDocument.Load(jsonReader);
				SafeCleanXmlDeclaration(document);
				return document;
			}
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public static void AppendQuotedValue(StringBuilder builder, string value, bool isKey = false) {
			builder.Append('\"');
			JsonGenerator.AppendSafeValue(builder, value);
			builder.Append('\"');
			if(isKey) {
				builder.Append(':');
			}
		}
		static void AppendSafeValue(StringBuilder builder, string value) {
			foreach(char ch in value) {
				if(Char.IsControl(ch) || ch == '\'') {
					int ich = (int)ch;
					builder.Append(@"\u" + ich.ToString("x4"));
					continue;
				} else if(ch == '\"' || ch == '\\' || ch == '/') {
					builder.Append('\\');
				}
				builder.Append(ch);
			}
		}
		static void SafeCleanXmlDeclaration(XDocument document) {
			var firstElement = document.Root.FirstNode as XElement;
			if(firstElement == null) {
				return;
			}
			var itemAttribute = firstElement.Attribute("item");
			if(itemAttribute != null && itemAttribute.Value == "?xml") {
				firstElement.Remove();
			}
		}
	}
}
