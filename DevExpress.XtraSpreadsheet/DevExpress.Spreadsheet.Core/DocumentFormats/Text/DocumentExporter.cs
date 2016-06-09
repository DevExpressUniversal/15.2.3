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

using System.ComponentModel;
using System.Globalization;
using System.IO;
using DevExpress.Office;
using DevExpress.Office.Export;
using DevExpress.Office.Internal;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Import;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using System;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraSpreadsheet.Export {
	public class TxtDocumentExporterOptions : TextDocumentExporterOptionsBase {
		public const char TextDefaultValueSeparator = '\t';
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override DocumentFormat Format { get { return DocumentFormat.Text; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentExporterOptionsValueSeparator")]
#endif
		public override char ValueSeparator { get { return base.ValueSeparator; } set { base.ValueSeparator = value; } }
		bool ShouldSerializeValueSeparator() { return ValueSeparator != TxtDocumentExporterOptions.TextDefaultValueSeparator; }
		void ResetValueSeparator() { this.ValueSeparator = TxtDocumentExporterOptions.TextDefaultValueSeparator; }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentExporterOptionsTextQualifier"),
#endif
 DefaultValue(TxtDocumentExporterOptions.DefaultTextQualifier)]
		public override char TextQualifier { get { return base.TextQualifier; } set { base.TextQualifier = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentExporterOptionsCulture"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override CultureInfo Culture { get { return base.Culture; } set { base.Culture = value; } }
		[ DefaultValue(TxtDocumentExporterOptions.DefaultReferenceStyle)]
		public override CellReferenceStyle CellReferenceStyle { get { return base.CellReferenceStyle; } set { base.CellReferenceStyle = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentExporterOptionsNewlineType"),
#endif
 DefaultValue(TxtDocumentExporterOptions.DefaultNewlineType)]
		public override NewlineType NewlineType { get { return base.NewlineType; } set { base.NewlineType = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentExporterOptionsWritePreamble"),
#endif
 DefaultValue(TxtDocumentExporterOptions.DefaultWritePreamble)]
		public override bool WritePreamble { get { return base.WritePreamble; } set { base.WritePreamble = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentExporterOptionsFormulaExportMode"),
#endif
 DefaultValue(TxtDocumentExporterOptions.DefaultFormulaExportMode)]
		public override FormulaExportMode FormulaExportMode { get { return base.FormulaExportMode; } set { base.FormulaExportMode = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentExporterOptionsUseCellNumberFormat"),
#endif
 DefaultValue(TxtDocumentExporterOptions.DefaultUseCellNumberFormat)]
		public override bool UseCellNumberFormat { get { return base.UseCellNumberFormat; } set { base.UseCellNumberFormat = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentExporterOptionsNewlineAfterLastRow"),
#endif
 DefaultValue(TxtDocumentExporterOptions.DefaultNewlineAfterLastRow)]
		public override bool NewlineAfterLastRow { get { return base.NewlineAfterLastRow; } set { base.NewlineAfterLastRow = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentExporterOptionsDiscardTrailingEmptyCells"),
#endif
 DefaultValue(TxtDocumentExporterOptions.DefaultDiscardTrailingEmptyCells)]
		public override bool DiscardTrailingEmptyCells { get { return base.DiscardTrailingEmptyCells; } set { base.DiscardTrailingEmptyCells = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentExporterOptionsShiftTopLeft"),
#endif
 DefaultValue(TxtDocumentExporterOptions.DefaultShiftTopLeft)]
		public override bool ShiftTopLeft { get { return base.ShiftTopLeft; } set { base.ShiftTopLeft = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentExporterOptionsWorksheet"),
#endif
 DefaultValue("")]
		public override string Worksheet { get { return base.Worksheet; } set { base.Worksheet = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("TxtDocumentExporterOptionsRange"),
#endif
 DefaultValue("")]
		public override string Range { get { return base.Range; } set { base.Range = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),  DefaultValue(false)]
		protected internal override bool IsNullTerminated { get { return base.IsNullTerminated; } set { base.IsNullTerminated = value; } }
		bool ShouldSerializeCulture() {
			return Culture != CultureInfo.InvariantCulture;
		}
		void ResetCulture() {
			Culture = CultureInfo.InvariantCulture;
		}
		protected override TextDocumentExporterOptionsBase CreateInstance() {
			return new TxtDocumentExporterOptions();
		}
		protected internal override void ResetCore() {
			base.ResetCore();
			ResetValueSeparator(); 
		}
	}
	#region TxtDocumentExporter
	public class TxtDocumentExporter : CsvDocumentExporter {
		internal static readonly FileDialogFilter txtfilter =
			new FileDialogFilter(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FileFilterDescription_TextFiles), new string[] { "txt" });
		#region IDocumentExporter Members
		public override FileDialogFilter Filter { get { return txtfilter; } }
		public override DocumentFormat Format { get { return DocumentFormat.Text; } }
		public override IExporterOptions SetupSaving() {
			return new TxtDocumentExporterOptions();
		}
		#endregion
	}
	#endregion
}
