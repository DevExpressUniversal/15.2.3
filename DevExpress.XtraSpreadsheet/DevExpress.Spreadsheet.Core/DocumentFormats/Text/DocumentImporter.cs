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
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraSpreadsheet.Import {
	#region TxtDocumentImporterOptions
	public class TxtDocumentImporterOptions : TextDocumentImporterOptionsBase {
		public TxtDocumentImporterOptions()
			: base() {
		}
		#region Properties
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override DocumentFormat Format { get { return DocumentFormat.Text; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentImporterOptionsDelimiter")]
#endif
		public override char Delimiter { get { return base.Delimiter; } set { base.Delimiter = value; } }
		protected override char DefaultDelimiter { get { return '\0'; } }
		protected internal override char FallbackDelimiter { get { return '\t'; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentImporterOptionsTextQualifier"),
#endif
 DefaultValue(TextDocumentImporterOptionsBase.DefaultTextQualifier)]
		public override char TextQualifier { get { return base.TextQualifier; } set { base.TextQualifier = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentImporterOptionsValueTypeDetectMode"),
#endif
 DefaultValue(TextDocumentImporterOptionsBase.DefaultValueTypeDetectMode)]
		public override CsvValueTypeDetectMode ValueTypeDetectMode { get { return base.ValueTypeDetectMode; } set { base.ValueTypeDetectMode = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentImporterOptionsCulture"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override CultureInfo Culture { get { return base.Culture; } set { base.Culture = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentImporterOptionsCellReferenceStyle"),
#endif
 DefaultValue(TextDocumentImporterOptionsBase.DefaultCellReferenceStyle)]
		public override CellReferenceStyle CellReferenceStyle { get { return base.CellReferenceStyle; } set { base.CellReferenceStyle = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentImporterOptionsNewlineType"),
#endif
 DefaultValue(TextDocumentImporterOptionsBase.DefaultNewlineType)]
		public override NewlineType NewlineType { get { return base.NewlineType; } set { base.NewlineType = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentImporterOptionsStartRowToImport"),
#endif
 DefaultValue(TextDocumentImporterOptionsBase.DefaultSkipHeaderLines)]
		public override int StartRowToImport { get { return base.StartRowToImport; } set { base.StartRowToImport = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentImporterOptionsCellIndexOutOfRangeStrategy"),
#endif
 DefaultValue(TextDocumentImporterOptionsBase.DefaultCellIndexOutOfRangeStrategy)]
		public override CsvImportCellIndexStrategy CellIndexOutOfRangeStrategy { get { return base.CellIndexOutOfRangeStrategy; } set { base.CellIndexOutOfRangeStrategy = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentImporterOptionsMaxRowCountToImport"),
#endif
 DefaultValue(TextDocumentImporterOptionsBase.DefaultMaxRows)]
		public override int MaxRowCountToImport { get { return base.MaxRowCountToImport; } set { base.MaxRowCountToImport = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentImporterOptionsTrimBlanks"),
#endif
 DefaultValue(TextDocumentImporterOptionsBase.DefaultTrimBlanks)]
		public override bool TrimBlanks { get { return base.TrimBlanks; } set { base.TrimBlanks = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentImporterOptionsClearWorksheetBeforeImport"),
#endif
 DefaultValue(TextDocumentImporterOptionsBase.DefaultClearSheetBeforeImport)]
		public override bool ClearWorksheetBeforeImport { get { return base.ClearWorksheetBeforeImport; } set { base.ClearWorksheetBeforeImport = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentImporterOptionsWorksheet"),
#endif
 DefaultValue("")]
		public override string Worksheet { get { return base.Worksheet; } set { base.Worksheet = value; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentImporterOptionsStartCellToInsert")]
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
#if !DXPORTABLE
		[TypeConverter(typeof(EncodingConverter))]
#endif
		public override Encoding Encoding { get { return base.Encoding; } set { base.Encoding = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentImporterOptionsAutoDetectEncoding"),
#endif
 NotifyParentProperty(true), DefaultValue(true)]
		public override bool AutoDetectEncoding { get { return base.AutoDetectEncoding; } set { base.AutoDetectEncoding = value; } }
		#endregion
		protected override TextDocumentImporterOptionsBase CreateInstance() {
			return new TxtDocumentImporterOptions();
		}
		public new TxtDocumentImporterOptions Clone() {
			return base.Clone() as TxtDocumentImporterOptions;
		}
	}
	#endregion
	#region TxtDocumentImporter
	public class TxtDocumentImporter : CsvDocumentImporter {
		#region IDocumentImporter Members
		public override FileDialogFilter Filter { get { return TxtDocumentExporter.txtfilter; } }
		public override DocumentFormat Format { get { return DocumentFormat.Text; } }
		public override IImporterOptions SetupLoading() {
			return new TxtDocumentImporterOptions();
		}
		#endregion
	}
	#endregion
}
