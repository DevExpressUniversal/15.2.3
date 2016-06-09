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
using System.Text;
using System.Xml;
using DevExpress.Utils.Zip;
using DevExpress.Office.Utils;
#if !SL
using System.IO.Compression;
#endif
namespace DevExpress.Utils {
	#region XmlBasedExporterUtils
	public class XmlBasedExporterUtils {
		readonly StringBuilder preProcessVariableValueStringBuilder;
		readonly FastCharacterMultiReplacement xmlCharsReplacement;
		readonly Dictionary<char, string> xmlCharsReplacementTable = CreateVariableValueReplacementTable();
		readonly Dictionary<char, string> xmlCharsNoCrLfReplacementTable = CreateVariableValueNoCrLfReplacementTable();
		readonly Dictionary<char, string> xmlCharsNoCrLfTabReplacementTable = CreateVariableValueNoTabCrLfReplacementTable();
		[ThreadStatic]
		static XmlBasedExporterUtils instance;
		public static XmlBasedExporterUtils Instance {
			get {
				if (instance == null)
					instance = new XmlBasedExporterUtils();
				return instance;
			}
		}
		XmlBasedExporterUtils() {
			this.preProcessVariableValueStringBuilder = new StringBuilder();
			this.xmlCharsReplacement = new FastCharacterMultiReplacement(preProcessVariableValueStringBuilder);
			this.xmlCharsReplacementTable = CreateVariableValueReplacementTable();
			this.xmlCharsNoCrLfReplacementTable = CreateVariableValueNoCrLfReplacementTable();
		}
		public CompressedXmlStreamInfo BeginCreateUncompressedXmlContent(XmlWriterSettings xmlSettings) {
			CompressedXmlStreamInfo result = new CompressedXmlStreamInfo();
			result.MemoryStream = new MemoryStream();
			result.StreamWriter = new StreamWriter(result.MemoryStream, Encoding.UTF8);
			result.Writer = XmlWriter.Create(result.StreamWriter, xmlSettings);
			return result;
		}
		public Stream EndCreateUncompressedXmlContent(CompressedXmlStreamInfo info) {
			try {
				if (info.Writer != null)
					info.Writer.Flush();
				return new MemoryStream(info.MemoryStream.GetBuffer(), 0, (int)info.Stream.Length);
			}
			finally {
				Dispose(info.Writer);
				Dispose(info.StreamWriter);
				Dispose(info.MemoryStream);
			}
		}
		public Stream CreateUncompressedXmlContent(Action<XmlWriter> action, XmlWriterSettings xmlSettings) {
			CompressedXmlStreamInfo streamInfo = BeginCreateUncompressedXmlContent(xmlSettings);
			action(streamInfo.Writer);
			return EndCreateUncompressedXmlContent(streamInfo);
		}
		public CompressedXmlStreamInfo BeginCreateCompressedXmlContent(XmlWriterSettings xmlSettings) {
			CompressedXmlStreamInfo result = new CompressedXmlStreamInfo();
			result.Stream = new ChunkedMemoryStream();
			result.DeflateStream = new DeflateStream(result.Stream, CompressionMode.Compress);
			result.CrcStream = new Crc32Stream(result.DeflateStream);
			result.UncompressedSizeStream = new ByteCountStream(result.CrcStream);
			result.StreamWriter = new StreamWriter(result.UncompressedSizeStream, Encoding.UTF8);
			result.Writer = XmlWriter.Create(result.StreamWriter, xmlSettings);
			return result;
		}
		public CompressedStream EndCreateCompressedXmlContent(CompressedXmlStreamInfo info) {
			if (info.Writer != null) {
				info.Writer.Flush();
				Dispose(info.Writer);
			}
			if (info.StreamWriter != null) {
				info.StreamWriter.Flush();
				Dispose(info.StreamWriter);
			}
			if (info.DeflateStream != null)
				Dispose(info.DeflateStream);
			info.Stream.Seek(0, SeekOrigin.Begin);
			CompressedStream result = new CompressedStream();
			result.UncompressedSize = (int)info.UncompressedSizeStream.WriteCheckSum;
			result.Crc32 = (int)info.CrcStream.WriteCheckSum;
			result.Stream = info.Stream;
			return result;
		}
		void Dispose(IDisposable disposable) {
			if (disposable != null)
				disposable.Dispose();
		}
		public CompressedStream CreateCompressedXmlContent(Action<XmlWriter> action, XmlWriterSettings xmlSettings) {
			CompressedXmlStreamInfo streamInfo = BeginCreateCompressedXmlContent(xmlSettings);
			action(streamInfo.Writer);
			return EndCreateCompressedXmlContent(streamInfo);
		}
		public XmlWriterSettings CreateDefaultXmlWriterSettings() {
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = false;
			settings.Encoding = DXEncoding.UTF8NoByteOrderMarks;
			settings.CheckCharacters = true;
			settings.OmitXmlDeclaration = false;
			return settings;
		}
#region VariableValueReplacementTable
		static Dictionary<char, string> CreateVariableValueReplacementTable() {
			Dictionary<char, string> result = new Dictionary<char, string>();
			for (char i = '\x0'; i <= '\x1f'; i++)
				result.Add(i, String.Format("_x{0:x4}_", (int)i));
			result.Add('\xffff', String.Format("_x{0:x4}_", (int)'\xffff'));
			return result;
		}
		static Dictionary<char, string> CreateVariableValueNoCrLfReplacementTable() {
			Dictionary<char, string> result = new Dictionary<char, string>();
			for (char i = '\x0'; i <= '\x1f'; i++) {
				if (i != '\xA' && i != '\xD')
					result.Add(i, String.Format("_x{0:x4}_", (int)i));
			}
			result.Add('\xffff', String.Format("_x{0:x4}_", (int)'\xffff'));
			return result;
		}
		static Dictionary<char, string> CreateVariableValueNoTabCrLfReplacementTable() {
			Dictionary<char, string> result = new Dictionary<char, string>();
			for (char i = '\x0'; i <= '\x1f'; i++) {
				if (i != '\xA' && i != '\xD' && i != '\x9')
					result.Add(i, String.Format("_x{0:x4}_", (int)i));
			}
			result.Add('\xffff', String.Format("_x{0:x4}_", (int)'\xffff'));
			return result;
		}
#endregion
		public string EncodeXmlChars(string value) {
			return xmlCharsReplacement.PerformReplacements(value, xmlCharsReplacementTable);
		}
		public string EncodeXmlCharsNoCrLf(string value) {
			return xmlCharsReplacement.PerformReplacements(value, xmlCharsNoCrLfReplacementTable);
		}
		public string EncodeXmlCharsXML1_0(string value) {
			return xmlCharsReplacement.PerformReplacements(value, xmlCharsNoCrLfTabReplacementTable);
		}
	}
#endregion
	public class CompressedXmlStreamInfo {
		public MemoryStream MemoryStream { get; set; }
		public ChunkedMemoryStream Stream { get; set; }
		[CLSCompliant(false)]
		public Crc32Stream CrcStream { get; set; }
		public ByteCountStream UncompressedSizeStream { get; set; }
		public DeflateStream DeflateStream { get; set; }
		public StreamWriter StreamWriter { get; set; }
		public XmlWriter Writer { get; set; }
	}
}
