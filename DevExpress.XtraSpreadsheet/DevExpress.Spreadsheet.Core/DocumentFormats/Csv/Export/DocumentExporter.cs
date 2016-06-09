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
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Import;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Compatibility.System.ComponentModel;
using System;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Export {
	#region CsvFormulaExportMode
	public enum FormulaExportMode {
		CalculatedValue, 
		Formula		  
	}
	#endregion
	#region CsvValueExportMode
	#endregion
	public class CsvDocumentExporterOptions : TextDocumentExporterOptionsBase {
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override DocumentFormat Format { get { return DocumentFormat.Csv; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentExporterOptionsValueSeparator")]
#endif
		public override char ValueSeparator { get { return base.ValueSeparator; } set { base.ValueSeparator = value; } }
		bool ShouldSerializeValueSeparator() { return ValueSeparator != TextDocumentExporterOptionsBase.DefaultValueSeparator; }
		void ResetValueSeparator() { this.ValueSeparator = TextDocumentExporterOptionsBase.DefaultValueSeparator; }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentExporterOptionsTextQualifier"),
#endif
 DefaultValue(CsvDocumentExporterOptions.DefaultTextQualifier)]
		public override char TextQualifier { get { return base.TextQualifier; } set { base.TextQualifier = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentExporterOptionsCulture"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override CultureInfo Culture { get { return base.Culture; } set { base.Culture = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentExporterOptionsCellReferenceStyle"),
#endif
 DefaultValue(CsvDocumentExporterOptions.DefaultReferenceStyle)]
		public override CellReferenceStyle CellReferenceStyle { get { return base.CellReferenceStyle; } set { base.CellReferenceStyle = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentExporterOptionsNewlineType"),
#endif
 DefaultValue(CsvDocumentExporterOptions.DefaultNewlineType)]
		public override NewlineType NewlineType { get { return base.NewlineType; } set { base.NewlineType = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentExporterOptionsWritePreamble"),
#endif
 DefaultValue(CsvDocumentExporterOptions.DefaultWritePreamble)]
		public override bool WritePreamble { get { return base.WritePreamble; } set { base.WritePreamble = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentExporterOptionsFormulaExportMode"),
#endif
 DefaultValue(CsvDocumentExporterOptions.DefaultFormulaExportMode)]
		public override FormulaExportMode FormulaExportMode { get { return base.FormulaExportMode; } set { base.FormulaExportMode = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentExporterOptionsUseCellNumberFormat"),
#endif
 DefaultValue(CsvDocumentExporterOptions.DefaultUseCellNumberFormat)]
		public override bool UseCellNumberFormat { get { return base.UseCellNumberFormat; } set { base.UseCellNumberFormat = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentExporterOptionsNewlineAfterLastRow"),
#endif
 DefaultValue(CsvDocumentExporterOptions.DefaultNewlineAfterLastRow)]
		public override bool NewlineAfterLastRow { get { return base.NewlineAfterLastRow; } set { base.NewlineAfterLastRow = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentExporterOptionsDiscardTrailingEmptyCells"),
#endif
 DefaultValue(CsvDocumentExporterOptions.DefaultDiscardTrailingEmptyCells)]
		public override bool DiscardTrailingEmptyCells { get { return base.DiscardTrailingEmptyCells; } set { base.DiscardTrailingEmptyCells = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentExporterOptionsShiftTopLeft"),
#endif
 DefaultValue(CsvDocumentExporterOptions.DefaultShiftTopLeft)]
		public override bool ShiftTopLeft { get { return base.ShiftTopLeft; } set { base.ShiftTopLeft = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentExporterOptionsWorksheet"),
#endif
 DefaultValue("")]
		public override string Worksheet { get { return base.Worksheet; } set { base.Worksheet = value; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("CsvDocumentExporterOptionsRange"),
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
			return new CsvDocumentExporterOptions();
		}
	}
	#region TextDocumentExporterOptionsBase (abstract)
	public abstract class TextDocumentExporterOptionsBase : DocumentExporterOptions, ICloneable<TextDocumentExporterOptionsBase>, ISupportsCopyFrom<TextDocumentExporterOptionsBase> {
		#region Fields
		const char defaultValueSeparator = ',';
		const char alternativeValueSeparator = ';';
		const char defaultQuoteChar = '\"';
		char valueSeparator; 
		char textQualifier;
		NewlineType newlineType;
		bool writePreamble;
		CultureInfo culture;
		CellReferenceStyle referenceStyle;
		FormulaExportMode formulaExportMode;
		bool useCellNumberFormat;
		bool discardTrailingEmptyCells;
		bool newlineAfterLastRow;
		bool shiftTopLeft;
		string worksheet;
		string range;
		bool terminatingNullByte;
		public const char DefaultValueSeparator = defaultValueSeparator;
		public const char DefaultTextQualifier = defaultQuoteChar;
		public const CellReferenceStyle DefaultReferenceStyle = CellReferenceStyle.A1;
		public const FormulaExportMode DefaultFormulaExportMode = FormulaExportMode.CalculatedValue;
		public const NewlineType DefaultNewlineType = NewlineType.CrLf;
		public const bool DefaultWritePreamble = false;
		public const bool DefaultNewlineAfterLastRow = false;
		public const bool DefaultDiscardTrailingEmptyCells = false;
		public const bool DefaultShiftTopLeft = false;
		public const bool DefaultUseCellNumberFormat = true;
		#endregion
		protected TextDocumentExporterOptionsBase()
			: base() {
		}
		#region Properties
		#region ValueSeparator
		public virtual char ValueSeparator { 
			get { return valueSeparator; } 
			set {
				if (this.valueSeparator == value)
					return;
				char oldValue = this.valueSeparator;
				this.valueSeparator = value;
				OnChanged("ValueSeparator", oldValue, value);
			}
		}
		#endregion
		#region TextQualifier
		[ DefaultValue(TextDocumentExporterOptionsBase.DefaultTextQualifier)]
		public virtual char TextQualifier { 
			get { return textQualifier; } 
			set {
				if (this.textQualifier == value)
					return;
				char oldValue = this.textQualifier;
				this.textQualifier = value;
				OnChanged("TextQualifier", oldValue, value);
			} 
		}
		#endregion
		#region Culture
		[
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public virtual CultureInfo Culture { 
			get { return culture; } 
			set {
				if (this.culture == value)
					return;
				CultureInfo oldValue = this.culture;
				SetCulture(value);
				OnChanged("Culture", oldValue, value);
			}
		}
		#endregion
		#region CellReferenceStyle
		[ DefaultValue(TextDocumentExporterOptionsBase.DefaultReferenceStyle)]
		public virtual CellReferenceStyle CellReferenceStyle { 
			get { return referenceStyle; }
			set {
				if (this.referenceStyle == value)
					return;
				CellReferenceStyle oldValue = this.referenceStyle;
				this.referenceStyle = value;
				OnChanged("CellReferenceStyle", oldValue, value);
			} 
		}
		#endregion
		#region NewlineType
		[ DefaultValue(TextDocumentExporterOptionsBase.DefaultNewlineType)]
		public virtual NewlineType NewlineType {
			get { return newlineType; }
			set {
				if (this.newlineType == value)
					return;
				NewlineType oldValue = this.newlineType;
				this.newlineType = value;
				OnChanged("NewlineType", oldValue, value);
			} 
		}
		#endregion
		#region WritePreamble
		[ DefaultValue(TextDocumentExporterOptionsBase.DefaultWritePreamble)]
		public virtual bool WritePreamble { 
			get { return writePreamble; }
			set {
				if (this.writePreamble == value)
					return;
				bool oldValue = this.writePreamble;
				this.writePreamble = value;
				OnChanged("WritePreamble", oldValue, value);
			} 
		}
		#endregion
		#region FormulaExportMode
		[ DefaultValue(TextDocumentExporterOptionsBase.DefaultFormulaExportMode)]
		public virtual FormulaExportMode FormulaExportMode {
			get { return formulaExportMode; }
			set {
				if (this.formulaExportMode == value)
					return;
				FormulaExportMode oldValue = this.formulaExportMode;
				this.formulaExportMode = value;
				OnChanged("FormulaExportMode", oldValue, value);
			}
		}
		#endregion
		#region UseCellNumberFormat
		[ DefaultValue(TextDocumentExporterOptionsBase.DefaultUseCellNumberFormat)]
		public virtual bool UseCellNumberFormat { 
			get { return useCellNumberFormat; }
			set {
				if (this.useCellNumberFormat == value)
					return;
				bool oldValue = this.useCellNumberFormat;
				this.useCellNumberFormat = value;
				OnChanged("UseCellNumberFormat", oldValue, value);
			}
		}
		#endregion
		#region NewlineAfterLastRow
		[ DefaultValue(TextDocumentExporterOptionsBase.DefaultNewlineAfterLastRow)]
		public virtual bool NewlineAfterLastRow { 
			get { return newlineAfterLastRow; }
			set {
				if (this.newlineAfterLastRow == value)
					return;
				bool oldValue = this.newlineAfterLastRow;
				this.newlineAfterLastRow = value;
				OnChanged("NewlineAfterLastRow", oldValue, value);
			} 
		}
		#endregion
		#region DiscardTrailingEmptyCells
		[ DefaultValue(TextDocumentExporterOptionsBase.DefaultDiscardTrailingEmptyCells)]
		public virtual bool DiscardTrailingEmptyCells { 
			get { return discardTrailingEmptyCells; }
			set {
				if (this.discardTrailingEmptyCells == value)
					return;
				bool oldValue = this.discardTrailingEmptyCells;
				this.discardTrailingEmptyCells = value;
				OnChanged("DiscardTrailingEmptyCells", oldValue, value);
			} 
		}
		#endregion
		#region ShiftTopLeft
		[ DefaultValue(TextDocumentExporterOptionsBase.DefaultShiftTopLeft)]
		public virtual bool ShiftTopLeft {
			get { return shiftTopLeft; }
			set {
				if (this.shiftTopLeft == value)
					return;
				bool oldValue = this.shiftTopLeft;
				this.shiftTopLeft = value;
				OnChanged("ShiftTopLeft", oldValue, value);
			}
		}
		#endregion
		#region Worksheet
		[ DefaultValue("")]
		public virtual string Worksheet {
			get { return worksheet; }
			set {
				if (this.worksheet == value)
					return;
				string oldValue = this.worksheet;
				this.worksheet = value;
				OnChanged("Worksheet", oldValue, value);
			}
		}
		#endregion
		#region Range
		[ DefaultValue("")]
		public virtual string Range {
			get { return range; }
			set {
				if (this.range == value)
					return;
				string oldValue = this.range;
				this.range = value;
				OnChanged("Range", oldValue, value);
			} 
		}
		#endregion
		#region Encoding
#if !SL && !DXPORTABLE
		[
TypeConverter(typeof(DevExpress.Office.Design.EncodingConverter))]
#endif
		public Encoding Encoding { get { return ActualEncoding; } set { ActualEncoding = value; } }
		protected internal virtual bool ShouldSerializeEncoding() {
			return !Object.Equals(Encoding.UTF8, Encoding);
		}
		protected internal virtual void ResetEncoding() {
			Encoding = Encoding.UTF8;
		}
		#endregion
		#endregion
		protected void SetCulture(CultureInfo newValue) {
			culture = newValue;
			if (culture.NumberFormat.NumberDecimalSeparator.IndexOf(defaultValueSeparator) >= 0) {
				if ((ValueSeparator == defaultValueSeparator) || (ValueSeparator == 0))
					ValueSeparator = alternativeValueSeparator;
				else
					if (ValueSeparator == 0)
						ValueSeparator = defaultValueSeparator;
			}
		}
		[Browsable(false),EditorBrowsable(EditorBrowsableState.Never),  DefaultValue(false)]
		protected internal virtual bool IsNullTerminated {
			get { return terminatingNullByte; }
			set {
				if (this.terminatingNullByte == value)
					return;
				bool oldValue = this.terminatingNullByte;
				this.terminatingNullByte = value;
				OnChanged("IsNullTerminated", oldValue, value);
			}
		}
		protected internal override void ResetCore() {
			base.ResetCore();
			ValueSeparator = DefaultValueSeparator;
			NewlineType = DefaultNewlineType;
			WritePreamble = DefaultWritePreamble;
			CellReferenceStyle = DefaultReferenceStyle;
			FormulaExportMode = DefaultFormulaExportMode;
			UseCellNumberFormat = DefaultUseCellNumberFormat;
			NewlineAfterLastRow = DefaultNewlineAfterLastRow;
			DiscardTrailingEmptyCells = DefaultDiscardTrailingEmptyCells;
			ShiftTopLeft = DefaultShiftTopLeft;
			TextQualifier = DefaultTextQualifier;
			Worksheet = String.Empty;
			Range = String.Empty;
			Encoding = Encoding.UTF8;
			IsNullTerminated = false;
			SetCulture(CultureInfo.InvariantCulture);
		}
		#region ICloneable<TextDocumentExporterOptionsBase> implementation
		public TextDocumentExporterOptionsBase Clone() {
			TextDocumentExporterOptionsBase result = CreateInstance();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<TextDocumentExporterOptionsBase> implementation
		public void CopyFrom(TextDocumentExporterOptionsBase value) {
			if (object.ReferenceEquals(this, value))
				return;
			base.CopyFrom(value);
			Culture = value.Culture;
			ValueSeparator = value.ValueSeparator;
			NewlineType = value.NewlineType;
			WritePreamble = value.WritePreamble;
			CellReferenceStyle = value.CellReferenceStyle;
			FormulaExportMode = value.FormulaExportMode;
			UseCellNumberFormat = value.UseCellNumberFormat;
			NewlineAfterLastRow = value.NewlineAfterLastRow;
			DiscardTrailingEmptyCells = value.DiscardTrailingEmptyCells;
			ShiftTopLeft = value.ShiftTopLeft;
			TextQualifier = value.TextQualifier;
			TargetUri = value.TargetUri; 
			Worksheet = value.Worksheet;
			Range = value.Range;
			IsNullTerminated = value.IsNullTerminated;
		}
		public override void CopyFrom(IExporterOptions value) {
			base.CopyFrom(value);
			this.CopyFrom(value as TextDocumentExporterOptionsBase);
		}
		#endregion
		protected abstract TextDocumentExporterOptionsBase CreateInstance();
	}
	#endregion
	#region CsvDocumentExporter
	public class CsvDocumentExporter : IExporter<DocumentFormat, bool> {
		internal static readonly FileDialogFilter filter =
			new FileDialogFilter(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FileFilterDescription_CSVFiles), new string[] { "csv" });
		#region IDocumentExporter Members
		public virtual FileDialogFilter Filter { get { return filter; } }
		public virtual DocumentFormat Format { get { return DocumentFormat.Csv; } }
		public virtual IExporterOptions SetupSaving() {
			return new CsvDocumentExporterOptions();
		}
		public bool SaveDocument(IDocumentModel documentModel, Stream stream, IExporterOptions options) {
			DocumentModel model = (DocumentModel)documentModel;
			model.InternalAPI.SaveDocumentCsvContent(stream, (TextDocumentExporterOptionsBase)options);
			return true;
		}
		#endregion
	}
	#endregion
}
