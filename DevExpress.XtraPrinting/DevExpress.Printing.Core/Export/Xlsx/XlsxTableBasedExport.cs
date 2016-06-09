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
using System.Globalization;
using System.IO;
using System.Xml;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using DevExpress.Office;
using DevExpress.XtraExport.Implementation;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using DevExpress.Export.Xl;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraExport.Xlsx {
	public class XlsxDataAwareExporterOptions : IXlDocumentOptions {
		public XlDocumentFormat DocumentFormat { get { return XlDocumentFormat.Xlsx; } }
		public CultureInfo Culture { get; set; }
		public bool SupportsFormulas { get { return true; } }
		public bool SupportsDocumentParts { get { return true; } }
		public bool SupportsOutlineGrouping { get { return true; } }
		public int MaxColumnCount { get { return 16384; } }
		public int MaxRowCount { get { return 1048576; } }
	}
	public partial class XlsxDataAwareExporter : IXlExport, IXlFormulaEngine, IXlExporter {
		XlsxPackageBuilder builder;
		XmlWriter writer;
		int sheetIndex;
		Stream outputStream = null;
		Stream bufferStream = null;
		readonly List<IXlSheet> sheets = new List<IXlSheet>();
		readonly Dictionary<IXlSheet, SheetInfo> sheetInfos = new Dictionary<IXlSheet, SheetInfo>();
		readonly Stack<CompressedXmlStreamInfo> streamInfoStack = new Stack<CompressedXmlStreamInfo>();
		readonly SimpleSharedStringTable sharedStringTable = new SimpleSharedStringTable();
		readonly XlsxDataAwareExporterOptions options = new XlsxDataAwareExporterOptions();
		readonly IXlFormulaParser formulaParser;
		public XlsxDataAwareExporter() {
			this.formulaParser = null;
		}
		public XlsxDataAwareExporter(IXlFormulaParser formulaParser) {
			this.formulaParser = formulaParser;
		}
		protected XlsxPackageBuilder Builder { get { return builder; } }
		public IXlDocumentOptions DocumentOptions { get { return options; } }
		public XlDocumentProperties DocumentProperties { get { return documentProperties; } }
		internal XlCalculationOptions CalculationOptions { get { return calculationOptions; } }
		CultureInfo CurrentCulture {
			get {
				if(options.Culture != null)
					return options.Culture;
				return CultureInfo.InvariantCulture;
			}
		}
		#region IXlExporter implementation
		public IXlDocument CreateDocument(Stream stream) {
			return BeginExport(stream);
		}
		#endregion
		#region IXlExport implementation
		public IXlFormulaEngine FormulaEngine { get { return this; } }
		public IXlDocument BeginExport(Stream outputStream) {
			Guard.ArgumentNotNull(outputStream, "outputStream");
			if (Builder != null)
				throw new InvalidOperationException(); 
			this.outputStream = outputStream;
			if(outputStream.CanSeek)
				this.bufferStream = outputStream;
			else
				this.bufferStream = new ChunkedMemoryStream();
			this.builder = new XlsxPackageBuilder(bufferStream);
			this.Builder.BeginExport();
			InitializeExport();
			return BeginDocument();
		}
		public void EndExport() {
			EndDocument();
			this.Builder.EndExport();
			this.builder = null;
			if(!object.ReferenceEquals(outputStream, bufferStream)) {
				this.bufferStream.Flush();
				this.bufferStream.Position = 0;
				this.bufferStream.CopyTo(this.outputStream);
			}
			this.bufferStream = null;
			this.outputStream = null;
		}
		#endregion
		protected internal virtual void InitializeExport() {
			streamInfoStack.Clear();
			sheets.Clear();
			sheetInfos.Clear();
			sharedStringTable.Clear();
			pendingColumns.Clear();
			columns.Clear();
			groups.Clear();
			imageFiles.Clear();
			exportedPictureHyperlinkTable.Clear();
			InitializeStyles();
			sheetIndex = 0;
			rowIndex = -1;
			columnIndex = -1;
			Builder.UsedContentTypes.Add("rels", XlsxPackageBuilder.RelsContentType);
			Builder.UsedContentTypes.Add("xml", XlsxPackageBuilder.XmlContentType);
			shapeId = 0;
			drawingId = 0;
			imageId = 0;
			tableId = 0;
			sheetRelations.Clear();
		}
		void BeginWriteXmlContent() {
			CompressedXmlStreamInfo streamInfo = XmlBasedExporterUtils.Instance.BeginCreateCompressedXmlContent(CreateXmlWriterSettings());
			BeginWriteXmlContentCore(streamInfo);
		}
		CompressedStream EndWriteXmlContent() {
			CompressedXmlStreamInfo streamInfo = EndWriteXmlContentCore();
			return XmlBasedExporterUtils.Instance.EndCreateCompressedXmlContent(streamInfo);
		}
		void BeginWriteUncompressedXmlContent() {
			CompressedXmlStreamInfo streamInfo = XmlBasedExporterUtils.Instance.BeginCreateUncompressedXmlContent(CreateXmlWriterSettings());
			BeginWriteXmlContentCore(streamInfo);
		}
		Stream EndWriteUncompressedXmlContent() {
			CompressedXmlStreamInfo streamInfo = EndWriteXmlContentCore();
			return XmlBasedExporterUtils.Instance.EndCreateUncompressedXmlContent(streamInfo);
		}
		void BeginWriteXmlContentCore(CompressedXmlStreamInfo streamInfo) {
			streamInfoStack.Push(streamInfo);
			this.writer = streamInfo.Writer;
		}
		CompressedXmlStreamInfo EndWriteXmlContentCore() {
			CompressedXmlStreamInfo streamInfo = streamInfoStack.Pop();
			if (streamInfoStack.Count > 0)
				this.writer = streamInfoStack.Peek().Writer;
			else
				this.writer = null;
			return streamInfo;
		}
		protected internal virtual void AddPackageContent(string fileName, Stream content) {
			if (content != null)
				Builder.Package.Add(fileName, Builder.Now, content);
		}
		protected internal virtual void AddPackageContent(string fileName, CompressedStream content) {
			if (content != null)
				Builder.Package.AddCompressed(fileName, Builder.Now, content);
		}
		protected internal virtual XmlWriterSettings CreateXmlWriterSettings() {
			return XmlBasedExporterUtils.Instance.CreateDefaultXmlWriterSettings();
		}
		string EncodeXmlChars(string value) {
			return XmlBasedExporterUtils.Instance.EncodeXmlChars(value);
		}
		string EncodeXmlCharsNoCrLf(string value) {
			return XmlBasedExporterUtils.Instance.EncodeXmlCharsNoCrLf(value);
		}
		#region String value
		protected internal virtual void WriteShStringValue(string tag, string value) {
			WriteStringValue(tag, value);
		}
		protected internal virtual void WriteStringValue(string tag, string value) {
			WriteStringAttr(null, tag, null, value);
		}
		protected internal virtual void WriteStringValue(string attr, string value, bool shouldExport) {
			if (shouldExport)
				WriteStringValue(attr, value);
		}
		protected internal virtual void WriteShStringAttr(string attr, string value) {
			writer.WriteAttributeString(attr, XlsxPackageBuilder.SpreadsheetNamespaceConst, value);
		}
		protected internal virtual void WriteStringAttr(string prefix, string attr, string ns, string value) {
			writer.WriteAttributeString(prefix, attr, ns, value);
		}
		protected internal virtual void WriteShString(string tag, string text) {
			WriteShString(tag, text, false);
		}
		protected internal virtual void WriteShString(string tag, string text, bool writeXmlSpaceAttr) {
			WriteString(tag, text, XlsxPackageBuilder.SpreadsheetNamespaceConst, writeXmlSpaceAttr);
		}
		protected internal virtual void WriteShString(string text) {
			writer.WriteString(text);
		}
		protected internal virtual void WriteString(string tag, string ns, string text) {
			WriteString(tag, text, ns, false);
		}
		protected internal virtual void WriteString(string tag, string text, string ns, bool writeXmlSpaceAttr) {
			WriteString(String.Empty, tag, text, ns, writeXmlSpaceAttr);
		}
		protected internal virtual void WriteString(string prefix, string tag, string text, string ns, bool writeXmlSpaceAttr) {
			if (!String.IsNullOrEmpty(prefix))
				WriteStartElement(prefix, tag, ns);
			else
				WriteStartElement(tag, ns);
			try {
				if (!string.IsNullOrEmpty(text)) {
					if (writeXmlSpaceAttr && IsNeedWriteXmlSpaceAttr(text))
						WriteStringAttr("xml", "space", null, "preserve");
					writer.WriteString(text);
				}
			}
			finally {
				WriteEndElement();
			}
		}
		bool IsNeedWriteXmlSpaceAttr(string text) {
			if (String.IsNullOrEmpty(text))
				return false;
			return IsSpace(text[0]) || IsSpace(text[text.Length - 1]) || text.Contains("  ");
		}
		bool IsSpace(char ch) {
			const char EmSpace = '\u2003';
			const char EnSpace = '\u2002';
			const char QmSpace = '\u2005';
			const char CR = '\u000A';
			const char LF = '\u000D';
			return ch == ' ' || ch == EmSpace || ch == EnSpace || ch == QmSpace || ch == CR || ch == LF;
		}
		#endregion
		#region Bool value
		protected internal string ConvertBoolToString(bool value) {
			return value ? "1" : "0";
		}
		protected internal void WriteShBoolValue(string tag, bool value) {
			WriteShStringValue(tag, ConvertBoolToString(value));
		}
		protected internal void WriteBoolValue(string tag, bool value) {
			WriteStringAttr(null, tag, null, ConvertBoolToString(value));
		}
		protected internal void WriteShBoolAttr(string attr, bool value) {
			WriteShStringAttr(attr, ConvertBoolToString(value));
		}
		protected internal void WriteBoolValue(string attr, bool value, bool defaultValue) {
			if (value != defaultValue)
				WriteBoolValue(attr, value);
		}
		protected internal void WriteOptionalBoolValue(string attr, bool value, bool shouldExport) {
			if (shouldExport)
				WriteBoolValue(attr, value);
		}
		#endregion
		#region Int value
		protected internal void WriteShIntValue(string tag, int value) {
			WriteShStringValue(tag, value.ToString());
		}
		protected internal void WriteIntValue(string tag, int value) {
			WriteStringAttr(null, tag, null, value.ToString());
		}
		protected internal void WriteShIntAttr(string attr, int value) {
			WriteShStringAttr(attr, value.ToString());
		}
		protected internal void WriteIntValue(string attr, int value, bool shouldExport) {
			if (shouldExport)
				WriteIntValue(attr, value);
		}
		protected internal void WriteIntValue(string attr, int value, int defaultValue) {
			WriteIntValue(attr, value, value != defaultValue);
		}
		#endregion
		#region Double value
		protected internal void WriteDoubleValue(string tag, double value) {
			WriteStringAttr(null, tag, null, value.ToString(CultureInfo.InvariantCulture));
		}
		#endregion
		void WriteColor(XlColor color, string shStartElement, string ns) {
			if (color.IsEmpty)
				return;
			WriteStartElement(shStartElement, ns);
			try {
				if(color.ColorType == XlColorType.Auto)
					WriteBoolValue("auto", true);
				else if(color.ColorType == XlColorType.Rgb)
					WriteStringValue("rgb", ConvertARGBColorToString(color.Rgb));
				else if(color.ColorType == XlColorType.Theme) {
					WriteIntValue("theme", (int)color.ThemeColor);
					if(color.Tint != 0.0)
						WriteStringValue("tint", color.Tint.ToString(CultureInfo.InvariantCulture.NumberFormat));
				}
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual string ConvertARGBColorToString(Color value) {
			return String.Format("{0:X2}{1:X2}{2:X2}{3:X2}", (int)value.A, (int)value.R, (int)value.G, (int)value.B);
		}
		void WriteColor(XlColor info, string shStartElement) {
			WriteColor(info, shStartElement, XlsxPackageBuilder.SpreadsheetNamespaceConst);
		}
		#region Start\end elements
		protected internal void WriteShStartElement(string tag) {
			WriteStartElement(tag, XlsxPackageBuilder.SpreadsheetNamespaceConst);
		}
		protected internal void WriteStartElement(string tag, string ns) {
			writer.WriteStartElement(tag, ns);
		}
		protected internal void WriteStartElement(string prefix, string tag, string ns) {
			writer.WriteStartElement(prefix, tag, ns);
		}
		protected internal void WriteShEndElement() {
			WriteEndElement();
		}
		protected internal void WriteEndElement() {
			writer.WriteEndElement();
		}
		#endregion
		#region Unit conversion
		float DpiX { get { return DevExpress.XtraPrinting.GraphicsDpi.Pixel; } }
		float DpiY { get { return DevExpress.XtraPrinting.GraphicsDpi.Pixel; } }
		static float MulDivF(float value, float mul, float div) {
			return (float)((float)(mul * value) / div);
		}
		internal static float PixelsToPointsF(float val, float dpi) {
			return MulDivF(val, 72, dpi);
		}
		#endregion
		#region IXlFormulaEngine methods
		IXlFormulaParameter IXlFormulaEngine.Param(XlVariantValue value) {
			return new XlFormulaParameter(value);
		}
		IXlFormulaParameter IXlFormulaEngine.Subtotal(XlCellRange range, XlSummary summary, bool ignoreHidden) {
			return XlSubtotalFunction.Create(range, summary, ignoreHidden);
		}
		IXlFormulaParameter IXlFormulaEngine.Subtotal(IList<XlCellRange> ranges, XlSummary summary, bool ignoreHidden) {
			return XlSubtotalFunction.Create(ranges, summary, ignoreHidden);
		}
		IXlFormulaParameter IXlFormulaEngine.VLookup(XlVariantValue lookupValue, XlCellRange table, int columnIndex, bool rangeLookup) {
			return XlVLookupFunction.Create(lookupValue, table, columnIndex, rangeLookup);
		}
		IXlFormulaParameter IXlFormulaEngine.VLookup(IXlFormulaParameter lookupValue, XlCellRange table, int columnIndex, bool rangeLookup) {
			return XlVLookupFunction.Create(lookupValue, table, columnIndex, rangeLookup);
		}
		IXlFormulaParameter IXlFormulaEngine.Text(XlVariantValue value, string netFormatString, bool isDateTimeFormatString) {
			return XlTextFunction.Create(value, netFormatString, isDateTimeFormatString);
		}
		IXlFormulaParameter IXlFormulaEngine.Text(IXlFormulaParameter formula, string netFormatString, bool isDateTimeFormatString) {
			return XlTextFunction.Create(formula, netFormatString, isDateTimeFormatString);
		}
		IXlFormulaParameter IXlFormulaEngine.Concatenate(params IXlFormulaParameter[] parameters) {
			return new XlConcatenateFunction(parameters);
		}
		#endregion
	}
}
