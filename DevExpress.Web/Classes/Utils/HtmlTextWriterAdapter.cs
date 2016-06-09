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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Internal {
	public class HtmlTextWriterAdapter : HtmlTextWriter {
		public HtmlTextWriterAdapter(HtmlTextWriter innerWriter) : base(innerWriter) { }
		protected HtmlTextWriter InnerHtmlTextWriter { get { return (HtmlTextWriter)InnerWriter; } }
		public override Encoding Encoding { get { return InnerHtmlTextWriter.Encoding; } }
		public override IFormatProvider FormatProvider { get { return InnerHtmlTextWriter.FormatProvider; } }
		public override string NewLine { get { return InnerHtmlTextWriter.NewLine; } set { InnerHtmlTextWriter.NewLine = value; } }
		public override void AddAttribute(string name, string value) {
			InnerHtmlTextWriter.AddAttribute(name, value);
		}
		public override void AddAttribute(string name, string value, bool fEndode) {
			InnerHtmlTextWriter.AddAttribute(name, value, fEndode);
		}
		public override void AddAttribute(HtmlTextWriterAttribute key, string value) {
			InnerHtmlTextWriter.AddAttribute(key, value);
		}
		public override void AddAttribute(HtmlTextWriterAttribute key, string value, bool fEncode) {
			InnerHtmlTextWriter.AddAttribute(key, value, fEncode);
		}
		public override void AddStyleAttribute(string name, string value) {
			InnerHtmlTextWriter.AddStyleAttribute(name, value);
		}
		public override void AddStyleAttribute(HtmlTextWriterStyle key, string value) {
			InnerHtmlTextWriter.AddStyleAttribute(key, value);
		}
		public override void BeginRender() {
			InnerHtmlTextWriter.BeginRender();
		}
		public override void Close() {
			InnerHtmlTextWriter.Close();
		}
		public override void EndRender() {
			InnerHtmlTextWriter.EndRender();
		}
		public override void EnterStyle(Style style) {
			InnerHtmlTextWriter.EnterStyle(style);
		}
		public override void EnterStyle(Style style, HtmlTextWriterTag tag) {
			InnerHtmlTextWriter.EnterStyle(style, tag);
		}
		public override void ExitStyle(Style style) {
			InnerHtmlTextWriter.ExitStyle(style);
		}
		public override void ExitStyle(Style style, HtmlTextWriterTag tag) {
			InnerHtmlTextWriter.ExitStyle(style, tag);
		}
		public override void Flush() {
			InnerHtmlTextWriter.Flush();
		}
		public override bool IsValidFormAttribute(string attribute) {
			return InnerHtmlTextWriter.IsValidFormAttribute(attribute);
		}
		public override void RenderBeginTag(string tagName) {
			InnerHtmlTextWriter.RenderBeginTag(tagName);
		}
		public override void RenderBeginTag(HtmlTextWriterTag tagKey) {
			InnerHtmlTextWriter.RenderBeginTag(tagKey);
		}
		public override void RenderEndTag() {
			InnerHtmlTextWriter.RenderEndTag();
		}
		public override string ToString() {
			return InnerHtmlTextWriter.ToString();
		}
		public override void Write(bool value) {
			InnerHtmlTextWriter.Write(value);
		}
		public override void Write(char value) {
			InnerHtmlTextWriter.Write(value);
		}
		public override void Write(char[] buffer) {
			InnerHtmlTextWriter.Write(buffer);
		}
		public override void Write(char[] buffer, int index, int count) {
			InnerHtmlTextWriter.Write(buffer, index, count);
		}
		public override void Write(decimal value) {
			InnerHtmlTextWriter.Write(value);
		}
		public override void Write(double value) {
			InnerHtmlTextWriter.Write(value);
		}
		public override void Write(float value) {
			InnerHtmlTextWriter.Write(value);
		}
		public override void Write(int value) {
			InnerHtmlTextWriter.Write(value);
		}
		public override void Write(long value) {
			InnerHtmlTextWriter.Write(value);
		}
		public override void Write(object value) {
			InnerHtmlTextWriter.Write(value);
		}
		public override void Write(string format, object arg0) {
			InnerHtmlTextWriter.Write(format, arg0);
		}
		public override void Write(string format, object arg0, object arg1) {
			InnerHtmlTextWriter.Write(format, arg0, arg1);
		}
		public override void Write(string format, object arg0, object arg1, object arg2) {
			InnerHtmlTextWriter.Write(format, arg0, arg1, arg2);
		}
		public override void Write(string format, params object[] arg) {
			InnerHtmlTextWriter.Write(format, arg);
		}
		public override void Write(string s) {
			InnerHtmlTextWriter.Write(s);
		}
		[CLSCompliant(false)]
		public override void Write(uint value) {
			InnerHtmlTextWriter.Write(value);
		}
		[CLSCompliant(false)]
		public override void Write(ulong value) {
			InnerHtmlTextWriter.Write(value);
		}
		public override void WriteAttribute(string name, string value) {
			InnerHtmlTextWriter.WriteAttribute(name, value);
		}
		public override void WriteAttribute(string name, string value, bool fEncode) {
			InnerHtmlTextWriter.WriteAttribute(name, value, fEncode);
		}
		public override void WriteBeginTag(string tagName) {
			InnerHtmlTextWriter.WriteBeginTag(tagName);
		}
		public override void WriteBreak() {
			InnerHtmlTextWriter.WriteBreak();
		}
		public override void WriteEncodedText(string text) {
			InnerHtmlTextWriter.WriteEncodedText(text);
		}
		public override void WriteEncodedUrl(string url) {
			InnerHtmlTextWriter.WriteEncodedUrl(url);
		}
		public override void WriteEncodedUrlParameter(string urlText) {
			InnerHtmlTextWriter.WriteEncodedUrlParameter(urlText);
		}
		public override void WriteEndTag(string tagName) {
			InnerHtmlTextWriter.WriteEndTag(tagName);
		}
		public override void WriteFullBeginTag(string tagName) {
			InnerHtmlTextWriter.WriteFullBeginTag(tagName);
		}
		public override void WriteLine() {
			InnerHtmlTextWriter.WriteLine();
		}
		public override void WriteLine(bool value) {
			InnerHtmlTextWriter.WriteLine(value);
		}
		public override void WriteLine(char value) {
			InnerHtmlTextWriter.WriteLine(value);
		}
		public override void WriteLine(char[] buffer) {
			InnerHtmlTextWriter.WriteLine(buffer);
		}
		public override void WriteLine(char[] buffer, int index, int count) {
			InnerHtmlTextWriter.WriteLine(buffer, index, count);
		}
		public override void WriteLine(decimal value) {
			InnerHtmlTextWriter.WriteLine(value);
		}
		public override void WriteLine(double value) {
			InnerHtmlTextWriter.WriteLine(value);
		}
		public override void WriteLine(float value) {
			InnerHtmlTextWriter.WriteLine(value);
		}
		public override void WriteLine(int value) {
			InnerHtmlTextWriter.WriteLine(value);
		}
		public override void WriteLine(long value) {
			InnerHtmlTextWriter.WriteLine(value);
		}
		public override void WriteLine(object value) {
			InnerHtmlTextWriter.WriteLine(value);
		}
		public override void WriteLine(string format, object arg0) {
			InnerHtmlTextWriter.WriteLine(format, arg0);
		}
		public override void WriteLine(string format, object arg0, object arg1) {
			InnerHtmlTextWriter.WriteLine(format, arg0, arg1);
		}
		public override void WriteLine(string format, object arg0, object arg1, object arg2) {
			InnerHtmlTextWriter.WriteLine(format, arg0, arg1, arg2);
		}
		public override void WriteLine(string format, params object[] arg) {
			InnerHtmlTextWriter.WriteLine(format, arg);
		}
		public override void WriteLine(string s) {
			InnerHtmlTextWriter.WriteLine(s);
		}
		[CLSCompliant(false)]
		public override void WriteLine(uint value) {
			InnerHtmlTextWriter.WriteLine(value);
		}
		[CLSCompliant(false)]
		public override void WriteLine(ulong value) {
			InnerHtmlTextWriter.WriteLine(value);
		}
		public override void WriteStyleAttribute(string name, string value) {
			InnerHtmlTextWriter.WriteStyleAttribute(name, value);
		}
		public override void WriteStyleAttribute(string name, string value, bool fEncode) {
			InnerHtmlTextWriter.WriteStyleAttribute(name, value, fEncode);
		}
	}
}
