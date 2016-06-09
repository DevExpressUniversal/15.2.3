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
using System.Text;
using System.IO;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using System.Globalization;
using System.Collections.Generic;
namespace DevExpress.XtraPrinting.HtmlExport.Native {
	public sealed class DXCssTextWriter : TextWriter {
		#region static
		static void WriteUrlAttribute(TextWriter writer, string url) {
			string str = url;
			if(url.StartsWith("url(")) {
				int startIndex = 4;
				int length = url.Length - 4;
				if(DXWebStringUtil.StringEndsWith(url, ')')) {
					length--;
				}
				str = url.Substring(startIndex, length).Trim();
			}
			writer.Write("url(");
			writer.Write(DXHttpUtility.UrlPathEncode(str));
			writer.Write(')');
		}
		static DXCssTextWriter() {
			RegisterAttribute("background-color", DXHtmlTextWriterStyle.BackgroundColor);
			RegisterAttribute("background-image", DXHtmlTextWriterStyle.BackgroundImage, true, true);
			RegisterAttribute("border-collapse", DXHtmlTextWriterStyle.BorderCollapse);
			RegisterAttribute("border-color", DXHtmlTextWriterStyle.BorderColor);
			RegisterAttribute("border-style", DXHtmlTextWriterStyle.BorderStyle);
			RegisterAttribute("border-width", DXHtmlTextWriterStyle.BorderWidth);
			RegisterAttribute("color", DXHtmlTextWriterStyle.Color);
			RegisterAttribute("cursor", DXHtmlTextWriterStyle.Cursor);
			RegisterAttribute("direction", DXHtmlTextWriterStyle.Direction);
			RegisterAttribute("display", DXHtmlTextWriterStyle.Display);
			RegisterAttribute("filter", DXHtmlTextWriterStyle.Filter);
			RegisterAttribute("font-family", DXHtmlTextWriterStyle.FontFamily, true);
			RegisterAttribute("font-size", DXHtmlTextWriterStyle.FontSize);
			RegisterAttribute("font-style", DXHtmlTextWriterStyle.FontStyle);
			RegisterAttribute("font-variant", DXHtmlTextWriterStyle.FontVariant);
			RegisterAttribute("font-weight", DXHtmlTextWriterStyle.FontWeight);
			RegisterAttribute("height", DXHtmlTextWriterStyle.Height);
			RegisterAttribute("left", DXHtmlTextWriterStyle.Left);
			RegisterAttribute("list-style-image", DXHtmlTextWriterStyle.ListStyleImage, true, true);
			RegisterAttribute("list-style-type", DXHtmlTextWriterStyle.ListStyleType);
			RegisterAttribute("margin", DXHtmlTextWriterStyle.Margin);
			RegisterAttribute("margin-bottom", DXHtmlTextWriterStyle.MarginBottom);
			RegisterAttribute("margin-left", DXHtmlTextWriterStyle.MarginLeft);
			RegisterAttribute("margin-right", DXHtmlTextWriterStyle.MarginRight);
			RegisterAttribute("margin-top", DXHtmlTextWriterStyle.MarginTop);
			RegisterAttribute("overflow-x", DXHtmlTextWriterStyle.OverflowX);
			RegisterAttribute("overflow-y", DXHtmlTextWriterStyle.OverflowY);
			RegisterAttribute("overflow", DXHtmlTextWriterStyle.Overflow);
			RegisterAttribute("padding", DXHtmlTextWriterStyle.Padding);
			RegisterAttribute("padding-bottom", DXHtmlTextWriterStyle.PaddingBottom);
			RegisterAttribute("padding-left", DXHtmlTextWriterStyle.PaddingLeft);
			RegisterAttribute("padding-right", DXHtmlTextWriterStyle.PaddingRight);
			RegisterAttribute("padding-top", DXHtmlTextWriterStyle.PaddingTop);
			RegisterAttribute("position", DXHtmlTextWriterStyle.Position);
			RegisterAttribute("text-align", DXHtmlTextWriterStyle.TextAlign);
			RegisterAttribute("text-decoration", DXHtmlTextWriterStyle.TextDecoration);
			RegisterAttribute("text-overflow", DXHtmlTextWriterStyle.TextOverflow);
			RegisterAttribute("top", DXHtmlTextWriterStyle.Top);
			RegisterAttribute("vertical-align", DXHtmlTextWriterStyle.VerticalAlign);
			RegisterAttribute("visibility", DXHtmlTextWriterStyle.Visibility);
			RegisterAttribute("width", DXHtmlTextWriterStyle.Width);
			RegisterAttribute("white-space", DXHtmlTextWriterStyle.WhiteSpace);
			RegisterAttribute("z-index", DXHtmlTextWriterStyle.ZIndex);
		}
		static Dictionary<string, DXHtmlTextWriterStyle> styleLookup = new Dictionary<string, DXHtmlTextWriterStyle>(43);
		static AttributeInformation[] attributeNameLookup = new AttributeInformation[43];
		#endregion
		public DXCssTextWriter(TextWriter writer) {
			this.writer = writer;
		}
#if DXRESTRICTED
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (writer != null) {
						writer.Dispose();
						writer = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
#else
		public override void Close() {
			writer.Close();
		}
#endif
		public override void Flush() {
			writer.Flush();
		}
		public static DXHtmlTextWriterStyle GetStyleKey(string styleName) {
			if(!string.IsNullOrEmpty(styleName)) {
				DXHtmlTextWriterStyle obj2;
				if (styleLookup.TryGetValue(styleName.ToLowerInvariant(), out obj2))
					return obj2;
			}
			return (DXHtmlTextWriterStyle)(-1);
		}
		public static string GetStyleName(DXHtmlTextWriterStyle styleKey) {
			if((styleKey >= DXHtmlTextWriterStyle.BackgroundColor) && ((int)styleKey < attributeNameLookup.Length)) {
				return attributeNameLookup[(int)styleKey].name;
			}
			return string.Empty;
		}
		public static bool IsStyleEncoded(DXHtmlTextWriterStyle styleKey) {
			if((styleKey >= DXHtmlTextWriterStyle.BackgroundColor) && ((int)styleKey < attributeNameLookup.Length)) {
				return attributeNameLookup[(int)styleKey].encode;
			}
			return true;
		}
		internal static void RegisterAttribute(string name, DXHtmlTextWriterStyle key) {
			RegisterAttribute(name, key, false, false);
		}
		internal static void RegisterAttribute(string name, DXHtmlTextWriterStyle key, bool encode) {
			RegisterAttribute(name, key, encode, false);
		}
		internal static void RegisterAttribute(string name, DXHtmlTextWriterStyle key, bool encode, bool isUrl) {
			string str = name.ToLowerInvariant();
			styleLookup.Add(str, key);
			if((int)key < attributeNameLookup.Length)
				attributeNameLookup[(int)key] = new AttributeInformation(name, encode, isUrl);
		}
		public override void Write(bool value) {
			writer.Write(value);
		}
		public override void Write(char value) {
			writer.Write(value);
		}
		public override void Write(int value) {
			writer.Write(value);
		}
		public override void Write(char[] buffer) {
			writer.Write(buffer);
		}
		public override void Write(double value) {
			writer.Write(value);
		}
		public override void Write(long value) {
			writer.Write(value);
		}
		public override void Write(object value) {
			writer.Write(value);
		}
		public override void Write(float value) {
			writer.Write(value);
		}
		public override void Write(string s) {
			writer.Write(s);
		}
		public override void Write(string format, params object[] arg) {
			writer.Write(format, arg);
		}
		public override void Write(string format, object arg0) {
			writer.Write(format, arg0);
		}
		public override void Write(string format, object arg0, object arg1) {
			writer.Write(format, arg0, arg1);
		}
		public override void Write(char[] buffer, int index, int count) {
			writer.Write(buffer, index, count);
		}
		public void WriteAttribute(string name, string value) {
			WriteAttribute(writer, GetStyleKey(name), name, value);
		}
		public void WriteAttribute(DXHtmlTextWriterStyle key, string value) {
			WriteAttribute(writer, key, GetStyleName(key), value);
		}
		private static void WriteAttribute(TextWriter writer, DXHtmlTextWriterStyle key, string name, string value) {
			writer.Write(name);
			writer.Write(':');
			bool isUrl = false;
			if(key != (DXHtmlTextWriterStyle)(-1))
				isUrl = attributeNameLookup[(int)key].isUrl;
			if(!isUrl)
				writer.Write(value);
			else
				WriteUrlAttribute(writer, value);
			writer.Write(';');
		}
		internal static void WriteAttributes(TextWriter writer, List<DXWebRenderStyle> styles) {
			for(int i = 0; i < styles.Count; i++) {
				DXWebRenderStyle style = styles[i];
				WriteAttribute(writer, style.key, style.name, style.value);
			}
		}
		public void WriteBeginCssRule(string selector) {
			writer.Write(selector);
			writer.Write(" { ");
		}
		public void WriteEndCssRule() {
			writer.WriteLine(" }");
		}
		public override void WriteLine() {
			writer.WriteLine();
		}
		public override void WriteLine(bool value) {
			writer.WriteLine(value);
		}
		public override void WriteLine(char value) {
			writer.WriteLine(value);
		}
		public override void WriteLine(double value) {
			writer.WriteLine(value);
		}
		public override void WriteLine(char[] buffer) {
			writer.WriteLine(buffer);
		}
		public override void WriteLine(int value) {
			writer.WriteLine(value);
		}
		public override void WriteLine(long value) {
			writer.WriteLine(value);
		}
		public override void WriteLine(object value) {
			writer.WriteLine(value);
		}
		public override void WriteLine(float value) {
			writer.WriteLine(value);
		}
		public override void WriteLine(string s) {
			writer.WriteLine(s);
		}
		[CLSCompliant(false)]
		public override void WriteLine(uint value) {
			writer.WriteLine(value);
		}
		public override void WriteLine(string format, object arg0) {
			writer.WriteLine(format, arg0);
		}
		public override void WriteLine(string format, params object[] arg) {
			writer.WriteLine(format, arg);
		}
		public override void WriteLine(char[] buffer, int index, int count) {
			writer.WriteLine(buffer, index, count);
		}
		public override void WriteLine(string format, object arg0, object arg1) {
			writer.WriteLine(format, arg0, arg1);
		}
		public override Encoding Encoding {
			get { return writer.Encoding; }
		}
		public override string NewLine {
			get { return writer.NewLine; }
			set { writer.NewLine = value; }
		}
		private struct AttributeInformation {
			public string name;
			public bool isUrl;
			public bool encode;
			public AttributeInformation(string name, bool encode, bool isUrl) {
				this.name = name;
				this.encode = encode;
				this.isUrl = isUrl;
			}
		}
		TextWriter writer;
	}
}
