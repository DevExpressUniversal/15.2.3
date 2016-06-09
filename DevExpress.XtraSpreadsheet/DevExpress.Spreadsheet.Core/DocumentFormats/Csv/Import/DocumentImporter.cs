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
using System.ComponentModel;
using System.Globalization;
using System.IO;
using DevExpress.Office;
using DevExpress.Office.Import;
using DevExpress.Office.Internal;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office.Design;
using System.Text;
using DevExpress.Utils;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraSpreadsheet.Import {
	#region CsvImportCellIndexStrategy
	public enum CsvImportCellIndexStrategy {
		Stop = 0,
		Throw
	}
	#endregion
	#region CsvDocumentImporterOptions
	public class CsvDocumentImporterOptions : TextDocumentImporterOptionsBase {
		public CsvDocumentImporterOptions()
			: base() {
		}
		#region Properties
		protected override char DefaultDelimiter { get { return '\0'; } }
		protected internal override char FallbackDelimiter { get { return ','; } }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override DocumentFormat Format { get { return DocumentFormat.Csv; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentImporterOptionsDelimiter")]
#endif
		public override char Delimiter { get { return base.Delimiter; } set { base.Delimiter = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentImporterOptionsTextQualifier"),
#endif
 DefaultValue(TextDocumentImporterOptionsBase.DefaultTextQualifier)]
		public override char TextQualifier { get { return base.TextQualifier; } set { base.TextQualifier = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentImporterOptionsValueTypeDetectMode"),
#endif
 DefaultValue(TextDocumentImporterOptionsBase.DefaultValueTypeDetectMode)]
		public override CsvValueTypeDetectMode ValueTypeDetectMode { get { return base.ValueTypeDetectMode; } set { base.ValueTypeDetectMode = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentImporterOptionsCulture"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override CultureInfo Culture { get { return base.Culture; } set { base.Culture = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentImporterOptionsCellReferenceStyle"),
#endif
 DefaultValue(TextDocumentImporterOptionsBase.DefaultCellReferenceStyle)]
		public override CellReferenceStyle CellReferenceStyle { get { return base.CellReferenceStyle; } set { base.CellReferenceStyle = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentImporterOptionsNewlineType"),
#endif
 DefaultValue(TextDocumentImporterOptionsBase.DefaultNewlineType)]
		public override NewlineType NewlineType { get { return base.NewlineType; } set { base.NewlineType = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentImporterOptionsStartRowToImport"),
#endif
 DefaultValue(TextDocumentImporterOptionsBase.DefaultSkipHeaderLines)]
		public override int StartRowToImport { get { return base.StartRowToImport; } set { base.StartRowToImport = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentImporterOptionsCellIndexOutOfRangeStrategy"),
#endif
 DefaultValue(TextDocumentImporterOptionsBase.DefaultCellIndexOutOfRangeStrategy)]
		public override CsvImportCellIndexStrategy CellIndexOutOfRangeStrategy { get { return base.CellIndexOutOfRangeStrategy; } set { base.CellIndexOutOfRangeStrategy = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentImporterOptionsMaxRowCountToImport"),
#endif
 DefaultValue(TextDocumentImporterOptionsBase.DefaultMaxRows)]
		public override int MaxRowCountToImport { get { return base.MaxRowCountToImport; } set { base.MaxRowCountToImport = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentImporterOptionsTrimBlanks"),
#endif
 DefaultValue(TextDocumentImporterOptionsBase.DefaultTrimBlanks)]
		public override bool TrimBlanks { get { return base.TrimBlanks; } set { base.TrimBlanks = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentImporterOptionsClearWorksheetBeforeImport"),
#endif
 DefaultValue(TextDocumentImporterOptionsBase.DefaultClearSheetBeforeImport)]
		public override bool ClearWorksheetBeforeImport { get { return base.ClearWorksheetBeforeImport; } set { base.ClearWorksheetBeforeImport = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentImporterOptionsWorksheet"),
#endif
 DefaultValue("")]
		public override string Worksheet { get { return base.Worksheet; } set { base.Worksheet = value; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentImporterOptionsStartCellToInsert")]
#endif
		public override string StartCellToInsert { get { return base.StartCellToInsert; } set { base.StartCellToInsert = value; } }
		bool ShouldSerializeStartCellToInsert() {
			return StartCellToInsert != DefaultStartCellToInsert;
		}
		void ResetStartCellToInsert() {
			StartCellToInsert = DefaultStartCellToInsert;
		}
		bool ShouldSerializeCulture() {
			return Culture != CultureInfo.InvariantCulture;
		}
		void ResetCulture() {
			Culture = CultureInfo.InvariantCulture;
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentImporterOptionsEncoding")]
#endif
#if !DXPORTABLE
		[TypeConverter(typeof(EncodingConverter))]
#endif
		public override Encoding Encoding { get { return base.Encoding; } set { base.Encoding = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentImporterOptionsAutoDetectEncoding"),
#endif
 NotifyParentProperty(true), DefaultValue(true)]
		public override bool AutoDetectEncoding { get { return base.AutoDetectEncoding; } set { base.AutoDetectEncoding = value; } }
		#endregion
		protected override TextDocumentImporterOptionsBase CreateInstance() {
			return new CsvDocumentImporterOptions();
		}
		public new CsvDocumentImporterOptions Clone() {
			return base.Clone() as CsvDocumentImporterOptions;
		}
	}
#endregion
	public enum CsvValueTypeDetectMode {
		Default,
		None,
		Simple,
		Advanced
	}
#region TextDocumentImporterOptionsBase
	public abstract class TextDocumentImporterOptionsBase : DocumentImporterOptions, ISupportsCopyFrom<TextDocumentImporterOptionsBase>, ICloneable<TextDocumentImporterOptionsBase> {
#region Fields
		const char csvDelimiter = ',';
		const char alternativeValueSeparator = ';';
		public const char DefaultTextQualifier = '"';
		public const char AlternativeValueSeparator = alternativeValueSeparator;
		public const bool DefaultClearSheetBeforeImport = true;
		public const bool DefaultTrimBlanks = false;
		public const bool DefaultDetectCellValueType = true;
		public const int DefaultMaxRows = Int32.MaxValue;
		public const int DefaultSkipHeaderLines = 0;
		public const CsvImportCellIndexStrategy DefaultCellIndexOutOfRangeStrategy = CsvImportCellIndexStrategy.Stop;
		public const NewlineType DefaultNewlineType = NewlineType.AnyCrLf;
		public const CellReferenceStyle DefaultCellReferenceStyle = CellReferenceStyle.WorkbookDefined;
		public const CsvValueTypeDetectMode DefaultValueTypeDetectMode = CsvValueTypeDetectMode.Default;
		public static readonly string DefaultStartCellToInsert = "A1"; 
		CultureInfo culture;
		char delimiterUserDefined;
		char delimiterDetected;
		bool autoDetectDelimiter = true;
		char textQualifier;
		Encoding encoding;
		bool autoDetectEncoding;
		CellReferenceStyle cellReferenceStyle;
		bool trimBlanks;
		bool clearWorksheetBeforeImport;
		NewlineType newlineType;
		CsvImportCellIndexStrategy cellIndexOutOfRangeStrategy;
		int startRowToImport;
		int maxRowCountToImport;
		string worksheet;
		string startCellToInsert; 
		CsvValueTypeDetectMode valueTypeDetectMode;
#endregion
		protected TextDocumentImporterOptionsBase()
			: base() {
		}
#region Properties
#region Delimiter
		public virtual char Delimiter {
			get { return delimiterUserDefined; }
			set {
				if (delimiterUserDefined == value)
					return;
				autoDetectDelimiter = false;
				SetUserDefinedDelimiter(value);
			}
		}
		protected bool ShouldSerializeDelimiter() {
			return Delimiter != DefaultDelimiter;
		}
		protected void ResetDelimiter() {
			this.Delimiter = DefaultDelimiter;
			this.autoDetectDelimiter = true; 
		}
		protected internal bool AutoDetectDelimiter { get { return autoDetectDelimiter; } }
		protected internal char ActualDelimiter {
			get {
				if (AutoDetectDelimiter)
					return delimiterDetected;
				else
					return delimiterUserDefined; 
			}
		}
		void SetUserDefinedDelimiter(char newDelimiter) {
			char oldValue = delimiterUserDefined;
			delimiterUserDefined = newDelimiter;
			OnChanged("Delimiter", oldValue, newDelimiter);
		}
		protected internal void SetDetectedDelimiter(char detected) {
			if (!autoDetectDelimiter)
				throw new InvalidOperationException();
			delimiterDetected = detected;
		}
#endregion
#region TextQualifier
		public virtual char TextQualifier {
			get { return textQualifier; }
			set {
				if (textQualifier == value)
					return;
				char oldValue = textQualifier;
				textQualifier = value;
				OnChanged("TextQualifier", oldValue, value);
			}
		}
#endregion
#region ValueTypeDetectMode
		public virtual CsvValueTypeDetectMode ValueTypeDetectMode {
			get { return valueTypeDetectMode; }
			set {
				if (valueTypeDetectMode == value)
					return;
				CsvValueTypeDetectMode oldValue = valueTypeDetectMode;
				valueTypeDetectMode = value;
				OnChanged("ValueTypeDetectMode", oldValue, value);
			}
		}
#endregion
#region Culture
		public virtual CultureInfo Culture {
			get { return culture; }
			set {
				if (culture == value)
					return;
				CultureInfo oldValue = culture;
				culture = value;
				OnChanged("Culture", oldValue, value);
			}
		}
#endregion
#region CellReferenceStyle
		public virtual CellReferenceStyle CellReferenceStyle {
			get { return cellReferenceStyle; }
			set {
				if (cellReferenceStyle == value)
					return;
				CellReferenceStyle oldValue = cellReferenceStyle;
				cellReferenceStyle = value;
				OnChanged("CellReferenceStyle", oldValue, value);
			}
		}
#endregion
#region NewlineType
		public virtual NewlineType NewlineType {
			get { return newlineType; }
			set {
				if (newlineType == value)
					return;
				NewlineType oldValue = newlineType;
				newlineType = value;
				OnChanged("NewlineType", oldValue, value);
			}
		}
#endregion
#region StartRowToImport
		public virtual int StartRowToImport {
			get { return startRowToImport; }
			set {
				if (startRowToImport == value)
					return;
				int oldValue = startRowToImport;
				startRowToImport = value;
				OnChanged("StartRowToImport", oldValue, value);
			}
		}
#endregion
#region CellIndexOutOfRangeStrategy
		public virtual CsvImportCellIndexStrategy CellIndexOutOfRangeStrategy {
			get { return cellIndexOutOfRangeStrategy; }
			set {
				if (cellIndexOutOfRangeStrategy == value)
					return;
				CsvImportCellIndexStrategy oldValue = cellIndexOutOfRangeStrategy;
				cellIndexOutOfRangeStrategy = value;
				OnChanged("CellIndexOutOfRangeStrategy", oldValue, value);
			}
		}
#endregion
#region MaxRowCountToImport
		public virtual int MaxRowCountToImport {
			get { return maxRowCountToImport; }
			set {
				if (maxRowCountToImport == value)
					return;
				int oldValue = maxRowCountToImport;
				maxRowCountToImport = value;
				OnChanged("MaxRowCountToImport", oldValue, value);
			}
		}
#endregion
#region TrimBlanks
		public virtual bool TrimBlanks {
			get { return trimBlanks; }
			set {
				if (trimBlanks == value)
					return;
				trimBlanks = value;
				OnChanged("TrimBlanks", !value, value);
			}
		}
#endregion
#region ClearWorksheetBeforeImport
		public virtual bool ClearWorksheetBeforeImport {
			get { return clearWorksheetBeforeImport; }
			set {
				if (clearWorksheetBeforeImport == value)
					return;
				clearWorksheetBeforeImport = value;
				OnChanged("ClearWorksheetBeforeImport", !value, value);
			}
		}
#endregion
#region Worksheet
		public virtual string Worksheet {
			get { return worksheet; }
			set {
				if (worksheet == value)
					return;
				string oldValue = worksheet;
				worksheet = value;
				OnChanged("Worksheet", oldValue, value);
			}
		}
#endregion
#region StartCellToInsert
		public virtual string StartCellToInsert {
			get { return startCellToInsert; }
			set {
				if (startCellToInsert == value)
					return;
				string oldValue = this.startCellToInsert;
				this.startCellToInsert = value;
				OnChanged("StartCellToInsert", oldValue, value);
			}
		}
		bool ShouldSerializeStartCellToInsert() {
			return StartCellToInsert != DefaultStartCellToInsert;
		}
		void ResetStartCellToInsert() {
			StartCellToInsert = DefaultStartCellToInsert;
		}
#endregion
#region Encoding
		public virtual Encoding Encoding {
			get { return encoding; }
			set {
				Encoding oldValue = encoding;
				encoding = value;
				OnChanged("Encoding", oldValue, value);
				ActualEncoding = value;
			}
		}
		protected internal virtual bool ShouldSerializeEncoding() {
			return !Object.Equals(DXEncoding.Default, Encoding);
		}
		protected internal virtual void ResetEncoding() {
			Encoding = DXEncoding.Default;
		}
#endregion
#region AutoDetectEncoding
		public virtual bool AutoDetectEncoding {
			get { return autoDetectEncoding; }
			set {
				if (value == autoDetectEncoding)
					return;
				autoDetectEncoding = value;
				OnChanged("AutoDetectEncoding", !value, value);
			}
		}
#endregion
		protected abstract char DefaultDelimiter { get; }
		protected internal abstract char FallbackDelimiter { get; }
#endregion
		protected internal override void ResetCore() {
			base.ResetCore();
			ResetEncoding();
			AutoDetectEncoding = true;
			ValueTypeDetectMode = DefaultValueTypeDetectMode;
			CellReferenceStyle = DefaultCellReferenceStyle;
			StartCellToInsert = DefaultStartCellToInsert;
			TrimBlanks = DefaultTrimBlanks;
			ClearWorksheetBeforeImport = DefaultClearSheetBeforeImport;
			TextQualifier = DefaultTextQualifier;
			NewlineType = DefaultNewlineType;
			CellIndexOutOfRangeStrategy = DefaultCellIndexOutOfRangeStrategy;
			StartRowToImport = DefaultSkipHeaderLines;
			MaxRowCountToImport = DefaultMaxRows;
			Worksheet = string.Empty;
			culture = CultureInfo.InvariantCulture;
			ResetDelimiter();
		}
		public void CopyFrom(TextDocumentImporterOptionsBase value) {
			if (object.ReferenceEquals(this, value))
				return;
			base.CopyFrom(value);
			StartCellToInsert = value.StartCellToInsert;
			Culture = value.Culture;
			Delimiter = value.Delimiter; 
			this.delimiterDetected = value.delimiterDetected;
			this.autoDetectDelimiter = value.autoDetectDelimiter;
			Encoding = value.Encoding;
			AutoDetectEncoding = value.AutoDetectEncoding;
			ValueTypeDetectMode = value.ValueTypeDetectMode;
			CellReferenceStyle = value.CellReferenceStyle;
			TrimBlanks = value.TrimBlanks;
			ClearWorksheetBeforeImport = value.ClearWorksheetBeforeImport;
			TextQualifier = value.TextQualifier;
			NewlineType = value.NewlineType;
			CellIndexOutOfRangeStrategy = value.CellIndexOutOfRangeStrategy;
			StartRowToImport = value.StartRowToImport;
			SourceUri = value.SourceUri; 
			MaxRowCountToImport = value.MaxRowCountToImport;
			Worksheet = value.Worksheet;
		}
		public override void CopyFrom(IImporterOptions value) {
			base.CopyFrom(value);
			this.CopyFrom(value as TextDocumentImporterOptionsBase);
		}
		public virtual TextDocumentImporterOptionsBase Clone() {
			TextDocumentImporterOptionsBase result = CreateInstance();
			result.CopyFrom(this);
			return result;
		}
		protected abstract TextDocumentImporterOptionsBase CreateInstance();
	}
#endregion
#region CsvDocumentImporter
	public class CsvDocumentImporter : IImporter<DocumentFormat, bool> {
#region IDocumentImporter Members
		public virtual FileDialogFilter Filter { get { return CsvDocumentExporter.filter; } }
		public virtual DocumentFormat Format { get { return DocumentFormat.Csv; } }
		public virtual IImporterOptions SetupLoading() {
			return new CsvDocumentImporterOptions();
		}
		public virtual bool LoadDocument(IDocumentModel documentModel, Stream stream, IImporterOptions options) {
			DocumentModel model = (DocumentModel)documentModel;
			model.InternalAPI.LoadDocumentCsvContent(stream, (TextDocumentImporterOptionsBase)options);
			return true;
		}
#endregion
	}
#endregion
}
