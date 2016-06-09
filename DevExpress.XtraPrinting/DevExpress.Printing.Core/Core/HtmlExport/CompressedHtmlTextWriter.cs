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
using System.IO;
using System.Text;
using DevExpress.XtraPrinting.HtmlExport.Native;
namespace DevExpress.XtraPrinting.Export.Web {
	public class Base64Writer : TextWriter {
		DXHtmlTextWriter writer;
		char[] buffer;
		Encoding encoding;
		int size;
		bool haveWrittenPreamble;
		public Base64Writer(DXHtmlTextWriter writer) : this(writer, true) { 
		}
		public Base64Writer(DXHtmlTextWriter writer, bool writePreamble) {
			this.writer = writer;
			buffer = new char[8196];
			encoding = new UTF8Encoding(writePreamble, true);
		}
		public override void Write(char value) {
			if(size >= buffer.Length)
				Flush();
			buffer[size] = value;
			size++;
		}
		public override void Flush() {
			Flush(false);
		}
		void Flush(bool end) {
			WritePreamble();
			int len = size;
			byte[] bytes;
			for(; ; len--) {
				if(len == 0)
					return;
				bytes = encoding.GetBytes(buffer, 0, len);
				if(end || bytes.Length % 3 == 0)
					break;
			}
			writer.Write(Convert.ToBase64String(bytes));
			writer.Flush();
			size -= len;
			if(size != 0)
				Array.Copy(buffer, len, buffer, 0, size);
		}
		void WritePreamble() {
			if(!haveWrittenPreamble) {
				writer.Write(Convert.ToBase64String(Encoding.GetPreamble()));
				haveWrittenPreamble = true;
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing && writer != null) {
				Flush(true);
				writer = null;
			}
			base.Dispose(disposing);
		}
		public override Encoding Encoding {
			get { return encoding; }
		}
	}
	public class CompressedHtmlTextWriter : DXHtmlTextWriter {
		public CompressedHtmlTextWriter(TextWriter writer)
			: base(writer) {
			NewLine = "";
		}
		protected override void OutputTabs() {
		}
		public override void WriteAttribute(string name, string value, bool fEncode) {
			if(name == "colspan" && value == "1")
				return;
			base.WriteAttribute(name, value, fEncode);
		}
	}
}
